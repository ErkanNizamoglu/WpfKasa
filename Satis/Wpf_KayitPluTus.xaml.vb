Imports System.Data
Imports System.IO
Imports System.Windows.Controls.Primitives
Imports System.Windows.Threading




Public Class Wpf_KayitPluTus

    ' Dim DtUrunlerCount As Integer = 0
    ' Dim DtBarkodlarCount As Integer = 0
    ' Dim errNumber As Integer = 0
    ' Dim RecordSize As Integer = 100000


    Private Sub Olaylar()
        AddHandler BtPluTasi.Click, AddressOf UrunYerDegistir
        AddHandler BtKapat.Click, AddressOf Cikis
        AddHandler BtPluSil.Click, AddressOf PluSil
    End Sub



    Dim Renk_Kayit As Brush = Brushes.LawnGreen
    Dim Renk_Fis As Brush = Brushes.Salmon
    Dim Renk_Cikis As Brush = Brushes.Red

    Private Function UrunButton(ByVal Renk As String,
                                ByVal En As Integer,
                                ByVal fontsize As Integer,
                                ByVal ref As Integer,
                                ByVal Aciklama As String)
        Dim Bt As New System.Windows.Controls.Button
        Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
        Bt.Name = "_" & ref
        Dim style As New Style
        style = FindResource("RoundedButton")
        'style = FindResource("MetroButton")
        ' style = FindResource("BlueGlassButtonStyle")
        Bt.Style = style
        Bt.Margin = New Thickness(1, 1, 1, 1)
        Dim color_ As Color
        If Renk <> "" Then
            color_ = DirectCast(ColorConverter.ConvertFromString(Renk), Color)
            Bt.Background = New SolidColorBrush(color_)
        Else : Bt.Background = Brushes.LightGray
        End If
        '************** olaylar ***************************
        AddHandler Bt.Click, AddressOf UrunSec
        '**************************************************
        Dim VbBt As New Viewbox
        Dim TbBt As New TextBlock
        If En <= 0 Then En = 170
        If fontsize = 0 Then fontsize = 15
        TbBt.Width = En
        TbBt.FontSize = FontSize
        TbBt.TextWrapping = TextWrapping.Wrap
        TbBt.Text = Aciklama
        VbBt.Child = TbBt
        VbBt.Margin = New Thickness(1, 1, 1, 1)
        Bt.Content = VbBt


        VbBt.Width = En
        TbBt.Width = En
        TbBt.FontSize = FontSize
        VbBt.Stretch = Stretch.Uniform
        Return Bt
    End Function


    Private Function UrunListele(ByVal GrGuid As String) As Grid
        'On Error Resume Next
        Dim Grurunler As New Grid

        Grurunler.Children.Clear()
        Grurunler.RowDefinitions.Clear()
        Grurunler.ColumnDefinitions.Clear()

        Gr.satir = 0
        Gr.sutun = 0
        For Each dr As System.Data.DataRow In Ds.Tables(Grup).Select("guid = '" & GrGuid & "'")
            Gr.satir = Chk_Null("satir", dr)
            Gr.sutun = Chk_Null("sutun", dr)
        Next
        If Gr.satir = 0 Or Gr.sutun = 0 Then
            Msg("Satır ve Sütun Değerlerini Giriniz!!", False, False, True)
            Return Nothing
        End If

        For x As Integer = 0 To Gr.satir - 1
            Dim GrRowDef As New RowDefinition
            Grurunler.RowDefinitions.Add(GrRowDef)
        Next

        For y As Integer = 0 To Gr.sutun - 1
            Dim GrColDef As New ColumnDefinition
            Grurunler.ColumnDefinitions.Add(GrColDef)
        Next
        Dim sayac As Integer = 0

        For Gx As Integer = 1 To Gr.satir
            For Gy As Integer = 1 To Gr.sutun
                Dim GrUrun As New Grid
                GrUrun.Margin = New Thickness(2)
                GrUrun.Background = Brushes.Navy

                Dim GrRdBt As New RowDefinition
                GrUrun.RowDefinitions.Add(GrRdBt)
                GrRdBt.Height = New GridLength(5, GridUnitType.Star)
                Dim Bt As New System.Windows.Controls.Button
                Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center

                Dim style As New Style
                style = FindResource("RoundedButton")
                'style = FindResource("MetroButton")
                ' style = FindResource("BlueGlassButtonStyle")
                Bt.Style = style
                Bt.Margin = New Thickness(1, 1, 1, 1)
                Dim color_ As Color
                Bt.Background = Brushes.LightGray
                Dim VbBt As New Viewbox
                Dim TbBt As New TextBlock
                TbBt.Width = 90
                TbBt.FontSize = 15
                TbBt.TextWrapping = TextWrapping.Wrap
                VbBt.Child = TbBt
                VbBt.Margin = New Thickness(1, 1, 1, 1)
                Bt.Content = VbBt
                GrUrun.Margin = New Thickness(2)
                Grid.SetRow(Bt, 0)
                GrUrun.Children.Add(Bt)
                AddHandler Bt.Click, AddressOf UrunSec

                For Each dr_urun As System.Data.DataRow In Ds.Tables(Urunler).Select("grupguid = '" & GrGuid & "' and ozellik = 1")
                    Dim Ref As Int64 = dr_urun.Item("ref")
                    Dim Uuid As String = dr_urun.Item("guid")

                    For Each dr_ozellik As System.Data.DataRow In Ds.Tables(Urunozellik).Select("sablon = '" & Kl.plutus & "' and urunguid = '" & Uuid & "' and x = '" & Gx & "' and y = '" & Gy & "'")
                        Dim Fiyat As Decimal = 0
                        Dim Stok As Decimal = 0
                        Dim UrunStokTakibi As Boolean = Chk_Null("stok_takip", dr_urun)
                        Stok = Chk_Null("stok_miktar", dr_urun)
                        Fiyat = dr_urun.Item("fiyat1")

                        '   For Each dr_fiyat As System.Data.DataRow In Ds.Tables("fiyat").Select("item = '" & ref & "' and birim_alt = '" & BirimRef & "' and type = '1'")
                        ' Fiyat = dr_fiyat.Item("fiyat")
                        '  Next
                        If Fiyat <> 0 Then
                            Dim Aciklama As String = dr_urun.Item("aciklama")
                            Dim X As Integer = dr_ozellik.Item("x")
                            Dim Y As Integer = dr_ozellik.Item("y")
                            Dim fiyat_str As String = fiyat_al(Fiyat).ToString
                            If Aciklama <> "" And X <> 0 And Y <> 0 Then
                                FontSize = Chk_Null("fontsize", dr_ozellik)
                                If FontSize = 0 Then FontSize = 15
                                Dim En As Integer = Chk_Null("en", dr_ozellik)
                                If En = 0 Then En = 90
                                Dim sigdir As String = Chk_Null("sigdir", dr_ozellik)
                                Dim Renk As String = Chk_Null("renk", dr_ozellik)
                                GrUrun.Name = "_" & Ref

                                Bt.Name = "_" & Ref
                                If Renk <> "" Then
                                    color_ = DirectCast(ColorConverter.ConvertFromString(Renk), Color)
                                    Bt.Background = New SolidColorBrush(color_)
                                End If
                                '************** olaylar ***************************

                                '**************************************************
                                TbBt.Width = En
                                TbBt.FontSize = FontSize
                                TbBt.TextWrapping = TextWrapping.Wrap
                                TbBt.Text = Aciklama
                                VbBt.Child = TbBt
                                VbBt.Margin = New Thickness(1, 1, 1, 1)
                                Bt.Content = VbBt


                                If En = 0 Or FontSize = 0 Then
                                    Dim EnUrunButon As Integer = Grurunler.RenderSize.Width
                                    EnUrunButon = EnUrunButon - ((Gr.satir - 1) * 2)
                                    EnUrunButon = EnUrunButon / Gr.satir
                                    TbBt.Width = EnUrunButon / 2
                                    VbBt.Stretch = Stretch.Uniform
                                Else
                                    VbBt.Width = En
                                    TbBt.Width = En
                                    TbBt.FontSize = FontSize
                                    If sigdir Then
                                        VbBt.Stretch = Stretch.Uniform
                                    Else
                                        VbBt.Stretch = Stretch.None
                                    End If
                                End If



                                If ay.fiyatgoster_aktif Then
                                    Dim GrRdFy As New RowDefinition
                                    GrUrun.RowDefinitions.Add(GrRdFy)
                                    GrRdFy.Height = New GridLength(1, GridUnitType.Star)
                                    Dim VbFy As New Viewbox
                                    VbFy.HorizontalAlignment = Windows.HorizontalAlignment.Right
                                    VbFy.Stretch = Stretch.Uniform
                                    Dim TbFy As New TextBlock
                                    TbFy.HorizontalAlignment = Windows.HorizontalAlignment.Left
                                    TbFy.Foreground = Brushes.WhiteSmoke
                                    TbFy.Text = "Fiyat:" & fiyat_str
                                    VbFy.Child = TbFy
                                    VbFy.Margin = New Thickness(1, 1, 1, 1)
                                    Grid.SetRow(VbFy, 1)
                                    GrUrun.Children.Add(VbFy)
                                End If
                                If UrunStokTakibi Then
                                    If ay.stokgoster_aktif Then
                                        If ay.stokgoster_aktif Then
                                            Dim bolen As Integer = 0
                                            Dim carpan As Integer = 0
                                            For Each dr_birim As System.Data.DataRow In Ds.Tables("birim_alt").Select("ref = '" & BirimRef & "'")
                                                bolen = dr_birim.Item("bolen")
                                                carpan = dr_birim.Item("carpan")
                                            Next

                                            Try
                                                Stok = (Stok * bolen) / carpan
                                            Catch ex As Exception
                                                Stok = 0
                                            End Try

                                            Dim GrRdSt As New RowDefinition
                                            GrUrun.RowDefinitions.Add(GrRdSt)
                                            GrRdSt.Height = New GridLength(1, GridUnitType.Star)
                                            Dim VbSt As New Viewbox
                                            VbSt.Name = "_" & Ref
                                            VbSt.HorizontalAlignment = Windows.HorizontalAlignment.Right
                                            VbSt.Stretch = Stretch.Uniform
                                            Dim TbSt As New TextBlock
                                            TbSt.Name = "_" & Ref
                                            TbSt.HorizontalAlignment = Windows.HorizontalAlignment.Left
                                            TbSt.Foreground = Brushes.WhiteSmoke
                                            '  TbSt.Text = "Stok:" & adet_al(Stok).ToString & TbMakinaOrta.Text

                                            VbSt.Child = TbSt
                                            VbSt.Margin = New Thickness(1, 1, 1, 1)
                                            Grid.SetRow(VbSt, 2)
                                            GrUrun.Children.Add(VbSt)
                                        End If
                                    End If
                                End If
                                sayac += 1
                            End If
                        End If
                    Next
                Next
                Grid.SetRow(GrUrun, Gx - 1)
                Grid.SetColumn(GrUrun, Gy - 1)
                Grurunler.Children.Add(GrUrun)
            Next
        Next
        Return Grurunler
    End Function

    Dim GrupTbitem As New TabItem

    Private Sub GrupListele()
        'On Error Resume Next
        Dim ilkref As Integer = 0

        Ds_read("select * from urunozellik order by ref", Urunozellik, Conn, True, True)
        Ds_read("select * from urunler order by ref", Urunler, Conn, True, True)

        Dim CountGrup As Integer = 0
        CountGrup = Ds.Tables(Grup).Rows.Count
        If CountGrup = 0 Then Exit Sub
        Dim EnGrupButon As Integer = TcGrUrunler.RenderSize.Width
        EnGrupButon = EnGrupButon - ((CountGrup - 3) * 2)
        EnGrupButon = EnGrupButon / CountGrup
        RemoveHandler TcGrUrunler.SelectionChanged, AddressOf GrupSecim
        TcGrUrunler.Items.Clear()
        If Gn.Grup_Sayisi = 0 Then Gn.Grup_Sayisi = 10


        For i As Integer = 1 To Gn.Grup_Sayisi
            Dim Tb As New TextBlock
            Tb.Width = EnGrupButon
            With Tb
                .HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                .VerticalAlignment = Windows.VerticalAlignment.Stretch
                .TextWrapping = TextWrapping.Wrap
            End With
            Dim Vb As New Viewbox
            With Vb
                .HorizontalAlignment = Windows.HorizontalAlignment.Stretch
                .VerticalAlignment = Windows.VerticalAlignment.Stretch
                .Stretch = Stretch.Uniform
                .Child = Tb
                .Margin = New Thickness(3, 3, 3, 3)
            End With


            Dim HeigtTc As Integer = 0
            HeigtTc = TcGrUrunler.RenderSize.Height
            HeigtTc = HeigtTc / CountGrup

            Dim Tbitem As New TabItem
            With Tbitem
                .Header = Vb
                .Height = HeigtTc
                .Width = 120
            End With

            Dim GrTab As New Grid
            With GrTab
                .VerticalAlignment = VerticalAlignment.Stretch
                .HorizontalAlignment = HorizontalAlignment.Stretch
            End With

            For Each drGrup As System.Data.DataRow In Ds.Tables("grup").Select("sira = '" & i & "'")
                Dim Aciklama As String = drGrup.Item("aciklama")
                Tb.Text = Aciklama
                Grupref = drGrup.Item("ref")
                Grupguid = drGrup.Item("guid")
                GrupAciklama = drGrup.Item("aciklama")
                Tbitem.Name = "_" & Grupref
                If ilkref = 0 Then
                    ilkref = Grupref
                    BtGrup.Background = Brushes.Lime
                    GrupTbitem = Tbitem
                End If
                Dim Satir As Int64 = drGrup.Item("satir")
                Gr.satir = Satir
                Dim Sutun As Int64 = drGrup.Item("sutun")
                Gr.sutun = Sutun
                GrTab.Children.Add(UrunListele(Grupguid))
            Next
            Tbitem.Content = GrTab
            TcGrUrunler.Items.Add(Tbitem)
        Next
        AddHandler TcGrUrunler.SelectionChanged, AddressOf GrupSecim
        Grupref = ilkref
    End Sub

    Dim Grupref As Integer = 0
    Dim Grupguid As String = ""
    Dim GrupAciklama As String = ""
    Dim BtGrup As New System.Windows.Controls.Button


    Dim TcIndex As Integer = 0
    Private Sub GrupSecim(ByVal sender As Object, ByVal e As System.EventArgs)
        BtGrup.Background = Brushes.LightGray
        Dim Tc As New TabControl
        Tc = sender
        Dim Tpitem As New System.Windows.Controls.TabItem
        Tpitem = Tc.SelectedItem

        Try
            Grupref = Convert.ToInt16(Mid(Tpitem.Name, 2, Tpitem.Name.Length - 1))
        Catch ex As Exception
        End Try
        For Each drGrup As System.Data.DataRow In Ds.Tables("grup").Select("ref = '" & Grupref & "'")
            Grupguid = drGrup.Item("guid")
            GrupAciklama = drGrup.Item("aciklama")
            GrupTbitem = Tpitem
            TcIndex = Tc.SelectedIndex
        Next
    End Sub

    Private Sub Cikis()
        Ds_read("select * from urunozellik order by ref", Urunozellik, Conn, True, True)
        Ds_read("select * from urunler order by ref", Urunler, Conn, True, True)
        Me.Close()
    End Sub

    Dim BtnUrun As New System.Windows.Controls.Button
    Dim RenkUrun As Brush

    Dim BirimRef As Integer = 0
    Dim Birim_Ust As Integer = 0

    Private Structure HsSonIslem_Degisken
        Dim Urun As Integer
        Dim BirimAlt As Integer
        Dim Miktar As Decimal
    End Structure
    Dim HsSonIslem As HsSonIslem_Degisken

    Private Sub Sesuyari()
        Console.Beep(5000, 100)
    End Sub




    Dim Tasima As Boolean = False


    Private Sub UrunYerDegistir(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Tasima Then
            Tasima = False
            BtGridUrunDurum.Background = Brushes.Blue
            BtGridUrunDurum.Content = ""
        Else
            If IsNothing(BtnUrun.Name) = False Then
                Tasima = True
                BtnUrun.Background = Brushes.Yellow
                BtGridUrunDurum.Background = Brushes.Yellow
                BtGridUrunDurum.Content = Ur.aciklama

            End If
        End If
    End Sub


    Private Sub UrunSec(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Btn As Button = sender
        Dim Ref As Integer
        Try
            Ref = Convert.ToInt16(Mid(Btn.Name, 2, Btn.Name.Length - 1))
        Catch ex As Exception
        End Try

        Dim Grd As Grid = CType(Btn.Parent, Grid)


        Dim X As Integer = Grid.GetRow(Grd)
        Dim Y As Integer = Grid.GetColumn(Grd)



        If Ref = 0 And Tasima = False Then

            ' Urun Yerleştirilecek
            Ur = Nothing
            Dim W As New WpfListeUrunler

            W.Main(Grupref, Ds)


            If Ur.ref <> 0 Then

                TbUrunDurum.Text = Ur.aciklama
                Btn.Background = Brushes.Yellow
                For Each dr_urun As DataRow In Ds.Tables(Urunler).Select("ref = '" & Ur.ref & "' and ozellik <> 1")
                    Dim UrunGuid As String = Chk_Null("guid", dr_urun)
                    Dim UrunStokTakibi As Boolean = Chk_Null("stok_takip", dr_urun)
                    Dim Stok As Decimal = Chk_Null("stok_miktar", dr_urun)
                    Dim Fiyat As Decimal = dr_urun.Item("fiyat1")
                    Dim fiyat_str As String = fiyat_al(Fiyat).ToString
                    For Each dr_ozellik As DataRow In Ds.Tables(Urunozellik).Select("urunguid = '" & Ur.guid & "'and sablon = '" & Kl.plutus & "'")
                        Msg("Ürün Yeri Tanımlı!!", False, False, True)
                        Exit Sub
                    Next
                    Uo.ref = 0
                    Uo.createdate = Now
                    Uo.guid = Guid.NewGuid.ToString
                    Uo.urunguid = UrunGuid
                    Uo.sablon = Kl.plutus
                    Uo.grupguid = Grupguid
                    Uo.fontsize = 15
                    Uo.renk = Brushes.LightGray.ToString
                    Uo.sigdir = True
                    Uo.en = 0
                    Uo.x = X + 1
                    Uo.y = Y + 1

                    Dim Sql As String = ""
                    Sql = "update urunler set ozellik = 1, grupguid = '" & Grupguid & "',grup_aciklama = '" & GrupAciklama & "' where guid = '" & Ur.guid & "'"

                    If Execute_run(Sql, ConnRemote, True) Then
                        Execute_run(Sql, Conn, True)
                        Kayitsql(Urunozellik, Uo, ConnRemote, True)
                        Kayitsql(Urunozellik, Uo, Conn, True)
                    End If
                    If Ds_read("select * from urunler order by ref", Urunler, Conn, True, True) Then
                    End If
                    If Ds_read("select * from urunozellik order by ref", Urunozellik, Conn, True, True) Then
                    End If
                    Grd.Children.Clear()
                    Dim Bt As Button = UrunButton("", 0, 15, Ur.ref, Ur.aciklama)
                    Grid.SetRow(Bt, 0)
                    Grd.Children.Add(Bt)
                    If ay.fiyatgoster_aktif Then
                        Dim GrRdFy As New RowDefinition
                        Grd.RowDefinitions.Add(GrRdFy)
                        GrRdFy.Height = New GridLength(1, GridUnitType.Star)
                        Dim VbFy As New Viewbox
                        VbFy.HorizontalAlignment = Windows.HorizontalAlignment.Right
                        VbFy.Stretch = Stretch.Uniform
                        Dim TbFy As New TextBlock
                        TbFy.HorizontalAlignment = Windows.HorizontalAlignment.Left
                        TbFy.Foreground = Brushes.WhiteSmoke
                        If fiyat_str <> "" Then
                            TbFy.Text = "Fiyat:" & fiyat_str
                        End If
                        VbFy.Child = TbFy
                        VbFy.Margin = New Thickness(1, 1, 1, 1)
                        Grid.SetRow(VbFy, 1)
                        Grd.Children.Add(VbFy)
                    End If
                    If UrunStokTakibi Then
                        ay.stokgoster_aktif = False
                        If ay.stokgoster_aktif Then
                            If ay.stokgoster_aktif Then
                                Dim bolen As Integer = 0
                                Dim carpan As Integer = 0
                                For Each dr_birim As System.Data.DataRow In Ds.Tables("birim_alt").Select("ref = '" & BirimRef & "'")
                                    bolen = dr_birim.Item("bolen")
                                    carpan = dr_birim.Item("carpan")
                                Next
                                Stok = (Stok * bolen) / carpan
                                Dim GrRdSt As New RowDefinition
                                Grd.RowDefinitions.Add(GrRdSt)
                                GrRdSt.Height = New GridLength(1, GridUnitType.Star)
                                Dim VbSt As New Viewbox
                                VbSt.Name = "_" & Ref
                                VbSt.HorizontalAlignment = Windows.HorizontalAlignment.Right
                                VbSt.Stretch = Stretch.Uniform
                                Dim TbSt As New TextBlock
                                TbSt.Name = "_" & Ref
                                TbSt.HorizontalAlignment = Windows.HorizontalAlignment.Left
                                TbSt.Foreground = Brushes.WhiteSmoke
                                VbSt.Child = TbSt
                                VbSt.Margin = New Thickness(1, 1, 1, 1)
                                Grid.SetRow(VbSt, 2)
                                Grd.Children.Add(VbSt)
                            End If
                        End If
                    End If
                Next
            End If
        End If

        Try
            BtGridUrunDurum.Background = Brushes.Blue
            BtnUrun.Background = RenkUrun
            BtnUrun = sender
            RenkUrun = BtnUrun.Background
            BtnUrun.Background = Brushes.Lime
            '   Ur = Nothing
            Ur.ref = Convert.ToInt16(Mid(Btn.Name, 2, Btn.Name.Length - 1))
            DegiskenAl(Urunler, Ur.ref, True)
            'TbEn.Text = Ur.en
            'TbFontSize.Text = Ur.fontsize
            'CbSigdir.IsChecked = Ur.sigdir
            TbUrunDurum.Text = Ur.aciklama
        Catch ex As Exception
        End Try
        If Tasima Then
            If Ref = 0 Then
                Dim SqlOzellik As String
                SqlOzellik = "update urunozellik set x = '" & X + 1 & "',y = '" & Y + 1 & "',grupguid = '" & Grupguid & "' where urunguid = '" & Ur.guid & "'"
                If Execute_Run(SqlOzellik, Conn) Then
                    Dim SqlUrun As String
                    SqlUrun = "update urunler set grup = '" & Grupref & "',grupguid = '" & Grupguid & "' where guid = '" & Ur.guid & "'"
                    Execute_Run(SqlUrun, Conn)
                    '******************************************************************************************************************
                    GrupListele()
                    TcGrUrunler.SelectedIndex = TcIndex
                    Dim TcItem As TabItem = TcGrUrunler.Items.Item(TcIndex)
                    TcGrUrunler.SelectedItem = TcItem
                    TcItem.IsSelected = True
                    TcGrUrunler.SelectedValue = TcItem
                End If
            Else
                Msg("Boş Alan Seçiniz!!", False, False, True)
            End If
            Tasima = False
            BtGridUrunDurum.Background = Brushes.Blue
            BtnUrun.Background = Brushes.LightGray
            BtGridUrunDurum.Content = ""
        Else
        End If

    End Sub

    Private Sub PluSil()
        Ur = Nothing
        Uo = Nothing
        '****************************************
        Dim Btn As New Button
        Btn = BtnUrun
        Dim GrdBtnUrun As New Grid
        Dim Grd As New Grid
        Try

            GrdBtnUrun = CType(Btn.Parent, Grid)
            Grd = CType(GrdBtnUrun.Parent, Grid)
            'VbFy.Child = TbFy
            'VbFy.Margin = New Thickness(1, 1, 1, 1)
            'Grid.SetRow(VbFy, 1)
            'GrUrun.Children.Add(VbFy)
            Ur.ref = Convert.ToInt16(Mid(Btn.Name, 2, Btn.Name.Length - 1))
        Catch ex As Exception
        End Try
        ' Urun Silinecek
        If Ur.ref <> 0 Then
            TbUrunDurum.Text = Ur.aciklama
            Btn.Background = Brushes.Red
            For Each dr_urun As DataRow In Ds.Tables(Urunler).Select("ref = '" & Ur.ref & "'")
                Ur.guid = Chk_Null("guid", dr_urun)
                Dim Tanimli As Boolean = False
                For Each dr_ozellik As DataRow In Ds.Tables(Urunozellik).Select("urunguid = '" & Ur.guid & "'")
                    Uo.ref = Ur.ref
                    Uo.urunguid = Ur.guid
                    Uo.guid = Chk_Null("guid", dr_ozellik)
                    Dim SqlUrun As String = "update urunler set ozellik = 0 where guid = '" & Uo.urunguid & "'"
                    If Execute_run(SqlUrun, Conn, True) Then
                        Execute_run(SqlUrun, ConnRemote, True)
                        Dim SqlOzellik As String = "delete from urunozellik where guid = '" & Uo.guid & "'"
                        If Execute_run(SqlOzellik, Conn, True) Then
                            Execute_run(SqlOzellik, ConnRemote, True)
                        End If
                    End If
                    Exit For
                Next
                Dim VbFy As Viewbox = CType(GrdBtnUrun.Children(1), Viewbox)
                Dim TbFy As TextBlock = CType(VbFy.Child, TextBlock)
                Btn.Content = Nothing
                Btn.Name = ""
                TbFy.Text = ""
                Ur = Nothing
                Uo = Nothing
            Next
        End If
    End Sub

    Private Sub Main()
        EkranDizayn(Me)
        hb = Nothing
        hs = Nothing
        kh = Nothing
        th = Nothing
        Ur = Nothing
        Uo = Nothing
        Br = Nothing
        Ba = Nothing
        Bu = Nothing
        '**************************************************
        ConnRemote = ConnectionRemote()
        '**************************************************
        'System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        If ay.ref = 1 Then
            ProgramAyarlari()
            ay.indirim_aktif = True

        Else
            Msg("Program Ayarlarını Tanımlayınız !", False, False, True)
            Me.Close()
            Exit Sub
        End If

        DataSetOku(True, True, True, True, True, True, True, True, True, True, True, True, True, True)

        GrupListele()


        If ay.stokgoster_aktif = False Then
            BrGridStokDurum.Visibility = Windows.Visibility.Hidden
            Grid.SetColumnSpan(BrGridGrupDurum, 2)
        Else
            BrGridStokDurum.Visibility = Windows.Visibility.Visible
            Grid.SetColumnSpan(BrGridGrupDurum, 1)
        End If
        Olaylar()
    End Sub

End Class

