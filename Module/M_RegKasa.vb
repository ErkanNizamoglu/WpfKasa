'Namespace M_FrontSide
#Disable Warning IDE0040
Module M_RegKasa
    Public Const SettingsKey As String = "PerakendeUno"


    Structure RegDegisken
        Dim Server As String
        Dim Data As String
        Dim User As String
        Dim Pass As String
        Dim Port As String
        Dim EndPoint As String
        Dim Sube As String
        Dim Yazici As String
        Dim DisplayAktif As String
        Dim FisBoyutu As String
    End Structure
    Friend Reg As New RegDegisken

    Function RegOku() As Object
        On Error GoTo 1
        My.Settings.SettingsKey = SettingsKey
        If My.Settings.Server = "" Then
            My.Settings.Upgrade()
            If My.Settings.Server <> "" Then ChkTable = True
        End If
        Reg.Server = My.Settings.Server
        Reg.Data = My.Settings.Data
        Reg.User = My.Settings.User
        Reg.Pass = My.Settings.Pass
        Reg.Port = My.Settings.Port
        Reg.EndPoint = My.Settings.EndPoint
        Reg.Yazici = My.Settings.Yazici
        Reg.Sube = My.Settings.Sube
        Reg.DisplayAktif = My.Settings.DisplayAktif
        Reg.FisBoyutu = My.Settings.FisBoyutu

        Return Reg
1:
        MsgBox(Err.Description)
        My.Settings.Reset()


    End Function



    Public Function RegYaz(ByVal R As RegDegisken
                       ) As RegDegisken

        My.Settings.SettingsKey = SettingsKey
        '───────────────────────────────────────────────────
        My.Settings.Server = R.Server
        My.Settings.Data = R.Data
        My.Settings.User = R.User
        My.Settings.Pass = R.Pass
        My.Settings.Port = R.Port
        My.Settings.EndPoint = R.EndPoint
        My.Settings.Yazici = R.Yazici
        My.Settings.Sube = R.Sube
        My.Settings.DisplayAktif = R.DisplayAktif
        My.Settings.FisBoyutu = R.FisBoyutu

        My.Settings.Save()
        Reg = RegOku()
        Return Reg
    End Function

End Module
'End Namespace

