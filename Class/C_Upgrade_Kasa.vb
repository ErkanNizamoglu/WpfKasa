Imports System.Deployment.Application

Public Class C_Upgrade_Kasa
#Disable Warning IDE0081

    Public Function ChkUpGrade(ByVal KasaKod As String, ByVal Manuel As Boolean) As Boolean
        Try
            Dim OkUpgrade As Boolean
            If Manuel = False Then
                Dim C As New C_ChkDataLocalDb
                Conn = C.Sql_conn_localDb()
                Try
                    OkUpgrade = Convert.ToBoolean(Execute_Oku("Select autoupgrade from kasa where kod = '" & KasaKod & "'", "autoupgrade", Conn, False))
                Catch ex As Exception
                End Try
                If OkUpgrade = False Then Return False
            End If
            '────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
            If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
                Dim ad As ApplicationDeployment = ApplicationDeployment.CurrentDeployment
                Dim info As UpdateCheckInfo = ad.CheckForDetailedUpdate()
                If info.UpdateAvailable Then
                    Dim Ok As Boolean = Msg("Şu anki versiyonunuz:" & ad.CurrentVersion.ToString & vbCrLf _
                                            & "Yeni versiyon:" & info.AvailableVersion.ToString() & vbCrLf _
                                            & "kullanılabilir durumda. Yüklemek istiyor musunuz?",
                                            True, True, False)
                    If Ok Then
                        If ad.Update() Then
                            Dim Sql As String
                            Sql = "  Update"
                            Sql &= " kasa"
                            Sql &= " set"
                            Sql &= " versiyon = '" & info.AvailableVersion.ToString() & "'"
                            Sql &= " Where kod = '" & KasaKod & "'"
                            OkUpgrade = Execute_run(Sql, Conn, False)
                            Msg("Program Başarıyla Güncellendi. Şimdi yeniden Başlatılacak.", False, False, True)
                            Application.Current.Shutdown()
                            System.Windows.Forms.Application.Restart()
                        Else
                            Msg("Güncelleme Sırasında Hata Oluştu", False, False, True)
                        End If
                    End If
                Else
                End If
            Else
                Msg("Şu anki versiyonunuz Güncel", True, True, False)
            End If
            '────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
        Return True
    End Function


End Class
