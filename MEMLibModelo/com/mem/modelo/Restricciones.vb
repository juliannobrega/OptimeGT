Option Strict On
Imports Gurobi
Imports MEMLibCommon

Module ModuloRestricciones

    Public Sub Restricciones(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime, ByRef tiempoT As DateTime, Id_OferenteForzado As Integer, CorrerModoForzado As Boolean)
        Try


            Call EcuacionDeSumaPotencia(model, data, Vars, Vars2, NV, Dicc, tiempo)
            Call EcuacionDePotenciaMin(model, data, Vars, Vars2, NV, Dicc, tiempo)
            Call EcuacionDeSumaEnergia(model, data, Vars, Vars2, NV, Dicc, tiempo)
            Call EcuacionDeEnergiaDCCySE(model, data, Vars, Vars2, NV, Dicc, tiempo)
            Call EcuacionDeEnergiaOC(model, data, Vars, Vars2, NV, Dicc, tiempo)
            Call EcuacionDeVE(model, data, Vars, Vars2, NV, Dicc, tiempo)

            If CorrerModoForzado Then Call EcuacionForz(model, data, Vars, Vars2, NV, Dicc, tiempo, Id_OferenteForzado) 'para forzar a los oferentes haciendo pot adjudicada=pot max

            Call CalculoSegundos("Total Restricciones", tiempoT)

        Catch ex As Exception
            Log4NetError("Restricciones", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("Restricciones")
        End Try
    End Sub

    Private Sub EcuacionForz(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime, Id_OferenteForzado As Integer)
        Dim ID_AE As Integer
        Dim ID_Oferente As Integer
        Dim PMx As Double

        For Each j In data.Tables("Mod_OferenteMes").Select("ID_Oferente=" & Id_OferenteForzado)
            ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
            ID_AE = Dicc("AE:" & CStr(j("AE")))
            PMx = CDbl(j("PotenciaMaxima"))
            model.AddConstr(Vars.Item(NV("P"))(ID_Oferente, ID_AE) = PMx, "EqPotMxForz_" & ID_Oferente & "_" & ID_AE)
        Next j

        Call CalculoSegundos("Ecuacion Oferente Forzado:", tiempo)
    End Sub

    Private Sub EcuacionDeVE(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_AE As Integer
        Dim flag As Boolean = False

        For Each j As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
            For Each a As DataRow In data.Tables("IDAniosEstacionales").Rows
                ID_AE = Dicc("AE:" & CStr(a("AE")))
                model.AddConstr(Vars.Item(NV("VE"))(ID_Oferente, ID_AE) = Vars.Item(NV("VET"))(ID_Oferente, 0), "VET_" & ID_Oferente & "_0")
            Next a
        Next j

    End Sub


    Private Sub EcuacionDeEnergiaOC(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_Mes As Integer
        Dim ID_AE As Integer
        Dim ID_Hora As Integer
        Dim ID_Contrato As New Dictionary(Of Integer, String)

        For Each i As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Contrato.Add(ID_Oferente, CStr(i("ID_Contrato")))
        Next i

        For Each i As DataRow In data.Tables("Mod_OferentePerfil").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            If ID_Contrato(ID_Oferente) = "OC" Or ID_Contrato(ID_Oferente) = "DCC_OC" Then
                model.AddConstr(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora) <=
                                Vars.Item(NV("P"))(ID_Oferente, ID_AE),
                                "EqOC_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
            End If
        Next




        Call CalculoSegundos("EcuacionDeEnergiaOC:", tiempo)
    End Sub

    Private Sub EcuacionDeEnergiaDCCySE(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_AE As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer
        Dim ID_Contrato As New Dictionary(Of Integer, String)
        Dim RenNoRen As New Dictionary(Of String, String)
        Dim temp As String

        For Each i As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Contrato.Add(ID_Oferente, CStr(i("ID_Contrato")))
        Next i

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            temp = ID_Oferente & "|" & ID_Mes
            RenNoRen.Add(temp, CStr(i("RenNoRen")))
        Next i


        For Each i As DataRow In data.Tables("Mod_OferentePerfil").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            If ID_Contrato(ID_Oferente) = "SE" Or ID_Contrato(ID_Oferente) = "DCC" Then
                model.AddConstr(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora) =
                                CDbl(i("PerfilPorcentual")) * Vars.Item(NV("P"))(ID_Oferente, ID_AE),
                                "EqEDCCySE_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
            End If
            If ID_Contrato(ID_Oferente) = "DCC_OC" Then
                temp = ID_Oferente & "|" & ID_Mes
                If RenNoRen(temp) = "Ren" Then
                    model.AddConstr(Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora) =
                                CDbl(i("PerfilPorcentual")) * Vars.Item(NV("P"))(ID_Oferente, ID_AE),
                                "EqEDCCySE_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
                End If
            End If
        Next

        Call CalculoSegundos("EcuacionDeEnergiaDCCySE:", tiempo)

    End Sub

    Private Sub EcuacionDePotenciaMin(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_AE As Integer
        Dim PotenciaMin As Double = 0

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            PotenciaMin = CDbl(i("PotenciaMinima"))

            model.AddConstr(Vars.Item(NV("P"))(ID_Oferente, ID_AE) >=
                            PotenciaMin * Vars.Item(NV("VE"))(ID_Oferente, ID_AE),
                            "EqPotMin_" & ID_Oferente & "_" & ID_AE)
        Next i

        For Each i As DataRow In data.Tables("Mod_OferenteMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            model.AddConstr(Vars.Item(NV("P"))(ID_Oferente, ID_AE) <=
                            CDbl(i("PotenciaMaxima")) * Vars.Item(NV("VE"))(ID_Oferente, ID_AE),
                            "EqPotMax_" & ID_Oferente & "_" & ID_AE)
        Next i

        Call CalculoSegundos("EcuacionDePotenciaMin:", tiempo)

    End Sub

    Private Sub EcuacionDeSumaPotencia(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_OferenteV As Integer
        Dim ID_AE As Integer
        Dim PL As Double
        Dim ID_Contrato As New Dictionary(Of Integer, String)
        Dim DCCyOCMax As Double = 1000000
        Dim SPMax As Double = 1000000
        Dim PrimAE As Integer
        Dim primAELicit As Integer = 2999
        Dim DCCMax As Double = 1000000
        Dim EGMax As Double = 1000000
        Dim FlexPot As Integer = 0


        For Each i As DataRow In data.Tables("Mod_Parametros").Rows
            If CInt(i("ID_Parametro")) = 1 Then
                DCCyOCMax = CDbl(i("Valor"))
            End If
            If CInt(i("ID_Parametro")) = 2 Then
                SPMax = CDbl(i("Valor"))
            End If
            If CInt(i("ID_Parametro")) = 5 Then
                DCCMax = CDbl(i("Valor"))
            End If
            If CInt(i("ID_Parametro")) = 6 Then
                EGMax = CDbl(i("Valor"))
            End If
            If CInt(i("ID_Parametro")) = 7 Then
                FlexPot = CInt(i("Valor"))
            End If

        Next i

        For Each i As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Contrato.Add(ID_Oferente, CStr(i("ID_Contrato")))
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            Dim ntot As GRBLinExpr = 0
            Dim ntotsp As GRBLinExpr = 0
            For Each j In data.Tables("Mod_OferenteMes").Select("ID_Mes=" & CInt(i("ID_Mes")))
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                If ID_Contrato(ID_Oferente) <> "SE" And ID_Contrato(ID_Oferente) <> "OCE" And ID_Contrato(ID_Oferente) <> "SP" Then
                    ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
                End If
                If ID_Contrato(ID_Oferente) = "SP" Then
                    ntotsp.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
                End If
            Next j

            ID_OferenteV = 0
            ntot.AddTerm(1.0, Vars.Item(NV("POVRESTO"))(ID_OferenteV, ID_AE))
            ntotsp.AddTerm(1.0, Vars.Item(NV("POVSP"))(ID_OferenteV, ID_AE))

            PL = CDbl(i("PotenciaLicitacion")) - SPMax
            model.AddConstr(ntot = PL, "EqPotres_" & ID_AE)
            model.AddConstr(ntotsp = SPMax, "EqPotsp_" & ID_AE)
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            If primAELicit > CInt(i("AE")) Then
                primAELicit = CInt(i("AE"))
            End If
        Next

        If FlexPot = 0 Then
            For Each i As DataRow In data.Tables("Mod_Oferente").Rows
                ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
                PrimAE = Dicc("AE:" & primAELicit)
                For Each a As DataRow In data.Tables("IDAniosEstacionales").Rows
                    ID_AE = Dicc("AE:" & CStr(a("AE")))
                    If Dicc("AE:" & CInt(a("AE"))) > PrimAE Then
                        Dim ntot As GRBLinExpr = 0
                        ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, PrimAE))
                        ntot.AddTerm(-1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
                        model.AddConstr(ntot = 0, "RestFlexPot_" & ID_Oferente & "_" & ID_AE)
                    End If
                Next a
            Next i
        End If


        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            Dim ntot As GRBLinExpr = 0
            ID_Oferente = -1
            For Each j In data.Tables("Mod_OferenteMes").Select("ID_Mes=" & CInt(i("ID_Mes")) & " and (ID_Contrato='DCC' or ID_Contrato='OC')")
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
            Next j
            If ID_Oferente >= 0 Then model.AddConstr(ntot <= DCCyOCMax, "EqPotDCCyOCMx_" & "_" & ID_AE)
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            Dim ntot As GRBLinExpr = 0
            ID_Oferente = -1
            For Each j In data.Tables("Mod_OferenteMes").Select("ID_Mes=" & CInt(i("ID_Mes")) & " and (ID_Contrato='DCC')")
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
            Next j
            If ID_Oferente >= 0 Then model.AddConstr(ntot <= DCCMax, "EqPotDCCMx_" & "_" & ID_AE)
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            Dim ntot As GRBLinExpr = 0
            ID_Oferente = -1
            For Each j In data.Tables("Mod_OferenteMes").Select("ID_Mes=" & CInt(i("ID_Mes")) & " and (ID_Contrato='SE')")
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
            Next j
            If ID_Oferente >= 0 Then model.AddConstr(ntot <= EGMax, "EqPotEGMx_" & "_" & ID_AE)
        Next i


        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Select("Mes=9")
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            Dim ntot As GRBLinExpr = 0
            ID_Oferente = -1
            For Each j In data.Tables("Mod_OferenteMes").Select("ID_Mes=" & CInt(i("ID_Mes")) & " and ID_Contrato='SP'")
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                ntot.AddTerm(1.0, Vars.Item(NV("P"))(ID_Oferente, ID_AE))
            Next j
            If ID_Oferente >= 0 Then model.AddConstr(ntot <= SPMax, "EqPotSPMx_" & "_" & ID_AE)
        Next i


        Call CalculoSegundos("EcuacionDeSumaPotencia:", tiempo)

    End Sub

    Private Sub EcuacionDeSumaEnergia(ByRef model As GRBModel, data As DataSet, Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim ID_Oferente As Integer
        Dim ID_OferenteV As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer
        Dim ID_AE As Integer
        Dim EL As Double
        Dim PL As New Dictionary(Of Integer, Double)
        Dim ID_Contrato As New Dictionary(Of Integer, String)

        'Parametro q define si la equición de EE debe ser igual o mayor o igual
        Dim EEEqual As Boolean = True
        Dim OVasOC As Boolean = True

        For Each i As DataRow In data.Tables("Mod_Parametros").Rows
            If CInt(i("ID_Parametro")) = 3 Then
                If CDbl(i("Valor")) = 1 Then EEEqual = True Else EEEqual = False
            End If
            If CInt(i("ID_Parametro")) = 4 Then
                If CDbl(i("Valor")) = 1 Then OVasOC = True Else OVasOC = False
            End If
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionMes").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            PL.Add(ID_Mes, CDbl(i("PotenciaConEnergiaAsociada")))
        Next i

        For Each i As DataRow In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CInt(i("ID_Oferente")))
            ID_Contrato.Add(ID_Oferente, CStr(i("ID_Contrato")))
        Next i

        For Each i As DataRow In data.Tables("Mod_LicitacionPerfil").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            ID_Hora = Dicc("ID_Hora:" & CInt(i("ID_Hora")))
            ID_AE = Dicc("AE:" & CInt(i("AE")))
            EL = CDbl(i("PerfilPorcentual")) * PL(ID_Mes)

            Dim ntot As GRBLinExpr = 0
            For Each j As DataRow In data.Tables("Mod_OferentePerfil").Select("ID_Mes=" & CInt(i("ID_Mes")) & " and ID_Hora=" & CInt(i("ID_Hora")))
                ID_Oferente = Dicc("ID_Oferente:" & CInt(j("ID_Oferente")))
                If ID_Contrato(ID_Oferente) <> "SP" Then
                    ntot.AddTerm(1.0, Vars2.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora))
                End If
            Next j
            ID_OferenteV = 0
            ntot.AddTerm(1.0, Vars2.Item(NV("EOV"))(ID_OferenteV, ID_Mes, ID_Hora))

            If EEEqual Then
                model.AddConstr(ntot = EL, "EqEne_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
            Else
                model.AddConstr(ntot >= EL, "EqEne_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
            End If

            If OVasOC Then
                model.AddConstr(Vars2.Item(NV("EOV"))(ID_OferenteV, ID_Mes, ID_Hora) <=
                                Vars.Item(NV("POVRESTO"))(ID_OferenteV, ID_AE), "OVasOC_" & ID_OferenteV & "_" & ID_Mes & "_" & ID_Hora)
            End If
        Next i

        Call CalculoSegundos("EcuacionDeSumaEnergia:", tiempo)
    End Sub

End Module
