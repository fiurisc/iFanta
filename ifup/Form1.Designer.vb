<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.prb1 = New ifup.iProgressBar()
        Me.IForm1 = New ifup.iFormControl()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Enabled = False
        Me.PictureBox1.Image = Global.ifup.My.Resources.Resources._8
        Me.PictureBox1.Location = New System.Drawing.Point(12, 33)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(35, 35)
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Arial", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(47, 33)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(188, 16)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Aggiornamento in corso..."
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.DodgerBlue
        Me.Label2.Location = New System.Drawing.Point(47, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(246, 14)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Download file di configurazione in corso..."
        '
        'Timer1
        '
        Me.Timer1.Interval = 3000
        '
        'prb1
        '
        Me.prb1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prb1.BackColor = System.Drawing.Color.LightGray
        Me.prb1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.prb1.BarColor = System.Drawing.Color.DodgerBlue
        Me.prb1.BorderColor = System.Drawing.Color.Silver
        Me.prb1.Dull = True
        Me.prb1.FlatStyle = False
        Me.prb1.Location = New System.Drawing.Point(12, 70)
        Me.prb1.Margin = New System.Windows.Forms.Padding(0)
        Me.prb1.Max = 10.0R
        Me.prb1.Name = "prb1"
        Me.prb1.Size = New System.Drawing.Size(281, 17)
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
        Me.IForm1.FontTitleWindows = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
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
        Me.IForm1.Size = New System.Drawing.Size(302, 106)
        Me.IForm1.TabIndex = 0
        Me.IForm1.TH1 = 0
        Me.IForm1.TH2 = 1
        Me.IForm1.TH3 = 0
        Me.IForm1.TH4 = 0
        Me.IForm1.TH5 = 0
        Me.IForm1.TH6 = 0
        Me.IForm1.Theme = "Red"
        Me.IForm1.Title = "Form"
        Me.IForm1.TitleAlign = ifup.iFormControl.align.Left
        Me.IForm1.WindowsClipping = True
        Me.IForm1.WindowsTitle = "Aggiornamento..."
        '
        'Form1
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(302, 106)
        Me.ControlBox = False
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.prb1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.IForm1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Aggiornamento..."
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents IForm1 As iFormControl
    Friend WithEvents prb1 As ifup.iProgressBar
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
End Class
