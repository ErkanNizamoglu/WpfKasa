
Imports System.Data
Imports System.Data.SqlClient
Imports System.Reflection
Imports System.Threading
Imports System.Windows.Threading

Public Class WpfPluAktarimKasa
    Dim ok As Boolean = False
    Dim PluCount As Integer
    Dim PluSayac As Integer

    Private Sub Kapat()
        Me.Close()
    End Sub

    Private Sub BtSayac()
        PbPlu.Maximum = PluCount
        PbPlu.Minimum = 0
        PbPlu.Value = PluSayac
    End Sub

    Private Sub TextSAyac()
        TextPlu.Text = PbPlu.Value & "/" & PluCount
    End Sub

    Delegate Sub NextPrimeDelegate()
    Delegate Sub SecondPrimeDelegate()
    Delegate Sub ThirdPrimeDelegate()
    Delegate Sub FourtPrimeDelegate()

    Private Sub Baslik()
        'TextHead.Text = TabloTxt
        TextHead.Text = "LÜTFEN BEKLEYİNİZ!!"
    End Sub

    Private Sub Aciklama()
        'TextHead.Text = TabloTxt
        TextPlu.Text = TabloTxt
    End Sub


    Dim TabloTxt As String = ""


    Sub ChkPlu()
        Dim UpdateOk As Boolean = False
        Dim ConnRemote As New SqlConnection("Data Source=" & Reg.Server & ";User ID=" & Reg.User & ";Initial Catalog=" & Reg.Data & ";Password =" & Reg.Pass)
        Dim ConnLocal As SqlConnection = Conn
        Dim DsRemote As New DataSet
        Dim DsLocal As New DataSet
        TextHead.Dispatcher.BeginInvoke(DispatcherPriority.Background, New NextPrimeDelegate(AddressOf Baslik))
        If Ds_read("select * from plu where active = 1 order by sira", DsRemote, "PluRemote", ConnRemote, False) = False Then
            TextHead.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf Kapat))
            Exit Sub
        End If
        If Ds_read("select * from plu", DsLocal, "PluLocal", ConnLocal, False) = False Then
            'Msg("Plu Tablosu Okunamadı", False, False, True)
            TextHead.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf Kapat))
            Exit Sub
        End If

        Dim tablo As String = Nothing
        Dim myFieldInfo() As FieldInfo = Nothing
        Dim myType_genel As Type = GetType(tablo_degisken)
        myFieldInfo = myType_genel.GetFields
        For j As Integer = 0 To myFieldInfo.Length - 1
            tablo = myFieldInfo(j).Name

            Dim SubeGonderim As Boolean = False
            Dim MerkezAktarim As Boolean = False
            Select Case tablo
                Case ayarlar
                    MerkezAktarim = True
                    SubeGonderim = False
                Case backupdata
                    MerkezAktarim = False
                    SubeGonderim = False
                Case Cari
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Barkodlar
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Barkod_Ulke
                    MerkezAktarim = False
                    SubeGonderim = False
                Case tahsilat
                    MerkezAktarim = False
                    SubeGonderim = True
                Case pos
                    MerkezAktarim = True
                    SubeGonderim = False
                Case birim_ust
                    MerkezAktarim = True
                    SubeGonderim = False
                Case birim_alt
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Urunler
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Urunozellik
                    MerkezAktarim = True
                    SubeGonderim = False
                Case kasa
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Fiyat
                    MerkezAktarim = True
                    SubeGonderim = False
                Case FiyatGrubu
                    MerkezAktarim = True
                    SubeGonderim = False
                Case hareket_baslik
                    MerkezAktarim = False
                    SubeGonderim = True
                Case hareket_satir
                    MerkezAktarim = False
                    SubeGonderim = True
                Case kasa_hareket
                    MerkezAktarim = False
                    SubeGonderim = True
                Case Grup
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Indirim
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Kullanici
                    MerkezAktarim = True
                    SubeGonderim = False
                Case Plutus
                    MerkezAktarim = False
                    SubeGonderim = False
                Case yetki
                    MerkezAktarim = True
                    SubeGonderim = False
                Case odeme
                    MerkezAktarim = True
                    SubeGonderim = False
                Case kupur
                    MerkezAktarim = True
                    SubeGonderim = False
                    ' Case gunsonu
                    '    MerkezAktarim = False
                    '   SubeGonderim = True
                    ' Case gunsonu_merkez
                    '    MerkezAktarim = True
                 '   SubeGonderim = False
                Case giderler
                    MerkezAktarim = True
                    SubeGonderim = False
                Case para
                    MerkezAktarim = True
                    SubeGonderim = False
                Case flag
                    MerkezAktarim = True
                    SubeGonderim = False
                Case sube
                    MerkezAktarim = True
                    SubeGonderim = False
                Case bolge
                    MerkezAktarim = True
                    SubeGonderim = False
                Case kdv
                    MerkezAktarim = True
                    SubeGonderim = False
                Case kdvaktarim
                    MerkezAktarim = False
                    SubeGonderim = False
                Case terazi
                    MerkezAktarim = True
                    SubeGonderim = False
                Case etiket_baslik
                    MerkezAktarim = False
                    SubeGonderim = False
                Case etiket_satir
                    MerkezAktarim = False
                    SubeGonderim = False
                Case terazi_satir
                    MerkezAktarim = False
                    SubeGonderim = False
                Case terazi_baslik
                    MerkezAktarim = False
                    SubeGonderim = False
                Case Plu
                    MerkezAktarim = False
                    SubeGonderim = False
                Case Else

            End Select
            If MerkezAktarim = True Then
                For Each drLocal As DataRow In DsLocal.Tables("PluLocal").Select("tablo = '" & tablo & "'")
                    Dim TarihLocal As DateTime = Chk_Null("tarih", drLocal)
                    Dim Tarihstrlocal As String = TarihAyarlaislem(TarihLocal)
                    Dim Sql As String = "select * from " & tablo & " where createdate > " & Tarihstrlocal & " or modifieddate > " & Tarihstrlocal
                    If Ds_read(Sql, DsRemote, tablo, ConnRemote, False) Then
                        Dim TblRemote As System.Data.DataTable = DsRemote.Tables(tablo)
                        If TblRemote.Rows.Count > 0 Then
                            TextPlu.Dispatcher.BeginInvoke(DispatcherPriority.Background, New SecondPrimeDelegate(AddressOf Aciklama))
                            If Bulk_Update(TblRemote, Conn, True) Then
                                TarihLocal = Now
                                Execute_run("update plu Set oku = 0, tarih = " & TarihAyarlaislem(TarihLocal) & " where tablo = '" & tablo & "'", Conn, True)
                                UpdateOk = True
                            End If
                        End If
                    End If
                Next
            End If
            If SubeGonderim = True Then


            End If
        Next
        If UpdateOk Then
            DataSetOku(True, True, True, True, True, True, True, True, True, True, True, True, True, True)
        End If
        TextHead.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf Kapat))
    End Sub


    Private Sub WpPlu_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PluSayac = 0
        PbPlu.Value = 0
        Dim trd As Thread = Nothing
        trd = New Thread(AddressOf ChkPlu)
        trd.IsBackground = True
        trd.Start()
    End Sub
End Class
