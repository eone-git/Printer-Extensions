Imports System.Drawing.Printing.PrinterSettings
Imports System.Drawing.Printing

Public Class clsPrinterExtension
    Dim moduleName As String = "Printer Extension"
    Public Function GetInstalledPrinterList() As StringCollection
        Dim printerList As StringCollection
        Try
            printerList = PrinterSettings.InstalledPrinters
            Return printerList

        Catch ex As Exception
            Return Nothing
            MessageBox.Show(ex.Message, moduleName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

    End Function

End Class
