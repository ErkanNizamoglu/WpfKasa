

Imports System.Data
Imports System.Windows.Threading

Public Class WpfDegisimFisi
    '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
#Disable Warning IDE0059
#Disable Warning IDE0058
#Disable Warning IDE0081
    '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    Private Delegate Sub NextPrimeDelegate()


    '────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    Public Mesaj As String = ""
    '────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    Private Sub Olay_Ekle()
        AddHandler BtYaz.Click, AddressOf Yaz
        AddHandler BtCikis.Click, AddressOf Cikis
        AddHandler DgDegisim.SelectionChanged, AddressOf DegisimDetayListele
    End Sub



    Private Sub WaitActive()
        Me.UpdateLayout()
        UcWait.Visibility = Visibility.Visible
    End Sub
    Private Sub WaitDeActive()
        Me.UpdateLayout()
        UcWait.Visibility = Visibility.Hidden
    End Sub


    Private Sub DegisimDetayListele()
        Dim guid As String
        Try
            guid = DgDegisim.SelectedItem("guid")
        Catch ex As Exception
            Exit Sub
        End Try
        DetayListele(guid)
    End Sub

    Private Sub Cikis()
        hb = Nothing
        hs = Nothing
        Me.Close()
    End Sub

    Private Sub Sil()

        Dim ref As Integer = 0
        Try
            ref = DgDegisim.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            If Msg("Silme İşlemini Onaylıyormusunuz?", True, True, False) Then

                Execute_run("update hareket_baslik set iptal = 1 where ref = '" & ref & "'", Conn, True)
                DgDegisim.SelectedIndex = 0
                DegisimListele()
            End If
        Else
            Msg("Seçim Yapınız!!", False, False, True)
        End If
    End Sub

    Private Sub Yaz()
        Dim ref As Integer = 0
        Try
            ref = DgDegisim.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            hb.ref = ref
            '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
            DegiskenAl(hareket_baslik, ref, True)
            Ds_read("select ref,guid,stokkodu,barkod,urun_aciklama,miktar,birim_alt_isaret,iptal from hareket_satir where baslik = '" & ref & "' and iptal <> 1 order by satir", hareket_satir, Conn, True, True)
            Ds_read("select odeme_kod,iptal from tahsilat where baslik = '" & ref & "' and iptal <> 1", tahsilat, Conn, True, True)
            Dim C_prn As New Class_Print80mm
            C_prn.DegisimPrint80mm(reg.Yazici)
            '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
            hb.dokum = True
            'hb.dokum_sayisi += 1
            'Execute_run("update hareket_baslik set dokum_sayisi = '" & hb.dokum_sayisi & "', dokum = 1 where ref = '" & hb.ref & "'", Conn, True)
        Else
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End If
        Cikis()
    End Sub



    Private Sub DegisimListele()
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Me.Dispatcher.Invoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf WaitActive))
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        Dim Tbl As String = "degisim"
        Dim BasTarihStr As String = TarihAl(Now.ToString) & " 00:00:00"
        Dim BitTarihStr As String = TarihAl(Now.ToString) & " 23:59:00"
        Dim Sql As String
        Sql = "select "
        Sql &= "ref,"
        Sql &= "guid,"
        Sql &= "kullanici_kod,"
        Sql &= "kasa_kod,"
        Sql &= "sayac,"
        Sql &= "tarih "
        Sql &= "from hareket_baslik "
        Sql &= "where gunsonu = 0 "
        Sql &= "and aski = 0 "
        Sql &= "and fisturu = 1 "
        Sql &= "and iptal <> 1 "
        Sql &= "and CAST(tarih As datetime) BETWEEN '" & BasTarihStr & "' AND '" & BitTarihStr & "'"
        Sql &= "order by ref desc"
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Ds_read(Sql, Tbl, Conn, True, True)
        Dim Dt As DataTable = Ds.Tables(Tbl)
        If Dt.Rows.Count <= 0 Then
            Dt = ServisOkuTable(Tbl, Sql) ' Merkezden Okunuyor * Diğer kasadanda Fiş verilebilsin diye"
        End If
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Dim DegisimIlk As String
        For Each dr As DataRow In Dt.Rows
            DegisimIlk = Chk_Null("guid", dr)
            Exit For
        Next
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Try
            DvDegisim.Table = Dt
            DgDegisim.ItemsSource = DvDegisim
            DvDegisim.Sort = "sayac DESC"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        DgDegisim.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        DgDegisim.Columns(1).Visibility = Windows.Visibility.Hidden 'guid
        DgDegisim.Columns(2).Visibility = Windows.Visibility.Hidden 'ref
        DgDegisim.Columns(3).Visibility = Windows.Visibility.Hidden 'guid

        GridColNumeric(DgDegisim, 4, 0, "SAYAÇ", TextAlignment.Left, 150)
        GridColTarih(DgDegisim, 5, 1, "TARİH/SAAT", TextAlignment.Left, 350)
        DgDegisim.UpdateLayout()
        DetayListele(DegisimIlk)

    End Sub


    Private Sub DetayListele(ByVal guid As String)
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Me.Dispatcher.Invoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf WaitActive))
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        If guid = "" Then Exit Sub
        Dim Tbl As String = "DegisimDetay"
        Dim Sql As String
        Sql = "select "
        Sql &= "ref,"
        Sql &= "urunguid,"
        Sql &= "urun_aciklama,"
        Sql &= "miktar,"
        Sql &= "birim_alt_isaret,"
        Sql &= "iptal "
        Sql &= "from "
        Sql &= "hareket_satir "
        Sql &= "where "
        Sql &= "baslikguid = '" & guid & "' and iptal <> 1"
        '────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        Ds_read(Sql, Tbl, Conn, True, True)
        Dim Dt As DataTable = Ds.Tables(Tbl)
        If Dt.Rows.Count = 0 Then
            Dt = ServisOkuTable(Tbl, Sql) ' Merkezden Okunuyor * Diğer kasadanda Fiş verilebilsin diye"
        End If
        '────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Try
            'DvDegisimDetay.Table = Dt
            'DgDegisimDetay.ItemsSource = DvDegisimDetay
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'DgDegisimDetay.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        'DgDegisimDetay.Columns(1).Visibility = Windows.Visibility.Hidden 'ref
        'GridColString(DgDegisimDetay, 2, 2, "ÜRÜN AÇIKLAMA", TextAlignment.Left, 250)
        'GridColNumeric(DgDegisimDetay, 3, 3, "MİKTAR", TextAlignment.Left, 100)
        'GridColString(DgDegisimDetay, 4, 4, "BİRİM", TextAlignment.Left, 100)
        'DgDegisimDetay.Columns(4).Visibility = Windows.Visibility.Hidden 'iptal
        'DgDegisimDetay.UpdateLayout()


        'On Error Resume Next
        GrUrunler.Children.Clear()
        GrUrunler.RowDefinitions.Clear()
        GrUrunler.ColumnDefinitions.Clear()

        Gr.satir = 0
        Gr.sutun = 0

        If Dt.Rows.Count <= 0 Then Exit Sub

        For x As Integer = 0 To Dt.Rows.Count - 1

            Dim urunguid As String = Dt.Rows(x).Item("urunguid")
            Dim aciklama As String = Dt.Rows(x).Item("urun_aciklama")

            Dim GrRowDef As New RowDefinition
            GrRowDef.Height = New GridLength(1, GridUnitType.Star)
            GrUrunler.RowDefinitions.Add(GrRowDef)

            Dim Bt As New System.Windows.Controls.Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt.Uid = urunguid
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Bt.Margin = New Thickness(1, 1, 1, 1)
            Bt.Background = Brushes.Turquoise
            Bt.Height = 65
            Bt.Width = 200

            '************** olaylar ***************************
            ' AddHandler Bt.Click, AddressOf UrunEkle
            '**************************************************
            Dim VbBt As New Viewbox
            Dim TbBt As New TextBlock
            TbBt.FontSize = 20
            TbBt.TextWrapping = TextWrapping.Wrap
            TbBt.Text = aciklama
            VbBt.Child = TbBt
            VbBt.Margin = New Thickness(1, 1, 1, 1)
            Bt.Content = VbBt
            Grid.SetRow(Bt, x)
            GrUrunler.Children.Add(Bt)
        Next
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Me.Dispatcher.Invoke(DispatcherPriority.Normal, New NextPrimeDelegate(AddressOf WaitDeActive))
        '──────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    End Sub

    Dim DtDegisim As New DataTable
    Dim DvDegisim As New DataView

    Dim DtDegisimDetay As New DataTable
    Dim DvDegisimDetay As New DataView


    Private Sub Main()
        UcWait.Visibility = Visibility.Hidden
        hb = Nothing
        hs = Nothing
        EkranDizayn_ThreeDBorderWindow(Me)
        Olay_Ekle()
        DegisimListele()
    End Sub

End Class


