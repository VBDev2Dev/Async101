Imports System.ComponentModel
Imports System.IO

Public Class Form1
    Private WithEvents Log As New BindingList(Of LogMessageEventArgs)
    Private ReadOnly logStore As New Concurrent.ConcurrentQueue(Of LogMessageEventArgs)
    Dim clicks As Guid = Guid.NewGuid
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

    Sub UpdateLog(Optional EmptyLog As Boolean = False)
        Dim tm As Date = Now

        Dim empty As Boolean = logStore.IsEmpty
        If empty Then Return
        lbLog.BeginUpdate()
        While Not logStore.IsEmpty AndAlso (EmptyLog OrElse Now - tm < TimeSpan.FromMilliseconds(200))
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

End Class
