Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection

Public Class C_ChkDataLocalDb


    Function Sql_conn_localDb() As SqlConnection
        'Dim DataDir As String = System.AppDomain.CurrentDomain.BaseDirectory & "Data"

        Dim LocalDbDataName As String = "PerakendeLdb"
        Dim DataFile As String = DataDir & "\" & LocalDbDataName & ".mdf"
        Return New SqlConnection("Data Source=" & Reg.Server & ";AttachDbFileName=" & DataFile & ";Connect Timeout=320")
    End Function


    Function Create_Database_LocalDb() As Boolean
        Dim SqlConn As SqlConnection = New SqlConnection("Data Source=" & Reg.Server)
        'Dim DataDir As String = System.AppDomain.CurrentDomain.BaseDirectory & "Data"
        If System.IO.Directory.Exists(DataDir) = False Then
            System.IO.Directory.CreateDirectory(DataDir)
        End If
        Dim LocalDbDataName As String = "PerakendeLdb"
        Dim DataFile As String = DataDir & "\" & LocalDbDataName & ".mdf"
        Dim LogFile As String = DataDir & "\" & LocalDbDataName & ".ldf"
        If System.IO.File.Exists(DataFile) Then Return True
        Dim sql As String = Nothing
        sql &= "CREATE DATABASE [" & LocalDbDataName & "] "
        sql &= "CONTAINMENT = NONE "
        sql &= "ON  PRIMARY "
        sql &= "( NAME = N'" & LocalDbDataName & "', FILENAME = N'" & DataFile & "' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) "
        sql &= "LOG ON "
        sql &= "( NAME = N'" & LocalDbDataName & "_log', FILENAME = N'" & LogFile & "' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) "
        sql &= " COLLATE Turkish_CI_AS;"
        If Execute_run_no_transaction(sql, SqlConn) = True Then
            Return True

        End If
        Return False
    End Function



    Private Function Alter_Database() As Boolean
        Dim sql As String = Nothing
        sql &= "ALTER DATABASE " & LocalDbDataName & " COLLATE Turkish_CI_AS"
        Return Execute_run_no_transaction(sql, Conn)
    End Function

    Function Chk_Data_Localdb() As Boolean
        Conn = Sql_conn_localDb()
        If Create_Database_LocalDb() Then
            Try
                Conn.Open()
                Conn.Close()
            Catch ex As Exception
                Return False
            End Try
        Else
            Return False
        End If
        Return True
    End Function



    Sub DropDatabase()


        Dim batFileName As String = Guid.NewGuid().ToString & ".bat"

        Using batFile As StreamWriter = New StreamWriter(batFileName)
            batFile.WriteLine($"c:")
            batFile.WriteLine($"cd\windows\system32")
            batFile.WriteLine($"sqllocaldb p MSSQLLocalDB")
            batFile.WriteLine($"sqllocaldb d MSSQLLocalDB")
        End Using

        Dim ProcessStartInfo As ProcessStartInfo = New ProcessStartInfo("cmd.exe", "/c " + batFileName)
        ProcessStartInfo.RedirectStandardInput = True
        ProcessStartInfo.UseShellExecute = False
        ProcessStartInfo.CreateNoWindow = True
        ProcessStartInfo.WindowStyle = ProcessWindowStyle.Normal

        Dim p As Process = New Process()
        p.StartInfo = ProcessStartInfo

        Try
            p.Start()
            p.WaitForExit()
        Catch ex As Exception
            Msg(ex.Message, False, False, True)
        End Try
        Try
            File.Delete(My.Application.Info.DirectoryPath & "\data\PerakendeLdb.ldf")
            File.Delete(My.Application.Info.DirectoryPath & "\data\PerakendeLdb.mdf")
        Catch ex As Exception
            Msg(ex.Message, False, False, True)
        End Try
        Process.GetCurrentProcess.Kill()
    End Sub
End Class
