Imports System.Runtime.CompilerServices

Module Utility
    <Extension>
    Public Function ToHumanReadableByteCount(ByVal bytes As Long, ByVal si As Boolean) As String
        Dim unit = If(si, 1000, 1024)

        If bytes < unit Then
            Return $"{bytes} B"
        End If

        Dim exp = CInt((Math.Log(bytes) / Math.Log(unit))) - 1
        Return $"{bytes / Math.Pow(unit, exp)} " & $"{(If(si, "kMGTPE", "KMGTPE"))(exp - 1) & (If(si, String.Empty, "i"))}B"
    End Function
End Module
