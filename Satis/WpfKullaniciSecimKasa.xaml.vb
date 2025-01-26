Imports System.Data
Imports System.Data.SqlClient
Imports System.Deployment.Application
Imports System.IO
Imports System.Media
Imports System.ServiceModel
Imports System.Text
Imports System.Threading
Imports System.Windows.Threading
Imports Microsoft.VisualBasic.Devices

Public Class WpfKullaniciSecimKasa

#Disable Warning IDE0047

    Private Sub OlayEkle()
        AddHandler Microsoft.Win32.SystemEvents.DisplaySettingsChanged, AddressOf EkranCevir
        AddHandler Nmr0.Click, AddressOf Numarator
        AddHandler Nmr1.Click, AddressOf Numarator
        AddHandler Nmr2.Click, AddressOf Numarator
        AddHandler Nmr3.Click, AddressOf Numarator
        AddHandler Nmr4.Click, AddressOf Numarator
        AddHandler Nmr5.Click, AddressOf Numarator
        AddHandler Nmr6.Click, AddressOf Numarator
        AddHandler Nmr7.Click, AddressOf Numarator
        AddHandler Nmr8.Click, AddressOf Numarator
        AddHandler Nmr9.Click, AddressOf Numarator
        AddHandler BtnSil.Click, AddressOf Numarator
        AddHandler Nmr0.MouseDoubleClick, AddressOf Me.Close
        '   AddHandler Nmr1.MouseDoubleClick, AddressOf Pc_Kapat
        AddHandler BtKullanici.Click, AddressOf KullaniciListele
        AddHandler BtPckapat.Click, AddressOf Pc_Kapat
        AddHandler BtPcMasaustu.Click, AddressOf Pc_MasaUstu
        '    AddHandler Bty.Click, AddressOf Guncelle
        AddHandler BtnGiris.Click, AddressOf Kullanici_Kontrol
        '  AddHandler BtBaglanti.Click, AddressOf Wpf_Wifi_baslat
        AddHandler TimerClock.Elapsed, AddressOf TimerClockTick
    End Sub
    '─────────────────────────────────────────────────────────────────────────────────────────────────────────
    Sub Pc_Kapat()
        Kapat = True
        CancelPrintJob()
        Process.Start("shutdown", "/s /t 1")
        Me.Close()
    End Sub

    Sub Pc_MasaUstu()
        CancelPrintJob()
        Me.Close()
    End Sub

    Public Sub Pc_Restart()
        System.Diagnostics.Process.Start("shutdown", "-r -t 00")
    End Sub

    Private Sub Kullanici_Degistir(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim LbItem As New ListBoxItem
            LbItem = sender
            Kl.ref = LbItem.Uid
            BtKullanici.Content = LbItem.Content
        Catch ex As Exception
        End Try
    End Sub



    Private Sub Kullanici_Kontrol()
        If PasswordBox.Password = "1122335544" Then
            Kl.kod = "Admin"
            PasswordBox.Password = ""
            Dim W As New WpfVeritabaniAyarlariKasa
            W.ShowDialog()
            Exit Sub
        End If

        If Kl.ref <> 0 Then
            DegiskenAl(Kullanici, Kl.ref, True)
            If Kl.sifre = sifre(Kl.kod & PasswordBox.Password) Then
                Kl.kod = Kl.kod
                DegiskenAl(yetki, Kl.yetki, True)
                PasswordBox.Password = ""
                BilgisayarAktif(1, Conn)
                If TimerClock.Enabled = True Then TimerClock.Stop()
                Try
                    Me.ShowInTaskbar = False
                    Dim W As New WpfSatis
                    W.ShowDialog()
                    Me.ShowInTaskbar = True
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try

                If TimerClock.Enabled = False Then TimerClock.Start()
                If Ds_read("select * from kullanici where iptal <> 1 and sube = '" & Reg.Sube & "'", Kullanici, Conn, True, True) Then
                    Kullanici_Listele()
                End If
            Else
                Msg("Şifre Hatalı", False, False, True)
                PasswordBox.Password = ""
            End If
        Else
            Msg("Kullanici Seciniz!!", False, False, True)
        End If
    End Sub

    Private Sub Numarator(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As System.Windows.Controls.Button = sender
        If Me.PasswordBox.Password.Length > 9 Then
            Exit Sub
        End If
        Select Case Btn.Name
            Case "BtnSil"
                Me.PasswordBox.Password = ""

            Case "Nmr0"
                Me.PasswordBox.Password &= "0"

            Case "Nmr1"
                Me.PasswordBox.Password &= "1"

            Case "Nmr2"
                Me.PasswordBox.Password &= "2"

            Case "Nmr3"
                Me.PasswordBox.Password &= "3"

            Case "Nmr4"
                Me.PasswordBox.Password &= "4"

            Case "Nmr5"
                Me.PasswordBox.Password &= "5"

            Case "Nmr6"
                Me.PasswordBox.Password &= "6"

            Case "Nmr7"
                Me.PasswordBox.Password &= "7"

            Case "Nmr8"
                Me.PasswordBox.Password &= "8"

            Case "Nmr9"
                Me.PasswordBox.Password &= "9"


        End Select
        SystemSounds.Asterisk.Play()

        If Me.PasswordBox.Password = "11223344" Then
            Process.GetCurrentProcess.Kill()
        End If
    End Sub

    Private Sub EkranCevir()
        EkranDizayn_None(Me)
    End Sub

    Private Sub KullaniciListele()
        Ds_read("select * from " & Kullanici & " where iptal <> 1 and sube = '" & Reg.Sube & "'", Kullanici, Conn, True, True)
        Kullanici_Listele()
    End Sub

    Private Sub Kullanici_Listele()
        LbKullanici.Items.Clear()
        For Each dr As System.Data.DataRow In Ds.Tables(Kullanici).Rows
            Dim ref As Long = Chk_Null("ref", dr)
            Dim kod As String = Chk_Null("kod", dr)
            '  Dim ad As String = Chk_Null("ad", dr)
            '  Dim soyad As String = Chk_Null("soyad", dr)
            '  Dim aktif As Boolean = Chk_Null("aktif", dr)
            '  Dim sifre As String = Chk_Null("sifre", dr)
            Dim LbItem As New ListBoxItem
            LbItem.Uid = ref
            LbItem.Content = dr.Item("kod")
            Dim Styl As New Style
            LbItem.Height = 60
            LbItem.FontSize = 40
            LbKullanici.Items.Add(LbItem)
            AddHandler LbItem.Selected, AddressOf Kullanici_Degistir
        Next
    End Sub

    Private Sub ProgramKapat()
        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub Customer_Display(ByVal PortName As String)
        Try
            Dim sp As New System.IO.Ports.SerialPort
            sp.PortName = PortName
            sp.BaudRate = 9600
            sp.Parity = System.IO.Ports.Parity.None
            sp.DataBits = 8
            sp.StopBits = System.IO.Ports.StopBits.One
            sp.Open()
            sp.WriteLine("Selam Millet")
            sp.Close()
            sp.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Msg(PortName, False, False, True)
    End Sub


    Public Function ChkUpGrade() As Boolean
        Try

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
                            Msg("Program Başarıyla Güncellendi. Şimdi yeniden Başlatılacak.", False, False, True)
                            Application.Current.Shutdown()
                            System.Windows.Forms.Application.Restart()
                        Else
                            Msg("Güncelleme Sırasında Hata Oluştu", False, False, True)
                        End If
                    End If
                Else
                    '  MessageBox.Show("Güncelleme bulunmamaktadır.")
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return True
    End Function

    Private Sub VeritabaniAyarlari()
        Dim VeritabaniAyarlari As New WpfVeritabaniAyarlariKasa
        VeritabaniAyarlari.ShowDialog()
        'Chktable Aktif oluyor
    End Sub



    Private Const path As String = "C:\Program Files(x86)\TeamViewer\TeamViewer.exe"
    Private Const arguments As String = " -i {0} --Password {1}"

    Private Sub SaatFark()
        Dim LocalDateTime As Date = Now
        Dim ZamanFark As TimeSpan = New TimeSpan(0, 1, 0).Duration
        Dim MssqlDateTime As Date = Execute_Oku("Select Getdate() as TarihSaat", "TarihSaat", Conn)
        Dim TSpan As TimeSpan = (LocalDateTime - MssqlDateTime).Duration
        '────────────────────────────────────────────────────────────────────────────────────────────
        If TSpan > ZamanFark Then
            Dim Mesaj As String
            Mesaj = "Tarih Farklı" & vbCrLf
            Mesaj &= String.Format("{0:dd.MM.yyy HH:mm:ss}", LocalDateTime) & vbCrLf
            Mesaj &= String.Format("{0:dd.MM.yyy HH:mm:ss}", MssqlDateTime)
            Msg(Mesaj, False, False, True)
            If ay.gercek_zaman_aktif Then
                End
            End If
        End If
    End Sub

    Public TimerClock As New System.Timers.Timer
    'Public ClockIsRunning As New Boolean

    Private Sub ClockRun()
        TbTarihSaat.Text = String.Format("{0:dd.MM.yyy HH:mm:ss}", Now)
    End Sub

    Delegate Sub NextPrimeDelegate()

    Private Sub TimerClockTick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispatcher.BeginInvoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf ClockRun))
    End Sub

    Private Function PosOku() As Boolean
        On Error GoTo 1
        If Ds_read("select * from pos where sube = '" & Reg.Sube & "'", pos, Conn, True, True) Then
        Else
            Return False
        End If
        Return True
1:
        Return False
    End Function

    Private Function kullaniciOku() As Boolean
        On Error GoTo 1
        Kl = Nothing
        If Ds_read("select * from kullanici where iptal <> 1 and sube = '" & Reg.Sube & "'", Kullanici, Conn, True, True) Then
            If Ds.Tables(Kullanici).Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
1:
        Return False
    End Function

    Private Function TabloKontrol() As Boolean
        Dim Path As String = System.AppDomain.CurrentDomain.BaseDirectory
        If ChkTableTablolar(Conn, Path, "PerakendeLdb") Then
            Return True
        End If
        Return False
    End Function



    Private Sub Main()
        On Error GoTo HataYaz

        '────────────────────────────────────────────────────────────────────────────────────────────────────
        Reg = RegOku()
        If (Reg.Server = "" Or Reg.Server = "" Or Reg.EndPoint = "") Then
            VeritabaniAyarlari()
        End If
        '────────────────────────────────────────────────────────────────────────────────────────────────────
        Dim Ver As String = VerKasa()
        TbVer.Text = "V." & Ver
        '────────────────────────────────────────────────────────────────────────────────────────────────────

        '  Dim F As New F_ChkPort
        '  F.ShowDialog()
        EkranDizayn_None(Me)
        '─────────────────────────────────────────────────────────────────────────────────────────────────────
        Dim U As New C_Upgrade_Kasa
        Dim Ok As Boolean = U.ChkUpGrade(System.Net.Dns.GetHostName(), False)
        '─────────────────────────────────────────────────────────────────────────────────────────────────────
        EpAddress = New EndpointAddress("http://" & Reg.EndPoint & ":3265/WSiletisim.asmx")
        Dim C As New C_ChkDataLocalDb
        If C.Chk_Data_Localdb() = True Then
            TabloKontrol()
        Else
            Msg("Database Oluşturulamadı!!", False, False, True)
            Process.GetCurrentProcess.Kill()
        End If
        '────────────────────────────────────────────────────────────────────────────────────────────────────
        ChkPluTable(Conn)
        '────────────────────────────────────────────────────────────────────────────────────────────────────
        Dim W_Plu As New WpfPluKasa
        '    W_Plu.ShowDialog()
        W_Plu.Main(True)
        '────────────────────────────────────────────────────────────────────────────────────────────────────
        If Execute_Oku("select OBJECT_ID('v_satisurunler', 'V') as plu", "plu", Conn, False) = Nothing Then
            Execute_Run(Create_ViewSatisUrunler, Conn)
        End If
        '────────────────────────────────────────────────────────────────────────────────────────────────────
        If PosOku() = False Then
            VeritabaniAyarlari()
        End If
        If kullaniciOku() = False Then
            VeritabaniAyarlari()
        Else
            OlayEkle()
            KullaniciListele()
        End If
        SaatFark()
        TimerClock.Start()
        '  Me.Show()


        Exit Sub
HataYaz:
        Me.ShowInTaskbar = True
        MsgBox(Err.Description)
        'HataYaz.KayitHataLog(Name, Err)
        Err.Clear()
        Me.Close()
    End Sub
End Class


'  Dim C_v As New CVeriban_EarsivFatura
'  C_v.TRANSFER_OKC_DOCUMENT_SORGULAMA_TEST("274267b9-d38d-4bb8-a25f-4c342b578106")
'  C_v.FATURA_SORGULAMA("2444080c-955e-4cba-820d-362fe0758c6b".ToUpper)
'C_v.VKNTCKN_BAZINDA_TARIH_ARALIKLI_FATURA_ETTN_LISTESI("15556043852")
'C_v.VKNTCKN_BAZINDA_TARIH_ARALIKLI_FATURA_ETTN_LISTESI("11111111111")

' C_v.TRANSFER_SORGULAMA_TEST("20240814_F05D01DD-4769-40AB-9E09-E9488AED1501".ToUpper)
' C_v.TRANSFER_SORGULAMA_TEST("fda24211-8bf8-4b2e-b121-34e8bc3bd3b9")
'  C_v.VKNTCKN_EFATURA_MUKELLEFIMI("15556043852")

' Dim C_f As New CVeriban_Efatura
' C_f.FATURA_DURUMUNU_SORGULA("274267b9-d38d-4bb8-a25f-4c342b578106".ToUpper)
' C_f.MUKELLEF_ISIM_BILGISI("15556043852")
' C_f.MUKELLEF_ISIM_BILGISI("15556043852")

'********************************
