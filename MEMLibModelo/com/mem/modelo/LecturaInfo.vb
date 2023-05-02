Option Strict On
Imports MySql.Data.MySqlClient
Imports MEMLibCommon

Module Lecturas

    Public Sub ImportarDatosRonda(ByRef data As DataSet, MySqlBase As MySqlBase, ByRef tiempo As DateTime, ByRef Bandera As Boolean, database As String,
                                  server As String, password As String, user As String)

        'FALTARIA AGREGAR CODIGO PARA QUE SEA EL NUMERO DE RONDA
        Dim NroRonda As Integer = 0

        'traigo los resultados de la ronda ultima procesada
        MySqlBase.FillAdapterMultiple(data, "gq_ofertas", "SELECT Id, RondaNumero, OferenteId, IdContrato, Ctung, CoMnr, CoMr, Peo, Ppo, Ci, Citt, Cem, Cte, Pcal, Fagn, Fpnr, Fpr, F, K, Estado FROM gq_ofertas
                                                where RondaNumero = " & NroRonda)

        Call CalculoSegundos("Importar ronda", tiempo)
        Auxiliares.Evento("FIN --- ImportarDatosRonda", Auxiliares.LOG_TYPE_INFO)

    End Sub

    Public Sub LecturaInfo(ByRef data As DataSet, cantElem As Dictionary(Of String, Int32), Dicc As Dictionary(Of String, Int32), DiccTxt As Dictionary(Of String, String), MySqlBase As MySqlBase, ByRef tiempo As DateTime, ByRef Bandera As Boolean, CorrerModoForzado As Boolean)
        Try


            'MySqlBase.connString = Con
            Auxiliares.Evento("INICIO --- LecturaInfo", Auxiliares.LOG_TYPE_INFO)
            Dim Tabla As String
            Dim PrimaryKey As String
            Dim i As DataRow

            MySqlBase.FillAdapterMultiple(data, "Esquema", "SELECT Tabla,PrimaryKey,Tomar FROM Sis_TablasEscritura group by Tabla")


            For Each i In data.Tables("Esquema").Rows
                Tabla = CStr(i("Tabla"))
                If CInt(i("Tomar")) = 1 Then
                    PrimaryKey = CStr(i("PrimaryKey"))
                    MySqlBase.FillAdapterMultiple(data, Tabla, "SELECT * FROM " & Tabla & " group by " & PrimaryKey & " order by " & PrimaryKey)
                End If
            Next

            'Traigo la evolución de k
            MySqlBase.FillAdapterMultiple(data, "mod_evolucion_K", "SELECT * FROM mod_evolucion_K")

            Call LeerTablaPreciosYActualizar(data, MySqlBase, Bandera)


            If CorrerModoForzado Then Call CompletarMod_Oferente(data.Tables("Mod_Oferente"))
            Call ControlConsistencia(data, Bandera)

            Call GenIDCol(data, "Mod_LicitacionMes", "IDAniosEstacionales", "AE")
            Call GenIDCol(data, "Mod_Oferente", "IDOferentes", "ID_Oferente")
            Call GenIDCol(data, "Mod_LicitacionMes", "IDMeses", "ID_Mes")
            Call GenIDCol(data, "Mod_LicitacionPerfil", "IDHoras", "ID_Hora")

            cantElem.Add("cantOferentesV", 1)
            cantElem.Add("cantOferentes", data.Tables("IDOferentes").Rows.Count)
            cantElem.Add("cantMeses", data.Tables("IDMeses").Rows.Count)
            cantElem.Add("cantHoras", data.Tables("IDHoras").Rows.Count)
            cantElem.Add("cantAE", data.Tables("IDAniosEstacionales").Rows.Count)

            Call CargarDicccionarioIDIndex(Dicc, data.Tables("IDOferentes"), "ID_Oferente")
            Call CargarDicccionarioIDIndex(Dicc, data.Tables("IDMeses"), "ID_Mes")
            Call CargarDicccionarioIDIndex(Dicc, data.Tables("IDHoras"), "ID_Hora")
            Call CargarDicccionarioIDIndex(Dicc, data.Tables("IDAniosEstacionales"), "AE")

            Call CalculoSegundos("Lectura", tiempo)

            Auxiliares.Evento("FIN --- LecturaInfo", Auxiliares.LOG_TYPE_INFO)

        Catch ex As Exception
            Log4NetError("LecturaInfo", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("LecturaInfo")
        End Try
    End Sub

    Public Sub LeerTablaPreciosYActualizar(ByRef data As DataSet, MySqlBase As MySqlBase, ByRef Bandera As Boolean)
        Dim DataBase As String = MySqlBase.DatabaseName
        Auxiliares.Evento(DataBase, Auxiliares.LOG_TYPE_INFO)
        Dim query As String = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema ='" & DataBase & "' and table_name = 'tabla_precios'"


        If CInt(MySqlBase.ExecuteScalar(query)) = 1 Then
            MySqlBase.FillAdapterMultiple(data, "Tabla_precios", "SELECT * FROM tabla_precios group by Nombre order by Nombre")
            Call ActualizarModOfModOfMes(data, Bandera)
        End If

    End Sub


    Public Sub ActualizarModOfModOfMes(ByRef data As DataSet, ByRef Bandera As Boolean)
        'primero actualizo mod oferente, con la columna Considerar
        Dim ofertas As DataTable = data.Tables("tabla_precios")
        Dim mod_oferente As DataTable = data.Tables("mod_oferente")

        For Each i As DataRow In ofertas.Rows
            For Each f As DataRow In mod_oferente.Select("Nombre = '" & CStr(i("Nombre")) & "'")
                f("Considerar") = CInt(i("Habilitado_Pujar"))
            Next f
        Next i
        Auxiliares.Evento("FIN --- ActualizarMod_Oferente", Auxiliares.LOG_TYPE_INFO)


        'luego actualizo mod oferente_mes 
        Auxiliares.Evento("INICIO --- ActualizarMod_OferenteMes", Auxiliares.LOG_TYPE_INFO)

        Dim mod_oferentemes As DataTable = data.Tables("mod_oferentemes")
        Dim evolucionK As DataTable = data.Tables("mod_evolucion_K")
        Dim cantidadOferentes As Integer
        Dim aniomin As Integer = 2500
        Dim aniomax As Integer = 2000
        Dim ofe_contrato As New Dictionary(Of String, String)

        For Each i As DataRow In ofertas.Rows
            cantidadOferentes = cantidadOferentes + 1
            If ofe_contrato.ContainsKey(CStr(i("Nombre"))) Then
                Auxiliares.Evento("Existe un oferente repetido en la ronda procesada", Auxiliares.LOG_TYPE_INFO)
                Bandera = False
                Exit Sub
            Else
                ofe_contrato.Add(CStr(i("Nombre")), CStr(i("ID_Contrato")))
            End If
        Next

        For Each r As DataRow In mod_oferentemes.Select("ID_Oferente = 1") 'elijo un of cualqueira para sacar año min y max
            If aniomin > CInt(r("A")) Then
                aniomin = CInt(r("A"))
            End If
            If aniomax < CInt(r("A")) Then
                aniomax = CInt(r("A"))
            End If
        Next

        Call ActualizarMod_OferenteMes(ofertas, mod_oferentemes, evolucionK, mod_oferente, cantidadOferentes, aniomin, aniomax, Bandera)

        Auxiliares.Evento("FIN --- ActualizarconRonda", Auxiliares.LOG_TYPE_INFO)

    End Sub

    Private Sub ActualizarMod_OferenteMes(ofertas As DataTable, ByRef mod_oferentemes As DataTable, evolucionKTabla As DataTable, mod_oferente As DataTable,
                                          cantidadOferentes As Integer, aniomin As Integer, aniomax As Integer, ByRef bandera As Boolean)

        Dim OAM As New List(Of String)
        Dim OAMPMx As New Dictionary(Of String, Double)
        Dim OAMPMn As New Dictionary(Of String, Double)
        Dim OAMEO As New Dictionary(Of String, Double)
        Dim OAMEO2 As New Dictionary(Of String, Double)
        Dim Comb As New Dictionary(Of Integer, String)
        Dim OAMPp As New Dictionary(Of String, Double)
        Dim OAMPp_sinIndex As New Dictionary(Of String, Double)
        Dim OAMPe As New Dictionary(Of String, Double)
        Dim OAMPe_sinIndex As New Dictionary(Of String, Double)
        Dim OC As New Dictionary(Of Int32, String)
        Dim ONo As New Dictionary(Of Int32, String)
        Dim Hoja As String = ""
        Dim temp As String


        Try
            'temp = id_oferente_idmes_anio

            Dim MesNumero As New Dictionary(Of Integer, String)
            Dim evolucionK As New Dictionary(Of String, Double)


            For Each f As DataRow In evolucionKTabla.Rows
                Auxiliares.Evento("INICIO --- INICIO FOR EACH EVOLUCIONK", Auxiliares.LOG_TYPE_INFO)
                evolucionK.Add(CStr(f("Anio")) & ":Bunker", CDbl(f("Bunker")))
                evolucionK.Add(CStr(f("Anio")) & ":Gas Natural", CDbl(f("Gas_Natural")))
                evolucionK.Add(CStr(f("Anio")) & ":Carbon", CDbl(f("Carbon")))
                evolucionK.Add(CStr(f("Anio")) & ":PPi_PP0", CDbl(f("PPi_PP0")))
            Next

            Dim MesesRen As New Dictionary(Of String, String)
            Call MesesRenYNoRen(mod_oferentemes, MesesRen) 'trae un dicc con ofe_mes_anio y ren o noren
            Dim ofe_id As New Dictionary(Of String, Integer)

            For Each f As DataRow In mod_oferente.Rows
                ofe_id.Add(CStr(f("Nombre")), CInt(f("ID_Oferente")))
            Next

            'Calculo los nuevos precios
            For Each f As DataRow In ofertas.Rows
                For Each r As DataRow In mod_oferentemes.Select("ID_Oferente = '" & ofe_id(CStr(f("Nombre"))) & "'")
                    temp = CStr(r("ID_Oferente")) & "_" & CStr(r("ID_Mes")) & "_" & CStr(r("A"))
                    Call CalculoPrecios(CInt(r("ID_Oferente")), CInt(r("AE")), CInt(r("A")), CInt(r("ID_Mes")), aniomin, f, OAMPe_sinIndex, OAMPe, OAMPp_sinIndex, OAMPp, evolucionK, MesesRen(temp), temp, bandera)
                    If bandera = False Then
                        Exit For
                    End If
                Next r
            Next f

            Auxiliares.Evento("INICIO --- ReemplazoValores mod_oferentemes", Auxiliares.LOG_TYPE_INFO)

            'Reemplazo valores
            For Each f As DataRow In mod_oferentemes.Rows
                temp = CStr(f("ID_Oferente")) & "_" & CStr(f("ID_Mes")) & "_" & CStr(f("A"))
                f("PrecioPotencia") = OAMPp(temp)
                f("PrecioPSinIndex") = OAMPp_sinIndex(temp)
                f("PrecioEnergia") = OAMPe(temp)
                f("PrecioESinIndex") = OAMPe_sinIndex(temp)
            Next

            Auxiliares.Evento("FIN --- ReemplazoValores mod_oferentemes", Auxiliares.LOG_TYPE_INFO)

        Catch ex As Exception
            bandera = False
            Auxiliares.Evento("ActualizarMod_OferenteMes " & ex.ToString, Auxiliares.LOG_TYPE_ERROR)
        End Try

    End Sub
    Private Sub MesesRenYNoRen(mod_oferentemes As DataTable, ByRef MesesRen As Dictionary(Of String, String))
        Auxiliares.Evento("INICIO --- entro a mesesrenynoren", Auxiliares.LOG_TYPE_INFO)
        Dim temp As String

        For Each r As DataRow In mod_oferentemes.Rows
            temp = CStr(r("ID_Oferente")) & "_" & CStr(r("ID_Mes")) & "_" & CStr(r("A"))
            MesesRen.Add(temp, "|" & CStr(r("RenNoRen")))
        Next

        Auxiliares.Evento("FIN --- MesesRenNoRen", Auxiliares.LOG_TYPE_INFO)

    End Sub

    Private Sub CalculoPrecios(i As Integer,
                              AE As Integer,
                              AC As Integer,
                              c As Integer,
                              aniomin As Integer,
                              f As DataRow,
                              OAMPe_sinIndex As Dictionary(Of String, Double),
                              OAMPe As Dictionary(Of String, Double),
                              OAMPp_sinIndex As Dictionary(Of String, Double),
                              OAMPp As Dictionary(Of String, Double),
                              evolucionK As Dictionary(Of String, Double),
                              RenNoRen As String,
                              temp As String,
                              ByRef banderaOk As Boolean)

        Dim PrecioESinIndex As Double = 0
        Dim PrecioE As Double = 0

        'CTUNG	O&MNoRen	O&MRen	PEO	PPO	CI	CITT	CEM	CTE	PCAL	FAGN	HorasRen	HorasNoRen	PGMax	PGMin	AnioInicio	FPNoRen	FPRen	F	K	Habilitado_Pujar

        Select Case CStr(f("Id_Contrato")) & RenNoRen
            Case "OCBK|NoRen", "OCMBK|NoRen"
                'USD/kWh       =        BBL/MWh            USD/BBL          USD/MWh        USD/MWh
                PrecioESinIndex = (CDbl(f("Ctung")) * CDbl(f("F")) + CDbl(f("Ci")) + CDbl(f("O&MNoRen"))) / 1000
                PrecioE = (CDbl(f("Ctung")) * (CDbl(f("F")) * evolucionK(AC & ":Bunker")) + CDbl(f("Ci")) + CDbl(f("O&MNoRen")) * evolucionK(AC & ":PPi_PP0")) / 1000

            Case "SP|NoRen"
                PrecioESinIndex = 0
                PrecioE = 0
            Case "OCC1|NoRen", "OCMC1|NoRen"
                'USD/kWh       =        TM/MWh              USD/TM       USD/MWh                  USD/MWh
                PrecioESinIndex = ((CDbl(f("Ctung")) * 4.209 / 100000000) * 1000 * (CDbl(f("F"))) + CDbl(f("Citt")) + CDbl(f("O&MNoRen"))) / 1000
                PrecioE = ((CDbl(f("Ctung")) * 4.209 / 100000000) * 1000 * (CDbl(f("F")) * evolucionK(AC & ":Carbon")) + CDbl(f("Citt")) + CDbl(f("O&MNoRen")) * evolucionK(AC & ":PPi_PP0")) / 1000

            Case "OCC2|NoRen", "OCMC2|NoRen"
                'USD/kWh        =        BTU/kWh               USD/TM          USD/TM           BTU/TM               USD/MWh
                PrecioESinIndex = (CDbl(f("Ctung")) * 1000 * (((CDbl(f("Cem"))) + CDbl(f("Cte"))) / CDbl(f("Pcal"))) + CDbl(f("O&MNoRen"))) / 1000 'el CTUNG esta en kwh
                PrecioE = (CDbl(f("Ctung")) * 1000 * (((CDbl(f("Cem")) * evolucionK(AC & ":Carbon")) + CDbl(f("Cte"))) / CDbl(f("Pcal"))) + CDbl(f("O&MNoRen")) * evolucionK(AC & ":PPi_PP0")) / 1000 'el CTUNG esta en kwh

            Case "OCGN|NoRen", "OCMGN|NoRen"
                'USD/kWh       =        BTU/KWh        USD/MMBTU        USD/MMBTU           USD/MWh              USD/MWh
                PrecioESinIndex = ((CDbl(f("Ctung")) / 1000) * (CDbl(f("F")) + CDbl(f("Fagn"))) + CDbl(f("Ci")) + CDbl(f("O&MNoRen"))) / 1000
                PrecioE = (CDbl(f("Ctung")) / 1000 * ((CDbl(f("F")) * evolucionK(AC & ":Gas Natural")) + CDbl(f("Fagn"))) + CDbl(f("Ci")) + CDbl(f("O&MNoRen")) * evolucionK(AC & ":PPi_PP0")) / 1000

            Case "DCCR|Ren", "OCR|Ren",
                  "OCMBK|Ren", "OCMC1|Ren", "OCMC2|Ren", "OCMGN|Ren"
                'USD/kWh        =        USD/MWh                  USD/MWh
                PrecioESinIndex = (CDbl(f("Peo")) + CDbl(f("O&MRen"))) / 1000
                PrecioE = (CDbl(f("Peo")) + CDbl(f("O&MRen")) * evolucionK(AC & ":PPi_PP0")) / 1000

            Case "EG|Ren"
                'USD/kWh        =        USD/MWh        
                PrecioESinIndex = CDbl(f("Peo")) / 1000
                PrecioE = PrecioESinIndex
            Case Else
                Evento("El siguiente contrato: " & CStr(f("ID_Contrato")) & RenNoRen & ", no tiene definición de precio", Auxiliares.LOG_TYPE_ERROR)
                banderaOk = False
        End Select

        OAMPe_sinIndex.Add(temp, PrecioESinIndex)
        OAMPe.Add(temp, PrecioE)
        OAMPp.Add(temp, CDbl(f("Ppo")) * evolucionK(AC & ":PPi_PP0"))
        OAMPp_sinIndex.Add(temp, CDbl(f("Ppo")))


    End Sub



    Private Sub CompletarMod_Oferente(mod_oferente As DataTable)

        If Not mod_oferente.Columns.Contains("Adjudicado") Then
            Call CargarCol(mod_oferente, "Adjudicado", "System.Int32")
            For Each i As DataRow In mod_oferente.Rows
                i("Adjudicado") = 0
            Next
        End If
        If Not mod_oferente.Columns.Contains("Forzado") Then
            Call CargarCol(mod_oferente, "Forzado", "System.Int32")
            For Each i As DataRow In mod_oferente.Rows
                i("Forzado") = 0
            Next
        End If

        If Not mod_oferente.Columns.Contains("CostoTotalOferente") Then
            Call CargarCol(mod_oferente, "CostoTotalOferente", "System.Double")
            For Each i As DataRow In mod_oferente.Rows
                i("CostoTotalOferente") = 0
            Next
        End If

        If Not mod_oferente.Columns.Contains("SobreCostoFO") Then
            Call CargarCol(mod_oferente, "SobreCostoFO", "System.Double")
            For Each i As DataRow In mod_oferente.Rows
                i("SobreCostoFO") = 0
            Next
        End If




    End Sub

    Private Sub CargarDicccionarioIDIndex(Dicc As Dictionary(Of String, Integer), tabla As DataTable, ID As String)
        Auxiliares.Evento("INICIO --- CargarDicccionarioIDIndex", Auxiliares.LOG_TYPE_INFO)

        Dim i As DataRow
        Dim temp As String
        Dim Num As Integer = 0

        For Each i In tabla.Rows
            temp = ID & ":" & CStr(i(ID))
            Dicc.Add(temp, Num)
            Num = Num + 1
        Next

        Auxiliares.Evento("FIN --- CargarDicccionarioIDIndex", Auxiliares.LOG_TYPE_INFO)
    End Sub

    Private Sub GenIDCol(data As DataSet, tableName As String, TablaRes As String, ID As String)
        Auxiliares.Evento("INICIO --- GenIDCol", Auxiliares.LOG_TYPE_INFO)
        Dim row As DataRow
        Dim temp As New DataTable
        Dim col As New Collection

        Call CargarCol(temp, ID, "System.String")
        For Each i As DataRow In data.Tables(tableName).Rows
            If Not col.Contains(CStr(i(ID))) Then
                col.Add(i(ID), CStr(i(ID)))
            End If
        Next
        For Each c In col
            row = temp.NewRow()
            row(ID) = c
            temp.Rows.Add(row)
        Next
        temp.TableName = TablaRes
        data.Tables.Add(temp)
        Auxiliares.Evento("FIN --- GenIDCol", Auxiliares.LOG_TYPE_INFO)
    End Sub

    Private Sub CargarControlOptimizacion(data As DataSet, Dicc As Dictionary(Of String, Int32))
        Auxiliares.Evento("INICIO --- CargarControlOptimizacion", Auxiliares.LOG_TYPE_INFO)
        Dim i As DataRow
        For Each i In data.Tables("Sup_ControlOptimizacion").Rows
            Dicc.Add("Parametro:" & CStr(i("Parametro")), CInt(i("Valor")))
        Next
        Auxiliares.Evento("FIN --- CargarControlOptimizacion", Auxiliares.LOG_TYPE_INFO)
    End Sub


    Private Sub ControlConsistencia(Data As DataSet, ByRef bandera As Boolean)
        Try


            Dim listaOferentes As New List(Of Integer)
            Dim listaIDMes As New List(Of String)
            Dim contrato As New Dictionary(Of Int32, String)
            Dim listaZonas As New List(Of Integer)
            Dim listaTipos As New List(Of String)
            Dim ID_Mes As Integer = 0

            Call ControlmodParametros(Data.Tables("Mod_Parametros"), bandera)
            Call ControlmodLicitacionmes(Data.Tables("Mod_LicitacionMes"), listaIDMes, bandera, ID_Mes)
            Call ControlModOferente(Data.Tables("Mod_Oferente"), listaOferentes, listaZonas, listaTipos, contrato, bandera, ID_Mes)
            Call ControlmodLicitacionPerfil(Data.Tables("Mod_LicitacionPerfil"), listaIDMes, bandera)
            Call ControlmodOferentemes(Data.Tables("Mod_OferenteMes"), listaOferentes, listaIDMes, contrato, bandera)
            Call ControlmodOferentePerfil(Data.Tables("Mod_OferentePerfil"), listaOferentes, listaIDMes, contrato, bandera)
        Catch ex As Exception
            Log4NetError("ControlConsistencia", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("ControlConsistencia")
        End Try
    End Sub

    Private Sub ControlmodLicitacionPerfil(Tabla As DataTable, listaIDMes As List(Of String), ByRef bandera As Boolean)
        Try


            Dim listaidMes_A_Mes_H As New Dictionary(Of String, String)
            Dim listaidMes_A_Mes_H2 As New Dictionary(Of String, String)
            Dim temp As String = ""
            Dim perfiltemp As Double

            For Each l In listaIDMes
                For h = 1 To 24
                    listaidMes_A_Mes_H.Add(l & "_" & h, l & "_" & h)
                Next
            Next


            For Each f As DataRow In Tabla.Rows
                temp = CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes")) & "_" & CStr(f("ID_Hora"))
                listaidMes_A_Mes_H2.Add(temp, temp)
                If Not listaidMes_A_Mes_H.ContainsKey(temp) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionPerfil: contiene la combinación " & temp & " desconocida.", Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If Not (CDbl(f("PerfilPorcentual")) >= 0 And CDbl(f("PerfilPorcentual")) <= 1) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionPerfil: perfil porcentual de la licitación, para el ID_Mes|Hora '" & CStr(f("ID_Mes")) & "|" & CStr(f("ID_Hora")) & "', se encuentra fuera del rango  (0 - 1).", Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If CDbl(f("PerfilPorcentual")) > perfiltemp Then
                    perfiltemp = CDbl(f("PerfilPorcentual"))
                End If

                If (CInt(f("ID_Hora")) = 24) Then
                    'Cambiar a 1 si se quiere indicar error cuando no existe al menos una hora de 100%
                    If perfiltemp < 0 Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionPerfil: perfil porcentual, para el ID_Mes '" & CStr(f("ID_Mes")) & "', debe valer 100% durante alguna hora.", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                    perfiltemp = 0
                End If


            Next

            For Each l In listaIDMes
                For h = 1 To 24
                    If Not listaidMes_A_Mes_H2.ContainsKey(l & "_" & h) Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionPerfil: no contiene la combinación " & l & "_" & h & ".", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Next
            Next

            If bandera Then
                Auxiliares.Evento("ControlmodLicitacionPerfil -> Ok", Auxiliares.LOG_TYPE_INFO)
            End If
        Catch ex As Exception
            Log4NetError("ControlmodLicitacionPerfil", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("ControlmodLicitacionPerfil")
        End Try
    End Sub

    Private Sub ControlmodParametros(Tabla As DataTable, ByRef bandera As Boolean)
        Dim param As New List(Of Int32)
        Dim vparam As New List(Of Int32) From {
            0,
            1,
            2,
            3,
            4
        }

        For Each f As DataRow In Tabla.Rows
            param.Add(CInt(f("ID_Parametro")))
        Next

        For i = 1 To 6
            If Not param.Contains(i) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: no contiene el parámetro " & i, Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If
        Next

        For Each f As DataRow In Tabla.Rows
            Select Case CInt(f("ID_Parametro"))
                Case 1
                    If CStr(f("Descripcion")) <> "DCCyOCMax" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: DCCyOCMax", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 2
                    If CStr(f("Descripcion")) <> "SPMax" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: SPMax", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 3
                    If CStr(f("Descripcion")) <> "EEEqual" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: EEEqual", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 4
                    If CStr(f("Descripcion")) <> "OVasOC" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: OVasOC", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 5
                    If CStr(f("Descripcion")) <> "DCCMax" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: DCCMax", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 6
                    If CStr(f("Descripcion")) <> "EGMax" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: EGMax", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Case 7
                    If CStr(f("Descripcion")) <> "FlexPot" Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_Parametros: El parámetro " & CStr(f("ID_Parametro")) & " de la tabla Mod_Parametros, tiene como descripción: " & CStr(f("Descripcion")) & ". Valores posibles: FlexPot", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If

            End Select
        Next

        If bandera Then
            Auxiliares.Evento("ControlmodParametros -> Ok", Auxiliares.LOG_TYPE_INFO)
        End If


    End Sub

    Private Sub ControlmodLicitacionmes(Tabla As DataTable, listaIDMes As List(Of String), ByRef bandera As Boolean, ByRef ID_Mes As Integer)
        Try

            Dim k As Integer
            Dim Mes As Integer = 0
            Dim A As Integer = 0
            Dim temp1 As String
            Dim temp2 As String
            k = 0

            For Each f As DataRow In Tabla.Rows
                k = k + 1
                If (k = 1) Then
                    ID_Mes = 0
                    Mes = CInt(f("Mes")) - 1
                    A = CInt(f("A"))
                End If

                ID_Mes = ID_Mes + 1
                Mes = Mes + 1
                If (Mes = 13) Then
                    A = A + 1
                    Mes = 1
                End If

                temp1 = CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes"))
                temp2 = ID_Mes & "_" & A & "_" & Mes

                If Not (temp1 = temp2) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: fila " & k & " ID Mes _ A _ Mes: " & temp1 & ", difiere de los valores: " & temp2, Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If


                If Not (CDbl(f("PotenciaLicitacion")) >= 0 And CDbl(f("PotenciaLicitacion")) <= 20000) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: La potencia de '" & CStr(f("PotenciaLicitacion")) & " MW', no se encuentra en el rango lógico. Revisar el ID Mes " & CStr(f("ID_Mes")), Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If Not (CDbl(f("PotenciaConEnergiaAsociada")) >= 0 And CDbl(f("PotenciaConEnergiaAsociada")) <= 20000) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: La potencia con energía asociada de '" & CStr(f("PotenciaConEnergiaAsociada")) & " MW', no se encuentra en el rango lógico. Revisar el ID Mes " & CStr(f("ID_Mes")), Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If Not (CDbl(f("PrecioPotenciaOferenteVirtualSP")) >= 0 And CDbl(f("PrecioPotenciaOferenteVirtualSP")) <= 100) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: El precio de potencia del oferente virtual de '" & CStr(f("PrecioPotenciaOferenteVirtualSP")) & " USD/KW-mes', no se encuentra en el rango lógico (0 a 100 USD/KW-mes). Revisar el ID Mes " & CStr(f("ID_Mes")), Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If
                If Not (CDbl(f("PrecioPotenciaOferenteVirtualResto")) >= 0 And CDbl(f("PrecioPotenciaOferenteVirtualResto")) <= 100) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: El precio de potencia del oferente virtual de '" & CStr(f("PrecioPotenciaOferenteVirtualResto")) & " USD/KW-mes', no se encuentra en el rango lógico (0 a 100 USD/KW-mes). Revisar el ID Mes " & CStr(f("ID_Mes")), Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If Not (CDbl(f("PrecioEnergiaOferenteVirtual")) >= 0 And CDbl(f("PrecioEnergiaOferenteVirtual")) <= 1) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_LicitacionMes: El precio de energía del oferente virtual de '" & CStr(f("PrecioEnergiaOferenteVirtual")) & " USD/KWh', no se encuentra en el rango lógico (0 a 1 USD/KWh). Revisar el ID Mes " & CStr(f("ID_Mes")), Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                Select Case CInt(f("Mes"))
                    Case 1, 3, 5, 7, 8, 10, 12
                        f("Dias") = 31
                    Case 4, 6, 9, 11
                        f("Dias") = 30
                    Case 2
                        Select Case CInt(f("A"))
                            Case 2020, 2024, 2028, 2032, 2036, 2040, 2044, 2048, 2052, 2056, 2060, 2064, 2068, 2072, 2076, 2080, 2084, 2088
                                f("Dias") = 29
                            Case Else
                                f("Dias") = 28
                        End Select
                End Select
                listaIDMes.Add(CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes")))

            Next

            If bandera Then
                Auxiliares.Evento("ControlmodLicitacionmes -> Ok", Auxiliares.LOG_TYPE_INFO)
            End If
        Catch ex As Exception
            Log4NetError("ControlmodLicitacionmes", ex)
            Auxiliares.Evento(ex.ToString, Auxiliares.LOG_TYPE_INFO)
            Throw New Exception("ControlmodLicitacionmes")
        End Try

    End Sub

    Private Sub ControlmodOferentePerfil(Tabla As DataTable, listaOferentes As List(Of Integer), listaIDMes As List(Of String), contrato As Dictionary(Of Int32, String), ByRef bandera As Boolean)
        Dim listaIdOferente_idMes_A_Mes_H As New Dictionary(Of String, String)
        Dim listaIdOferente_idMes_A_Mes_H2 As New Dictionary(Of String, String)

        Dim temp As String = ""
        Dim perfiltemp As Double = 0
        Try
            For Each o As Integer In listaOferentes
                For Each l In listaIDMes
                    For h = 1 To 24
                        listaIdOferente_idMes_A_Mes_H.Add(CStr(o) & "_" & CStr(l) & "_" & CStr(h), CStr(o) & "_" & CStr(l) & "_" & CStr(h))
                    Next
                Next

            Next
            'Auxiliares.Evento("ControlmodOferentePerfil-Fin FOR1 ", Auxiliares.LOG_TYPE_INFO)
            GC.Collect()

            For Each f As DataRow In Tabla.Rows
                temp = CStr(f("ID_Oferente")) & "_" & CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes")) & "_" & CStr(f("ID_Hora"))
                listaIdOferente_idMes_A_Mes_H2.Add(temp, temp)

                If Not listaIdOferente_idMes_A_Mes_H.ContainsKey(temp) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_OferentePerfil: contiene la combinación " & temp & " desconocida.", Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If

                If CStr(f("ID_Contrato")) = "DCC" Then
                    If Not (CDbl(f("PerfilPorcentual")) >= 0 And CDbl(f("PerfilPorcentual")) <= 1) Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_OferentePerfil: perfil porcentual del oferente " & CStr(f("ID_Oferente")) & ", para el ID_Mes|Hora '" & CStr(f("ID_Mes")) & "|" & CStr(f("ID_Hora")) & "', se encuentra fuera del rango  (0 - 1.25).", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                Else
                    If Not (CDbl(f("PerfilPorcentual")) >= 0 And CDbl(f("PerfilPorcentual")) <= 1) Then
                        Auxiliares.Evento("Error: Control Coherencia - Mod_OferentePerfil: perfil porcentual del oferente " & CStr(f("ID_Oferente")) & ", para el ID_Mes|Hora '" & CStr(f("ID_Mes")) & "|" & CStr(f("ID_Hora")) & "', se encuentra fuera del rango  (0 - 1).", Auxiliares.LOG_TYPE_ERROR)
                        bandera = False
                    End If
                End If

                If contrato.ContainsKey(CInt(f("ID_Oferente"))) Then
                    f("ID_Contrato") = contrato(CInt(f("ID_Oferente")))
                End If

            Next
            ' Auxiliares.Evento("ControlmodOferentePerfil-Fin FOR2 ", Auxiliares.LOG_TYPE_INFO)

            For Each o In listaOferentes

                For Each l In listaIDMes
                    For h = 1 To 24
                        If Not listaIdOferente_idMes_A_Mes_H2.ContainsKey(CStr(o) & "_" & CStr(l) & "_" & CStr(h)) Then
                            Auxiliares.Evento("Error: Control Coherencia - Mod_OferentePerfil: no contiene la combinación " & CStr(o) & "_" & CStr(l) & "_" & CStr(h) & ".", Auxiliares.LOG_TYPE_ERROR)
                            bandera = False
                        End If
                    Next
                Next
            Next
            'Auxiliares.Evento("ControlmodOferentePerfil-Fin FOR3 ", Auxiliares.LOG_TYPE_INFO)
            If bandera Then
                Auxiliares.Evento("ControlmodOferentePerfil -> Ok", Auxiliares.LOG_TYPE_INFO)
            End If
        Catch ex As Exception
            Auxiliares.Evento("ControlmodOferentePerfil -> Falló. " & ex.ToString, Auxiliares.LOG_TYPE_INFO)
        End Try



    End Sub


    Private Sub ControlmodOferentemes(Tabla As DataTable, listaOferentes As List(Of Integer), listaIDMes As List(Of String), contrato As Dictionary(Of Int32, String), ByRef bandera As Boolean)
        Dim oferentemes As New Dictionary(Of String, String)
        Dim oferentemes2 As New Dictionary(Of String, String)
        Dim RenNoRen As New HashSet(Of String)
        Dim temp As String

        For Each f As DataRow In Tabla.Rows
            temp = CStr(f("ID_Oferente")) & "_" & CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes"))
            oferentemes.Add(temp, temp)
        Next

        RenNoRen.Add("Ren")
        RenNoRen.Add("NoRen")

        For Each o In listaOferentes
            For Each m In listaIDMes
                temp = o & "_" & m
                oferentemes2.Add(temp, temp)
                If Not oferentemes.ContainsKey(temp) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: ID Oferente, ID Mes, A y Mes '" & temp & "', no se encuentra en la tabla Mod_LicitacionMes.", Auxiliares.LOG_TYPE_ERROR)
                    bandera = False
                End If
            Next
        Next

        For Each f As DataRow In Tabla.Rows
            temp = CStr(f("ID_Oferente")) & "_" & CStr(f("ID_Mes")) & "_" & CStr(f("A")) & "_" & CStr(f("Mes"))
            If Not oferentemes2.ContainsKey(temp) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: ID Oferente, ID Mes, A y Mes '" & temp & "' de la tabla Mod_OferenteMes, no se encuentra en la combinatoria de las tablas Mod_Oferente y Mod_LicitacionMes ", Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If

            If Not (CDbl(f("PotenciaMaxima")) >= 0 And CDbl(f("PotenciaMaxima")) <= 10000) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: La potencia Máxima indicada de '" & CStr(f("PotenciaMaxima")) & " MW', no se encuentra en el rango lógico. Revisar el ID Oferente_ID_Mes_Mes_A " & temp, Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If

            If Not (CDbl(f("PotenciaMinima")) >= 0 And CDbl(f("PotenciaMinima")) <= CDbl(f("PotenciaMaxima"))) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: La potencia Minima indicada de '" & CStr(f("PotenciaMinima")) & " MW', no se encuentra en el rango lógico. Revisar el ID Oferente_ID_Mes_Mes_A " & temp, Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If

            If Not (CDbl(f("PrecioPotencia")) >= 0 And CDbl(f("PrecioPotencia")) <= 300) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: El precio de potencia  de '" & CStr(f("PrecioPotencia")) & " USD/kW-mes', no se encuentra en el rango lógico (0 y 300 USD/kW-mes). Revisar el ID Oferente_ID_Mes_Mes_A " & temp, Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If

            If Not (CDbl(f("PrecioEnergia")) >= 0 And CDbl(f("PrecioEnergia")) <= 1) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: El precio de energía  de '" & CStr(f("PrecioEnergia")) & " USD/kWh', no se encuentra en el rango lógico (0 y 1 USD/kWh). Revisar el ID Oferente_ID_Mes_Mes_A " & temp, Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If


            If contrato.ContainsKey(CInt(f("ID_Oferente"))) Then
                f("ID_Contrato") = contrato(CInt(f("ID_Oferente")))
            End If

            If CStr(f("ID_Contrato")) = "DCC_OC" Or CStr(f("ID_Contrato")) = "DCC" Then
                If Not RenNoRen.Contains(CStr(f("RenNoRen"))) Then
                    Auxiliares.Evento("Error: Control Coherencia - Mod_OferenteMes: columna RenNoRen '" & CStr(f("RenNoRen")) & "', para el oferente|Año|Mes|Id_Mes " & temp, Auxiliares.LOG_TYPE_ERROR)
                End If
            End If
        Next

        If bandera Then
            Auxiliares.Evento("ControlmodOferentemes -> Ok", Auxiliares.LOG_TYPE_INFO)
        End If
    End Sub

    Private Sub ControlModOferente(Tabla As DataTable, listaOferentes As List(Of Integer), Zonas As List(Of Integer), listaTipos As List(Of String), contrato As Dictionary(Of Int32, String), ByRef bandera As Boolean, ID_Mes As Integer)
        Dim listaContratos As New Dictionary(Of String, String)
        Dim listaNacionalidad As New Dictionary(Of String, String)
        Dim ints As New List(Of Integer)

        listaContratos.Add("DCC", "DCC")
        listaContratos.Add("OC", "OC")
        listaContratos.Add("SP", "SP")
        listaContratos.Add("SE", "SE")
        listaContratos.Add("DCC_OC", "SE")

        listaNacionalidad.Add("N", "N")
        listaNacionalidad.Add("I", "I")

        ints.Add(0)
        ints.Add(1)

        For Each f As DataRow In Tabla.Rows
            If Not listaContratos.ContainsKey(CStr(f("ID_Contrato"))) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_Oferente: ID de Contrato '" & CStr(f("ID_Contrato")) & "', desconocido. Revisar Oferente " & CStr(f("Nombre")), Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If
            If Not ints.Contains(CInt(f("Considerar"))) Then
                Auxiliares.Evento("Error: Control Coherencia - Mod_Oferente: Campo Considerar '" & CStr(f("Considerar")) & "', desconocido. Revisar Oferente " & CStr(f("Nombre")), Auxiliares.LOG_TYPE_ERROR)
                bandera = False
            End If

            listaOferentes.Add(CInt(f("ID_Oferente")))
            contrato.Add(CInt(f("ID_Oferente")), CStr(f("ID_Contrato")))
        Next

        If bandera Then
            Auxiliares.Evento("controlModOferente -> Ok", Auxiliares.LOG_TYPE_INFO)
        End If


    End Sub



End Module
