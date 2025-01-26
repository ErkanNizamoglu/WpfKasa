Public Class WpfFiyatGor
    Dim ok As Boolean = False

    Private Sub OlayEkle()
        AddHandler BtCikis.Click, AddressOf Cikis
        AddHandler BtAktar.Click, AddressOf Cikis
    End Sub


    Dim Aktar As Boolean = False

    Private Sub Cikis(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As System.Windows.Controls.Button = sender
        Select Case Btn.Name
            Case BtCikis.Name.ToString
                Aktar = False
            Case BtAktar.Name.ToString
                Aktar = True
        End Select
        Me.Close()
    End Sub



    Dim Fiyat As Decimal = 0

    Function main() As Boolean
        Me.ShowDialog()
        Return Aktar

    End Function

    Private Sub EkranGoster()
        If Ur.guid <> "" Then
            For Each drUrun As System.Data.DataRow In Ds.Tables(Urunler).Select("urunguid = '" & Ur.guid & "'")
                Ur.aciklama = Chk_Null("urunaciklama", drUrun)
                Ur.fiyat1 = Chk_Null("urunfiyat", drUrun)
            Next
            TbFiyat.Text = fiyat_al(Ur.fiyat1)
            TbAciklama.Text = Ur.aciklama
        Else
            Console.Beep(3000, 450)
            TbAciklama.Text = "Ürün Bulunamadı!"
        End If
    End Sub

    Private Sub WpFiyatGor(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        OlayEkle()
        EkranGoster()
    End Sub
End Class
