Imports System.Threading
Imports System.Xml
Imports log4net
Imports log4net.Appender
Imports log4net.Layout
Imports log4net.Repository.Hierarchy

Public Module Auxiliares

    Public LogIndex As Long = 0

    Public Const LOG_TYPE_INFO As String = "INFO"
    Public Const LOG_TYPE_ERROR As String = "ERROR"

    Public logger As log4net.ILog = Nothing

    Public Sub Log4NetStart(logName As String)
        If logger Is Nothing Then
            Dim docxml As XmlDocument = New XmlDocument()

            docxml.LoadXml("<?xml version='1.0' encoding='utf-8' ?>
            <log4net debug='false'>
               <root>
                 <level value='DEBUG' />
               </root>

               <!-- more config -->

               <logger name='EmptyLogger' additivity='false'>
               </logger>
               <!-- additivity='false' specifies not to load any appenders defined on the root logger -->
            </log4net>")

            Config.XmlConfigurator.Configure(docxml.DocumentElement)

            ' Get a uniquely-named empty logger containing no appenders
            logger = LogManager.GetLogger("EmptyLogger." & logName)

            ' Create an appender manually, setting the path as required
            Dim sPath As String = logName & ".log"

            Dim appender = New RollingFileAppender() With {
                 .Layout = New PatternLayout("%d [%t] %-5p %c - %m%n"),
                 .File = sPath,
                 .AppendToFile = True,
                 .RollingStyle = RollingFileAppender.RollingMode.Composite.Date,
                 .DatePattern = "yyyy.MM.dd",
                 .StaticLogFileName = True
            }

            appender.ActivateOptions()

            ' Add the appender to the logger
            DirectCast(logger.Logger, Logger).AddAppender(appender)

            ' And off we go.
            'log.Info("Hello from thread " + Thread.CurrentThread.ManagedThreadId.ToString())
            logger.Info("Start Log " & logName)

            Thread.Sleep(100)
        End If
    End Sub

    Public Sub Log4NetError(message As String, ex As Exception)
        If Not (logger Is Nothing) Then
            If ex Is Nothing Then
                logger.Error(message)
            Else
                logger.Error(message, ex)
            End If
        End If
    End Sub

    Public Sub Log4NetInfo(message As String)
        If Not (logger Is Nothing) Then
            logger.Info(message)
        End If
    End Sub

    Public Function GetAnio(value As Long) As Long
        GetAnio = CLng(Math.Truncate(value / 100))
    End Function

    Public Function GetSemana(value As Long) As Long
        GetSemana = value - (GetAnio(value) * 100)
    End Function

    Public Sub Log(tipo As String, info As String)
        Try
            Dim index As Int32 = 0

            Dim s As StackTrace = New StackTrace()

            For i As Integer = 0 To s.FrameCount - 1
                If (Not s.GetFrame(i).GetMethod().ReflectedType.GetInterface("IPlugin") Is Nothing) Then
                    index = i
                    Exit For
                End If
            Next

            Dim version = ""

            Try
                version = s.GetFrame(index).GetMethod().ReflectedType.GetMethod("VersionPlugin").Invoke(Nothing, Nothing).ToString()
            Catch ex As Exception

            End Try

            LogIndex = LogIndex + 1
            MySqlBase.Log(tipo, info, s.GetFrame(index).GetMethod().ReflectedType.Assembly.FullName, version, LogIndex)

        Catch ex As Exception

        End Try
    End Sub

    Public Sub Evento(value As String)
        Debug.WriteLine(value)
        Log(LOG_TYPE_INFO, value)
    End Sub

    Public Sub Evento(value As String, tipo As String)
        Debug.WriteLine(value)
        Log(tipo, value)
    End Sub

    Public Sub CalculoSegundos(label As String, ByRef tiempo As DateTime)
        Dim tiempoS As String
        Dim tiempoTemp As Integer
        tiempoTemp = (DateTime.Now.Hour * 60 * 60 + DateTime.Now.Minute * 60 + DateTime.Now.Second - (tiempo.Hour * 60 * 60 + tiempo.Minute * 60 + tiempo.Second))
        tiempo = DateTime.Now
        tiempoS = tiempoTemp.ToString
        Call Evento(label & ": " & tiempoS & " segundos")
    End Sub

    Public Sub AcumVar(ValorProm As Dictionary(Of String, Double), key As String, Valor As Double)
        If ValorProm.ContainsKey(key) Then
            ValorProm(key) = ValorProm(key) + Valor
        Else
            ValorProm.Add(key, Valor)
        End If
    End Sub

    Public Sub CargarCol(Tabla As DataTable, Name As String, dataType As String)
        Dim column As DataColumn
        column = New DataColumn() With {
            .DataType = System.Type.GetType(dataType),
            .ColumnName = Name,
            .Caption = ""
        }
        Tabla.Columns.Add(column)
    End Sub

    Public Sub CargarCol(Tabla As DataTable, Name As String, dataType As System.Type)
        CargarCol(Tabla, Name, dataType, False, "")
    End Sub

    Public Sub CargarCol(Tabla As DataTable, Name As String, dataType As System.Type, isPimary As Boolean)
        CargarCol(Tabla, Name, dataType, isPimary, "")
    End Sub

    Public Sub CargarCol(Tabla As DataTable, Name As String, dataType As System.Type, ExtraInfo As String)
        CargarCol(Tabla, Name, dataType, False, ExtraInfo)
    End Sub

    Public Sub CargarCol(Tabla As DataTable, Name As String, dataType As System.Type, isPimary As Boolean, ExtraInfo As String)
        Dim column As DataColumn
        column = New DataColumn() With {
            .Unique = isPimary,
            .DataType = dataType,
            .ColumnName = Name,
            .Caption = ExtraInfo
        }
        Tabla.Columns.Add(column)
    End Sub

    Public Function Percentile(marks() As Double, percentil As Integer) As Double
        Percentile = marks.OrderBy(Function(n) n).Skip(CInt(Math.Floor(percentil * marks.Length / 100))).First()
    End Function

    Public Sub CalcularPromedioMinMax(datares As DataSet, CantidadEscenarios As Integer, ByRef tiempoT As DateTime)
        Dim i As DataRow
        Dim t As DataTable
        Dim c As Integer
        Dim promedio As Double
        Dim max As Double
        Dim min As Double
        Dim temp As Double

        For Each t In datares.Tables
            Select Case t.TableName
                Case "S_driverFlujoHidroductos", "S_potenciaGenTN"

                Case Else
                    Call CargarCol(t, "Promedio", "System.Double")
                    Call CargarCol(t, "Max", "System.Double")
                    Call CargarCol(t, "Percentil25", "System.Double")
                    Call CargarCol(t, "Percentil75", "System.Double")
                    Call CargarCol(t, "Min", "System.Double")
                    For Each i In t.Rows
                        temp = 0
                        max = -10000000000
                        min = 1000000000
                        Dim valores(CantidadEscenarios - 1) As Double
                        For c = 1 To CantidadEscenarios
                            valores(c - 1) = CDbl(i("Valor" & c))
                            temp = temp + CDbl(i("Valor" & c))
                            If CDbl(i("Valor" & c)) > max Then max = CDbl(i("Valor" & c))
                            If CDbl(i("Valor" & c)) < min Then min = CDbl(i("Valor" & c))
                        Next c
                        promedio = temp / CantidadEscenarios
                        i("Promedio") = promedio
                        i("Percentil25") = Percentile(valores, 25)
                        i("Percentil75") = Percentile(valores, 75)
                        i("Max") = max
                        i("Min") = min
                    Next i
            End Select
        Next t
        Call CalculoSegundos("calcularPromedioMinMax", tiempoT)

    End Sub

End Module
