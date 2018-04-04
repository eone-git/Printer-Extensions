Imports System.Drawing.Printing

Public Class frmLabelPrinterSelector
    Dim clsPrinterExtensionObj As New clsPrinterExtension
    Public lablePrinterStatus As Integer
    Dim isSaved As Boolean = False
    Dim okPressed As Boolean = False
    Public isDirectCall As Boolean = False

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

    Public Function LabelPrintingMessage(Optional ByRef messageType As String = "", Optional exceptionMessage As Exception = Nothing, Optional ByRef customeMessage As String = "") As DialogResult
        Try
            If messageType = "NoPrinter" Then
                Return MessageBox.Show("Please select the label printer", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ElseIf messageType = "missingPrinter" Then
                Return MessageBox.Show("Selected " & labelPrinterAfterSelect & "printer is no longer exists.Please select a correct label printer", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ElseIf messageType = "catch" Then
                Return MessageBox.Show(exceptionMessage.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf messageType = "warning" Then
                Return MessageBox.Show(customeMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ElseIf messageType = "question" Then
                Return MessageBox.Show(customeMessage, Me.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try
    End Function

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
                    If isDirectCall = False Then
                        chkIsDefault.Checked = isDefault
                    Else
                        If isDefault = True Then
                            labelPrinterBeforeSelect = ds.Tables(0).Rows(0).Item("defaultLblPrinter")
                            labelPrinterAfterSelect = ds.Tables(0).Rows(0).Item("defaultLblPrinter")
                        End If
                        isDirectCall = False
                    End If
                End If
            End If
        Catch ex As Exception
            LabelPrintingMessage("catch", ex)

        End Try
    End Sub

    Function SaveDefaultData() As Integer
        If cmbPrinterList.Text = "" Then
            LabelPrintingMessage("warning", Nothing, "Plese select a printer before save")
            Return 0
        Else
            Dim querySQL As String = "UPDATE spilAgentEmailSettings SET hasDefaultLblPrinter='" & chkIsDefault.Checked & "', defaultLblPrinter = '" & cmbPrinterList.Text & "' WHERE AgentID = '" & AgentID & "'"
            Dim ds As DataSet = DataBaseAcess("crud", querySQL)
            If IsNothing(ds) = True Then
                LabelPrintingMessage("warning", Nothing, "Data not saved")
            Else
                Return 1
            End If
        End If
    End Function

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim messageResult As DialogResult
        Try
            messageResult = LabelPrintingMessage("question", Nothing, "Do you want to save printer settings?")
            okPressed = True
            If messageResult = Windows.Forms.DialogResult.Yes Then
                If SaveDefaultData() = 1 Then
                    isSaved = True
                Else
                    isSaved = False
                End If
            ElseIf messageResult =DialogResult.No Then
                isSaved = False
            ElseIf messageResult = DialogResult.Cancel Then
                isSaved = False
                'okPressed = False
            End If

        Catch ex As Exception
            LabelPrintingMessage("catch", ex)

        End Try
    End Sub

    Private Sub frmLabelPrinterSelector_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim messageResult As DialogResult
        Try

            If isSaved = False Then
                If okPressed = False Then
                    messageResult = LabelPrintingMessage("question", Nothing, "Do you want to save changes before exit?")
                    If messageResult = Windows.Forms.DialogResult.Yes Then
                        If SaveDefaultData() = 0 Then
                            e.Cancel = True
                            isSaved = False
                        Else
                            isSaved = True
                        End If

                    ElseIf messageResult = Windows.Forms.DialogResult.No Then
                        isSaved = False

                    ElseIf messageResult = Windows.Forms.DialogResult.Cancel Then
                        e.Cancel = True
                        isSaved = False
                    End If

                Else
                    e.Cancel = True
                End If
            End If
        Catch ex As Exception
            LabelPrintingMessage("catch", ex)

        Finally
            isSaved = False
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Try
            isSaved = True
        Catch ex As Exception
            LabelPrintingMessage("catch", ex)

        End Try
    End Sub
End Class