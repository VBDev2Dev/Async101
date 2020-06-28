Public Class Tester
    Inherits ComponentModel.Component

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
