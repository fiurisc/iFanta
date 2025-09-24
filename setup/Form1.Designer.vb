<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Dim ILinePen1 As setup.iLinePen = New setup.iLinePen()
        Me.pic1 = New System.Windows.Forms.PictureBox()
        Me.lbname = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.lbinfo = New System.Windows.Forms.Label()
        Me.lbdest1 = New System.Windows.Forms.Label()
        Me.lbby = New System.Windows.Forms.Label()
        Me.lbstatus = New System.Windows.Forms.Label()
        Me.lbdest2 = New setup.iLabel()
        Me.btnfolder = New setup.iButton()
        Me.IButton2 = New setup.iButton()
        Me.IButton1 = New setup.iButton()
        Me.ILine1 = New setup.iLine()
        Me.prb1 = New setup.iProgressBar()
        Me.IForm1 = New setup.iFormControl()
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pic1
        '
        Me.pic1.Image = Global.setup.My.Resources.Resources.InstallIcon96
        Me.pic1.Location = New System.Drawing.Point(20, 60)
        Me.pic1.Margin = New System.Windows.Forms.Padding(0)
        Me.pic1.Name = "pic1"
        Me.pic1.Size = New System.Drawing.Size(70, 70)
        Me.pic1.TabIndex = 2
        Me.pic1.TabStop = False
        '
        'lbname
        '
        Me.lbname.Font = New System.Drawing.Font("Arial", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbname.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.lbname.Location = New System.Drawing.Point(105, 60)
        Me.lbname.Margin = New System.Windows.Forms.Padding(0)
        Me.lbname.Name = "lbname"
        Me.lbname.Size = New System.Drawing.Size(281, 52)
        Me.lbname.TabIndex = 4
        Me.lbname.Text = "iFanta rel 1.0"
        '
        'Timer1
        '
        Me.Timer1.Interval = 3000
        '
        'lbinfo
        '
        Me.lbinfo.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbinfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.lbinfo.Location = New System.Drawing.Point(109, 112)
        Me.lbinfo.Margin = New System.Windows.Forms.Padding(0)
        Me.lbinfo.Name = "lbinfo"
        Me.lbinfo.Size = New System.Drawing.Size(307, 50)
        Me.lbinfo.TabIndex = 5
        Me.lbinfo.Text = "Programma per la gestionedi uno o piu' tornei di fantacalcio"
        '
        'lbdest1
        '
        Me.lbdest1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbdest1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.lbdest1.Location = New System.Drawing.Point(109, 201)
        Me.lbdest1.Margin = New System.Windows.Forms.Padding(0)
        Me.lbdest1.Name = "lbdest1"
        Me.lbdest1.Size = New System.Drawing.Size(296, 27)
        Me.lbdest1.TabIndex = 6
        Me.lbdest1.Text = "Percorso di destinazione:"
        '
        'lbby
        '
        Me.lbby.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbby.AutoSize = True
        Me.lbby.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbby.ForeColor = System.Drawing.Color.FromArgb(CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.lbby.Location = New System.Drawing.Point(28, 347)
        Me.lbby.Margin = New System.Windows.Forms.Padding(0)
        Me.lbby.Name = "lbby"
        Me.lbby.Size = New System.Drawing.Size(31, 14)
        Me.lbby.TabIndex = 9
        Me.lbby.Text = "xxxx"
        '
        'lbstatus
        '
        Me.lbstatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbstatus.AutoSize = True
        Me.lbstatus.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbstatus.Location = New System.Drawing.Point(28, 246)
        Me.lbstatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbstatus.Name = "lbstatus"
        Me.lbstatus.Size = New System.Drawing.Size(38, 14)
        Me.lbstatus.TabIndex = 267
        Me.lbstatus.Text = "Ready"
        '
        'lbdest2
        '
        Me.lbdest2.AutoSizeStyle = setup.iLabel.eAutoSizeStyle.OnlyWitdh
        Me.lbdest2.Background = Nothing
        Me.lbdest2.BackgroundImage = CType(resources.GetObject("lbdest2.BackgroundImage"), System.Drawing.Image)
        Me.lbdest2.BorderColor = System.Drawing.Color.DimGray
        Me.lbdest2.BorderSize = 1
        Me.lbdest2.BorderVisible = True
        Me.lbdest2.Dull = False
        Me.lbdest2.Font = New System.Drawing.Font("Arial", 9.75!)
        Me.lbdest2.ForeColor = System.Drawing.Color.RoyalBlue
        Me.lbdest2.Image = Nothing
        Me.lbdest2.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left
        Me.lbdest2.InternalBorderColor = System.Drawing.Color.White
        Me.lbdest2.InternalBorderSize = 0
        Me.lbdest2.LineAlignment = System.Drawing.StringAlignment.Center
        Me.lbdest2.Location = New System.Drawing.Point(112, 228)
        Me.lbdest2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.lbdest2.Name = "lbdest2"
        Me.lbdest2.Size = New System.Drawing.Size(195, 20)
        Me.lbdest2.SuspundeLayout = False
        Me.lbdest2.TabIndex = 270
        Me.lbdest2.Text = "xxxxx"
        Me.lbdest2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        Me.lbdest2.TextAntiAlis = False
        Me.lbdest2.TextGlowEffetct = False
        Me.lbdest2.TextShadows = False
        Me.lbdest2.TextVertical = False
        Me.lbdest2.ToolTip = ""
        Me.lbdest2.WordWrap = False
        '
        'btnfolder
        '
        Me.btnfolder.BackColor = System.Drawing.Color.White
        Me.btnfolder.Background = Nothing
        Me.btnfolder.BackgroundImage = CType(resources.GetObject("btnfolder.BackgroundImage"), System.Drawing.Image)
        Me.btnfolder.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.btnfolder.BorderColorEnalbed = System.Drawing.Color.DimGray
        Me.btnfolder.BorderSize = 1
        Me.btnfolder.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.btnfolder.ColorLeave = System.Drawing.Color.White
        Me.btnfolder.ColorOver = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.btnfolder.Dull = False
        Me.btnfolder.FlatStyle = True
        Me.btnfolder.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnfolder.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnfolder.Image = Global.setup.My.Resources.Resources.folder16
        Me.btnfolder.Location = New System.Drawing.Point(323, 228)
        Me.btnfolder.Margin = New System.Windows.Forms.Padding(4)
        Me.btnfolder.Name = "btnfolder"
        Me.btnfolder.Size = New System.Drawing.Size(20, 20)
        Me.btnfolder.SuspundeLayout = False
        Me.btnfolder.TabIndex = 269
        Me.btnfolder.Text = "Botton"
        Me.btnfolder.TextAntiAlis = False
        Me.btnfolder.TextGlowEffetct = False
        Me.btnfolder.TextShadows = False
        Me.btnfolder.ToolTip = ""
        '
        'IButton2
        '
        Me.IButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IButton2.BackColor = System.Drawing.Color.White
        Me.IButton2.Background = Nothing
        Me.IButton2.BackgroundImage = CType(resources.GetObject("IButton2.BackgroundImage"), System.Drawing.Image)
        Me.IButton2.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.IButton2.BorderColorEnalbed = System.Drawing.Color.DarkGray
        Me.IButton2.BorderSize = 0
        Me.IButton2.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.IButton2.ColorLeave = System.Drawing.Color.White
        Me.IButton2.ColorOver = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.IButton2.Dull = False
        Me.IButton2.FlatStyle = True
        Me.IButton2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IButton2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.IButton2.Image = Global.setup.My.Resources.Resources.reload16
        Me.IButton2.Location = New System.Drawing.Point(237, 357)
        Me.IButton2.Margin = New System.Windows.Forms.Padding(4)
        Me.IButton2.Name = "IButton2"
        Me.IButton2.Size = New System.Drawing.Size(70, 20)
        Me.IButton2.SuspundeLayout = False
        Me.IButton2.TabIndex = 11
        Me.IButton2.Text = "Installa"
        Me.IButton2.TextAntiAlis = False
        Me.IButton2.TextGlowEffetct = False
        Me.IButton2.TextShadows = False
        Me.IButton2.ToolTip = ""
        '
        'IButton1
        '
        Me.IButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IButton1.BackColor = System.Drawing.Color.White
        Me.IButton1.Background = Nothing
        Me.IButton1.BackgroundImage = CType(resources.GetObject("IButton1.BackgroundImage"), System.Drawing.Image)
        Me.IButton1.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.IButton1.BorderColorEnalbed = System.Drawing.Color.DarkGray
        Me.IButton1.BorderSize = 0
        Me.IButton1.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.IButton1.ColorLeave = System.Drawing.Color.White
        Me.IButton1.ColorOver = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.IButton1.Dull = False
        Me.IButton1.FlatStyle = True
        Me.IButton1.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IButton1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.IButton1.Image = Global.setup.My.Resources.Resources.closer16
        Me.IButton1.Location = New System.Drawing.Point(335, 357)
        Me.IButton1.Margin = New System.Windows.Forms.Padding(4)
        Me.IButton1.Name = "IButton1"
        Me.IButton1.Size = New System.Drawing.Size(70, 20)
        Me.IButton1.SuspundeLayout = False
        Me.IButton1.TabIndex = 10
        Me.IButton1.Text = "Chiudi"
        Me.IButton1.TextAntiAlis = False
        Me.IButton1.TextGlowEffetct = False
        Me.IButton1.TextShadows = False
        Me.IButton1.ToolTip = ""
        '
        'ILine1
        '
        Me.ILine1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ILine1.BackgroundImage = CType(resources.GetObject("ILine1.BackgroundImage"), System.Drawing.Image)
        ILinePen1.Color = System.Drawing.Color.DimGray
        ILinePen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
        ILinePen1.Width = 1
        Me.ILine1.Line = ILinePen1
        Me.ILine1.Location = New System.Drawing.Point(32, 318)
        Me.ILine1.Margin = New System.Windows.Forms.Padding(4)
        Me.ILine1.Name = "ILine1"
        Me.ILine1.Size = New System.Drawing.Size(354, 10)
        Me.ILine1.SuspundeLayout = False
        Me.ILine1.TabIndex = 8
        Me.ILine1.TextAntiAlis = True
        Me.ILine1.TextShadows = False
        '
        'prb1
        '
        Me.prb1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prb1.BackColor = System.Drawing.Color.White
        Me.prb1.BackgroundImage = CType(resources.GetObject("prb1.BackgroundImage"), System.Drawing.Image)
        Me.prb1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.prb1.BarColor = System.Drawing.Color.Lime
        Me.prb1.BorderColor = System.Drawing.Color.Silver
        Me.prb1.Dull = True
        Me.prb1.FlatStyle = False
        Me.prb1.Location = New System.Drawing.Point(32, 281)
        Me.prb1.Margin = New System.Windows.Forms.Padding(0)
        Me.prb1.Max = 10.0R
        Me.prb1.Name = "prb1"
        Me.prb1.Size = New System.Drawing.Size(366, 21)
        Me.prb1.Smooth = True
        Me.prb1.SuspundeLayout = False
        Me.prb1.TabIndex = 1
        Me.prb1.Text = Nothing
        Me.prb1.Value = 0R
        '
        'IForm1
        '
        Me.IForm1.BackColor = System.Drawing.Color.White
        Me.IForm1.BH1 = 0
        Me.IForm1.BH2 = 0
        Me.IForm1.BH3 = 0
        Me.IForm1.BH4 = 0
        Me.IForm1.BH5 = 0
        Me.IForm1.BH6 = 0
        Me.IForm1.CloseButton = True
        Me.IForm1.ControlBox = False
        Me.IForm1.Cursor = System.Windows.Forms.Cursors.Default
        Me.IForm1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.IForm1.FlatStyle = False
        Me.IForm1.FontTitleWindows = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IForm1.ForeColorTitleWindows = System.Drawing.Color.White
        Me.IForm1.Icon = Nothing
        Me.IForm1.Location = New System.Drawing.Point(0, 0)
        Me.IForm1.LogoLeft = Nothing
        Me.IForm1.LogoRight = CType(resources.GetObject("IForm1.LogoRight"), System.Drawing.Image)
        Me.IForm1.Margin = New System.Windows.Forms.Padding(0)
        Me.IForm1.MaximizeButton = True
        Me.IForm1.MinimizeButton = True
        Me.IForm1.Name = "IForm1"
        Me.IForm1.Rel = ""
        Me.IForm1.ShowLogoLeft = True
        Me.IForm1.ShowLogoRight = True
        Me.IForm1.ShowSystemMenu = True
        Me.IForm1.Sizable = False
        Me.IForm1.Size = New System.Drawing.Size(447, 400)
        Me.IForm1.TabIndex = 0
        Me.IForm1.TH1 = 0
        Me.IForm1.TH2 = 1
        Me.IForm1.TH3 = 0
        Me.IForm1.TH4 = 0
        Me.IForm1.TH5 = 0
        Me.IForm1.TH6 = 0
        Me.IForm1.Theme = "Red"
        Me.IForm1.Title = "Form"
        Me.IForm1.TitleAlign = setup.iFormControl.align.Left
        Me.IForm1.WindowsClipping = True
        Me.IForm1.WindowsTitle = "Installazione"
        '
        'Form1
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(447, 400)
        Me.ControlBox = False
        Me.Controls.Add(Me.lbdest2)
        Me.Controls.Add(Me.btnfolder)
        Me.Controls.Add(Me.lbstatus)
        Me.Controls.Add(Me.IButton2)
        Me.Controls.Add(Me.IButton1)
        Me.Controls.Add(Me.lbby)
        Me.Controls.Add(Me.ILine1)
        Me.Controls.Add(Me.lbdest1)
        Me.Controls.Add(Me.lbinfo)
        Me.Controls.Add(Me.pic1)
        Me.Controls.Add(Me.prb1)
        Me.Controls.Add(Me.IForm1)
        Me.Controls.Add(Me.lbname)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Setup"
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents IForm1 As iFormControl
    Friend WithEvents prb1 As Setup.iProgressBar
    Friend WithEvents pic1 As System.Windows.Forms.PictureBox
    Friend WithEvents lbname As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents lbinfo As System.Windows.Forms.Label
    Friend WithEvents lbdest1 As System.Windows.Forms.Label
    Friend WithEvents ILine1 As setup.iLine
    Friend WithEvents lbby As System.Windows.Forms.Label
    Friend WithEvents IButton1 As setup.iButton
    Friend WithEvents IButton2 As setup.iButton
    Friend WithEvents lbstatus As System.Windows.Forms.Label
    Friend WithEvents btnfolder As iButton
    Friend WithEvents lbdest2 As iLabel
End Class
