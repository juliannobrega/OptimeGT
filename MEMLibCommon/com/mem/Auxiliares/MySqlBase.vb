Imports System.IO
Imports MySql.Data.MySqlClient

Public Class MySqlBase

    Public Const ConectionString1 As String = "server={0};Database={1};uid={2};pwd={3};default command timeout=3000"
    Public Const ConectionString2 As String = "server={0};Database={1};uid={2};default command timeout=3000"
    Public Const ConectionString3 As String = "server={0};uid={1};pwd={2};default command timeout=3000"
    Public Const ConectionString4 As String = "server={0};uid={1};default command timeout=3000"

    Public Shared ServerName As String 'cadena de connecion a la base de datos
    Public Shared DatabaseName As String 'cadena de connecion a la base de datos

    Public Shared Function GetConectionString(server As String, database As String, user As String, pass As String) As String
        ServerName = server
        DatabaseName = database
        If (String.IsNullOrWhiteSpace(database)) Then
            If (String.IsNullOrWhiteSpace(pass)) Then
                GetConectionString = String.Format(ConectionString4, server, user)
            Else
                GetConectionString = String.Format(ConectionString3, server, user, pass)
            End If
        Else
            If (String.IsNullOrWhiteSpace(pass)) Then
                GetConectionString = String.Format(ConectionString2, server, database, user)
            Else
                GetConectionString = String.Format(ConectionString1, server, database, user, pass)
            End If
        End If
    End Function

    Public Shared connString As String 'cadena de connecion a la base de datos

    Private conn As New MySql.Data.MySqlClient.MySqlConnection

    'Public Event ErrorAlActualizarDatos(mensajeError As String)

    'Event InicioConsulta(nombreTabla As String)
    'Event FinConsulta(nombreTabla As String, tabla As DataTable)

    'Testea la coneccion con la base de datos segun la configuración de connstring
    'devuelve true si la coneccion es establecida con exito
    Public Function TestDB() As Boolean
        'test para saber si la coneccion anda o no
        Dim conn As New MySql.Data.MySqlClient.MySqlConnection
        Try
            conn.ConnectionString = connString
            conn.Open()
            conn.Close()
            Return True
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Return False
        End Try
    End Function

    'llega un MySqlDataReader con la informacion obtenida de la base a partir de la consultaSQL enviada
    Public Function FillReader(ByVal consultaSQL As String) As MySqlDataReader
        Dim conn As New MySqlConnection
        Dim cmd As New MySqlCommand
        Dim myReader As MySqlDataReader
        Try
            conn.ConnectionString = connString
            conn.Open()
            'ejecuta una consulta 
            cmd.Connection = conn
            cmd.CommandText = consultaSQL
            myReader = cmd.ExecuteReader
            Return myReader
            myReader.Close()
            conn.Close()
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Throw ex
            Return Nothing
        End Try
    End Function

    Public Sub FillAdapterMultiple(ByRef myDataSet As DataSet, ByVal nombreTabla As String, ByVal consultaSQL As String)
        Dim conn As New MySqlConnection


        Try
            'RaiseEvent InicioConsulta(nombreTabla)

            conn.ConnectionString = connString
            conn.Open()

            Dim myAdapter As New MySqlDataAdapter(consultaSQL, conn)
            myAdapter.Fill(myDataSet, nombreTabla)

            conn.Close()

            'RaiseEvent FinConsulta(nombreTabla, myDataSet.Tables(nombreTabla))

        Catch ex As MySql.Data.MySqlClient.MySqlException

            Throw ex

        End Try
    End Sub

    Public Function FillAdapterMultiple(ByVal consultaSQL As String) As DataTable
        Dim conn As New MySqlConnection
        Dim result As New DataTable
        Try

            conn.ConnectionString = connString
            conn.Open()

            Dim myAdapter As New MySqlDataAdapter(consultaSQL, conn)
            myAdapter.Fill(result)

            conn.Close()

        Catch ex As MySql.Data.MySqlClient.MySqlException

        End Try
        FillAdapterMultiple = result
    End Function

    Public Function ExecuteScalar(ByVal consultaSQL As String, Optional ByVal showErrors As Boolean = True) As Object
        Dim conn As New MySqlConnection
        Dim result As New Object
        Try
            'Dim myAdapter As New MySqlDataAdapter(consultaSQL, conn)
            'ejecuta una consulta escalar
            Dim myCommand As New MySqlCommand(consultaSQL) With {
                .Connection = conn,
                .CommandTimeout = 3000
            }
            conn.ConnectionString = connString
            conn.Open()

            result = myCommand.ExecuteScalar()

            conn.Close()
        Catch ex As MySql.Data.MySqlClient.MySqlException

            If showErrors Then
                Throw ex
            Else
                result = ex.Message
            End If

        End Try
        Return result
    End Function

    'llega un DataSet con la informacion obtenida de la base a partir de la consultaSQL enviada
    Public Function FillAdapter(ByVal consultaSQL As String) As DataSet
        Dim conn As New MySqlConnection
        Dim myDataset As New DataSet
        Try
            conn.ConnectionString = connString
            conn.Open()
            Dim myAdapter As New MySqlDataAdapter(consultaSQL, conn)
            'ejecuta una consulta 
            myAdapter.Fill(myDataset)
            Return myDataset
            conn.Close()
        Catch ex As MySql.Data.MySqlClient.MySqlException
            'MessageBox.Show(ex.Message)
            Return Nothing
        End Try
    End Function

    Public Sub CreateTable(ByVal dt As DataTable, ByVal Optional nodrop As Boolean = False)

        Dim Primary As String = ""
        Dim sql As String = ""

        If nodrop = False Then
            sql += "DROP TABLE IF EXISTS `" + dt.TableName + "`; "
        End If

        sql += "CREATE TABLE IF NOT EXISTS `" + dt.TableName + "` "
        sql += "("
        Dim i As Integer

        For i = 0 To dt.Columns.Count - 1
            If (dt.Columns(i).Unique) Then
                If (String.IsNullOrWhiteSpace(Primary)) Then
                    Primary = "Primary KEY(`" + dt.Columns(i).ColumnName + "`, "
                Else
                    Primary += "`" + dt.Columns(i).ColumnName + "`, "
                End If
            End If
            sql += "`" + dt.Columns(i).ColumnName + "` " + GetTypeColumn(dt.Columns(i)) + IIf(i = dt.Columns.Count - 1, "", ",")
        Next

        If (Not String.IsNullOrWhiteSpace(Primary)) Then
            Primary = "," + Primary.Substring(0, Primary.Length - 2) + ")"
        End If

        sql += Primary
        sql += ") "
        sql += "ENGINE=MyISAM DEFAULT CHARSET=utf8"


        ActualizarDB(sql)

    End Sub

    Public Sub CreateSchema(database As String)
        Dim sql As String = ""
        sql += "DROP SCHEMA IF EXISTS `" + database + "`; "
        sql += "CREATE SCHEMA `" + database + "` ; "

        ActualizarDB(sql)
    End Sub

    Private Function GetTypeColumn(dataColumn As DataColumn) As String
        GetTypeColumn = ""
        If (dataColumn.DataType.Name.Contains("Int")) Then
            GetTypeColumn = " BigInt "
        ElseIf (dataColumn.DataType.Name = "Double") Then
            GetTypeColumn = " Double "
        ElseIf (dataColumn.DataType.Name = "Decimal") Then
            If (String.IsNullOrWhiteSpace(dataColumn.Caption)) Then
                GetTypeColumn = " Decimal(20,6) "
            Else
                GetTypeColumn = " Decimal(" + dataColumn.Caption + ") "
            End If
        ElseIf (dataColumn.DataType.Name = "DateTime") Then
            GetTypeColumn = " DATETIME "
        Else
            If (String.IsNullOrWhiteSpace(dataColumn.Caption)) Then
                GetTypeColumn = " varchar(255) "
            Else
                GetTypeColumn = " varchar(" + dataColumn.Caption + ") "
            End If
        End If


        'GetTypeColumn = GetTypeColumn + " DEFAULT NULL "
    End Function

    Private dataTable As Dictionary(Of String, DataTable) = New Dictionary(Of String, DataTable)

    Public Function GetTable(ByRef table As String) As DataTable
        If (dataTable.ContainsKey(table) = False) Then

            'MySqlBase.FillAdapterMultiple("SELECT * FROM " & table & " group by " & PrimaryKey

            dataTable.Add(table, FillAdapterMultiple("SELECT * FROM " & table))

        End If
        GetTable = dataTable(table)
    End Function

    Public Sub RunScript(filename As String)
        ActualizarDB(File.ReadAllText(filename))
    End Sub

    'ejecuta el comando consultaSQL en la base seleccionada
    Public Sub ActualizarDB(ByVal consultaSQL As String, Optional ByVal showErrores As Boolean = True)
        Dim conn As New MySql.Data.MySqlClient.MySqlConnection
        Dim cmd As New MySqlCommand

        ''Try

        conn.ConnectionString = connString
        conn.Open()
        'ejecuta una consulta 
        cmd.Connection = conn
        cmd.CommandText = consultaSQL
        cmd.ExecuteNonQuery()

        'Catch ex As MySql.Data.MySqlClient.MySqlException

        'If showErrores Then
        '        Throw ex
        '    Else
        '        RaiseEvent ErrorAlActualizarDatos(ex.Message)
        '    End If

        'Finally

        'cierro la coneccion si logro abrirla 
        If conn.State Then conn.Close()

        'End Try

    End Sub

    'ejecuta los comando consultaSQL en la base seleccionada
    Public Sub ActualizarDB(ByVal listaconsultasSQL As List(Of String), Optional ByVal showErrores As Boolean = True)
        Try


            Dim conn As New MySql.Data.MySqlClient.MySqlConnection
            Dim cmd As New MySqlCommand

            Dim contConsultas As Integer = 0

            conn.ConnectionString = connString
            conn.Open()
            cmd.Connection = conn

            'ejecuta las consultas 1 a 1
            For Each consultaSQL As String In listaconsultasSQL
                Try

                    cmd.CommandText = consultaSQL
                    cmd.ExecuteNonQuery()
                    contConsultas += 1

                Catch ex As MySql.Data.MySqlClient.MySqlException

                    If showErrores Then
                        Throw ex
                    Else
                        'RaiseEvent ErrorAlActualizarDatos(ex.Message)
                    End If

                End Try
            Next

            'cierro la coneccion si logro abrirla 
            If conn.State Then conn.Close()
        Catch ex As Exception

        End Try
    End Sub

    'Permite actualizar multiples campos en forma sensilla (EN DESARROLLO)
    Public Sub ActualizarDBMultiple(ByVal consultaSQL As String, ByVal values() As String)
        Dim conn As New MySql.Data.MySqlClient.MySqlConnection
        Dim cmd As New MySqlCommand
        Try
            conn.ConnectionString = connString
            conn.Open()
            'ejecuta una consulta 
            cmd.Connection = conn
            cmd.CommandText = "INSERT INTO nodosconectados VALUES(1, @number)"
            cmd.Prepare()
            cmd.Parameters.AddWithValue("@number", 1)
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Throw ex
        End Try
    End Sub

    Public Shared Sub Log(tipo As String, info As String, plugin As String, version As String, orden As Long)

        'Tabla.TableName = "sis_log"
        'Call CargarCol(Tabla, "fecha", GetType(System.DateTime))
        'Call CargarCol(Tabla, "orden", GetType(System.Int32))
        'Call CargarCol(Tabla, "tipo", GetType(System.String), "20")
        'Call CargarCol(Tabla, "informacion", GetType(System.String), "2048")
        'Call CargarCol(Tabla, "plugin", GetType(System.String), "20")
        'Call CargarCol(Tabla, "version", GetType(System.String), "20")

        Dim conn As New MySql.Data.MySqlClient.MySqlConnection
        Dim cmd As New MySqlCommand
        Try
            conn.ConnectionString = connString
            conn.Open()
            'ejecuta una consulta 
            cmd.Connection = conn
            cmd.CommandText = "INSERT INTO sis_log (fecha,orden,tipo,informacion,plugin,version) VALUES (now(),@orden, @tipo, @info, @plugin, @version)"
            cmd.Prepare()
            cmd.Parameters.AddWithValue("@orden", orden)
            cmd.Parameters.AddWithValue("@tipo", tipo)
            cmd.Parameters.AddWithValue("@info", info)
            cmd.Parameters.AddWithValue("@plugin", plugin)
            cmd.Parameters.AddWithValue("@version", version)
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Throw ex
        End Try
    End Sub

    Public Function ContarCantidadDatos(ByVal campoAContar As String, ByVal tabla As String, Optional ByVal condicion As String = "") As String
        Dim cantidadDatos As New DataSet
        FillAdapterMultiple(cantidadDatos, "CountTabla", "SELECT count( " & campoAContar & " ) FROM " & tabla & " " & condicion)
        ContarCantidadDatos = cantidadDatos.Tables("CountTabla").Rows(0).Item(0)
    End Function

    'alternativa usando los comando de mysqldataadapter, sin embargo hay un bug que hace que esto ande re lento
    Public Sub CopiarTablaDBViejo(ByVal tabla As DataTable, ByVal nombreTablaDB As String)
        Dim conn As New MySqlConnection With {
            .ConnectionString = connString
        }
        conn.Open()
        Dim myDataAdapter As New MySqlDataAdapter("select * from " & nombreTablaDB, conn)
        Dim myDataSet As New DataSet
        Dim myDataRowsCommandBuilder As MySqlCommandBuilder = New MySqlCommandBuilder(myDataAdapter)
        Dim row As DataRow
        myDataAdapter.Fill(myDataSet, nombreTablaDB)
        myDataSet.Tables(0).Merge(tabla) 'agrego los datos de la tabla que llama a la funcion
        For Each row In myDataSet.Tables(0).Rows 'cambio el estado de los rows a no agregado aun
            row.SetAdded()
        Next
        myDataAdapter.Update(myDataSet, nombreTablaDB)
    End Sub

    Public Sub CopiarTablaDB(ByVal tabla As DataTable, ByVal nombreTablaDB As String,
                             Optional tamanoConsulta As Integer = 100, Optional mostrarErrores As Boolean = True)
        Dim listaErrores As New List(Of String)
        Dim listaConsultas As List(Of String)

        listaConsultas = MySqlBase.GenerarInsertsFromTable(nombreTablaDB, tabla, tamanoConsulta)

        ActualizarDB(listaConsultas, mostrarErrores)

    End Sub

    Public Function ObtenerAutoIncrement(nombreTabla As String) As Integer
        Dim resultado As Integer

        Dim consulta As String = <sql>
                                     SHOW TABLE STATUS LIKE '<%= nombreTabla %>'
                                 </sql>
        Dim datosTabla As DataTable = FillAdapter(consulta).Tables(0)

        resultado = datosTabla.Rows(0).Item("Auto_increment")

        Return resultado
    End Function

    Public Shared Function GenerarInsertsFromTable(ByVal nombretabla As String, ByVal tabla As DataTable, Optional ByVal tamanoConsulta As Integer = 500) As List(Of String)
        Dim columna As DataColumn
        Dim row As DataRow
        Dim contadorFilas As Integer = 1
        Dim cantidadColumnas As Integer = tabla.Columns.Count
        Dim cantidadFilas As Integer = tabla.Rows.Count
        Dim stringColumnas As New System.Text.StringBuilder()
        Dim stringDatos As New System.Text.StringBuilder()
        Dim listaConsultas As New List(Of String)

        'genero las columnas
        stringColumnas.Append("(`")

        For Each columna In tabla.Columns
            stringColumnas.Append(columna.ColumnName)
            stringColumnas.Append("`,`")
        Next

        'remuevo la ultima ","
        stringColumnas.Remove(stringColumnas.ToString.Length - 3, 3)
        stringColumnas.Append("`)")

        'genero los datos

        For Each row In tabla.Rows

            stringDatos.Append("(")
            For i = 0 To cantidadColumnas - 1
                If IsDBNull(row(i)) Then
                    stringDatos.Append("NULL,")
                Else
                    Select Case tabla.Columns(i).DataType.Name
                        Case "Double", "Decimal", "Int16", "Int32", "Int64"
                            stringDatos.Append("" & row(i).ToString().Replace(",", ".") & ",")
                        Case Else
                            stringDatos.Append("'" & row(i) & "',")
                    End Select
                End If
            Next
            stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
            stringDatos.Append("),")

            contadorFilas += 1

            'controlo si tengo que generar el insert
            If contadorFilas >= tamanoConsulta Then
                stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
                listaConsultas.Add("INSERT INTO " & nombretabla & stringColumnas.ToString & " values " & stringDatos.ToString)

                contadorFilas = 1
                stringDatos = New System.Text.StringBuilder()
            End If

        Next

        'falta sumar los ultimos datos, genero una consulta con estos datos 
        If stringDatos.ToString <> "" Then

            stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
            listaConsultas.Add("INSERT INTO " & nombretabla & stringColumnas.ToString & " values " & stringDatos.ToString)

        End If

        Return listaConsultas
    End Function

    Public Sub GenerarInsertsFromTable2(ByVal nombretabla As String, ByVal tabla As DataTable, Optional ByVal tamanoConsulta As Integer = 500)
        Dim columna As DataColumn
        Dim row As DataRow
        Dim contadorFilas As Integer = 1
        Dim cantidadColumnas As Integer = tabla.Columns.Count
        Dim cantidadFilas As Integer = tabla.Rows.Count
        Dim stringColumnas As New System.Text.StringBuilder()
        Dim stringDatos As New System.Text.StringBuilder()
        Dim conn As New MySql.Data.MySqlClient.MySqlConnection
        Dim cmd As New MySqlCommand

        conn.ConnectionString = connString
        conn.Open()
        cmd.Connection = conn
        Try
            'genero las columnas
            stringColumnas.Append("(`")

            For Each columna In tabla.Columns
                stringColumnas.Append(columna.ColumnName)
                stringColumnas.Append("`,`")
            Next

            'remuevo la ultima ","
            stringColumnas.Remove(stringColumnas.ToString.Length - 3, 3)
            stringColumnas.Append("`)")

            'genero los datos
            For Each row In tabla.Rows
                stringDatos.Append("(")
                For i = 0 To cantidadColumnas - 1
                    If IsDBNull(row(i)) Then
                        stringDatos.Append("NULL,")
                    Else
                        stringDatos.Append("'" & row(i) & "',")
                    End If
                Next
                stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
                stringDatos.Append("),")
                contadorFilas += 1
                If contadorFilas >= tamanoConsulta Then
                    stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
                    cmd.CommandText = "INSERT INTO " & nombretabla & stringColumnas.ToString & " values " & stringDatos.ToString
                    cmd.ExecuteNonQuery()
                    contadorFilas = 1
                    stringDatos = New System.Text.StringBuilder()
                End If
            Next
            'falta sumar los ultimos datos, genero una consulta con estos datos 
            If stringDatos.ToString <> "" Then
                stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
                cmd.CommandText = "INSERT INTO " & nombretabla & stringColumnas.ToString & " values " & stringDatos.ToString
                cmd.ExecuteNonQuery()
            End If
        Catch ex As Exception
            Auxiliares.Log(Auxiliares.LOG_TYPE_ERROR, ex.Message + "/" + nombretabla + "/"+ stringDatos.ToString)
            Throw ex
            
        End Try

        conn.Close()
    End Sub

    'Genera una sentencia insert multiple con el formato
    'insert into @nombreTabla (@camposConsulta()) values (@camposDatos)
    'es ideal cuadno se tiene que insertar gran cantidad de datos para ganar en rendimiento
    'los vectores tienen que ser enviados con el tamaño correcto para que funcione correctamente
    'devuelve un string con la consulta lista para ser ejecutada en la base de datos
    Public Shared Function GenerarINSERTMultiple(ByVal nombretabla As String, ByVal camposConsulta() As String, ByVal camposDatos() As String) As String
        Dim campos As String = ""
        Dim consulta As String = ""
        Dim numCampoDato As Integer
        Dim i As Integer
        Dim j As Integer
        Dim stringDatos As New System.Text.StringBuilder()
        consulta = ""
        'genero los campos
        For i = 1 To camposConsulta.Length
            If i = 1 Then
                campos = "`" & camposConsulta(i - 1) & "`"
            Else
                campos = campos & ",`" & camposConsulta(i - 1) & "`"
            End If
        Next i

        numCampoDato = 0

        For j = 1 To camposDatos.Length / camposConsulta.Length

            stringDatos.Append("(")

            For i = 1 To camposConsulta.Length
                If camposDatos(numCampoDato) = "" Or IsDBNull(camposDatos(numCampoDato)) Then
                    camposDatos(numCampoDato) = "NULL" 'para poner los nulos en los campos vacios
                Else
                    camposDatos(numCampoDato) = "'" & camposDatos(numCampoDato) & "'" 'si no, le pongo comillas para pasar el dato
                End If

                stringDatos.Append(camposDatos(numCampoDato))
                stringDatos.Append(",")

                numCampoDato = numCampoDato + 1
            Next i

            'remuevo la ultima ","
            stringDatos.Remove(stringDatos.ToString.Length - 1, 1)
            stringDatos.Append("),")

        Next j

        'remuevo la ultima ","
        stringDatos.Remove(stringDatos.ToString.Length - 1, 1)

        Dim textoDatos As String = stringDatos.ToString

        Return "INSERT INTO " & nombretabla & "(" & campos & ") Values " & textoDatos

    End Function

    Public Sub BorrarCamposDB(nombreTabla As String, condicion As String)

        Dim consulta As String = <sql>
                                     DELETE FROM <%= nombreTabla %>
                                     WHERE <%= condicion %>
                                 </sql>
        ExecuteScalar(consulta)

    End Sub

    Public Sub ActualizarTablaDB(ByVal tablaDatos As DataTable, ByVal nombreTablaDB As String,
                                 Optional ByVal campoId As String = Nothing, Optional tamanoConsulta As Integer = 500)

        Dim listaConsultas As New List(Of String)

        Dim camposConsulta As New List(Of String)
        Dim camposDatos As New List(Of String)
        Dim camposDatosOriginales As New List(Of String)


        Dim consulta As String
        Dim row As DataRow

        Dim tablaModificada As DataTable = tablaDatos.GetChanges

        'me fijo si la tabla tiene cambios
        If tablaModificada Is Nothing Then Exit Sub

        'saco los nombres de las columnas
        For Each columna As DataColumn In tablaModificada.Columns
            camposConsulta.Add(columna.ColumnName)
        Next

        'me fijo si se seteo un campo id o no
        Dim posicionCampoId As Integer = -1

        If campoId <> Nothing Then
            Dim i As Integer = 0
            For Each columna As DataColumn In tablaModificada.Columns
                ' ignorar case sensitive
                If String.Compare(columna.ColumnName, campoId, True) = 0 Then
                    posicionCampoId = i
                End If
                i += 1
            Next
        End If

        If posicionCampoId = -1 And campoId <> Nothing Then 'si pasa esto no encontro el campo id en la lista de campos genero error
            Auxiliares.Evento("no se encontro el campo id")
            Exit Sub
        End If

        'genero una sentencia update para cada registro modificado
        Dim contFila As Integer = 1
        Dim contTamañoConsulta As Integer = 1
        For Each row In tablaModificada.Rows

            camposDatos = New List(Of String)
            camposDatosOriginales = New List(Of String)

            For Each columna As DataColumn In tablaModificada.Columns

                If Not IsDBNull(row(columna)) Then
                    camposDatos.Add(row(columna).ToString)
                Else
                    camposDatos.Add("")
                End If

                'si no seteo un campo id, espero tener un dato modificado en la tabla para saber que actualizar y que no 
                If posicionCampoId = -1 Then
                    Dim valorOriginalRow As String = row.Item(columna, DataRowVersion.Original)

                    If Not IsDBNull(valorOriginalRow) Then
                        camposDatosOriginales.Add(valorOriginalRow)
                    Else
                        camposDatosOriginales.Add("")
                    End If

                Else

                    If columna.ColumnName = campoId Then
                        camposDatosOriginales.Add(row(columna))
                    Else
                        camposDatosOriginales.Add("/NO_VALUE/")
                    End If

                End If

            Next

            consulta = GenerarUPDATE(nombreTablaDB, camposConsulta.ToArray, camposDatos.ToArray,
                                      camposDatosOriginales.ToArray, campoId, posicionCampoId)

            listaConsultas.Add(consulta)

            contFila += 1
            contTamañoConsulta += 1

        Next

        ActualizarDB(listaConsultas)

    End Sub

    Public Function GenerarUPDATE(nombreTabla As String, ByVal camposConsulta() As String, ByVal camposDatos() As String, ByVal camposDatosOriginales() As String, nombreCampoId As String, posicionCampoId As Integer) As String
        Dim datos As String = ""
        Dim consulta As String = ""
        Dim condicion As String = ""
        Dim i As Integer

        For i = 0 To UBound(camposConsulta)
            If camposDatos(i) = "" Then
                camposDatos(i) = "NULL" 'para poner los nulos en los campos vacios
            Else
                camposDatos(i) = "'" & camposDatos(i) & "'" 'si no, le pongo comillas para pasar el dato
            End If
            If i = 0 Then
                datos = "`" & camposConsulta(i) & "` = " & camposDatos(i)
            Else
                datos = datos & ", `" & camposConsulta(i) & "` = " & camposDatos(i)
            End If
        Next i

        If posicionCampoId <> -1 Then 'graba por id
            condicion = nombreCampoId & " = " & camposDatos(posicionCampoId)
        Else 'graba segun la tabla original
            For i = 0 To UBound(camposConsulta)
                If i <> 0 Then condicion += " AND "
                If camposDatosOriginales(i) <> "/NO_VALUE/" Then
                    condicion += "`" & camposConsulta(i) & "`" & " = '" & camposDatosOriginales(i) & "'"
                End If
            Next
        End If

        consulta = "UPDATE " & nombreTabla & " SET " & datos & " where " & condicion
        Return consulta
    End Function

    'Generic method for getting a field from a table
    Public Function ObtenerDatosColumna(ByVal tabla As String, ByVal columna As String,
             Optional ByVal isDistinct As Boolean = False,
             Optional ByVal isNotNull As Boolean = False) As DataSet
        Dim myDataSet As New DataSet
        Dim consulta As String
        Dim distinct As New String("")
        Dim null As New String("")
        'Add distinct clausule
        If (isDistinct) Then
            distinct = " distinct "
        End If
        'Add not null clausule
        If (isNotNull) Then
            null = " where " + columna + " is not null "
        End If
        'Generate query
        consulta = "select " + distinct + columna + " as " + columna + " from " + tabla + null + " order by " + columna
        'Fill Data Set
        FillAdapterMultiple(myDataSet, tabla, consulta)
        Return myDataSet
    End Function

    Public Function ObtenerDatosRegistro(ByVal tabla As String, Optional ByVal columnaFiltro As String = Nothing,
          Optional ByVal columnaValue As Object = Nothing) As DataSet
        Dim myDataSet As New DataSet
        Dim consulta As String
        'Generate query
        consulta = "select * from " + tabla
        'Add where clausule
        If (columnaFiltro <> Nothing) Then
            'TODO Improve columnaValue sql convertion, now we assume is numeric value
            consulta += " where " + columnaFiltro + " = " + columnaValue.ToString()
        End If
        FillAdapterMultiple(myDataSet, tabla, consulta)
        Return myDataSet
    End Function

    Public Sub OptimizarTabla(ByVal tabla As String)
        Dim consulta As String
        consulta = <sql>OPTIMIZE TABLE <%= tabla %></sql>
        ActualizarDB(consulta)
    End Sub

    'obtiene el nuevo idTXT para cargar nuevos archivos
    Public Function ObtenerProximoIdTXT(tabla As String) As Integer
        Dim idTXT As Integer
        Dim consulta As String
        Dim myDataSet As New DataSet

        consulta = <sql>
                    Select max(ID_TXT)
                    from <%= tabla %> 
                    group by 1=1
                   </sql>

        FillAdapterMultiple(myDataSet, "IdTXT", consulta)

        If myDataSet.Tables("IdTXT").Rows.Count <> 0 Then
            idTXT = myDataSet.Tables("IdTXT").Rows(0).Item(0)
        Else
            idTXT = 0
        End If

        Return idTXT + 1
    End Function

    Public Function ObtenerProximoId(ByVal nombreTabla As String, ByVal nombreCampo As String) As Long

        Dim idTablaDatos As Object
        Dim idTablaDiff As Object

        nombreTabla = nombreTabla.ToLower

        If Not nombreTabla.Contains("bdgd_") Then
            nombreTabla = "bdgd_" & nombreTabla
        End If

        Dim query As String = <sql>
                                    SELECT MAX(<%= nombreCampo %>) FROM <%= nombreTabla %>
                              </sql>

        idTablaDatos = ExecuteScalar(query)
        If IsDBNull(idTablaDatos) Then idTablaDatos = 0

        Dim queryDiff As String = <sql>
                                    SELECT MAX(<%= nombreCampo %>) FROM <%= nombreTabla.Replace("bdgd", "diff") %>
                                  </sql>

        idTablaDiff = ExecuteScalar(queryDiff)
        If IsDBNull(idTablaDiff) Then idTablaDiff = 0

        If (idTablaDiff > idTablaDatos) Then
            Return idTablaDiff + 1
        Else
            Return idTablaDatos + 1
        End If

    End Function

End Class
