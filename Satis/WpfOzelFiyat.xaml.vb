Public Class WpfOzelFiyat
    Dim ok As Boolean = False

    Private Sub OlayEkle()
        AddHandler BtKayit.Click, AddressOf Cevap
        AddHandler BtCikis.Click, AddressOf Cevap

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
        AddHandler BtVirgul.Click, AddressOf Numarator
    End Sub



    Private Sub Cevap(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case BtKayit.Name
                Me.Close()
            Case BtCikis.Name
                TbFiyat.Text = 0
                Me.Close()
        End Select
    End Sub

    Private Sub Numarator(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Select Case Btn.Name
            Case "Bt0"
                Me.TbFiyat.Text &= "0"
            Case "Bt1"
                Me.TbFiyat.Text &= "1"
            Case "Bt2"
                Me.TbFiyat.Text &= "2"
            Case "Bt3"
                Me.TbFiyat.Text &= "3"
            Case "Bt4"
                Me.TbFiyat.Text &= "4"
            Case "Bt5"
                Me.TbFiyat.Text &= "5"
            Case "Bt6"
                Me.TbFiyat.Text &= "6"
            Case "Bt7"
                Me.TbFiyat.Text &= "7"
            Case "Bt8"
                Me.TbFiyat.Text &= "8"
            Case "Bt9"
                Me.TbFiyat.Text &= "9"
            Case "BtVirgul"
                If TbFiyat.Text.IndexOf(",") <= 0 Then
                    Me.TbFiyat.Text &= ","
                End If
        End Select
    End Sub

    Dim Fiyat As Decimal = 0

    Function main() As Decimal
        Me.ShowDialog()
        If IsNumeric(TbFiyat.Text) Then
            Fiyat = Convert.ToDecimal(TbFiyat.Text)
            Return Fiyat
        End If
        Return 0
    End Function

    Private Sub WpfOzelFiyat(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        OlayEkle()
    End Sub
End Class
