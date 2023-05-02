Imports MEMDataService.com.gq.constantes
Imports MEMDataService.com.gq.constantes.Constantes
Imports MEMDataService.com.gq.InterfaceLibreriaExterna
Imports MEMLibCommon
Imports MEMLibModelo.com.mem.ejecutar

Public Class Plugin
    Implements IPlugin

    Public Function CommandsList() As String() Implements IPlugin.CommandsList
        CommandsList = {
            "GenerarTablasPlanas",
            "EjecutarModelo"
            }

    End Function

    Public Function Command(comando As String, ParamArray parametros() As Object) As String Implements IPlugin.Command

        Command = "OK"
        Try
            Select Case comando.ToLower()
                Case "generartablasplanas"
                    If (parametros.Length = 4) Then
                        Dim em As com.mem.ejecutar.EjecutarModelo = New com.mem.ejecutar.EjecutarModelo()
                        em.GenerarTablasPlanas(parametros(0), parametros(1), parametros(2), parametros(3))
                    Else
                        Command = "
                                    GENERACION DE TABLAS PLANAS MOD
                                    Necesita 6 parametros server, database, user, password
                                    "
                    End If
                Case "ejecutarmodelo"
                    If (parametros.Length = 4) Then

                        Dim em As com.mem.ejecutar.EjecutarModelo = New com.mem.ejecutar.EjecutarModelo()
                        em.CorrerModelo(parametros(0), parametros(1), parametros(2), parametros(3))
                    Else
                        Command = "
                                    EJECUTAR MODELO
                                    Necesita 6 parametros server, database, user, password
                                    "
                    End If
            End Select
        Catch ex As Exception
            Command = "FAIL : " + ex.Message + vbCrLf + ex.StackTrace
            Auxiliares.Evento(Command, Auxiliares.LOG_TYPE_ERROR)
            Throw New Exception("Command")
        End Try

    End Function

    Public Function Version() As String Implements IPlugin.Version
        Version = VersionPlugin()
    End Function

    Public Shared Function VersionPlugin() As String
        VersionPlugin = "20171026"
    End Function

End Class
