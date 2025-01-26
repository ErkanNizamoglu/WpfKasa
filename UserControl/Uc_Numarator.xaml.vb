Namespace Uc_Numarator
    Public Class UcNumarator

        Public Event NumaratorClick As RoutedEventHandler

        Private Sub BtClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
            RaiseEvent NumaratorClick(sender, e)
        End Sub

    End Class
End Namespace

