Public Class WpfMsgBx
    Dim ok As Boolean = False
    Function main(Txt As String, ByVal BtnEvet As Boolean, ByVal BtnHayir As Boolean, ByVal BtnTamam As Boolean) As Boolean
        If BtnEvet Then
            BtEvet.Visibility = Windows.Visibility.Visible
            SpSoru.Visibility = Windows.Visibility.Visible
        Else
            BtEvet.Visibility = Windows.Visibility.Hidden
            SpSoru.Visibility = Windows.Visibility.Hidden
        End If
        If BtnHayir Then
            BtHayir.Visibility = Windows.Visibility.Visible
            SpSoru.Visibility = Windows.Visibility.Visible
        Else
            BtHayir.Visibility = Windows.Visibility.Hidden
            SpSoru.Visibility = Windows.Visibility.Hidden
        End If
        If BtnTamam Then
            BtTamam.Visibility = Windows.Visibility.Visible
            SpUyari.Visibility = Windows.Visibility.Visible
        Else
            BtTamam.Visibility = Windows.Visibility.Hidden
            SpUyari.Visibility = Windows.Visibility.Hidden
        End If
        LbSoru.Text = Txt
        Me.ShowDialog()
        Return ok
    End Function
    Private Sub Cevap(sender As Object, e As RoutedEventArgs)
        If sender.name = BtEvet.Name Then
            ok = True
        End If
        If sender.name = BtHayir.Name Then
            ok = False
        End If
        If sender.name = BtTamam.Name Then
            ok = True
        End If
        Cikis()
    End Sub

    Private Sub Cikis()
        Me.Close()
    End Sub

    Private Sub WpMsgBx_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Topmost = True
        AddHandler BtEvet.Click, AddressOf Cevap
        AddHandler BtHayir.Click, AddressOf Cevap
        AddHandler BtTamam.Click, AddressOf Cevap
    End Sub
End Class
