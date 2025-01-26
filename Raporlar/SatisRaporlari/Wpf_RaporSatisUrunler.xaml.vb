
Public Class Wpf_RaporSatisUrunler

    Dim Cr As Cursors


    Private Sub OlayEkle()
        AddHandler BtBasArti.Click, AddressOf BasTarihArttir
        AddHandler BtBasEksi.Click, AddressOf BasTarihEksilt
        AddHandler BtBitisArti.Click, AddressOf BitTarihArttir
        AddHandler BtBitEksi.Click, AddressOf BitTarihEksilt
    End Sub


    Dim DvR As New System.Data.DataView
    Dim DtR As New System.Data.DataTable

    Private Sub BasTarihArttir()
        DpBasTarih.DisplayDate = DpBasTarih.DisplayDate.AddDays(1)
        DpBasTarih.Text = DpBasTarih.DisplayDate
    End Sub

    Private Sub BasTarihEksilt()
        DpBasTarih.DisplayDate = DpBasTarih.DisplayDate.AddDays(-1)
        DpBasTarih.Text = DpBasTarih.DisplayDate
    End Sub

    Private Sub BitTarihArttir()
        DpBitTarih.DisplayDate = DpBitTarih.DisplayDate.AddDays(1)
        DpBitTarih.Text = DpBitTarih.DisplayDate
    End Sub

    Private Sub BitTarihEksilt()
        DpBitTarih.DisplayDate = DpBitTarih.DisplayDate.AddDays(-1)
        DpBitTarih.Text = DpBitTarih.DisplayDate
    End Sub


    Private Sub SatisListele()
        DtR = Ds.Tables("rapor")
        DvR.Table = DtR
        DvR.RowFilter = "miktar > 0"
        DgRapor.ItemsSource = DvR
        DgRapor.UpdateLayout()
        Dim Toplam As Decimal = 0
        Try
            Toplam = fiyat_al(DvR.Table.Compute("Sum(tutar)", "tutar > 0"))
        Catch ex As Exception
        End Try
        TbToplam.Text = "Toplam:" & Toplam
    End Sub

    Private Function RaporTarihAl(ByVal tarih As Date) As String
        Dim Yil As String = Year(tarih).ToString
        Dim Ay As String = Format(Month(tarih), "00")
        Dim Gun As String = Format(Day(tarih), "00")
        Dim TarihStr As String = Yil & "-" & Ay & "-" & Gun
        Return TarihStr
    End Function

    Private Sub Listele()

        Dim BasTarihStr As String = ""
        If IsDate(DpBasTarih.Text) Then
            BasTarihStr = RaporTarihAl(DpBasTarih.Text) & " 00:00:00"

            '11/3/2012 00:00:00' and '11/5/2012 23:59:00' 
        Else
            Msg("Başlangıç Tarihini Seçiniz! ", False, False, True)
            Exit Sub
        End If
        Dim BitTarihStr As String = ""
        If IsDate(DpBitTarih.Text) Then
            BitTarihStr = RaporTarihAl(DpBitTarih.Text) & " 23:59:00"
        Else
            Msg("Bitiş Tarihini Seçiniz! ", False, False, True)
            Exit Sub
        End If

        Dim Sql As String = ""

        Sql &= " SELECT     "
        Sql &= "   hareket_satir.urun_aciklama AS aciklama"
        Sql &= " , hareket_satir.birim_alt_isaret AS birim"
        Sql &= " , SUM(hareket_satir.miktar) AS miktar"
        Sql &= " , hareket_satir.fiyat "
        Sql &= " , SUM(hareket_satir.net_tutar) AS tutar"
        Sql &= " FROM         hareket_satir "
        Sql &= "  INNER JOIN hareket_baslik ON hareket_baslik.guid = hareket_satir.baslikguid"
        Sql &= " WHERE     "
        Sql &= "  (CAST(hareket_baslik.tarih AS datetime) BETWEEN '" & BasTarihStr & "' AND '" & BitTarihStr & "') "
        Sql &= "  AND (hareket_baslik.iptal <> 1) "
        Sql &= "  AND (hareket_baslik.aski <> 1)"
        Sql &= "  AND (hareket_baslik.fisturu <> 9)"
        Sql &= "  AND (hareket_satir.iptal <> 1)"
        Sql &= " GROUP BY "
        Sql &= "   hareket_satir.urun_aciklama"
        Sql &= " , hareket_satir.birim_alt_isaret"
        Sql &= " , hareket_satir.fiyat"
        Sql &= " ORDER BY "
        Sql &= " aciklama"
        Sql &= " , birim"

        ds_read(Sql, "rapor", Conn, True, True)
        SatisListele()

    End Sub


    Private Sub Aktar()

    End Sub

    Private Sub Yazdir()
        On Error Resume Next
        Dim Tbl As System.Data.DataTable

        If Ds.Tables("rapor").Rows.Count > 0 Then
            Tbl = Ds.Tables("rapor")
            Dim BasTarih As String = DpBasTarih.Text
            Dim BitTarih As String = DpBitTarih.Text

            Select Case reg.fisboyutu
                Case "80mm"
                    Dim C As New Class_Print
                    C.UrunSatisRaporuPrint(reg.Yazici, BasTarih, BitTarih)
                Case "58mm"
                    Dim C As New Class_Print58mm
                    C.UrunSatisRaporuPrint58mm(reg.Yazici, BasTarih, BitTarih)
            End Select


        End If
    End Sub

    Private Sub Raporla()
        Listele()
    End Sub

    Private Sub Cikis()
        Try
            Ds.Tables.Remove("rapor")
        Catch ex As Exception

        End Try
        Me.Close()
    End Sub

    Function MenuYap(rapor_ As Boolean, _
                     aktar_ As Boolean,
                     yazdir_ As Boolean,
                     cikis_ As Boolean) As Grid

        Dim Grd As New Grid
        Dim sayi As Integer
        If rapor_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("rapor", "RAPOR", 0, sayi, Brushes.Red, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf Raporla
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If

        If aktar_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("aktar", "AKTAR", 0, sayi, Brushes.Lime, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf Aktar
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If

        If yazdir_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("yazdir", "YAZDIR", 0, sayi, Brushes.Bisque, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf Yazdir
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If

        For i As Integer = sayi To 5
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
        Next

        If cikis_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("cikis", "ÇIKIŞ", 0, sayi, Brushes.Bisque, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf Cikis
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If

        Grid.SetRow(Grd, 3)
        Return Grd
    End Function

    Private Sub Wpf_Liste_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        TbBaslik.Text = "ÜRÜN SATIŞ LİSTESİ"
        EkranDizayn_ThreeDBorderWindow(Me)
        DpBasTarih.Text = DpBasTarih.DisplayDate
        DpBitTarih.Text = DpBitTarih.DisplayDate
        OlayEkle()
        GrMenu.Children.Add(MenuYap(True, True, True, True))
    End Sub
End Class
