Public Class WpfFonksiyon
    Dim ok As Boolean = False

    Private Sub OlayEkle()
        AddHandler BtCikis.Click, AddressOf Cevap

        'AddHandler TbFiyat.KeyUp, AddressOf FiyatText

        AddHandler BtCikis.Click, AddressOf Olay

        AddHandler BtZRaporu.Click, AddressOf Olay
        AddHandler BtXRaporu.Click, AddressOf Olay
        AddHandler BtProgramdanCikis.Click, AddressOf Olay
        AddHandler BtAktarim.Click, AddressOf Olay
        AddHandler BtDisplay.Click, AddressOf Olay
        AddHandler BtPluTusSablonu.Click, AddressOf Olay
        AddHandler BtRaportUrunSatis.Click, AddressOf Olay
        AddHandler BtGecmis.Click, AddressOf Olay
        AddHandler BTSayim.Click, AddressOf Olay
        AddHandler BtKilit.Click, AddressOf Olay

        AddHandler BtUpgrade.Click, AddressOf Upgrade

    End Sub


    Private Sub Etiketleme()
        Dim W As New W_Etiketleme
        W.ShowDialog()
    End Sub

    Private Sub Upgrade()
        Dim C As New C_Upgrade_Kasa
        Dim Ok As Boolean = C.ChkUpGrade(System.Net.Dns.GetHostName(), True)
    End Sub

    Dim Rtn As Integer = 0
    Private Sub Olay(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender

        Select Case Btn.Name.ToString
            Case BtGecmis.Name.ToString
                Rtn = Fonksiyon.belgetekrar
            Case BTEtiketleme.Name.ToString
                Rtn = Fonksiyon.etiketleme
            Case BtKilit.Name.ToString
                Rtn = Fonksiyon.ekrankilit
            Case BtProgramdanCikis.Name.ToString
                Rtn = Fonksiyon.cikis
            Case BtZRaporu.Name.ToString
                Rtn = Fonksiyon.zraporu
            Case BtXRaporu.Name.ToString
                Rtn = Fonksiyon.xraporu
            Case BtAktarim.Name.ToString
                Rtn = Fonksiyon.aktarim
            Case BtDisplay.Name.ToString
                Rtn = Fonksiyon.display
            Case BtDisplay.Name.ToString
                Rtn = Fonksiyon.display
            Case BtRaportUrunSatis.Name.ToString
                Rtn = Fonksiyon.raporurunsatis
            Case BtPluTusSablonu.Name.ToString
                Rtn = Fonksiyon.plutussablonu
            Case BtCekmece.Name.ToString
                Rtn = Fonksiyon.cekmece
            Case BTSayim.Name.ToString
                Rtn = Fonksiyon.sayim
            Case BtDegisim.Name.ToString
                Rtn = Fonksiyon.degisim
        End Select
        Me.Close()
    End Sub

    Private Sub Cevap(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case BtCikis.Name
                Me.Close()
        End Select
    End Sub

    Dim AktarimOk As Boolean = False
    Function Main(ByVal aktarimOk_ As Boolean) As Integer
        AktarimOk = aktarimOk_
        Select Case aktarimOk_
            Case True
                TbAktarim.Text = "AKTARIM KAPAT"
            Case False
                TbAktarim.Text = "AKTARIM AÇ"
        End Select
        Rtn = 0
        OlayEkle()
        Me.ShowDialog()
        Return Rtn
    End Function

End Class
