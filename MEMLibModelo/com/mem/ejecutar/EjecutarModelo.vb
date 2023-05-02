Option Strict On
Imports Gurobi
Imports MEMLibCommon

Namespace com.mem.ejecutar

    Public Class EjecutarModelo

        Public Sub GenerarTablasPlanas(server As String, database As String, user As String, password As String)
            Try
                Log4NetStart(database)
                Log4NetInfo("GenerarTablasPlanas database: " + database + ", user: " + user + ", password: " + password)
                Dim MySqlBase As New MySqlBase
                MySqlBase.connString = MySqlBase.GetConectionString(server, database, user, password)
                MySqlBase = New MySqlBase()
                Auxiliares.Evento("INICIO --- CrearTablasPlanas", Auxiliares.LOG_TYPE_INFO)
                'tabla escritura
                Call GenerarLog(MySqlBase)
                Call GenerarSis_tablasescritura(MySqlBase)
                'tablas Mod
                Call GenerarMod_licitacionmes(MySqlBase)
                Call GenerarMod_licitacionperfil(MySqlBase)
                Call GenerarMod_oferente(MySqlBase)
                Call GenerarMod_oferentemes(MySqlBase)
                Call GenerarMod_oferenteperfil(MySqlBase)
                Call GenerarMod_parametros(MySqlBase)
                Call GenerarRes_Resultado(MySqlBase)
                Call GenerarMod_Adjudicados(MySqlBase)
                Call GenerarMod_EvolucionK(MySqlBase)
                Call GenerarTabla_Precios(MySqlBase)

                Auxiliares.Evento("FIN --- CrearTablasPlanas", Auxiliares.LOG_TYPE_INFO)
            Catch ex As Exception
                Log4NetError("GenerarTablasPlanas", ex)
                Throw New Exception("GenerarTablasPlanas")
            End Try
        End Sub

        Public Sub CorrerModelo(server As String, database As String, user As String, password As String)
            Try
                'Dim database2 As String = "peg2020"

                'Log4NetStart(database)
                'Log4NetInfo("CorrerModelo")
                Dim MySqlBase As New MySqlBase
                MySqlBase.connString = MySqlBase.GetConectionString(server, database, user, password)
                MySqlBase = New MySqlBase()

                Dim IDRonda As Integer = CInt(MySqlBase.ExecuteScalar("select distinct IDRonda from tabla_precios"))


                'Log4NetStart(database2)
                'Log4NetInfo("CorrerModelo")
                'Dim MySqlBase As New MySqlBase
                'MySqlBase.connString = MySqlBase.GetConectionString(server, database2, user, password)
                'MySqlBase = New MySqlBase()

                Dim CorrerModoForzado As Boolean

                CorrerModoForzado = True 'False: no genera columnas adjudicado,forzado,CostoTotalOferenteSobreCostoFO

                Auxiliares.Evento("INICIO --- CorrerModelo --- Ronda Nro: " & IDRonda, Auxiliares.LOG_TYPE_INFO)

                Log4NetInfo("data")
                Dim data As New DataSet

                Log4NetInfo("env")
                Dim env As New GRBEnv()

                Log4NetInfo("model")
                Dim model As New GRBModel(env)

                Log4NetInfo("NV")
                Dim NV As New Dictionary(Of String, Int32)

                Log4NetInfo("CantElem")
                Dim CantElem As New Dictionary(Of String, Int32)

                Log4NetInfo("Dicc")
                Dim Dicc As New Dictionary(Of String, Int32)

                Log4NetInfo("DiccTxt")
                Dim DiccTxt As New Dictionary(Of String, String)

                Log4NetInfo("Vars")
                Dim Vars As New List(Of GRBVar(,))

                Log4NetInfo("Vars2")
                Dim Vars2 As New List(Of GRBVar(,,))

                Log4NetInfo("tiempo")
                Dim tiempo As DateTime = DateTime.Now

                Log4NetInfo("tiempoT")
                Dim tiempoT As DateTime = DateTime.Now

                Log4NetInfo("tiempoTT")
                Dim tiempoTT As DateTime = DateTime.Now
                Dim Bandera As Boolean = True
                env.Threads = 4
                env.LogToConsole = 1
                env.OutputFlag = 1


                'Se importan datos para no tener que cargar la nueva base
                'Log4NetInfo("ImportarDatosRonda")
                'Call ImportarDatosRonda(data, MySqlBase, tiempo, Bandera, database, server, password, user)

                'Vuelve a conectarse a la BD para obtener las tablas del esquema actual
                'MySqlBase.connString = MySqlBase.GetConectionString(server, database, user, password)
                'Log4NetInfo("LecturaInfo")
                Call LecturaInfo(data, CantElem, Dicc, DiccTxt, MySqlBase, tiempo, Bandera, CorrerModoForzado)

                'Se actualizan los datos para no tener que cargar la nueva base
                'Log4NetInfo("ActualizarDatos")
                'Call ActualizarModOfModOfMes(data, Bandera)

                If Bandera Then
                    Log4NetInfo("AgregarVariables")
                    Call AgregarVariables(model, data, CantElem, Dicc, Vars, Vars2, NV, tiempo, tiempoT, CorrerModoForzado)

                    Log4NetInfo("Restricciones")
                    Call Restricciones(model, data, Vars, Vars2, NV, Dicc, tiempo, tiempoT, 0, CorrerModoForzado)

                    Log4NetInfo("EjecModelo")
                    Call EjecModelo(model, tiempoT)

                    Log4NetInfo("GenerarTablasSalida")
                    Call GenerarTablasSalida(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo, tiempoT, CorrerModoForzado)
                    Call CalculoSegundos("Fin - CorrerModelo", tiempoTT)
                    Auxiliares.Evento("FIN --- CorrerModelo", Auxiliares.LOG_TYPE_INFO)

                    Dim FOPOptima As Double
                    Dim FOEOptima As Double
                    Dim FOOptima As Double

                    CostoTotalOferente(data, Vars, Vars2, NV, Dicc, -1, FOOptima, FOPOptima, FOEOptima, model)

                    FOOptima = model.ObjVal

                    If CorrerModoForzado Then GenerarSalidasxOferenteForz(data, CantElem, Dicc, MySqlBase, FOOptima, FOPOptima, FOEOptima) ' rutina que itera sobre todos los oferentes forzando uno por uno

                Else
                    Auxiliares.Evento("ERROR --- No se ejecutó el modelo por problemas de inconsistencias", Auxiliares.LOG_TYPE_INFO)
                End If


                model.Dispose()
                data.Dispose()
                env.Dispose()

                Auxiliares.Evento("FIN --- CorrerModelo --- Ronda Nro: " & IDRonda, Auxiliares.LOG_TYPE_INFO)

            Catch ex As Exception
                Log4NetError("CorrerModelo", ex)
                Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
                Throw New Exception("CorrerModelo")
            End Try
        End Sub

        Sub GenerarSalidasxOferenteForz(data As DataSet,
                                        cantElem As Dictionary(Of String, Int32),
                                        Dicc As Dictionary(Of String, Int32),
                                        MySqlBase As MySqlBase,
                                        FOOptima As Double,
                                        FOPOptima As Double,
                                        FOEOptima As Double)

            Dim ID_Oferente As Integer
            Dim env As New GRBEnv()
            Dim model As New GRBModel(env)
            Dim tiempo As DateTime = DateTime.Now
            Dim tiempoT As DateTime = DateTime.Now
            Dim Vars As New List(Of GRBVar(,))
            Dim Vars2 As New List(Of GRBVar(,,))
            Dim NV As New Dictionary(Of String, Int32)

            For Each i As DataRow In data.Tables("Mod_Oferente").Select("Forzado=-1")
                model = New GRBModel(env)
                ID_Oferente = CInt(i("ID_Oferente"))
                tiempo = DateTime.Now
                tiempoT = DateTime.Now
                Vars = New List(Of GRBVar(,))
                Vars2 = New List(Of GRBVar(,,))
                NV = New Dictionary(Of String, Int32)

                Log4NetInfo("AgregarVariables, Id Oferente forzado: " & CInt(i("ID_Oferente")))
                Call AgregarVariables(model, data, cantElem, Dicc, Vars, Vars2, NV, tiempo, tiempoT, True)

                Log4NetInfo("Restricciones, Id Oferente forzado: " & CInt(i("ID_Oferente")))
                Call Restricciones(model, data, Vars, Vars2, NV, Dicc, tiempo, tiempoT, ID_Oferente, True) ' le pasa en la restriccion el oferente a forzar

                Log4NetInfo("EjecModelo, Id Oferente forzado: " & CInt(i("ID_Oferente")))
                Call EjecModelo(model, tiempoT)

                If model.Status = 2 Then
                    Dim costoTotal As Double = 0
                    Dim costoTotalP As Double = 0
                    Dim costoTotalE As Double = 0

                    'se calcula para el oferente analizado
                    CostoTotalOferente(data, Vars, Vars2, NV, Dicc, ID_Oferente, costoTotal, costoTotalP, costoTotalE, model)
                    i("CostoTotalOferente") = costoTotal

                    'se calcula el total
                    costoTotal = 0
                    costoTotalP = 0
                    costoTotalE = 0

                    CostoTotalOferente(data, Vars, Vars2, NV, Dicc, -1, costoTotal, costoTotalP, costoTotalE, model)
                    i("SobreCostoFO") = model.ObjVal - FOOptima


                Else
                    i("CostoTotalOferente") = -1
                    i("SobreCostoFO") = -1
                End If

                model.Dispose()

                Call CalculoSegundos("Fin Oferente Forzado:" & CStr(i("ID_Oferente")), tiempo)
            Next i

            Call GenerarMod_OferenteEsc(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo, True, True)
            Call AgregarReduccionModAdjudicados(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo, True, True)

        End Sub

        Private Sub CostoTotalOferente(Data As DataSet,
                                           ByVal Vars As List(Of GRBVar(,)),
                                           ByRef Vars2 As List(Of GRBVar(,,)),
                                           NV As Dictionary(Of String, Int32),
                                           Dicc As Dictionary(Of String, Int32),
                                           Oferente As Integer,
                                           ByRef costoTotal As Double,
                                           ByRef costoTotalP As Double,
                                           ByRef costoTotalE As Double,
                                       GRBModel As GRBModel)
            Dim ID_Oferente As Integer
            Dim ID_Mes As Integer
            Dim ID_AE As Integer
            Dim DiasMes As New Dictionary(Of Integer, Integer)
            Dim PerfilOferente As New Dictionary(Of String, Double)
            Dim Temp As String
            Dim ID_Hora As Integer

            For Each i As DataRow In Data.Tables("Mod_LicitacionMes").Rows
                DiasMes.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
            Next

            'se calcula el costo total de energía del OV
            If Oferente = -1 Then
                For Each i As DataRow In Data.Tables("Mod_LicitacionPerfil").Rows
                    ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
                    ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
                    For Each j As DataRow In Data.Tables("Mod_LicitacionMes").Select
                        costoTotalE = costoTotalE + CDbl(j("PrecioEnergiaOferenteVirtual")) * Vars2.Item(NV("EOV"))(0, ID_Mes, ID_Hora).X * DiasMes(CInt(i("ID_Mes"))) * 1000
                    Next
                Next
                'se calcula el costo total de potencia del OV
                For Each i As DataRow In Data.Tables("Mod_LicitacionMes").Rows
                    ID_AE = Dicc("AE:" & CStr(i("AE")))
                    ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))

                    costoTotalP = costoTotalP + CDbl(i("PrecioPotenciaOferenteVirtualResto")) * Vars.Item(NV("POVRESTO"))(0, ID_AE).X * 1000
                    costoTotalP = costoTotalP + CDbl(i("PrecioPotenciaOferenteVirtualSP")) * Vars.Item(NV("POVSP"))(0, ID_AE).X * 1000
                Next
            End If

            For Each i As DataRow In Data.Tables("Mod_OferentePerfil").Select("ID_Oferente=" & Oferente & " or '-1'='" & Oferente & "'")
                Temp = CInt(i("ID_Oferente")) & "_" & CInt(i("ID_Mes"))
                If Not PerfilOferente.ContainsKey(Temp) Then
                    PerfilOferente.Add(Temp, 0)
                End If
                ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
                ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
                ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
                PerfilOferente(Temp) = PerfilOferente(Temp) + Math.Round(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora).X * DiasMes(CInt(i("ID_Mes"))), 6)
            Next

            For Each i As DataRow In Data.Tables("Mod_OferenteMes").Select("ID_Oferente=" & Oferente & " or '-1'='" & Oferente & "'")
                ID_AE = Dicc("AE:" & CStr(i("AE")))
                ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
                Temp = CInt(i("ID_Oferente")) & "_" & CInt(i("ID_Mes"))
                i("PotenciaAdjudicada") = Math.Round(Vars.Item(NV("P"))(ID_Oferente, ID_AE).X, 6)
                i("EnergiaAsignada") = PerfilOferente(Temp)
                i("CostoPotencia") = CDbl(i("PotenciaAdjudicada")) * CDbl(i("PrecioPotencia")) * 1000

                If Oferente > 0 And CStr(i("ID_Contrato")) = "OC" Then
                    'se calcula el costo de energía teórico, a fines de estimar un porcentaje de reducción correcto (FC=1)
                    i("CostoEnergia") = CDbl(i("PotenciaAdjudicada")) * 24 * DiasMes(CInt(i("ID_Mes"))) * CDbl(i("PrecioEnergia")) * 1000
                Else
                    i("CostoEnergia") = CDbl(i("EnergiaAsignada")) * CDbl(i("PrecioEnergia")) * 1000
                End If

                costoTotalE = costoTotalE + CDbl(i("CostoEnergia"))
                costoTotalP = costoTotalP + CDbl(i("CostoPotencia"))


            Next

            costoTotal = costoTotalE + costoTotalP
        End Sub

    End Class
End Namespace