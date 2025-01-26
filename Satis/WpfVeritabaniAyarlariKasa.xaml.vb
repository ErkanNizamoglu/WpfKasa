Imports System.Data.SqlClient
Imports System.Globalization

Public Class WpfVeritabaniAyarlariKasa
    Private Sub Olay_Ekle()
        AddHandler BtKlavye.Click, AddressOf KlavyeAc
        AddHandler BtYazici.Click, AddressOf FisYaziciChk
        'AddHandler BtSubeNo.Click, AddressOf SubeSec
        AddHandler TbServer.GotFocus, AddressOf ElemanSec
        'AddHandler BtLocalServer.Click, AddressOf LocalServerTest
        AddHandler BtEndpoint.Click, AddressOf EndpointTest
        AddHandler BtVersiyon.Click, AddressOf Upgrade
        AddHandler UcMenu.BtKayit.Click, AddressOf Kayit
        AddHandler UcMenu.BtCikis.Click, AddressOf Cikis
    End Sub

    Private Sub FisYaziciChk()
        CheckPrinter(CbFisYazici)
    End Sub

    Private Sub SubeSec()
        Dim W As New WpfListeSube
        W.ShowDialog()
        TbSube.Text = sb.aciklama
    End Sub

    Private Sub Upgrade()
        Dim C As New C_Upgrade_Kasa
        Dim Ok As Boolean = C.ChkUpGrade(System.Net.Dns.GetHostName(), True)
    End Sub

    Private Sub KlavyeAc()
        ' Rdd = New GridLength(5, GridUnitType.Star)
        If KlavyeOk = False Then
            RdKlavye.Height = New GridLength(0, GridUnitType.Star)
            KlavyeOk = True
        Else
            RdKlavye.Height = New GridLength(1, GridUnitType.Star)
            KlavyeOk = False
        End If
    End Sub

    Dim KlavyeOk As Boolean = False

    Private Sub KlavyeSec(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Klavye(Btn, Nothing)
    End Sub


    Private Sub Olaylar(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case "BtCikis"
                Cikis()
            Case "BtKayit"
                Kayit()
        End Select

    End Sub

    Private Sub Cikis()
        Me.Close()
    End Sub

    Sub CheckPrinter(ByVal Cb As ComboBox)
        Dim ok As Boolean = False
        Dim MR As Management.ManagementObjectCollection
        Dim MS As Management.ManagementObjectSearcher
        Dim MO As Management.ManagementObject
        MS = New Management.ManagementObjectSearcher("Select * from Win32_Printer")
        MR = MS.Get
        Dim MyPrinter As String = ""
        Dim MyPrinterType As String = ""
        Cb.Items.Clear()
        For Each MO In MR
            MyPrinter = MO("Name")
            MyPrinterType = MO("DriverName")
            If MO("WorkOffline") = False Then
                Cb.Items.Add(MyPrinter)
                '  Pd.PrintQueue = New Printing.PrintQueue(New Printing.PrintServer, MyPrinter)
                ok = True
            End If
        Next
        Cb.Background = Brushes.Aquamarine
    End Sub

    Function CheckPrinterModel(name_ As String) As String
        Dim ok As Boolean = False
        Dim MR As Management.ManagementObjectCollection
        Dim MS As Management.ManagementObjectSearcher
        Dim MO As Management.ManagementObject
        MS = New Management.ManagementObjectSearcher("Select * from Win32_Printer")
        MR = MS.Get
        Dim MyPrinter As String = ""
        Dim MyPrinterModel As String = ""
        For Each MO In MR
            MyPrinter = MO("Name")
            If MyPrinter = name_ Then
                MyPrinterModel = MO("DriverName")
                Exit For
            End If
        Next
        Return MyPrinterModel
    End Function

    Private Sub Kayit()
        Dim Err As Boolean = False

        If TbServer.Text = "" Then
            Msg("Server Adını Giriniz!!", False, False, True)
            Err = True
        End If
        If TbSube.Text = "" Or IsNumeric(TbSube.Text) = False Then
            Msg("Şube Seciniz!!", False, False, True)
            Err = True
        End If
        If TbEndpoint.Text = "" Then
            Msg("endpoint Yazınız!!", False, False, True)
            Err = True
        End If
        If Err Then Exit Sub


        Reg.Server = TbServer.Text
        Reg.Yazici = CbFisYazici.Text
        Reg.EndPoint = TbEndpoint.Text
        Reg.Sube = TbSube.Text
        Reg.FisBoyutu = CbFisBoyutu.Text

        Reg = RegYaz(Reg)
        Cikis()
    End Sub

    Private Sub Ekran_Getir()
        Reg = RegOku()
        CheckPrinter(CbFisYazici)

        If Reg.Server = "" Then
            TbServer.Text = "(LocalDb)\MSSQLLocalDB"
            TbServer.Background = Brushes.Pink
            Reg.Server = "(LocalDb)\MSSQLLocalDB"

        Else
            TbServer.Text = Reg.Server
            TbServer.Background = Brushes.LightGreen
        End If

        CbFisBoyutu.Items.Add("80mm")
        CbFisBoyutu.Items.Add("58mm")

        TbSube.Text = Reg.Sube
        CbFisYazici.Text = Reg.Yazici
        TbServer.Text = Reg.Server
        TbEndpoint.Text = Reg.EndPoint
        CbFisBoyutu.Text = Reg.FisBoyutu
        TbVersiyon.Text = VerKasa()
    End Sub

    Private Sub Main()

    End Sub

    Private Sub Wpf_Kayit_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.Key
            Case Key.F2
                Kayit()
            Case Key.F3
                Cikis()
        End Select
    End Sub


    Private Sub Wpf_Ayarlar_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        EkranDizayn_None(Me)

        Ekran_Getir()
        Olay_Ekle()


    End Sub

    Private Sub EndpointTest()
        On Error Resume Next
        Dim ci = CultureInfo.InvariantCulture
        Dim tSpan As New TimeSpan
        Dim NStart As Date = Now
        If ServisExecuteRun("Select * from ayarlar") Then
            Dim NStop As Date = Now
            tSpan = NStop - NStart
            Dim BString As String = tSpan.ToString("ss\,fff", ci)
            Msg("Test Başarılı !!!" & vbCrLf & BString, False, False, True)
        Else
            Msg("*** Test Başarısiz !!! ***", False, False, True)
        End If
    End Sub
End Class
