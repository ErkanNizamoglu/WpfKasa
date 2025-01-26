Imports System.Collections
Imports System.Data
Imports System.Data.SqlClient
Imports System.ServiceModel
Imports System.Threading
Imports System.Windows.Threading
Imports Microsoft.VisualBasic.Devices

Partial Module M_DataKasa
    Public Const DataDir As String = "C:\Perakende\Data"
    Public Const LocalDbDataName As String = "PerakendeLdb"

    Friend EpAddress As EndpointAddress

    Friend tablo_ As ArrayList
    Friend OkChkPlu As Boolean = False





    Enum Fonksiyon
        etiketleme = 22
        belgetekrar = 20
        ekrankilit = 30
        cekmece = 32
        cikis = 999
        zraporu = 100
        xraporu = 101
        aktarim = 200
        display = 400
        raporurunsatis = 500
        plutussablonu = 700
        sayim = 800
        degisim = 850
    End Enum

    Function Msg(Txt As String, ByVal BtnEvet As Boolean, ByVal BtnHayir As Boolean, ByVal BtnTamam As Boolean) As Boolean
        Dim msgbx As New WpfMsgBx
        Return msgbx.main(Txt, BtnEvet, BtnHayir, BtnTamam)
    End Function

    Private Function LocalDbSql_calis()

        Dim ok As Boolean = True
        Try
            Dim Prm As String = " start 'local'"
            Dim ProcessPath As String = "C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe"
            Dim objProcess As System.Diagnostics.Process
            objProcess = New System.Diagnostics.Process()
            objProcess.StartInfo.FileName = ProcessPath
            objProcess.StartInfo.WorkingDirectory = "C:\Program Files\Microsoft SQL Server\120\Tools\Binn"
            objProcess.StartInfo.Arguments = Prm
            objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal
            objProcess.Start()
            objProcess.WaitForExit()
            objProcess.Close()
        Catch ex As Exception
            ok = False
        End Try
        Return ok
    End Function

    Function BilgisayarAktif(ByVal aktif As Boolean, ByVal ConnLocal As SqlConnection) As Boolean
        On Error Resume Next
        Dim Cver As New C_Versiyon
        '──────────────────────────────────────────────────────────────────────────────────────────────
        pd.kod = My.Computer.Name
        pd.macadress = mac_adress()
        pd.serial = DriveSerial
        pd.ip = GetMachineIPAddress()
        pd.versiyon = Cver.VerKasa
        pd.aktif = True
        '──────────────────────────────────────────────────────────────────────────────────────────────
        For Each dr As System.Data.DataRow In Ds.Tables(pos).Select("kod = '" & pd.kod & "'")
            pd.ref = dr.Item("ref")
            DegiskenAl(pos, pd.ref, True)
            Reg.Sube = pd.sube
            Exit For
        Next
        '──────────────────────────────────────────────────────────────────────────────────────────────
        Dim aktifbyte As String = 0
        If aktif Then aktifbyte = 1
        '──────────────────────────────────────────────────────────────────────────────────────────────
        Dim sql As String
        sql = "  update pos set "
        sql &= " aktif = " & aktifbyte
        sql &= ",macadress = '" & pd.macadress & "'"
        sql &= ",versiyon = '" & pd.versiyon & "'"
        sql &= ",serial = '" & pd.serial & "'"
        sql &= ",ip = '" & pd.ip & "'"
        sql &= " where "
        sql &= " sube = '" & Reg.Sube & "'"
        sql &= " and kod =  N'" & pd.kod & "'"
        '──────────────────────────────────────────────────────────────────────────────────────────────
        ServisExecuteRunThr(sql)
        Return Execute_run(sql, ConnLocal, False)
        '──────────────────────────────────────────────────────────────────────────────────────────────
        'Ds_read("select * from pos where macadress = '" & pd.macadress & "'", "pos", True, True)
        'If Bulk_Update(Ds.Tables("pos"), ConnRemote, False) = True Then Return True
        ' Return False
    End Function

    Sub RemoveTable(ByVal Tbl As String)
        Try
            Ds.Tables.Remove(Tbl)
        Catch ex As Exception
        End Try
    End Sub

    Function SayacAl_HareketBaslik() As Int16
        On Error Resume Next

        Dim Sayac As Integer = 0
        Dim Yil As String = Year(Now).ToString
        Dim Ay As String = Format(Month(Now), "00")
        Dim Gun As String = Format(Microsoft.VisualBasic.DateAndTime.Day(Now), "00")
        Dim Tarih As String = Yil & "-" & Ay & "-" & Gun
        Dim Sql As String = "SELECT Max(sayac) as sayac FROM hareket_baslik where tarih LIKE '" & Tarih & "%'"
        Sayac = Execute_Oku(Sql, "sayac", Conn, False)
        Return Sayac
    End Function

    Function SayacAl_StokBaslik() As Int16
        On Error Resume Next

        Dim Sayac As Integer = 0
        Dim Yil As String = Year(Now).ToString
        Dim Ay As String = Format(Month(Now), "00")
        Dim Gun As String = Format(Microsoft.VisualBasic.DateAndTime.Day(Now), "00")
        Dim Tarih As String = Yil & "-" & Ay & "-" & Gun
        Dim Sql As String = "SELECT Max(sayac) as sayac FROM hareket_baslik where tarih LIKE '" & Tarih & "%'"
        Sayac = Execute_Oku(Sql, "sayac", Conn, False)
        If Sayac = 0 Then Sayac = 1
        Return Sayac
    End Function

    Function SayacAl_SayimBaslik() As Int16
        On Error Resume Next

        Dim Sayac As Integer = 0
        Dim Yil As String = Year(Now).ToString
        Dim Ay As String = Format(Month(Now), "00")
        Dim Gun As String = Format(Microsoft.VisualBasic.DateAndTime.Day(Now), "00")
        Dim Tarih As String = Yil & "-" & Ay & "-" & Gun
        Dim Sql As String = "SELECT Max(sayac) as sayac FROM hareket_baslik where tarih LIKE '" & Tarih & "%'"
        Sayac = Execute_Oku(Sql, "sayac", Conn, False)
        If Sayac = 0 Then Sayac = 1
        Return Sayac
    End Function


    Function ServisOkuDataset(ByVal Tbl As String, ByVal Sql As String) As Boolean
        Try
            Dim Service As New Sriletisim.WSiletisimSoapClient
            Service.ChannelFactory.Endpoint.Binding.CreateBindingElements()
            Service.Endpoint.Address = EpAddress
            ' DsServisOku.Tables.Add(Service.ReadDataTable(Sql, Tbl))
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Function ServisOkuTable(ByVal Tbl As String, ByVal Sql As String) As DataTable
        Dim Dt As New DataTable
        Try
            DsServisOku.Tables.Remove(Tbl)
        Catch ex As Exception
        End Try
        Try
            Dim S As New Sriletisim.WSiletisimSoapClient
            CType(S.Endpoint.Binding, ServiceModel.BasicHttpBinding).MaxReceivedMessageSize = Int32.MaxValue
            S.ChannelFactory.Endpoint.Binding.CreateBindingElements()
            S.Endpoint.Address = EpAddress
            Dt = S.ReadDataTable(Sql, Tbl)
        Catch ex As Exception

            Dim a = 9
        End Try
        Return Dt
    End Function

    Friend Function ServisExecuteRun(ByVal Sql As String) As Boolean
        Dim Ok As Boolean = False
        Try
            Dim Service As New Sriletisim.WSiletisimSoapClient
            Service.ChannelFactory.Endpoint.Binding.CreateBindingElements()
            Service.Endpoint.Address = EpAddress
            Ok = Service.ExecuteSql(Sql)
        Catch ex As Exception
        End Try
        Return Ok
    End Function

    Private Function ServisExecuteThr() As Boolean
        Dim Ok As Boolean = False
        Try
            Dim Service As New Sriletisim.WSiletisimSoapClient
            Service.ChannelFactory.Endpoint.Binding.CreateBindingElements()
            Service.Endpoint.Address = EpAddress
            Ok = Service.ExecuteSql(ExecuteRunThrSql)
        Catch ex As Exception
        End Try
        Return Ok
    End Function

    Friend ExecuteRunThrSql As String

    Friend Sub ServisExecuteRunThr(ByVal Sql As String)
        If Sql = "" Then Exit Sub
        ExecuteRunThrSql = Sql
        Dim trd As Thread = Nothing
        trd = New Thread(AddressOf ServisExecuteThr) With {
            .IsBackground = True
        }
        trd.Start()
    End Sub

    Function ServisOkuCreateTable(ByVal Tbl As String, ByVal Sql As String) As String
        Try
            Dim Service As New Sriletisim.WSiletisimSoapClient
            Service.ChannelFactory.Endpoint.Binding.CreateBindingElements()
            Service.Endpoint.Address = EpAddress
            '  Ds.Tables.Add(Service.ReadDataTable(Sql, Tbl))
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    Public Sub DatagridSira(Dg As DataGrid, Sira As Long)
        Try
            Dim Sm As System.Windows.Controls.DataGridSelectionMode = Dg.SelectionMode
            Dg.ScrollIntoView(Dg.Items.GetItemAt(Sira))
            Dg.SelectionMode = DataGridSelectionMode.Single
            Dg.SelectionUnit = DataGridSelectionUnit.FullRow
            Dg.SelectedIndex = Sira
            Dim row As DataGridRow = CType(Dg.ItemContainerGenerator.ContainerFromIndex(Sira), DataGridRow)
            If row IsNot Nothing Then
                Dg.CurrentCell = New DataGridCellInfo(Dg.Items(Sira), Dg.Columns(0))
            End If
            Dg.SelectionMode = Sm
        Catch ex As Exception
        End Try
    End Sub


    Function VerKasa() As String
        On Error GoTo 2
        Dim U_Major As String = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Major
        Dim U_Minor As String = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor
        Dim U_Build As String = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Build
        Dim U_Revision As String = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision

        Return U_Major & "." & U_Minor & "." & U_Build & "." & U_Revision
2:
        Dim M_Major As String = My.Application.Info.Version.Major
        Dim M_Minor As String = My.Application.Info.Version.Minor
        Dim M_Build As String = My.Application.Info.Version.Build
        Dim M_Revision As String = My.Application.Info.Version.Revision
        Return M_Major & "." & M_Minor & "." & M_Build & "." & M_Revision

        Return ""
    End Function

    Friend RAMTotal As String
    Friend RAMAvailable As String
    Friend RAMUsed As String

    Public Sub getAvailableRAM()
        On Error Resume Next
        Dim CI As New ComputerInfo()

        Dim m_PerformanceCounter As New _
    System.Diagnostics.PerformanceCounter(
        "Processor", "% Processor Time", "_Total")

        Dim mem As ULong = ULong.Parse(CI.AvailablePhysicalMemory.ToString())
        Dim mem1 As ULong = ULong.Parse(CI.TotalPhysicalMemory.ToString()) - ULong.Parse(CI.AvailablePhysicalMemory.ToString())

        RAMTotal = Convert.ToInt64(CI.TotalPhysicalMemory / (1024 * 1024))
        RAMAvailable = Convert.ToInt64(mem / (1024 * 1024))
        RAMUsed = Convert.ToInt64(mem1 / (1024 * 1024))
        RAMUsed = (RAMUsed / RAMTotal) * 100

        '        RAMAvailable = (mem / (1024 * 1024) & " MB").ToString() 'changed + to &
        '        RAMUsed = (mem1 / (1024 * 1024) & " MB").ToString() 'changed + to &
    End Sub


End Module
