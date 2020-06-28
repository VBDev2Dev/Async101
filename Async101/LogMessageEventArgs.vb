Imports System.Threading

Public Class LogMessageEventArgs
    Inherits EventArgs
    Public Sub New(message As String, GroupID As Guid?, Optional EndID As Guid? = Nothing)
        Me.Message = message
        If GroupID.HasValue Then Me.GroupID = GroupID.Value
        If EndID.HasValue Then EndsID = EndID.Value
    End Sub

    ReadOnly Property Time As DateTimeOffset = DateTimeOffset.Now
    Property Message As String
    ReadOnly Property GroupID As Guid = Guid.NewGuid

    Property ID As Guid = Guid.NewGuid
    Property EndsID As Guid? = Nothing
    Property ExcludeTimeInTotal As Boolean
    ReadOnly Property Display As String
        Get
            Return ToString()
        End Get
    End Property
    ReadOnly Property ThreadID As Integer = Thread.CurrentThread.ManagedThreadId
    Public Overrides Function ToString() As String
        Return $"{Message} {GroupID} {Time:O}  ThreadID:{ThreadID}"
    End Function
End Class
