Imports System.ComponentModel
Imports System.Windows.Threading

Public Class WpfListeSube

    Private Sub OlayEkle()
        AddHandler DgListe.MouseDoubleClick, AddressOf IslemSec
        AddHandler DgListe.KeyDown, AddressOf GridIslemSec
        AddHandler TbSorgu.TextChanged, AddressOf Sorgula
    End Sub

    Private Sub klavye_ac()
        Dim proc As New System.Diagnostics.Process
        proc = System.Diagnostics.Process.Start("TabTip.exe")
    End Sub


    Dim Dv As New System.Data.DataView

    Dim ListeSatirSayisi As Integer = 0


    Private Sub Listele()
        '    On Error Resume Next
        Dim Dt As New System.Data.DataTable
        ' Genel olarak var ama ikinci listelemede hata veriyor
        TbBaslik.Text = "ŞUBE LİSTESİ"
        Dim Sql As String = "select ref,subeno,aciklama,iptal from sube where iptal <> 1 order by aciklama"
        Ds_read(Sql, sube, Conn, True, True)
        Dt = Ds.Tables(sube)
        Dv.Table = Dt
        DgListe.ItemsSource = Dv
        Dv.RowFilter = "iptal <> 1"

        DgListe.Columns(0).Visibility = Windows.Visibility.Hidden
        GridColString(DgListe, 1, 0, "ŞUBE NO", TextAlignment.Left, 200)
        GridColString(DgListe, 2, 1, "AÇIKLAMA", TextAlignment.Left, 400)
        DgListe.Columns(3).Visibility = Windows.Visibility.Hidden

        DgListe.IsReadOnly = True
        DgListe.UpdateLayout()
    End Sub


    Dim sira As Integer = 0


    Private Sub GridIslemSec(sender As Object, e As Input.KeyEventArgs)
        Select Case e.Key
            Case Key.Enter
                Sec()
        End Select
    End Sub

    Private Sub IslemSec(sender As Object, e As RoutedEventArgs)
        Sec()
    End Sub



    Private Sub Sec()
        Dim ref As New Integer
        Try
            sira = DgListe.SelectedIndex
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        Try
            ref = DgListe.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            DegiskenAl(sube, ref, True)
            Me.Close()
        Else
            Msg("Seçim Yapınız!!", False, False, True)
        End If

    End Sub



    Private Sub Cikis()
        Try
            Ds.Tables.Remove("liste")
        Catch ex As Exception
        End Try
        Me.Close()
    End Sub

    Function MenuYap() As Grid
        Dim cik As Boolean = True
        Dim secim As Boolean = True
        Dim Grd As New Grid
        Dim sayi As Integer

        If secim Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New System.Windows.Controls.Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("sec", "SEÇ", 0, sayi, Brushes.Lime, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf Sec
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        If cik Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New System.Windows.Controls.Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("cikis", "ÇIKIŞ", 0, sayi, Brushes.IndianRed, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf Cikis
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        Grid.SetColumnSpan(Grd, 3)
        Grid.SetRow(Grd, 3)
        Return Grd
    End Function

    Private Sub Sorgula()
        On Error Resume Next
        If SorguName = "" Then Exit Sub
        Select Case SorguType
            Case "System.String"
                Dv.RowFilter = SorguName & " like '%" & TbSorgu.Text & "%'"
            Case "System.Decimal"
                If IsNumeric(TbSorgu.Text) Then
                    Dv.RowFilter = SorguName & " > " & TbSorgu.Text
                End If
        End Select

    End Sub

    Private Sub SatirAsagi()
        Dim s As Integer = 0
        Try
            s = DgListe.SelectedIndex
        Catch ex As Exception
        End Try
        s += 1
        Try
            Dim Obj = DgListe.Items(s)
            DgListe.ScrollIntoView(Obj)
            DgListe.SelectedItem = Obj
        Catch ex As Exception
        End Try
        DgListe.Focus()
    End Sub

    Private Sub SatirYukari()
        Dim s As Integer = 0
        Try
            s = DgListe.SelectedIndex
        Catch ex As Exception
        End Try
        s -= 1
        Try
            DgListe.Focus()
            Dim Obj = DgListe.Items(s)
            DgListe.ScrollIntoView(Obj)
            DgListe.SelectedItem = Obj
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Liste_KeyDown(sender As Object, e As Input.KeyEventArgs) Handles Me.KeyDown
        Select Case e.Key
            Case Key.Enter
                Sec()
        End Select
    End Sub

    Dim SorguName As String = ""
    Dim SorguType As String




    Private Sub Grid_Sorting(ByVal sender As Object, ByVal e As DataGridSortingEventArgs) Handles DgListe.Sorting
        Dim direction As ListSortDirection = If((e.Column.SortDirection <> ListSortDirection.Ascending), ListSortDirection.Ascending, ListSortDirection.Descending)
        Dim aa As Integer = e.Column.DisplayIndex
        Dim header_ As String = DgListe.Columns(aa).Header.ToString
        SorguName = e.Column.SortMemberPath
        TbSorguBolum.Text = header_
        TbSorgu.Text = ""
    End Sub


    Public Delegate Sub NextPrimeDelegate()


    Private Sub Wpf_Liste_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        EkranDizayn_None(Me)
        GrMenu.Children.Add(MenuYap)
        OlayEkle()
        DgListe.Dispatcher.BeginInvoke(DispatcherPriority.Normal, New NextPrimeDelegate(AddressOf Listele))
        TbSorgu.Focus()
        'Dim Obj = DgListe.Items(0)
        'DgListe.ScrollIntoView(Obj)
        'DgListe.SelectedItem = Obj

    End Sub
End Class
