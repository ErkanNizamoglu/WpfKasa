Public Class W_EkranKilit

    Private Sub Olaylar()
        AddHandler TbSifre.GotFocus, AddressOf ElemanSec
        AddHandler BtTamam.Click, AddressOf Kullanici_Kontrol
    End Sub


    Private Sub Kullanici_Kontrol()
        If Kl.ref <> 0 Then
            DegiskenAl(Kullanici, Kl.ref, True)
            If Kl.sifre = sifre(Kl.kod & TbSifre.Password) Then
                Me.Close()
            Else
                Me.Topmost = False
                Me.Opacity = 0

                Msg("Şifre Hatalı", False, False, True)
                Me.Opacity = 1

                Me.Topmost = True
                TbSifre.Password = ""
            End If
        Else
            Msg("Kullanici Seciniz!!", False, False, True)
        End If
    End Sub

    Private Sub KlavyeSec(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim Btn As Button = sender
        Klavye(Btn, Nothing)
    End Sub


    Sub Main()
        ShowInTaskbar = False
        Me.Topmost = True
        Olaylar()
        Me.ShowDialog()
    End Sub

    Private Sub W_EkranKilit_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        TbSifre.Focus()
    End Sub
End Class
