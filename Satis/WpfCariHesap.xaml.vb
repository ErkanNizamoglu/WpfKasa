Public Class WpfCariHesap
    Dim ok As Boolean = False
    Public Const eArşivFatura As String = "e-Arşiv Fatura"
    Public Const eFatura As String = "e-Fatura"

    Private Sub OlayEkle()
        AddHandler BtTemizle.Click, AddressOf EkranTemizle
        AddHandler BtKabul.Click, AddressOf Kabul
        AddHandler BtSorgula.Click, AddressOf Sorgula
        AddHandler BtCikis.Click, AddressOf Cikis


        'AddHandler TbFiyat.KeyUp, AddressOf FiyatText

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
        AddHandler BtSil.Click, AddressOf Numarator
    End Sub

    Dim MukellefIsım As String = ""
    Dim MukellefVno As String = ""

    Private Sub EkranTemizle()
        TbVergiNo.Text = ""
        TbAciklama.Text = ""
        TbAdres.Text = ""
        CbFaturaTuru.Text = ""
    End Sub

    Private Sub KlavyeSec(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Klavye(Btn, Nothing)
    End Sub

    Private Sub MusteriSorgula()
        TbAciklama.Text = ""
        TbAdres.Text = ""

        Dim C_VrbEfatura As New CVeriban_Efatura

        If C_VrbEfatura.BAGLANTI_TESTI Then
            Dim VergiNo As String = TbVergiNo.Text
            If IsNumeric(VergiNo) Then

                Select Case C_VrbEfatura.MUKELLEF_GIB_MUKELLEF_LISTESINDE_VARMI(VergiNo)
                    Case KayitDurumu.Evet
                        EfaturaMusterisi = True
                        MukellefIsım = C_VrbEfatura.MUKELLEF_ISIM_BILGISI(VergiNo)
                        CbFaturaTuru.Text = "EFATURA MÜKELLEFİ"
                        TbAciklama.Text = MukellefIsım
                    Case KayitDurumu.Hayir
                        EfaturaMusterisi = False
                        CbFaturaTuru.Text = "EFATURA DEĞİL"
                    Case KayitDurumu.Belirsiz

                End Select
            End If


        End If
    End Sub

    Private Sub Kabul(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If EfaturaMusterisi Then
            MukellefVno = TbVergiNo.Text
            cr.aciklama = MukellefIsım
            cr.vn = MukellefVno
            hb.ebelgegonderim = True
            hb.belgeturu = eFatura
            hb.fisturu = 1
            hb.efaturaturu = belge_turu.efatura
            cr.ebelge = eFatura
        Else
            cr.aciklama = "Nihai Tüketici"
            cr.vn = "11111111111"
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

    Dim Fiyat As Decimal = 0

    Dim EfaturaMusterisi As Boolean = False
    Function Main() As Object
        OlayEkle()
        EkranDizayn_None(Me)
        Me.ShowDialog()
        If EfaturaMusterisi Then
            cr.aciklama = TbAciklama.Text
            cr.vn = TbVergiNo.Text
            Return cr
        End If
        Return Nothing
    End Function

    Private Sub WpfOzelFiyat(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    End Sub
End Class
