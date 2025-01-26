Imports System.Data
Imports System.Data.SqlClient

Public Class C_Zraporu

    Function KdvMatrah() As Boolean
        Dim Sql As String = ""
        Sql &= "Select hareket_satir.kdv_oran"
        Sql &= ", SUM(hareket_satir.kdv_tutar) AS [Kdv Tutarı]"
        Sql &= ", SUM(hareket_satir.net_tutar) AS [Kdv Matrahı]"
        Sql &= ", COUNT(hareket_baslik.ref) AS [Belge Adeti]"
        Sql &= " From hareket_baslik INNER Join hareket_satir On hareket_baslik.guid = hareket_satir.baslikguid"
        Sql &= " Where (hareket_baslik.gunsonu <> 1) And (hareket_satir.iptal <> 1) And (hareket_baslik.iptal <> 1)"
        Sql &= " Group By hareket_satir.kdv_oran, hareket_baslik.fisturu, hareket_baslik.kasa_kod"
        Sql &= " HAVING(hareket_baslik.kasa_kod = N'" & pd.kasa_kod & "')"
        If Ds_read(Sql, "kdvmatrah", Conn, True, True) = False Then Return False
        Return True
    End Function

    Function OdemeToplam() As Boolean
        Dim Sql As String = ""
        Sql &= " Select "
        Sql &= " tahsilat.odeme_kod as odeme"
        Sql &= ",cast(sum(tahsilat.tutar) + sum(tahsilat.paraustu) as decimal(15,2)) as toplam "
        Sql &= " FROM"
        Sql &= " tahsilat"
        Sql &= " INNER JOIN hareket_baslik ON tahsilat.baslik = hareket_baslik.ref"
        Sql &= " where "
        '  Sql &= " (hareket_baslik.ref > 66  '" & HbIlkRef & "') AND (hareket_baslik.ref <= '" & HbSonRef & "')"
        Sql &= " hareket_baslik.gunsonu <> 1 "
        Sql &= " and hareket_baslik.iptal <> 1 and hareket_baslik.kasa_kod = '" & pd.kasa_kod & "'"
        Sql &= " group by"
        Sql &= " tahsilat.odeme_kod"
        If Ds_read(Sql, "odemetoplam", Conn, True, True) = False Then Return False
        Return True
    End Function



    Public Function KayitZraporu(Zonay As Boolean) As Boolean
        Dim Zno As Int64 = 0
        Try
            Zno = Convert.ToInt64(Execute_Oku("select max(zno) as zno from zraporu", "zno", Conn, False))
        Catch ex As Exception
        End Try
        Zno += 1

        Ds_read("select * from hareket_baslik where gunsonu = 0", "HbR", True, True)
        Ds_read("select * from hareket_satir where  gunsonu = 0", "HsR", True, True)
        Ds_read("select * from kasa_hareket where  gunsonu = 0", "KhR", True, True)
        Ds_read("select * from tahsilat where iptal = 0 and gunsonu = 0", "ThR", True, True)

        Ds_read("select * from kdv where iptal = 0", kdv, True, True)
        Ds_read("select * from odeme where iptal = 0", odeme, True, True)
        zr = Nothing

        zr.guid = Guid.NewGuid.ToString
        zr.subeguid = sb.guid
        zr.kasaguid = ks.guid
        zr.tarih = Now
        zr.zno = Zno

        '****************************************************************************************************
        Dim kdvtutar(Ds.Tables(kdv).Rows.Count - 1) As Decimal
        Dim kdvmatrah(Ds.Tables(kdv).Rows.Count - 1) As Decimal
        Dim kdvoran(Ds.Tables(kdv).Rows.Count - 1) As Byte

        Dim Sayac As Byte = 0
        For Each Dr As DataRow In Ds.Tables(kdv).Rows
            kdvoran(Sayac) = Dr.Item("oran")
            If Ds.Tables("HsR").Compute("Sum(net_tutar)", "iptal = 0 and baslik_iptal = 0 and kdv_oran = '" & kdvoran(Sayac) & "'") IsNot DBNull.Value Then
                kdvmatrah(Sayac) = Ds.Tables("HsR").Compute("Sum(kdv_matrah)", "iptal = 0 and baslik_iptal = 0 and kdv_oran = '" & kdvoran(Sayac) & "'")
            End If
            If Ds.Tables("HsR").Compute("Sum(kdv_tutar)", "iptal = 0 and baslik_iptal = 0 and kdv_oran = '" & kdvoran(Sayac) & "'") IsNot DBNull.Value Then
                kdvtutar(Sayac) = Ds.Tables("HsR").Compute("Sum(kdv_tutar)", "iptal = 0 and baslik_iptal = 0 and kdv_oran = '" & kdvoran(Sayac) & "'")
            End If
            Sayac += 1
        Next

        Dim odemetutar(Ds.Tables(odeme).Rows.Count - 1) As Decimal
        Dim odemekod(Ds.Tables(odeme).Rows.Count - 1) As String

        Sayac = 0
        For Each Dr As DataRow In Ds.Tables(odeme).Rows
            odemekod(Sayac) = Dr.Item("kod")
            If Ds.Tables("ThR").Compute("Sum(tutar)", "iptal = 0 and odeme_kod = '" & odemekod(Sayac) & "'") IsNot DBNull.Value Then
                Dim Paraustu As Decimal = Ds.Tables("ThR").Compute("Sum(paraustu)", "iptal = 0 and odeme_kod = '" & odemekod(Sayac) & "'")
                odemetutar(Sayac) = Ds.Tables("ThR").Compute("Sum(tutar)", "iptal = 0 and odeme_kod = '" & odemekod(Sayac) & "'")
                odemetutar(Sayac) += Paraustu
            End If
            Sayac += 1
        Next

        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "iptal = 0") IsNot DBNull.Value Then
            zr.geneltoplam = Ds.Tables("HbR").Compute("Sum(net_toplam)", "iptal = 0")
        End If
        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "iptal = 0") IsNot DBNull.Value Then
            zr.genelkdvtoplam = Ds.Tables("HbR").Compute("Sum(kdv_toplam)", "iptal = 0")
        End If
        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "iptal = 1") IsNot DBNull.Value Then
            zr.geneliptaltoplam = Ds.Tables("HbR").Compute("Sum(net_toplam)", "iptal = 1")
        End If
        If Ds.Tables("HsR").Compute("Sum(net_tutar)", "iptal = 1") IsNot DBNull.Value Then
            zr.satiriptaltoplam = Ds.Tables("HsR").Compute("Sum(net_tutar)", "iptal = 1")
        End If
        zr.fisadet = Ds.Tables("HbR").Select("belgeturu = 'fis' And iptal = 0").Count
        zr.efaturaadet = Ds.Tables("HbR").Select("belgeturu = 'efatura' And iptal = 0").Count
        zr.earsivadet = Ds.Tables("HbR").Select("belgeturu = 'earsiv' And iptal = 0").Count

        zr.fisiptaladet = Ds.Tables("HbR").Select("belgeturu = 'fis' And iptal = 1").Count
        zr.efaturaiptaladet = Ds.Tables("HbR").Select("belgeturu = 'efatura' And iptal = 1").Count
        zr.earsiviptaladet = Ds.Tables("HbR").Select("belgeturu = 'earsiv' And iptal = 1").Count

        If Ds.Tables("HbR").Compute("Sum(indirim_toplam)", "iptal = 0") IsNot DBNull.Value Then
            zr.indirimtoplam = Ds.Tables("HbR").Compute("Sum(indirim_toplam)", "iptal = 0")
        End If
        If Ds.Tables("HbR").Compute("Sum(indirim_satir_toplam)", "iptal = 0") IsNot DBNull.Value Then
            zr.satirindirimtoplam = Ds.Tables("HbR").Compute("Sum(indirim_satir_toplam)", "iptal = 0")
        End If

        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'fis' And iptal = 0") IsNot DBNull.Value Then
            zr.fistoplam = Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'fis' And iptal = 0")
        End If
        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'efatura' And iptal = 0") IsNot DBNull.Value Then
            zr.fistoplam = Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'efatura' And iptal = 0")
        End If
        If Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'earsiv' And iptal = 0") IsNot DBNull.Value Then
            zr.earsivtoplam = Ds.Tables("HbR").Compute("Sum(net_toplam)", "belgeturu = 'earsiv' And iptal = 0")
        End If

        If Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 2 And iptal = 0") IsNot DBNull.Value Then
            zr.kasaodeme = Ds.Tables("KhR").Compute("Sum()", "fisturu = 2 And iptal = 0")
        End If
        If Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 3 And iptal = 0") IsNot DBNull.Value Then
            zr.kasaavans = Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 3 And iptal = 0")
        End If
        If Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 4 And iptal = 0") IsNot DBNull.Value Then
            zr.cariodeme = Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 4 And iptal = 0")
        End If
        If Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 5 And iptal = 0") IsNot DBNull.Value Then
            zr.caritahsilat = Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 5 And iptal = 0")
        End If
        If Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 6 And iptal = 0") IsNot DBNull.Value Then
            zr.giderpusulasi = Ds.Tables("KhR").Compute("Sum(tutar)", "fisturu = 6 And iptal = 0")
        End If

        zr.merkezmutabakat = True
        zr.mac = mac_adress()

        zr.upload = False
        zr.createdate = Now
        'Drz.Item("modifieddate") = "aqa"

        If Kayitsql(zraporu, zr, Conn, True) > 0 Then
            Dim C As New Class_Print80mm
            C.ZraporuPrint80mm(reg.Yazici, odemekod, odemetutar, kdvoran, kdvmatrah, kdvtutar, Zonay)
            If Zonay = True Then ZSifirla()
        End If

        ' Dim mesaj As String = DokumZRaporuMailHtml(GS.sayac, tbl, HbIlkTarih, HbSonTarih)
        ' Select Case reg.fisboyutu
        ' Case "80mm"
        ' Dim C As New Class_Print
        ' C.ZraporuPrint(reg.Yazici, GS.sayac, HbIlkTarih, HbSonTarih)
        ' Case "58mm"
        ' Dim C As New Class_Print58mm
        ' C.ZraporuPrint58mm(reg.Yazici, GS.sayac, HbIlkTarih, HbSonTarih)
        ' Case Else
        ' Dim C As New Class_Print58mm
        ' C.ZraporuPrint58mm(reg.Yazici, GS.sayac, HbIlkTarih, HbSonTarih)
        ' End Select
        ' If ay.sender <> "" Then
        ' If ay.email1 <> "" Then
        ' SendMail(ay.sender, ay.email1, "ZRaporu", mesaj)
        ' End If
        '     If ay.email2 <> "" Then
        '     SendMail(ay.sender, ay.email2, "ZRaporu", mesaj)
        ' End If
        ' End If
        Return False
    End Function

    Private Sub ZSifirla()
        Execute_run("update hareket_baslik set gunsonu = 1 where gunsonu = 0", Conn, True)
        Execute_run("update hareket_satir set gunsonu = 1 where gunsonu = 0", Conn, True)
        Execute_run("update kasa_hareket set gunsonu = 1 where gunsonu = 0", Conn, True)
        Execute_run("update tahsilat set gunsonu = 1 where gunsonu = 0", Conn, True)
    End Sub

End Class
