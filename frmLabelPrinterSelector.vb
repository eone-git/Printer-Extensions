Imports System.Drawing.Printing

Public Class frmLabelPrinterSelector
    Dim clsPrinterExtensionObj As New clsPrinterExtension
    Public lablePrinterStatus As Integer
    Sub FillPrinterList()
        Try
            cmbPrinterList.Items.Clear()
            Dim i As Integer
            Dim pkInstalledPrinters As String
            For i = 0 To PrinterSettings.InstalledPrinters.Count - 1
                pkInstalledPrinters = PrinterSettings.InstalledPrinters.Item(i)
                cmbPrinterList.Items.Add(pkInstalledPrinters)
            Next
            cmbPrinterList.Text = labelPrinterAfterSelect
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        End Try
    End Sub

    Private Sub frmLabelPrinterSelector_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FillPrinterList()
        LoadDefaultData()
    End Sub

    Private Sub cmbPrinterList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPrinterList.SelectedIndexChanged
        labelPrinterAfterSelect = cmbPrinterList.SelectedItem

    End Sub

    Public Sub LabelPrintingMessage(Optional ByRef messageType As String = "", Optional exceptionMessage As Exception = Nothing, Optional ByRef customeMessage As String = "")
        If messageType = "NoPrinter" Then
            MessageBox.Show("Please select the label printer", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf messageType = "missingPrinter" Then
            MessageBox.Show("Selected " & labelPrinterAfterSelect & "printer is no longer exists.Please select a correct label printer", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf messageType = "catch" Then
            MessageBox.Show(exceptionMessage.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf messageType = "warning" Then
            MessageBox.Show(customeMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf messageType = "warning" Then
        End If

    End Sub

    Public Function PrinterSatus() As String
        Dim printerSatusText As String
        Dim isprinterExist As Boolean
        Try
            If IsNothing(labelPrinterAfterSelect) = False Then
                For Each item As String In PrinterSettings.InstalledPrinters
                    If item = labelPrinterAfterSelect Then
                        isprinterExist = True
                        Exit For
                    End If
                Next

                If isprinterExist = True Then
                    printerSatusText = labelPrinterAfterSelect & " printer is selected"
                    lablePrinterStatus = LablePrinterStatusKey.OK

                Else
                    printerSatusText = labelPrinterAfterSelect & " printer is no longer exists"
                    lablePrinterStatus = LablePrinterStatusKey.Hold

                End If

            Else
                printerSatusText = "Please select the label printer"
                lablePrinterStatus = LablePrinterStatusKey.NO
            End If
            Return printerSatusText
        Catch ex As Exception
            printerSatusText = "Error in default printer. Please reselect the label printer"
            Return printerSatusText

        End Try

    End Function

    Function DataBaseAcess(ByRef quarType As String, Optional ByRef querySQL As String = "", Optional ByRef withPara As Boolean = False, Optional ByRef withTrans As Boolean = False, Optional ByRef clsSqlConnObj As clsSqlConn = Nothing) As DataSet

        Try
            If IsNothing(querySQL) = True Then
                LabelPrintingMessage("warning", Nothing, "SQL query can't be nothing")
                Return Nothing
                Exit Function
            Else
                querySQL.Replace("'", "''")

            End If
            If withTrans = False Then
                clsSqlConnObj = New clsSqlConn
                If withPara = False Then
                    If quarType = "crud" Then
                        DS = clsSqlConnObj.GET_INSERT_UPDATE(querySQL)
                    End If
                Else
                    'With parameters
                End If
            Else
                'With nothing
            End If
            Return DS

        Catch ex As Exception
            Return Nothing
            LabelPrintingMessage("warning", Nothing, "Error when accessing database")

        End Try

    End Function
   
    Public Sub LoadDefaultData()
        Try
            Dim querySQL As String = "SELECT hasDefaultLblPrinter, defaultLblPrinter FROM spilAgentEmailSettings WHERE AgentID = '" & AgentID & "'"
            Dim isDefault As Boolean
            Dim ds As DataSet = DataBaseAcess("crud", querySQL)
            If IsNothing(ds) = False Then
                isDefault = ds.Tables(0).Rows(0).Item("hasDefaultLblPrinter")
                If IsNothing(chkIsDefault) = False Then
                    chkIsDefault.Checked = isDefault
                End If
            End If
            labelPrinterBeforeSelect = ds.Tables(0).Rows(0).Item("defaultLblPrinter")
            labelPrinterAfterSelect = ds.Tables(0).Rows(0).Item("defaultLblPrinter")
        Catch ex As Exception
            LabelPrintingMessage("catch", ex)

        End Try
    End Sub

    Sub SaveDefaultData()
        Dim querySQL As String = "UPDATE spilAgentEmailSettings SET hasDefaultLblPrinter='" & chkIsDefault.Checked & "', defaultLblPrinter = '" & cmbPrinterList.Text & "' WHERE AgentID = '" & AgentID & "'"
        Dim ds As DataSet = DataBaseAcess("crud", querySQL)
        If IsNothing(ds) = True Then
            LabelPrintingMessage("warning", Nothing, "Data not saved")
        End If
    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Try
            If MessageBox Then
        Catch ex As Exception

        End Try
    End Sub
End Class