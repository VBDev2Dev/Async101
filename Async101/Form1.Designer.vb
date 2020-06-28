<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.btnGui = New System.Windows.Forms.Button()
        Me.tmrTime = New System.Windows.Forms.Timer(Me.components)
        Me.lblTime = New System.Windows.Forms.Label()
        Me.btnAsyncDelay = New System.Windows.Forms.Button()
        Me.tstAsync = New Async101.Tester()
        Me.lbLog = New System.Windows.Forms.ListBox()
        Me.MessageEventArgsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.tmrLog = New System.Windows.Forms.Timer(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnFiles = New System.Windows.Forms.Button()
        CType(Me.MessageEventArgsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnGui
        '
        Me.btnGui.Location = New System.Drawing.Point(20, 3)
        Me.btnGui.Name = "btnGui"
        Me.btnGui.Size = New System.Drawing.Size(75, 23)
        Me.btnGui.TabIndex = 0
        Me.btnGui.Text = "In Gui Thread"
        Me.btnGui.UseVisualStyleBackColor = True
        '
        'tmrTime
        '
        Me.tmrTime.Enabled = True
        Me.tmrTime.Interval = 500
        '
        'lblTime
        '
        Me.lblTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTime.Location = New System.Drawing.Point(278, 9)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Size = New System.Drawing.Size(268, 21)
        Me.lblTime.TabIndex = 1
        Me.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnAsyncDelay
        '
        Me.btnAsyncDelay.Location = New System.Drawing.Point(20, 50)
        Me.btnAsyncDelay.Name = "btnAsyncDelay"
        Me.btnAsyncDelay.Size = New System.Drawing.Size(75, 23)
        Me.btnAsyncDelay.TabIndex = 2
        Me.btnAsyncDelay.Text = "In Async"
        Me.btnAsyncDelay.UseVisualStyleBackColor = True
        '
        'tstAsync
        '
        '
        'lbLog
        '
        Me.lbLog.DataSource = Me.MessageEventArgsBindingSource
        Me.lbLog.DisplayMember = "Display"
        Me.lbLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbLog.FormattingEnabled = True
        Me.lbLog.Location = New System.Drawing.Point(0, 100)
        Me.lbLog.Name = "lbLog"
        Me.lbLog.Size = New System.Drawing.Size(558, 259)
        Me.lbLog.TabIndex = 3
        Me.lbLog.ValueMember = "ID"
        '
        'MessageEventArgsBindingSource
        '
        Me.MessageEventArgsBindingSource.DataSource = GetType(Async101.LogMessageEventArgs)
        '
        'tmrLog
        '
        Me.tmrLog.Enabled = True
        Me.tmrLog.Interval = 400
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnFiles)
        Me.Panel1.Controls.Add(Me.lblTime)
        Me.Panel1.Controls.Add(Me.btnGui)
        Me.Panel1.Controls.Add(Me.btnAsyncDelay)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(558, 100)
        Me.Panel1.TabIndex = 4
        '
        'btnFiles
        '
        Me.btnFiles.Location = New System.Drawing.Point(133, 50)
        Me.btnFiles.Name = "btnFiles"
        Me.btnFiles.Size = New System.Drawing.Size(133, 23)
        Me.btnFiles.TabIndex = 3
        Me.btnFiles.Text = "Generate Files"
        Me.btnFiles.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(558, 359)
        Me.Controls.Add(Me.lbLog)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.MessageEventArgsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnGui As Button
    Friend WithEvents tmrTime As Timer
    Friend WithEvents lblTime As Label
    Friend WithEvents btnAsyncDelay As Button
    Friend WithEvents tstAsync As Tester
    Friend WithEvents lbLog As ListBox
    Friend WithEvents MessageEventArgsBindingSource As BindingSource
    Friend WithEvents tmrLog As Timer
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnFiles As Button
End Class
