Imports System.Data
Imports System.Data.SqlClient

Public Class C_TabloUpgrade

    Private Function HareketBaslik() As Boolean
        Dim Sql As String = ""
        Sql &= "INSERT INTO [dbo].[hareket_baslik]"
        Sql &= "([guid]"
        Sql &= ",[suberef]"
        Sql &= ",[sube]"
        Sql &= ",[upload]"
        Sql &= ",[faturano]"
        Sql &= ",[belgeno]"
        'Sql &= ",[gnfisturu]"
        Sql &= ",[fisturu]"
        Sql &= ",[sayim_turu]"
        Sql &= ",[fiskod]"
        Sql &= ",[sayac]"
        Sql &= ",[cari]"
        Sql &= ",[cari_kod]"
        Sql &= ",[kullanici]"
        Sql &= ",[kullanici_kod]"
        Sql &= ",[kasa]"
        Sql &= ",[kasa_kod]"
        Sql &= ",[tarih]"
        Sql &= ",[islem_tarihi]"
        Sql &= ",[satir_sayisi]"
        Sql &= ",[gunsonu]"
        Sql &= ",[yuvarlama]"
        Sql &= ",[brut_toplam]"
        Sql &= ",[net_toplam]"
        Sql &= ",[dokum]"
        Sql &= ",[dokum_sayisi]"
        Sql &= ",[onay]"
        Sql &= ",[mac]"
        Sql &= ",[aski]"
        Sql &= ",[iptal]"
        Sql &= ",[islemyonu]"
        Sql &= ",[kdv_dahil]"
        Sql &= ",[kapali]"
        Sql &= ",[kdv_toplam]"
        Sql &= ",[indirim_yuzde_var]"
        Sql &= ",[indirim_tutar_var]"
        Sql &= ",[indirim_yuzde]"
        Sql &= ",[indirim_tutar]"
        Sql &= ",[indirim_yuzde_oran]"
        Sql &= ",[indirim_toplam]"
        Sql &= ",[indirim_satir_toplam]"
        Sql &= ",[indirim_alt_toplam]"
        Sql &= ",[indirim_alt_yuzde1]"
        Sql &= ",[indirim_alt_yuzde2]"
        Sql &= ",[indirim_alt_yuzde3]"
        Sql &= ",[createdate]"
        Sql &= ",[modifieddate])"
        Sql &= ")"
        Sql &= " Values"
        Sql &= "("
        Sql &= " @guid"
        Sql &= ",@suberef"
        Sql &= ",@sube"
        Sql &= ",@upload"
        Sql &= ",@faturano"
        Sql &= ",@belgeno"
        'Sql &= ",@gnfisturu"
        Sql &= ",@fisturu"
        Sql &= ",@sayim_turu"
        Sql &= ",@fiskod"
        Sql &= ",@sayac"
        Sql &= ",@cari"
        Sql &= ",@cari_kod"
        Sql &= ",@kullanici"
        Sql &= ",@kullanici_kod"
        Sql &= ",@kasa"
        Sql &= ",@kasa_kod"
        Sql &= ",@tarih"
        Sql &= ",@islem_tarihi"
        Sql &= ",@satir_sayisi"
        Sql &= ",@gunsonu"
        Sql &= ",@yuvarlama"
        Sql &= ",@brut_toplam"
        Sql &= ",@net_toplam"
        Sql &= ",@dokum"
        Sql &= ",@dokum_sayisi"
        Sql &= ",@onay"
        Sql &= ",@mac"
        Sql &= ",@aski"
        Sql &= ",@iptal"
        Sql &= ",@islemyonu"
        Sql &= ",@kdv_dahil"
        Sql &= ",@kapali"
        Sql &= ",@kdv_toplam"
        Sql &= ",@indirim_yuzde_var"
        Sql &= ",@indirim_tutar_var"
        Sql &= ",@indirim_yuzde"
        Sql &= ",@indirim_tutar"
        Sql &= ",@indirim_yuzde_oran"
        Sql &= ",@indirim_toplam"
        Sql &= ",@indirim_satir_toplam"
        Sql &= ",@indirim_alt_toplam"
        Sql &= ",@indirim_alt_yuzde1"
        Sql &= ",@indirim_alt_yuzde2"
        Sql &= ",@indirim_alt_yuzde3"
        Sql &= ",@createdate"
        Sql &= ",@modifieddate)"

        Ds_read("select * from hareket_baslik", hareket_satir, True, False)

        For Each dr As DataRow In Ds.Tables(hareket_baslik).Rows
            Try
                Dim cmd As New SqlCommand
                With cmd
                    .CommandText = Sql
                    .Parameters.Add(Kayit_Parametre("@guid", Chk_Null("guid", dr)))
                    .Parameters.Add(Kayit_Parametre("@suberef", Chk_Null("suberef", dr)))
                    .Parameters.Add(Kayit_Parametre("@sube", Chk_Null("sube", dr)))
                    .Parameters.Add(Kayit_Parametre("@upload", Chk_Null("upload", dr)))
                    .Parameters.Add(Kayit_Parametre("@faturano", Chk_Null("faturano", dr)))
                    .Parameters.Add(Kayit_Parametre("@belgeno", Chk_Null("belgeno", dr)))
                    '.Parameters.Add(Kayit_Parametre("@gnfisturu", Chk_Null("gnfisturu", dr)))
                    .Parameters.Add(Kayit_Parametre("@fisturu", Chk_Null("fisturu", dr)))
                    .Parameters.Add(Kayit_Parametre("@sayim_turu", Chk_Null("sayim_turu", dr)))
                    .Parameters.Add(Kayit_Parametre("@fiskod", Chk_Null("fiskod", dr)))
                    .Parameters.Add(Kayit_Parametre("@sayac", Chk_Null("sayac", dr)))
                    .Parameters.Add(Kayit_Parametre("@cari", Chk_Null("cari", dr)))
                    .Parameters.Add(Kayit_Parametre("@cari_kod", Chk_Null("cari_kod", dr)))
                    .Parameters.Add(Kayit_Parametre("@kullanici", Chk_Null("kullanici", dr)))
                    .Parameters.Add(Kayit_Parametre("@kullanici_kod", Chk_Null("kullanici_kod", dr)))
                    .Parameters.Add(Kayit_Parametre("@kasa", Chk_Null("kasa", dr)))
                    .Parameters.Add(Kayit_Parametre("@kasa_kod", Chk_Null("kasa_kod", dr)))
                    .Parameters.Add(Kayit_Parametre("@tarih", Chk_Null("tarih", dr)))
                    .Parameters.Add(Kayit_Parametre("@islem_tarihi", Chk_Null("islem_tarihi", dr)))
                    .Parameters.Add(Kayit_Parametre("@satir_sayisi", Chk_Null("satir_sayisi", dr)))
                    .Parameters.Add(Kayit_Parametre("@gunsonu", Chk_Null("gunsonu", dr)))
                    .Parameters.Add(Kayit_Parametre("@yuvarlama", Chk_Null("yuvarlama", dr)))
                    .Parameters.Add(Kayit_Parametre("@brut_toplam", Chk_Null("brut_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@net_toplam", Chk_Null("net_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@dokum", Chk_Null("dokum", dr)))
                    .Parameters.Add(Kayit_Parametre("@dokum_sayisi", Chk_Null("dokum_sayisi", dr)))
                    .Parameters.Add(Kayit_Parametre("@onay", Chk_Null("onay", dr)))
                    .Parameters.Add(Kayit_Parametre("@mac", Chk_Null("mac", dr)))
                    .Parameters.Add(Kayit_Parametre("@aski", Chk_Null("aski", dr)))
                    .Parameters.Add(Kayit_Parametre("@iptal", Chk_Null("iptal", dr)))
                    .Parameters.Add(Kayit_Parametre("@islemyonu", Chk_Null("islemyonu", dr)))
                    .Parameters.Add(Kayit_Parametre("@kdv_dahil", Chk_Null("kdv_dahil", dr)))
                    .Parameters.Add(Kayit_Parametre("@kapali", Chk_Null("kapali", dr)))
                    .Parameters.Add(Kayit_Parametre("@kdv_toplam", Chk_Null("kdv_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_yuzde_var", Chk_Null("indirim_yuzde_var", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_tutar_var", Chk_Null("indirim_tutar_var", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_yuzde", Chk_Null("indirim_yuzde", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_tutar", Chk_Null("indirim_tutar", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_yuzde_oran", Chk_Null("indirim_yuzde_oran", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_toplam", Chk_Null("indirim_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_satir_toplam", Chk_Null("indirim_satir_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_alt_toplam", Chk_Null("indirim_alt_toplam", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_alt_yuzde1", Chk_Null("@indirim_alt_yuzde1", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_alt_yuzde2", Chk_Null("indirim_alt_yuzde2", dr)))
                    .Parameters.Add(Kayit_Parametre("@indirim_alt_yuzde3", Chk_Null("indirim_alt_yuzde3", dr)))
                    .Parameters.Add(Kayit_Parametre("@createdate", Chk_Null("createdate", dr)))
                    .Parameters.Add(Kayit_Parametre("@modifieddate)", Chk_Null("modifieddate", dr)))


                    .Connection = Conn
                    .Connection.Open()
                    .ExecuteNonQuery()
                    .Connection.Close()
                End With
            Catch ex As Exception

                MsgBox(ex.Message)
            End Try

        Next








        Return True

    End Function

    Private Sub InsertToplam()

        Dim Sql As String = "insert into uruntoplam "
        Sql &= "(guid"
        Sql &= ",urunguid"
        Sql &= ",miktar"
        Sql &= ",fiyat"
        Sql &= ",tutar"
        Sql &= ",nettutar"
        Sql &= ",islemyonu"
        Sql &= ",satirguid"
        Sql &= ",baslikguid"
        Sql &= ",fisturu"
        Sql &= ",fiskod"
        '  Sql &= ",magazaguid"
        Sql &= ",islemtarihi"
        Sql &= ",iptal"
        Sql &= ",upload"
        Sql &= ",createdate"
        '  Sql &= ",modifieddate"
        Sql &= ") Values"
        Sql &= "("
        Sql &= " @guid"
        Sql &= ",@urunguid"
        Sql &= ",@miktar"
        Sql &= ",@fiyat"
        Sql &= ",@tutar"
        Sql &= ",@nettutar"
        Sql &= ",@islemyonu"
        Sql &= ",@satirguid"
        Sql &= ",@baslikguid"
        Sql &= ",@fisturu"
        Sql &= ",@fiskod"
        '   Sql &= ",@magazaguid"
        Sql &= ",@islemtarihi"
        Sql &= ",@iptal"
        Sql &= ",@upload"
        Sql &= ",@createdate"
        ' Sql &= ",@modifieddate"
        Sql &= ")"

        Ds_read("select * from hareket_satir", hareket_satir, True, False)
        Ds_read("select * from uruntoplam", Uruntoplam, True, False)
        For Each dr As DataRow In Ds.Tables(hareket_satir).Rows
            Dim satirguid As String = Chk_Null("guid", dr)
            Dim Id As String = Execute_Oku("select guid from uruntoplam where satirguid = '" & satirguid & "'", "guid", Conn, False)
            If Id = "" Then
                Dim urunguid As String = Chk_Null("urunguid", dr)
                Dim miktar As Decimal = Chk_Null("miktar", dr)
                Dim fiyat As Decimal = Chk_Null("fiyat", dr)
                Dim islemyonu As Integer = 0
                Dim tutar As Decimal = Chk_Null("brut_tutar", dr)
                Dim nettutar As Decimal = Chk_Null("net_tutar", dr)
                Dim baslikguid As String = Chk_Null("baslikguid", dr)
                Dim iptal As Boolean = Chk_Null("iptal", dr)
                Dim upload As Boolean = True

                Dim createdate As Date

                Dim fisturu As Integer = 0
                Dim fiskod As String = ""
                'Dim magazaguid As String = ""
                Dim islemtarihi As Date = Nothing

                Ds_read("select * from hareket_baslik where guid = '" & baslikguid & "'", hareket_baslik, True, True)
                For Each drB As DataRow In Ds.Tables(hareket_baslik).Rows
                    fisturu = Chk_Null("fisturu", drB)
                    fiskod = Chk_Null("fiskod", drB)
                    'magazaguid = Chk_Null("magazaguid", drB)
                    islemtarihi = Chk_Null("tarih", drB)
                    createdate = Chk_Null("tarih", drB)
                    islemyonu = Chk_Null("islemyonu", drB)
                Next

                Try
                    Dim cmd As New SqlCommand
                    With cmd
                        .CommandText = Sql
                        .Parameters.Add(Kayit_Parametre("@guid", Guid.NewGuid.ToString))
                        .Parameters.Add(Kayit_Parametre("@urunguid", urunguid))
                        .Parameters.Add(Kayit_Parametre("@miktar", miktar))
                        .Parameters.Add(Kayit_Parametre("@fiyat", fiyat))
                        .Parameters.Add(Kayit_Parametre("@tutar", tutar))
                        .Parameters.Add(Kayit_Parametre("@nettutar", nettutar))
                        .Parameters.Add(Kayit_Parametre("@islemyonu", islemyonu))
                        .Parameters.Add(Kayit_Parametre("@satirguid", satirguid))
                        .Parameters.Add(Kayit_Parametre("@baslikguid", baslikguid))
                        .Parameters.Add(Kayit_Parametre("@fisturu", fisturu))
                        .Parameters.Add(Kayit_Parametre("@fiskod", fiskod))
                        '  .Parameters.Add(Kayit_Parametre("@magazaguid", magazaguid))
                        .Parameters.Add(Kayit_Parametre("@islemtarihi", islemtarihi))
                        .Parameters.Add(Kayit_Parametre("@iptal", iptal))
                        .Parameters.Add(Kayit_Parametre("@upload", upload))
                        .Parameters.Add(Kayit_Parametre("@createdate", createdate))

                        .Connection = Conn
                        .Connection.Open()
                        .ExecuteNonQuery()
                        .Connection.Close()
                    End With
                Catch ex As Exception

                    MsgBox(ex.Message)
                End Try
            End If

        Next

    End Sub

End Class
