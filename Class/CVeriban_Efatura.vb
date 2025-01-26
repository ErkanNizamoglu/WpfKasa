Imports System.IO
Imports System.ServiceModel
Imports System.Xml
Imports Ionic.Zip

Public Class CVeriban_Efatura

    Public Shared sessionID = String.Empty

    Public Shared Sub LOGIN_TEST()
        Dim client As EfaturaVeribanServiceReference.TransferDocumentServiceClient = New EfaturaVeribanServiceReference.TransferDocumentServiceClient()
        client.Login(pd.ekullanici, pd.esifre, sessionID)
    End Sub

    Public Function BAGLANTI_TESTI() As Boolean
        Try
            Dim client As EfaturaVeribanServiceReference.TransferDocumentServiceClient = New EfaturaVeribanServiceReference.TransferDocumentServiceClient()
            Dim connectionTestResult As Boolean = client.ConnectionTest()
            Return connectionTestResult
        Catch ex As Exception
        End Try
        Return False
    End Function




    Friend Function MUKELLEF_ISIM_BILGISI(ByVal Vkn_Tckn As String) As String
        Try
            If Vkn_Tckn = "" Then
                'Msg("Vergi Numarası Boş", False, False, True)
                Return ""
            End If
            'reg.ekullanici = "SIMARIKAVM@SIMARIKAVM"
            'reg.esifre = "Zeynel1980"
            Dim client As EfaturaVeribanServiceReference.TransferDocumentServiceClient = New EfaturaVeribanServiceReference.TransferDocumentServiceClient()
            BAGLANTI_TESTI()
            If pd.ekullanici = "" Or pd.esifre = "" Then Return ""
            If client.Login(sessionID, pd.ekullanici, pd.esifre) Then
                Dim customers As EfaturaVeribanServiceReference.CustomerData() = client.CheckIsThereCustomer(Vkn_Tckn, sessionID)
                If customers IsNot Nothing Then
                    For Each item In customers
                        Return item.Title
                    Next
                    Return ""
                Else
                    Return ""
                End If
            Else
                'Msg("Bağlantı Kurulamadı", False, False, True)
            End If
        Catch ex As Exception
        End Try
        Return ""
    End Function

    Friend Function MUKELLEF_ETIKET_BILGISI(ByVal Vkn_Tckn As String) As String
        Try
            If Vkn_Tckn = "" Then
                'Msg("Vergi Numarası Boş", False, False, True)
                Return ""
            End If
            If pd.ekullanici = "" Or pd.esifre = "" Then Return ""
            'reg.ekullanici = "SIMARIKAVM@SIMARIKAVM"
            'reg.esifre = "Zeynel1980"
            Dim client As EfaturaVeribanServiceReference.TransferDocumentServiceClient = New EfaturaVeribanServiceReference.TransferDocumentServiceClient()
            If client.Login(sessionID, pd.ekullanici, pd.esifre) Then
                Dim customers As EfaturaVeribanServiceReference.CustomerData() = client.CheckIsThereCustomer(Vkn_Tckn, sessionID)
                If customers IsNot Nothing Then
                    For Each item In customers
                        Return item.Alias
                    Next
                    Return ""
                Else
                    Return ""
                End If
            Else
                'Msg("Bağlantı Kurulamadı", False, False, True)
            End If
        Catch ex As Exception
        End Try
        Return ""
    End Function

    Friend Function MUKELLEF_GIB_MUKELLEF_LISTESINDE_VARMI(ByVal Vkn_Tckn As String) As KayitDurumu
        Try
            If Vkn_Tckn = "" Then
                'Msg("Vergi Numarası Boş", False, False, True)
                Return KayitDurumu.Belirsiz
            End If
            If pd.ekullanici = "" Or pd.esifre = "" Then Return ""
            'reg.ekullanici = "SIMARIKAVM@SIMARIKAVM"
            'reg.esifre = "Zeynel1980"
            'Dim client As EfaturaVeribanServiceReference.TransferServiceClient = New EfaturaVeribanServiceReference.TransferServiceClient()
            Dim client As EfaturaVeribanServiceReference.TransferDocumentServiceClient = New EfaturaVeribanServiceReference.TransferDocumentServiceClient()
            If client.Login(sessionID, pd.ekullanici, pd.esifre) Then
                Dim customers As EfaturaVeribanServiceReference.CustomerData() = client.CheckIsThereCustomer(Vkn_Tckn, sessionID)
                If customers IsNot Nothing Then
                    For Each item In customers
                        Return KayitDurumu.Evet
                    Next
                    Return KayitDurumu.Hayir
                Else
                    Return KayitDurumu.Hayir
                End If
            Else
                'Msg("Bağlantı Kurulamadı", False, False, True)
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return KayitDurumu.Belirsiz
    End Function




End Class
