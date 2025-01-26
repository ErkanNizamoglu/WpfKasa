Imports System.Data.SqlClient
Imports System.IO
Imports System.Windows.Controls.Primitives

Partial Public Class WpfSatis  ' Terazi İşlemleri
    Private Sub TeraziAktar(ByVal Satir As Integer, ByVal Sutun As Integer)
        '        On Error Resume Next
        UgTerazi.Rows = Satir
        UgTerazi.Columns = Sutun

        '  UgTerazi.Children.Clear()
        Ds_read("select * from terazi_baslik where okundu = 0 and yazildi = 0 and iptal = 0", terazi_baslik, True, True)
        Ds_read("select * from terazi_satir  where okundu = 0 and yazildi = 0 and iptal = 0", terazi_satir, True, True)

        ' For Each drBaslik As System.Data.DataRow In Ds.Tables(terazi_baslik).Select("okundu = 0 and yazildi = 0 and iptal = 0")
        For Each drBaslik As System.Data.DataRow In Ds.Tables(terazi_baslik).Rows
            Dim FisNo As String = drBaslik.Item("fisno")
            Dim Satici As String = drBaslik.Item("satici")
            Dim Terazi As Integer = drBaslik.Item("terazi")
            Dim BasTarih As Date = drBaslik.Item("bas_tarih")
            Dim BitTarih As Date = drBaslik.Item("bit_tarih")
            Dim ToplamTutar As Decimal = drBaslik.Item("tutar")

            Dim BtTerazi As New Button
            Dim GrTerazi As New Grid

            '************** olaylar ***************************
            GrTerazi.Uid = FisNo
            AddHandler GrTerazi.MouseDown, AddressOf TeraziGridSecim
            '**************************************************

            GrTerazi.Margin = New Thickness(2)

            Dim GrRdTeraziBaslik As New RowDefinition
            GrRdTeraziBaslik.Height = New GridLength(20, GridUnitType.Star)
            GrTerazi.RowDefinitions.Add(GrRdTeraziBaslik)


            Dim GrRdTeraziSatir As New RowDefinition
            GrRdTeraziSatir.Height = New GridLength(60, GridUnitType.Star)
            GrTerazi.RowDefinitions.Add(GrRdTeraziSatir)


            Dim GrRdTeraziToplam As New RowDefinition
            GrRdTeraziToplam.Height = New GridLength(20, GridUnitType.Star)
            GrTerazi.RowDefinitions.Add(GrRdTeraziToplam)


            'Terazi Gridleri
            Dim GrBaslik As New Grid
            GrBaslik.Background = Brushes.LawnGreen
            Grid.SetRow(GrBaslik, 0)

            Dim GrToplam As New Grid
            GrToplam.Background = Brushes.Blue
            Grid.SetRow(GrToplam, 2)

            Dim GrSatir As New Grid
            GrSatir.Background = Brushes.Red
            Grid.SetRow(GrSatir, 1)




            Dim GrRdFisno As New RowDefinition
            GrBaslik.RowDefinitions.Add(GrRdFisno)
            'Fisno vb buraya yaz
            Dim VbFisNo As New Viewbox
            Dim TbFisNo As New TextBlock
            TbFisNo.Text = "Fiş No  : " & FisNo
            VbFisNo.Child = TbFisNo
            Grid.SetRow(VbFisNo, 0)
            GrBaslik.Children.Add(VbFisNo)

            Dim GrRdSatici As New RowDefinition
            GrBaslik.RowDefinitions.Add(GrRdSatici)
            'satici vb buraya yaz
            Dim VbSatici As New Viewbox
            Dim TbSatici As New TextBlock
            TbSatici.Text = "Satici  : " & Satici
            VbSatici.Child = TbSatici
            Grid.SetRow(VbSatici, 1)
            GrBaslik.Children.Add(VbSatici)


            GrTerazi.Children.Add(GrBaslik)




            'toplam vb buraya yaz
            Dim VbToplam As New Viewbox
            Dim TbToplam As New TextBlock
            TbToplam.Text = "Toplam  : " & fiyat_al(ToplamTutar)
            VbToplam.Child = TbToplam
            GrToplam.Children.Add(VbToplam)


            GrTerazi.Children.Add(GrToplam)


            Dim GrCdAciklama As New ColumnDefinition
            GrCdAciklama.Width = New GridLength(40, GridUnitType.Star)
            GrSatir.ColumnDefinitions.Add(GrCdAciklama)

            Dim GrCdGramaj As New ColumnDefinition
            GrCdGramaj.Width = New GridLength(20, GridUnitType.Star)
            GrSatir.ColumnDefinitions.Add(GrCdGramaj)

            'Dim GrCdFiyat As New ColumnDefinition
            'GrCdFiyat.Width = New GridLength(20, GridUnitType.Star)
            'GrSatir.ColumnDefinitions.Add(GrCdFiyat)

            Dim GrCdTutar As New ColumnDefinition
            GrCdTutar.Width = New GridLength(20, GridUnitType.Star)
            GrSatir.ColumnDefinitions.Add(GrCdTutar)



            Dim sayac As Integer = 0
            For Each drSatir As System.Data.DataRow In Ds.Tables(terazi_satir).Select("fisno = '" & FisNo & "'")
                Dim Plu As Int64 = drSatir.Item("plu")
                Dim Fiyat As Decimal = drSatir.Item("fiyat")
                Dim fiyat_str As String = fiyat_al(Fiyat).ToString
                Dim agirlik As Decimal = drSatir.Item("agirlik")
                agirlik = agirlik / 1000
                Dim tutar As Decimal = drSatir.Item("tutar")
                Dim ref As Long = 0
                Dim Aciklama As String = ""
                For Each drBarkod As System.Data.DataRow In Ds.Tables(Barkodlar).Select("plu = '" & Plu & "'")
                    Ref = drBarkod.Item("urun")
                    For Each drUrun As System.Data.DataRow In Ds.Tables(Urunler).Select("ref = '" & Ref & "'")
                        Aciklama = drUrun.Item("aciklama")
                    Next
                Next


                Dim GrRdSatir As New RowDefinition
                GrRdSatir.Height = New GridLength(1, GridUnitType.Star)
                GrSatir.RowDefinitions.Add(GrRdSatir)




                Dim VbAciklama As New Viewbox
                VbAciklama.Margin = New Thickness(1)
                VbAciklama.Stretch = Stretch.None
                Dim TbAciklama As New TextBlock
                TbAciklama.Text = Aciklama
                TbAciklama.FontSize = 10
                VbAciklama.Child = TbAciklama
                Grid.SetRow(VbAciklama, sayac)
                Grid.SetColumn(VbAciklama, 0)
                GrSatir.Children.Add(VbAciklama)


                Dim VbGramaj As New Viewbox
                VbGramaj.Margin = New Thickness(1)
                VbGramaj.Stretch = Stretch.None
                Dim TbGramaj As New TextBlock
                TbGramaj.Text = gramaj_al(agirlik) & "Gr"
                TbGramaj.FontSize = 10
                VbGramaj.Child = TbGramaj
                Grid.SetRow(VbGramaj, sayac)
                Grid.SetColumn(VbGramaj, 1)
                GrSatir.Children.Add(VbGramaj)

                Dim VbTutar As New Viewbox
                VbTutar.Margin = New Thickness(1)
                VbTutar.Stretch = Stretch.None
                Dim TbTutar As New TextBlock
                TbTutar.Text = fiyat_al(tutar) & "TL"
                TbTutar.FontSize = 10
                VbTutar.Child = TbTutar
                Grid.SetRow(VbTutar, sayac)
                Grid.SetColumn(VbTutar, 2)
                GrSatir.Children.Add(VbTutar)

                sayac += 1
            Next
            GrTerazi.Children.Add(GrSatir)

            '  BtTerazi.Content = GrTerazi
            UgTerazi.Children.Add(GrTerazi)

            Execute_run("update terazi_baslik set okundu = 1 where fisno = '" & FisNo & "'", Conn, True)
            Execute_run("update terazi_satir  set okundu = 1 where fisno = '" & FisNo & "'", Conn, True)
        Next
        TpTerazi.Content = UgTerazi
    End Sub



    Private Sub TimerPluBizerba(ByVal sender As Object, ByVal e As EventArgs)
        On Error Resume Next
        TimerBizerba.Interval = New TimeSpan(0, 0, 1)
        TimerBizerba.Stop()
        Dim file_name As String = "C:\ScTicket\Satis.txt"
        Dim satir As String = ""
        Dim file_info As New FileInfo(file_name)
        If file_info.Exists Then
            Dim fr As New FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.None)
            Dim oku As New StreamReader(fr, System.Text.Encoding.Default)

            While oku.Peek <> -1
                Dim MyString As String = oku.ReadLine
                Dim MyStringSplit() As String = MyString.Split(";")

                tb = Nothing
                Dim islem As Integer = MyStringSplit(0)


                Select Case islem
                    Case 1
                        tb.fisno = MyStringSplit(1)
                        tb.terazi = MyStringSplit(2)
                        tb.reyon = MyStringSplit(3)
                        tb.satici = MyStringSplit(4)
                        tb.sayac = MyStringSplit(5)
                        tb.satir_sayisi = MyStringSplit(6)
                        tb.toplam_agirlik = MyStringSplit(7)
                        tb.satici1 = MyStringSplit(8)
                        tb.tutar = MyStringSplit(9)
                        tb.sifir = MyStringSplit(10)
                        ' tb.bas_tarih = MyStringSplit(11)
                        ' tb.bit_tarih = MyStringSplit(12)
                        tb.okundu = False
                        tb.yazildi = False

                        Kayitsql(terazi_baslik, tb, Conn, True)
                    Case 2
                        ts = Nothing
                        ts.tarih = Now
                        ts.okundu = False
                        ts.fisno = MyStringSplit(1)
                        ts.terazi = MyStringSplit(2)
                        ts.plu = MyStringSplit(3)
                        ts.satici = MyStringSplit(5)
                        ts.fiyat = MyStringSplit(6)
                        ts.agirlik = MyStringSplit(8)
                        ts.tutar = MyStringSplit(9)
                        ts.iptal = MyStringSplit(10)
                        Kayitsql(terazi_satir, ts, Conn, True)
                End Select

            End While
            fr.Close()
            fr.Dispose()
            oku.Close()
            oku.Dispose()
            file_info.Delete()
            TeraziAktar(2, 4)
        End If
        TimerBizerba.Start()
    End Sub

    Private Sub TeraziGridSecim(sender As Object, e As RoutedEventArgs)
        Dim Gr As New Grid
        Gr = sender
        Dim fisno As Integer = Gr.Uid

        If SilAktif = False Then
            Ds_read("select * from terazi_satir where fisno = '" & fisno & "'", "terazi_Satis", True, True)
            For Each dr As System.Data.DataRow In Ds.Tables("terazi_satis").Select("fisno = '" & fisno & "'")
                Dim plu As Int64 = Chk_Null("plu", dr)
                Dim miktar As Decimal = Chk_Null("agirlik", dr)
                miktar = miktar / 1000
                TbMakina.Text = miktar
                Ur.ref = 0
                Br.birim_alt = 0
                For Each drBarkod As System.Data.DataRow In Ds.Tables(Barkodlar).Select("plu = '" & plu & "'")
                    Ur.ref = Chk_Null("urun", drBarkod)
                    Ur.guid = Chk_Null("urunguid", drBarkod)
                    Br.birim_alt = Chk_Null("birim_alt", drBarkod)
                Next
                If Ur.ref <> 0 And Br.birim_alt <> 0 Then
                    Satis_grid(Ur.guid, Br.birim_alt)
                End If

                Execute_run("update terazi_baslik set yazildi = 1 where fisno = '" & fisno & "'", Conn, True)
                Execute_run("update terazi_satir  set yazildi = 1 where fisno = '" & fisno & "'", Conn, True)
                UgTerazi.Children.Remove(Gr)
                UgTerazi.UpdateLayout()
            Next
        Else
            SilAktif = False
            Dim ok As Boolean = False
            ok = Msg("Terazi Fişi İptal Edilecektir!!", True, True, False)
            If ok = True Then

                Execute_run("update terazi_baslik set iptal = 1 where fisno = '" & fisno & "'", Conn, True)
                Execute_run("update terazi_satir  set iptal = 1 where fisno = '" & fisno & "'", Conn, True)
                UgTerazi.Children.Remove(Gr)
                UgTerazi.UpdateLayout()
            End If
        End If
        BtGridUrunDurum.Background = Brushes.Blue
    End Sub

    Private Function TeraziListele(ByVal Satir As Integer, ByVal Sutun As Integer) As Grid
        On Error Resume Next
        Dim UgO As New UniformGrid With {
            .Columns = Sutun,
            .Rows = Satir
        }

        Dim GrTeraziler As New Grid

        For Each drTerazi As System.Data.DataRow In Ds.Tables("teraziler").Rows
            Dim GrUrun As New Grid
            Dim UrunRef As Int64 = drTerazi.Item("ref")
            Dim FontSize As Integer = drTerazi.Item("fontsize")
            If FontSize = 0 Then FontSize = 15
            Dim En As Integer = drTerazi.Item("en")
            If En = 0 Then En = 90
            Dim sigdir As Boolean = drTerazi.Item("sigdir")
            Dim Renk As String = ""
            Renk = drTerazi.Item("renk")
            Dim UrunFiyat As Decimal = 0
            Dim Stok As Decimal = 0
            Stok = drTerazi.Item("stok_miktar")
            UrunFiyat = drTerazi.Item("fiyat1")


            Dim UrunAciklama As String = drTerazi.Item("aciklama")
            Dim fiyat_str As String = fiyat_al(UrunFiyat).ToString
            GrUrun.Margin = New Thickness(2)
            GrUrun.Background = Brushes.Navy
            Dim GrRdBt As New RowDefinition
            GrUrun.RowDefinitions.Add(GrRdBt)
            GrRdBt.Height = New GridLength(5, GridUnitType.Star)
            Dim BtUrun As New System.Windows.Controls.Button
            BtUrun.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            BtUrun.Uid = UrunRef
            Dim Urunstyle As New Style
            Urunstyle = FindResource("RoundedButton")
            BtUrun.Style = Urunstyle
            BtUrun.Margin = New Thickness(1, 1, 1, 1)
            Dim color_ As Color
            If Renk <> "" Then
                color_ = DirectCast(ColorConverter.ConvertFromString(Renk), Color)
                BtUrun.Background = New SolidColorBrush(color_)
            Else : BtUrun.Background = Brushes.LightGray
            End If
            '************** olaylar ***************************
            AddHandler BtUrun.Click, AddressOf UrunEkle
            '**************************************************
            Dim VbBt As New Viewbox
            Dim TbBt As New TextBlock
            TbBt.Width = En
            TbBt.FontSize = FontSize
            TbBt.TextWrapping = TextWrapping.Wrap
            TbBt.Text = UrunAciklama
            VbBt.Child = TbBt
            VbBt.Margin = New Thickness(1, 1, 1, 1)
            BtUrun.Content = VbBt
            If En = 0 Or FontSize = 0 Then
                Dim EnUrunButon As Integer = GrUrun.RenderSize.Width
                EnUrunButon = EnUrunButon - ((Gr.satir - 1) * 2)
                EnUrunButon = EnUrunButon / Gr.satir
                TbBt.Width = EnUrunButon / 2
                VbBt.Stretch = Stretch.Uniform
            Else
                VbBt.Width = En
                TbBt.Width = En
                TbBt.FontSize = FontSize
                If sigdir = True Then
                    VbBt.Stretch = Stretch.Uniform
                Else
                    VbBt.Stretch = Stretch.None
                End If
            End If

            Grid.SetRow(BtUrun, 0)
            GrUrun.Children.Add(BtUrun)

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

            If ay.stokgoster_aktif Then
                Dim bolen As Integer = 0
                Dim carpan As Integer = 0
                For Each dr_birim As System.Data.DataRow In Ds.Tables("birim_alt").Select("ref = '" & Ur.birim_ust & "'")
                    bolen = dr_birim.Item("bolen")
                    carpan = dr_birim.Item("carpan")
                Next
                Stok = (Stok * bolen) / carpan
                Dim GrRdSt As New RowDefinition
                GrUrun.RowDefinitions.Add(GrRdSt)
                GrRdSt.Height = New GridLength(1, GridUnitType.Star)
                Dim VbSt As New Viewbox
                VbSt.Uid = UrunRef
                VbSt.HorizontalAlignment = Windows.HorizontalAlignment.Right
                VbSt.Stretch = Stretch.Uniform
                Dim TbSt As New TextBlock
                TbSt.Uid = UrunRef
                TbSt.HorizontalAlignment = Windows.HorizontalAlignment.Left
                TbSt.Foreground = Brushes.WhiteSmoke
                TbSt.Text = "Stok:" & adet_al(Stok).ToString

                VbSt.Child = TbSt
                VbSt.Margin = New Thickness(1, 1, 1, 1)
                Grid.SetRow(VbSt, 2)
                GrUrun.Children.Add(VbSt)
            End If
            UgO.Children.Add(GrUrun)
        Next
        GrTeraziler.Children.Add(UgO)
        Return GrTeraziler
    End Function

    Private Sub TeraziAyarlari()
        If ay.terazi_aktif Then
            BrTeraziAktif.Visibility = Windows.Visibility.Visible
            BrTeraziPasif.Visibility = Windows.Visibility.Hidden

            Teraziİp = "192.168.1.202"
            If PingTest(Teraziİp) Then
            Else
                Msg("Terazi Bağlantısı Kurulamadı Lütfen Kontrol Ediniz!!!", False, False, True)
            End If
            UgTerazi.Columns = 4
            UgTerazi.Rows = 4


            Execute_run("update terazi_baslik set okundu = 0 where yazildi = 0", Conn, True)
            Execute_run("update terazi_satir set  okundu = 0 where yazildi = 0", Conn, True)
            TeraziAktar(2, 4)
            TimerBizerba.Start()
        Else
            BrTeraziAktif.Visibility = Windows.Visibility.Hidden
            BrTeraziPasif.Visibility = Windows.Visibility.Visible
        End If
    End Sub






End Class
