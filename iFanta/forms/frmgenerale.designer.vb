<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmgenerale
    Inherits iFantaForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmgenerale))
        Dim ToolbarButton1 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton2 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton3 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton4 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton5 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton6 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton7 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Dim ToolbarButton8 As iControl.ToolbarButton = New iControl.ToolbarButton()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.mnu1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.F1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.pictheme = New System.Windows.Forms.PictureBox()
        Me.picfind = New System.Windows.Forms.PictureBox()
        Me.picapp = New System.Windows.Forms.PictureBox()
        Me.picmenu = New System.Windows.Forms.PictureBox()
        Me.cnt3 = New System.Windows.Forms.PictureBox()
        Me.cnt2 = New System.Windows.Forms.PictureBox()
        Me.cnt1 = New System.Windows.Forms.PictureBox()
        Me.pic7 = New System.Windows.Forms.PictureBox()
        Me.pic6 = New System.Windows.Forms.PictureBox()
        Me.pic5 = New System.Windows.Forms.PictureBox()
        Me.pic4 = New System.Windows.Forms.PictureBox()
        Me.pic3 = New System.Windows.Forms.PictureBox()
        Me.pic2 = New System.Windows.Forms.PictureBox()
        Me.pic1 = New System.Windows.Forms.PictureBox()
        Me.pic0 = New System.Windows.Forms.PictureBox()
        Me.picback = New System.Windows.Forms.PictureBox()
        Me.vsrc2 = New iControl.iVScroll()
        Me.cmdrest = New iControl.iButton()
        Me.cmdconne = New iControl.iButton()
        Me.cmdupd = New iControl.iButton()
        Me.tlbmenu = New iControl.iToolBar()
        Me.tlb1 = New iControl.iToolBar()
        Me.txt3 = New iControl.iText()
        Me.vsrc1 = New iControl.iVScroll()
        Me.mnu1.SuspendLayout()
        CType(Me.pictheme, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picfind, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picapp, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picmenu, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cnt3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cnt2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cnt1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic7, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic6, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic0, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picback, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'mnu1
        '
        Me.mnu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.F1ToolStripMenuItem})
        Me.mnu1.Name = "mnu1"
        Me.mnu1.Size = New System.Drawing.Size(87, 26)
        '
        'F1ToolStripMenuItem
        '
        Me.F1ToolStripMenuItem.Name = "F1ToolStripMenuItem"
        Me.F1ToolStripMenuItem.Size = New System.Drawing.Size(86, 22)
        Me.F1ToolStripMenuItem.Text = "F1"
        '
        'Timer2
        '
        Me.Timer2.Interval = 50
        '
        'pictheme
        '
        Me.pictheme.Location = New System.Drawing.Point(276, 212)
        Me.pictheme.Name = "pictheme"
        Me.pictheme.Size = New System.Drawing.Size(105, 61)
        Me.pictheme.TabIndex = 185
        Me.pictheme.TabStop = False
        Me.pictheme.Visible = False
        '
        'picfind
        '
        Me.picfind.Image = Global.iFanta.My.Resources.Resources.search1
        Me.picfind.Location = New System.Drawing.Point(130, 293)
        Me.picfind.Name = "picfind"
        Me.picfind.Size = New System.Drawing.Size(16, 16)
        Me.picfind.TabIndex = 168
        Me.picfind.TabStop = False
        '
        'picapp
        '
        Me.picapp.Location = New System.Drawing.Point(57, 212)
        Me.picapp.Name = "picapp"
        Me.picapp.Size = New System.Drawing.Size(123, 61)
        Me.picapp.TabIndex = 164
        Me.picapp.TabStop = False
        Me.picapp.Visible = False
        '
        'picmenu
        '
        Me.picmenu.BackColor = System.Drawing.Color.Blue
        Me.picmenu.Location = New System.Drawing.Point(73, 76)
        Me.picmenu.Name = "picmenu"
        Me.picmenu.Size = New System.Drawing.Size(107, 24)
        Me.picmenu.TabIndex = 162
        Me.picmenu.TabStop = False
        Me.picmenu.Visible = False
        '
        'cnt3
        '
        Me.cnt3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cnt3.Location = New System.Drawing.Point(253, 38)
        Me.cnt3.Name = "cnt3"
        Me.cnt3.Size = New System.Drawing.Size(42, 16)
        Me.cnt3.TabIndex = 145
        Me.cnt3.TabStop = False
        '
        'cnt2
        '
        Me.cnt2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cnt2.Location = New System.Drawing.Point(221, 38)
        Me.cnt2.Name = "cnt2"
        Me.cnt2.Size = New System.Drawing.Size(26, 16)
        Me.cnt2.TabIndex = 144
        Me.cnt2.TabStop = False
        '
        'cnt1
        '
        Me.cnt1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cnt1.Location = New System.Drawing.Point(185, 38)
        Me.cnt1.Name = "cnt1"
        Me.cnt1.Size = New System.Drawing.Size(28, 16)
        Me.cnt1.TabIndex = 143
        Me.cnt1.TabStop = False
        '
        'pic7
        '
        Me.pic7.Location = New System.Drawing.Point(84, 159)
        Me.pic7.Name = "pic7"
        Me.pic7.Size = New System.Drawing.Size(28, 19)
        Me.pic7.TabIndex = 29
        Me.pic7.TabStop = False
        '
        'pic6
        '
        Me.pic6.Location = New System.Drawing.Point(84, 184)
        Me.pic6.Name = "pic6"
        Me.pic6.Size = New System.Drawing.Size(28, 19)
        Me.pic6.TabIndex = 28
        Me.pic6.TabStop = False
        '
        'pic5
        '
        Me.pic5.Location = New System.Drawing.Point(118, 184)
        Me.pic5.Name = "pic5"
        Me.pic5.Size = New System.Drawing.Size(28, 19)
        Me.pic5.TabIndex = 27
        Me.pic5.TabStop = False
        '
        'pic4
        '
        Me.pic4.Location = New System.Drawing.Point(152, 184)
        Me.pic4.Name = "pic4"
        Me.pic4.Size = New System.Drawing.Size(28, 19)
        Me.pic4.TabIndex = 26
        Me.pic4.TabStop = False
        '
        'pic3
        '
        Me.pic3.Location = New System.Drawing.Point(152, 159)
        Me.pic3.Name = "pic3"
        Me.pic3.Size = New System.Drawing.Size(28, 19)
        Me.pic3.TabIndex = 25
        Me.pic3.TabStop = False
        '
        'pic2
        '
        Me.pic2.Location = New System.Drawing.Point(152, 134)
        Me.pic2.Name = "pic2"
        Me.pic2.Size = New System.Drawing.Size(28, 19)
        Me.pic2.TabIndex = 24
        Me.pic2.TabStop = False
        '
        'pic1
        '
        Me.pic1.Location = New System.Drawing.Point(118, 134)
        Me.pic1.Name = "pic1"
        Me.pic1.Size = New System.Drawing.Size(28, 19)
        Me.pic1.TabIndex = 23
        Me.pic1.TabStop = False
        '
        'pic0
        '
        Me.pic0.Location = New System.Drawing.Point(84, 134)
        Me.pic0.Name = "pic0"
        Me.pic0.Size = New System.Drawing.Size(28, 19)
        Me.pic0.TabIndex = 22
        Me.pic0.TabStop = False
        '
        'picback
        '
        Me.picback.Location = New System.Drawing.Point(31, 30)
        Me.picback.Name = "picback"
        Me.picback.Size = New System.Drawing.Size(386, 304)
        Me.picback.TabIndex = 19
        Me.picback.TabStop = False
        '
        'vsrc2
        '
        Me.vsrc2.BackColor = System.Drawing.Color.Transparent
        Me.vsrc2.Background = Nothing
        Me.vsrc2.BackgroundImage = CType(resources.GetObject("vsrc2.BackgroundImage"), System.Drawing.Image)
        Me.vsrc2.BorderColor = System.Drawing.Color.LightGray
        Me.vsrc2.BorderSize = 0
        Me.vsrc2.Dull = True
        Me.vsrc2.FlatStyle = False
        Me.vsrc2.Location = New System.Drawing.Point(393, 159)
        Me.vsrc2.Max = 1
        Me.vsrc2.Min = 0
        Me.vsrc2.MinimumSize = New System.Drawing.Size(6, 30)
        Me.vsrc2.Name = "vsrc2"
        Me.vsrc2.ShowArrow = True
        Me.vsrc2.Size = New System.Drawing.Size(9, 162)
        Me.vsrc2.SmallChange = 1
        Me.vsrc2.SuspundeLayout = False
        Me.vsrc2.TabIndex = 186
        Me.vsrc2.Value = 0
        Me.vsrc2.Visible = False
        '
        'cmdrest
        '
        Me.cmdrest.BackColor = System.Drawing.Color.White
        Me.cmdrest.Background = Nothing
        Me.cmdrest.BackgroundImage = CType(resources.GetObject("cmdrest.BackgroundImage"), System.Drawing.Image)
        Me.cmdrest.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.cmdrest.BorderColorEnalbed = System.Drawing.Color.Silver
        Me.cmdrest.BorderSize = 1
        Me.cmdrest.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.cmdrest.ColorLeave = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.cmdrest.ColorOver = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.cmdrest.Dull = False
        Me.cmdrest.FlatStyle = False
        Me.cmdrest.Image = Nothing
        Me.cmdrest.Location = New System.Drawing.Point(319, 385)
        Me.cmdrest.Name = "cmdrest"
        Me.cmdrest.Size = New System.Drawing.Size(62, 18)
        Me.cmdrest.SuspundeLayout = False
        Me.cmdrest.TabIndex = 183
        Me.cmdrest.Text = "Ripristina"
        Me.cmdrest.TextAntiAlis = False
        Me.cmdrest.TextGlowEffetct = False
        Me.cmdrest.TextShadows = False
        Me.cmdrest.ToolTip = ""
        Me.cmdrest.Visible = False
        '
        'cmdconne
        '
        Me.cmdconne.BackColor = System.Drawing.Color.Red
        Me.cmdconne.Background = Nothing
        Me.cmdconne.BackgroundImage = CType(resources.GetObject("cmdconne.BackgroundImage"), System.Drawing.Image)
        Me.cmdconne.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.cmdconne.BorderColorEnalbed = System.Drawing.Color.Silver
        Me.cmdconne.BorderSize = 1
        Me.cmdconne.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.cmdconne.ColorLeave = System.Drawing.Color.Transparent
        Me.cmdconne.ColorOver = System.Drawing.Color.Black
        Me.cmdconne.Dull = False
        Me.cmdconne.FlatStyle = False
        Me.cmdconne.Image = Nothing
        Me.cmdconne.Location = New System.Drawing.Point(387, 361)
        Me.cmdconne.Name = "cmdconne"
        Me.cmdconne.Size = New System.Drawing.Size(62, 18)
        Me.cmdconne.SuspundeLayout = False
        Me.cmdconne.TabIndex = 181
        Me.cmdconne.Text = "Verifica"
        Me.cmdconne.TextAntiAlis = False
        Me.cmdconne.TextGlowEffetct = False
        Me.cmdconne.TextShadows = False
        Me.cmdconne.ToolTip = ""
        Me.cmdconne.Visible = False
        '
        'cmdupd
        '
        Me.cmdupd.BackColor = System.Drawing.Color.White
        Me.cmdupd.Background = Nothing
        Me.cmdupd.BackgroundImage = CType(resources.GetObject("cmdupd.BackgroundImage"), System.Drawing.Image)
        Me.cmdupd.BorderColorDisabled = System.Drawing.Color.Gainsboro
        Me.cmdupd.BorderColorEnalbed = System.Drawing.Color.Silver
        Me.cmdupd.BorderSize = 1
        Me.cmdupd.ColorDisabled = System.Drawing.Color.WhiteSmoke
        Me.cmdupd.ColorLeave = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.cmdupd.ColorOver = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.cmdupd.Dull = False
        Me.cmdupd.FlatStyle = False
        Me.cmdupd.Image = Nothing
        Me.cmdupd.Location = New System.Drawing.Point(387, 385)
        Me.cmdupd.Name = "cmdupd"
        Me.cmdupd.Size = New System.Drawing.Size(62, 18)
        Me.cmdupd.SuspundeLayout = False
        Me.cmdupd.TabIndex = 180
        Me.cmdupd.Text = "Aggiorna"
        Me.cmdupd.TextAntiAlis = False
        Me.cmdupd.TextGlowEffetct = False
        Me.cmdupd.TextShadows = False
        Me.cmdupd.ToolTip = ""
        Me.cmdupd.Visible = False
        '
        'tlbmenu
        '
        Me.tlbmenu.AutoSize = True
        Me.tlbmenu.AutoSizeMaxWidth = -1.0!
        Me.tlbmenu.AutoSizeMinWidth = -1.0!
        Me.tlbmenu.BackColor = System.Drawing.Color.White
        Me.tlbmenu.Background = CType(resources.GetObject("tlbmenu.Background"), System.Drawing.Image)
        Me.tlbmenu.BackgroundImage = CType(resources.GetObject("tlbmenu.BackgroundImage"), System.Drawing.Image)
        Me.tlbmenu.BorderColor = System.Drawing.Color.Gray
        Me.tlbmenu.BorderColorDropDown = System.Drawing.Color.Gray
        ToolbarButton1.BackColor = System.Drawing.Color.Empty
        ToolbarButton1.BorderColor = System.Drawing.Color.Silver
        ToolbarButton1.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton1.BorderVisible = False
        ToolbarButton1.DefaultFont = Nothing
        ToolbarButton1.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton1.DefaultRowItemHeight = 0
        ToolbarButton1.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton1.DefaultTitleFont = Nothing
        ToolbarButton1.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton1.DropDown = False
        ToolbarButton1.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton1.DropDownMenuColoumnsCount = 1
        ToolbarButton1.Enabled = True
        ToolbarButton1.EnableSubItemsSelection = True
        ToolbarButton1.Font = Nothing
        ToolbarButton1.ForeColor = System.Drawing.Color.Empty
        ToolbarButton1.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton1.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton1.FullRowSelect = False
        ToolbarButton1.Image = Nothing
        ToolbarButton1.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton1.InternalBorderVisible = False
        ToolbarButton1.MaxDropDownItems = 0
        ToolbarButton1.MaxSubWidth = -1
        ToolbarButton1.MaxWidth = -1
        ToolbarButton1.MenuHorizontalAlignment = -1
        ToolbarButton1.MenuVerticalAlignment = -1
        ToolbarButton1.MinSubWidth = -1
        ToolbarButton1.MinWidth = -1
        ToolbarButton1.Name = ""
        ToolbarButton1.RowsHeightAutoSize = True
        ToolbarButton1.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton1.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton1.ShowColumnCheck = False
        ToolbarButton1.ShowColumnImage = False
        ToolbarButton1.State = False
        ToolbarButton1.SubColumnWidth = CType(resources.GetObject("ToolbarButton1.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton1.SubHeight = 100
        ToolbarButton1.SubItemsAutoSize = False
        ToolbarButton1.SubWidth = 100
        ToolbarButton1.Tag = Nothing
        ToolbarButton1.Text = "Menu"
        ToolbarButton1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton1.ToolTips = ""
        ToolbarButton1.Type = iControl.ToolbarButton.iType.ButtonDropDown
        ToolbarButton1.Visible = True
        ToolbarButton1.Width = 47
        Me.tlbmenu.Button.Add(ToolbarButton1)
        Me.tlbmenu.Dull = False
        Me.tlbmenu.EnabledSelection = False
        Me.tlbmenu.FlatStyle = False
        Me.tlbmenu.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.tlbmenu.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlbmenu.ForeColorDropDown = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlbmenu.ForeColorSelect = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlbmenu.Location = New System.Drawing.Point(253, 100)
        Me.tlbmenu.MaxDropDownItems = 8
        Me.tlbmenu.MenuHorizontalAlignment = 0
        Me.tlbmenu.MenuVerticalAlignment = 0
        Me.tlbmenu.MultiSelection = False
        Me.tlbmenu.Name = "tlbmenu"
        Me.tlbmenu.ReadOnly = False
        Me.tlbmenu.SelectionColor = System.Drawing.Color.White
        Me.tlbmenu.SelectionColorDropDown = System.Drawing.Color.White
        Me.tlbmenu.SeparatorColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.tlbmenu.Size = New System.Drawing.Size(50, 24)
        Me.tlbmenu.SubItemsMultiSelection = False
        Me.tlbmenu.SuspundeLayout = False
        Me.tlbmenu.TabIndex = 172
        Me.tlbmenu.TextAntiAlis = False
        Me.tlbmenu.TextGlowEffetct = False
        Me.tlbmenu.TextShadows = False
        Me.tlbmenu.ToolbarBorderColor = System.Drawing.Color.DimGray
        Me.tlbmenu.ToolbarBorderVisible = False
        Me.tlbmenu.ToolbarPadding = 1
        Me.tlbmenu.ToolTipDown = True
        Me.tlbmenu.Visible = False
        '
        'tlb1
        '
        Me.tlb1.AutoSize = True
        Me.tlb1.AutoSizeMaxWidth = -1.0!
        Me.tlb1.AutoSizeMinWidth = -1.0!
        Me.tlb1.Background = CType(resources.GetObject("tlb1.Background"), System.Drawing.Image)
        Me.tlb1.BackgroundImage = CType(resources.GetObject("tlb1.BackgroundImage"), System.Drawing.Image)
        Me.tlb1.BorderColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlb1.BorderColorDropDown = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        ToolbarButton2.BackColor = System.Drawing.Color.Transparent
        ToolbarButton2.BorderColor = System.Drawing.Color.Silver
        ToolbarButton2.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton2.BorderVisible = False
        ToolbarButton2.DefaultFont = Nothing
        ToolbarButton2.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton2.DefaultRowItemHeight = 0
        ToolbarButton2.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton2.DefaultTitleFont = Nothing
        ToolbarButton2.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton2.DropDown = False
        ToolbarButton2.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton2.DropDownMenuColoumnsCount = 1
        ToolbarButton2.Enabled = True
        ToolbarButton2.EnableSubItemsSelection = True
        ToolbarButton2.Font = Nothing
        ToolbarButton2.ForeColor = System.Drawing.Color.Empty
        ToolbarButton2.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton2.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton2.FullRowSelect = False
        ToolbarButton2.Image = Nothing
        ToolbarButton2.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton2.InternalBorderVisible = False
        ToolbarButton2.MaxDropDownItems = 0
        ToolbarButton2.MaxSubWidth = -1
        ToolbarButton2.MaxWidth = -1
        ToolbarButton2.MenuHorizontalAlignment = -1
        ToolbarButton2.MenuVerticalAlignment = -1
        ToolbarButton2.MinSubWidth = -1
        ToolbarButton2.MinWidth = -1
        ToolbarButton2.Name = ""
        ToolbarButton2.RowsHeightAutoSize = True
        ToolbarButton2.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton2.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton2.ShowColumnCheck = False
        ToolbarButton2.ShowColumnImage = False
        ToolbarButton2.State = False
        ToolbarButton2.SubColumnWidth = CType(resources.GetObject("ToolbarButton2.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton2.SubHeight = 100
        ToolbarButton2.SubItemsAutoSize = False
        ToolbarButton2.SubWidth = 100
        ToolbarButton2.Tag = ""
        ToolbarButton2.Text = ""
        ToolbarButton2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton2.ToolTips = ""
        ToolbarButton2.Type = iControl.ToolbarButton.iType.Separator
        ToolbarButton2.Visible = True
        ToolbarButton2.Width = 6
        ToolbarButton3.BackColor = System.Drawing.Color.Empty
        ToolbarButton3.BorderColor = System.Drawing.Color.Silver
        ToolbarButton3.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton3.BorderVisible = False
        ToolbarButton3.DefaultFont = Nothing
        ToolbarButton3.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton3.DefaultRowItemHeight = 0
        ToolbarButton3.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton3.DefaultTitleFont = Nothing
        ToolbarButton3.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton3.DropDown = False
        ToolbarButton3.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton3.DropDownMenuColoumnsCount = 1
        ToolbarButton3.Enabled = True
        ToolbarButton3.EnableSubItemsSelection = True
        ToolbarButton3.Font = Nothing
        ToolbarButton3.ForeColor = System.Drawing.Color.Empty
        ToolbarButton3.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton3.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton3.FullRowSelect = False
        ToolbarButton3.Image = Global.iFanta.My.Resources.Resources.windws_size_white
        ToolbarButton3.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton3.InternalBorderVisible = False
        ToolbarButton3.MaxDropDownItems = 0
        ToolbarButton3.MaxSubWidth = -1
        ToolbarButton3.MaxWidth = -1
        ToolbarButton3.MenuHorizontalAlignment = -1
        ToolbarButton3.MenuVerticalAlignment = -1
        ToolbarButton3.MinSubWidth = -1
        ToolbarButton3.MinWidth = -1
        ToolbarButton3.Name = ""
        ToolbarButton3.RowsHeightAutoSize = True
        ToolbarButton3.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton3.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton3.ShowColumnCheck = True
        ToolbarButton3.ShowColumnImage = False
        ToolbarButton3.State = False
        ToolbarButton3.SubColumnWidth = CType(resources.GetObject("ToolbarButton3.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton3.SubHeight = 100
        ToolbarButton3.SubItemsAutoSize = True
        ToolbarButton3.SubWidth = 100
        ToolbarButton3.Tag = Nothing
        ToolbarButton3.Text = ""
        ToolbarButton3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton3.ToolTips = "Dimensione finestra"
        ToolbarButton3.Type = iControl.ToolbarButton.iType.ButtonDropDown
        ToolbarButton3.Visible = True
        ToolbarButton3.Width = 32
        ToolbarButton4.BackColor = System.Drawing.Color.Transparent
        ToolbarButton4.BorderColor = System.Drawing.Color.Silver
        ToolbarButton4.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton4.BorderVisible = False
        ToolbarButton4.DefaultFont = Nothing
        ToolbarButton4.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton4.DefaultRowItemHeight = 0
        ToolbarButton4.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton4.DefaultTitleFont = Nothing
        ToolbarButton4.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton4.DropDown = False
        ToolbarButton4.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton4.DropDownMenuColoumnsCount = 1
        ToolbarButton4.Enabled = True
        ToolbarButton4.EnableSubItemsSelection = True
        ToolbarButton4.Font = Nothing
        ToolbarButton4.ForeColor = System.Drawing.Color.Empty
        ToolbarButton4.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton4.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton4.FullRowSelect = False
        ToolbarButton4.Image = Global.iFanta.My.Resources.Resources.detail_16
        ToolbarButton4.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton4.InternalBorderVisible = False
        ToolbarButton4.MaxDropDownItems = 0
        ToolbarButton4.MaxSubWidth = -1
        ToolbarButton4.MaxWidth = -1
        ToolbarButton4.MenuHorizontalAlignment = -1
        ToolbarButton4.MenuVerticalAlignment = -1
        ToolbarButton4.MinSubWidth = -1
        ToolbarButton4.MinWidth = -1
        ToolbarButton4.Name = ""
        ToolbarButton4.RowsHeightAutoSize = True
        ToolbarButton4.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton4.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton4.ShowColumnCheck = True
        ToolbarButton4.ShowColumnImage = False
        ToolbarButton4.State = False
        ToolbarButton4.SubColumnWidth = CType(resources.GetObject("ToolbarButton4.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton4.SubHeight = 100
        ToolbarButton4.SubItemsAutoSize = True
        ToolbarButton4.SubWidth = 110
        ToolbarButton4.Tag = ""
        ToolbarButton4.Text = ""
        ToolbarButton4.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton4.ToolTips = "Imposta la modalitá di visualizzazione"
        ToolbarButton4.Type = iControl.ToolbarButton.iType.ButtonDropDown
        ToolbarButton4.Visible = True
        ToolbarButton4.Width = 32
        ToolbarButton5.BackColor = System.Drawing.Color.Empty
        ToolbarButton5.BorderColor = System.Drawing.Color.Silver
        ToolbarButton5.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton5.BorderVisible = False
        ToolbarButton5.DefaultFont = Nothing
        ToolbarButton5.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton5.DefaultRowItemHeight = 0
        ToolbarButton5.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton5.DefaultTitleFont = Nothing
        ToolbarButton5.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton5.DropDown = False
        ToolbarButton5.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton5.DropDownMenuColoumnsCount = 1
        ToolbarButton5.Enabled = True
        ToolbarButton5.EnableSubItemsSelection = True
        ToolbarButton5.Font = Nothing
        ToolbarButton5.ForeColor = System.Drawing.Color.Empty
        ToolbarButton5.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton5.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton5.FullRowSelect = False
        ToolbarButton5.Image = Global.iFanta.My.Resources.Resources.font_size16
        ToolbarButton5.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton5.InternalBorderVisible = False
        ToolbarButton5.MaxDropDownItems = 0
        ToolbarButton5.MaxSubWidth = -1
        ToolbarButton5.MaxWidth = -1
        ToolbarButton5.MenuHorizontalAlignment = -1
        ToolbarButton5.MenuVerticalAlignment = -1
        ToolbarButton5.MinSubWidth = -1
        ToolbarButton5.MinWidth = -1
        ToolbarButton5.Name = ""
        ToolbarButton5.RowsHeightAutoSize = True
        ToolbarButton5.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton5.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton5.ShowColumnCheck = True
        ToolbarButton5.ShowColumnImage = False
        ToolbarButton5.State = False
        ToolbarButton5.SubColumnWidth = CType(resources.GetObject("ToolbarButton5.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton5.SubHeight = 100
        ToolbarButton5.SubItemsAutoSize = True
        ToolbarButton5.SubWidth = 100
        ToolbarButton5.Tag = Nothing
        ToolbarButton5.Text = ""
        ToolbarButton5.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton5.ToolTips = "Dimensioni carattere"
        ToolbarButton5.Type = iControl.ToolbarButton.iType.ButtonDropDown
        ToolbarButton5.Visible = True
        ToolbarButton5.Width = 32
        ToolbarButton6.BackColor = System.Drawing.Color.Transparent
        ToolbarButton6.BorderColor = System.Drawing.Color.Silver
        ToolbarButton6.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton6.BorderVisible = False
        ToolbarButton6.DefaultFont = Nothing
        ToolbarButton6.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton6.DefaultRowItemHeight = 0
        ToolbarButton6.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton6.DefaultTitleFont = Nothing
        ToolbarButton6.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton6.DropDown = False
        ToolbarButton6.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton6.DropDownMenuColoumnsCount = 1
        ToolbarButton6.Enabled = True
        ToolbarButton6.EnableSubItemsSelection = True
        ToolbarButton6.Font = Nothing
        ToolbarButton6.ForeColor = System.Drawing.Color.Empty
        ToolbarButton6.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton6.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton6.FullRowSelect = False
        ToolbarButton6.Image = Nothing
        ToolbarButton6.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton6.InternalBorderVisible = False
        ToolbarButton6.MaxDropDownItems = 0
        ToolbarButton6.MaxSubWidth = -1
        ToolbarButton6.MaxWidth = -1
        ToolbarButton6.MenuHorizontalAlignment = -1
        ToolbarButton6.MenuVerticalAlignment = -1
        ToolbarButton6.MinSubWidth = -1
        ToolbarButton6.MinWidth = -1
        ToolbarButton6.Name = ""
        ToolbarButton6.RowsHeightAutoSize = True
        ToolbarButton6.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton6.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton6.ShowColumnCheck = False
        ToolbarButton6.ShowColumnImage = False
        ToolbarButton6.State = False
        ToolbarButton6.SubColumnWidth = CType(resources.GetObject("ToolbarButton6.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton6.SubHeight = 100
        ToolbarButton6.SubItemsAutoSize = False
        ToolbarButton6.SubWidth = 100
        ToolbarButton6.Tag = ""
        ToolbarButton6.Text = ""
        ToolbarButton6.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton6.ToolTips = ""
        ToolbarButton6.Type = iControl.ToolbarButton.iType.Separator
        ToolbarButton6.Visible = True
        ToolbarButton6.Width = 6
        ToolbarButton7.BackColor = System.Drawing.Color.Transparent
        ToolbarButton7.BorderColor = System.Drawing.Color.Silver
        ToolbarButton7.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton7.BorderVisible = False
        ToolbarButton7.DefaultFont = Nothing
        ToolbarButton7.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton7.DefaultRowItemHeight = 0
        ToolbarButton7.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton7.DefaultTitleFont = Nothing
        ToolbarButton7.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton7.DropDown = False
        ToolbarButton7.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton7.DropDownMenuColoumnsCount = 1
        ToolbarButton7.Enabled = True
        ToolbarButton7.EnableSubItemsSelection = True
        ToolbarButton7.Font = Nothing
        ToolbarButton7.ForeColor = System.Drawing.Color.Empty
        ToolbarButton7.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton7.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton7.FullRowSelect = False
        ToolbarButton7.Image = Global.iFanta.My.Resources.Resources.folder_white
        ToolbarButton7.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton7.InternalBorderVisible = False
        ToolbarButton7.MaxDropDownItems = 0
        ToolbarButton7.MaxSubWidth = -1
        ToolbarButton7.MaxWidth = -1
        ToolbarButton7.MenuHorizontalAlignment = -1
        ToolbarButton7.MenuVerticalAlignment = -1
        ToolbarButton7.MinSubWidth = -1
        ToolbarButton7.MinWidth = -1
        ToolbarButton7.Name = ""
        ToolbarButton7.RowsHeightAutoSize = True
        ToolbarButton7.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton7.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton7.ShowColumnCheck = False
        ToolbarButton7.ShowColumnImage = False
        ToolbarButton7.State = False
        ToolbarButton7.SubColumnWidth = CType(resources.GetObject("ToolbarButton7.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton7.SubHeight = 100
        ToolbarButton7.SubItemsAutoSize = False
        ToolbarButton7.SubWidth = 100
        ToolbarButton7.Tag = ""
        ToolbarButton7.Text = ""
        ToolbarButton7.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton7.ToolTips = "Apri cartella importa e esporta"
        ToolbarButton7.Type = iControl.ToolbarButton.iType.Button
        ToolbarButton7.Visible = True
        ToolbarButton7.Width = 20
        ToolbarButton8.BackColor = System.Drawing.Color.Transparent
        ToolbarButton8.BorderColor = System.Drawing.Color.Silver
        ToolbarButton8.BorderColorDropDown = System.Drawing.Color.Silver
        ToolbarButton8.BorderVisible = False
        ToolbarButton8.DefaultFont = Nothing
        ToolbarButton8.DefaultForeColor = System.Drawing.Color.Empty
        ToolbarButton8.DefaultRowItemHeight = 0
        ToolbarButton8.DefaultSubItemsContentAlignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        ToolbarButton8.DefaultTitleFont = Nothing
        ToolbarButton8.DefaultTitleForeColor = System.Drawing.Color.Empty
        ToolbarButton8.DropDown = False
        ToolbarButton8.DropDownArrowAlignment = System.Windows.Forms.HorizontalAlignment.Right
        ToolbarButton8.DropDownMenuColoumnsCount = 1
        ToolbarButton8.Enabled = True
        ToolbarButton8.EnableSubItemsSelection = True
        ToolbarButton8.Font = Nothing
        ToolbarButton8.ForeColor = System.Drawing.Color.Empty
        ToolbarButton8.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton8.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton8.FullRowSelect = False
        ToolbarButton8.Image = Global.iFanta.My.Resources.Resources.settings_white
        ToolbarButton8.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton8.InternalBorderVisible = False
        ToolbarButton8.MaxDropDownItems = 0
        ToolbarButton8.MaxSubWidth = -1
        ToolbarButton8.MaxWidth = -1
        ToolbarButton8.MenuHorizontalAlignment = -1
        ToolbarButton8.MenuVerticalAlignment = -1
        ToolbarButton8.MinSubWidth = -1
        ToolbarButton8.MinWidth = -1
        ToolbarButton8.Name = ""
        ToolbarButton8.RowsHeightAutoSize = True
        ToolbarButton8.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton8.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton8.ShowColumnCheck = False
        ToolbarButton8.ShowColumnImage = False
        ToolbarButton8.State = False
        ToolbarButton8.SubColumnWidth = CType(resources.GetObject("ToolbarButton8.SubColumnWidth"), System.Collections.Generic.List(Of Integer))
        ToolbarButton8.SubHeight = 100
        ToolbarButton8.SubItemsAutoSize = False
        ToolbarButton8.SubWidth = 100
        ToolbarButton8.Tag = ""
        ToolbarButton8.Text = ""
        ToolbarButton8.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        ToolbarButton8.ToolTips = "Application settings"
        ToolbarButton8.Type = iControl.ToolbarButton.iType.Button
        ToolbarButton8.Visible = False
        ToolbarButton8.Width = 20
        Me.tlb1.Button.Add(ToolbarButton2)
        Me.tlb1.Button.Add(ToolbarButton3)
        Me.tlb1.Button.Add(ToolbarButton4)
        Me.tlb1.Button.Add(ToolbarButton5)
        Me.tlb1.Button.Add(ToolbarButton6)
        Me.tlb1.Button.Add(ToolbarButton7)
        Me.tlb1.Button.Add(ToolbarButton8)
        Me.tlb1.Dull = False
        Me.tlb1.EnabledSelection = False
        Me.tlb1.FlatStyle = False
        Me.tlb1.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.tlb1.ForeColor = System.Drawing.Color.White
        Me.tlb1.ForeColorDropDown = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlb1.ForeColorSelect = System.Drawing.Color.White
        Me.tlb1.Location = New System.Drawing.Point(235, 301)
        Me.tlb1.MaxDropDownItems = 10
        Me.tlb1.MenuHorizontalAlignment = 1
        Me.tlb1.MenuVerticalAlignment = 1
        Me.tlb1.MultiSelection = False
        Me.tlb1.Name = "tlb1"
        Me.tlb1.ReadOnly = False
        Me.tlb1.SelectionColor = System.Drawing.Color.White
        Me.tlb1.SelectionColorDropDown = System.Drawing.Color.White
        Me.tlb1.SeparatorColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.tlb1.Size = New System.Drawing.Size(131, 20)
        Me.tlb1.SubItemsMultiSelection = False
        Me.tlb1.SuspundeLayout = False
        Me.tlb1.TabIndex = 169
        Me.tlb1.TextAntiAlis = True
        Me.tlb1.TextGlowEffetct = True
        Me.tlb1.TextShadows = False
        Me.tlb1.ToolbarBorderColor = System.Drawing.Color.DimGray
        Me.tlb1.ToolbarBorderVisible = False
        Me.tlb1.ToolbarPadding = 1
        Me.tlb1.ToolTipDown = True
        '
        'txt3
        '
        Me.txt3.AutoComplete = False
        Me.txt3.AutoCompleteList = CType(resources.GetObject("txt3.AutoCompleteList"), System.Collections.Generic.List(Of String))
        Me.txt3.AutoCompleteMenuAlignment = System.Windows.Forms.HorizontalAlignment.Left
        Me.txt3.AutoCompleteMenuItems = 9
        Me.txt3.BackColor = System.Drawing.Color.White
        Me.txt3.BackgroundImage = CType(resources.GetObject("txt3.BackgroundImage"), System.Drawing.Image)
        Me.txt3.BorderColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.txt3.BorderSize = 1
        Me.txt3.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal
        Me.txt3.CharacterType = iControl.iText.eCharacterType.All
        Me.txt3.Dull = False
        Me.txt3.FlatStyle = False
        Me.txt3.Font = New System.Drawing.Font("Verdana", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.txt3.ForeColor = System.Drawing.Color.Black
        Me.txt3.InternalBorderColor = System.Drawing.Color.WhiteSmoke
        Me.txt3.InternalBorderSize = 0
        Me.txt3.Location = New System.Drawing.Point(152, 293)
        Me.txt3.MaxLength = 32767
        Me.txt3.MultiLine = False
        Me.txt3.Name = "txt3"
        Me.txt3.PasswordChar = "" & Global.Microsoft.VisualBasic.ChrW(0)
        Me.txt3.RegExCharacterType = ""
        Me.txt3.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.txt3.SelectionLenght = 0
        Me.txt3.SelectionStart = 0
        Me.txt3.Size = New System.Drawing.Size(95, 14)
        Me.txt3.SuspundeLayout = False
        Me.txt3.TabIndex = 166
        Me.txt3.Text = Nothing
        Me.txt3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left
        Me.txt3.TextLocked = False
        Me.txt3.ToolTip = "Filter application"
        Me.txt3.UseSystemPasswordChar = False
        '
        'vsrc1
        '
        Me.vsrc1.BackColor = System.Drawing.Color.Transparent
        Me.vsrc1.Background = Nothing
        Me.vsrc1.BackgroundImage = CType(resources.GetObject("vsrc1.BackgroundImage"), System.Drawing.Image)
        Me.vsrc1.BorderColor = System.Drawing.Color.LightGray
        Me.vsrc1.BorderSize = 0
        Me.vsrc1.Dull = True
        Me.vsrc1.FlatStyle = False
        Me.vsrc1.Location = New System.Drawing.Point(354, 48)
        Me.vsrc1.Max = 1
        Me.vsrc1.Min = 0
        Me.vsrc1.MinimumSize = New System.Drawing.Size(6, 30)
        Me.vsrc1.Name = "vsrc1"
        Me.vsrc1.ShowArrow = True
        Me.vsrc1.Size = New System.Drawing.Size(9, 247)
        Me.vsrc1.SmallChange = 1
        Me.vsrc1.SuspundeLayout = False
        Me.vsrc1.TabIndex = 165
        Me.vsrc1.Value = 0
        Me.vsrc1.Visible = False
        '
        'frmgenerale
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(525, 470)
        Me.Controls.Add(Me.vsrc2)
        Me.Controls.Add(Me.pictheme)
        Me.Controls.Add(Me.cmdrest)
        Me.Controls.Add(Me.cmdconne)
        Me.Controls.Add(Me.cmdupd)
        Me.Controls.Add(Me.tlbmenu)
        Me.Controls.Add(Me.tlb1)
        Me.Controls.Add(Me.picfind)
        Me.Controls.Add(Me.txt3)
        Me.Controls.Add(Me.vsrc1)
        Me.Controls.Add(Me.picapp)
        Me.Controls.Add(Me.picmenu)
        Me.Controls.Add(Me.cnt3)
        Me.Controls.Add(Me.cnt2)
        Me.Controls.Add(Me.cnt1)
        Me.Controls.Add(Me.pic7)
        Me.Controls.Add(Me.pic6)
        Me.Controls.Add(Me.pic5)
        Me.Controls.Add(Me.pic4)
        Me.Controls.Add(Me.pic3)
        Me.Controls.Add(Me.pic2)
        Me.Controls.Add(Me.pic1)
        Me.Controls.Add(Me.pic0)
        Me.Controls.Add(Me.picback)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmgenerale"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "iFanta"
        Me.mnu1.ResumeLayout(False)
        CType(Me.pictheme, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picfind, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picapp, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picmenu, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cnt3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cnt2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cnt1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic7, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic6, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic0, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picback, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents picback As System.Windows.Forms.PictureBox
    Friend WithEvents pic0 As System.Windows.Forms.PictureBox
    Friend WithEvents pic1 As System.Windows.Forms.PictureBox
    Friend WithEvents pic2 As System.Windows.Forms.PictureBox
    Friend WithEvents pic3 As System.Windows.Forms.PictureBox
    Friend WithEvents pic4 As System.Windows.Forms.PictureBox
    Friend WithEvents pic5 As System.Windows.Forms.PictureBox
    Friend WithEvents pic6 As System.Windows.Forms.PictureBox
    Friend WithEvents pic7 As System.Windows.Forms.PictureBox
    Friend WithEvents cnt3 As System.Windows.Forms.PictureBox
    Friend WithEvents cnt2 As System.Windows.Forms.PictureBox
    Friend WithEvents cnt1 As System.Windows.Forms.PictureBox
    Friend WithEvents picmenu As System.Windows.Forms.PictureBox
    Friend WithEvents picapp As System.Windows.Forms.PictureBox
    Friend WithEvents picfind As System.Windows.Forms.PictureBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents pictheme As System.Windows.Forms.PictureBox
    Friend WithEvents mnu1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents F1ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents vsrc1 As iControl.iVScroll
    Friend WithEvents txt3 As iControl.iText
    Friend WithEvents tlb1 As iControl.iToolBar
    Friend WithEvents tlbmenu As iControl.iToolBar
    Friend WithEvents cmdrest As iControl.iButton
    Friend WithEvents cmdconne As iControl.iButton
    Friend WithEvents cmdupd As iControl.iButton
    Friend WithEvents vsrc2 As iControl.iVScroll

End Class
