Public Class C_Versiyon
#Disable Warning IDE0040

    Dim Versiyon As String = ""

    Structure Ver
        Dim Major As String
        Dim Minor As String
        Dim Build As String
        Dim Revision As String
    End Structure
    Friend V As Ver


    Function VerKasa() As String
        On Error GoTo 2
        V = Nothing
        V.Major = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Major
        V.Minor = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor
        V.Build = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Build
        V.Revision = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision
        Versiyon = V.Major & "." & V.Minor & "." & V.Build & "." & V.Revision
        Return V.Major & "." & V.Minor & "." & V.Build & "." & V.Revision
        '─────────────────────────────────────────────────────────────────────────────────────────────────────────────────
2:
        V.Major = My.Application.Info.Version.Major
        V.Minor = My.Application.Info.Version.Minor
        V.Build = My.Application.Info.Version.Build
        V.Revision = My.Application.Info.Version.Revision
        Versiyon = V.Major & "." & V.Minor & "." & V.Build & "." & V.Revision
        Return V.Major & "." & V.Minor & "." & V.Build & "." & V.Revision
        '─────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Return ""
    End Function

End Class
