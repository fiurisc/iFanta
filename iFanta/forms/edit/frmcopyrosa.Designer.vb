<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmcopyrosa
    Inherits System.Windows.Forms.Form

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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmcopyrosa))
        Dim ToolbarButton1 As iControl.ToolbarButton = New iControl.ToolbarButton
        Dim ToolbarButton2 As iControl.ToolbarButton = New iControl.ToolbarButton
        Me.chk1 = New iControl.iCheckListBox
        Me.tlbaction = New iControl.iToolBar
        Me.SuspendLayout()
        '
        'chk1
        '
        Me.chk1.BackColor = System.Drawing.Color.FromArgb(CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.chk1.BorderColor = System.Drawing.Color.DarkGray
        Me.chk1.BorderVisible = True
        Me.chk1.Dull = True
        Me.chk1.InternalBorderColor = System.Drawing.Color.White
        Me.chk1.InternalBorderVisible = False
        Me.chk1.Location = New System.Drawing.Point(33, 52)
        Me.chk1.Name = "chk1"
        Me.chk1.RowHeight = 14
        Me.chk1.Size = New System.Drawing.Size(253, 137)
        Me.chk1.TabIndex = 0
        '
        'tlbaction
        '
        Me.tlbaction.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tlbaction.AutoSize = True
        Me.tlbaction.Background = Nothing
        Me.tlbaction.BackgroundImage = CType(resources.GetObject("tlbaction.BackgroundImage"), System.Drawing.Image)
        Me.tlbaction.BorderColor = System.Drawing.Color.DimGray
        Me.tlbaction.BorderColorDropDown = System.Drawing.Color.DimGray
        ToolbarButton1.BackColor = System.Drawing.Color.Empty
        ToolbarButton1.BorderColor = System.Drawing.Color.DimGray
        ToolbarButton1.BorderColorDropDown = System.Drawing.Color.DimGray
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
        ToolbarButton1.Font = Nothing
        ToolbarButton1.ForeColor = System.Drawing.Color.Empty
        ToolbarButton1.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton1.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton1.FullRowSelect = False
        ToolbarButton1.Image = Global.iFanta.My.Resources.Resources.reload16
        ToolbarButton1.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton1.InternalBorderVisible = False
        ToolbarButton1.MaxDropDownItems = 0
        ToolbarButton1.MenuHorizontalAlignment = -1
        ToolbarButton1.MenuVerticalAlignment = -1
        ToolbarButton1.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton1.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton1.ShowColumnCheck = False
        ToolbarButton1.ShowColumnImage = False
        ToolbarButton1.State = False
        ToolbarButton1.SubItemsAutoSize = True
        ToolbarButton1.SubWidth = 100
        ToolbarButton1.Tag = Nothing
        ToolbarButton1.Text = "Esegui ora"
        ToolbarButton1.ToolTips = ""
        ToolbarButton1.Type = iControl.ToolbarButton.iType.Button
        ToolbarButton1.Visible = True
        ToolbarButton1.Width = 85
        ToolbarButton2.BackColor = System.Drawing.Color.Empty
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
        ToolbarButton2.Font = Nothing
        ToolbarButton2.ForeColor = System.Drawing.Color.Empty
        ToolbarButton2.ForeColorDropDown = System.Drawing.Color.Empty
        ToolbarButton2.ForeColorSelect = System.Drawing.Color.Empty
        ToolbarButton2.FullRowSelect = False
        ToolbarButton2.Image = Global.iFanta.My.Resources.Resources.closer16
        ToolbarButton2.InternalBorderColor = System.Drawing.Color.Empty
        ToolbarButton2.InternalBorderVisible = False
        ToolbarButton2.MaxDropDownItems = 0
        ToolbarButton2.MenuHorizontalAlignment = -1
        ToolbarButton2.MenuVerticalAlignment = -1
        ToolbarButton2.SelectionColor = System.Drawing.Color.Empty
        ToolbarButton2.SelectionColorDropDown = System.Drawing.Color.Empty
        ToolbarButton2.ShowColumnCheck = False
        ToolbarButton2.ShowColumnImage = False
        ToolbarButton2.State = False
        ToolbarButton2.SubItemsAutoSize = False
        ToolbarButton2.SubWidth = 100
        ToolbarButton2.Tag = Nothing
        ToolbarButton2.Text = "Chiudi"
        ToolbarButton2.ToolTips = ""
        ToolbarButton2.Type = iControl.ToolbarButton.iType.Button
        ToolbarButton2.Visible = True
        ToolbarButton2.Width = 64
        Me.tlbaction.Button.Add(ToolbarButton1)
        Me.tlbaction.Button.Add(ToolbarButton2)
        Me.tlbaction.Dull = False
        Me.tlbaction.EnabledSelection = False
        Me.tlbaction.FlatStyle = False
        Me.tlbaction.Font = New System.Drawing.Font("Tahoma", 11!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, CType(0, Byte))
        Me.tlbaction.ForeColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlbaction.ForeColorDropDown = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.tlbaction.ForeColorSelect = System.Drawing.Color.Red
        Me.tlbaction.Location = New System.Drawing.Point(159, 206)
        Me.tlbaction.MaxDropDownItems = 15
        Me.tlbaction.MenuHorizontalAlignment = 1
        Me.tlbaction.MenuVerticalAlignment = 0
        Me.tlbaction.MultiSelection = False
        Me.tlbaction.Name = "tlbaction"
        Me.tlbaction.SelectionColor = System.Drawing.Color.White
        Me.tlbaction.SelectionColorDropDown = System.Drawing.Color.White
        Me.tlbaction.SeparatorColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.tlbaction.Size = New System.Drawing.Size(150, 19)
        Me.tlbaction.SubItemsMultiSelection = False
        Me.tlbaction.SuspundeLayout = False
        Me.tlbaction.TabIndex = 285
        Me.tlbaction.TextAntiAlis = False
        Me.tlbaction.TextGlowEffetct = False
        Me.tlbaction.TextShadows = False
        Me.tlbaction.ToolbarPadding = 0
        '
        'frmcopyrosa
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(321, 237)
        Me.Controls.Add(Me.tlbaction)
        Me.Controls.Add(Me.chk1)
        Me.Name = "frmcopyrosa"
        Me.Text = "frmcopyrosa"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chk1 As iControl.iCheckListBox
    Friend WithEvents tlbaction As iControl.iToolBar
End Class
