Public Class W_GramajUrun


    Private Sub olay_ekle()
        AddHandler BtEvet.Click, AddressOf Kayit
        AddHandler BtHayir.Click, AddressOf Vazgec
        AddHandler TbGramaj.KeyUp, AddressOf GramajKeydown

    End Sub


    Private Sub Kayit()

    End Sub

    Private Sub Vazgec()

    End Sub


    Private Sub GramajKeydown()
        On Error Resume Next
        If TbGramaj.Text <> "" Then
            If IsNumeric(TbGramaj.Text) Then
                Dim GrmStr As String = Replace(TbGramaj.Text, ".", ",")
                Dim GrmDec As Decimal = 0
                GrmDec = Convert.ToDecimal(GrmStr)
                If GrmDec = 0 Then
                    TbBirimFiyat.Content = fiyat_al(0)
                Else
                    TbBirimFiyat.Content = fiyat_al(GVd.Fiyat / GrmDec)
                End If
            End If
        End If
    End Sub


    Private Sub EkranYaz()

        CbGramajBirim.Items.Add("Adet")
        CbGramajBirim.Items.Add("Kilo")
        CbGramajBirim.Items.Add("Metre")
        CbGramajBirim.Items.Add("Litre")

        CbUlke.ItemsSource = UlkeListe()
        LbAciklama.Content = GVd.Aciklama
        TbGramaj.Text = GVd.Gramaj
        CbGramajBirim.Text = GVd.Gramaj_Birim_Aciklama
        TbBirimFiyat.Content = fiyat_al(GVd.Birim_Fiyat)
        CbUlke.Text = UlkeAl(GVd.Barkod)
        TbFiyat.Content = fiyat_al(GVd.Fiyat)
    End Sub

    Private Sub WpfGramajUrun() Handles Me.Loaded
        EkranYaz()
        olay_ekle()
    End Sub

End Class
