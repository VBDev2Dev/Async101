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
