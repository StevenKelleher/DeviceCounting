﻿
Imports Cognex.VisionPro
Imports Cognex.VisionPro.ImageProcessing
Imports Cognex.VisionPro.PMAlign
Imports Cognex.VisionPro.ImageFile
Imports System.IO
Imports Cognex.VisionPro.Display

Public Class Form1


    Dim FilePath As DialogResult
    Dim S_PMAlign As New CogPMAlignTool
    Dim NewImageFile As New CogImageFile
    Dim ImageConverter As New CogImageConvertTool
    Dim FileOpened As Boolean = False
    Dim FileOpened2 As Boolean = False
    Dim LPattern As Bitmap

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal
        FolderBrowserDialog1.ShowNewFolderButton = True
        FolderBrowserDialog2.RootFolder = Environment.SpecialFolder.Personal
        FolderBrowserDialog2.ShowNewFolderButton = True

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        If (CheckBox1.Checked = True) Then
            S_PMAlign.RunParams.ScoreUsingClutter = True
        Else
            S_PMAlign.RunParams.ScoreUsingClutter = False
        End If
        Try
            ImageConverter.InputImage = NewImageFile(0)
            ImageConverter.RunParams.RunMode = CogImageConvertRunModeConstants.Intensity
            ImageConverter.Run()

            ToolStripStatusLabel1.Text = ImageConverter.RunStatus.Message

            S_PMAlign.InputImage = ImageConverter.OutputImage
            S_PMAlign.RunParams.AcceptThreshold = NumericUpDown2.Value
            S_PMAlign.RunParams.ApproximateNumberToFind = NumericUpDown1.Value

            S_PMAlign.Run()

            ToolStripStatusLabel1.Text = S_PMAlign.RunStatus.Message

            TextBox1.Text = (S_PMAlign.Results.Count / NumericUpDown3.Value) * 100

        Catch exp As Exception

            If (Not FileOpened) Then
                MessageBox.Show("Please load Image and Model files" _
                                + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine)
            End If

            FileOpened = False
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        OpenFileDialog1.Filter = "Bitmap image (.bmp) |*.bmp"

        If (Not FileOpened) Then
            OpenFileDialog1.InitialDirectory = FolderBrowserDialog1.RootFolder
            OpenFileDialog1.FileName = Nothing

        End If

        Try

            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                Dim LImage As New StreamReader(OpenFileDialog1.FileName)
                NewImageFile.Open(OpenFileDialog1.FileName, CogImageFileModeConstants.Read)
                PictureBox1.Load(OpenFileDialog1.FileName)
                LImage.Close()
            End If

            FileOpened = True

        Catch exp As Exception
            MessageBox.Show("An error occurred while attempting to load the file. The error is:" _
                                + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine)
            FileOpened = False
            PictureBox1.Image = PictureBox1.ErrorImage
        End Try

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        OpenFileDialog2.Filter = "VPP File (.vpp) |*.vpp"

        If (Not FileOpened2) Then
            OpenFileDialog2.InitialDirectory = FolderBrowserDialog2.RootFolder
            OpenFileDialog2.FileName = Nothing

        End If

        Try

            If OpenFileDialog2.ShowDialog() = DialogResult.OK Then

                Dim SavePattern As New CogPMAlignPattern
                SavePattern = CogSerializer.LoadObjectFromFile(OpenFileDialog2.FileName)
                S_PMAlign.Pattern = CogSerializer.LoadObjectFromFile(OpenFileDialog2.FileName)
                LPattern = SavePattern.GetTrainedPatternImage.ToBitmap()
                PictureBox2.Image = LPattern
                ToolStripStatusLabel1.Text = S_PMAlign.RunStatus.Message
            End If

            FileOpened2 = True

        Catch exp As Exception
            MessageBox.Show("An error occurred while attempting to load the file. The error is:" _
                               + System.Environment.NewLine + exp.ToString() + System.Environment.NewLine)
            PictureBox2.Image = PictureBox2.ErrorImage
            FileOpened2 = False
        End Try

    End Sub


End Class
