Option Strict On
Imports Gurobi
Imports MEMLibCommon

Module AgregarVars
    Public Sub AgregarVariables(ByRef model As GRBModel, data As DataSet, cantElem As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef Vars As List(Of GRBVar(,)), ByRef Vars2 As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), ByRef tiempo As DateTime, ByRef tiempoT As DateTime, CorrerModoForzado As Boolean)
        Dim VE As GRBVar(,) = New GRBVar(cantElem("cantOferentes"), cantElem("cantAE")) {}
        Dim VET As GRBVar(,) = New GRBVar(cantElem("cantOferentes"), 0) {}
        Dim P As GRBVar(,) = New GRBVar(cantElem("cantOferentes"), cantElem("cantAE")) {}
        Dim E As GRBVar(,,) = New GRBVar(cantElem("cantOferentes"), cantElem("cantMeses"), cantElem("cantHoras")) {}
        Dim EOV As GRBVar(,,) = New GRBVar(cantElem("cantOferentesV"), cantElem("cantMeses"), cantElem("cantHoras")) {}
        Dim POVSP As GRBVar(,) = New GRBVar(0, cantElem("cantAE")) {}
        Dim POVRESTO As GRBVar(,) = New GRBVar(0, cantElem("cantAE")) {}

        Dim NumElem As Integer = 0
        Dim NumElem2 As Integer = 0

        Call CargarNV(Vars, VE, "VE", NV, NumElem)
        Call CargarNV(Vars, VET, "VET", NV, NumElem)
        Call CargarNV(Vars, P, "P", NV, NumElem)
        Call CargarNV2(Vars2, E, "E", NV, NumElem2)
        Call CargarNV(Vars, POVSP, "POVSP", NV, NumElem)
        Call CargarNV(Vars, POVRESTO, "POVRESTO", NV, NumElem)
        Call CargarNV2(Vars2, EOV, "EOV", NV, NumElem2)

        Call AgregarVarsVE(model, data, Vars, NV, Dicc, tiempo)
        Call AgregarVarsVET(model, data, Vars, NV, Dicc, tiempo, CorrerModoForzado)
        Call AgregarVarsP(model, data, Vars, NV, Dicc, tiempo)
        Call AgregarVarsE(model, data, Vars2, NV, Dicc, tiempo)
        Call AgregarVarsPOV(model, data, Vars, NV, Dicc, tiempo)
        Call AgregarVarsEOV(model, data, Vars2, NV, Dicc, tiempo)

        Call CalculoSegundos("Total Agregar Variables", tiempoT)

    End Sub

    Private Sub CargarNV(ByRef Vars As List(Of GRBVar(,)), var As GRBVar(,), Nombre As String, NV As Dictionary(Of String, Int32), ByRef NumElem As Integer)
        Vars.Add(var)
        NV.Add(Nombre, NumElem)
        NumElem = NumElem + 1
    End Sub

    Private Sub CargarNV2(ByRef Vars As List(Of GRBVar(,,)), var As GRBVar(,,), Nombre As String, NV As Dictionary(Of String, Int32), ByRef NumElem As Integer)
        Vars.Add(var)
        NV.Add(Nombre, NumElem)
        NumElem = NumElem + 1
    End Sub

    Private Sub AgregarVarsVET(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime, CorrerModoForzado As Boolean)
        Dim i As DataRow
        Dim j As DataRow
        Dim ID_Oferente As Integer
        Dim Considerar As Integer
        Dim Forzado As Integer

        For Each i In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CStr(i("ID_Oferente")))
            Considerar = CInt(i("Considerar"))
            If CorrerModoForzado Then
                If CInt(i("Forzado")) = 1 Then
                    Forzado = 1
                Else
                    Forzado = 0
                End If
            Else
                Forzado = 0
            End If

            If Considerar = 0 Then Forzado = 0

            Vars.Item(NV("VET"))(ID_Oferente, 0) = model.AddVar(Forzado, Considerar, 1, GRB.BINARY, "VET_" & ID_Oferente & "_0") 'VET es una variable binaria (0 o 1) e indica si el oferente es seleccionado. Si forzado =1 la variable solo puede ir entre 1 y 1, por lo tanto siempre sera seleccionado 

        Next i

        Call CalculoSegundos("AgregarVarsVE: ", tiempo)
    End Sub


    Private Sub AgregarVarsVE(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim i As DataRow
        Dim j As DataRow
        Dim ID_Oferente As Integer
        Dim ID_AE As Integer
        Dim Considerar As Integer

        For Each i In data.Tables("Mod_Oferente").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CStr(i("ID_Oferente")))
            Considerar = CInt(i("Considerar"))
            For Each j In data.Tables("IDAniosEstacionales").Rows
                ID_AE = Dicc("AE:" & CStr(j("AE")))
                Vars.Item(NV("VE"))(ID_Oferente, ID_AE) =
                    model.AddVar(0, Considerar, 0, GRB.BINARY, "VE_" & ID_Oferente & "_" & ID_AE)
            Next
        Next

        Call CalculoSegundos("AgregarVarsVE: ", tiempo)

    End Sub

    Private Sub AgregarVarsP(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim i As DataRow
        Dim ID_Oferente As Integer
        Dim ID_AE As Integer

        For Each i In data.Tables("Mod_OferenteMes").Select("Mes=9")
            ID_Oferente = Dicc("ID_Oferente:" & CStr(i("ID_Oferente")))
            ID_AE = Dicc("AE:" & CStr(i("AE")))
            If CStr(i("ID_Contrato")).ToUpper = "SE" Then
                i("PrecioPotencia") = 0
            End If
            Vars.Item(NV("P"))(ID_Oferente, ID_AE) =
                model.AddVar(0, CDbl(i("PotenciaMaxima")),
                             CDbl(i("PrecioPotencia")) * 1000 * 12,
                             GRB.CONTINUOUS, "P_" & ID_Oferente & "_" & ID_AE)
        Next i

        Call CalculoSegundos("AgregarVarsP: ", tiempo)
    End Sub

    Private Sub AgregarVarsPOV(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim i As DataRow
        Dim ID_OferenteV As Integer
        Dim ID_AE As Integer
        Dim SPMax As Double = 0

        For Each i In data.Tables("Mod_Parametros").Rows
            If CInt(i("ID_Parametro")) = 2 Then
                SPMax = CDbl(i("Valor"))
            End If
        Next i

        For Each i In data.Tables("Mod_LicitacionMes").Select("Mes=9") ' selecciona 9 por q necesita un mes del año estacional
            ID_OferenteV = 0
            ID_AE = Dicc("AE:" & CStr(i("AE")))

            Vars.Item(NV("POVRESTO"))(ID_OferenteV, ID_AE) =
                model.AddVar(0, CDbl(i("PotenciaLicitacion")) - SPMax,
                             CDbl(i("PrecioPotenciaOferenteVirtualResto")) * 1000 * 12, GRB.CONTINUOUS, "POVRESTO_" & ID_OferenteV & "_" & ID_AE)

            Vars.Item(NV("POVSP"))(ID_OferenteV, ID_AE) =
                model.AddVar(0, SPMax,
                             CDbl(i("PrecioPotenciaOferenteVirtualSP")) * 1000 * 12, GRB.CONTINUOUS, "POVSP_" & ID_OferenteV & "_" & ID_AE)
        Next i

        Call CalculoSegundos("AgregarVarsPOV: ", tiempo)
    End Sub

    Private Sub AgregarVarsE(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim i As DataRow
        Dim ID_Oferente As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer
        Dim P As New Dictionary(Of String, Double)
        Dim PE As New Dictionary(Of String, Double)
        Dim OM As String
        Dim DiasMes As New Dictionary(Of Integer, Integer)

        For Each i In data.Tables("Mod_LicitacionMes").Rows
            DiasMes.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
        Next

        For Each i In data.Tables("Mod_OferenteMes").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CStr(i("ID_Oferente")))
            ID_Mes = Dicc("ID_Mes:" & CStr(i("ID_Mes")))
            If CStr(i("ID_Contrato")).ToUpper = "SP" Then
                i("PrecioEnergia") = 0
            End If
            P.Add(ID_Oferente & "_" & ID_Mes, CDbl(i("PotenciaMaxima")))
            PE.Add(ID_Oferente & "_" & ID_Mes, (CDbl(i("PrecioEnergia"))) * 1000 * DiasMes(CInt(i("ID_Mes"))))
        Next i

        For Each i In data.Tables("Mod_OferentePerfil").Rows
            ID_Oferente = Dicc("ID_Oferente:" & CStr(i("ID_Oferente")))
            ID_Mes = Dicc("ID_Mes:" & CStr(i("ID_Mes")))
            ID_Hora = Dicc("ID_Hora:" & CStr(i("ID_Hora")))
            OM = ID_Oferente & "_" & ID_Mes
            Vars.Item(NV("E"))(ID_Oferente, ID_Mes, ID_Hora) = model.AddVar(0, P(OM), PE(OM), GRB.CONTINUOUS, "E_" & ID_Oferente & "_" & ID_Mes & "_" & ID_Hora)
        Next i

        Call CalculoSegundos("AgregarVarsE: ", tiempo)
    End Sub

    Private Sub AgregarVarsEOV(ByRef model As GRBModel, data As DataSet, ByRef Vars As List(Of GRBVar(,,)), NV As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), ByRef tiempo As DateTime)
        Dim i As DataRow
        Dim ID_OferenteV As Integer
        Dim ID_Mes As Integer
        Dim ID_Hora As Integer
        ' Dim PEOV As Double

        Dim P As New Dictionary(Of Integer, Double)
        Dim PE As New Dictionary(Of Integer, Double)
        Dim DiasMes As New Dictionary(Of Integer, Integer)

        For Each i In data.Tables("Mod_LicitacionMes").Rows
            DiasMes.Add(CInt(i("ID_Mes")), CInt(i("Dias")))
        Next

        For Each i In data.Tables("Mod_LicitacionMes").Rows
            ID_Mes = Dicc("ID_Mes:" & CInt(i("ID_Mes")))
            P.Add(ID_Mes, CDbl(i("PotenciaConEnergiaAsociada")))
            PE.Add(ID_Mes, CDbl(i("PrecioEnergiaOferenteVirtual")) * 1000 * DiasMes(CInt(i("ID_Mes"))))
        Next i

        For Each i In data.Tables("Mod_LicitacionPerfil").Rows
            ID_OferenteV = 0
            ID_Mes = Dicc("ID_Mes:" & CStr(i("ID_Mes")))
            ID_Hora = Dicc("ID_Hora:" & CStr(i("ID_Hora")))
            ' PEOV = CDbl(i("PrecioEnergiaOferenteVirtual")) * 1000 * DiasMes(CInt(i("ID_Mes"))) esto se usaba para hacerlo horario al precio del OV
            Vars.Item(NV("EOV"))(ID_OferenteV, ID_Mes, ID_Hora) = model.AddVar(0, P(ID_Mes), PE(ID_Mes), GRB.CONTINUOUS, "EOV_" & ID_OferenteV & "_" & ID_Mes & "_" & ID_Hora)
        Next i
        Call CalculoSegundos("AgregarVarsEOV: ", tiempo)
    End Sub

End Module
