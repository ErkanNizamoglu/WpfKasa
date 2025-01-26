
Imports System.Data
Imports System.Threading
Imports System.Windows.Threading




Public Class WpfPluKasa
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
        '  TextPlu.Text = PbPlu.Value & "/" & PluCount
    End Sub

    Delegate Sub NextPrimeDelegate()
    Delegate Sub SecondPrimeDelegate()
    Delegate Sub ThirdPrimeDelegate()

    Private Sub Baslik()
        'TextHead.Text = TabloTxt
        TextHead.Text = "LÜTFEN BEKLEYİNİZ!!"
        TextHead.UpdateLayout()
    End Sub

    Dim AktarilanGrup As String
    Dim AktarilanTablo As String

    Private Sub GrupTextYaz()
        Dim Tb As New TextBlock
        VbText.Child = Tb
        Tb.Text = AktarilanTablo & "(" & Count.ToString & ")"
        Me.UpdateLayout()
    End Sub

    Private Sub GrupText()
        'TextHead.Text = TabloTxt
        'System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False

    End Sub

    Private Sub PbText()
    End Sub

    Dim Count As Long = 0

    Sub UpdatePluAcilis()

        Dim Mac As String = mac_adress()
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf Baslik))
        If Ds_read("select * from plu order by grup", DsServisOku, Plu, Conn, True) Then
            For Each DrPlu As DataRow In DsServisOku.Tables(Plu).Rows
                Dim Tablo As String = DrPlu.Item("tablo")
                AktarilanGrup = Chk_Null("grup", DrPlu)
                AktarilanTablo = Tablo
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf GrupTextYaz))


                'Me.Dispatcher.BeginInvoke(DispatcherPriority.Send, New SecondPrimeDelegate(AddressOf GrupText))


                '  System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Send, New NextPrimeDelegate(AddressOf GrupTextYaz))
                'System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Send)

                Dim Tarihlocal As String = TarihAyarlaislem(Chk_Null("tarih", DrPlu))
                If Tarihlocal = Nothing Then Tarihlocal = TarihAyarlaislem(Date.Parse("01/01/2000"))
                Dim Sql As String
                Select Case AktarilanGrup
                    Case "satis"
                    Case "satis"
                        Sql = "select * from " & Tablo & " where mac = '" & Mac & "zzzz' and gunsonu = 0"
                    Case Else
                        Sql = "select * from " & Tablo & " where createdate > " & Tarihlocal & " or modifieddate > " & Tarihlocal
                End Select
                Dim PluReadDateStr As String = TarihAyarlaislem(Now)
                Dim Dt As New DataTable
                Dt = ServisOkuTable(Tablo, Sql)
                PbPlu.Dispatcher.BeginInvoke(DispatcherPriority.Background, New ThirdPrimeDelegate(AddressOf PbText))
                '--------------------------------------------------------------------------------------------
                Dim Limit As Long = 1000
                Dim Sayac As Long = 0
                Dim Dtc As DataTable = Dt.Clone
                Dim Ok As Boolean = False
                Count = 0
                '--------------------------------------------------------------------------------------------
                If Dt.Rows.Count > 0 Then
                    Count = Dt.Rows.Count
                    '--------------------------------------------------------------------------------------------
                    For i As Integer = 0 To Dt.Rows.Count - 1
                        Dim r As DataRow = Dt.Rows(i)
                        Dtc.ImportRow(r)
                        Sayac += 1
                        If Sayac = Limit Or i = Dt.Rows.Count - 1 Then
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf GrupTextYaz))
                            Ok = Bulk_Update(Dtc, Conn, True)
                            If Ok = False Then Exit For
                            Dtc.Rows.Clear()
                            Sayac = 0
                        End If
                    Next
                    If Ok Then
                        Dim SqlExecute As String = "update plu Set tarih = " & PluReadDateStr & " where tablo = '" & Tablo & "'"
                        If Execute_Run(SqlExecute, Conn) Then
                        End If
                    End If

                    '--------------------------------------------------------------------------------------------
                    '  If Bulk_Update(Dt, Conn, True) Then
                    '  Dim SqlExecute As String = "update plu Set tarih = " & PluReadDateStr & " where tablo = '" & Tablo & "'"
                    '  If Execute_Run(SqlExecute, Conn) Then
                    ' End If
                    ' End If
                End If
            Next
        End If  ' dsread
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf Kapat))
    End Sub

    Sub UpdatePlu()
        OkChkPlu = True
        Dim Dt As New DataTable
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf Baslik))
        For Each DrAktarim As DataRow In DsServisOku.Tables("aktarim").Select("status = 0")
            Dim PluReadDateStr As String = TarihAyarlaislem(DrAktarim.Item("checkdate"))
            Dim AktarimGuid As Guid = Guid.Parse(DrAktarim.Item("guid"))
            Dim PluGrup As String = DrAktarim.Item("grup")
            Dim Sql As String = "select * from plu where grup = '" & PluGrup & "'"
            Try
                Dt = ServisOkuTable(Plu, Sql)
                DsServisOku.Tables.Add(Dt)
                For Each DrPlu As DataRow In DsServisOku.Tables("plu").Select("grup = '" & PluGrup & "'")
                    Count = 0
                    Dim Tbl As String = DrPlu.Item("tablo")
                    AktarilanTablo = Tbl
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf GrupTextYaz))
                    Dim LastUpdateDate As String = TarihAyarlaislem(DrPlu.Item("tarih"))
                    Dim DtAktarim As DataTable = ServisOkuTable(Tbl, "select * from " & Tbl & " where createdate > " & LastUpdateDate & " or modifieddate > " & LastUpdateDate)

                    Dim Limit As Long = 1000
                    Dim Sayac As Long = 0
                    Dim Dtc As DataTable = DtAktarim.Clone
                    Dim Ok As Boolean = False
                    Count = DtAktarim.Rows.Count
                    Dtc.TableName = DtAktarim.TableName
                    For i As Integer = 0 To DtAktarim.Rows.Count - 1
                        Dim r As DataRow = DtAktarim.Rows(i)
                        Dtc.ImportRow(r)
                        Sayac += 1
                        If Sayac = Limit Or i = DtAktarim.Rows.Count - 1 Then
                            Ok = Bulk_Update(Dtc, Conn, True)
                            If Ok = False Then Exit For
                            Dtc.Rows.Clear()
                            Sayac = 0
                        End If
                    Next
                    If Ok Then
                        ServisExecuteRunThr("update plu Set tarih = " & PluReadDateStr & " where tablo = '" & Tbl & "'")
                    End If
                Next
                ServisExecuteRunThr("update aktarim set readdate = " & PluReadDateStr & ",status = 1 where guid = '" & AktarimGuid.ToString & "'")
            Catch ex As Exception
            End Try

        Next
        DsServisOku.Tables.Clear()
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, New Action(AddressOf Kapat))
        OkChkPlu = False
    End Sub

    Dim Full As Boolean

    Sub Main(ByVal Full_ As Boolean)
        Full = Full_
        Me.Topmost = True
        Me.Show()
        PluSayac = 0
        PbPlu.Value = 0
        If Full Then
            UpdatePluAcilis()
        Else
            UpdatePlu()
        End If
    End Sub


    Private Sub WpfPluKasa_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    End Sub
End Class
