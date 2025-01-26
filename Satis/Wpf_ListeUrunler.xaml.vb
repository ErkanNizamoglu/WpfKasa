Imports System.ComponentModel
Imports System.Windows.Threading

Public Class Wpf_ListeUrunler

    Private Sub OlayEkle()
        AddHandler DgListe.KeyDown, AddressOf GridIslemSec
        AddHandler DgListe.MouseDoubleClick, AddressOf Sec
        AddHandler TbSorgu.TextChanged, AddressOf Sorgula
        AddHandler CbSorgu.SelectionChanged, AddressOf SorguDegistir
    End Sub

    Private Sub Klavye_ac()
        Dim proc As New System.Diagnostics.Process
        proc = System.Diagnostics.Process.Start("TabTip.exe")
    End Sub


    Dim Dv As New System.Data.DataView

    Dim ListeSatirSayisi As Integer = 0

    Dim GrupRef As Integer
    Dim GrupGuid As String
    Dim GrupIndex As Integer = 0
    Private Sub GrupListele()
        Grgrup.Children.Clear()
        Grgrup.RowDefinitions.Clear()

        Dim sayac As Integer = 0
        For Each dr As System.Data.DataRow In Ds.Tables(Grup).Select("active = 1 and goster = 1")
            Dim ref As Long = dr.Item("ref")

            If GrupRef = 0 Then
                GrupRef = Ref
                GrupGuid = dr.Item("guid")
            End If
            Dim GrRowDef As New RowDefinition
            Grgrup.RowDefinitions.Add(GrRowDef)
            Dim Bt As New System.Windows.Controls.Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt.VerticalAlignment = Windows.VerticalAlignment.Center
            'Dim style As Style = FindResource("RoundedButton")
            Dim style As Style = FindResource("MetroButton")
            'Dim style As Style = FindResource("BlueGlassButtonStyle")
            Bt.Style = style
            Bt.Margin = New Thickness(1, 1, 1, 1)
            Bt.Background = Brushes.LightGray
            AddHandler Bt.Click, AddressOf GrupSecim
            Dim VbBt As New Viewbox
            Dim TbBt As New TextBlock
            TbBt.Width = 90
            TbBt.FontSize = 15
            'TbBt.TextWrapping = TextWrapping.Wrap
            TbBt.Text = Chk_Null("aciklama", dr)
            Bt.Uid = Ref
            VbBt.Child = TbBt
            VbBt.Margin = New Thickness(1, 1, 1, 1)
            Bt.Content = VbBt

            If Ref = 0 Then
                GrupRef = Ref
            Else
                If GrupRef = Ref Then
                    Bt.Background = Brushes.Lime
                    GrupButon = Bt
                    GrupIndex = sayac
                End If
            End If
            Grid.SetRow(Bt, sayac)
            Grgrup.Children.Add(Bt)
            sayac += 1
        Next
    End Sub

    Dim GrupButon As New Button

    Private Sub GrupSecim(ByVal sender As Object, ByVal e As System.EventArgs)
        GrupButon.Background = Brushes.LightGray
        GrupButon = sender
        GrupButon.Background = Brushes.Lime
        On Error Resume Next
        Dim Btn As Button = sender
        GrupRef = Mid(Btn.Name, 2, Btn.Name.Length - 1)
        For Each Dr As System.Data.DataRow In Ds.Tables(Grup).Select("ref = '" & GrupRef & "'")
            GrupGuid = Dr.Item("guid")
        Next
        If GrupGuid > 0 Then Sorgula()
    End Sub


    Private Sub DatasetAlUrunlerListe()
        Dim Sql As String = ""
        Sql &= " SELECT"
        Sql &= " urunler.stokkodu as stokkodu"
        Sql &= ",urunler.grupguid as grupguid"
        Sql &= ",urunler.aciklama as aciklama"
        Sql &= ",birim_ust.kod as birim_ust"
        Sql &= ",birim_alt.isaret as birim_alt"
        Sql &= ",barkodlar.barkod as barkod"
        Sql &= ",urunler.fiyat1 as fiyat"
        Sql &= ",urunler.fiyat_alis as fiyat_alis"
        Sql &= ",CAST(urunler.stok_miktar as decimal(15,3)) as stok_miktar"
        Sql &= ",urunler.ozelkod as ozelkod"
        Sql &= ",urunler.iptal as iptal"
        Sql &= ",urunler.ref as ref"
        Sql &= ",birim_ust.ref as birim_ust_ref"
        Sql &= ",birim_alt.ref as birim_alt_ref"
        Sql &= ",urunler.ozellik as ozellik"
        Sql &= ",urunler.guid as urunguid"
        Sql &= ",urunler.ozellik as ozellik"
        Sql &= " FROM"
        Sql &= " Urunler"
        Sql &= " inner join barkodlar ON urunler.ref = barkodlar.urun"
        Sql &= " inner join birim_alt on birim_alt.ref = barkodlar.birim_alt"
        Sql &= " inner join birim_ust on birim_ust.ref = urunler.birim_ust"
        Sql &= " where "
        Sql &= " urunler.iptal <> 1 and"
        ' Sql &= " urunler.ozellik <> 1 and"
        Sql &= " barkodlar.sira = 1"
        Sql &= " order by urunler.aciklama"
        Ds_read(Sql, Ds, "liste", Conn, True)
    End Sub





    Private Sub ColonAyarla()
        TbBaslik.Text = "ÜRÜN LİSTESİ"
        DgListe.Columns.Clear()

        Dim Dt As System.Data.DataTable = Ds.Tables("liste")

        Dv.Table = Dt
        Dv.RowFilter = "iptal <> 1  And grupguid = '" & GrupGuid & "'"
        DgListe.ItemsSource = Dv



        GridColString(DgListe, 0, 0, "STOKKODU", TextAlignment.Left, 150)
        'GridColString(DgListe, 1, 1, "GRUP", TextAlignment.Left, 200)
        GridColString(DgListe, 2, 2, "AÇIKLAMA", TextAlignment.Left, 400)
        DgListe.Columns(3).Visibility = Windows.Visibility.Hidden
        GridColString(DgListe, 4, 4, "BİRİM", TextAlignment.Left, 100)
        GridColString(DgListe, 5, 5, "BARKOD", TextAlignment.Left, 200)
        GridColTL(DgListe, 6, 6, "SATIŞ FİYATI", TextAlignment.Right, 150)
        GridColTL(DgListe, 7, 7, "ALIŞ FİYATI", TextAlignment.Right, 150)
        If ay.stokgoster_aktif Then
            GridColNumeric(DgListe, 8, 8, "STOK MİKTARI", TextAlignment.Right, 150)
        Else
            DgListe.Columns(7).Visibility = Windows.Visibility.Hidden
        End If
        DgListe.Columns(1).Visibility = Windows.Visibility.Hidden ' Grup
        DgListe.Columns(7).Visibility = Windows.Visibility.Hidden ' Alis Fiyatı
        DgListe.Columns(9).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(10).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(11).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(12).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(13).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(14).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(15).Visibility = Windows.Visibility.Hidden
        DgListe.Columns(16).Visibility = Windows.Visibility.Hidden

        Dim DgGridEn As Integer = DgListe.RenderSize.Width
        TbSorgu.Width = DgGridEn
        SorguName = "aciklama"

        CbSorgu.Items.Add("AÇIKLAMA")
        CbSorgu.Items.Add("BARKOD")
        CbSorgu.Items.Add("STOKKODU")
        CbSorgu.Text = "AÇIKLAMA"
        SorguType = "System.String"
        TbSayi.Text = "Ürün Sayısı:" & Dv.Count
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
            DegiskenAl(Urunler, ref, True)
            Cikis()
        Else
            Msg("Seçim Yapınız!!", False, False, True)
        End If

    End Sub

    Private Sub Cikis()
        Me.Close()
    End Sub

    Function MenuYap(secim As Boolean, cik As Boolean) As Grid

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

    Dim ListeTbl As String = ""
    Dim MenuSec As Boolean




    Private Sub Sorgula()
        On Error Resume Next
        If SorguName = "" Then Exit Sub
        Select Case SorguType
            Case "System.String"
                Dv.RowFilter = SorguName & " like '%" & TbSorgu.Text & "%' and iptal <> 1 And grupguid = '" & GrupGuid & "'"
            Case "System.Decimal"
                If IsNumeric(TbSorgu.Text) Then
                    Dv.RowFilter = SorguName & " > " & TbSorgu.Text & " and iptal <> 1 And grupguid = '" & GrupGuid & "'"
                End If
        End Select
        TbSayi.Text = "Ürün Sayısı:" & Dv.Count
    End Sub

    Private Sub SorguDegistir()
        On Error Resume Next
        TbSorgu.Focus()
        Select Case CbSorgu.SelectedValue
            Case "AÇIKLAMA"
                SorguName = "aciklama"
            Case "BARKOD"
                SorguName = "barkod"
            Case "STOKKODU"
                SorguName = "stokkodu"
            Case Else
                MsgBox("Sorgu Açıklaması Yok")
        End Select
        If SorguName = "" Then Exit Sub
        Select Case SorguType
            Case "System.String"
                Dv.RowFilter = SorguName & " like '%" & TbSorgu.Text & "%' and grupguid = '" & GrupGuid & "'"
            Case "System.Decimal"
                If IsNumeric(TbSorgu.Text) Then
                    Dv.RowFilter = SorguName & " > " & TbSorgu.Text & " and grupguid = '" & GrupGuid & "'"
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
            Case Key.Down
                '    SatirAsagi()
            Case Key.Back
        End Select
    End Sub

    Dim SorguName As String = ""
    Dim SorguType As String


    Private Sub Grid_Sorting(ByVal sender As Object, ByVal e As DataGridSortingEventArgs) Handles DgListe.Sorting
        Dim direction As ListSortDirection = If((e.Column.SortDirection <> ListSortDirection.Ascending), ListSortDirection.Ascending, ListSortDirection.Descending)
        Dim aa As Integer = e.Column.DisplayIndex
        Dim header_ As String = DgListe.Columns(aa).Header.ToString
        SorguName = e.Column.SortMemberPath
        CbSorgu.Text = header_
        TbSorgu.Text = ""
    End Sub


    Public Delegate Sub NextPrimeDelegate()

    ' Dim Ds As New System.Data.DataSet
    ' Dim ilk As Boolean

    Private Sub Wpf_Liste_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ' System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        EkranDizayn_ThreeDBorderWindow(Me)
        OlayEkle()
        GrupListele()
        DatasetAlUrunlerListe()
        ColonAyarla()
        GrMenu.Children.Add(MenuYap(True, True))
        TbSorgu.Focus()
        'Dim Obj = DgListe.Items(0)
        'DgListe.ScrollIntoView(Obj)
        'DgListe.SelectedItem = Obj
    End Sub
End Class
