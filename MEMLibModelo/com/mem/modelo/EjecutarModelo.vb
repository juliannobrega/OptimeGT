Option Strict On
Imports Gurobi
Imports MEMLibCommon

Module EjecutarModelo



    Public Sub EjecModelo(Model As GRBModel, ByRef tiempoT As DateTime)
        Model.ModelName = "SIMMEM"
        Model.ModelSense = GRB.MINIMIZE
        'Model.Tune()
        Model.GetEnv.Method = 2
        Model.GetEnv.ScaleFlag = 2
        Model.GetEnv.Presolve = 1

        Model.Optimize()
        Call CalculoSegundos("EjecModelo ", tiempoT)
    End Sub

End Module
