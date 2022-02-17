Imports System.ComponentModel
Imports System.IO
Imports System.Diagnostics
Imports System.Text.RegularExpressions
Imports YoutubeExtractor
Public Class YTImport

    Public file As String
    Sub ProgressChangedHandler(sender, args)
        Console.WriteLine(args.ProgressPercentage)
    End Sub

    Private Sub ImportButton_Click(sender As Object, e As EventArgs) Handles ImportButton.Click
        Dim youtubeMatch As Match = New Regex("youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)").Match(TextBox1.Text)
        Dim fileName As Match = New Regex("[a-zA-Z0-9-]+").Match(TextBox2.Text)

        '////This code sends link to the downloader
        If youtubeMatch.Success And fileName.Success Then
            TextBox1.Enabled = False
            ImportButton.Enabled = False
            DownloadWorker.RunWorkerAsync("youtube.com/watch?v=" & youtubeMatch.Groups(1).Value)
            ToolStripStatusLabel1.Text = "Status: Downloading..."
        Else
            MessageBox.Show("Invalid YouTube URL or Invalid File Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TextBox1.Enabled = True
            ImportButton.Enabled = True
        End If
    End Sub

    Private Sub DownloadWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles DownloadWorker.DoWork
        Try

            If Not Directory.Exists(Path.GetFullPath("lagnos\")) Then
                Directory.CreateDirectory(Path.GetFullPath("lagnos\"))
            End If

            Dim filename As String = TextBox2.Text

            'Remove special characters from video name
            Regex.Replace(filename, "[^\w-_]", "")
            filename = filename.Replace(" ", "")

            ' Make the filename 15 chars long
            If filename.Length > 20 Then
                filename = filename.Substring(0, 20)
            End If


            Dim proc As Process = New Process()
            Dim name As String
            name = Path.GetFullPath("yt-dlp.exe")
            proc.StartInfo.FileName = name
            proc.StartInfo.Arguments = e.Argument & " -o lagnos\" & filename & ".mp4 --format mp4"
            proc.StartInfo.UseShellExecute = False
            proc.StartInfo.RedirectStandardOutput = False
            proc.StartInfo.CreateNoWindow = False
            proc.Start()
            proc.WaitForExit()

            e.Result = Path.GetFullPath("lagnos\" & filename & ".mp4")


        Catch ex As Exception
            Form1.LogError(ex)
            e.Result = ex
        End Try
    End Sub


    Private Sub DownloadWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles DownloadWorker.RunWorkerCompleted
        If TypeOf e.Result Is Exception Then
            MessageBox.Show(e.Result.Message & " See errorlog.txt for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            'this is the file to be sent............................................................
            file = e.Result
            DialogResult = Windows.Forms.DialogResult.OK
        End If
    End Sub

    Private Sub YTImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Select()
    End Sub

End Class