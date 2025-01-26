Imports System.Data
Imports System.Data.SqlClient
Imports System.Media
Imports System.Windows.Controls.Primitives
Imports System.Threading
Imports System.Windows.Threading
Imports System.Globalization
Imports System.Management

Partial Public Class WpfSatis

    Private TimerPlu As DispatcherTimer = New DispatcherTimer()

    Dim UgTerazi As New UniformGrid
    Private TimerBizerba As DispatcherTimer = New DispatcherTimer()
    Public Const acil As String = Chr(27) + Chr(112) + Chr(0) + Chr(25)
    Public Const eArsivFatura As String = "e-Arşiv Fatura"
    Public Const eFatura As String = "e-Fatura"
    Public Const fis As String = "fis"
    Public Const iade As String = "iade"

    Private WithEvents TimerRam As New System.Timers.Timer
    Private WithEvents TimerCpu As New System.Timers.Timer


    Private Sub Olaylar()


        AddHandler BtMakina.Click, AddressOf BtMakinaTemizle
        AddHandler BtMakinaOrta.Click, AddressOf UrunEkleBarkod
        '  AddHandler BtGridUrunDurum.Click, AddressOf DurumTemizle
        AddHandler BtUrunAdet.Click, AddressOf AdetYaz

        AddHandler BtToplam.Click, AddressOf ToplamAc
        AddHandler TimerPlu.Tick, AddressOf TimerPluSubeChk



        If ay.terazi_aktif Then
            AddHandler TimerBizerba.Tick, AddressOf TimerPluBizerba
        End If
        If ay.yuvarlama_aktif Then
            AddHandler BtYuvarla.Click, AddressOf Yuvarla
        End If
        If ay.indirim_aktif Then
            AddHandler BtTutarIndirim.Click, AddressOf TutarToplamIndirim
        End If
        If ay.indirim_aktif Then
            AddHandler BtYuzdeIndirim.Click, AddressOf TutarYuzdeIndirim
        End If

        '   AddHandler BtKrediKarti.Click, AddressOf TahsilatAc
        '   AddHandler BtNakit.Click, AddressOf TahsilatAc

        AddHandler BtUrunSil.Click, AddressOf Sil

        AddHandler Bt0.Click, AddressOf Numarator
        AddHandler Bt1.Click, AddressOf Numarator
        AddHandler Bt2.Click, AddressOf Numarator
        AddHandler Bt3.Click, AddressOf Numarator
        AddHandler Bt4.Click, AddressOf Numarator
        AddHandler Bt5.Click, AddressOf Numarator
        AddHandler Bt6.Click, AddressOf Numarator
        AddHandler Bt7.Click, AddressOf Numarator
        AddHandler Bt8.Click, AddressOf Numarator
        AddHandler Bt9.Click, AddressOf Numarator
        AddHandler BtBackSpace.Click, AddressOf Numarator
        AddHandler BtVirgul.Click, AddressOf Numarator

        AddHandler TimerRam.Elapsed, AddressOf TimerRamTick
        AddHandler TimerCpu.Elapsed, AddressOf TimerCpuTick


    End Sub


    Private Function EmurateCentralProcessingUnitUsage() As String



    End Function

    Private Sub CpuShowRun()
        On Error GoTo 1
        Dim searcher = New ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor")
        Dim cpuUsages = searcher.[Get]().Cast(Of ManagementObject)().[Select](Function(x) New With {Key .Name = x("Name"), Key .Usage = x("PercentProcessorTime")
    }).ToList()
        Dim totalUsage = cpuUsages.Where(Function(x) x.Name.ToString() = "_Total").[Select](Function(x) x.Usage).SingleOrDefault()

        Dim val As Single = totalUsage
        pbCpu.Maximum = 100
        pbCpu.Value = val
        Exit Sub
1:
        TimerCpu.Enabled = False
        TimerCpu.Stop()
    End Sub

    Private Sub TimerCpuTick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispatcher.BeginInvoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf CpuShowRun))
    End Sub

    Private Sub MemoryShowRun()
        On Error GoTo 1
        getAvailableRAM()
        pbRam.Maximum = 100
        pbRam.Value = RAMUsed
        Exit Sub
1:
        TimerRam.Enabled = False
        TimerRam.Stop()





    End Sub

    Private Sub StatusBarAc()
        BrStatausBar.Visibility = Visibility.Visible
        TimerRam.Start()
    End Sub

    Private Sub StatusBarKapat()
        BrStatausBar.Visibility = Visibility.Collapsed
        TimerRam.Enabled = False
        TimerRam.Stop()
    End Sub

    Private Sub TimerRamTick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispatcher.BeginInvoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf MemoryShowRun))
    End Sub



    Dim DtUrunlerCount As Integer = 0
    Dim DtBarkodlarCount As Integer = 0
    Dim errNumber As Integer = 0
    Dim RecordSize As Integer = 100000

    Dim MakinaOrtaText As String
    Private Sub MakinaOrtaKeydown()
        MakinaOrtaText = TbMakinaOrta.Text
    End Sub

    Dim MakinaText As String
    Private Sub MakinaKeydown()
        MakinaText = TbMakina.Text
    End Sub



    Private Function LastDateYaz(ByVal tablo As String, ByVal Tarih As Date) As Boolean
        Dim tarihstr As String = TarihAyarlaislem(Tarih)

        If Execute_run("update updatetable set tarih = " & tarihstr & " where tablo = '" & tablo & "'", Conn, True) Then
            Return True
        End If
        Return False
    End Function

    Dim odemeilk As Integer = 0
#Disable Warning IDE0047
    Private Sub TahsilatSatir(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Bt As New Button
        Bt = sender
        Dim ref As Integer = Bt.Uid
        If Hesap Then
            Try
                hb.indirim_toplam += hb.indirim_alt_toplam
                Dim ToplamIndırımYuzde As Decimal = ((hb.indirim_alt_toplam) * 100) / (hb.brut_toplam + hb.indirim_satir_toplam)
                hb.kdv_toplam = hb.kdv_toplam + ((hb.kdv_toplam * ToplamIndırımYuzde) / 100)
            Catch ex As Exception
            End Try

            SatisYaz()
            Hesap = False
            odemeilk = 0
            Exit Sub
        End If
        DegiskenAl(odeme, ref, False)
        th.baslik = hb.ref
        th.baslikguid = hb.guid
        th.tutar = hb.net_toplam
        If TbMakinaOrta.Text <> "" Then
            If IsNumeric(TbMakinaOrta.Text) Then
                th.tutar = TbMakinaOrta.Text
            End If
        Else
            If IsNumeric(TbToplam.Text) Then
                th.tutar = TbToplam.Text
            End If
        End If
        th.odeme = od.ref
        th.odeme_kod = od.kod
        th.para = od.para
        th.kur = 1
        odemeilk = th.odeme
        For Each dr As DataRow In Ds.Tables(para).Select("ref = '" & od.para & "'")
            th.kur = Chk_Null("kur", dr)
        Next
        If th.kur = 0 Then th.kur = 1
        th.tutar = th.tutar * th.kur
        Ds.Tables(tahsilat).Rows.Add()
        Dim index As Integer = Ds.Tables(tahsilat).Rows.Count - 1
        Ds.Tables(tahsilat).Rows(index).Item("baslik") = hb.ref
        Ds.Tables(tahsilat).Rows(index).Item("tutar") = th.tutar
        Ds.Tables(tahsilat).Rows(index).Item("odeme") = th.odeme
        Ds.Tables(tahsilat).Rows(index).Item("odeme_kod") = th.odeme_kod
        Ds.Tables(tahsilat).Rows(index).Item("para") = th.para
        Dim toplam As Decimal = 0
        For Each dr As DataRow In Ds.Tables(tahsilat).Rows
            toplam += Chk_Null("tutar", dr)
        Next
        th.toplam = toplam
        th.kalan = hb.net_toplam - th.toplam
        Ds.Tables(tahsilat).Rows(index).Item("toplam") = th.toplam
        Ds.Tables(tahsilat).Rows(index).Item("kalan") = th.kalan
        If th.kalan < 0 Then
            Ds.Tables(tahsilat).Rows(index).Item("paraustu") = th.kalan
            th.paraustu = th.kalan
        End If
        TbMakinaOrta.Text = ""
        Select Case Math.Round(th.kalan, 0)
            Case Is = 0
                TbToplam.Text = "Hesap Kapatıldı"
                Hesap = True
            Case Is > 0
                TbToplam.Text = fiyat_al(th.kalan)
            Case Is < 0
                TbToplam.Text = fiyat_al(th.kalan)
                Hesap = True
                If Reg.DisplayAktif Then SendSerialData("PARA ÜSTÜ:", fiyat_al(th.kalan) & "TL")
        End Select
    End Sub
    Dim Hesap As Boolean = False


    Private Sub ToplamAc()
        If SilAktif Then
            Dim ok As Boolean = False
            ok = Msg("Belge İptal Edilecektir!!", True, True, False)
            If ok Then
                hb.iptal = True
                SilAktif = False ' satiş yazdan sonra toplam ac var o yuzden sil aktıf yukarda olacak
                BtGridUrunDurum.Background = Brushes.Blue
                SatisYaz()
                Exit Sub
            Else
                Sil()
                SilAktif = False
                Exit Sub
            End If
        End If
        If BrDgSatis.Visibility = False Then
            TbMakinaOrta.Text = ""
            TbMakina.Text = ""
            If DgSatis.Items.Count > 0 Then
                TbToplam.Text = fiyat_al(hb.net_toplam)
                BrDgTahsilat.Visibility = Windows.Visibility.Visible
                GrTahsilat.Visibility = Windows.Visibility.Visible
                GrSatis.Visibility = Windows.Visibility.Hidden
                BrDgSatis.Visibility = Windows.Visibility.Hidden

                GrSatisMenuNumarator.Visibility = Windows.Visibility.Hidden
                GrTahsilatMenuNumarator.Visibility = Windows.Visibility.Visible

                TbUrunDurum.Text = fiyat_al(hb.net_toplam)
                TbYudeIndirim.Text = "%İndirim"
                TbIndirim.Text = "T.İNDİRİM"
                BtGridUrunDurum.Background = Brushes.SpringGreen
            End If
        Else
            BrDgTahsilat.Visibility = Windows.Visibility.Hidden
            BrDgSatis.Visibility = Windows.Visibility.Visible
            GrTahsilat.Visibility = Windows.Visibility.Hidden
            GrSatis.Visibility = Windows.Visibility.Visible


            GrSatisMenuNumarator.Visibility = Windows.Visibility.Visible
            GrTahsilatMenuNumarator.Visibility = Windows.Visibility.Hidden

            If hb.yuvarlama <> 0 Then
                hb.net_toplam = hb.net_toplam - hb.yuvarlama
                hb.yuvarlama = 0
            End If

            If hb.indirim_toplam <> 0 Then
                hb.indirim_toplam = hb.indirim_toplam
                hb.net_toplam = hb.net_toplam - hb.indirim_alt_toplam
                hb.indirim_alt_toplam = 0
            End If

            th = Nothing
            GridTemizleTahsilat()
            TbToplam.Text = fiyat_al(hb.net_toplam)
            Hesap = False
            TbMakinaOrta.Text = ""
            TbUrunDurum.Text = ""
            BtGridUrunDurum.Background = Brushes.Blue
        End If
        BtMakinaOrta.Focus()
        If Reg.DisplayAktif Then SendSerialData("TOPLAM", fiyat_al(hb.net_toplam) & "TL")
    End Sub

    Private Sub Yuvarla()
        If ay.yuvarlama_aktif = True Then
            If hb.yuvarlama = 0 Then
                ' hb.yuvarlama = Math.Round(hb.net_toplam, MidpointRounding.AwayFromZero)
                hb.yuvarlama = (Math.Floor(hb.net_toplam * 4) / 4) - hb.net_toplam
                hb.net_toplam = hb.net_toplam + hb.yuvarlama
                TbToplam.Text = fiyat_al(hb.net_toplam)
            Else
                hb.net_toplam = hb.net_toplam - hb.yuvarlama
                hb.yuvarlama = 0
                TbToplam.Text = fiyat_al(hb.net_toplam)
            End If
        End If
    End Sub



    '<TextBlock Name = "TbIndirim" Text="T.İNDİRİM"/>

    '<TextBlock Name = "TbYudeIndirim" Text="%İndirim" />

    Private Sub TutarToplamIndirim()
        If ay.indirim_aktif = True Then
            If hb.indirim_tutar_var = False Then
                If IsNumeric(TbMakinaOrta.Text) Then
                    If Convert.ToDecimal(TbMakinaOrta.Text) > hb.net_toplam Then
                        Msg("İndirim Satış Toplamından Büyük Olamaz", False, False, True)
                        TbMakinaOrta.Text = ""
                        Exit Sub
                    End If
                    hb.indirim_tutar_var = True
                    hb.indirim_tutar = Convert.ToDecimal(TbMakinaOrta.Text) * -1
                    hb.net_toplam = hb.net_toplam + hb.indirim_tutar
                    hb.indirim_alt_toplam += hb.indirim_tutar
                    TbToplam.Text = fiyat_al(hb.net_toplam)
                    TbMakinaOrta.Text = ""
                    TbIndirim.Text = "T.İNDİRİM"
                    TbIndirim.Text &= vbCrLf
                    TbIndirim.Text &= fiyat_al(hb.indirim_tutar * -1)
                End If
            Else
                hb.indirim_tutar_var = False
                hb.net_toplam = hb.net_toplam - hb.indirim_tutar
                hb.indirim_toplam = 0
                TbToplam.Text = fiyat_al(hb.net_toplam)
                TbIndirim.Text = "T.İNDİRİM"
            End If
        End If
    End Sub

    Private Sub TutarYuzdeIndirim()
        If ay.indirim_aktif = True Then
            If hb.indirim_yuzde_var = False Then
                If IsNumeric(TbMakinaOrta.Text) Then
                    If Convert.ToDecimal(TbMakinaOrta.Text) > 100 Then
                        Msg("İndirim %100 den Büyük Olamaz", False, False, True)
                        TbMakinaOrta.Text = ""
                        Exit Sub
                    End If
                    hb.indirim_yuzde_var = True
                    hb.indirim_yuzde_oran = Convert.ToDecimal(TbMakinaOrta.Text)
                    hb.indirim_yuzde = ((hb.net_toplam * hb.indirim_yuzde_oran) / 100) * -1

                    hb.indirim_alt_toplam += hb.indirim_yuzde
                    hb.net_toplam = hb.net_toplam + hb.indirim_yuzde
                    TbToplam.Text = fiyat_al(hb.net_toplam)
                    TbMakinaOrta.Text = ""
                    TbYudeIndirim.Text = "%" & hb.indirim_yuzde_oran & "İndirim"
                    TbYudeIndirim.Text &= vbCrLf
                    TbYudeIndirim.Text &= fiyat_al(hb.indirim_yuzde * -1)
                End If
            Else
                hb.indirim_yuzde_var = False
                hb.indirim_alt_toplam -= hb.indirim_yuzde
                hb.net_toplam -= hb.indirim_yuzde
                hb.indirim_yuzde_oran = 0
                hb.indirim_yuzde = 0
                TbToplam.Text = fiyat_al(hb.net_toplam)
                TbYudeIndirim.Text = "%İndirim"
            End If
        End If
    End Sub

    Dim TahsilatAcik As Boolean

    Private Sub TahsilatAc(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BrDgSatis.Visibility = Windows.Visibility.Hidden
        BrDgTahsilat.Visibility = Windows.Visibility.Visible
        TahsilatSatir(sender, e)
    End Sub

    Private Sub AdetYaz()
        If TbMakinaOrta.Text <> "" Then
            If IsNumeric(TbMakinaOrta.Text) Then
                If Convert.ToDecimal(TbMakinaOrta.Text) > 99 Then
                    Msg("Adet Miktarı Çok Yüksek!!!", False, False, True)
                    Exit Sub
                End If
                AdetAc(True)
                TbMakina.Text = TbMakinaOrta.Text
                TbMakinaOrta.Text = ""
            End If
        End If
        BtMakinaOrta.Focus()
    End Sub


    Private Sub BtSayac()
        PbPlu.Maximum = PluCount
        PbPlu.Minimum = 0
        PbPlu.Value = Plusayac
    End Sub

    Private Sub XRaporu()
        Dim CZraporu As New C_Zraporu
        CZraporu.KayitZraporu(False)
    End Sub

    Private Sub ZRaporu()
        Dim CZraporu As New C_Zraporu
        CZraporu.KayitZraporu(True)
    End Sub

    Private Sub Hata()

    End Sub


    Private Sub SatirYuzdeIndirim(sender As Object, e As RoutedEventArgs)
        hs.satir = DgSatis.SelectedIndex
        For Each dr As System.Data.DataRow In Ds.Tables(hareket_satir).Select("satir = '" & hs.satir & "'")
            HareketSatirOku(dr)
            If SilAktif = False Then
                If TbMakinaOrta.Text = "" Then
                    Exit Sub
                End If
                If IsNumeric(TbMakinaOrta.Text) = False Then
                    Msg("Geçerli Bir Değer Yazınız!!", False, False, True)
                    Exit Sub
                End If
                If DgSatis.Items.Count <= 0 Then Exit Sub
                If Convert.ToDecimal(TbMakinaOrta.Text) > 100 Then
                    Msg("İndirim %100 den Büyük Olamaz", False, False, True)
                    TbMakinaOrta.Text = ""
                    Exit Sub
                End If
                If hs.indirim_yuzde_oran > 0 Then
                    Msg("%" & hs.indirim_yuzde_oran & "indirim Yapılmış!", False, False, True)
                    Exit Sub
                End If
                If hs.indirim_sira = 0 Then
                    hs.indirim_sira = 1
                End If

                hs.indirim_yuzde_var = True
                hs.indirim_yuzde_oran = Convert.ToDecimal(TbMakinaOrta.Text)
                hs.indirim_yuzde = (hs.net_tutar * hs.indirim_yuzde_oran) / 100
                hs.indirim_satir_tutar += hs.indirim_yuzde
                hs.net_tutar = hs.net_tutar - hs.indirim_yuzde
            Else
                hs.indirim_yuzde_var = False
                hs.indirim_satir_tutar -= hs.indirim_yuzde
                hs.net_tutar += hs.indirim_yuzde
                hs.indirim_yuzde_oran = 0
                hs.indirim_yuzde = 0
                TbToplam.Text = fiyat_al(hs.net_tutar)
            End If
            hs.fiyat = hs.net_tutar / hs.miktar
            hs.kdv_tutar = hs.net_tutar - ((hs.net_tutar / (hs.kdv_oran + 100)) * 100)
            SatisDatasetYaz(hs.satir)
            SilAktif = False
        Next
        SatisGrid_Toplam()
        SatisGrid_Temizle()
    End Sub


    Private Sub SatirTutarIndirim(sender As Object, e As RoutedEventArgs)
        hs.satir = DgSatis.SelectedIndex
        For Each dr As System.Data.DataRow In Ds.Tables(hareket_satir).Select("satir = '" & hs.satir & "'")
            HareketSatirOku(dr)
            If SilAktif = False Then
                If TbMakinaOrta.Text = "" Then
                    Exit Sub
                End If
                If IsNumeric(TbMakinaOrta.Text) = False Then
                    Msg("Geçerli Bir Değer Yazınız!!", False, False, True)
                    Exit Sub
                End If
                If DgSatis.Items.Count <= 0 Then Exit Sub
                If Convert.ToDecimal(TbMakinaOrta.Text) > 100 Then
                    Msg("İndirim Tutardan Büyük Olamaz", False, False, True)
                    TbMakinaOrta.Text = ""
                    Exit Sub
                End If
                If hs.indirim_tutar > 0 Then
                    Msg(hs.indirim_tutar & "indirim Yapılmış!", False, False, True)
                    Exit Sub
                End If
                If hs.indirim_sira = 0 Then
                    hs.indirim_sira = 2
                End If

                hs.indirim_tutar_var = True
                hs.indirim_tutar = Convert.ToDecimal(TbMakinaOrta.Text)
                hs.indirim_satir_tutar += hs.indirim_tutar
                hs.net_tutar = hs.net_tutar - hs.indirim_tutar
            Else
                hs.indirim_tutar_var = False
                hs.indirim_satir_tutar -= hs.indirim_tutar
                hs.net_tutar = hs.net_tutar + hs.indirim_tutar
                hs.indirim_tutar = 0
                TbToplam.Text = fiyat_al(hs.net_tutar)
            End If
            hs.fiyat = hs.net_tutar / hs.miktar
            hs.kdv_tutar = hs.net_tutar - ((hs.net_tutar / (hs.kdv_oran + 100)) * 100)
            SatisDatasetYaz(hs.satir)
            SilAktif = False
        Next
        SatisGrid_Toplam()
        SatisGrid_Temizle()
    End Sub

    Dim aktarimOk As Boolean = True

    Private Sub CekmeceAc()

        RawPrinterHelper.SendStringToPrinter("fis" & hb.ref, Reg.Yazici, acil)

    End Sub

    Private Sub GecmisListele()
        If DgSatis.Items.Count > 0 Then
            Msg("Satiş İşlemini Bitiriniz!!!", False, False, True)
            Console.Beep(3000, 145)
            TbMakinaOrta.Text = ""
            Exit Sub
        End If
        Dim W As New W_Gecmis
        W.ShowDialog()
        DurumTemizle()
    End Sub

    Private Sub Etiketleme()
        If DgSatis.Items.Count > 0 Then
            Msg("Satiş İşlemini Bitiriniz!!!", False, False, True)
            Console.Beep(3000, 145)
            TbMakinaOrta.Text = ""
            Exit Sub
        End If
        Dim W As New W_Etiketleme
        W.ShowDialog()
        DurumTemizle()
    End Sub

    Private Sub EkranKilit()
        Me.Opacity = 0.5
        Dim W As New W_EkranKilit
        W.Main()
        Me.Opacity = 1
    End Sub

    Dim StatusBar As Boolean = False


    Private Sub FonksiyonUygula(ByVal Fnk As Integer, ByVal mesaj As Boolean)
        On Error GoTo 1

        Select Case Fnk
            Case "35"
                Select Case StatusBar
                    Case True
                        StatusBar = False
                        StatusBarKapat()
                        TimerCpu.Enabled = False
                        TimerCpu.Stop()
                    Case False
                        StatusBar = True
                        TimerRam.Interval = 1000
                        StatusBarAc()
                        TimerCpu.Interval = 1000


                        TimerCpu.Start()

                End Select

            Case "999"
                Cikis()
            Case "100"
                If Msg("Z Raporu Alınsın Mı?", True, True, False) = True Then
                    ZRaporu()
                End If
            Case "101"
                If Msg("X Raporu Alınsın Mı?", True, True, False) = True Then
                    XRaporu()
                End If
            Case "32"
                CekmeceAc()
            Case "30"
                EkranKilit()
            Case "20"
                GecmisListele()
            Case "22"
                Etiketleme()
            Case "200"
                Select Case aktarimOk
                    Case True
                        If TimerPlu.IsEnabled = False Then Exit Sub
                        PbPlu.Value = 0
                        TimerPlu.Stop()
                        aktarimOk = False
                        RtAnagrid.Stroke = Brushes.Red
                    Case False
                        If TimerPlu.IsEnabled = True Then Exit Sub
                        Plusayac = 0
                        PbPlu.Value = 0
                        TimerPlu.Start()
                        aktarimOk = True
                        RtAnagrid.Stroke = Brushes.RoyalBlue
                End Select
            Case "400"
                Select Case Reg.DisplayAktif
                    Case True
                        Reg.DisplayAktif = False
                        SendSerialData("**** GÖSTERGE ******", "****KAPATILDI******")
                    Case False
                        Reg.DisplayAktif = True
                        SendSerialData("**** GÖSTERGE ******", "***  AÇILDI  ******")
                End Select

            Case "500"
                Dim C As New Wpf_RaporSatisUrunler
                C.ShowDialog()
            Case "700"
                'If DgSatis.Items.Count > 0 Then
                ' Msg("Satışı Tamamlayınız!!", False, False, True)
                ' End If
                ' If DgTahsilat.Items.Count > 0 Then
                ' Msg("Satışı Tamamlayınız!!", False, False, True)
                ' End If

                'TimerPlu.Stop()
                'Dim W As New Wpf_PluTus
                'W.Main(Conn)
                'TimerPlu.Start()
            Case "800"
                TimerPlu.Stop()
                Dim W As New Wpf_ListeSayim
                W.ShowDialog()
                TimerPlu.Start()
            Case "137"
                aktarimOk = False
                TimerPlu.Stop()
                Dim C As New C_ChkDataLocalDb
                C.DropDatabase()


                'Dim Str As String = "MSSQLLocalDB"
                'Dim Proc As New System.Diagnostics.Process
                'Proc.StartInfo = New ProcessStartInfo("C:\Windows\System32\cmd.exe")
                'Proc.StartInfo.Arguments = "/C sqllocaldb\sqllocaldb p " & Str
                'Proc.StartInfo.RedirectStandardInput = True
                'Proc.StartInfo.RedirectStandardOutput = False
                'Proc.StartInfo.UseShellExecute = False
                'Proc.StartInfo.CreateNoWindow = False

                'Proc.Start()
                ' Allows script to execute sequentially instead of simultaneously
                'Proc.WaitForExit()
            Case "850"
                ' Değişim Fişi
                belgeturu = belge_turu.iade
            Case Else
                If mesaj Then Msg("Tanımsız Fonksiyon!!!", False, False, True)
        End Select
        Exit Sub
1:

    End Sub
    Private Sub Fonksiyon(sender As Object, e As RoutedEventArgs)
        If TbMakinaOrta.Text = "" Then
            Dim C As New WpfFonksiyon
            Dim Rtn As Integer = C.Main(aktarimOk)
            FonksiyonUygula(Rtn, False)
        Else
            If IsNumeric(TbMakinaOrta.Text) Then
                FonksiyonUygula(TbMakinaOrta.Text, True)
            End If
            TbMakinaOrta.Text = ""
        End If
        BtMakinaOrta.Focus()
    End Sub


    Private Sub GridTemizleSatis()

        Ds_read("select * from hareket_satir where ref = 0", hareket_satir, Conn, True, True)
        Ds_read("select * from hareket_baslik where ref = 0", hareket_baslik, Conn, True, True)
        DtS = Ds.Tables(hareket_satir)
        DvS.Table = DtS
        DgSatis.ItemsSource = DvS
    End Sub

    Private Sub GridTemizleTahsilat()

        Ds_read("select * from tahsilat where baslik = 0", tahsilat, Conn, True, True)
        DtT = Ds.Tables(tahsilat)
        DvT.Table = DtT
        DgTahsilat.ItemsSource = DvT
    End Sub


    Private Sub HesapKapat()

        For Each drT As DataRow In Ds.Tables(tahsilat).Rows
            th.baslik = hb.ref
            th.baslikguid = hb.guid
            th.createdate = hb.createdate
            '**********************************************
            '**********************************************
            th.guid = Guid.NewGuid.ToString
            '**********************************************
            '**********************************************
            th.sube = ay.sube
            th.kalan = Chk_Null("kalan", drT)
            th.odeme = Chk_Null("odeme", drT)
            th.odeme_kod = Chk_Null("odeme_kod", drT)
            th.para = Chk_Null("para", drT)
            th.tutar = Chk_Null("tutar", drT)
            th.paraustu = Chk_Null("paraustu", drT)
            th.paraustu = Chk_Null("paraustu", drT)

            Kayitsql(tahsilat, th, Conn, True)

            For Each drO As DataRow In Ds.Tables(odeme).Select("ref = '" & th.odeme & "'")
                od.kasadahil = Chk_Null("kasadahil", drO)
                od.bankadahil = Chk_Null("bankadahil", drO)
                od.caridahil = Chk_Null("caridahil", drO)

                If od.kasadahil Then
                    kh.kasa_islemi = True
                Else
                    kh.kasa_islemi = False
                End If
                If od.bankadahil Then
                    kh.banka_islemi = True
                    hb.ebelgegonderim = True
                    If hb.belgeturu = eFatura Then
                    Else
                        hb.belgeturu = eArsivFatura
                    End If
                Else
                    kh.banka_islemi = False
                End If
                If od.caridahil Then
                    kh.cari_islemi = True
                Else
                    kh.cari_islemi = False
                End If
                kh.baslik = th.baslik
                kh.baslikguid = hb.guid
                '***********************************************
                kh.guid = Guid.NewGuid.ToString
                '***********************************************
                kh.sube = ay.sube
                kh.kasa = pd.ref
                kh.kasa_kod = pd.kasa_kod
                kh.fisturu = kasa_fisleri.SatisFisi
                kh.fiskod = "Satis"
                kh.cari = hb.cari
                kh.cari_kod = hb.cari_kod
                kh.tutar = th.tutar + th.paraustu
                kh.tarih = hb.tarih
                kh.islemtarihi = Now
                kh.kasa_islemyonu = 1
                kh.cari_islemyonu = -1
                kh.banka_islemyonu = 1
                kh.iptal = False
                kh.iptal = False
                Kayitsql(kasa_hareket, kh, Conn, True)
            Next
        Next
    End Sub




    Private Function FaturaNo(belgeturu As String) As String
        DegiskenAl(pos, pd.ref, True)
        '──────────────────────────────────────────────────────────────────────────────────────
        Dim FtrNoStr As String = ""
        Dim FaturaSeri As String = ""
        Select Case belgeturu
            Case eFatura
                hb.belgeturu = eFatura
                FaturaSeri = pd.efaturaseri
                FtrNoStr = pd.efaturaserino
            Case eArsivFatura
                hb.belgeturu = eArsivFatura
                FaturaSeri = pd.earsivfaturaseri
                FtrNoStr = pd.earsivfaturaserino
            Case fis
                hb.belgeturu = fis
                FaturaSeri = pd.okcseri
                FtrNoStr = pd.okcserino
        End Select
        If FaturaSeri = "" Then
            FaturaSeri = "AKT"
        End If
        If FtrNoStr = "" Then
            FtrNoStr = FaturaSeri & hb.tarih.Year.ToString & "000000001"
        Else
            Dim FtrNoInt As Integer = Convert.ToInt64(FtrNoStr)
            FtrNoInt += 1
            FtrNoStr = StrDup(9 - FtrNoInt.ToString.Length, "0")
            FtrNoStr = FaturaSeri & hb.tarih.Year.ToString & FtrNoStr & FtrNoInt.ToString
        End If
        If FaturaNoSayacArttir(hb.belgeturu) Then
        End If
        Return FtrNoStr
    End Function

    Private Function FaturaNoSayacArttir(belgeturu As String) As Boolean
        Try
            Dim Sql As String = ""
            Sql = " Update Pos set "
            Select Case belgeturu
                Case eFatura
                    Sql &= " efaturaserino = efaturaserino + 1"
                Case eArsivFatura
                    Sql &= " earsivfaturaserino = earsivfaturaserino + 1"
                Case fis
                    Sql &= " okcserino = okcserino + 1"
            End Select
            Sql &= " Where ref = '" & pd.ref & "'"
            Execute_Run(Sql, Conn)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub SatisYaz()
        If DgSatis.Items.Count > 0 Then
            If hb.ref = 0 Then
                Try
                    hb.sayac = SayacAl()
                    hb.sayac += 1
                Catch ex As Exception
                End Try
                hb.guid = Guid.NewGuid.ToString


                hb.sube = ay.sube
                hb.suberef = ay.suberef

                Try
                    hb.createdate = Now
                    hb.ref = Kayitsql(hareket_baslik, hb, Conn, True)
                    HesapKapat()
                    SatisSatirYaz()
                Catch ex As Exception
                    Msg("Kayıt İşlemi Hatalı", False, False, True)
                    Exit Sub
                End Try
            Else
                hb.modifieddate = Now
                If UpdateSql(hareket_baslik, hb, Conn, True) = True Then
                    HesapKapat()
                    SatisSatirYaz()
                Else
                    Msg("Kayıt İşlemi Hatalı", False, False, True)
                    Exit Sub
                End If
            End If
            hb.faturano = FaturaNo(hb.belgeturu)
            Try
                Yazdir(True)
            Catch ex As Exception
            End Try

            If hb.aski = False Then
                If hb.iptal = False Then

                Else
                    '  If Ds_read("select * from urunler where iptal <> 1 order by ref", Urunler, Conn, True, True) Then
                    ' End If
                End If
            End If
        End If

        DurumTemizle()
        If aktarimOk Then TimerPlu.Start()
        ServisSatisyazThr()
    End Sub

    Friend Sub ServisSatisyazThr()
        Dim trd As Thread = Nothing
        trd = New Thread(AddressOf MerkezSatisYaz) With {
            .IsBackground = True
        }
        trd.Start()
    End Sub


    Private Sub Yazdir(ByVal fatura As Boolean)

        Select Case Reg.FisBoyutu
            Case "80mm"
                If fatura Then
                    Dim C_prn As New Class_Print80mm
                    C_prn.FisPrint80mm(Reg.Yazici)
                Else
                    Dim C_prn As New Class_Print
                    C_prn.FisPrint(Reg.Yazici)
                End If
            Case "58mm"
                Dim C_prn As New Class_Print58mm
                C_prn.FisPrint58mm(Reg.Yazici)
            Case Else
                Dim C_prn As New Class_Print58mm
                C_prn.FisPrint58mm(Reg.Yazici)
                ' Msg("Fiş boyutunu Şeciniz!!", False, False, True)
                ' Exit Sub
        End Select
        hb.dokum = True
        hb.dokum_sayisi += 1
        Execute_run("update hareket_baslik set dokum_sayisi = '" & hb.dokum_sayisi & "', dokum = 1 where ref = '" & hb.ref & "'", Conn, True)
    End Sub



    Private Sub Satis_Durum_Yaz()
        On Error Resume Next
        TbUrunDurum.Text = ""
        Dim Txt As String = ""
        Txt = adet_al(HsSonIslem.Miktar) & " " & hs.urun_aciklama & "  Toplam: " & adet_al(hs.miktar) & hs.birim_alt_isaret
        If hs.indirim_yuzde_oran > 0 Then
            Txt &= "  %İndirim: " & hs.indirim_yuzde_oran
        End If
        TbUrunDurum.Text = Txt
        If SilAktif Then
            BtGridUrunDurum.Background = Brushes.Red
        Else
            BtGridUrunDurum.Background = Brushes.Blue
        End If
        If Reg.DisplayAktif Then SendSerialData(hs.urun_aciklama, hs.miktar & hs.birim_alt_isaret & " Tutar:" & fiyat_al(hs.net_tutar) & "TL")
    End Sub

    Private Sub DurumTemizle()
        On Error Resume Next
        GridTemizleSatis()
        GridTemizleTahsilat()
        hb = Nothing
        hs = Nothing
        th = Nothing
        Ind = Nothing
        cr = Nothing
        TbUrunDurum.Text = ""
        TbToplam.Text = ""
        TbMakinaOrta.Text = ""
        TbMakina.Text = ""
        ToplamAc()

        BrGridStokDurum.Background = Brushes.Blue
        TbStokDurum.Background = Brushes.WhiteSmoke
        TbStokDurum.Text = ""
        TbGrupDurum.Text = ""
        If Reg.DisplayAktif Then SendSerialData(" ", " ")
    End Sub

    Dim Renk_Kayit As Brush = Brushes.LawnGreen
    Dim Renk_Fis As Brush = Brushes.Salmon
    Dim Renk_Cikis As Brush = Brushes.Red

    Private Function UrunListele(ByVal GrGuid As String) As Grid
        'On Error Resume Next
        Dim Grurunler As New Grid
        Grurunler.Children.Clear()
        Grurunler.RowDefinitions.Clear()
        Grurunler.ColumnDefinitions.Clear()

        Gr.satir = 0
        Gr.sutun = 0
        For Each dr As System.Data.DataRow In Ds.Tables(Grup).Select("guid = '" & GrGuid & "'")
            Gr.satir = Chk_Null("satir", dr)
            Gr.sutun = Chk_Null("sutun", dr)
        Next
        If Gr.satir = 0 Or Gr.sutun = 0 Then
            Msg("Satır ve Sütun Değerlerini Giriniz!!", False, False, True)
            Return Nothing
        End If

        For x As Integer = 0 To Gr.satir - 1
            Dim GrRowDef As New RowDefinition
            Grurunler.RowDefinitions.Add(GrRowDef)
        Next

        For y As Integer = 0 To Gr.sutun - 1
            Dim GrColDef As New ColumnDefinition
            Grurunler.ColumnDefinitions.Add(GrColDef)
        Next
        Dim sayac As Integer = 0


        For Each dr As System.Data.DataRow In Ds.Tables(Urunler).Select("urunozellik = 1 and urunozellikiptal <> 1 and grupguid = '" & GrGuid & "' and urunozelliksablon = '" & Kl.plutus & "' and urunozellikx > 0 and urunozelliky > 0")
            Dim Uuid As String = dr.Item("urunguid")
            Dim ref As Long = dr.Item("urunref")

            Dim Fiyat As Decimal = 0
            Dim Stok As Decimal = 0
            Dim UrunStokTakibi As Boolean = Chk_Null("urunstoktakip", dr)
            'Dim Birim_Ust As Integer = Chk_Null("birimustref", dr)
            Stok = Chk_Null("urunstokmiktar", dr)
            Fiyat = dr.Item("urunfiyat")

            If Fiyat <> 0 Then
                Dim Aciklama As String = dr.Item("urunaciklama")
                Dim X As Integer = dr.Item("urunozellikx")
                Dim Y As Integer = dr.Item("urunozelliky")
                Dim fiyat_str As String = fiyat_al(Fiyat).ToString
                If Aciklama <> "" And X <> 0 And Y <> 0 Then

                    Dim FontSize As Integer = Chk_Null("urunozellikfontsize", dr)
                    If FontSize = 0 Then FontSize = 15
                    Dim En As Integer = Chk_Null("urunozelliken", dr)
                    If En = 0 Then En = 90
                    Dim sigdir As String = Chk_Null("urunozelliksigdir", dr)
                    Dim Renk As String = ""
                    Renk = Chk_Null("urunozellikrenk", dr)

                    Dim GrUrun As New Grid

                    GrUrun.Uid = Ref
                    GrUrun.Margin = New Thickness(2)
                    GrUrun.Background = Brushes.Navy

                    Dim GrRdBt As New RowDefinition
                    GrUrun.RowDefinitions.Add(GrRdBt)
                    GrRdBt.Height = New GridLength(5, GridUnitType.Star)
                    Dim Bt As New System.Windows.Controls.Button
                    Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
                    Bt.Uid = Ref
                    Dim style As New Style
                    style = FindResource("RoundedButton")
                    'style = FindResource("MetroButton")
                    ' style = FindResource("BlueGlassButtonStyle")
                    Bt.Style = style
                    Bt.Margin = New Thickness(1, 1, 1, 1)
                    Dim color_ As Color
                    If Renk <> "" Then
                        color_ = DirectCast(ColorConverter.ConvertFromString(Renk), Color)
                        Bt.Background = New SolidColorBrush(color_)
                    Else : Bt.Background = Brushes.LightGray
                    End If
                    '************** olaylar ***************************
                    AddHandler Bt.Click, AddressOf UrunEkle
                    '**************************************************
                    Dim VbBt As New Viewbox
                    Dim TbBt As New TextBlock
                    TbBt.Width = En
                    TbBt.FontSize = FontSize
                    TbBt.TextWrapping = TextWrapping.Wrap
                    TbBt.Text = Aciklama
                    VbBt.Child = TbBt
                    VbBt.Margin = New Thickness(1, 1, 1, 1)
                    Bt.Content = VbBt


                    If En = 0 Or FontSize = 0 Then
                        Dim EnUrunButon As Integer = Grurunler.RenderSize.Width
                        EnUrunButon = EnUrunButon - ((Gr.satir - 1) * 2)
                        EnUrunButon = EnUrunButon / Gr.satir
                        TbBt.Width = EnUrunButon / 2
                        VbBt.Stretch = Stretch.Uniform
                    Else
                        VbBt.Width = En
                        TbBt.Width = En
                        TbBt.FontSize = FontSize
                        If sigdir Then
                            VbBt.Stretch = Stretch.Uniform
                        Else
                            VbBt.Stretch = Stretch.None
                        End If
                    End If

                    Grid.SetRow(Bt, 0)
                    GrUrun.Children.Add(Bt)

                    If ay.fiyatgoster_aktif Then
                        Dim GrRdFy As New RowDefinition
                        GrUrun.RowDefinitions.Add(GrRdFy)
                        GrRdFy.Height = New GridLength(1, GridUnitType.Star)
                        Dim VbFy As New Viewbox
                        VbFy.HorizontalAlignment = Windows.HorizontalAlignment.Right
                        VbFy.Stretch = Stretch.Uniform
                        Dim TbFy As New TextBlock
                        TbFy.HorizontalAlignment = Windows.HorizontalAlignment.Left
                        TbFy.Foreground = Brushes.WhiteSmoke
                        TbFy.Text = "₺" & fiyat_str
                        VbFy.Child = TbFy
                        VbFy.Margin = New Thickness(1, 1, 1, 1)
                        Grid.SetRow(VbFy, 1)
                        GrUrun.Children.Add(VbFy)
                    End If
                    If UrunStokTakibi Then
                        ay.stokgoster_aktif = False
                        If ay.stokgoster_aktif Then
                            If ay.stokgoster_aktif Then
                                Dim bolen As Integer = 0
                                Dim carpan As Integer = 0
                                bolen = dr.Item("birimaltbolen")
                                carpan = dr.Item("birimaltcarpan")
                                Try
                                    Stok = (Stok * bolen) / carpan
                                Catch ex As Exception
                                End Try


                                Dim GrRdSt As New RowDefinition
                                GrUrun.RowDefinitions.Add(GrRdSt)
                                GrRdSt.Height = New GridLength(1, GridUnitType.Star)
                                Dim VbSt As New Viewbox
                                VbSt.Uid = Ref
                                VbSt.HorizontalAlignment = Windows.HorizontalAlignment.Right
                                VbSt.Stretch = Stretch.Uniform
                                Dim TbSt As New TextBlock
                                TbSt.Uid = Ref
                                TbSt.HorizontalAlignment = Windows.HorizontalAlignment.Left
                                TbSt.Foreground = Brushes.WhiteSmoke
                                TbSt.Text = "Stok:" & adet_al(Stok).ToString & TbMakinaOrta.Text

                                VbSt.Child = TbSt
                                VbSt.Margin = New Thickness(1, 1, 1, 1)
                                Grid.SetRow(VbSt, 2)
                                GrUrun.Children.Add(VbSt)
                            End If
                        End If
                    End If

                    Grid.SetRow(GrUrun, X - 1)
                    Grid.SetColumn(GrUrun, Y - 1)
                    Grurunler.Children.Add(GrUrun)
                    sayac += 1
                End If

            End If
        Next
        Return Grurunler
    End Function


    Private Sub GrupListele()
        '  On Error Resume Next
        Dim ilkref As Integer = 0
        Dim CountGrup As Integer = 0
        Ds_read("select * from grup order by sira", Grup, True, True)
        CountGrup = Ds.Tables(Grup).Select("goster <> 0 and iptal <> 1 and active = 1").Count
        If CountGrup = 0 Then Exit Sub
        'ReadQueryView("select * from v_satisurunler where urunozellikguid is not null order by grupsira")

        '  Dim EnGrupButon As Integer = TcUrunler.RenderSize.Width
        '  EnGrupButon = EnGrupButon - ((CountGrup - 3) * 2)
        '  EnGrupButon = EnGrupButon / CountGrup
        Dim TcUrunler As New TabControl
        Dim HeigtTc As Integer = 0

        HeigtTc = BrTeraziPasif.RenderSize.Height
        TcUrunler = TcGrUrunler

        HeigtTc = HeigtTc / CountGrup

        TcUrunler.Items.Clear()


        For Each drGrup As System.Data.DataRow In Ds.Tables(Grup).Select("goster <> 0 and iptal <> 1 and active = 1")
            Dim Tb As New TextBlock
            ' Tb.Width = EnGrupButon
            Dim Aciklama As String = drGrup.Item("aciklama")
            With Tb
                .HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                .VerticalAlignment = Windows.VerticalAlignment.Stretch
                .TextWrapping = TextWrapping.Wrap
                .Text = Aciklama
            End With

            Dim Vb As New Viewbox
            With Vb
                .HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                .VerticalAlignment = Windows.VerticalAlignment.Stretch
                .Stretch = Stretch.Uniform
                .Child = Tb
                .Margin = New Thickness(3, 3, 3, 3)
            End With

            Dim Grupref As Int64 = drGrup.Item("ref")
            Dim GrupGuid As String = drGrup.Item("guid")
            Dim Tbitem As New TabItem

            With Tbitem
                .Header = Vb
                .Height = HeigtTc
                .Width = 120
                .Uid = Grupref
            End With

            If ilkref = 0 Then
                ilkref = Grupref
                BtGrup.Background = Brushes.Lime
            End If

            Dim GrTab As New Grid
            Dim Satir As Int64 = drGrup.Item("satir")
            Gr.satir = Satir
            Dim Sutun As Int64 = drGrup.Item("sutun")
            Gr.sutun = Sutun
            '    GrTab.Children.Add(UrunListele(Gr.satir, Gr.sutun, Grupref))
            GrTab.Children.Add(UrunListele(GrupGuid))
            Tbitem.Content = GrTab
            TcUrunler.Items.Add(Tbitem)
        Next
    End Sub

    Dim Grupref As Integer = 0
    Dim BtGrup As New System.Windows.Controls.Button

    Private Sub GrupSecim(ByVal sender As Object, ByVal e As System.EventArgs)
        BtGrup.Background = Brushes.LightGray
        Dim Bt As New System.Windows.Controls.Button
        Bt = sender
        BtGrup = Bt
        BtGrup.Background = Brushes.Lime
        Grupref = Bt.Name
        '   UrunListele(Grupref)
    End Sub


    Private Sub BtMakinaTemizle()
        TbMakina.Text = ""
        AdetAc(False)
    End Sub



    Private Sub Cikis()
        '    If trdPlu.IsAlive Then
        ' Msg("Aktarım İşlemi Devam Ediyor!!!", False, False, True)
        ' Exit Sub
        ' End If
        If DgSatis.Items.Count > 0 Then
            Msg("Satiş İşlemini Bitiriniz!!!", False, False, True)
            Console.Beep(3000, 145)
            TbMakinaOrta.Text = ""
            Exit Sub
        End If
        TimerRam.Stop()
        TimerPlu.Stop()
        TimerBizerba.Stop()
        BilgisayarAktif(0, Conn)
        Me.Close()
    End Sub

    Dim BtnUrun As New System.Windows.Controls.Button
    Dim RenkUrun As Brush

    Dim BirimRef As Integer = 0
    Dim Birim_Ust As Integer = 0


    Private Sub FlagKontrol()
        fl = Nothing
        For Each drflag As System.Data.DataRow In Ds.Tables(flag).Rows
            Dim Brfl As String = drflag.Item("flag")
            Dim Flg As String = Mid(Br.barkod, 1, Brfl.Length)
            If Flg = Brfl Then
                fl.flag = Brfl
                fl.barkod_baslangic = drflag.Item("barkod_baslangic")
                fl.barkod_uzunluk = drflag.Item("barkod_uzunluk")
                fl.data_baslangic = drflag.Item("data_baslangic")
                fl.data_uzunluk = drflag.Item("data_uzunluk")
                fl.carpan = drflag.Item("carpan")
                fl.bolen = drflag.Item("bolen")
                If fl.data_uzunluk = 0 Then Exit Sub

                If Br.barkod.Length > fl.flag.Length + fl.barkod_uzunluk + fl.data_uzunluk Then
                    hs.miktar = fl.carpan * (Mid(Br.barkod, fl.data_baslangic, fl.data_uzunluk)) / fl.bolen
                    Br.barkod = Flg & Mid(Br.barkod, fl.barkod_baslangic, fl.barkod_uzunluk)
                End If
                Exit For
            End If
        Next
    End Sub


    Dim SilAktif As Boolean = False

    Private Sub Sil()
        If hs.ozelfiyat = False Then
            If DgSatis.Items.Count > 0 Then
                If SilAktif Then
                    TbUrunDurum.Text = ""
                    BtGridUrunDurum.Background = Brushes.Blue
                    SilAktif = False
                Else
                    TbUrunDurum.Text = "SİL"
                    BtGridUrunDurum.Background = Brushes.Red
                    SilAktif = True
                End If
            Else
                Msg("Satış Yapınız!!", False, False, True)
            End If
        Else
            Msg("Özel Fiyat Aktif", False, False, True)
        End If
        BtMakinaOrta.Focus()
    End Sub


    Private Sub OzelFiyat()
        If FiyatGorAktif = True Then
            Msg("Fiyat Gör Aktif", False, False, True)
            Exit Sub
        End If
        If SilAktif = False Then
            If hs.ozelfiyat = True Then
                hs.ozelfiyat = False
                BtGridUrunDurum.Background = Brushes.Blue
                TbUrunDurum.Text = ""
                Exit Sub
            Else
                Try
                    If IsNumeric(TbMakinaOrta.Text) Then
                        TbUrunDurum.Text = fiyat_al(Convert.ToDecimal(TbMakinaOrta.Text))
                        hs.ozelfiyat = True
                        hs.fiyat = Convert.ToDecimal(TbMakinaOrta.Text)
                        BtGridUrunDurum.Background = Brushes.Yellow
                        TbMakinaOrta.Text = ""
                    End If
                Catch ex As Exception
                End Try
            End If
        Else
            Msg("Sil Aktif", False, False, True)
        End If
        BtMakinaOrta.Focus()
    End Sub


    Private Sub UrunEkleBarkod()
        If BrDgSatis.Visibility = Windows.Visibility.Visible Then
            If TbMakinaOrta.Text <> "" Then
                Br.barkod = TbMakinaOrta.Text
            Else
                Exit Sub
            End If
            FlagKontrol()
            ReadQueryView("select * from v_satisurunler where barkodlarbarkod = '" & Br.barkod & "'")
            Ur.ref = 0
            Ur.guid = ""
            Br.birim_alt = 0
            For Each drBarkod As System.Data.DataRow In Ds.Tables(Urunler).Select("barkodlarbarkod = '" & Br.barkod & "'")
                Ur.ref = Chk_Null("urunref", drBarkod)
                Ur.guid = Chk_Null("urunguid", drBarkod)
                Br.birim_alt = Chk_Null("birimaltref", drBarkod)
            Next
            If FiyatGorAktif Then
                Dim F As New WpfFiyatGor
                Dim Aktar As Boolean = False
                Aktar = F.main()
                FiyatGor()
                TbMakinaOrta.Text = ""
                If Aktar = True Then
                    Satis_grid(Ur.guid, Br.birim_alt)
                End If
            Else
                Satis_grid(Ur.guid, Br.birim_alt)
            End If
        Else
            TbMakina.Text = ""
        End If
        BtMakinaOrta.Focus()
    End Sub



    Private Sub StokYaz(ByVal UrunRef As Integer, ByVal BirimAlt As Integer)


    End Sub

    Private Function Satisgrid_UrunDetay() As Boolean
        For Each dr As DataRow In Ds.Tables(Urunler).Select("urunguid = '" & Ur.guid & "'")
            ' Ur.stok_miktar = Chk_Null("urunstokmiktar", dr)
            hs.brut_fiyat = Chk_Null("urunfiyat", dr)
            hs.urun_aciklama = Chk_Null("urunaciklama", dr)
            hs.birim_ust = Chk_Null("birimustref", dr)
            If hs.barkod = "" Then hs.barkod = Chk_Null("barkodlarbarkod", dr)
            If hs.ozelfiyat = False Then
                hs.fiyat = Chk_Null("urunfiyat", dr)
            Else
                hs.brut_fiyat = hs.fiyat
            End If
            hs.kdv_oran = Chk_Null("kdvoran", dr)
            hs.stokkodu = Chk_Null("urunlerstokkodu", dr)
            If hs.fiyat <= 0 Then
                Msg("Fiyat Hatalı", False, False, True)
                Return False
            End If
            Ba.kesirli = dr.Item("birimaltkesirli")
            hs.carpan = Chk_Null("birimaltcarpan", dr)
            hs.bolen = Chk_Null("birimaltbolen", dr)
            hs.birim_alt_isaret = Chk_Null("birimaltisaret", dr)
            Select Case hs.birim_alt_isaret
                Case "Ad"
                    hs.birim_sira = 0
                Case "Kg"
                    hs.birim_sira = 1
            End Select
            If fl.flag <> "" Then
                hs.miktar = (hs.miktar / hs.bolen) * hs.carpan
                If hs.miktar <> 0 Then
                    TbMakina.Text = hs.miktar
                Else
                    If IsNumeric(TbMakina.Text) Then
                        hs.miktar = TbMakina.Text
                    End If
                End If
            End If
            If Ba.kesirli = False Then
                If IsNumeric(TbMakina.Text) Then
                    Dim MiktarNet As Decimal = Convert.ToDecimal(TbMakina.Text)
                    ' Eğer Miktar küsüratlı ve birim Adetli ise
                    MiktarNet = MiktarNet - ((MiktarNet) - (MiktarNet Mod 1))
                    If MiktarNet > 0 Then
                        Msg("Miktar Kesirli Olamaz!!", False, False, True)
                        Return False
                    End If
                End If
            End If
        Next

        Return True
    End Function

    Private Function SatisGrid_Miktar() As Decimal
        On Error Resume Next
        Dim Miktar As Decimal
        Miktar = Convert.ToDecimal(TbMakina.Text)
        If Miktar = 0 Then Miktar = 1
        Return Miktar
    End Function

    Private Sub SatisGrid_HareketBaslik()
        hb.sube = ay.sube
        hb.suberef = ay.suberef
        hb.upload = False
        hb.sayac += 1
        hb.kullanici = Kl.ref
        hb.kullanici_kod = Kl.kod
        hb.kasa = pd.ref
        hb.kasa_kod = pd.kasa_kod
        hb.tarih = Now
        hb.islem_tarihi = Now
        hb.islemyonu = -1
        hb.ebelgegonderim = False
        hb.ebelgeonay = False
        If hb.belgeturu = eFatura Or hb.belgeturu = eArsivFatura Then
        Else
            hb.belgeturu = "fis"
            hb.efaturaturu = belge_turu.fis
        End If
        '   hb.gnfisturu = GnFis_Turu.SatisFisi
        hb.fisturu = stok_fisleri.SatisFisi
        hb.fiskod = "Satis"
        hb.kdv_dahil = True
        hb.iptal = False
        hb.aski = False
        hb.guid = Guid.NewGuid.ToString
        hb.mac = pd.macadress
        hb.gunsonu = False
    End Sub

    Private Function SatisGrid_UrunEkle(ByVal UrunGuid As String,
                                   ByVal BirimAlt As Integer,
                                   ByVal miktar As Decimal) As Boolean
        For Each dr As System.Data.DataRow In Ds.Tables(hareket_satir).Select("iptal <> 1 and urunguid = '" & UrunGuid & "' and birim_alt = '" & BirimAlt & "'")
            hs.satir = Ds.Tables(hareket_satir).Rows.IndexOf(dr)
            hs.urunguid = Chk_Null("urunguid", dr)
            hs.urun_aciklama = Chk_Null("urun_aciklama", dr)
            hs.barkod = Chk_Null("barkod", dr)
            hs.ozelfiyat = Chk_Null("ozelfiyat", dr)
            hs.fiyat = Chk_Null("fiyat", dr)
            hs.brut_fiyat = Chk_Null("fiyat", dr)
            hs.kdv_dahil = True
            dr.Item("kdv_dahil") = hs.kdv_dahil
            hs.kdv_oran = Chk_Null("kdv_oran", dr)
            If SilAktif = True Then miktar = miktar * -1
            hs.miktar = dr.Item("miktar") + miktar
            If hs.miktar > 0 Then
                hs.iptal = False
            Else
                hs.iptal = True
                hs.miktar = 0
            End If
            hs.islemyonu = dr.Item("islemyonu")
            If hs.ozelfiyat = True And hs.fiyat > 0 Then
            Else
                hs.fiyat = dr.Item("fiyat")
            End If
            '******************************************************
            dr.Item("brut_tutar") = hs.miktar * hs.fiyat
            dr.Item("net_tutar") = hs.miktar * hs.fiyat
            '******************************************************
            hs.birim_alt_isaret = dr.Item("birim_alt_isaret")
            hs.carpan = dr.Item("carpan")
            hs.bolen = dr.Item("bolen")
            'hs.iptal = dr.Item("iptal")****************************
            hs.indirim_yuzde_oran = dr.Item("indirim_yuzde_oran")
            hs.indirim_yuzde = dr.Item("indirim_yuzde")
            hs.indirim_tutar = dr.Item("indirim_tutar")

            hs.indirim_tutar_var = dr.Item("indirim_tutar_var")
            hs.indirim_yuzde_var = dr.Item("indirim_yuzde_var")

            hs.indirim_satir_tutar = dr.Item("indirim_satir_tutar")
            hs.satir = dr.Item("satir")
            hs.birim_alt = dr.Item("birim_alt")
            hs.birim_ust = dr.Item("birim_ust")


            If ay.indirim_aktif = True Then
                If hs.indirim_satir_tutar = 0 Then
                Else
                    hs.net_tutar = hs.net_tutar + hs.indirim_satir_tutar
                    hs.indirim_satir_tutar = 0
                    hs.indirim_yuzde_oran = 0
                    TbToplam.Text = fiyat_al(hs.net_tutar)
                End If
            End If
            hs.kdv_tutar = hs.net_tutar - ((hs.net_tutar / (hs.kdv_oran + 100)) * 100)

            Return False
        Next
        Return True
    End Function


    Private Sub SatisGrid_StokGoster(ByVal UrunGuid As String,
                                     ByVal BirimAlt As Integer,
                                     ByVal miktar As Decimal)
        For Each drBAlt As System.Data.DataRow In Ds.Tables(birim_alt).Select("ref = '" & BirimAlt & "'")
            hs.carpan = drBAlt.Item("carpan")
            hs.bolen = drBAlt.Item("bolen")
            hs.birim_ust = drBAlt.Item("ref_ust")
            Ur.stok_miktar = Ur.stok_miktar - ((miktar * hs.carpan) / hs.bolen)
            For Each dr_Urun As System.Data.DataRow In Ds.Tables(Urunler).Select("guid = '" & UrunGuid & "'")
                Dim AsgariStok As Decimal = Chk_Null("asgari_stok", dr_Urun)
                dr_Urun.Item("stok_miktar") = Ur.stok_miktar
                Dim TbMiktar As Decimal = (Ur.stok_miktar * hs.bolen) / hs.carpan
                If TbMiktar > AsgariStok Then
                    BrGridStokDurum.Background = Brushes.Lime
                Else
                    BrGridStokDurum.Background = Brushes.Pink
                End If
                If TbMiktar < 0 Then
                    BrGridStokDurum.Background = Brushes.Red
                End If
                TbGrupDurum.Text = hs.urun_aciklama
                TbStokDurum.Text = "Stok:" & adet_al(TbMiktar).ToString & hs.birim_alt_isaret
            Next
        Next
    End Sub

    Private Sub SatisGrid_Toplam()
        Try
            DgSatis.Focus()
            Dim Obj = DgSatis.Items(hs.satir)
            DgSatis.ScrollIntoView(Obj)
            DgSatis.SelectedItem = Obj
            DgSatis.SelectedItem("ozelfiyat") = hs.ozelfiyat
        Catch ex As Exception
        End Try
        DgSatis.ItemsSource = DvS
        Try
            If DvS.Table.Compute("Sum(net_tutar)", "iptal <> 1") IsNot DBNull.Value Then
                'hb.net_toplam = fiyat_al(DvS.Table.Compute("Sum(net_tutar)", "iptal <> 1"))
                hb.net_toplam = DvS.Table.Compute("Sum(net_tutar)", "iptal <> 1")
            Else
                hb.net_toplam = 0
            End If
            If DvS.Table.Compute("Sum(brut_tutar)", "iptal <> 1") IsNot DBNull.Value Then
                hb.brut_toplam = fiyat_al(DvS.Table.Compute("Sum(brut_tutar)", "iptal <> 1"))
            Else
                hb.brut_toplam = 0
            End If
            If DvS.Table.Compute("Sum(kdv_tutar)", "iptal = 0") IsNot DBNull.Value Then
                ' hb.kdv_toplam = fiyat_al(DvS.Table.Compute("Sum(kdv_tutar)", "iptal = 0"))
                hb.kdv_toplam = DvS.Table.Compute("Sum(kdv_tutar)", "iptal <> 1")
            Else
                hb.kdv_toplam = 0
            End If
            hb.indirim_satir_toplam = hb.net_toplam - hb.brut_toplam
            '            hb.indirim_toplam = hb.net_toplam * (hb.indirim_yuzde / 100)
            hb.indirim_toplam = hb.indirim_alt_toplam + hb.indirim_satir_toplam

            '  hb.net_toplam = hb.net_toplam - hb.indirim_toplam
            hb.tarih = Now
            hb.islem_tarihi = Now
            TbToplam.Text = fiyat_al(hb.net_toplam)
        Catch ex As Exception
        End Try
        DgSatis.UpdateLayout()
        Satis_Durum_Yaz()
    End Sub

    Private Sub SatisGrid_Temizle()
        TbMakina.Text = ""
        TbMakinaOrta.Text = ""
        AdetAc(False)
        HsSonIslem.UrunGuid = hs.urunguid
        HsSonIslem.BirimAlt = hs.birim_alt
        hs = Nothing
        Ur = Nothing
        Ba = Nothing
        '   Fy = Nothing
        Br = Nothing
        SilAktif = False
        'Console.Beep(5000, 50)
    End Sub




    Private Sub Satis_grid(ByVal UrunGuid As String,
                           ByVal BirimAlt As Integer)

        '  Console.Beep(3800, 100)
        SystemSounds.Beep.Play()


        TimerPlu.Stop()

        If belgeturu = belge_turu.iade Then
            Dim a = 0
        End If

        HsSonIslem = Nothing
        If UrunGuid = "" Then
            Console.Beep(3000, 450)
            Msg("Ürün Hatalı", False, False, True)
            TbMakinaOrta.Text = ""
            Exit Sub
        End If
        If BirimAlt = 0 Then
            SystemSounds.Beep.Play()
            Msg("Birim Hatalı", False, False, True)
            Exit Sub
        End If
        Ur.guid = UrunGuid
        If Satisgrid_UrunDetay() = False Then Exit Sub
        Dim miktar As Decimal = SatisGrid_Miktar()
        hs.satir = DgSatis.Items.Count
        If hb.tarih = Nothing Then
            SatisGrid_HareketBaslik()
        End If
        hb.satir_sayisi = hs.satir
        HsSonIslem.Miktar = miktar
        Dim yeni_urun As Boolean = True
        If SilAktif = True Then
            yeni_urun = SatisGrid_UrunEkle(UrunGuid, BirimAlt, miktar)
        End If

        If yeni_urun = True And SilAktif = True Then
            Msg("Satırlarla Tutarsız!!", False, False, True)
            Exit Sub
        End If
        If yeni_urun And SilAktif = False Then
            hs.urunguid = Ur.guid
            hs.guid = Guid.NewGuid.ToString
            hs.baslikguid = hb.guid
            hs.mac = hb.mac
            hs.miktar = miktar
            hs.brut_tutar = miktar * hs.brut_fiyat
            hs.net_tutar = miktar * hs.fiyat
            hs.kdv_tutar = hs.net_tutar - ((hs.net_tutar / (hs.kdv_oran + 100)) * 100)
            hs.islemyonu = -1
            hs.birim_alt = BirimAlt
            If hs.barkod = "" Then hs.barkod = Br.barkod
            hs.createdate = Now
            Ds.Tables(hareket_satir).Rows.Add()
        End If
        If ay.stokgoster_aktif Then SatisGrid_StokGoster(UrunGuid, BirimAlt, miktar)
        SatisDatasetYaz(hs.satir)
        SatisGrid_Toplam()
        SatisGrid_Temizle()
    End Sub

    Private Structure HsSonIslem_Degisken
        Dim UrunGuid As String
        Dim BirimAlt As Integer
        Dim Miktar As Decimal
    End Structure
    Dim HsSonIslem As HsSonIslem_Degisken

    Private Sub Sesuyari()
        Console.Beep(5000, 100)
    End Sub

    Private Sub Aski(sender As Object, e As RoutedEventArgs)
        Dim count As Integer = DgSatis.Items.Count
        If count <= 0 Then
            Ds_read("select ref,sayac,tarih from hareket_baslik where aski = 1 and iptal <> 1 order by ref asc", "aski", Conn, True, True)
            If Ds.Tables("aski").Rows.Count > 0 Then
                Dim F As New WpfAskiListe
                F.ShowDialog()
                If hb.ref <> 0 Then
                    DegiskenAl(hareket_baslik, hb.ref, True)
                    Ds_read("select * from hareket_satir where baslik = '" & hb.ref & "'", hareket_satir, Conn, True, True)
                    For Each dr As DataRow In Ds.Tables(hareket_baslik).Rows
                        dr.Item("aski") = False
                        dr.Item("upload") = False
                        hb.aski = False
                        hb.upload = False
                    Next
                    DtS = Ds.Tables(hareket_satir)
                    DvS.Table = DtS
                    DgSatis.ItemsSource = DvS
                    Try
                        hb.net_toplam = fiyat_al(DvS.Table.Compute("Sum(net_tutar)", "iptal = 0"))
                        hb.indirim_toplam = hb.net_toplam * (hb.indirim_yuzde / 100)
                        hb.net_toplam = hb.net_toplam - hb.indirim_toplam
                        hb.tarih = Now
                        hb.islem_tarihi = Now
                        TbToplam.Text = fiyat_al(hb.net_toplam)
                    Catch ex As Exception
                    End Try
                    DgSatis.UpdateLayout()
                    Satis_Durum_Yaz()
                    TbMakina.Text = ""
                    TbMakinaOrta.Text = ""
                    AdetAc(False)
                    hs = Nothing
                    Ur = Nothing
                    Ba = Nothing
                    '   Fy = Nothing
                    Br = Nothing
                    SilAktif = False
                End If
                Exit Sub
            Else
                Msg("Askı Listesi Boş!!", False, False, True)
            End If
        Else
            If Msg("Belge Askıya Alınacaktır Onaylıyormusunuz?", True, True, False) Then
                hb.aski = True
                SatisYaz()
            End If
        End If
        BtMakinaOrta.Focus()
    End Sub

    Private Sub GridUrunClick(sender As Object, e As RoutedEventArgs)
        'On Error Resume Next 
        If SilAktif = True Then
            If TbMakinaOrta.Text <> "" Then
                Sesuyari()
                Msg("Barkod Alanını Siliniz", False, False, True)
                Exit Sub
            End If
            Dim satir As Integer
            satir = DgSatis.SelectedIndex
            For Each dr As System.Data.DataRow In Ds.Tables(hareket_satir).Select("satir = '" & satir & "'")
                HareketSatirOku(dr)
                hs.satir = Ds.Tables(hareket_satir).Rows.IndexOf(dr)
                If SilAktif = True Then hs.iptal = True
                'If ay.stokgoster_aktif Then SatisGrid_StokGoster(UrunRef, BirimAlt, miktar)
                SatisDatasetYaz(hs.satir)
                SatisGrid_Toplam()
                SatisGrid_Temizle()
                Exit For
            Next
        End If
        BtMakinaOrta.Focus()
    End Sub

    Private Sub UrunEkle(sender As Object, e As RoutedEventArgs)
        If TimerPlu.Dispatcher.Thread.IsAlive Then
            TimerPlu.Stop()
        End If

        If TbMakinaOrta.Text <> "" Then
            Sesuyari()
            Msg("Barkod Alanını Siliniz", False, False, True)
            Exit Sub
        End If
        BtnUrun.Background = RenkUrun
        Dim Bt As New System.Windows.Controls.Button
        Bt = sender
        BtnUrun = Bt
        RenkUrun = Bt.Background
        BtnUrun.Background = Brushes.Lime
        Dim Ref As Integer = Bt.Uid
        ReadQueryView("select * from v_satisurunler where urunref = '" & Ref & "'")
        For Each dr As System.Data.DataRow In Ds.Tables(Urunler).Select("urunref = '" & Ref & "'")
            Ur.ref = Ref 'Chk_Null("ref", drUrun)
            Ur.guid = Chk_Null("urunguid", dr)
            Ur.birim_ust = Chk_Null("birimustref", dr)
            '  Ur.stok_miktar = Chk_Null("urunstokmiktar", dr)
            Br.birim_alt = Chk_Null("birimaltref", dr)
        Next
        If FiyatGorAktif Then
            Dim F As New WpfFiyatGor
            F.ShowDialog()
            FiyatGor()
        Else
            Satis_grid(Ur.guid, Br.birim_alt)
        End If
    End Sub

    Private Sub Giris(sender As Object, e As RoutedEventArgs)
        If TbMakinaOrta.Text <> "" Then
            Dim Stokkodu As String = TbMakinaOrta.Text
            Dim Tanimli As Boolean = False
            ReadQueryView("select * from v_satisurunler where urunlerstokkodu = '" & Stokkodu & "'")
            For Each drUrun As System.Data.DataRow In Ds.Tables("urunler").Select("urunlerstokkodu = '" & Stokkodu & "'")
                Tanimli = True
                Ur.ref = Chk_Null("urunref", drUrun)
                Ur.guid = Chk_Null("urunguid", drUrun)
                Ur.birim_ust = Chk_Null("birimustref", drUrun)
                Br.birim_alt = Chk_Null("birimaltref", drUrun)
            Next
            If Tanimli = False Then
                Msg("Tanımsız Ürün!!", False, False, True)
                TbUrunDurum.Text = ""
                BtGridUrunDurum.Background = Brushes.Blue
                FiyatGorAktif = False
                AdetAc(False)
                TbMakinaOrta.Text = ""
                TbUrunDurum.Text = ""
                Exit Sub
            End If
            If FiyatGorAktif Then
                Dim F As New WpfFiyatGor
                If F.main() Then
                    Satis_grid(Ur.guid, Br.birim_alt)
                Else
                    TbUrunDurum.Text = ""
                End If
            Else
                Satis_grid(Ur.guid, Br.birim_alt)
            End If
            BtGridUrunDurum.Background = Brushes.Blue
            FiyatGorAktif = False
            AdetAc(False)
            TbMakinaOrta.Text = ""
        Else
            If HsSonIslem.Miktar > 0 Then
                TbMakina.Text = HsSonIslem.Miktar
                Satis_grid(HsSonIslem.UrunGuid, HsSonIslem.BirimAlt)
            Else
                Msg("Son İşlem Yok!!", False, False, True)
            End If
        End If
    End Sub


    Private Sub DegisimFisi(sender As Object, e As RoutedEventArgs)
        '────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
#Disable Warning IDE0058

        If DgSatis.Items.Count > 0 Then
            Msg("Satiş İşlemini Bitiriniz!!!", False, False, True)
            Console.Beep(3000, 145)
            TbMakinaOrta.Text = ""
            Exit Sub
        End If

        Dim F As New WpfDegisimFisi
        F.ShowDialog()
        DurumTemizle()

        '────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        BtMakinaOrta.Focus()
    End Sub


    Dim FiyatGorAktif As Boolean = False
    Private Sub FiyatGor()
        If hs.ozelfiyat = True Then
            Msg("Özel Fiyat Açık", False, False, True)
            Exit Sub
        End If
        If FiyatGorAktif Then
            TbUrunDurum.Text = ""
            BtGridUrunDurum.Background = Brushes.Blue
            FiyatGorAktif = False
        Else
            TbUrunDurum.Text = "FİYAT GÖR"
            BtGridUrunDurum.Background = Brushes.Red
            FiyatGorAktif = True
        End If
        BtMakinaOrta.Focus()
    End Sub

    Private Sub CariHesap()
        Me.Opacity = 0.5
        Dim WCari As New W_CariHesap
        WCari.Main()
        Me.Opacity = 1
        If cr.aciklama <> "" Then
            Me.TbGrupDurum.Text = cr.aciklama
        End If
    End Sub

    Private Sub Numarator(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As System.Windows.Controls.Button = sender
        Select Case Btn.Name
            Case "Bt0"
                Me.TbMakinaOrta.Text &= "0"
            Case "Bt1"
                Me.TbMakinaOrta.Text &= "1"
            Case "Bt2"
                Me.TbMakinaOrta.Text &= "2"
            Case "Bt3"
                Me.TbMakinaOrta.Text &= "3"
            Case "Bt4"
                Me.TbMakinaOrta.Text &= "4"
            Case "Bt5"
                Me.TbMakinaOrta.Text &= "5"
            Case "Bt6"
                Me.TbMakinaOrta.Text &= "6"
            Case "Bt7"
                Me.TbMakinaOrta.Text &= "7"
            Case "Bt8"
                Me.TbMakinaOrta.Text &= "8"
            Case "Bt9"
                Me.TbMakinaOrta.Text &= "9"
            Case BtVirgul.Name
                Dim Str As String = Me.TbMakinaOrta.Text
                Dim index As Integer = 0
                index = Str.IndexOf(",")
                If index < 0 Then
                    Me.TbMakinaOrta.Text &= ","
                End If
            Case BtBackSpace.Name
                Try
                    If SilAktif Then
                        TbMakinaOrta.Text = ""
                        BtGridUrunDurum.Background = Brushes.Blue
                        SilAktif = False
                    Else
                        TbMakinaOrta.Text = Mid(TbMakinaOrta.Text, 1, TbMakinaOrta.Text.Length - 1)
                    End If
                Catch ex As Exception
                End Try
        End Select
    End Sub



    Private Sub SatisDataGridOran()
        Dim DgGridEn As Integer = DgSatis.RenderSize.Width
        DgSatis.Columns(0).Width = (DgGridEn * 40) / 100
        DgSatis.Columns(1).Width = (DgGridEn * 13) / 100
        DgSatis.Columns(2).Width = (DgGridEn * 13) / 100
        DgSatis.Columns(3).Width = (DgGridEn * 10) / 100
        DgSatis.Columns(4).Width = (DgGridEn * 15) / 100
    End Sub

    Private Sub TahsilatDataGridOran()
        Dim DgGridEn As Integer = DgTahsilat.RenderSize.Width
        DgTahsilat.Columns(0).Width = (DgGridEn * 33) / 100
        DgTahsilat.Columns(1).Width = (DgGridEn * 27) / 100
        DgTahsilat.Columns(2).Width = (DgGridEn * 25) / 100
    End Sub



    Private Sub WpfSatis_KeyDown(sender As Object, e As Input.KeyEventArgs) Handles Me.KeyDown
        Select Case e.Key
            Case Key.Enter
                If TbMakinaOrta.Text <> "" Then
                    UrunEkleBarkod()
                End If
            Case Key.NumPad0, Key.D0
                TbMakinaOrta.Text &= 0
            Case Key.NumPad1, Key.D1
                TbMakinaOrta.Text &= 1
            Case Key.NumPad2, Key.D2
                TbMakinaOrta.Text &= 2
            Case Key.NumPad3, Key.D3
                TbMakinaOrta.Text &= 3
            Case Key.NumPad4, Key.D4
                TbMakinaOrta.Text &= 4
            Case Key.NumPad5, Key.D5
                TbMakinaOrta.Text &= 5
            Case Key.NumPad6, Key.D6
                TbMakinaOrta.Text &= 6
            Case Key.NumPad7, Key.D7
                TbMakinaOrta.Text &= 7
            Case Key.NumPad8, Key.D8
                TbMakinaOrta.Text &= 8
            Case Key.NumPad9, Key.D9
                TbMakinaOrta.Text &= 9
            Case Key.Back
                Try
                    TbMakinaOrta.Text = Mid(TbMakinaOrta.Text, 1, TbMakinaOrta.Text.Length - 1)
                Catch ex As Exception
                End Try
        End Select
    End Sub



    Private Function SayacAl() As Integer
        Dim BasTarihStr As String = TarihAl(Now.ToString) & " 00:00:00"
        Dim BitTarihStr As String = TarihAl(Now.ToString) & " 23:59:00"
        Dim Sql As String
        Sql = "  SELECT max(sayac) as sayac FROM hareket_baslik"
        Sql &= " WHERE CAST(tarih as datetime) BETWEEN '" & BasTarihStr & "' AND '" & BitTarihStr & "'"
        Dim sayac As Integer = 0

        Try
            sayac = Execute_Oku(Sql, "sayac", Conn, False)
        Catch ex As Exception
        End Try
        Return sayac
    End Function

    Dim Plusayac As Integer = 0
    Dim PluCount As Integer = 0


    Private Function SatisBrMenuYap(ByVal Name As String,
                               ByVal Text As String,
                               ByVal Colon As Integer,
                               ByVal renk As Brush,
                               ByVal Evnt As RoutedEventHandler) As Border
        Dim Br As New Border
        Br.Background = Brushes.Aqua
        Br.Margin = New Thickness(3)
        Dim Bt As New Button
        Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
        Bt = ButtonYap(Name, Text, 0, Colon, renk, Windows.HorizontalAlignment.Stretch)
        Bt.Margin = New Thickness(3)
        Bt.AddHandler(Button.ClickEvent, Evnt, True)
        Dim style As New Style
        style = FindResource("RoundedButton")
        Bt.Style = style
        Br.Child = Bt
        Grid.SetColumn(Br, Colon)
        Return Br
    End Function




    Private Sub OdemeMenuYap()
        Dim UgO As New UniformGrid
        UgO.Columns = 10
        UgO.Columns = 1
        For Each dr As DataRow In Ds.Tables(odeme).Rows
            Dim Ref As String = dr.Item("ref")
            Dim Kod As String = dr.Item("kod")

            Dim Br As New Border
            Br.Background = Brushes.Aqua
            Br.Margin = New Thickness(3)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap(Ref, Kod, 0, 0, Brushes.LightGray, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf TahsilatSatir

            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Br.Child = Bt
            UgO.Children.Add(Br)
        Next
        GrOdeme.Children.Add(UgO)

    End Sub

    Private Sub ResimGoster(ByVal bt As Button, ByVal Resim() As Byte)
        Try
            Dim ResimMs As New System.IO.MemoryStream(Resim)
            Dim Br As Brush = New ImageBrush()
            bt.Background = Br
            ResimMs.Close()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub KupurDegerYaz(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Bt As Button = sender
        Dim ref As Long = Bt.Uid
        For Each dr As DataRow In Ds.Tables(kupur).Select("Ref = '" & Ref & "'")
            Dim deger As Decimal = Chk_Null("deger", dr)
            If TbMakinaOrta.Text <> "" Then
                If IsNumeric(TbMakinaOrta.Text) Then
                    deger = deger + Convert.ToDecimal(TbMakinaOrta.Text)
                    TbMakinaOrta.Text = fiyat_al(deger)
                End If
            Else
                TbMakinaOrta.Text = fiyat_al(deger)
            End If
        Next
    End Sub


    Private Sub BanknotCiz()
        Dim UgB As New UniformGrid
        UgB.Columns = 3
        UgB.Rows = 2
        Dim UgM As New UniformGrid
        UgM.Columns = 3
        UgM.Rows = 2

        For Each dr As DataRow In Ds.Tables(kupur).Rows
            Dim Ref As String = dr.Item("ref")
            Dim deger As Decimal = dr.Item("deger")
            Dim aciklama As String = dr.Item("aciklama")
            Dim banknot As Boolean = dr.Item("banknot")
            Dim Resim() As Byte = Chk_Null("resim", dr)

            Dim Br As New Border
            Br.Background = Brushes.Aqua
            Br.Margin = New Thickness(3)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            'Bt = ButtonYap(Ref, fiyat_al(deger), 0, 0, Brushes.LightGray, Windows.HorizontalAlignment.Stretch)
            Bt = ButtonYap(Ref, aciklama, 0, 0, Brushes.LightGray, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf KupurDegerYaz
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style

            If Resim IsNot Nothing Then
                ResimGoster(Bt, Resim)
            End If
            Br.Child = Bt
            If banknot = True Then
                UgB.Children.Add(Br)
            Else
                UgM.Children.Add(Br)
            End If

        Next
        GrBanknot.Children.Add(UgB)
        GrMadeni.Children.Add(UgM)
    End Sub


    Private Function SatisMenuYap(
                                 giris_ As Boolean,
                                 DegisimFisi_ As Boolean,
                                 fiyat_gor As Boolean,
                                 ozel_fiyat As Boolean,
                                 plu As Boolean,
                                 aski_ As Boolean,
                                 musteri_ As Boolean,
                                 tarti_ As Boolean,
                                 satir_yuzde_indirim_ As Boolean,
                                 satir_tutar_indirim_ As Boolean,
                                 belge_turu_ As Boolean,
                                 fnk_ As Boolean) As Grid

        Dim Grd As New Grid
        Dim sayi As Integer

        If giris_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Giris)
            Grd.Children.Add(SatisBrMenuYap("BtGiris", "GİRİŞ", sayi, Brushes.WhiteSmoke, ev))
            sayi += 1
        End If

        If DegisimFisi_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf DegisimFisi)
            Grd.Children.Add(SatisBrMenuYap("BtDegisim", "İADE/DEĞİŞİM", sayi, Brushes.Beige, ev))
            sayi += 1
        End If

        If fiyat_gor Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf FiyatGor)
            Grd.Children.Add(SatisBrMenuYap("BtFiyatGor", "FİYAT GÖR", sayi, Brushes.Aqua, ev))
            sayi += 1
        End If

        If 1 = 1 Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf CariHesap)
            Grd.Children.Add(SatisBrMenuYap("BtCariHesap", "MÜŞTERİ", sayi, Brushes.Aqua, ev))
            sayi += 1
        End If

        If ozel_fiyat Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf OzelFiyat)
            Grd.Children.Add(SatisBrMenuYap("BtOzelfiyat", "ÖZEL FİYAT", sayi, Brushes.Goldenrod, ev))
            sayi += 1
        End If

        If plu Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Fonksiyon)
            Grd.Children.Add(SatisBrMenuYap("BtPlu", "PLU", sayi, Brushes.Aqua, ev))
            sayi += 1
        End If
        If aski_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Aski)
            Grd.Children.Add(SatisBrMenuYap("BtAski", "ASKI", sayi, Brushes.Lime, ev))
            sayi += 1
        End If
        If musteri_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Fonksiyon)
            Grd.Children.Add(SatisBrMenuYap("BtMusteri", "MÜŞTERİ", sayi, Brushes.Aquamarine, ev))
            sayi += 1
        End If
        If tarti_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Fonksiyon)
            Grd.Children.Add(SatisBrMenuYap("BtTarti", "TARTI", sayi, Brushes.LightSkyBlue, ev))
            sayi += 1
        End If
        If satir_yuzde_indirim_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf SatirYuzdeIndirim)
            Grd.Children.Add(SatisBrMenuYap("BtSatirIndirim", "SATIR % İNDİRİM", sayi, Brushes.Tomato, ev))
            sayi += 1
        End If
        If satir_tutar_indirim_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf SatirTutarIndirim)
            Grd.Children.Add(SatisBrMenuYap("BtSatirIndirim", "SATIR T. İNDİRİM", sayi, Brushes.Tomato, ev))
            sayi += 1
        End If

        If belge_turu_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Fonksiyon)
            Grd.Children.Add(SatisBrMenuYap("BtBelgeTuru", "BELGE TÜRÜ", sayi, Brushes.Green, ev))
            sayi += 1
        End If
        If fnk_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim ev As System.Windows.RoutedEventHandler = New RoutedEventHandler(AddressOf Fonksiyon)
            Grd.Children.Add(SatisBrMenuYap("BtFnk", "FNK", sayi, Brushes.IndianRed, ev))
            sayi += 1
        End If
        Grid.SetRow(Grd, 1)
        Return Grd
    End Function

    Private Sub AdetAc(ByVal Ac As Boolean)
        If Ac Then
            BtMakina.Visibility = Windows.Visibility.Visible
            Grid.SetColumn(BtMakinaOrta, 2)
            Grid.SetColumnSpan(BtMakinaOrta, 4)
        Else
            BtMakina.Visibility = Windows.Visibility.Hidden
            Grid.SetColumn(BtMakinaOrta, 0)
            Grid.SetColumnSpan(BtMakinaOrta, 6)
        End If
    End Sub

    Private Function CountKontrol(ByVal tbl As String) As Boolean
        Dim Count As Integer = Ds.Tables(odeme).Rows.Count
        If Count <= 0 Then
            Msg(tbl.ToUpper & "Tanımlarını Tamamlayınız!!", False, False, True)
            Return False
        End If
        Return True
    End Function


    Private Sub GenelAyarlar()
        Ds_read("Select * from ayarlar where sube = '" & Reg.Sube & "'", ayarlar, True, True)
        For Each dr As DataRow In Ds.Tables(ayarlar).Rows
            ay = Deger_Al(dr)
        Next
        If ay.sube = 0 Then
            Msg("Mağaza Ayarlarını Aktarınız!!", False, False, True)
        End If
    End Sub

    Private Sub TextHead_()
        'TextHead.Text = TabloTxt
        TbGrupDurum.Text = "BİLGİLER AKTARILIYOR!!"
    End Sub

    Private Sub Kapat()
        'TextHead.Text = TabloTxt
        TbGrupDurum.Text = "AKTARIM İŞLEMİ TAMAMLANDI!"
    End Sub

    Private Sub TbYaz()
        TbGrupDurum.Text = "BİLGİLER AKTARILIYOR!!"
    End Sub

    Private Sub Grup_Listele()
        GrupListele()
    End Sub

    Dim ExitThr As Boolean = False
    Private Sub OrtaKontrol()
        If TbMakinaOrta.Text <> "" Then
            ExitThr = True
        End If
    End Sub


    Dim TabloTxt As String = ""

    Private Delegate Sub NextPrimeDelegate()
    Private Delegate Sub SecondPrimeDelegate()
    Private Delegate Sub ThirdPrimeDelegate()
    Private Delegate Sub FourtPrimeDelegate()



    Dim DsWait As Boolean = False


    Sub TbGrupDurum_Text_Aktarim_Yapılıyor()
        TbGrupDurum.Text = "Aktarim Yapılıyor"

    End Sub
    Sub TbGrupDurum_Text_Aktarim_Tamamlandi()
        TbGrupDurum.Text = "Aktarim Tamamlandı"
        GrupListele()


    End Sub
    Sub PluAktar()
        On Error Resume Next
        TbGrupDurum.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf TbGrupDurum_Text_Aktarim_Yapılıyor))
        Dim W As New WpfPluKasa
        W.Main(False)
        TbGrupDurum.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf TbGrupDurum_Text_Aktarim_Tamamlandi))
        TimerPlu.Start()
    End Sub



    Sub ChkPlu()
        If OkChkPlu = True Then Exit Sub
        TimerPlu.Stop()
        If DgSatis.Items.Count <= 0 Then
            If MakinaOrtaText = "" Then
                If MakinaText = "" Then
                    'Dim Sql As String = "select * from aktarim where pos = '" & pd.guid & "' and status <> 1"
                    Dim Sql As String = "select *,GETDATE() AS checkdate from aktarim where status <> 1 and pos = '" & pd.guid.ToString & "'"
                    Try
                        DsServisOku.Tables.Remove("aktarim")
                    Catch ex As Exception
                    End Try
                    Dim Dt As DataTable = ServisOkuTable("aktarim", Sql)
                    If Dt.Rows.Count > 0 Then
                        DsServisOku.Tables.Add(Dt)
                        TbGrupDurum.Dispatcher.BeginInvoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf PluAktar))
                    Else
                        TimerPlu.Start()
                    End If
                End If
            End If
        End If
        '        TimerPlu.Start()
    End Sub

    Private Sub MerkezSatisYaz()
        TimerPlu.Stop()
        Do While OkChkPlu = True
            TimerPlu.Stop()
        Loop
        Dim DsS As New DataSet
        Dim Count As Int64 = 0
        Dim C As New C_ChkDataLocalDb
        Dim Conn As SqlConnection = C.Sql_conn_localDb
        Ds_read("select * from plu", Ds, Plu, Conn, False)
        For Each Dr As DataRow In Ds.Tables(Plu).Select("grup = 'satis'")
            Dim Tbl As String = Dr.Item("tablo")
            Ds_read("select * from " & Tbl & " where upload <> 1", DsS, Tbl, Conn, False)
            DsS.Tables(Tbl).Columns("upload").Expression = True
            Count += DsS.Tables(Tbl).Rows.Count
        Next
        If Count > 0 Then
            Dim ok As Boolean
            Try
                Dim Service As New Sriletisim.WSiletisimSoapClient
                Service.ChannelFactory.Endpoint.Binding.CreateBindingElements()
                Service.Endpoint.Address = EpAddress
                ok = Service.UploadSatislar(DsS)
            Catch ex As Exception
                MsgBox(ex.Message)

            End Try
            If ok Then
                For Each Dt As DataTable In DsS.Tables
                    Bulk_Update(Dt, Conn, True)
                Next
            End If
        End If
        TimerPlu.Start()
    End Sub


    Private Sub TimerPluSubeChk(ByVal sender As Object, ByVal e As EventArgs)
        TimerPlu.Interval = New TimeSpan(0, 0, 10)
        TimerPlu.Stop()
        Dim trd As Thread = Nothing
        trd = New Thread(AddressOf ChkPlu) With {
            .IsBackground = True
        }
        trd.Start()
    End Sub


    Private Sub FiyatAktarimBaslat()
        If TimerPlu.IsEnabled = True Then Exit Sub
        Plusayac = 0
        PbPlu.Value = 0
        If aktarimOk Then TimerPlu.Start()
    End Sub


    Private Sub ReadQueryView(ByVal Sql As String)
        Ds.Locale = CultureInfo.InvariantCulture
        Dim cmd As New SqlClient.SqlCommand(Sql, Conn)
        Dim da As New SqlClient.SqlDataAdapter
        da.SelectCommand = cmd
        Try
            da.Fill(Ds, Urunler)
        Catch ex As Exception
        End Try
    End Sub






    Private Sub Main()
        EmurateCentralProcessingUnitUsage()

        On Error GoTo 1
        BrDgSatis.Visibility = Visibility.Collapsed
        hb = Nothing
        hs = Nothing
        hi = Nothing
        kh = Nothing
        th = Nothing
        Ur = Nothing
        Br = Nothing
        Br = Nothing
        Ba = Nothing
        Bu = Nothing
        GenelAyarlar()
        '        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        If ay.ref <> 0 Then
            ProgramAyarlari()
            ay.indirim_aktif = True
        Else
            Msg("Program Ayarlarını Tanımlayınız !", False, False, True)
            Me.Close()
            Exit Sub
        End If
        If pd.kasa_kod = "" Then
            Msg("Kasa Kodunu Tanımlayınız !", False, False, True)
            Me.Close()
            Exit Sub
        End If
        If Kl.kod = "" Then
            Msg("Kullanici Kodunu Tanımlayınız !", False, False, True)
            Me.Close()
            Exit Sub
        End If
        GrSatisMenu.Children.Add(SatisMenuYap(True, True, True, True, False, True, False, False, True, True, False, True))
        AdetAc(False)
        EkranDizayn_None(Me)
        DataSetOku(False, False, False, True, False, True, True, True, True, True, True, True, True, True)
        'ReadQueryView("select * from v_satisurunler where urunozellikguid is not null order by grupsira")
        ReadQueryView("select * from v_satisurunler where (urunozellikguid IS NOT NULL) order by grupsira")
        '─────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        If CountKontrol(odeme) = False Then Me.Close()
        '─────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        OdemeMenuYap()
        BanknotCiz()
        Olaylar()
        GridTemizleSatis()
        GridTemizleTahsilat()
        SatisDataGridOran()
        TahsilatDataGridOran()

        BrDgSatis.Visibility = Windows.Visibility.Visible
        BrDgTahsilat.Visibility = Windows.Visibility.Hidden

        GrSatis.Visibility = Windows.Visibility.Visible
        GrTahsilat.Visibility = Windows.Visibility.Hidden

        GrupListele()
        TeraziAyarlari()

        If ay.stokgoster_aktif = False Then
            BrGridStokDurum.Visibility = Windows.Visibility.Hidden
            Grid.SetColumnSpan(BrGridGrupDurum, 2)
        Else
            BrGridStokDurum.Visibility = Windows.Visibility.Visible
            Grid.SetColumnSpan(BrGridGrupDurum, 1)
        End If
        GrTahsilatMenuNumarator.Visibility = Windows.Visibility.Hidden
        FiyatAktarimBaslat()
        ServisSatisyazThr()
        FonksiyonUygula(400, False)
        Exit Sub
1:
        Msg(Err.Description, False, False, True)
        MsgBox(Err.Description)
    End Sub


End Class

