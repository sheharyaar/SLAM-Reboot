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
        '////This code sends link to the downloader
        If youtubeMatch.Success Then
            TextBox1.Enabled = False
            ImportButton.Enabled = False
            DownloadWorker.RunWorkerAsync("youtube.com/watch?v=" & youtubeMatch.Groups(1).Value)
            ToolStripStatusLabel1.Text = "Status: Downloading..."
        Else
            MessageBox.Show("Invalid YouTube URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TextBox1.Enabled = True
            ImportButton.Enabled = True
        End If
    End Sub

    Private Sub DownloadWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles DownloadWorker.DoWork
        Try
            'Dim videoInfos As IEnumerable(Of VideoInfo) = DownloadUrlResolver.GetDownloadUrls(e.Argument).OrderBy(Function(vid) vid.Resolution)

            '////To understand this part-----------------------
            'Dim video As VideoInfo = videoInfos.First(Function(info) info.AdaptiveType = AdaptiveType.Audio AndAlso info.AudioType = AudioType.Aac OrElse info.AdaptiveType = AdaptiveType.None AndAlso info.VideoType = VideoType.Mp4 AndAlso info.AudioBitrate >= 128)
            'If IsNothing(video) Then
            '    If videoInfos.Any(Function(info) info.AdaptiveType = AdaptiveType.None AndAlso info.VideoType = VideoType.Mp4) Then
            '        video = videoInfos.First(Function(info) info.AdaptiveType = AdaptiveType.None AndAlso info.VideoType = VideoType.Mp4)
            '    Else
            '        Throw New System.Exception("Could not find download.")
            '    End If
            'End If

            'If video.RequiresDecryption Then
            '    DownloadUrlResolver.DecryptDownloadUrl(video)
            'End If

            If Not Directory.Exists(Path.GetFullPath("lagnos\")) Then
                Directory.CreateDirectory(Path.GetFullPath("lagnos\"))
            End If

            Dim filename As String = ""

            ' Get the name of the video using -e option
            Dim proc2 As Process = New Process()
            Dim name2 As String
            name2 = Path.GetFullPath("yt-dlp.exe")
            proc2.StartInfo.FileName = name2
            proc2.StartInfo.Arguments = e.Argument & " -e"
            proc2.StartInfo.UseShellExecute = False
            proc2.StartInfo.RedirectStandardOutput = True
            proc2.StartInfo.CreateNoWindow = True
            proc2.Start()
            While proc2.StandardOutput.EndOfStream = False
                filename = proc2.StandardOutput.ReadLine()
            End While
            proc2.WaitForExit()

            'File name exceeding Limit causing errors - shorten yt video name

            filename = filename.Replace("|", "")
            filename = filename.Replace("<", "")
            filename = filename.Replace(">", "")
            filename = filename.Replace(":", "")
            filename = filename.Replace("\", "")
            filename = filename.Replace("/", "")
            filename = filename.Replace(" ", "")
            filename = filename.Replace("|", "")
            filename = filename.Replace("?", "")
            filename = filename.Replace("*", "")
            filename = filename.Replace("-", "")
            filename = filename.Replace("*", "")

            ' Make the filename 15 chars long
            filename = filename.Substring(0, 15)


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

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub DonateLabel_Click(sender As Object, e As EventArgs)

    End Sub

End Class