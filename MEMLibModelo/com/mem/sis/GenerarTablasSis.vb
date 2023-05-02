Imports MEMLibCommon

Module GenerarTablasSis

    Public Sub GenerarLog(MySqlBase As MySqlBase)

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "sis_log"
        Call CargarCol(Tabla, "fecha", GetType(System.DateTime))
        Call CargarCol(Tabla, "orden", GetType(System.Int32))
        Call CargarCol(Tabla, "tipo", GetType(System.String), "20")
        Call CargarCol(Tabla, "informacion", GetType(System.String), "2048")
        Call CargarCol(Tabla, "plugin", GetType(System.String), "256")
        Call CargarCol(Tabla, "version", GetType(System.String), "20")

        MySqlBase.CreateTable(Tabla)
    End Sub

    'Tabala escritura
    Public Sub GenerarSis_tablasescritura(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "sis_tablasescritura"
        Call CargarCol(Tabla, "Archivo", GetType(System.String), True)
        Call CargarCol(Tabla, "Tabla", GetType(System.String))
        Call CargarCol(Tabla, "Borrar", GetType(System.Int32))
        Call CargarCol(Tabla, "Leer", GetType(System.Int32))
        Call CargarCol(Tabla, "primarykey", GetType(System.String))
        Call CargarCol(Tabla, "server", GetType(System.String))
        Call CargarCol(Tabla, "user", GetType(System.String))
        Call CargarCol(Tabla, "pass", GetType(System.String))
        Call CargarCol(Tabla, "database", GetType(System.String))
        Call CargarCol(Tabla, "Dato", GetType(System.String))
        Call CargarCol(Tabla, "Tomar", GetType(System.Int32))

#Region "Datos"
        Dim row As DataRow
        row = Tabla.NewRow()
        row("Archivo") = "Sis_TablasEscritura"
        row("Tabla") = "Sis_TablasEscritura"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "Tabla"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_Parametros"
        row("Tabla") = "Mod_Parametros"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Parametro"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_Oferente"
        row("Tabla") = "Mod_Oferente"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Oferente"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_LicitacionMes"
        row("Tabla") = "Mod_LicitacionMes"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Mes"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_LicitacionPerfil"
        row("Tabla") = "Mod_LicitacionPerfil"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Mes,ID_Hora"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_OferenteMes"
        row("Tabla") = "Mod_OferenteMes"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Oferente,ID_Mes"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

        row = Tabla.NewRow()
        row("Archivo") = "Mod_OferentePerfil"
        row("Tabla") = "Mod_OferentePerfil"
        row("Borrar") = 1
        row("Leer") = 1
        row("primarykey") = "ID_Oferente,ID_Mes,ID_Hora"
        row("server") = "localhost"
        row("user") = "root"
        row("pass") = "root"
        row("database") = ""
        row("Dato") = 1
        row("Tomar") = 1
        Tabla.Rows.Add(row)

#End Region
        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarSis_tablasescritura", tiempo)

    End Sub

    Public Sub GenerarMod_Adjudicados(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_adjudicados"
        Call CargarCol(Tabla, "Nombre", GetType(System.String), True)
        Call CargarCol(Tabla, "AE", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Seleccionado", GetType(System.Int32))
        Call CargarCol(Tabla, "PGMx", GetType(System.Double))
        Call CargarCol(Tabla, "PGMn", GetType(System.Double))
        Call CargarCol(Tabla, "PMedAdj", GetType(System.Double))
        Call CargarCol(Tabla, "Energia", GetType(System.Double))
        Call CargarCol(Tabla, "PorcentajeReduccion", GetType(System.Double))
        Call CargarCol(Tabla, "PorcentajeParaTotal", GetType(System.Double))
        Call CargarCol(Tabla, "MonomicoTot", GetType(System.Double))

        MySqlBase.CreateTable(Tabla)
        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("Generarmod_adjudicados", tiempo)
    End Sub
    Public Sub GenerarTabla_Precios(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "tabla_precios"
        Call CargarCol(Tabla, "Nombre", GetType(System.String), True)
        Call CargarCol(Tabla, "ID_Contrato", GetType(System.String))
        Call CargarCol(Tabla, "ID_Combustible1", GetType(System.String))
        Call CargarCol(Tabla, "ID_Combustible2", GetType(System.String))
        Call CargarCol(Tabla, "Estado", GetType(System.String))
        Call CargarCol(Tabla, "CTUNG", GetType(System.Double))
        Call CargarCol(Tabla, "O&MNoRen", GetType(System.Double))
        Call CargarCol(Tabla, "O&MRen", GetType(System.Double))
        Call CargarCol(Tabla, "PEO", GetType(System.Double))
        Call CargarCol(Tabla, "PPO", GetType(System.Double))
        Call CargarCol(Tabla, "CI", GetType(System.Double))
        Call CargarCol(Tabla, "CITT", GetType(System.Double))
        Call CargarCol(Tabla, "CEM", GetType(System.Double))
        Call CargarCol(Tabla, "CTE", GetType(System.Double))
        Call CargarCol(Tabla, "PCAL", GetType(System.Double))
        Call CargarCol(Tabla, "FAGN", GetType(System.Double))
        Call CargarCol(Tabla, "HorasRen", GetType(System.Double))
        Call CargarCol(Tabla, "HorasNoRen", GetType(System.Double))
        Call CargarCol(Tabla, "PGMax", GetType(System.Double))
        Call CargarCol(Tabla, "PGMin", GetType(System.Double))
        Call CargarCol(Tabla, "AnioInicio", GetType(System.Double))
        Call CargarCol(Tabla, "FPNoRen", GetType(System.Double))
        Call CargarCol(Tabla, "FPRen", GetType(System.Double))
        Call CargarCol(Tabla, "F", GetType(System.Double))
        Call CargarCol(Tabla, "k", GetType(System.Double))
        Call CargarCol(Tabla, "Habilitado_Pujar", GetType(System.Double))
        Call CargarCol(Tabla, "IDRonda", GetType(System.Double))
        MySqlBase.CreateTable(Tabla)
        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarTabla_Precios", tiempo)
    End Sub
    Public Sub GenerarMod_EvolucionK(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_evolucion_K"
        Call CargarCol(Tabla, "Anio", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Bunker", GetType(System.Double))
        Call CargarCol(Tabla, "Gas_Natural", GetType(System.Double))
        Call CargarCol(Tabla, "Carbon", GetType(System.Double))
        Call CargarCol(Tabla, "PPi_PP0", GetType(System.Double))
        MySqlBase.CreateTable(Tabla)
        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("Generarmod_evolucion_K", tiempo)
    End Sub

    'Tablas Mod
    Public Sub GenerarMod_licitacionmes(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_licitacionmes"
        Call CargarCol(Tabla, "ID_Mes", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Dias", GetType(System.Int32))
        Call CargarCol(Tabla, "Mes", GetType(System.Int32))
        Call CargarCol(Tabla, "A", GetType(System.Int32))
        Call CargarCol(Tabla, "AE", GetType(System.Int32))
        Call CargarCol(Tabla, "PotenciaLicitacion", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaConEnergiaAsociada", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioPotenciaOferenteVirtualResto", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioPotenciaOferenteVirtualSP", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaAdjudicadaOVResto", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaAdjudicadaOVSP", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaAdjudicadaOV", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioEnergiaOferenteVirtual", GetType(System.Double))
        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarMod_licitacionmes", tiempo)

    End Sub

    Public Sub GenerarRes_Resultado(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now
        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "res_resultado"
        Call CargarCol(Tabla, "ID_Oferente", GetType(System.String), True)
        Call CargarCol(Tabla, "Nombre", GetType(System.String))
        Call CargarCol(Tabla, "Contrato", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb1", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb2", GetType(System.String))
        Call CargarCol(Tabla, "Precio_Potencia", GetType(System.Double), False, "6,3")
        Call CargarCol(Tabla, "Precio_Energia", GetType(System.Double), False, "10,5")
        Call CargarCol(Tabla, "Energia_Asignada", GetType(System.Double), False, "12,0")
        Call CargarCol(Tabla, "Potencia_Media_Ofertada", GetType(System.Double), False, "12,0")
        Call CargarCol(Tabla, "Potencia_Media_Adjudicada", GetType(System.Double), False, "12,0")
        Call CargarCol(Tabla, "Periodos_de_Potencia_Adjudicada", GetType(System.Int32))
        Call CargarCol(Tabla, "Periodos_de_Energia_Asignada", GetType(System.Int32))
        Call CargarCol(Tabla, "Costo_Total_Potencia", GetType(System.Double), False, "15,0")
        Call CargarCol(Tabla, "Costo_Total_Energia", GetType(System.Double), False, "15,0")
        Call CargarCol(Tabla, "Costo_Total", GetType(System.Double), False, "15,0")
        MySqlBase.CreateTable(Tabla)
        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))
        Call CalculoSegundos("GenerarRes_resultado", tiempo)
    End Sub


    Public Sub GenerarMod_licitacionperfil(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_licitacionperfil"
        Call CargarCol(Tabla, "ID_Mes", GetType(System.Int32), True)
        Call CargarCol(Tabla, "ID_Hora", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Mes", GetType(System.Int32))
        Call CargarCol(Tabla, "A", GetType(System.Int32))
        Call CargarCol(Tabla, "AE", GetType(System.Int32))
        Call CargarCol(Tabla, "PerfilPorcentual", GetType(System.Double))
        Call CargarCol(Tabla, "PerfilAsignadoOV", GetType(System.Double))

        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarMod_licitacionperfil", tiempo)

    End Sub


    Public Sub GenerarMod_oferente(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_oferente"
        Call CargarCol(Tabla, "ID_Oferente", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Nombre", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb1", GetType(System.String))
        Call CargarCol(Tabla, "ID_Comb2", GetType(System.String))
        Call CargarCol(Tabla, "ID_Contrato", GetType(System.String))
        Call CargarCol(Tabla, "Considerar", GetType(System.Int32))
        Call CargarCol(Tabla, "Adjudicado", GetType(System.Int32))
        Call CargarCol(Tabla, "Forzado", GetType(System.Int32))
        Call CargarCol(Tabla, "CostoTotalOferente", GetType(System.Double))
        Call CargarCol(Tabla, "SobreCostoFO", GetType(System.Double))

        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarMod_oferente", tiempo)

    End Sub

    Public Sub GenerarMod_oferentemes(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now
        Dim Tabla As DataTable = New DataTable

        Tabla.TableName = "mod_oferentemes"
        Call CargarCol(Tabla, "ID_Oferente", GetType(System.Int32))
        Call CargarCol(Tabla, "ID_Mes", GetType(System.Int32))
        Call CargarCol(Tabla, "Mes", GetType(System.Int32))
        Call CargarCol(Tabla, "A", GetType(System.Int32))
        Call CargarCol(Tabla, "AE", GetType(System.Int32))
        Call CargarCol(Tabla, "PotenciaMaxima", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaMinima", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioPotencia", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioPSinIndex", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioEnergia", GetType(System.Double))
        Call CargarCol(Tabla, "PrecioESinIndex", GetType(System.Double))
        Call CargarCol(Tabla, "PotenciaAdjudicada", GetType(System.Double))
        Call CargarCol(Tabla, "EnergiaAsignada", GetType(System.Double))
        Call CargarCol(Tabla, "CostoPotencia", GetType(System.Double))
        Call CargarCol(Tabla, "CostoEnergia", GetType(System.Double))
        Call CargarCol(Tabla, "ID_Contrato", GetType(System.String))
        Call CargarCol(Tabla, "RenNoRen", GetType(System.String))

        MySqlBase.CreateTable(Tabla)
        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))
        Call CalculoSegundos("GenerarMod_oferentemes", tiempo)

    End Sub

    Public Sub GenerarMod_oferenteperfil(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_oferenteperfil"
        Call CargarCol(Tabla, "ID_Oferente", GetType(System.Int32), True)
        Call CargarCol(Tabla, "ID_Mes", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Mes", GetType(System.Int32))
        Call CargarCol(Tabla, "A", GetType(System.Int32))
        Call CargarCol(Tabla, "AE", GetType(System.Int32))
        Call CargarCol(Tabla, "ID_Hora", GetType(System.Int32), True)
        Call CargarCol(Tabla, "PerfilPorcentual", GetType(System.Double))
        Call CargarCol(Tabla, "PerfilAsignado", GetType(System.Double))
        Call CargarCol(Tabla, "ID_Contrato", GetType(System.String))

        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))

        Call CalculoSegundos("GenerarMod_oferenteperfil", tiempo)

    End Sub

    Public Sub GenerarMod_parametros(MySqlBase As MySqlBase)
        Dim tiempo As DateTime = DateTime.Now

        Dim Tabla As DataTable = New DataTable()
        Tabla.TableName = "mod_parametros"
        Call CargarCol(Tabla, "Id_Parametro", GetType(System.Int32), True)
        Call CargarCol(Tabla, "Descripcion", GetType(System.String))
        Call CargarCol(Tabla, "Valor", GetType(System.Double))

        MySqlBase.CreateTable(Tabla)

        MySqlBase.ActualizarDB(MySqlBase.GenerarInsertsFromTable(Tabla.TableName, Tabla))
        Call CalculoSegundos("GenerarMod_parametros", tiempo)
    End Sub


End Module
