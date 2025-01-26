Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.IO
Imports System.Drawing
Imports System.Windows.Xps
Imports System.Drawing.Printing
Imports System.Drawing.Text
Imports System.Data.SqlClient



Public Class W_Etiketleme


    Sub olay_ekle()

        AddHandler DgOnay.GotFocus, AddressOf DgSecOnay
        AddHandler DgStok.GotFocus, AddressOf DgSecStok
        '************************************************************
        AddHandler DgStok.KeyDown, AddressOf dgw_liste_KeyDown
        AddHandler DgOnay.KeyDown, AddressOf DgStokListeOnay_KeyDown
        '************************************************************
        AddHandler DgStok.MouseDoubleClick, AddressOf dgw_liste_DoubleClick
        AddHandler TbAciklama.KeyDown, AddressOf tb_sorgu_KeyDown

        AddHandler BtYukari.Click, AddressOf SatirYukari
        AddHandler BtAsagi.Click, AddressOf SatirAsagi

        AddHandler BtSec.Click, AddressOf Sec
        AddHandler BtSil.Click, AddressOf Sil


        AddHandler BtYaz.Click, AddressOf Olaylar_Button
        AddHandler BtListeSil.Click, AddressOf Olaylar_Button
        AddHandler BtSecilenleriSil.Click, AddressOf Olaylar_Button
        AddHandler BtTumunuSec.Click, AddressOf Olaylar_Button
        AddHandler BtDeğişenler.Click, AddressOf Olaylar_Button
        AddHandler BtCikis.Click, AddressOf Cikis
        AddHandler CbEtiket.SelectionChanged, AddressOf EtiketDegistir

        AddHandler BtGramaj.Click, AddressOf GramajYaz


    End Sub

    Private Sub GramajYaz()
        Dim Satir As Integer = 0
        Try
            Satir = DgStok.SelectedIndex
        Catch ex As Exception
            Exit Sub
        End Try
        GridVeriAktar(Satir)
        If GVd.ref > 0 Then
            If GVd.Gramaj = 0 Or GVd.Gramaj_Birim_Aciklama = "" Or GVd.Uretim_Ulke = "" Then
                GramajYaz(Satir, GVd.ref, GVd.Stokkodu, GVd.Aciklama, GVd.Fiyat, GVd.Barkod, GVd.Birim)
                GridVeriAktar(Satir)
            End If
            Dim W As New W_GramajUrun
            W.ShowDialog()
        End If
    End Sub

    Dim Dg As New System.Windows.Controls.DataGrid
    Private Sub DgSecOnay()
        Try
            Dg = DgOnay
            BtSec.Visibility = Windows.Visibility.Hidden
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DgSecStok()
        Try
            Dg = DgStok
            BtSec.Visibility = Windows.Visibility.Visible
        Catch ex As Exception
        End Try
    End Sub


    Private Sub Cikis()
        Me.Close()
    End Sub

    Private Sub EtiketDegistir()
        Dim Baslik As String = CbEtiket.Text
        If Baslik <> "" Then
            For Each dr As DataRow In Ds.Tables(etiket_baslik).Select("baslik = '" & Baslik & "'")
                Dim yazici As String = Chk_Null("yazici", dr)
                Dim i As Integer = CbYazici.Items.IndexOf(yazici)
                Try
                    CbYazici.Text = CbYazici.Items.Item(i)
                Catch ex As Exception
                End Try
            Next
        End If
    End Sub

    Private Sub Tumunu_Sec()
        Dim sayac As Integer = 0
        For Each dr As DataRow In DtStok.Rows
            onay_aktar(0)
            sayac += 1
        Next
        'For i As Integer = 0 To DgStok.Items.Count - 1


        ' onay_aktar(i)
        ' Next
        '  For Each dr As DataRow In DtStok.Rows


        '  dr.Item("onay") = 1
        ' DtOnay.Rows.Add()
        ' Dim aa As Integer = DtOnay.Rows.Count
        ' DtOnay.Rows(aa - 1).Item("ref") = dr.Item("ref")
        ' DtOnay.Rows(aa - 1).Item("stokkodu") = dr.Item("stokkodu")
        ' DtOnay.Rows(aa - 1).Item("barkod") = dr.Item("barkod")
        ' DtOnay.Rows(aa - 1).Item("aciklama") = dr.Item("aciklama")
        ' DtOnay.Rows(aa - 1).Item("birim") = dr.Item("birim")
        ' DtOnay.Rows(aa - 1).Item("fiyat") = dr.Item("fiyat")
        ' DtOnay.Rows(aa - 1).Item("birim_fiyat") = dr.Item("birim_fiyat")
        ' DtOnay.Rows(aa - 1).Item("gramaj") = dr.Item("gramaj")
        ' DtOnay.Rows(aa - 1).Item("gramaj_birim_aciklama") = dr.Item("gramaj_birim_aciklama")
        ' DtOnay.Rows(aa - 1).Item("uretim_ulke") = dr.Item("uretim_ulke")
        ' DtOnay.Rows(aa - 1).Item("son_alis_tarihi") = dr.Item("son_alis_tarihi")
        ' DtOnay.Rows(aa - 1).Item("onay") = dr.Item("onay")
        'Next

        '        DvOnay.Table = DtOnay
        '        DgOnay.ItemsSource = DvOnay
        '        DvOnay.RowFilter = "onay = 0"
        '        Grid_Liste(DgOnay)
        '        DgOnay.UpdateLayout()
        '        DtStok.Rows.Clear()
        '        liste_sayi()
    End Sub

    Private Sub Olaylar_Button(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Select Case sender.name
            Case BtSecilenleriSil.Name
                DtOnay.Rows.Clear()
                liste_sayi()

            Case BtListeSil.Name
                DtStok.Rows.Clear()
                liste_sayi()

            Case BtTumunuSec.Name
                Tumunu_Sec()


            Case BtDeğişenler.Name
                'Cursor.Current = Cursors.WaitCursor
                Degisenler()
                liste_sayi()
                'Cursor.Current = Cursors.AppStarting
            Case BtYaz.Name
                If CountListeOnay = 0 Then
                    Msg("Secim Yapınız!!?", False, False, True)
                    Exit Sub
                End If
                liste_sayi()
                If CountListeOnay > 10 Then
                    If Msg("Etiket Sayısı " & CountListeOnay & " Yinede Etiket Dökülsün Mü?", True, True, False) = False Then
                        Exit Sub
                    End If
                End If
                Yazdir()
        End Select
    End Sub


    Private Sub Yazdir()
        If CbYazici.Text = "" Then
            Msg("Yazıcı Seçiniz !!", False, False, True)
            Exit Sub
        Else
            Dim Etiket As String = CbEtiket.Text
            If Etiket <> "" Then
                Dim Adet As Integer = 1
                If tBAdet.Text <> "" And IsNumeric(tBAdet.Text) Then
                    Adet = tBAdet.Text
                End If
                Dim C_prn As New Class_Print
                C_prn.RafEtiketiPrint(Etiket, Adet, CbYazici.Text, DtOnay)
                Execute_run("update etiket_baslik set yazici = '" & CbYazici.Text & "' where baslik = '" & Etiket & "'", Conn, True)
                DtOnay.Rows.Clear()
            Else
                Msg("Etiket Seçiniz !", False, False, True)
            End If
        End If
    End Sub


    Sub CheckPrinter()
        Dim ok As Boolean = False
        Dim MR As Management.ManagementObjectCollection
        Dim MS As Management.ManagementObjectSearcher
        Dim MO As Management.ManagementObject
        MS = New Management.ManagementObjectSearcher("Select * from Win32_Printer")
        MR = MS.Get
        Dim MyPrinter As String = ""
        CbYazici.Items.Clear()
        For Each MO In MR
            MyPrinter = MO("Name")
            If MO("WorkOffline") = False Then
                CbYazici.Items.Add(MyPrinter)
                '  Pd.PrintQueue = New Printing.PrintQueue(New Printing.PrintServer, MyPrinter)
                ok = True
            End If
        Next
    End Sub

    Friend F As System.Windows.Size

    Function ValueGridRowStokListe(ByVal satir As Integer, ByVal colon As String) As String
        Dim drv As System.Data.DataRowView = CType(DgStok.Items(satir), System.Data.DataRowView)
        Return (drv.Item(colon).ToString())
    End Function
    Function ValueGridRowStokListeOnay(ByVal satir As Integer, ByVal colon As String) As String
        On Error Resume Next
        Dim drv As System.Data.DataRowView = CType(Me.DgOnay.Items(satir), System.Data.DataRowView)
        Return (drv.Item(colon).ToString())
    End Function


    Private Sub GridOlustur_Onay()
        DtOnay.TableName = "Liste_Onay"

        DtOnay.Columns.Add("ref")
        DtOnay.Columns.Add("stokkodu")
        DtOnay.Columns.Add("barkod")
        DtOnay.Columns.Add("aciklama")
        DtOnay.Columns.Add("birim")

        Dim dc_fiyat As New System.Data.DataColumn
        dc_fiyat = New System.Data.DataColumn("fiyat", System.Type.GetType("System.Decimal"))
        DtOnay.Columns.Add(dc_fiyat)

        Dim dc_birimfiyat As New System.Data.DataColumn
        dc_birimfiyat = New System.Data.DataColumn("birim_fiyat", System.Type.GetType("System.Decimal"))
        DtOnay.Columns.Add(dc_birimfiyat)

        Dim dc_gramaj As New System.Data.DataColumn
        dc_gramaj = New System.Data.DataColumn("gramaj", System.Type.GetType("System.Decimal"))
        DtOnay.Columns.Add(dc_gramaj)

        DtOnay.Columns.Add("gramaj_birim_aciklama")

        DtOnay.Columns.Add("uretim_ulke")

        Dim dc_son_alis_tarihi As New System.Data.DataColumn
        dc_son_alis_tarihi = New System.Data.DataColumn("son_alis_tarihi", System.Type.GetType("System.DateTime"))
        DtOnay.Columns.Add(dc_son_alis_tarihi)

        Dim dc_modifieddate As New System.Data.DataColumn
        dc_modifieddate = New System.Data.DataColumn("modifieddate", System.Type.GetType("System.DateTime"))
        DtOnay.Columns.Add(dc_modifieddate)

        Dim dc_createdate As New System.Data.DataColumn
        dc_createdate = New System.Data.DataColumn("createdate", System.Type.GetType("System.DateTime"))
        DtOnay.Columns.Add(dc_createdate)



        Try
            Dim dc_onay As New DataColumn
            dc_onay = New DataColumn("onay", System.Type.GetType("System.Boolean"))
            dc_onay.DefaultValue = False
            DtOnay.Columns.Add(dc_onay)
        Catch ex As Exception
        End Try
        DvOnay.RowFilter = "onay = 0"
    End Sub




    Private Sub GridVeriAktar(ByVal satir As Integer)
        On Error Resume Next
        GVd.ref = ValueGridRowStokListe(satir, "ref")
        GVd.Stokkodu = ValueGridRowStokListe(satir, "stokkodu")
        GVd.Barkod = ValueGridRowStokListe(satir, "barkod")
        GVd.Aciklama = ValueGridRowStokListe(satir, "aciklama")
        GVd.Birim = ValueGridRowStokListe(satir, "birim")
        GVd.Fiyat = ValueGridRowStokListe(satir, "fiyat")
        GVd.Birim_Fiyat = ValueGridRowStokListe(satir, "birim_fiyat")
        GVd.Gramaj = ValueGridRowStokListe(satir, "gramaj")
        GVd.Gramaj_Birim_Aciklama = ValueGridRowStokListe(satir, "gramaj_birim_aciklama")
        GVd.Uretim_Ulke = ValueGridRowStokListe(satir, "uretim_ulke")
        GVd.Son_Alis_Tarihi = ValueGridRowStokListe(satir, "son_alis_tarihi")
    End Sub


    Private Sub onay_aktar(ByVal satir As Integer)
        '  On Error Resume Next
        GVd = Nothing
        GVd.onay = True
        Dim sayac As Integer = 0
        Dim satir_onay As Integer = DgOnay.Items.Count
        GridVeriAktar(satir)
        Dim gramaj As Boolean = False
        If gramaj = True Then
            If GVd.ref = 0 Then Exit Sub
            If GVd.Gramaj = 0 Or GVd.Gramaj_Birim_Aciklama = "" Or GVd.Uretim_Ulke = "" Then
                GramajYaz(satir, GVd.ref, GVd.Stokkodu, GVd.Aciklama, GVd.Fiyat, GVd.Barkod, GVd.Birim)
                GridVeriAktar(satir)
            End If
        End If


        Dim drvO As System.Data.DataRowView = CType(DgStok.Items(satir), System.Data.DataRowView)
        drvO.Item("onay") = True
        Dim ok As Boolean = False
        If GVd.ref <> 0 Then
            For Each dr As DataRow In DtOnay.Select("ref = " & GVd.ref)
                ok = True
            Next
            GridVeri(GVd.ref,
                     GVd.Stokkodu,
                     GVd.Barkod,
                     GVd.Aciklama,
                     GVd.Birim,
                     GVd.Fiyat,
                     GVd.Birim_Fiyat,
                     GVd.Gramaj,
                     GVd.Gramaj_Birim_Aciklama,
                     GVd.Uretim_Ulke,
                     GVd.Son_Alis_Tarihi,
                     GVd.onay)
        End If
    End Sub


    Private Sub UpdateGramaj(ByVal ref As Int64,
                             ByVal Gramaj As Decimal,
                             ByVal Gramaj_Birim_Aciklama As String,
                             ByVal Gramaj_Birim_Fiyat As Decimal,
                             ByVal Gramaj_Birim_Sira As Integer,
                             ByVal Uretim_Ulke As String)

        Dim Sql As String = ""
        Sql &= " update urunler set "
        Sql &= " gramaj = @gramaj"
        Sql &= ",gramaj_birim_aciklama =  @gramaj_birim_aciklama "
        Sql &= ",gramaj_birim_fiyat = @gramaj_birim_fiyat"
        Sql &= ",gramaj_birim_sira = @gramaj_birim_sira"
        Sql &= ",uretim_ulke =  @uretim_ulke "
        Sql &= " where ref = '" & ref & "'"
        Dim cmd As New SqlCommand
        cmd = urunler_Parametre(Sql, cmd, Gramaj, Gramaj_Birim_Aciklama, Gramaj_Birim_Fiyat, Gramaj_Birim_Sira, Uretim_Ulke)
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()
        cmd.Connection.Close()
    End Sub

    Private Sub GramajYaz(ByVal satir As Integer,
                          ByVal ref As Int64,
                          ByVal stokkodu As String,
                          ByVal aciklama As String,
                          ByVal fiyat As Decimal,
                          ByVal barkod As String,
                          ByVal birim As String)
        '  On Error Resume Next

        Dim bolen As Decimal
        Dim brm As String = ""
        Dim GrmStr As String = ""
        Dim GrmDec As Decimal = 0
        Dim Gr As String = "GR"
        Dim Index As Integer = 0
        Select Case birim
            Case "Kilo"
                bolen = 1
                brm = "Kilo"
                GrmStr = 1
            Case "Litre"
                bolen = 1
                brm = "Litre"
                GrmStr = 1
            Case Else
                Index = aciklama.IndexOf("GR")
                If Index > 0 Then
                    bolen = 1000
                    brm = "Kilo"
                Else
                    Index = aciklama.IndexOf("KG")
                    If Index > 0 Then
                        bolen = 1
                        brm = "Kilo"
                    Else
                        Index = aciklama.IndexOf("LT")
                        If Index > 0 Then
                            bolen = 1
                            brm = "Litre"
                        Else
                            Index = aciklama.IndexOf("ML")
                            If Index > 0 Then
                                bolen = 1000
                                brm = "Litre"
                            Else
                                Index = aciklama.IndexOf("CL")
                                If Index > 0 Then
                                    bolen = 100
                                    brm = "Litre"
                                Else
                                    Index = aciklama.IndexOf("MT")
                                    If Index > 0 Then
                                        bolen = 1
                                        brm = "Metre"
                                    Else
                                        Index = aciklama.IndexOf("CM")
                                        If Index > 0 Then
                                            bolen = 100
                                            brm = "Metre"
                                        Else
                                            Index = aciklama.IndexOf("AD")
                                            If Index > 0 Then
                                                bolen = 1
                                                brm = "Adet"
                                            Else
                                                Index = aciklama.IndexOf("LI")
                                                If Index > 0 Then
                                                    bolen = 1
                                                    brm = "Adet"
                                                Else
                                                    Index = aciklama.IndexOf("Lİ")
                                                    If Index > 0 Then
                                                        bolen = 1
                                                        brm = "Adet"
                                                    Else
                                                        Index = aciklama.IndexOf("LU")
                                                        If Index > 0 Then
                                                            bolen = 1
                                                            brm = "Adet"
                                                        Else

                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                If Index > 0 Then
                    Dim TotIndex As Integer = aciklama.Length

                    Dim numerik As Integer = 0
                    For i As Integer = Index - 1 To 0 Step -1
                        Dim Chr As Char = aciklama(i)
                        If IsNumeric(Chr) = True Or Chr = "," Or Chr = "." Or Chr = "/" Then
                            If IsNumeric(Chr) = True Then
                                numerik += +1
                                GrmStr = Chr & GrmStr
                            Else
                                If Chr = "." Then GrmStr = "," & GrmStr
                                If Chr = "/" Then GrmStr = "/" & GrmStr
                            End If
                        Else
                            If numerik >= 1 Then
                                Select Case GrmStr
                                    Case "1/2"
                                        GrmStr = "0,5"
                                    Case "1/1"
                                        GrmStr = "1"
                                    Case "1/5"
                                        GrmStr = "0,2"
                                End Select
                                GrmStr = GrmStr / bolen
                                Exit For
                            End If
                        End If
                    Next
                End If
        End Select
        '********************************************************************
        If IsNumeric(GrmStr) Then
            If GrmStr > 0 Then
                Dim ulke As String = ""
                If barkod <> "" Then
                    ulke = UlkeAl(barkod)
                End If
                GrmDec = Convert.ToDecimal(GrmStr)
                Dim BirimFiyat As Decimal = 0
                BirimFiyat = fiyat_al(fiyat / GrmDec)
                '  Dim satir As Integer = 0
                '  satir = DgStok.SelectedIndex
                If satir < 0 Then Exit Sub

                Dim BirimSira As Integer = 0
                Select Case brm
                    Case "Adet"
                        BirimSira = 0
                    Case "Kilo"
                        BirimSira = 1
                    Case "Metre"
                        BirimSira = 2
                    Case "Litre"
                        BirimSira = 3
                End Select
                UpdateGramaj(ref, GrmDec, brm, BirimFiyat, BirimSira, ulke)
                Dim drvO As System.Data.DataRowView = CType(DgStok.Items(satir), System.Data.DataRowView)
                drvO.Item("gramaj") = GrmDec
                drvO.Item("birim_fiyat") = BirimFiyat
                drvO.Item("gramaj_birim_aciklama") = brm
                drvO.Item("uretim_ulke") = ulke
                liste_sayi()
            End If
        End If
    End Sub


    Function urunler_Parametre(ByVal Sql As String,
                             ByVal cmd As SqlCommand,
                             ByVal gramaj As Decimal,
                             ByVal gramaj_birim_aciklama As String,
                             ByVal gramaj_birim_fiyat As Decimal,
                             ByVal gramaj_birim_sira As Integer,
                             ByVal uretim_ulke As String)

        With cmd
            .Parameters.Clear()
            .CommandText = Sql
            .Connection = Conn
            .Parameters.Add(Kayit_Parametre("@gramaj", gramaj))
            .Parameters.Add(Kayit_Parametre("@gramaj_birim_aciklama", gramaj_birim_aciklama))
            .Parameters.Add(Kayit_Parametre("@gramaj_birim_fiyat", gramaj_birim_fiyat))
            .Parameters.Add(Kayit_Parametre("@gramaj_birim_sira", gramaj_birim_sira))
            .Parameters.Add(Kayit_Parametre("@uretim_ulke", uretim_ulke))
        End With
        Return cmd
    End Function



    Dim CountListe As Integer = 0
    Dim CountListeOnay As String = 0



    Private Sub liste_sayi()
        Try
            CountListe = DgStok.Items.Count
            CountListeOnay = DgOnay.Items.Count
            TbListeSayi.Text = CountListe & "/" & CountListeOnay
        Catch ex As Exception
        End Try
    End Sub


    Private Sub SatiraGitDgStok(ByVal satir As Integer)
        Try
            Dim Obj = DgStok.Items(satir)
            DgStok.ScrollIntoView(Obj)
            DgStok.SelectedItem = Obj
        Catch ex As Exception
        End Try
        DgStok.Focus()
    End Sub


    Private Sub Sec()
        Dim Satir As Integer
        Try
            Satir = DgStok.SelectedIndex
        Catch ex As Exception
            Exit Sub
        End Try
        If Satir >= 0 Then
            Try
                onay_aktar(DgStok.SelectedIndex)
                liste_sayi()
            Catch ex As Exception
            End Try
            SatiraGitDgStok(Satir)
        End If
    End Sub

    Private Sub Sil()
        Dim Satir As Integer
        Try

            Satir = Dg.SelectedIndex
            If Satir >= 0 Then
                Dim drvO As System.Data.DataRowView = CType(Dg.Items(Satir), System.Data.DataRowView)
                drvO.Item("onay") = True
            End If
        Catch ex As Exception
        End Try
        Try
            Dim Obj = Dg.Items(Satir)
            Dg.ScrollIntoView(Obj)
            Dg.SelectedItem = Obj
        Catch ex As Exception
        End Try
        Dg.Focus()
    End Sub

    Private Sub dgw_liste_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        Select Case e.Key
            Case Key.Enter
                onay_aktar(DgStok.Items.IndexOf(DgStok.CurrentItem))
                liste_sayi()
        End Select
    End Sub

    Private Sub DgStokListeOnay_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        Select Case e.Key
            Case Key.Delete
        End Select
    End Sub

    Private Sub dgw_liste_DoubleClick()
        Try
            Dim aciklama As String = ""
            Dim aciklama_kirp As String = ""
            Dim satir As Integer = 0
            satir = DgStok.SelectedIndex
            onay_aktar(DgStok.Items.IndexOf(DgStok.CurrentItem))
            liste_sayi()
            SatiraGitDgStok(satir)
        Catch ex As Exception
        End Try
    End Sub

    Function fiyat_al(ByVal fiyat As String) As String
        If fiyat = "" Then fiyat = 0
        fiyat = Math.Round(Convert.ToDecimal(fiyat), 2).ToString("N")
        Return fiyat
    End Function





    Dim AfisAciklama() As String
    Dim AfisFiyat() As String
    Dim AfisBarkod() As String


    Dim DvStok As New System.Data.DataView
    Dim DvOnay As New System.Data.DataView
    Dim DtStok As New System.Data.DataTable
    Dim DtOnay As New System.Data.DataTable

    Private Sub GridVeri(ByVal Ref As String,
                         ByVal Stokkodu As String,
                         ByVal Barkod As String,
                         ByVal Aciklama As String,
                         ByVal Birim As String,
                         ByVal Fiyat As String,
                         ByVal Birim_Fiyat As String,
                         ByVal Gramaj As String,
                         ByVal Gramaj_Birim_Aciklama As String,
                         ByVal Uretim_Ulke As String,
                         ByVal Son_Alis_Tarihi As String,
                         ByVal onay As Boolean)

        DtOnay.Rows.Add()
        Dim aa As Integer = DtOnay.Rows.Count
        DtOnay.Rows(aa - 1).Item("ref") = Ref
        DtOnay.Rows(aa - 1).Item("stokkodu") = Stokkodu
        DtOnay.Rows(aa - 1).Item("barkod") = Barkod
        DtOnay.Rows(aa - 1).Item("aciklama") = Aciklama
        DtOnay.Rows(aa - 1).Item("birim") = Birim
        If Fiyat <> "" Then DtOnay.Rows(aa - 1).Item("fiyat") = Fiyat
        If Birim_Fiyat <> "" Then DtOnay.Rows(aa - 1).Item("birim_fiyat") = Birim_Fiyat
        If Gramaj <> "" Then DtOnay.Rows(aa - 1).Item("gramaj") = Gramaj
        If Gramaj_Birim_Aciklama <> "" Then DtOnay.Rows(aa - 1).Item("gramaj_birim_aciklama") = Gramaj_Birim_Aciklama

        DtOnay.Rows(aa - 1).Item("uretim_ulke") = Uretim_Ulke
        If Son_Alis_Tarihi <> "" Then
            DtOnay.Rows(aa - 1).Item("son_alis_tarihi") = Son_Alis_Tarihi
        Else
            DtOnay.Rows(aa - 1).Item("son_alis_tarihi") = Now
        End If
        DtOnay.Rows(aa - 1).Item("onay") = False


        'DgOnay.ItemsSource = DtOnay.DefaultView


        DvOnay.Table = DtOnay
        DgOnay.ItemsSource = DvOnay
        DvOnay.RowFilter = "onay = 0"

        DgOnay.Columns(0).Visibility = System.Windows.Visibility.Hidden

        DgOnay.Columns(1).Header = "STOKKODU"
        DgOnay.Columns(1).Width = 100

        DgOnay.Columns(2).Header = "BARKOD"
        DgOnay.Columns(2).Width = 125

        DgOnay.Columns(3).Header = "AÇIKLAMA"
        DgOnay.Columns(3).Width = 350

        DgOnay.Columns(4).Header = "BİRİM"
        Dim TbColBirim As New DataGridTextColumn
        TbColBirim = DgOnay.Columns(4)
        TbColBirim = TextBoxStyle(TbColBirim, TextAlignment.Right, 75)

        DgOnay.Columns(5).Header = "FİYAT"
        Dim TbColFiyat As New DataGridTextColumn
        TbColFiyat = DgOnay.Columns(5)
        Try
            TbColFiyat.Binding.StringFormat = "₺#,##0.#0"
        Catch ex As Exception
        End Try
        TbColFiyat = TextBoxStyle(TbColFiyat, TextAlignment.Right, 80)

        DgOnay.Columns(6).Header = "BİRİM FİYAT"
        Dim TbColBirimFiyat As New DataGridTextColumn
        TbColBirimFiyat = DgOnay.Columns(6)
        Try
            TbColBirimFiyat.Binding.StringFormat = "₺#,##0.##0"
        Catch ex As Exception
        End Try
        TbColBirimFiyat = TextBoxStyle(TbColBirimFiyat, TextAlignment.Right, 100)

        DgOnay.Columns(7).Header = "GRAMAJ"
        Dim TbColGramaj As New DataGridTextColumn
        TbColGramaj = DgOnay.Columns(7)
        Try
            TbColGramaj.Binding.StringFormat = "#,##0.##0"
        Catch ex As Exception
        End Try
        TbColGramaj = TextBoxStyle(TbColGramaj, TextAlignment.Right, 100)

        DgOnay.Columns(8).Visibility = System.Windows.Visibility.Hidden ' Gramaj Birim Açıklama


        DgOnay.Columns(9).Header = "ÜRETİM ÜLKE"
        DgOnay.Columns(9).Width = 110

        DgOnay.Columns(10).Header = "ALIM TARİHİ"
        Dim TbColAlimTarihi As New DataGridTextColumn
        TbColAlimTarihi = DgOnay.Columns(10)
        Try
            TbColAlimTarihi.Binding.StringFormat = "dd/MM/yyyy"
        Catch ex As Exception
        End Try
        TbColAlimTarihi = TextBoxStyle(TbColAlimTarihi, TextAlignment.Left, 125)



        DgOnay.Columns(11).Visibility = System.Windows.Visibility.Hidden
        DgOnay.Columns(12).Visibility = System.Windows.Visibility.Hidden
        DgOnay.Columns(13).Visibility = System.Windows.Visibility.Hidden
        '  DgOnay.Columns(14).Visibility = System.Windows.Visibility.Hidden



        DgOnay.IsReadOnly = True
        DgOnay.CanUserDeleteRows = True





    End Sub

    Private Function TextBoxStyle(ByVal TbCol As DataGridTextColumn,
                                  ByVal al As TextAlignment,
                                  ByVal Width_ As Integer) As DataGridTextColumn
        TbCol.Width = Width_
        Dim styl As New Style
        styl.Setters.Add(New Setter(TextBlock.TextAlignmentProperty, al))
        styl.Setters.Add(New Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap))
        TbCol.CellStyle = styl
        Return TbCol
    End Function


    Public Sub CheckForExistingInstance()
        Dim prc As New Process
        Dim prg_name As String = Process.GetCurrentProcess.ProcessName
        If Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName).Length > 1 Then
            Dim sayi As Integer = Process.GetCurrentProcess.SessionId
            Dim prg_mem() As Process = Process.GetProcesses()
            For i As Integer = 0 To prg_mem.Length - 1
                Dim prg As String = prg_mem(i).ProcessName
                ' MsgBox(prg)
                If (prg_name = prg_mem(i).ProcessName) And (sayi <> prg_mem(i).SessionId) Then
                End If
            Next
        End If
    End Sub

    Private Sub Grid_Liste(ByVal dg As System.Windows.Controls.DataGrid)
        dg.Columns(0).Visibility = System.Windows.Visibility.Hidden

        dg.Columns(1).Header = "STOKKODU"
        dg.Columns(1).Width = 100

        dg.Columns(2).Header = "BARKOD"
        dg.Columns(2).Width = 125

        dg.Columns(3).Header = "AÇIKLAMA"
        dg.Columns(3).Width = 350

        dg.Columns(4).Header = "BİRİM"
        Dim TbColBirim As New DataGridTextColumn
        TbColBirim = dg.Columns(4)
        TbColBirim = TextBoxStyle(TbColBirim, TextAlignment.Right, 75)


        dg.Columns(5).Header = "FİYAT"
        Dim TbColFiyat As New DataGridTextColumn
        TbColFiyat = dg.Columns(5)
        Try
            TbColFiyat.Binding.StringFormat = "₺#,##0.#0"
        Catch ex As Exception
        End Try
        TbColFiyat = TextBoxStyle(TbColFiyat, TextAlignment.Right, 80)

        dg.Columns(6).Header = "BİRİM FİYAT"
        Dim TbColBirimFiyat As New DataGridTextColumn
        TbColBirimFiyat = dg.Columns(6)
        Try
            TbColBirimFiyat.Binding.StringFormat = "₺#,##0.#0"
        Catch ex As Exception
        End Try
        TbColBirimFiyat = TextBoxStyle(TbColBirimFiyat, TextAlignment.Right, 100)

        dg.Columns(7).Header = "GRAMAJ"
        Dim TbColGramaj As New DataGridTextColumn
        TbColGramaj = dg.Columns(7)
        Try
            TbColGramaj.Binding.StringFormat = "#,##0.##0"
        Catch ex As Exception
        End Try
        TbColGramaj = TextBoxStyle(TbColGramaj, TextAlignment.Right, 100)

        dg.Columns(8).Visibility = System.Windows.Visibility.Hidden

        dg.Columns(9).Header = "ÜRETİM ÜLKE"
        dg.Columns(9).Width = 100

        dg.Columns(10).Header = "ALIM TARİHİ"
        Dim TbColAlimTarihi As New DataGridTextColumn
        TbColAlimTarihi = dg.Columns(10)
        Try
            TbColAlimTarihi.Binding.StringFormat = "dd/MM/yyyy"
        Catch ex As Exception
        End Try
        TbColAlimTarihi = TextBoxStyle(TbColAlimTarihi, TextAlignment.Left, 125)


        dg.Columns(11).Visibility = System.Windows.Visibility.Hidden
        dg.Columns(12).Visibility = System.Windows.Visibility.Hidden
        dg.Columns(13).Visibility = System.Windows.Visibility.Hidden
        '   dg.Columns(14).Visibility = System.Windows.Visibility.Hidden

        dg.IsReadOnly = True
        dg.UpdateLayout()
    End Sub

    Private Function SqlSorgu() As String
        Dim Sql As String
        Sql = "   SELECT     "
        Sql &= " urunler.ref as ref"
        Sql &= ",urunler.stokkodu as stokkodu"
        Sql &= ",barkodlar.barkod as barkod"
        Sql &= ",urunler.aciklama as aciklama"
        Sql &= ",birim_alt.isaret as birim"
        Sql &= ",urunler.fiyat1 as fiyat"
        Sql &= ",(ISNULL(urunler.fiyat1 / NULLIF(urunler.gramaj, 0), 0)) as [gramaj]"
        Sql &= ",urunler.gramaj as gramaj"
        Sql &= ",urunler.gramaj_birim_aciklama as gramaj_birim_aciklama"
        Sql &= ",urunler.uretim_ulke as uretim_ulke"
        Sql &= ",urunler.son_alis_tarihi as son_alis_tarihi"
        Sql &= ",urunler.createdate as createdate"
        Sql &= ",urunler.modifieddate as modifieddate"
        Sql &= " FROM         "
        Sql &= " urunler "
        Sql &= " INNER JOIN barkodlar ON urunler.ref = barkodlar.urun "
        Sql &= " INNER JOIN birim_alt ON barkodlar.birim_alt = birim_alt.ref"
        Return Sql
    End Function


    Private Sub Sor(ByVal tur As String, ByVal text As String)
        Dim Stokkodu As String = ""
        text = Replace(text, "*", "%")
        Try
            Ds.Tables("Stok_Liste").Clear()
        Catch ex As Exception
        End Try
        Dim Sql As String = SqlSorgu()
        Select Case CbTur.Text
            Case "Açıklama"
                Sql &= "  WHERE urunler.iptal = 0 and urunler.active = 1 and (aciklama LIKE N'%" & text & "%')"
            Case "Barkod"
                Sql &= "  WHERE urunler.iptal = 0 and urunler.active = 1 and (barkod LIKE N'%" & text & "%')"
            Case "Stokkodu"
                Sql &= "  WHERE urunler.iptal = 0 and urunler.active = 1 and (stokkodu LIKE N'%" & text & "%')"
        End Select
        TbAciklama.Text = ""


        Ds_read(Sql, "Stok_Liste", True, True)
        Try
            Stokkodu = Ds.Tables("Stok_Liste").Rows(0).Item("stokkodu")
        Catch ex As Exception
            MsgBox("Ürün Bulunamadı")
            Exit Sub
        End Try

        DtStok = Ds.Tables("Stok_Liste")
        DvStok.Table = DtStok

        Try
            Dim dc_onay As New DataColumn
            dc_onay = New DataColumn("onay", System.Type.GetType("System.Boolean"))
            dc_onay.DefaultValue = False
            DtStok.Columns.Add(dc_onay)
        Catch ex As Exception
        End Try

        DvStok.RowFilter = "onay = 0"

        DgStok.ItemsSource = DvStok
        Grid_Liste(DgStok)
        If RbDogrudanYazici.IsChecked = True Then
            If DtStok.Rows.Count > 10 Then
                If Msg("Etiket Miktarı " & DtStok.Rows.Count & " Yinede Döktürmek İstiyor Musunuz?", True, True, False) Then
                    Tumunu_Sec()
                    Yazdir()
                    DtStok.Rows.Clear()
                    DtOnay.Rows.Clear()
                End If
            Else
                Tumunu_Sec()
                Yazdir()
                DtStok.Rows.Clear()
                DtOnay.Rows.Clear()
            End If
        End If
    End Sub

    Private Sub Degisenler()
        Try
            Ds.Tables("Stok_Liste").Clear()
        Catch ex As Exception
        End Try
        Dim Sql As String = SqlSorgu()
        Sql &= "  WHERE "
        Sql &= " (urunler.modifieddate >= urunler.etiketdate) "
        Sql &= "  or urunler.etiketdate is null"
        Sql &= "  order by urunler.modifieddate"
        Ds_read(Sql, "Stok_Liste", True, True)
        Dim Stokkodu As String = ""
        Try
            Stokkodu = Ds.Tables("Stok_Liste").Rows(0).Item("stokkodu")
        Catch ex As Exception
            MsgBox("Ürün Bulunamadı")
            Exit Sub
        End Try

        DtStok = Ds.Tables("Stok_Liste")

        DvStok.Table = DtStok
        Try
            Dim dc_onay As New DataColumn
            dc_onay = New DataColumn("onay", System.Type.GetType("System.Boolean"))
            dc_onay.DefaultValue = False
            DtStok.Columns.Add(dc_onay)
        Catch ex As Exception
        End Try
        DvStok.RowFilter = "onay = 0"
        DgStok.ItemsSource = DvStok
        Grid_Liste(DgStok)
        ' If RbDogrudanYazici.IsChecked = True Then
        ' Yazdir()
        ' DtStok.Rows.Clear()
        'End If
    End Sub

    Private Sub tb_sorgu_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs)
        Select Case e.Key
            Case Key.Enter
                If TbAciklama.Text <> "" Then Sor(Me.CbTur.Text, TbAciklama.Text)
                liste_sayi()
        End Select
    End Sub


    Private Function Grid_Olustur(ByVal width As Integer,
                                  ByVal height As Integer,
                                  ByVal vertical_alingment As VerticalAlignment,
                                  ByVal horizontal_alingment As System.Windows.HorizontalAlignment,
                                  ByVal show_grid_line As Boolean,
                                  ByVal background_color As System.Windows.Media.Color) As Grid
        Dim Gr As New Grid
        With Gr
            .Width = width
            .VerticalAlignment = vertical_alingment
            .HorizontalAlignment = horizontal_alingment
            .ShowGridLines = show_grid_line
            .Background = New SolidColorBrush(background_color)
        End With
        Return Gr
    End Function


    Private Function TextBlock_Olustur(ByVal text As String,
                                       ByVal font_size As Integer,
                                       ByVal font_weight As FontWeight,
                                       ByVal foreground_color As System.Windows.Media.Color,
                                       ByVal vertical_alingment As VerticalAlignment,
                                       ByVal horizontal_alingment As System.Windows.HorizontalAlignment,
                                       ByVal setrow As Integer,
                                       ByVal setcolumn As Integer,
                                       ByVal margin As Integer)
        Dim Tb As New TextBlock
        With Tb
            .Text = text
            .FontSize = font_size
            .FontWeight = font_weight
            .Foreground = New SolidColorBrush(foreground_color)
            .VerticalAlignment = vertical_alingment
            .HorizontalAlignment = horizontal_alingment
        End With
        Grid.SetRow(Tb, setrow)
        Grid.SetColumn(Tb, setcolumn)
        Return Tb
    End Function

    Private Sub SatirAsagi()
        Dim s As Integer = 0
        Try
            s = Dg.SelectedIndex
        Catch ex As Exception
        End Try
        s += 1
        Try
            Dim Obj = Dg.Items(s)
            Dg.ScrollIntoView(Obj)
            Dg.SelectedItem = Obj
        Catch ex As Exception
        End Try
        Dg.Focus()
    End Sub

    Private Sub SatirYukari()
        Dim s As Integer = 0
        Try
            s = Dg.SelectedIndex
        Catch ex As Exception
        End Try
        s -= 1
        Try
            Dg.Focus()
            Dim Obj = Dg.Items(s)
            Dg.ScrollIntoView(Obj)
            Dg.SelectedItem = Obj
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Dataset_Oku_Etiketleme()
        On Error Resume Next
        Ds_read("select * from etiket_baslik", etiket_baslik, True, True)
        Ds_read("select * from etiket_satir", etiket_satir, True, True)
        For Each dr As DataRow In Ds.Tables(etiket_baslik).Rows
            CbEtiket.Items.Add(Chk_Null("baslik", dr))
            'CbYazici.Items.Clear()
            'CbYazici.Items.Add(Chk_Null("yazici", dr))
        Next
        CbEtiket.Text = CbEtiket.Items(0)
        For Each dr As DataRow In Ds.Tables(etiket_baslik).Select("baslik = '" & CbEtiket.Text & "'")
            For Each comboItem In CbYazici.Items
                Dim Yazici As String = Chk_Null("yazici", dr)
                If comboItem = Yazici Then
                    CbYazici.Text = Yazici
                End If
            Next comboItem
        Next
    End Sub

    Private Sub CbEtiket_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CbEtiket.SelectionChanged
        For Each dr As DataRow In Ds.Tables(etiket_baslik).Select("baslik = '" & CbEtiket.Text & "'")
            For Each comboItem In CbYazici.Items
                Dim Yazici As String = Chk_Null("yazici", dr)
                If comboItem = Yazici Then
                    CbYazici.Text = Yazici
                    Exit Sub
                End If
            Next comboItem
        Next
    End Sub


    Private Sub Main()
        CheckForExistingInstance()
        WindowState = System.Windows.WindowState.Maximized
        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
        WindowStyle = System.Windows.WindowStyle.None
        Try
            CheckPrinter()
        Catch ex As Exception
        End Try
        olay_ekle()
        GridOlustur_Onay()
        CbTur.Text = "Açıklama"
        Dataset_Oku_Etiketleme()
        Dg = DgStok
    End Sub


End Class

