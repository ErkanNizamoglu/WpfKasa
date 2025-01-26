Imports System.Collections
Imports System.Data
Imports System.Data.SQLite
Imports System.Globalization
Imports System.Reflection

Public Class C_ChkDataSqliteDb

    Function Bulk_UpdateSqlite(TblRemote As DataTable, Conn As SQLiteConnection, mesaj As Boolean) As Boolean
        ' Conn Local Oluyor
        Try
            Dim Trn As SQLiteTransaction = Conn.BeginTransaction
            Dim Sql As String = ""

            Dim Cmd As New SQLiteCommand(Sql, Conn, Trn)

            Using Cmd

                Dim Prm(5) As SQLiteParameter



                Dim sayac As Integer = 0
                For Each Dc As DataColumn In TblRemote.Columns
                    Prm(sayac) = New SQLiteParameter("@" & Dc.ColumnName)
                Next


            End Using

            Dim dt As DataTable = TblRemote
            '  Dim cmd As SQLiteCommand = New SQLiteCommand("Update_" & TblRemote.TableName, Conn)
            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.Connection = Conn
            Cmd.Parameters.AddWithValue("@tbl" & TblRemote.TableName, dt)
            If Cmd.Connection.State = ConnectionState.Closed Then Cmd.Connection.Open()
            If Cmd.Connection.State = ConnectionState.Open Then
                Cmd.ExecuteNonQuery()
                Cmd.Connection.Close()
            End If
        Catch ex As Exception
            If mesaj Then MsgBox(TblRemote.TableName & "   " & ex.Message)
            '   MsgBox(ex.Message)
            '   MsgBox(TblRemote.TableName)
            Return False
        End Try
        Return True
    End Function
    Function SqliteDbTypeAl(DbTypeStr As String) As String
        Select Case DbTypeStr
            Case "System.Guid"
                Return "TEXT"
            Case "System.String"
                Return "TEXT"
            Case "System.Decimal"
                Return "REAL"
            Case "System.Int64"
                Return "INTEGER"
            Case "System.Int16"
                Return "INTEGER"
            Case "System.Int32"
                Return "INTEGER"
            Case "System.Byte[]"
                Return "BLOB"
            Case "System.Byte"
                Return "INTEGER"
            Case "System.Windows.Controls.Image"
                Return "BLOB"
            Case "System.Boolean"
                Return "NUMERIC"
            Case "Date"
                Return "NUMERIC"
            Case "System.DateTime"
                Return "NUMERIC"
            Case Else
                MsgBox("MSsql DbType Yok")
        End Select
        Return ""
    End Function

    Function Ds_read(sql As String,
                     table As String,
                     Conn As SQLiteConnection,
                     Sil As Boolean,
                     mesaj As Boolean) As Boolean

        If Sil Then
            Try
                Ds.Tables.Remove(table)
            Catch ex As Exception
            End Try
        End If
        Ds.Locale = CultureInfo.InvariantCulture
        Dim cmd As New SQLiteCommand(sql, Conn)
        Dim da As New SQLiteDataAdapter
        da.SelectCommand = cmd
        Try
            da.Fill(Ds, table)
        Catch ex As Exception
            If mesaj Then MsgBox(ex.Message)
            Return False
        End Try
        Return True
    End Function

    Private Function SqliteCreateTable(tablo As String) As Boolean
        Dim myFieldInfo() As FieldInfo
        Dim myType As Type = Tablo_mtype(tablo)
        myFieldInfo = myType.GetFields
        Dim sql_degisken As New ArrayList
        Dim Sql As String = ""
        '[IF NOT EXISTS] [schema_name].
        Sql &= "CREATE Table IF NOT EXISTS " & tablo & " ("
        For i As Integer = 0 To myFieldInfo.Length - 1
            Dim kolon As String = myFieldInfo(i).Name
            Dim type As Type = myFieldInfo(i).FieldType
            Dim SqliteDbType As String = SqliteDbTypeAl(type.ToString)
            Select Case kolon
                Case "ref"
                    sql_degisken.Add("ref Integer Not NULL PRIMARY KEY AUTOINCREMENT" & vbCrLf)
                Case Else
                    sql_degisken.Add(kolon & " " & SqliteDbType & vbCrLf)
            End Select
        Next
        Sql &= String.Join(",", TryCast(sql_degisken.ToArray(GetType(String)), String()))
        Sql &= ");"
        If ExecuteSqliteCommand(SqliteConn, Sql) Then Return True
        Return False
    End Function

    Dim SqliteConn As New SQLiteConnection

    Private Function CreateSqliteConnection() As SQLiteConnection
        Dim FileFullPath As String = AppDomain.CurrentDomain.BaseDirectory & "\Data\Perakende.sqlite"
        SqliteConn = New SQLiteConnection("Data Source=" & FileFullPath & ";Version=3;New=True;Compress=True;")
        Try
            SqliteConn.Open()
        Catch ex As Exception
        End Try
        Return SqliteConn
    End Function

    Private Function ExecuteSqliteCommand(Conn As SQLiteConnection, Sql As String) As Boolean
        Dim SqliteCmd As New SQLiteCommand(Sql, Conn)
        Try
            SqliteCmd.ExecuteNonQuery()
            'SqliteCmd.Connection.Close()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Function ChkTableTablolar() As Boolean
        On Error GoTo 1
        Dim sqltablo As String = "Select name As TABLE_NAME FROM sqlite_schema WHERE Type ='table' AND name Not Like 'sqlite_%';"
        Ds_read(sqltablo, "tablolar", CreateSqliteConnection, True, False)
        Dim tablo As String
        Dim myType_genel As Type = GetType(tablo_degisken)
        Dim myFieldInfo_genel As FieldInfo() = myType_genel.GetFields

        For j As Integer = 0 To myFieldInfo_genel.Length - 1
            Dim Ok As Boolean = False
            Dim TabloOk As Boolean = False
            tablo = myFieldInfo_genel(j).Name
            For Each DrTablo As DataRow In Ds.Tables("tablolar").Select("TABLE_NAME = '" & tablo & "'")
                TabloOk = True
                Dim sqlkolon As String = "SELECT name as COLUMN_NAME,type,[notnull] FROM PRAGMA_TABLE_INFO('" & tablo & "')"
                Dim DsKolonlar As New DataSet
                Ds_read(sqlkolon, "kolonlar", CreateSqliteConnection, True, False)
                Dim myFieldInfo() As FieldInfo
                Dim myType As Type = Tablo_mtype(tablo)
                myFieldInfo = myType.GetFields
                For i As Integer = 0 To myFieldInfo.Length - 1
                    Dim kolon As String = myFieldInfo(i).Name
                    Dim type As Type = myFieldInfo(i).FieldType
                    For Each DrColon As DataRow In Ds.Tables("kolonlar").Select("COLUMN_NAME = '" & kolon & "'")
                        Dim SqlType As String = SqlDbTypeAl(type.ToString).ToString
                        Dim VbType As String
                        Select Case SqlType
                            Case "BigInt"
                                VbType = "bigint"
                            Case "Int"
                                VbType = "bigint"
                            Case "UniqueIdentifier"
                                VbType = "uniqueidentifier"
                            Case "Binary"
                                VbType = "varbinary"
                            Case "Image"
                                VbType = "varbinary"
                            Case "TinyInt"
                                VbType = "tinyint"
                            Case Else
                                VbType = SqlType.ToLower
                        End Select
                        Dim Position As Int64 = Chk_Null("ORDINAL_POSITION", DrColon)
                        Dim DataType As String = Chk_Null("DATA_TYPE", DrColon).ToString.ToLower
                        If Position = i + 1 And DataType = VbType Then
                            Ok = True
                        Else
                            Ok = False
                            Exit For
                        End If
                    Next
                    If Ok = False Then Exit For
                Next
                If Ok = False Then Exit For
            Next
            If TabloOk = False Then
                SqliteCreateTable(tablo)

            End If
        Next
        If SqliteConn.State = ConnectionState.Open Then SqliteConn.Close()
        Return True
1:
        Return False
    End Function



End Class
