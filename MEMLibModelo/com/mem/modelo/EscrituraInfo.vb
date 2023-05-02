Option Strict On
Imports Gurobi
Imports MEMLibCommon

Module EscrituraInfo

    Public Sub GenerarTablasSalida(model As GRBModel, data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime, ByRef tiempoT As DateTime, CorrerModoForzado As Boolean)
        Try
            If model.Status = GRB.Status.OPTIMAL Then
                tiempo = DateTime.Now
                Call GenerarMod_OferenteMes(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo)
                Call GenerarMod_OferentePerfil(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo)
                Call GenerarMod_LicitacionMes(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo)
                Call GenerarMod_LicitacionPerfil(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo)
                Call GenerarMod_Adjudicados(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo, False, CorrerModoForzado)
                Call LlenarRes_Resultado(MySqlBase)
                Call CalculoSegundos("GenerarTablasSalida Total ", tiempoT)
                Call GenerarMod_OferenteEsc(model, data, MySqlBase, Vars, Vars2, NV, Dicc, tiempo, False, CorrerModoForzado)
            Else
                Auxiliares.Evento("ERROR --- No obtuvo resultado", Auxiliares.LOG_TYPE_ERROR)
        End If

        Catch ex As Exception
            Log4NetError("GenerarTablasSalida", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("GenerarTablasSalida")
        End Try
    End Sub

    Private Sub LlenarRes_Resultado(MySqlBase As MySqlBase)
        Dim Query As String
        MySqlBase.ActualizarDB("delete from res_resultado")

        Query = "Insert into res_resultado
                SELECT t1.ID_Oferente,Nombre,t2.id_Contrato as Contrato, t2.ID_Comb1 as ID_Comb1, t2.ID_Comb2 as ID_Comb2,
                If (t1.PotenciaAdjudicada > 0, round((CostoPotencia) / PotenciaAdjudicada, 3), 0)  As 'Precio Potencia (USD/kW-Mes)', 
                If (t1.EnergiaAsignada > 0, round((CostoEnergia) / EnergiaAsignada, 5), 0)  As 'Precio Energia (USD/MWh)', 
                round((EnergiaAsignada), 0) As 'Energia Asignada (MWh)',
                round(PotenciaOfertada * 1000, 0) as 'Potencia Media Ofertada (kW)',
                If (PPA > 0, round(PotenciaAdjudicada / (PPA), 0), 0)  As 'Potencia Media Adjudicada (kW)',
                PPA as 'Periodos de Potencia Adjudicada',
                PeriodosEnergiaA as 'Periodos de Energía Asignada',
                round(If (t1.PotenciaAdjudicada > 0, round((CostoPotencia) / PotenciaAdjudicada, 3), 0) * If (PPA > 0, round(PotenciaAdjudicada / (PPA), 0), 0)  * PPA,2) as 'Costo Total Potencia (USD)',
                round(round((EnergiaAsignada), 0) * If (t1.EnergiaAsignada > 0, round((CostoEnergia) / EnergiaAsignada, 5), 0), 2) as 'Costo Total Energía (USD)',
                round(CostoPotencia+CostoEnergia,2) as 'Costo Total (USD)'

                FROM
                (SELECT
                ID_Oferente, 
                sum(EnergiaAsignada) As EnergiaAsignada,
                sum(If(PotenciaAdjudicada > 0, If(left(id_Contrato,2) ='SE',0,1),0)) as PPA,
                sum(PotenciaAdjudicada) * 1000 As PotenciaAdjudicada,
                count(if(EnergiaAsignada>0,1,0)) as PeriodosEnergiaA,
                sum(if(left(ID_Contrato,2) <> 'SE', CostoPotencia, 0))  As CostoPotencia, 
                sum(CostoEnergia) As CostoEnergia
                From mod_oferentemes Where PotenciaAdjudicada + EnergiaAsignada > 0
                Group By id_oferente
                ) as t1 
                inner Join mod_oferente as t2
                On t1.ID_Oferente=t2.id_oferente
                inner join
                (select ID_Oferente, avg(PotenciaMaxima) as PotenciaOfertada from mod_oferentemes where PotenciaMaxima > 0 group by ID_Oferente) t3 on t1.ID_Oferente = t3.ID_Oferente 
                group by t1.ID_Oferente
                union

                (SELECT 'OVRESTO', 'Oferente Virtual Resto','','','', 
                sum(round(PrecioPotOV,3)), 
                sum(round(PrecioEneOV*1000,5)), sum(EnergOV*365.25/12), 0, sum(round(PotMediaAdj,0)), sum(PerPotAdj), sum(PeriodosEnergiaA), 
                round(sum(PrecioPotOV) * sum(PotMediaAdj) * sum(PerPotAdj),2) as CostoTotPot, 
                round(sum(PrecioEneOV * 1000) * sum(EnergOV*365.25/12), 2) as CostoTotEne, 
                round(sum(PrecioPotOV) * sum(PotMediaAdj) * sum(PerPotAdj) + sum(PrecioEneOV*1000) * sum(EnergOV*365.25/12), 2) as CostoTot
                from(select 
                avg(PotenciaAdjudicadaOVResto) * 1000 as PotMediaAdj,
                count(PotenciaAdjudicadaOVResto) as PerPotAdj,
                avg(PrecioPotenciaOferenteVirtualResto) as PrecioPotOV,
                '' as PrecioEneOV, '' as EnergOV, '' as PeriodosEnergiaA
                From mod_licitacionmes where PotenciaAdjudicadaOVResto > 0
                
                union
                select '' PotMediaAdj, '' as PerPotAdj, '' as PrecioPotOV,
                avg(PrecioEnergiaOferenteVirtual) as PrecioEneOV, '' as EnergOV, '' as PeriodosEnergiaA
                From mod_licitacionmes k1
                inner join (select * from mod_licitacionperfil where PerfilAsignadoOV > 0) k2 on k1.ID_Mes = k2.ID_Mes
                union
                select '', '', '', '', round(sum(PerfilAsignadoOV),2) as EnergOV, count(distinct ID_Mes) as PeriodosEnergiaA 
                FROM mod_licitacionperfil where PerfilAsignadoOV > 0
                )as t4 )
                union

                SELECT 'OVSP', 'Oferente Virtual SP','','','', 
                sum(round(PrecioPotOV,3)), 
                sum(round(PrecioEneOV*1000,5)), sum(EnergOV*365.25/12), 0, sum(round(PotMediaAdj,0)), sum(PerPotAdj), sum(PeriodosEnergiaA), 
                round(sum(PrecioPotOV) * sum(PotMediaAdj) * sum(PerPotAdj),2) as CostoTotPot, 
                round(sum(PrecioEneOV * 1000) * sum(EnergOV*365.25/12), 2) as CostoTotEne, 
                round(sum(PrecioPotOV) * sum(PotMediaAdj) * sum(PerPotAdj) + sum(PrecioEneOV*1000) * sum(EnergOV*365.25/12), 2) as CostoTot
                from(select 
                avg(PotenciaAdjudicadaOVSP) * 1000 as PotMediaAdj,
                count(PotenciaAdjudicadaOVSP) as PerPotAdj,
                avg(PrecioPotenciaOferenteVirtualSP) as PrecioPotOV,
                '' as PrecioEneOV, '' as EnergOV, '' as PeriodosEnergiaA
                From mod_licitacionmes where PotenciaAdjudicadaOV > 0)as t4 


                union
                (SELECT 'Total Neto OV','','','','',
                If (PotenciaAdjudicada > 0, round((CostoPotencia) / PotenciaAdjudicada, 3), 0)  As 'Precio Potencia (USD/kW-Mes)',
                If (EnergiaAsignada > 0, round((CostoEnergia) / EnergiaAsignada, 5), 0)  As 'Precio Energia (USD/MWh)' ,
                round((EnergiaAsignada), 0) As 'Energia Asignada (MWh)',
                if (CantidadMeses>0,round(((PotenciaOfertada/CantidadMeses)),0),0),
                if (CantidadMeses>0,round(((PotenciaAdjudicada/CantidadMeses)),0),0),
                CantidadMeses, CantidadMeses,
                round(If (PotenciaAdjudicada > 0, round((CostoPotencia) / PotenciaAdjudicada, 3), 0) * if (CantidadMeses>0,round(((PotenciaAdjudicada/CantidadMeses)),0),0) * CantidadMeses,2) as CostoPotenciaTotal,
                round(round((EnergiaAsignada), 0) * If (EnergiaAsignada > 0, round((CostoEnergia) / EnergiaAsignada, 5), 0),2) as CostoEnergiaTotal,
                round((CostoPotencia + CostoEnergia), 2)  As 'Costo Total (USD)'
                FROM (
                (
                select '',sum(EnergiaAsignada) As EnergiaAsignada,
                sum(if(left(ID_Contrato,2) <> 'SE', PotenciaMaxima     * 1000, 0))  As PotenciaOfertada,                
                sum(if(left(ID_Contrato,2) <> 'SE', PotenciaAdjudicada * 1000, 0))  As PotenciaAdjudicada,
                count(if(EnergiaAsignada>0,1,0)) as PeriodosEnergiaA,
                sum(if(left(ID_Contrato,2) <> 'SE', CostoPotencia, 0))  As CostoPotencia, 
                sum(CostoEnergia) As CostoEnergia
                From mod_oferentemes Where CostoPotencia + CostoEnergia > 0
                ) as t1,
                (SELECT count(mes) as CantidadMeses from mod_licitacionmes where (PotenciaAdjudicadaOV<PotenciaLicitacion)
                ) as t0
                )
                )
                order by Id_Oferente, 'Costo Total (USD)', 'Precio Potencia (USD/kW-Mes)', 'Precio Energia (USD/MWh)'"

        'If (EnergiaAsignada > 0, round((CostoPotencia + CostoEnergia) / EnergiaAsignada, 2), 0)  As 'Precio Monómico (USD/MWh)'

        MySqlBase.ActualizarDB(Query, True)

    End Sub

    Private Sub GenerarMod_Adjudicados(model As GRBModel, ByRef data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Tiempo As DateTime, ResultadosForz As Boolean, CorrerModoForzado As Boolean)
        Dim ID_Oferente As Integer
        Dim ID_AES As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer
        Dim temp As String
        Dim AE As Integer

        'de cada año estacional, devuelve el primer mes 
        Dim ID_MesDic As New Dictionary(Of Integer, Integer)
        Dim ID_AE As New Dictionary(Of Integer, Integer)
        Dim DiasMes As New Dictionary(Of Integer, Integer)
        Dim PerfilOferente As New Dictionary(Of String, Double)
        Dim PotMax As New Dictionary(Of String, Double)
        Dim PotMin As New Dictionary(Of String, Double)

        Dim Mod_Adjudicados As New DataTable
        Mod_Adjudicados.TableName = "Mod_Adjudicados"
        Call CargarCol(Mod_Adjudicados, "Nombre", GetType(System.String))
        Call CargarCol(Mod_Adjudicados, "AE", GetType(System.Int32))
        Call CargarCol(Mod_Adjudicados, "Seleccionado", GetType(System.Int32))
        Call CargarCol(Mod_Adjudicados, "PGMx", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "PGMn", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "PMedAdj", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "Energia", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "PorcentajeReduccion", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "PorcentajeParaTotal", GetType(System.Double))
        Call CargarCol(Mod_Adjudicados, "MonomicoTot", GetType(System.Double))

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Rows
            temp = CInt(i("ID_Oferente")) & "_" & CInt(i("ID_Mes"))
            PotMax.Add(temp, CDbl(i("PotenciaMaxima")))
            PotMin.Add(temp, CDbl(i("PotenciaMinima")))
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            AE = CInt(i("AE"))
            If Not ID_MesDic.ContainsKey(AE) Then
                ID_MesDic.Add(AE, CInt(i("ID_Mes")))
            End If
            ID_AE.Add(CInt(i("ID_Mes")), AE)
            DiasMes.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
        Next i

        For Each i As DataRow In data.Tables("Mod_OferentePerfil").Rows
            temp = CInt(i("ID_Oferente")) & "_" & ID_AE(CInt(i("ID_Mes")))
            If Not PerfilOferente.ContainsKey(temp) Then
                PerfilOferente.Add(temp, 0)
            End If
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            PerfilOferente(temp) = PerfilOferente(temp) + Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora).X * DiasMes(CInt(i("ID_Mes")))
        Next

        For Each i As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            For Each a As DataRow In data.Tables("IDAniosEstacionales").Rows
                AE = CInt(a("AE"))
                ID_AES = Dicc("AE:" & AE)
                ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
                Dim row As DataRow = Mod_Adjudicados.NewRow
                row("Nombre") = CStr(i("Nombre"))
                row("AE") = AE
                row("Seleccionado") = Vars.Item(NV("VE"))(ID_Oferente, ID_AES).X

                temp = CInt(i("ID_Oferente")) & "_" & ID_MesDic(AE)
                row("PGMx") = PotMax(temp)
                row("PGMn") = PotMin(temp)
                row("PMedAdj") = Vars.Item(NV("P"))(ID_Oferente, ID_AES).X

                temp = CInt(i("ID_Oferente")) & "_" & AE
                row("Energia") = PerfilOferente(temp)
                row("PorcentajeReduccion") = 0
                row("PorcentajeParaTotal") = 0
                row("MonomicoTot") = 0
                Mod_Adjudicados.Rows.Add(row)
            Next
        Next

        'Insertar ultima fila con Monomico Total
        Dim Dias As New Dictionary(Of Integer, Integer)
        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            Dias.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
        Next i


        Dim rowmt As DataRow = Mod_Adjudicados.NewRow
        Dim CostoTotal As Double = 0
        Dim EnergiaTotal As Double = 0
        Mod_Adjudicados.Rows.Add(rowmt)
        rowmt("Nombre") = "MonomicoTot"
        rowmt("AE") = 0
        rowmt("Seleccionado") = 0
        rowmt("PGMx") = 0
        rowmt("PGMn") = 0
        rowmt("PMedAdj") = 0
        rowmt("Energia") = 0
        rowmt("PorcentajeReduccion") = 0
        rowmt("PorcentajeParaTotal") = 0

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Rows
            CostoTotal = CostoTotal + CDbl(i("CostoEnergia")) 'es costo total, es decir que está multiplicado por los días
            CostoTotal = CostoTotal + CDbl(i("CostoPotencia"))
        Next i

        For Each i As DataRow In data.Tables("mod_licitacionmes").Rows
            CostoTotal = CostoTotal + CDbl(i("PrecioPotenciaOferenteVirtualResto")) * 1000 * CDbl(i("PotenciaAdjudicadaOVResto"))
            CostoTotal = CostoTotal + CDbl(i("PrecioPotenciaOferenteVirtualSP")) * 1000 * CDbl(i("PotenciaAdjudicadaOVSP"))

            For Each j As DataRow In data.Tables("mod_licitacionperfil").Select("ID_Mes=" & CStr(i("ID_Mes")))
                CostoTotal = CostoTotal + CDbl(i("PrecioEnergiaOferenteVirtual")) * 1000 * CDbl(j("PerfilAsignadoOV")) * Dias(CInt(j("ID_Mes")))
            Next j

        Next i

        For Each i As DataRow In data.Tables("mod_licitacionperfil").Rows
            For Each j As DataRow In data.Tables("mod_licitacionmes").Select("ID_Mes='" & CInt(i("ID_Mes")) & "'")
                EnergiaTotal = EnergiaTotal + CDbl(i("PerfilPorcentual")) * CDbl(j("PotenciaConEnergiaAsociada")) * Dias(CInt(i("ID_Mes")))
            Next j
        Next i
        rowmt("MonomicoTot") = CostoTotal / EnergiaTotal
        '||||||||||||||||||||||||||Fin costo monómico

        MySqlBase.ActualizarDB("delete from Mod_Adjudicados")
        Call MySqlBase.GenerarInsertsFromTable2("Mod_Adjudicados", Mod_Adjudicados)

        data.Tables.Add(Mod_Adjudicados) ' cargo mod_adjudicados en data, para desp agregarles los % de reduccion

        Call CalculoSegundos("Crear Tabla GenerarMod_OferenteMes", Tiempo)
    End Sub

    Private Sub GenerarMod_OferenteMes(model As GRBModel, data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_Mes As Integer
        Dim ID_AE As Integer
        Dim DiasMes As New Dictionary(Of Integer, Integer)
        Dim PerfilOferente As New Dictionary(Of String, Double)
        Dim Temp As String
        Dim ID_Hora As Integer

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            DiasMes.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
        Next
        For Each i As DataRow In data.Tables("Mod_OferentePerfil").Rows
            Temp = CInt(i("ID_Oferente")) & "_" & CInt(i("ID_Mes"))
            If Not PerfilOferente.ContainsKey(Temp) Then
                PerfilOferente.Add(Temp, 0)
            End If
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            PerfilOferente(Temp) = PerfilOferente(Temp) + Math.Round(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora).X * DiasMes(CInt(i("ID_Mes"))), 6)
        Next

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            Temp = CInt(i("ID_Oferente")) & "_" & CInt(i("ID_Mes"))
            i("PotenciaAdjudicada") = Math.Round(Vars.Item(NV("P"))(ID_Oferente, ID_AE).X, 6)
            i("EnergiaAsignada") = PerfilOferente(Temp)
            i("CostoPotencia") = CDbl(i("PotenciaAdjudicada")) * CDbl(i("PrecioPotencia")) * 1000
            i("CostoEnergia") = CDbl(i("EnergiaAsignada")) * CDbl(i("PrecioEnergia")) * 1000
        Next

        MySqlBase.ActualizarDB("delete from Mod_OferenteMes")
        Call MySqlBase.GenerarInsertsFromTable2("Mod_OferenteMes", data.Tables("Mod_OferenteMes"))
        Call CalculoSegundos("Crear Tabla GenerarMod_OferenteMes", Tiempo)
    End Sub

    Private Sub GenerarMod_LicitacionMes(model As GRBModel, data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Tiempo As DateTime)
        Dim ID_OferenteV As Integer
        Dim ID_AE As Integer

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_OferenteV = 0
            i("PotenciaAdjudicadaOVResto") = Math.Round(Vars.Item(NV("POVRESTO"))(ID_OferenteV, ID_AE).X, 6)
            i("PotenciaAdjudicadaOVSP") = Math.Round(Vars.Item(NV("POVSP"))(ID_OferenteV, ID_AE).X, 6)
            i("PotenciaAdjudicadaOV") = CDbl(i("PotenciaAdjudicadaOVSP")) + CDbl(i("PotenciaAdjudicadaOVResto"))
        Next

        MySqlBase.ActualizarDB("delete from Mod_LicitacionMes")
        Call MySqlBase.GenerarInsertsFromTable2("Mod_LicitacionMes", data.Tables("Mod_LicitacionMes"))
        Call CalculoSegundos("Crear Tabla GenerarMod_LicitacionMes", Tiempo)
    End Sub

    Private Sub GenerarMod_OferentePerfil(model As GRBModel, data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer

        For Each i As DataRow In data.Tables("Mod_OferentePerfil").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            i("PerfilAsignado") = Math.Round(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora).X, 6)
        Next

        MySqlBase.ActualizarDB("delete from Mod_OferentePerfil")
        Call MySqlBase.GenerarInsertsFromTable2("Mod_OferentePerfil", data.Tables("Mod_OferentePerfil"))
        Call CalculoSegundos("Crear Tabla GenerarMod_OferentePerfil:", Tiempo)
    End Sub

    Private Sub GenerarMod_LicitacionPerfil(model As GRBModel, data As DataSet, MySqlBase As MySqlBase, ByVal Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Tiempo As DateTime)
        Dim ID_OferenteV As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer

        For Each i As DataRow In data.Tables("Mod_LicitacionPerfil").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_OferenteV = 0
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            i("PerfilAsignadoOV") = Math.Round(Vars2.Item(NV("EOV"))(ID_OferenteV, ID_Mes, ID_Hora).X, 6)
        Next
        MySqlBase.ActualizarDB("delete from Mod_LicitacionPerfil")
        Call MySqlBase.GenerarInsertsFromTable2("Mod_LicitacionPerfil", data.Tables("Mod_LicitacionPerfil"))
        Call CalculoSegundos("Crear Tabla GenerarMod_LicitacionPerfil:", Tiempo)
    End Sub

    Public Sub GenerarMod_OferenteEsc(model As GRBModel,
                                     data As DataSet,
                                     MySqlBase As MySqlBase,
                                     ByVal Vars As List(Of GRBVar(,)),
                                     ByRef Vars2 As List(Of GRBVar(,,)),
                                     NV As Dictionary(Of String, Int32),
                                     Dicc As Dictionary(Of String, Int32),
                                     ByRef Tiempo As DateTime,
                                     ResultadosForz As Boolean,
                                     CorrerModoForzado As Boolean)
        Dim ID_Oferente As Integer

        If Not ResultadosForz And CorrerModoForzado Then
            For Each i As DataRow In data.Tables("Mod_Oferente").Rows
                ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
                i("Adjudicado") = Vars.Item(NV("VET"))(ID_Oferente, 0).X
            Next
        End If
        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_oferente"
        Call CargarCol(Tabla, "ID_Oferente", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Nombre", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb1", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb2", GetType(System.String))
        Call CargarCol(Tabla, "ID_Contrato", GetType(System.String))
        Call CargarCol(Tabla, "Considerar", GetType(System.Int32))

        If CorrerModoForzado Then
            Call CargarCol(Tabla, "Adjudicado", GetType(System.Int32))
            Call CargarCol(Tabla, "Forzado", GetType(System.Int32))
            Call CargarCol(Tabla, "CostoTotalOferente", GetType(System.Double))
            Call CargarCol(Tabla, "SobreCostoFO", GetType(System.Double))

        End If

        MySqlBase.ActualizarDB("drop table Mod_Oferente")
        MySqlBase.CreateTable(Tabla)

        Call MySqlBase.GenerarInsertsFromTable2("Mod_Oferente", data.Tables("Mod_Oferente"))
        Call CalculoSegundos("Crear Tabla GenerarMod_Oferente", Tiempo)

    End Sub

    Public Sub AgregarReduccionModAdjudicados(model As GRBModel,
                                     data As DataSet,
                                     MySqlBase As MySqlBase,
                                     ByVal Vars As List(Of GRBVar(,)),
                                     ByRef Vars2 As List(Of GRBVar(,,)),
                                     NV As Dictionary(Of String, Int32),
                                     Dicc As Dictionary(Of String, Int32),
                                     ByRef Tiempo As DateTime,
                                     ResultadosForz As Boolean,
                                     CorrerModoForzado As Boolean)
        Dim porcRed As Double
        Dim Reduccion As New Dictionary(Of String, Double)
        Dim Adjudicado As New Dictionary(Of String, Boolean)

        For Each f2 As DataRow In data.Tables("Mod_Oferente").Rows
            If CDbl(f2("Adjudicado")) = 1 Then
                Adjudicado.Add(CStr(f2("Nombre")), True)
            Else
                Adjudicado.Add(CStr(f2("Nombre")), False)
            End If
            If CDbl(f2("CostoTotalOferente")) > 0 Then
                porcRed = CDbl(f2("SobreCostoFO")) / CDbl(f2("CostoTotalOferente"))
            Else
                porcRed = 0
            End If
            Reduccion.Add(CStr(f2("Nombre")), porcRed)
        Next f2

        For Each f1 As DataRow In data.Tables("Mod_Adjudicados").Select("Nombre<>'MonomicoTot'")
            If Adjudicado(CStr(f1("Nombre"))) Then
                f1("PorcentajeReduccion") = 0
                f1("PorcentajeParaTotal") = Math.Round(100 * Reduccion(CStr(f1("Nombre"))), 1)
            Else
                f1("PorcentajeReduccion") = Math.Round(100 * Reduccion(CStr(f1("Nombre"))), 1)
                f1("PorcentajeParaTotal") = Math.Round(100 * Reduccion(CStr(f1("Nombre"))), 1)
                If CDbl(f1("PorcentajeReduccion")) <= 0.1 Then f1("PorcentajeReduccion") = 0.1
                If CDbl(f1("PorcentajeParaTotal")) <= 0.1 Then f1("PorcentajeParaTotal") = 0.1
            End If
        Next

        MySqlBase.ActualizarDB("drop table Mod_Adjudicados")
        MySqlBase.CreateTable(data.Tables("Mod_Adjudicados"))

        Call MySqlBase.GenerarInsertsFromTable2("Mod_Adjudicados", data.Tables("Mod_Adjudicados"))
        Call CalculoSegundos("Crear Tabla Mod_Adjudicados con Porcentaje de Reducción", Tiempo)

    End Sub

End Module
