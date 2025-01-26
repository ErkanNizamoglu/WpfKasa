Imports System.Data.SqlClient


Public Class Wpf_ListeSayim


    Dim DvR As New System.Data.DataView
    Dim DtR As New System.Data.DataTable


    Private Sub Listele()
        Dim Sql As String = ""
        Sql &= " SELECT "
        Sql &= " ref as ref"
        Sql &= ",FORMAT(tarih,'dd.MM.yyyy') as tarih"
        Sql &= ",belgeno as belgeno"
        Sql &= ",iptal as iptal"
        Sql &= ",net_toplam as toplam"
        Sql &= " FROM"
        Sql &= " hareket_baslik"
        Sql &= " WHERE fisturu = 9 and iptal <> 1 order by sayac"


        ds_read(Sql, "rapor", Conn, True, True)

        DtR = Ds.Tables("rapor")
        DvR.Table = DtR

        DgFis.ItemsSource = DvR
        DgFis.UpdateLayout()
    End Sub


    Private Sub Degistir()
        Dim ref As Integer = 0
        Try
            ref = DgFis.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            hb = Nothing
            hs = Nothing
            hb.ref = ref
            Fis_Stok = stok_fisleri.SayimFisi
            Select Case Fis_Stok
                Case stok_fisleri.SayimFisi
                    Dim W As New WpfFisSayim
                    W.ShowDialog()
                Case stok_fisleri.MalAlim
                    TbBaslik.Text = "MAL ALIM FİŞLERİ"
                Case stok_fisleri.AlimIade
                    TbBaslik.Text = "ALIM İADE FİŞLERİ"
                Case stok_fisleri.UretimdenGiris
                    TbBaslik.Text = "ÜRETİMDEN GİRİŞ FİŞLERİ"
                Case stok_fisleri.SarfFisi
                    TbBaslik.Text = "SARF FİŞLERİ"
                Case stok_fisleri.FireFisi
                    TbBaslik.Text = "FİRE FİŞLERİ"
                Case stok_fisleri.MalSatis
                    TbBaslik.Text = "SATIŞ FİŞLERİ"
                Case stok_fisleri.DevirFisi
                    TbBaslik.Text = "DEVİR FİŞLERİ"
            End Select
            Listele()
        Else
            Msg("Seçim Yapınız!!", False, False, True)
        End If
    End Sub


    Private Sub Incele()


    End Sub

    Private Sub KayitSil()
        Dim ok As Boolean = False
        ok = Msg("Fiş Silinecektir Onaylıyor musunuz?", True, True, False)
        If ok = True Then
            Dim ref As New Integer
            Try
                ref = DgFis.SelectedItem("ref")
            Catch ex As Exception
                Msg("Seçim Yapınız!!", False, False, True)
                Exit Sub
            End Try
            If ref <> 0 Then

                Execute_run("update hareket_baslik set iptal = 1 where ref = '" & ref & "'", Conn, True)

                Ds_read("select * from hareket_satir  where baslik = '" & ref & "'", "hareket_satir_eski", True, True)
                For Each dr As System.Data.DataRow In Ds.Tables("hareket_satir_eski").Select("baslik = '" & ref & "'")
                    Dim SsMiktar As Decimal = Chk_Null("miktar", dr)
                    Dim SsIslemYonu As Byte = Chk_Null("islemyonu", dr)
                    Dim SsBolen As Byte = Chk_Null("bolen", dr)
                    Dim SsCarpan As Byte = Chk_Null("carpan", dr)
                    Dim Eskimiktar As Decimal = 0
                    Eskimiktar = ((SsMiktar * SsCarpan) / SsBolen) * SsIslemYonu
                    Dim UrunRef As Integer = Chk_Null("urun", dr)
                    ds_read("select * from urunler where ref = '" & UrunRef & "'", Urunler, True, True)
                    For Each drUrun As System.Data.DataRow In Ds.Tables(Urunler).Select("ref = '" & UrunRef & "'")
                        Dim stok_miktar As Decimal = Chk_Null("stok_miktar", drUrun)
                        stok_miktar = stok_miktar - Eskimiktar
                        Dim Sql As String = "update urunler set stok_miktar = @yenianamiktarfark where ref = '" & UrunRef & "'"
                        Try
                            Dim cmd As New SqlCommand
                            With cmd
                                .CommandText = Sql
                                .Parameters.Add(Kayit_Parametre("@yenianamiktarfark", stok_miktar))
                                .Connection = Conn
                                .Connection.Open()
                                .ExecuteNonQuery()
                                .Connection.Close()
                            End With
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                        If Conn.State = System.Data.ConnectionState.Open Then Conn.Close()
                    Next
                Next
                Listele()
            End If
        End If
    End Sub

    Private Sub YeniKayit()
        hb = Nothing
        hs = Nothing
        Fis_Stok = stok_fisleri.SayimFisi

        Select Case Fis_Stok
            Case stok_fisleri.SayimFisi
                Dim W As New WpfFisSayim
                W.ShowDialog()
            Case stok_fisleri.MalAlim
                TbBaslik.Text = "MAL ALIM FİŞLERİ"
            Case stok_fisleri.AlimIade
                TbBaslik.Text = "ALIM İADE FİŞLERİ"
            Case stok_fisleri.UretimdenGiris
                TbBaslik.Text = "ÜRETİMDEN GİRİŞ FİŞLERİ"
            Case stok_fisleri.SarfFisi
                TbBaslik.Text = "SARF FİŞLERİ"
            Case stok_fisleri.FireFisi
                TbBaslik.Text = "FİRE FİŞLERİ"
            Case stok_fisleri.MalSatis
                TbBaslik.Text = "SATIŞ FİŞLERİ"
            Case stok_fisleri.DevirFisi
                TbBaslik.Text = "DEVİR FİŞLERİ"
        End Select
        Listele()
    End Sub

    Private Sub Cikis()
        RemoveTable("rapor")
        RemoveTable("hareket_satir_eski")
        Me.Close()
    End Sub

    Function MenuYap(degis As Boolean,
                     incele_ As Boolean,
                     yeni As Boolean,
                     sil As Boolean,
                     cik As Boolean) As Grid

        Dim Grd As New Grid
        Dim sayi As Integer


        If degis Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("degistir", "DEĞİŞTİR", 0, sayi, Brushes.Lime, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf Degistir
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        If incele_ Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("incele", "İNCELE", 0, sayi, Brushes.Aquamarine, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf Incele

            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        If yeni Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("yenikayit", "YENİ KAYIT", 0, sayi, Brushes.LightSkyBlue, Windows.HorizontalAlignment.Stretch)
            Bt.Margin = New Thickness(3)
            AddHandler Bt.Click, AddressOf YeniKayit

            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        If sil Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("sil", "SİL", 0, sayi, Brushes.Green, Windows.HorizontalAlignment.Stretch)
            AddHandler Bt.Click, AddressOf KayitSil
            Dim style As New Style
            style = FindResource("RoundedButton")
            Bt.Style = style
            Grd.Children.Add(Bt)
            sayi += 1
        End If
        If cik Then
            Dim GrColDef As New ColumnDefinition
            Grd.ColumnDefinitions.Add(GrColDef)
            Dim Bt As New Button
            Bt.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
            Bt = ButtonYap("cikis", "ÇIKIŞ", 0, sayi, Brushes.IndianRed, Windows.HorizontalAlignment.Stretch)
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
        EkranDizayn_ThreeDBorderWindow(Me)

        Select Case Fis_Stok
            Case stok_fisleri.SayimFisi
                TbBaslik.Text = "SAYIM FİŞLERİ"
            Case stok_fisleri.MalAlim
                TbBaslik.Text = "MAL ALIM FİŞLERİ"
            Case stok_fisleri.AlimIade
                TbBaslik.Text = "ALIM İADE FİŞLERİ"
            Case stok_fisleri.UretimdenGiris
                TbBaslik.Text = "ÜRETİMDEN GİRİŞ FİŞLERİ"
            Case stok_fisleri.SarfFisi
                TbBaslik.Text = "SARF FİŞLERİ"
            Case stok_fisleri.FireFisi
                TbBaslik.Text = "FİRE FİŞLERİ"
            Case stok_fisleri.MalSatis
                TbBaslik.Text = "SATIŞ FİŞLERİ"
            Case stok_fisleri.DevirFisi
                TbBaslik.Text = "DEVİR FİŞLERİ"
        End Select
        Listele()
        GrMenu.Children.Add(MenuYap(True, True, True, True, True))
    End Sub
End Class
