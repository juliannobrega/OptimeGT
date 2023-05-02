Imports System.Data
Imports MEMLibCommon
Imports MEMLibModelo

Class MainWindow

    Private Sub btnEjecutarModelo_Click(sender As Object, e As RoutedEventArgs) Handles btnEjecutarModelo.Click
        Dim tablas As Plugin = New Plugin
        Debug.WriteLine("Dot Net Perls")

        tablas.Command("ejecutarmodelo", txtServerDatabase.Text, txtNombreDatabase.Text, txtUsuarioDatabase.Text, txtPasswordDatabase.Text)
    End Sub


    Private Sub MainWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Dim MySqlBase As New MySqlBase
        MySqlBase.connString = MySqlBase.GetConectionString(txtServerDatabase.Text, txtNombreDatabase.Text, txtUsuarioDatabase.Text, txtPasswordDatabase.Text)
        'MySqlBase.connString = MySqlBase.GetConectionString("gq-test2.cloudapp.net", "mem_9d985736-a49b-417c-887c-afc445989dda", "memuser", "memuser")
        MySqlBase.connString = MySqlBase.GetConectionString("localhost", "mem_panama_modelo", "root", "24Cuerdas")
        MySqlBase = New MySqlBase()
        Dim data As DataSet = New DataSet()

        MySqlBase.FillAdapterMultiple(data, "mod_licitacionmes", "SELECT * FROM mod_licitacionmes")

        Dim t As DataTable = data.Tables("mod_licitacionmes")
        Dim s As String = ""

        For i As Int32 = 0 To (t.Rows.Count - 1)
            Dim row As DataRow = t.Rows(i)
            s += "row = Tabla.NewRow()" + vbCrLf
            For j As Int32 = 0 To (t.Columns.Count - 1)
                If IsNumeric(row(j)) Then
                    s += "row(""" + t.Columns(j).ColumnName + """) = " + row(j).ToString().Replace(",", ".") + "" + vbCrLf
                Else
                    s += "row(""" + t.Columns(j).ColumnName + """) = """ + row(j).ToString() + """" + vbCrLf
                End If
                's += "row(""" + t.Columns(j).ColumnName + """) = """ + row(j).ToString() + """" + vbCrLf
            Next
            s += "Tabla.Rows.Add(row)" + vbCrLf + vbCrLf
        Next

    End Sub

End Class
