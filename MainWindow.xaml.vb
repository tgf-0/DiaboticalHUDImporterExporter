﻿Imports System.Environment
Imports System.IO
Imports Microsoft.Win32

Class MainWindow
    'Import
    Private Sub ImportDhud(sender As Object, e As RoutedEventArgs)
        StatusText1.Text = "Importing..."
        Dim SenderName As String = DirectCast(sender, FrameworkElement).Name
        Dim appData As String = GetFolderPath(SpecialFolder.ApplicationData)
        Dim diaboticalSettingsPath = appData & "\Diabotical\Settings.txt"
        Dim NewHudSettings As String = ""

        If SenderName = "ImportJSONFromPanel" Then
            NewHudSettings = InputDisplayBox.Text
        ElseIf SenderName = "ImportFromFile" Then
            Dim SelectDhudFileDialog As New OpenFileDialog With {
                .Filter = "dhud files (*.dhud)|*.dhud|All files (*.*)|*.*",
                .FilterIndex = 1,
                .RestoreDirectory = True
            }
            If SelectDhudFileDialog.ShowDialog() = True Then
                NewHudSettings = File.ReadAllText(SelectDhudFileDialog.FileName)
            End If
        End If
        If My.Computer.FileSystem.FileExists(diaboticalSettingsPath) Then
            Dim lines() As String = File.ReadAllLines(diaboticalSettingsPath)
            For i As Integer = 0 To lines.Length - 1
                If lines(i).Contains("hud_definition") Then
                    lines(i) = "hud_definition = " & NewHudSettings
                Else
                    StatusText1.Text = "HUD definition not found. Please restore a working settings file."
                End If

            Next
            File.WriteAllLines(diaboticalSettingsPath, lines)
            StatusText1.Text = "Wrote HUD to settings file."
        Else
            StatusText1.Text = "Settings file not found."
            MsgBox("Diabotical settings not found. Please check that %appdata%/Diabotical/Settings.txt exists.")
        End If
    End Sub
    'Export
    Private Sub ExportDhud(sender As Object, e As RoutedEventArgs)
        StatusText1.Text = "Exporting...."
        Dim SenderName As String = DirectCast(sender, System.Windows.FrameworkElement).Name
        Dim appData As String = GetFolderPath(SpecialFolder.ApplicationData)
        Dim diaboticalSettingsPath = appData & "\Diabotical\Settings.txt"
        Dim HudSettings As String = ""
        If My.Computer.FileSystem.FileExists(diaboticalSettingsPath) Then
            Using SettingsFile As New FileIO.TextFieldParser(diaboticalSettingsPath)
                SettingsFile.TextFieldType = FileIO.FieldType.Delimited
                SettingsFile.SetDelimiters("=")
                Dim currentRow As String()
                While Not SettingsFile.EndOfData
                    Try
                        currentRow = SettingsFile.ReadFields()
                        Dim currentField As String
                        For Each currentField In currentRow
                            If currentField = "hud_definition" Then
                                HudSettings = currentRow(1)
                                StatusText1.Text = "Found HUD settings."
                                'Else
                                '@TODO Set flag to display this message
                                'StatusText1.Text = "HUD definition not found. Please restore a working settings file."
                            End If
                        Next
                    Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                        MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
                    End Try
                End While
            End Using
        Else
            StatusText1.Text = "Settings file not found."
            MsgBox("Diabotical settings not found. Please check that %appdata%/Diabotical/Settings.txt exists.")
        End If
        If HudSettings IsNot Nothing Then
            If SenderName = "ExportToFile" Then
                WriteHudFile(HudSettings)
            Else
                ShowHudJSON(HudSettings)
            End If
        Else
            StatusText1.Text = "Could not find Diabolical settings."
        End If
    End Sub

    'Helper functions
    Private Sub WriteHudFile(contents As String)
        Dim saveFileDialog1 As New SaveFileDialog With {
            .Filter = "dhud files (*.dhud)|*.dhud|All files (*.*)|*.*",
            .FilterIndex = 1,
            .RestoreDirectory = True
        }

        If saveFileDialog1.ShowDialog() = True Then
            File.WriteAllText(saveFileDialog1.FileName, contents)
            StatusText1.Text = "Saved HUD settings to " & saveFileDialog1.FileName
        End If
    End Sub
    Private Sub ShowHudJSON(contents As String)
        OutputDisplayBox.Text = contents
        OutputDisplayBox.Focus()
        OutputDisplayBox.SelectAll()
    End Sub
    Private Sub CopyDhudJSON()
        OutputDisplayBox.Focus()
        OutputDisplayBox.SelectAll()
        OutputDisplayBox.Copy()
        StatusText1.Text = "Copied HUD settings to clipboard."
    End Sub
    Private Sub BackupAllSettings(sender As Object, e As RoutedEventArgs)
        Dim appData As String = GetFolderPath(SpecialFolder.ApplicationData)
        Dim sourcePath = appData & "\Diabotical\Settings.txt"
        Dim saveFileDialog2 As New SaveFileDialog With {
            .Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
            .FilterIndex = 1,
            .RestoreDirectory = True
        }

        If saveFileDialog2.ShowDialog() = True Then
            If File.Exists(sourcePath) = True Then
                File.Copy(sourcePath, saveFileDialog2.FileName)
                StatusText1.Text = "Backed up Settings.txt to: " & saveFileDialog2.FileName
            Else
                StatusText1.Text = "Could not find Diabotical Settings."
            End If
        End If
    End Sub
    Private Sub ImportFromJSON(sender As Object, e As RoutedEventArgs)
        If InputDisplayBox.Text IsNot Nothing Then
            MsgBox(InputDisplayBox.Text)
        End If
    End Sub

    'Panel controls
    Private Sub ShowExportPanel(sender As Object, e As RoutedEventArgs)
        ExportPanel.Visibility = Visibility.Visible
        DefaultPanel.Visibility = Visibility.Collapsed
        BackupPanel.Visibility = Visibility.Collapsed
        ImportPanel.Visibility = Visibility.Collapsed

        ExportDhud(sender, e)
    End Sub
    Private Sub ShowImportPanel(sender As Object, e As RoutedEventArgs)
        ImportPanel.Visibility = Visibility.Visible
        ExportPanel.Visibility = Visibility.Collapsed
        BackupPanel.Visibility = Visibility.Collapsed
        DefaultPanel.Visibility = Visibility.Collapsed


    End Sub
    Private Sub ShowBackupPanel(sender As Object, e As RoutedEventArgs)
        BackupPanel.Visibility = Visibility.Visible
        ExportPanel.Visibility = Visibility.Collapsed
        DefaultPanel.Visibility = Visibility.Collapsed
        ImportPanel.Visibility = Visibility.Collapsed

    End Sub


End Class
