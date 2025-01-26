Imports System.Data



Public Class WpfAskiListe

    Private Sub Olay_Ekle()
        AddHandler BtSil.Click, AddressOf Sil
        AddHandler BtCikis.Click, AddressOf Cikis
        AddHandler BtSec.Click, AddressOf Sec
        AddHandler DgAski.SelectionChanged, AddressOf AskiDetayListele
    End Sub


    Private Sub AskiDetayListele()
        Dim ref As New Integer
        Try
            ref = DgAski.SelectedItem("ref")
        Catch ex As Exception
            Exit Sub
        End Try
        DetayListele(ref)
    End Sub

   

    Private Sub Cikis()
        hb = Nothing
        hs = Nothing
        Me.Close()
    End Sub

    Private Sub Sil()
        Dim ref As Integer = 0
        Try
            ref = DgAski.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            If Msg("Silme İşlemini Onaylıyormusunuz?", True, True, False) Then

                Execute_run("update hareket_baslik set iptal = 1 where ref = '" & ref & "'", Conn, True)
                DgAski.SelectedIndex = 0
                AskiListele()
            End If
        Else
            Msg("Seçim Yapınız!!", False, False, True)
        End If
    End Sub

    Private Sub Sec()
        Dim ref As Integer = 0
        Try
            ref = DgAski.SelectedItem("ref")
        Catch ex As Exception
            Msg("Seçim Yapınız!!", False, False, True)
            Exit Sub
        End Try
        If ref <> 0 Then
            hb.ref = ref
            Me.Close()
        End If
    End Sub

    Private Sub AskiListele()

        Dim aski As String = "aski"
        ds_read("select ref,sayac,tarih from hareket_baslik where aski = 1 and iptal <> 1 order by ref asc", aski, Conn, True, True)
        Dim AskiIlk As Integer = 0
        For Each dr As DataRow In Ds.Tables(aski).Rows
            AskiIlk = dr.Item("ref")
            Exit For
        Next
        Try
            DtAski = Ds.Tables(aski)
            DvAski.Table = DtAski
            DgAski.ItemsSource = DvAski
            DvAski.Sort = "sayac ASC"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        DgAski.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        GridColNumeric(DgAski, 1, 1, "SAYAÇ", TextAlignment.Left, 150)
        GridColTarih(DgAski, 2, 2, "TARİH/SAAT", TextAlignment.Left, 350)
        DgAski.UpdateLayout()
        DetayListele(AskiIlk)
    End Sub


    Private Sub DetayListele(ByVal ref As Integer)
        If ref = 0 Then Exit Sub

        ds_read("select ref,urun_aciklama,miktar,birim_alt_isaret,iptal from hareket_satir where baslik = '" & ref & "' and iptal <> 1 order by satir", "askidetay", Conn, True, True)
        Try
            DtDetay = Ds.Tables("askidetay")
            DvDetay.Table = DtDetay
            DgDetay.ItemsSource = DvDetay
            DvDetay.RowFilter = "iptal <> 1"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        DgDetay.Columns(0).Visibility = Windows.Visibility.Hidden 'ref
        GridColString(DgDetay, 1, 1, "ÜRÜN AÇIKLAMA", TextAlignment.Left, 250)
        GridColNumeric(DgDetay, 2, 2, "MİKTAR", TextAlignment.Left, 100)
        GridColString(DgDetay, 3, 3, "BİRİM", TextAlignment.Left, 100)
        DgDetay.Columns(4).Visibility = Windows.Visibility.Hidden 'iptal
        DgDetay.UpdateLayout()
    End Sub

    Dim DtAski As New DataTable
    Dim DvAski As New DataView

    Dim DtDetay As New DataTable
    Dim DvDetay As New DataView


    Private Sub Main()
        hb = Nothing
        hs = Nothing
        EkranDizayn_ThreeDBorderWindow(Me)
        Olay_Ekle()
        AskiListele()
    End Sub

End Class
