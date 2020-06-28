Imports System.IO
Imports System.Security.Cryptography
Imports System.Threading

Public Class Tester
    Inherits ComponentModel.Component
    ''' <summary>
    ''' Runs in gui thread.  Blocks messages in windows message pump.  Timers don't run.  HEEEY the program is frozen.  This is why you do not do work in the gui thread.
    ''' </summary>
    Public Sub Wait10()
        MessageBox.Show("This will lock the gui thread for 10 seconds. Try moving the window after hitting ok.  Note the time display freezes.Note the log doesn't update and the tmrLog does not run to dequeue the messages.")
        Dim id As Guid = Guid.NewGuid
        Dim strt = Broadcast("Calling Thread.Sleep", id)
        Thread.Sleep(10000) ' do not use thread sleep in your code.  This forces the problem and is used here to demonstrate the issue of running long running code in the gui thread.
        Broadcast("Thread.Sleep complete", id, strt)
    End Sub

    ''' <summary>
    ''' Runs a task that waits 10 seconds to simulate work.
    ''' </summary>
    ''' <returns></returns>
    Public Async Function Wait10Async() As Task
        Dim id As Guid = Guid.NewGuid
        'in gui thread or at least the thread that is calling this method which happens to be the gui thread.  Read up on task schedulers in the tpl
        Dim strt = Broadcast("Wait10 delay start now", id)

        'spins up a task that completes after the delay
        Await Task.Delay(10000)

        'this is still run in the same context as the first line.  However it is called in a task continuation from the end of the delay call.  See https://ranjeet.dev/understanding-how-async-state-machine-works/
        Broadcast("Wait10 delay stop now", id, strt)
    End Function


    Public Async Function GenerateFilesAsync(ProgressInfo As IProgress(Of FileGenerationInfo)) As Task(Of IEnumerable(Of String))

        Dim filenames = Enumerable.Range(0, 3).Select(Function(n) Path.GetTempFileName).ToArray
        Dim id As Guid = Guid.NewGuid
        Dim sw As New Stopwatch
        Dim strt = Broadcast("Generating files.", id)
        sw.Start()
        Dim results = Await Task.WhenAll(filenames.Select(Function(f) GenerateFileAsync(f, ProgressInfo)))
        sw.Stop()
        Broadcast($"Done Generating files.  It took {sw.Elapsed} to generate the files.", id, strt, True) ' excluding because each generate file will record time
        Return results
    End Function
    ''' <summary>
    ''' A task that is CPU bound
    ''' </summary>
    ''' <returns></returns>
    Private Async Function GenerateFileDataAsync() As Task(Of Byte())
        Return Await Task.Run(Function()
                                  Dim rng As RNGCryptoServiceProvider = RNGCryptoServiceProvider.Create
                                  Dim sz As Integer = 52428800
                                  Dim data(sz - 1) As Byte
                                  rng.GetNonZeroBytes(data)
                                  Return data
                              End Function)
    End Function
    ''' <summary>
    ''' A task that uses the cpu bound task and is io bound.
    ''' </summary>
    ''' <param name="pth"></param>
    ''' <returns></returns>
    Private Async Function GenerateFileAsync(pth As String, ProgressInfo As IProgress(Of FileGenerationInfo)) As Task(Of String)
        Dim id As Guid = Guid.NewGuid
        Dim work = Broadcast($"Creating {pth}.", id)
        Using strm As New IO.FileStream(pth, FileMode.Create, FileAccess.Write)

            For x As Integer = 0 To 9 ' 10 10MB writes to file

                Broadcast("Generating data", id)
                Dim data = Await GenerateFileDataAsync()
                Broadcast($"Generated {data.LongLength.ToHumanReadableByteCount(False) } data", id)
                Broadcast($"Writing data {pth}", id)
                Using mem As New MemoryStream(data)
                    Await mem.CopyToAsync(strm)
                    Broadcast($"Done writing data {pth}", id)
                End Using
            Next
            Dim info As New FileInfo(pth)
            Broadcast($"Created {pth} with {info.Length.ToHumanReadableByteCount(False)} size", id, work)
            If ProgressInfo IsNot Nothing Then
                ProgressInfo.Report(New FileGenerationInfo With {.FilePath = pth, .Size = info.Length})
            End If
        End Using

        Return pth
    End Function

    Public Function Broadcast(message As String, Optional ID As Guid? = Nothing, Optional EndID As Guid? = Nothing, Optional Exclude As Boolean = False) As Guid

        Dim evnt As LogMessageEventArgs = New LogMessageEventArgs(message, ID, EndID)
        evnt.ExcludeTimeInTotal = Exclude
        OnBroadcast(evnt)
        Return evnt.ID
    End Function

    Event BroadcastMessage(sender As Object, e As LogMessageEventArgs)

    Protected Overridable Sub OnBroadcast(e As LogMessageEventArgs)
        RaiseEvent BroadcastMessage(Me, e)
    End Sub

End Class
