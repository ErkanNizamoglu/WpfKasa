#Disable Warning IDE0002
#Disable Warning IDE0040

Class Application

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        '  reg = Reg_Oku()
        '  If reg.LocalServer = "" Then
        '  Exit Sub
        '  End If
        '  Dim C As New C_Upgrade
        '  Dim Ok As Boolean = C.ChkUpGrade(System.Net.Dns.GetHostName(), False)
    End Sub
    ' Başlangıç, Çıkış ve DispatcherUnhandledException gibi uygulama düzeyinde olaylar
    ' bu dosyada işlenebilir.
End Class
