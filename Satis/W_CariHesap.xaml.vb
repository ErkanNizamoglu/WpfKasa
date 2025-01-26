Public Class W_CariHesap
    Dim ok As Boolean = False
    Public Const eArşivFatura As String = "e-Arşiv Fatura"
    Public Const eFatura As String = "e-Fatura"

    Private Sub OlayEkle()
        AddHandler BtTemizle.Click, AddressOf EkranTemizle
        AddHandler BtKabul.Click, AddressOf Kabul
        AddHandler BtSorgula.Click, AddressOf Sorgula
        AddHandler BtCikis.Click, AddressOf Cikis

        AddHandler TbVergiNo.GotFocus, AddressOf ElemanSec
        AddHandler TbVergiDairesi.GotFocus, AddressOf ElemanSec
        AddHandler TbAdi.GotFocus, AddressOf ElemanSec
        AddHandler TbSoyAdi.GotFocus, AddressOf ElemanSec
        AddHandler TbUnvani.GotFocus, AddressOf ElemanSec
        AddHandler TbAdresi.GotFocus, AddressOf ElemanSec
        AddHandler Tbilce.GotFocus, AddressOf ElemanSec

        AddHandler TbSehir.GotFocus, AddressOf ElemanSec
        AddHandler CbMukellefTipi.SelectionChanged, AddressOf MukelleftipiDegistir

        'AddHandler TbFiyat.KeyUp, AddressOf FiyatText

        'AddHandler Bt0.Click, AddressOf Numarator
        'AddHandler Bt1.Click, AddressOf Numarator
        'AddHandler Bt2.Click, AddressOf Numarator
        'AddHandler Bt3.Click, AddressOf Numarator
        'AddHandler Bt4.Click, AddressOf Numarator
        'AddHandler Bt5.Click, AddressOf Numarator
        'AddHandler Bt6.Click, AddressOf Numarator
        'AddHandler Bt7.Click, AddressOf Numarator
        'AddHandler Bt8.Click, AddressOf Numarator
        'AddHandler Bt9.Click, AddressOf Numarator
        'AddHandler BtSil.Click, AddressOf Numarator
    End Sub

    Dim MukellefIsim As String = ""
    Dim MukellefVno As String = ""

    Private Sub EkranTemizle()
        TbVergiNo.Text = ""
        TbUnvani.Text = ""
        TbAdi.Text = ""
        TbAdresi.Text = ""
        CbFaturaTuru.Text = ""
    End Sub

    Private Sub MukelleftipiDegistir()
        Select Case CbMukellefTipi.SelectedIndex
            Case 0 ' "KURUMSAL TİCARİ"
                '****************************************
                BrTbAdi.Visibility = Visibility.Hidden
                BrLbAdi.Visibility = Visibility.Hidden
                BrTbSoyadi.Visibility = Visibility.Hidden
                BrLbSoyadi.Visibility = Visibility.Hidden
                '****************************************
                '****************************************
                BrTbUnvani.Visibility = Visibility.Visible
                BrLbUnvan.Visibility = Visibility.Visible
                '****************************************
            Case 1 ' "ŞAHIS TİCARİ"
                '****************************************
                BrTbAdi.Visibility = Visibility.Visible
                BrLbAdi.Visibility = Visibility.Visible
                BrTbSoyadi.Visibility = Visibility.Visible
                BrLbSoyadi.Visibility = Visibility.Visible
                '****************************************
                '****************************************
                BrTbUnvani.Visibility = Visibility.Hidden
                BrLbUnvan.Visibility = Visibility.Hidden
                '****************************************
            Case Else

        End Select
    End Sub


    Private Sub MusteriSorgula()
        cr = Nothing
        TbUnvani.Text = ""
        TbAdresi.Text = ""
        TbVergiDairesi.Text = ""

        If TbVergiNo.Text = "" Then
            Msg("Vergi Numarası Giriniz!", False, False, True)
            Exit Sub
        End If
        Dim C_VrbEfatura As New CVeriban_Efatura
        If C_VrbEfatura.BAGLANTI_TESTI Then
            Dim VergiNo As String = TbVergiNo.Text
            If VergiNo <> "" Then
                Select Case C_VrbEfatura.MUKELLEF_GIB_MUKELLEF_LISTESINDE_VARMI(VergiNo)
                    Case KayitDurumu.Evet
                        EfaturaMusterisi = True
                        CbFaturaTuru.SelectedIndex = 0
                        MukellefIsim = C_VrbEfatura.MUKELLEF_ISIM_BILGISI(VergiNo)
                        '     CbFaturaTuru.Text = "EFATURA MÜKELLEFİ"
                        TbUnvani.Text = MukellefIsim
                        hb.cari_kod = MukellefIsim
                        hb.cari_vn = VergiNo
                    Case KayitDurumu.Hayir
                        EfaturaMusterisi = False
                        CbFaturaTuru.SelectedIndex = 1
                        '   CbFaturaTuru.Text = "EFATURA DEĞİL"
                        hb.cari_kod = MukellefIsim
                        hb.cari_vn = VergiNo
                    Case KayitDurumu.Belirsiz
                End Select
                Try
                    cr.ref = Execute_Oku("Select ref from cari where vn = '" & VergiNo & "'", "ref", Conn, False)
                Catch ex As Exception
                End Try

                If cr.ref > 0 Then
                    cr = DegiskenAl(Cari, cr.ref, True)
                    TbVergiDairesi.Text = cr.vd
                    Tbilce.Text = cr.ilce
                    TbSehir.Text = cr.sehir
                    TbAdresi.Text = cr.adres1 & vbCrLf & cr.adres2
                End If
            End If
        End If
    End Sub

    Private Sub Kabul(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If EfaturaMusterisi Then
            MukellefVno = TbVergiNo.Text
            cr.aciklama = MukellefIsim
            cr.vn = MukellefVno
            hb.cari_kod = MukellefIsim
            hb.cari_vn = cr.vn
            hb.ebelgegonderim = True
            hb.belgeturu = eFatura
            hb.fisturu = 1
            hb.efaturaturu = belge_turu.efatura
            cr.ebelge = eFatura
        Else
            If cr.aciklama = "" Then cr.aciklama = "Nihai Tüketici"
            If cr.vn = "" Then
                cr.vn = "11111111111"
            Else
                hb.fisturu = 1
            End If

            hb.cari_kod = cr.aciklama
            hb.cari_vn = cr.vn
            hb.belgeturu = eArşivFatura
            hb.efaturaturu = belge_turu.earsiv
        End If
        Me.Close()
    End Sub

    Private Sub Sorgula(ByVal sender As Object, ByVal e As RoutedEventArgs)
        MusteriSorgula()
    End Sub

    Private Sub Cikis(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TbVergiNo.Text = ""
        Me.Close()
    End Sub

    Private Sub Numarator(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case "Bt0"
                Me.TbVergiNo.Text &= "0"
            Case "Bt1"
                Me.TbVergiNo.Text &= "1"
            Case "Bt2"
                Me.TbVergiNo.Text &= "2"
            Case "Bt3"
                Me.TbVergiNo.Text &= "3"
            Case "Bt4"
                Me.TbVergiNo.Text &= "4"
            Case "Bt5"
                Me.TbVergiNo.Text &= "5"
            Case "Bt6"
                Me.TbVergiNo.Text &= "6"
            Case "Bt7"
                Me.TbVergiNo.Text &= "7"
            Case "Bt8"
                Me.TbVergiNo.Text &= "8"
            Case "Bt9"
                Me.TbVergiNo.Text &= "9"
            Case "BtSil"
                Try
                    Me.TbVergiNo.Text = Mid(Me.TbVergiNo.Text, 1, Me.TbVergiNo.Text.Length - 1)
                Catch ex As Exception
                End Try
                '  If TbVergiNo.Text.IndexOf(",") <= 0 Then
                '  Me.TbVergiNo.Text &= ","
                ' End If
        End Select
    End Sub

    Dim KlavyeOk As Boolean = False

    Private Sub KlavyeSec(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Klavye(Btn, Nothing)
    End Sub

    Private Sub Ekran_Getir()
        TbAdi.Text = "" '"cr.aciklama
        TbSoyAdi.Text = "" ' cr.aciklama
        TbVergiDairesi.Text = cr.vd
        TbVergiNo.Text = cr.vn
        TbAdresi.Text = cr.adres1 & vbCrLf & cr.adres2
        TbSehir.Text = cr.sehir
        TbUnvani.Text = cr.aciklama
        Tbilce.Text = cr.ilce
    End Sub

    Dim Fiyat As Decimal = 0

    Dim EfaturaMusterisi As Boolean = False
    Function Main() As Object
        OlayEkle()
        EkranDizayn_None(Me)

        Me.ShowDialog()
        If EfaturaMusterisi Then
            cr.aciklama = TbUnvani.Text
            cr.vn = TbVergiNo.Text
            Return cr
        End If
        Return Nothing
    End Function

    Private Sub WpfCariHesap(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Ekran_Getir()
        CbMukellefTipi.SelectedIndex = 0 ' cr.mukelleftipi
        CbFaturaTuru.SelectedIndex = 0 ' cr.faturaturu
    End Sub

    Private Sub TbVergiNo_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TbVergiNo.TextChanged

    End Sub
End Class
