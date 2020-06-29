Imports System.ComponentModel
Imports System.IO

Public Class Form1
    Private WithEvents Log As New BindingList(Of LogMessageEventArgs)
    Private ReadOnly logStore As New Concurrent.ConcurrentQueue(Of LogMessageEventArgs)
    Dim clicks As Guid = Guid.NewGuid
    Private ReadOnly fls As New Dictionary(Of String, FileGenerationInfo)
    Private allowClose As Boolean = False
    Private Sub btnGui_Click(sender As Object, e As EventArgs) Handles btnGui.Click
        Dim id = tstAsync.Broadcast($"{NameOf(btnGui)} clicked.", clicks)
        tstAsync.Wait10()
        tstAsync.Broadcast($"{NameOf(btnGui)} click done.", clicks, id, True) ' excluding because this is work that contains all the other work.  
    End Sub

    Private Async Sub btnAsyncDelay_Click(sender As Object, e As EventArgs) Handles btnAsyncDelay.Click
        Dim id = tstAsync.Broadcast($"{NameOf(btnAsyncDelay)} clicked.", clicks)
        Await tstAsync.Wait10Async
        tstAsync.Broadcast($"{NameOf(btnAsyncDelay)} done.", clicks, id, True) ' excluding because this is work that contains all the other work.  
    End Sub
    Private Async Sub btnFiles_Click(sender As Object, e As EventArgs) Handles btnFiles.Click
        Dim id = tstAsync.Broadcast($"{NameOf(btnFiles)} clicked.", clicks)
        Dim prog As New Progress(Of FileGenerationInfo)(Sub(info)
                                                            fls.Add(info.FilePath, info)
                                                        End Sub)
        Dim generated = Await Task.Run(Async Function()

                                           Return Await tstAsync.GenerateFilesAsync(prog)
                                       End Function)
        tstAsync.Broadcast($"{NameOf(btnFiles)} done.", clicks, id, True) ' excluding because this is work that contains all the other work.  

    End Sub

    Private Sub tmrTime_Tick(sender As Object, e As EventArgs) Handles tmrTime.Tick
        lblTime.Text = Now.ToLongTimeString
    End Sub



    Private Sub tstAsync_BroadcastMessage(sender As Object, e As LogMessageEventArgs) Handles tstAsync.BroadcastMessage
        logStore.Enqueue(e)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        MessageEventArgsBindingSource.DataSource = Log
        tstAsync.Broadcast("Running in Gui thread.  Form Loaded")
    End Sub
    Dim limit As TimeSpan = TimeSpan.FromMilliseconds(200)
    Sub UpdateLog(Optional EmptyLog As Boolean = False)
        Dim tm As Date = Now

        Dim empty As Boolean = logStore.IsEmpty
        If empty Then Return
        lbLog.BeginUpdate()
        While Not logStore.IsEmpty AndAlso (EmptyLog OrElse Now - tm < limit)
            Dim msg As LogMessageEventArgs = Nothing
            If logStore.TryDequeue(msg) Then
                Log.Add(msg)
            End If
        End While
        lbLog.SelectedIndex = lbLog.Items.Count - 1
        lbLog.EndUpdate()

    End Sub
    Private Sub tmrLog_Tick(sender As Object, e As EventArgs) Handles tmrLog.Tick

        UpdateLog()

    End Sub


    Private Async Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = Not allowClose
        tmrLog.Stop()
        If Not allowClose Then
            Await Task.Run(Sub()
                               Try
                                   Dim cleanup As Guid = Guid.NewGuid
                                   Dim strt As Guid
                                   For Each fl In fls
                                       strt = tstAsync.Broadcast($"Cleaning up {fl}", cleanup)
                                       Try
                                           File.Delete(fl.Key)
                                       Catch ex As Exception
                                           tstAsync.Broadcast($"Failed in Cleaning up {fl}.  {ex.Message}", cleanup)
                                       Finally
                                           tstAsync.Broadcast($"Completed Cleaning up {fl}.", cleanup, strt)
                                       End Try
                                   Next
                                   Invoke(Sub()
                                              UpdateLog(True)
                                          End Sub)

                                   Dim entries = WriteLogs(My.Application.Info.DirectoryPath, Log)
                                   For Each entry In entries
                                       logStore.Enqueue(entry)
                                   Next
                                   Invoke(Sub()
                                              UpdateLog(True)
                                              Activate()
                                          End Sub)
                               Catch ex As Exception
                                   Stop
                               End Try

                           End Sub)
            Dim frmc As New frmClosing
            frmc.Show(Me)

            Await Task.Delay(TimeSpan.FromSeconds(10))
            frmc.Close()
            allowClose = True
            Close()
        End If





    End Sub

    Public Function WriteLogs(fldr As String, log As IEnumerable(Of LogMessageEventArgs)) As IEnumerable(Of LogMessageEventArgs)
        Dim result As IEnumerable(Of LogMessageEventArgs)
        Try


            Dim pth As String = Path.Combine(fldr, $"{Now.Ticks}.log")
            Dim id As Guid = Guid.NewGuid
            Dim newLog As New List(Of LogMessageEventArgs) From {
                New LogMessageEventArgs($"Writting Log to {pth}", id)
            }
            ' log = newLog.Concat(log)
            Dim entries = log.GroupBy(Function(m) m.GroupID).OrderBy(Function(ms) ms.Min(Function(m) m.Time))
            Dim totalTime = TimeSpan.Zero
            Dim totalWorkTime = TimeSpan.Zero
            Using sw As New StreamWriter(pth, False)

                For Each ml In entries
                    sw.WriteLine($"{ml.Key} events")
                    For Each l In ml
                        sw.WriteLine(l.Display)
                    Next
                    Dim grpWorkEntries = ml.ToLookup(Function(l) l.EndsID.HasValue)
                    Dim grpWorkkspans = From stp In grpWorkEntries(True)
                                        Join beg In grpWorkEntries(False) On beg.ID Equals stp.EndsID
                                        Select New With {.Time = stp.Time.Subtract(beg.Time), .Exclude = stp.ExcludeTimeInTotal}

                    Dim diff = TimeSpan.FromTicks(grpWorkkspans.Sum(Function(w) w.Time.Ticks))
                    If Not grpWorkkspans.Any(Function(w) w.Exclude) Then totalTime += diff
                    sw.WriteLine($"All work done in {diff} to do this work.")
                    sw.WriteLine()
                Next
                Dim workEntries = log.Where(Function(l) l.GroupID = clicks).ToLookup(Function(l) l.EndsID.HasValue)
                Dim workspans = From stp In workEntries(True)
                                Join beg In workEntries(False) On beg.ID Equals stp.EndsID
                                Select stp.Time.Subtract(beg.Time)

                totalWorkTime = TimeSpan.FromTicks(workspans.Sum(Function(w) w.Ticks))
                sw.WriteLine($"Total time of doing work: {totalWorkTime}.")
                sw.WriteLine($"Total clock time doing work: {totalTime}.")

                sw.WriteLine(New String("-"c, 80))
                sw.WriteLine("Let's see the entries sorted by time.")

                sw.WriteLine()

                For Each lg In log.OrderBy(Function(l) l.Time)
                    sw.WriteLine(lg.Display)
                Next


                sw.WriteLine(New String("-"c, 80))
                sw.WriteLine("Let's see the entries sorted by thread id then time.")

                For Each glg In log.OrderBy(Function(l) l.ThreadID).ThenBy(Function(l) l.Time).GroupBy(Function(l) l.ThreadID)
                    sw.WriteLine($"Thread ID: {glg.Key}")
                    For Each lg In glg
                        sw.WriteLine(lg.Display)
                    Next
                    sw.WriteLine()
                Next



                newLog.Add(New LogMessageEventArgs($"Done writting Log to {pth}", id))
                sw.WriteLine()

                For Each lg In newLog
                    sw.WriteLine(lg.Display)
                Next
            End Using


            result = newLog
            Process.Start(pth)


        Catch ex As Exception
            Stop
            result = Enumerable.Empty(Of LogMessageEventArgs)
        End Try
        Return result

    End Function
End Class
