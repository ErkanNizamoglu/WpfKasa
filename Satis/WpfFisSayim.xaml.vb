Imports System.Data
Imports System.Windows.Controls.Primitives
Imports System.Data.SqlClient

Public Class WpfFisSayim

    Private Sub Olay_Ekle()
        AddHandler BtKayit.Click, AddressOf KayitSayimBaslik
        AddHandler BtCikis.Click, AddressOf Cikis

        AddHandler BtSatirEkle.Click, AddressOf UrunEkle
        AddHandler BtSatirSil.Click, AddressOf UrunSil
        AddHandler BtSatirDegistir.Click, AddressOf SatirDegistir


        AddHandler TbMiktar.PreviewMouseDown, AddressOf TextBlock_Degistir
        AddHandler TbFiyat.PreviewMouseDown, AddressOf TextBlock_Degistir
        AddHandler TbBarkod.PreviewMouseDown, AddressOf TextBlock_Degistir


        AddHandler TbMiktar.TextChanged, AddressOf SatirTextChange
        AddHandler TbFiyat.TextChanged, AddressOf SatirTextChange
        AddHandler TbBarkod.TextChanged, AddressOf SatirTextChange

        AddHandler TbBarkod.KeyDown, AddressOf Barkod_KeyDown



        AddHandler DgListe.SelectionChanged, AddressOf SatirSec

        AddHandler BtMiktar.Click, AddressOf Temizle
        AddHandler BtFiyat.Click, AddressOf Temizle


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
        AddHandler Btvirgul.Click, AddressOf Numarator
        AddHandler BtSil.Click, AddressOf Numarator
    End Sub


    Private Sub Temizle(ByVal sender As Object, ByVal e As RoutedEventArgs)

        KolonYazma = True

        Dim Btn As Button = sender
        Select Case Btn.Name
            Case BtMiktar.Name
                TextBlock_Degistir(TbMiktar, Nothing)
                TbMiktar.Text = ""
            Case BtFiyat.Name
                TextBlock_Degistir(TbFiyat, Nothing)
                TbFiyat.Text = ""
        End Select
    End Sub

    Private Sub Barkod_KeyDown(sender As Object, e As KeyEventArgs)
        Tb = sender
        KolonYazma = True
        Select Case e.Key
            Case Key.Enter
                If TbBarkod.Text <> "" Then
                    If UrunEkleBarkod() Then Exit Sub
                End If
        End Select
    End Sub

    Private Function UrunEkleBarkod() As Boolean
        Dim ok As Boolean
        For Each Dr As DataRow In Ds.Tables(Barkodlar).Select("barkod = '" & TbBarkod.Text & "'")
            Dim UrunRef As Int64 = Dr.Item("ref")
            DegiskenAl(Urunler, UrunRef, True)
            SatirEkle()
            TbBarkod.Text = ""
            'TbMiktar.Focus()
            ok = True
        Next
        Return ok
    End Function



    Private Sub Numarator(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Tb.Name = Nothing Then
            Exit Sub
        End If
        KolonYazma = True
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case "Bt0"
                Tb.Text &= "0"
            Case "Bt1"
                Tb.Text &= "1"
            Case "Bt2"
                Tb.Text &= "2"
            Case "Bt3"
                Tb.Text &= "3"
            Case "Bt4"
                Tb.Text &= "4"
            Case "Bt5"
                Tb.Text &= "5"
            Case "Bt6"
                Tb.Text &= "6"
            Case "Bt7"
                Tb.Text &= "7"
            Case "Bt8"
                Tb.Text &= "8"
            Case "Bt9"
                Tb.Text &= "9"
            Case "Btvirgul"
                KolonYazma = False
                Tb.Text &= ","
            Case "BtSil"
                Try
                    Tb.Text = Mid(Tb.Text, 1, Tb.Text.Length - 1)
                Catch ex As Exception
                End Try
        End Select
    End Sub

    Dim KolonYazma As Boolean = False
    Dim Tb As New TextBox

    Private Sub Numeric_KeyDown(sender As Object, e As KeyEventArgs)
        Tb = sender
        KolonYazma = True
        Select Case e.Key
            Case Key.OemComma
                If Tb.Text.IndexOf(",") <= 0 Then
                    KolonYazma = False
                    e.Handled = False
                Else
                    e.Handled = True
                End If
                Exit Sub
            Case Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
                e.Handled = False
            Case Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9
                e.Handled = False
            Case Key.Back
                e.Handled = False
            Case Key.Right
                e.Handled = False
            Case Key.Left
                e.Handled = False
            Case Key.F10, Key.System
                SatirEkle()
            Case Key.Tab

            Case Else
                e.Handled = True
        End Select
    End Sub

    Private Sub ToplamYaz()
        On Error Resume Next
        hb.net_toplam = Dt.Compute("Sum(net_toplam)", "iptal <> 1")
        LbNetTutar.Content = fiyat_al(hb.net_toplam)
    End Sub




    Private Sub SatirTextChange(sender As Object, e As RoutedEventArgs)
        On Error Resume Next
        If KolonYazma = False Then Exit Sub
        Dim Tb As New TextBox
        Tb = sender
        Dim tutar As Decimal
        If IsNumeric(Tb.Text) Then
            tutar = Tb.Text
        Else
            tutar = 0
        End If
        Select Case Tb.Name
            Case TbFiyat.Name
                If Tb.Text = "" Then
                    hs.fiyat = 0
                Else
                    If IsNumeric(Tb.Text) Then
                        hs.fiyat = Tb.Text
                    Else
                        hs.fiyat = 0
                    End If
                End If
            Case TbMiktar.Name
                If Tb.Text = "" Then
                    hs.miktar = 0
                Else
                    If IsNumeric(Tb.Text) Then
                        hs.miktar = Tb.Text
                        hs.miktar_fark = hs.miktar - hs.miktar_eldeki
                    Else
                        hs.miktar = 0
                    End If
                End If
            Case Else
                Exit Sub
        End Select
        hs.brut_tutar = hs.miktar * hs.fiyat
        hs.net_tutar = hs.miktar * hs.fiyat

        DgListe.SelectedItem("miktar") = hs.miktar
        DgListe.SelectedItem("miktar_eldeki") = hs.miktar_eldeki
        DgListe.SelectedItem("miktar_fark") = hs.miktar_fark
        DgListe.SelectedItem("fiyat") = hs.fiyat
        DgListe.SelectedItem("brut_tutar") = hs.brut_tutar
        DgListe.SelectedItem("net_tutar") = hs.net_tutar
        ToplamYaz()
    End Sub


    Private Sub DatasetOkuFisStok(ByVal ref As Integer)
        DegiskenAl(TblBaslik, ref, True)
        Ds_read("select * from hareket_satir where baslikguid = '" & hb.guid & "' and iptal <> 1", TblSatir, Conn, True, True)
        Dt = Ds.Tables(TblSatir)
        Dv.Table = Dt
        DgListe.ItemsSource = Dv
        Dv.RowFilter = "iptal <> 1"

        'Dim DgGridEn As Integer = DgListe.RenderSize.Width
        'DgListe.Columns(1).Width = (DgGridEn * 50) / 100
        'DgListe.Columns(2).Width = (DgGridEn * 30) / 100
        'DgListe.Columns(3).Width = (DgGridEn * 70) / 100

    End Sub

    Private Sub Cikis()
        RemoveTable(hareket_baslik)
        RemoveTable(hareket_satir)
        Me.Close()
    End Sub

    Private Sub DsWrite(ByVal dr As DataRow, ByVal colon As String, ByVal deger As Object)
        dr.Item(colon) = deger
    End Sub

    Private Sub DatasetYaz(ByVal Dr As DataRow)
        DsWrite(Dr, "satir", hs.satir)
        DsWrite(Dr, "iptal", hs.iptal)
        DsWrite(Dr, "urunguid", hs.urunguid)
        DsWrite(Dr, "urun_aciklama", hs.urun_aciklama)
        DsWrite(Dr, "birim_alt", hs.birim_alt)
        DsWrite(Dr, "birim_alt_isaret", hs.birim_alt_isaret)
        DsWrite(Dr, "bolen", hs.bolen)
        DsWrite(Dr, "carpan", hs.carpan)
    End Sub

    Private Function EldekiMiktar(ByVal UrunGuid As String) As Decimal

        Dim Sql As String = ""
        Return 0

    End Function


    Private Sub SatirEkle()
        hs = Nothing
        Try

            For Each drSatir As DataRow In Ds.Tables(TblSatir).Select("urunguid = '" & Ur.guid & "' and iptal <> 1")
                hs.satir = Chk_Null("satir", drSatir)
                hs.miktar_eldeki = Chk_Null("miktar_eldeki", drSatir)
                hs.miktar_fark = Chk_Null("miktar_fark", drSatir)
                hs.miktar = Chk_Null("miktar", drSatir)
                hs.fiyat = Chk_Null("fiyat", drSatir)
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        If hs.satir > 0 Then
            DgListe.SelectedIndex = hs.satir - 1
            Try
                DgListe.Focus()
                Dim Obj = DgListe.Items(hs.satir - 1)
                DgListe.ScrollIntoView(Obj)
                DgListe.SelectedItem = Obj
            Catch ex As Exception
            End Try
            Exit Sub
        End If

        hs.urunguid = Ur.guid
        hs.stokkodu = Ur.stokkodu
        hs.urun_aciklama = Ur.aciklama
        hs.fiyat = Ur.fiyat_alis
        hs.miktar_eldeki = Ur.stok_miktar
        hs.birim_ust = Ur.birim_ust
        hs.islemyonu = 1
        hs.mac = mac_adress()
        hs.createdate = Now

        hs.iptal = False

        DegiskenAl(birim_ust, Ur.birim_ust, True)
        Ds_read("select * from birim_alt where ref_ust = '" & Ur.birim_ust & "' order by sira", birim_alt, Conn, True, True)
        For Each drBirim As System.Data.DataRow In Ds.Tables(birim_alt).Select("sira = '1'")
            hs.birim_alt = Chk_Null("ref", drBirim)
            hs.birim_alt_isaret = Chk_Null("isaret", drBirim)
            hs.bolen = Chk_Null("bolen", drBirim)
            hs.carpan = Chk_Null("carpan", drBirim)
        Next

        hs.brut_tutar = hs.miktar_eldeki * hs.fiyat
        hs.net_tutar = hs.miktar_eldeki * hs.fiyat


        Dt.Rows.Add()
        hs.satir = Dt.Rows.Count
        Dim dr As DataRow = Dt.Rows(hs.satir - 1)
        DatasetYaz(dr)


        If hs.fiyat > 0 Then DsWrite(dr, "fiyat", hs.fiyat)
        If hs.miktar_eldeki > 0 Then DsWrite(dr, "miktar_eldeki", hs.miktar_eldeki)

        DsWrite(dr, "brut_tutar", hs.brut_tutar)
        DsWrite(dr, "net_tutar", hs.net_tutar)
        Try
            DgListe.Focus()
            Dim Obj = DgListe.Items(DgListe.Items.Count - 1)
            DgListe.ScrollIntoView(Obj)
            DgListe.SelectedItem = Obj
        Catch ex As Exception
        End Try
        ToplamYaz()
    End Sub


    Private Sub UrunSil()
        If DgListe.Items.Count <= 0 Then Exit Sub
        Dim ok As Boolean = False
        ok = Msg("Satır Silinecektir Onaylıyormusunuz?", True, True, False)
        If ok = True Then
            Dim s As Integer = 0
            Try
                s = DgListe.SelectedIndex
            Catch ex As Exception
                Msg("Satır Seçiniz!", False, False, True)
                Exit Sub
            End Try
            If s < 0 Then
                Msg("Satır Seçiniz!", False, False, True)
                Exit Sub
            End If
            DgListe.SelectedItem("iptal") = True

            Dim dr As DataRow = Ds.Tables(TblSatir).Rows(s)
            DsWrite(dr, "iptal", True)
            DgListe.UpdateLayout()
            Try
                DgListe.Focus()
                Dim Obj = DgListe.Items(DgListe.Items.Count - 1)
                DgListe.ScrollIntoView(Obj)
                DgListe.SelectedItem = Obj
            Catch ex As Exception
            End Try
            ToplamYaz()
        End If
    End Sub

    Dim Pozisyon As Integer
    Dim satir As Integer = 0
    Private Sub SatirSec()
        On Error Resume Next

        RemoveHandler TbMiktar.TextChanged, AddressOf SatirTextChange
        RemoveHandler TbFiyat.TextChanged, AddressOf SatirTextChange

        satir = DgListe.SelectedIndex
        If satir < 0 Then Exit Sub
        hs.miktar = 0

        hs.miktar = DgListe.SelectedItem("miktar")
        If hs.miktar > 0 Then
            TbMiktar.Text = hs.miktar.ToString("N3")
        Else
            TbMiktar.Text = ""
        End If

        hs.miktar_eldeki = 0
        hs.miktar_eldeki = DgListe.SelectedItem("miktar_eldeki")

        hs.miktar_fark = 0
        hs.miktar_fark = DgListe.SelectedItem("miktar_fark")

        hs.fiyat = 0
        hs.fiyat = DgListe.SelectedItem("fiyat")
        If hs.fiyat > 0 Then
            TbFiyat.Text = hs.fiyat.ToString("N3")
        Else
            TbFiyat.Text = ""
        End If

        If hs.miktar = 0 Then TbMiktar.Text = ""
        If hs.fiyat = 0 Then TbFiyat.Text = ""

        AddHandler TbMiktar.TextChanged, AddressOf SatirTextChange
        AddHandler TbFiyat.TextChanged, AddressOf SatirTextChange
    End Sub

    Private Sub SatirDegistir()
        On Error Resume Next

        Dim W As New Wpf_ListeUrunler
        W.ShowDialog()

        satir = DgListe.Items.CurrentPosition
        If satir < 0 Then Exit Sub

        hs.miktar = 0

        hs.miktar = DgListe.SelectedItem("miktar")
        If hs.miktar > 0 Then
            TbMiktar.Text = hs.miktar.ToString("N3")
        Else
            TbMiktar.Text = ""
        End If

        hs.miktar_eldeki = 0
        hs.miktar_eldeki = DgListe.SelectedItem("miktar_eldeki")

        hs.miktar_fark = 0
        hs.miktar_fark = DgListe.SelectedItem("miktar_fark")

        hs.fiyat = 0
        hs.fiyat = DgListe.SelectedItem("fiyat")
        If hs.fiyat > 0 Then
            TbFiyat.Text = hs.fiyat.ToString("N3")
        Else
            TbFiyat.Text = ""
        End If

        If hs.miktar = 0 Then TbMiktar.Text = ""
        If hs.fiyat = 0 Then TbFiyat.Text = ""

        AddHandler TbMiktar.TextChanged, AddressOf SatirTextChange
        AddHandler TbFiyat.TextChanged, AddressOf SatirTextChange
    End Sub



    Private Sub UrunEkle()
        If TbBarkod.Text <> "" Then
            If UrunEkleBarkod() Then Exit Sub
        End If
        Ur = Nothing

        Dim W As New Wpf_ListeUrunler
        W.ShowDialog()

        If Ur.ref <> 0 Then
            SatirEkle()
        Else
            Exit Sub
        End If
        If hs.miktar > 0 Then
            TbMiktar.Text = hs.miktar
        Else
            TbMiktar.Text = ""
        End If
        If hs.fiyat > 0 Then
            TbFiyat.Text = Ur.fiyat_alis
        Else
            TbFiyat.Text = ""
        End If
    End Sub

    Private Sub BarkodEkle()
        Ur = Nothing


        If Ur.ref <> 0 Then
            SatirEkle()
        Else
            Exit Sub
        End If
        If hs.miktar > 0 Then
            TbMiktar.Text = hs.miktar
        Else
            TbMiktar.Text = ""
        End If
        If hs.fiyat > 0 Then
            TbFiyat.Text = Ur.fiyat_alis
        Else
            TbFiyat.Text = ""
        End If
    End Sub

    Private Sub KayitSayimBaslik()
        'On Error Resume Next
        If DpTarih.SelectedDate Is Nothing Then
            hb.tarih = Now
        Else
            hb.tarih = DpTarih.SelectedDate
        End If
        hb.kullanici = Kl.ref
        hb.kullanici_kod = Kl.kod
        hb.fisturu = 9
        hb.fiskod = "Sayim"
        hb.gunsonu = False
        hb.iptal = False
        hb.onay = False
        hb.brut_toplam = 0
        hb.net_toplam = 0
        hb.sayim_turu = CbSayimTuru.Text
        hb.upload = 0
        If CbSayimTuru.Text = "" Then
            Msg("Sayım Türünü Seçiniz!!", False, False, True)
            Exit Sub
        End If
        Try
            hb.brut_toplam = Dt.Compute("Sum(brut_tutar)", "iptal <> 1")
        Catch ex As Exception
        End Try
        Try
            hb.net_toplam = Dt.Compute("Sum(net_tutar)", "iptal <> 1")
        Catch ex As Exception
        End Try


        If hb.ref = 0 Then
            hb.sube = ay.sube
            hb.suberef = ay.suberef
            hb.sayac = SayacAl_StokBaslik()
            hb.guid = Guid.NewGuid.ToString
            hb.tarih = DpTarih.DisplayDate
            hb.islem_tarihi = Now
            hb.createdate = Now
            hb.ref = Kayitsql(TblBaslik, hb, Conn, True)
            If hb.ref <> 0 Then
                KayitSayimSatir()
                Cikis()
            Else
                Msg("Kayıt İşlemi Hatalı!!", False, False, True)
            End If
        Else
            hb.modifieddate = Now
            If UpdateSql(TblBaslik, hb, Conn, True) Then
                KayitSayimSatir()
                Cikis()
            Else
                Msg("Kayıt İşlemi Hatalı!!", False, False, True)
            End If
        End If
    End Sub

    Private Sub KayitSayimSatir()
        'On Error Resume Next
        Dim sayac As Integer = 0

        For Each dr As DataRow In Ds.Tables(TblSatir).Rows
            hs = Nothing

            hs.ref = Chk_Null("ref", dr)
            hs.iptal = Chk_Null("iptal", dr)

            hs.baslik = hb.ref
            hs.upload = 0
            '  hs.tarih = xb.tarih   Satir Eklerken Tarih Giriliyor
            hs.satir = sayac

            hs.guid = Chk_Null("guid", dr)
            hs.urunguid = Chk_Null("urunguid", dr)
            hs.stokkodu = Chk_Null("stokkodu", dr)
            hs.urun_aciklama = Chk_Null("urun_aciklama", dr)
            hs.miktar_eldeki = Chk_Null("miktar_eldeki", dr)

            hs.birim_alt = Chk_Null("birim_alt", dr)
            hs.birim_alt_isaret = Chk_Null("birim_alt_isaret", dr)
            hs.bolen = Chk_Null("bolen", dr)
            hs.carpan = Chk_Null("carpan", dr)
            hs.fiyat = Chk_Null("fiyat", dr)
            hs.brut_tutar = Chk_Null("brut_tutar", dr)
            hs.net_tutar = Chk_Null("net_tutar", dr)

            hs.miktar_eldeki = Chk_Null("miktar_eldeki", dr)
            hs.miktar_fark = Chk_Null("miktar_fark", dr)
            hs.miktar = Chk_Null("miktar", dr)



            If hs.miktar > 0 Then
                If hs.ref = 0 Then
                    If hs.iptal = False Then
                        hs.guid = Guid.NewGuid.ToString
                        hs.baslikguid = hb.guid
                        hs.createdate = Now

                        If Kayitsql(TblSatir, hs, Conn, True) Then
                            Cikis()
                        Else
                            Msg("Kayıt İşlemi Hatalı!!", False, False, True)
                        End If
                    End If
                Else
                    If hs.iptal = True Then
                        Execute_run("delete from hareket_satir where ref = '" & hs.ref & "'", Conn, True)
                    Else
                        hs.baslikguid = hb.guid
                        hs.modifieddate = Now
                        If UpdateSql(TblSatir, hs, Conn, True) Then
                            Cikis()
                        Else
                            Msg("Kayıt İşlemi Hatalı!!", False, False, True)
                        End If
                    End If
                End If
            End If
            If hb.sayim_turu = "Düzeltme Sayımı" And hb.onay = False Then
                KayitStokMiktar(hs.urunguid, hs.miktar)
            End If

            sayac += 1

        Next
    End Sub

    Dim FisTuruAciklama As String
    Dim IslemYonu As Integer

    Private Sub FisturuAl()
        TblSatir = ""
        TblBaslik = ""
        Select Case Fis_Stok
            Case stok_fisleri.SayimFisi
                TbBaslik.Text = "Stok Fişler: SAYIM FİŞİ"
                FisTuruAciklama = "MalAlim"
                TblSatir = hareket_satir
                TblBaslik = hareket_baslik
            Case Else
                MsgBox("Fiş Türü Yok")
        End Select
    End Sub


    Dim Dt As New DataTable
    Dim Dv As New DataView


    Private Sub EkranGetir()
        DatasetOkuFisStok(hb.ref)
        If hb.ref = 0 Then hb.belgeno = SayacAl_SayimBaslik()
        TbFisNo.Text = hb.belgeno
        CbSayimTuru.Text = hb.sayim_turu

        If hb.tarih = Nothing Then
            hb.tarih = Now
            DpTarih.SelectedDate = Now
        Else
            DpTarih.SelectedDate = hb.tarih
        End If

        LbNetTutar.Content = fiyat_al(hb.net_toplam)

        Try
            DgListe.Focus()
            Dim Obj = DgListe.Items(DgListe.Items.Count - 1)
            DgListe.ScrollIntoView(Obj)
            DgListe.SelectedItem = Obj
        Catch ex As Exception
        End Try
        SatirSec()
    End Sub

    Dim TblSatir As String = ""
    Dim TblBaslik As String = ""

    Private Sub Wpf_FisStok_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        FisturuAl()
        EkranDizayn_ThreeDBorderWindow(Me)
        Olay_Ekle()

        CbSayimTuru.Items.Add("Düzeltme Sayımı")
        CbSayimTuru.Items.Add("Genel Sayım")
        CbSayimTuru.Text = "Genel Sayım"
        EkranGetir()
    End Sub

    Private Sub TextBlock_Degistir(sender As Object, e As RoutedEventArgs)
        Tb.Background = Brushes.Beige
        Tb = sender
        Tb.Background = Brushes.LightPink
    End Sub

End Class
