Imports Ionic.Zip
Imports System
Imports System.IO
Imports System.ServiceModel
Imports WpfKasa.EArsivfaturaVeribanServiceReference

Public Class CVeriban_EarsivFatura
    Public Shared sessionCode As String = Nothing

    Function LOGIN() As Boolean
        'reg.ekullanici = "SIMARIKAVM@SIMARIKAVM"
        'reg.esifre = "123456"
        'Reg.esifre = "Zeynel1980"

        Using Client As IntegrationServiceClient = New IntegrationServiceClient()
            Try
                sessionCode = Client.Login(pd.ekullanici, pd.esifre)
                Return True
            Catch timeProblem As TimeoutException
                'Console.WriteLine(timeProblem.Message)
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                'Console.WriteLine(veribanFault.Detail.FaultCode)
                'Console.WriteLine(veribanFault.Detail.FaultDescription)
            Catch commProblem As CommunicationException
                'Console.WriteLine(commProblem.Message)
            Catch unknownEx As Exception
                'Console.WriteLine(unknownEx.Message)
            End Try
        End Using
        Return False
    End Function

    Enum Hata
        basarili = 0
        timeProblem = 1
        veribanFault = 2
        commProblem = 3
        unknownEx = 4
    End Enum

    Function VKNTCKN_EFATURA_MUKELLEFIMI(ByVal registerNumber As String) As Hata
        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim operationResult As EArsivfaturaVeribanServiceReference.OperationResult = Nothing
            Try
                LOGIN()
                'sessionCode = serviceClient.Login(reg.ekullanici, reg.esifre)
                operationResult = serviceClient.CheckRegisterNumberIsEInvoiceCustomer(sessionCode, registerNumber)
                Dim Str As String = operationResult.Description
                Console.WriteLine(Str)
            Catch timeProblem As TimeoutException
                operationResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
                Return Hata.timeProblem
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                operationResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
                Return Hata.veribanFault
            Catch commProblem As CommunicationException
                operationResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
                Return Hata.commProblem
            Catch unknownEx As Exception
                operationResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("UnknownException: {0}", unknownEx.Message)
                    }

                Return Hata.unknownEx
            End Try
            Return Hata.basarili
        End Using
        Return Hata.unknownEx
    End Function








    Sub TRANSFER_SORGULAMA_TEST(ByVal transferFileUniqueId As String)

        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim transferQueryResult As EArsivfaturaVeribanServiceReference.TransferQueryResult = Nothing
            Try
                'LOGIN()
                sessionCode = serviceClient.Login(pd.ekullanici, pd.esifre)
                transferQueryResult = serviceClient.GetTransferFileStatus(sessionCode, transferFileUniqueId)
            Catch timeProblem As TimeoutException
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If transferQueryResult.StateCode <> -1 Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("!!! SORGULAMA SONUCU !!!")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! SORGULAMA BAŞARISIZ !!!")
            End If

            Console.ResetColor()
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)
            Console.WriteLine("StateCode : {0}", transferQueryResult.StateCode)
            Console.WriteLine("StateName : {0}", transferQueryResult.StateName)
            Console.WriteLine("StateDescription : {0}", transferQueryResult.StateDescription)
        End Using

        Console.ReadLine()
    End Sub

    Sub TRANSFER_OKC_DOCUMENT_SORGULAMA_TEST(ByVal transferFileUniqueId As String)
        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim transferQueryResult As EArsivfaturaVeribanServiceReference.TransferQueryResult = Nothing

            Try
                'transferFileUniqueId = "17F648FA-F54A-4A8B-9FED-B89ECD3F6627" 'Guid.NewGuid().ToString().ToUpper();
                LOGIN()

                'sessionCode = serviceClient.Login(reg.ekullanici, reg.esifre)
                transferQueryResult = serviceClient.GetTransferOkcDocumentFileStatus(sessionCode, transferFileUniqueId)
            Catch timeProblem As TimeoutException
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                transferQueryResult = New EArsivfaturaVeribanServiceReference.TransferQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If transferQueryResult.StateCode <> -1 Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("!!! SORGULAMA SONUCU !!!")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! SORGULAMA BAŞARISIZ !!!")
            End If

            Console.ResetColor()
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)
            Console.WriteLine("StateCode : {0}", transferQueryResult.StateCode)
            Console.WriteLine("StateName : {0}", transferQueryResult.StateName)
            Console.WriteLine("StateDescription : {0}", transferQueryResult.StateDescription)
        End Using

        Console.ReadLine()
    End Sub

    Sub VKNTCKN_BAZINDA_TARIH_ARALIKLI_FATURA_ETTN_LISTESI(customerRegisterNumber As String)
        Using serviceClient As New EArsivfaturaVeribanServiceReference.IntegrationServiceClient
            Dim salesInvoiceUUIDList As String() = Nothing
            Try
                'customerRegisterNumber = "15556043852"
                Dim dtStartIssueTime As Date = New DateTime(2024, 2, 1)
                Dim dtEndIssueTime As Date = New DateTime(2024, 2, 10)


                sessionCode = serviceClient.Login(pd.ekullanici, pd.esifre)
                salesInvoiceUUIDList =
                    serviceClient.GetSalesInvoiceUUIDListWithCustomerRegisterNumber(sessionCode, customerRegisterNumber, dtStartIssueTime, dtEndIssueTime)
            Catch timeProblem As TimeoutException
                Console.WriteLine(String.Format("TimeoutException: {0}", timeProblem.Message))
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                Console.WriteLine(String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription))
            Catch commProblem As CommunicationException
                Console.WriteLine(String.Format("CommunicationException: {0}", commProblem.Message))
            Catch unknownEx As Exception
                Console.WriteLine(String.Format("UnknownException: {0}", unknownEx.Message))
            End Try

            If salesInvoiceUUIDList IsNot Nothing AndAlso salesInvoiceUUIDList.Length > 0 Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("!!! FATURA ETTN LISTESI !!!")
                Console.ResetColor()

                For Each item In salesInvoiceUUIDList
                    Console.WriteLine("FATURA ETTN : {0}", item)
                Next
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! BU KRITERLERE UYGUN FATURA BULUNAMADI !!!")
            End If
        End Using

        Console.ReadLine()
    End Sub

    Function FATURA_SORGULAMA(ByVal invoiceNumber As String) As String

        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim invoiceQueryResult As New EArsivfaturaVeribanServiceReference.EArchiveInvoiceQueryResult

            Try
                pd.ekullanici = "SIMARIKAVM@SIMARIKAVM"
                'reg.esifre = "123456"
                pd.esifre = "Zeynel1980"

                sessionCode = serviceClient.Login(pd.ekullanici, pd.esifre)
                invoiceQueryResult = serviceClient.GetSalesInvoiceStatusWithInvoiceNumber(sessionCode, invoiceNumber)
            Catch timeProblem As TimeoutException
                invoiceQueryResult = New EArsivfaturaVeribanServiceReference.EArchiveInvoiceQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                invoiceQueryResult = New EArsivfaturaVeribanServiceReference.EArchiveInvoiceQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                invoiceQueryResult = New EArsivfaturaVeribanServiceReference.EArchiveInvoiceQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                invoiceQueryResult = New EArsivfaturaVeribanServiceReference.EArchiveInvoiceQueryResult() With {
                        .StateCode = -1,
                        .StateName = String.Empty,
                        .StateDescription = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If invoiceQueryResult.StateCode <> -1 Then
                'Console.ForegroundColor = ConsoleColor.Yellow
                'Console.WriteLine("!!! SORGULAMA SONUCU !!!")
            Else
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("!!! SORGULAMA BAŞARISIZ !!!")
            End If
            'MsgBox(invoiceQueryResult.StateCode)
            'MsgBox(invoiceQueryResult.StateName)
            Return invoiceQueryResult.StateCode.ToString



            Console.WriteLine("StateCode : {0}", invoiceQueryResult.StateCode)
            Console.WriteLine("StateName : {0}", invoiceQueryResult.StateName)
            Console.WriteLine("StateDescription : {0}", invoiceQueryResult.StateDescription)
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)
            Console.WriteLine("ReportStateCode : {0}", invoiceQueryResult.GIBReportStateCode)
            Console.WriteLine("ReportStateName : {0}", invoiceQueryResult.GIBReportStateName)
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)
            Console.WriteLine("MailStateCode : {0}", invoiceQueryResult.MailStateCode)
            Console.WriteLine("MailStateName : {0}", invoiceQueryResult.MailStateName)
        End Using

        Console.ReadLine()
    End Function



    Private Shared Sub FATURA_IPTAL_TEST()
        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim cancelResult As EArsivfaturaVeribanServiceReference.OperationResult = Nothing

            Try
                Dim invoiceNumber = "TST2017000000111"
                sessionCode = serviceClient.Login(pd.ekullanici, pd.esifre)
                cancelResult = serviceClient.CancelSalesInvoiceWithInvoiceNumber(sessionCode, Date.Now, invoiceNumber)
            Catch timeProblem As TimeoutException
                cancelResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                cancelResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                cancelResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                cancelResult = New EArsivfaturaVeribanServiceReference.OperationResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If cancelResult.OperationCompleted = True Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("!!! SORGULAMA SONUCU !!!")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! SORGULAMA BAŞARISIZ !!!")
            End If

            Console.ResetColor()
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)
            Console.WriteLine("Description : {0}", cancelResult.Description)
        End Using

        Console.ReadLine()
    End Sub



    Sub FATURA_DOWNLOAD_TEST()
        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim downloadResult As EArsivfaturaVeribanServiceReference.DownloadResult = Nothing

            Try
                Dim invoiceNumber = "OK22024000017341"
                'sb.ekullanici = "SIMARIKAVM@SIMARIKAVM"
                'reg.esifre = "123456"
                'sb.esifre = "Zeynel1980"
                sessionCode = serviceClient.Login(sb.ekullanici, sb.esifre)
                downloadResult = serviceClient.DownloadSalesInvoiceWithInvoiceNumber(sessionCode, EArsivfaturaVeribanServiceReference.DownloadDocumentDataTypes.XML_INZIP, invoiceNumber)
            Catch timeProblem As TimeoutException
                downloadResult = New EArsivfaturaVeribanServiceReference.DownloadResult() With {
                        .DownloadFileReady = False,
                        .DownloadDescription = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                downloadResult = New EArsivfaturaVeribanServiceReference.DownloadResult() With {
                        .DownloadFileReady = False,
                        .DownloadDescription = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                downloadResult = New EArsivfaturaVeribanServiceReference.DownloadResult() With {
                        .DownloadFileReady = False,
                        .DownloadDescription = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                downloadResult = New EArsivfaturaVeribanServiceReference.DownloadResult() With {
                        .DownloadFileReady = False,
                        .DownloadDescription = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If downloadResult.DownloadFileReady Then
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine("!!! INDIRME SONUCU !!!")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! INDIRME BAŞARISIZ !!!")
            End If

            Console.ResetColor()
            Console.WriteLine(Environment.NewLine & "========================" & Environment.NewLine)

            If downloadResult.DownloadFileReady Then
                Dim fileName = downloadResult.DownloadFile.FileName & downloadResult.DownloadFile.FileExtension
                File.WriteAllBytes("C:\" & fileName, downloadResult.DownloadFile.FileData)
                Console.WriteLine(fileName & " dosyası indirildi.")
            End If
        End Using

        Console.ReadLine()
    End Sub

    Private Shared Sub TRANSFER_EARSIV_YENI_NESIL_ENTEGRASYON_KODU_TEST()
        Using serviceClient As EArsivfaturaVeribanServiceReference.IntegrationServiceClient = New EArsivfaturaVeribanServiceReference.IntegrationServiceClient()
            Dim transferResult As EArsivfaturaVeribanServiceReference.TransferResult = Nothing

            Try
                Dim fileFullPath = "YM_RAPOR_DATA_1.xml"
                Dim uniqueIntegrationCode = "kendiUniqueIdniz"

                'Gönderilecek dosya ZipBinaryArray'e dönüştürülür.
                Dim zipFileBinaryDataArray As Byte() = Nothing

                Using memoryStreamOutput As MemoryStream = New MemoryStream()

                    Using zip As ZipFile = New ZipFile()
                        zip.AddFile(fileFullPath, String.Empty)
                        zip.Save(memoryStreamOutput)
                    End Using

                    zipFileBinaryDataArray = memoryStreamOutput.ToArray()
                End Using


                'Zip Binary Data Array'in Standart MD5 Hash bilgisi hesaplanır.
                Dim hashGenerator As [Global].Test.EArchive.ConsoleTestCustomer.HashGenerator = New [Global].Test.EArchive.ConsoleTestCustomer.HashGenerator()
                Dim zipFileHash = hashGenerator.GetMD5Hash(zipFileBinaryDataArray)
                Dim transferFile As EArsivfaturaVeribanServiceReference.EArchiveOkcDocumentFile = New EArsivfaturaVeribanServiceReference.EArchiveOkcDocumentFile With {
                        .FileNameWithExtension = Path.GetFileNameWithoutExtension(fileFullPath) & ".zip",    'Transfer edilecek dosya adı, dosya uzantısı .zip olmalıdır.
.FileDataType = EArsivfaturaVeribanServiceReference.TransferDocumentDataTypes.XML_INZIP,           'ZIP dosyası içerisindeki dosya formatı XML.
.BinaryData = zipFileBinaryDataArray,                                                'ZIP dosyası Binary64 Data
.BinaryDataHash = zipFileHash                                                       'ZIP dosyası Binary64 Data MD5 Hash değeri
}
                sessionCode = serviceClient.Login(pd.ekullanici, pd.esifre)
                'transferResult = serviceClient.TransferOkcInvoiceFileWithIntegrationCode(sessionCode, transferFile, uniqueIntegrationCode)
                transferResult = serviceClient.TransferOkcInvoiceFileWithIntegrationCode(sessionCode, Nothing, uniqueIntegrationCode)
            Catch timeProblem As TimeoutException
                transferResult = New EArsivfaturaVeribanServiceReference.TransferResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("TimeoutException: {0}", timeProblem.Message)
                    }
            Catch veribanFault As FaultException(Of EArsivfaturaVeribanServiceReference.VeribanServiceFault)
                transferResult = New EArsivfaturaVeribanServiceReference.TransferResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("Code:[{0}], Message: {1}", veribanFault.Detail.FaultCode, veribanFault.Detail.FaultDescription)
                    }
            Catch commProblem As CommunicationException
                transferResult = New EArsivfaturaVeribanServiceReference.TransferResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("CommunicationException: {0}", commProblem.Message)
                    }
            Catch unknownEx As Exception
                transferResult = New EArsivfaturaVeribanServiceReference.TransferResult() With {
                        .OperationCompleted = False,
                        .Description = String.Format("UnknownException: {0}", unknownEx.Message)
                    }
            End Try

            If transferResult.OperationCompleted Then
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine("!!! TRANSFER BAŞARILI !!!")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.WriteLine(Environment.NewLine & "TRANSFER DÖKÜMAN NUMARASI [ " & transferResult.TransferFileUniqueId & " ]")
            Else
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("!!! TRANSFER BAŞARISIZ !!!")
            End If

            Console.ResetColor()
            Console.WriteLine(Environment.NewLine & transferResult.Description)
        End Using

        Console.ReadLine()
    End Sub

End Class
