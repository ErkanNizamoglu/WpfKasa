Imports System.Data



Public Class W_Gecmis

    Private Sub Olay_Ekle()
        AddHandler BtiadeDegisim.Click, AddressOf IadeDegisimFisi
        AddHandler BtBelgeKopya.Click, AddressOf BelgeNushasi
        AddHandler BtCikis.Click, AddressOf Cikis

        AddHandler DgListe.SelectionChanged, AddressOf BaslikDetayListele
    End Sub


    Private Sub IadeDegisimFisi()


    End Sub


    Private Sub BaslikDetayListele()
        Dim ref As New Integer
        Try
            ref = DgListe.SelectedItem("ref")
        Catch ex As Exception
            Exit Sub
        End Try
        DetayListele(ref)
    End Sub

    Private Sub BaslikListele()
        Dim BasTarihStr As String = TarihAl(Now.ToString) & " 00:00:00"
        Dim BitTarihStr As String = TarihAl(Now.ToString) & " 23:59:00"
        '────────────────────────────────────────────────
        Dim Sql As String
        Sql = "  select Top(25)"
        Sql &= " ref "
        Sql &= ",sayac "
        Sql &= ",cast(net_toplam As Decimal(15,3)) As net_toplam "
        Sql &= ",tarih "
        Sql &= " from "
        Sql &= " hareket_baslik "
        Sql &= " Where "
        Sql &= " iptal <> 1 "
        Sql &= " AND CAST(tarih as datetime) BETWEEN '" & BasTarihStr & "' AND '" & BitTarihStr & "'"
        Sql &= " order by tarih DESC"
        '────────────────────────────────────────────────
        Ds_read(Sql, "liste", Conn, True, True)
        '────────────────────────────────────────────────
        Dim Baslikilk As Integer = 0
        For Each dr As DataRow In Ds.Tables("liste").Rows
            Baslikilk = dr.Item("ref")
            Exit For
        Next
        Try
            DtListe = Ds.Tables("liste")
            DvListe.Table = DtListe
            DgListe.ItemsSource = DvListe
            DvListe.Sort = "sayac DESC"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        '────────────────────────────────────────────────
        DgListe.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        GridColNumeric(DgListe, 1, 1, "SAYAÇ", TextAlignment.Left, 100)
        GridColTL(DgListe, 2, 2, "TOPLAM", TextAlignment.Left, 200)
        GridColTarih(DgListe, 3, 3, "TARİH/SAAT", TextAlignment.Left, 200)
        '────────────────────────────────────────────────
        DgListe.UpdateLayout()
        DatagridSira(DgListe, 0)
        DetayListele(Baslikilk)
    End Sub

    Private Sub DetayListele(ByVal Ref As Long)
        '────────────────────────────────────────────────
        If Ref = 0 Then Exit Sub
        '────────────────────────────────────────────────
        Dim Sql As String
        Sql = "  select ref"
        Sql &= " ,iptal"
        Sql &= " ,urun_aciklama"
        Sql &= " ,cast(miktar As Decimal(15,3)) As miktar"
        Sql &= " ,birim_alt_isaret"
        Sql &= " ,fiyat"
        Sql &= " ,net_tutar"
        Sql &= " from "
        Sql &= " hareket_satir "
        Sql &= " where "
        Sql &= " baslik = '" & Ref & "' and iptal <> 1 "
        Sql &= " order by satir"
        '────────────────────────────────────────────────
        Ds_read(Sql, "detay", Conn, True, True)
        Try
            DtDetay = Ds.Tables("detay")
            DvDetay.Table = DtDetay
            DgDetay.ItemsSource = DvDetay
            DvDetay.RowFilter = "iptal <> 1"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        '────────────────────────────────────────────────
        DgDetay.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        DgDetay.Columns(1).Visibility = Windows.Visibility.Hidden 'ref
        GridColString(DgDetay, 2, 2, "ÜRÜN AÇIKLAMA", TextAlignment.Left, 250)
        GridColNumeric(DgDetay, 3, 3, "MİKTAR", TextAlignment.Right, 100)
        GridColString(DgDetay, 4, 4, "BİRİM", TextAlignment.Left, 100)
        GridColTL(DgDetay, 5, 5, "FİYAT", TextAlignment.Right, 100)
        GridColTL(DgDetay, 6, 6, "TUTAR", TextAlignment.Right, 100)
        '────────────────────────────────────────────────
        DgDetay.UpdateLayout()
    End Sub

    Private Sub Cikis()
        On Error Resume Next
        '────────────────────────────────────────────────
        hb = Nothing
        hs = Nothing
        th = Nothing
        Ds.Tables.Remove("liste")
        Ds.Tables.Remove("detay")
        Me.Close()
    End Sub



    Private Sub BelgeNushasi()
        Dim ref As Integer = 0
        Try
            ref = DgListe.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            hb.ref = ref
            hb = DegiskenAl(hareket_baslik, hb.ref, True)
            If hb.ref > 0 Then
                If Ds_read("Select * from hareket_satir where baslik = '" & hb.ref & "'", hareket_satir, True, True) Then
                    If Ds_read("Select * from tahsilat where baslik = '" & hb.ref & "'", tahsilat, True, True) Then
                        Yazdir(True)
                        Cikis()
                    End If
                End If
            End If
        End If
    End Sub


    Private Sub Yazdir(ByVal fatura As Boolean)

        Select Case reg.fisboyutu
            Case "80mm"
                If fatura Then
                    Dim C_prn As New Class_Print80mm
                    C_prn.FisPrint80mm(reg.Yazici)
                Else
                    Dim C_prn As New Class_Print
                    C_prn.FisPrint(reg.Yazici)
                End If
            Case "58mm"
                Dim C_prn As New Class_Print58mm
                C_prn.FisPrint58mm(reg.Yazici)
            Case Else
                Dim C_prn As New Class_Print58mm
                C_prn.FisPrint58mm(reg.Yazici)
                ' Msg("Fiş boyutunu Şeciniz!!", False, False, True)
                ' Exit Sub
        End Select
        hb.dokum = True
        hb.dokum_sayisi += 1
        Execute_run("update hareket_baslik set belgeturu = '" & hb.belgeturu & "', faturano = '" & hb.faturano & "', ebelgegonderim = " & Convert.ToByte(hb.ebelgegonderim) & ", dokum_sayisi = '" & hb.dokum_sayisi & "', dokum = 1 where ref = '" & hb.ref & "'", Conn, True)
    End Sub



    Dim DtListe As New DataTable
    Dim DvListe As New DataView

    Dim DtDetay As New DataTable
    Dim DvDetay As New DataView


    Private Sub Main()
        hb = Nothing
        hs = Nothing
        EkranDizayn_ThreeDBorderWindow(Me)
        Olay_Ekle()
        BaslikListele()
    End Sub

End Class
