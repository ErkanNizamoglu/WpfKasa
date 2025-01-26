Namespace Uc_Klavye
    Public Class UcKlavye

        Public Event ButtonClick As RoutedEventHandler

        Private Sub BtClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            RaiseEvent ButtonClick(sender, e)
        End Sub

    End Class
End Namespace

