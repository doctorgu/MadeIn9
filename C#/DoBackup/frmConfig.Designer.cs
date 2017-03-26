namespace DoBackup
{
	partial class frmConfig
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tlsbtnOpen = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsbtnSave = new System.Windows.Forms.ToolStripButton();
			this.tlsbtnSaveAs = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsbtnMoveUp = new System.Windows.Forms.ToolStripButton();
			this.tlsbtnMoveDown = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsbtnDelete = new System.Windows.Forms.ToolStripButton();
			this.tlsbtnCopy = new System.Windows.Forms.ToolStripButton();
			this.tlsbtnCut = new System.Windows.Forms.ToolStripButton();
			this.tlsbtnPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsbtnHelp = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tlsbtnClose = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.grvList = new System.Windows.Forms.DataGridView();
			this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.rootFolderSrcDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.rootFolderDestDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ftpHostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ftpFolderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ftpPortDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.logFolderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dsConfig = new System.Data.DataSet();
			this.dataTable1 = new System.Data.DataTable();
			this.dataColumn1 = new System.Data.DataColumn();
			this.dataColumn2 = new System.Data.DataColumn();
			this.dataColumn3 = new System.Data.DataColumn();
			this.dataColumn4 = new System.Data.DataColumn();
			this.dataColumn5 = new System.Data.DataColumn();
			this.dataColumn6 = new System.Data.DataColumn();
			this.dataColumn7 = new System.Data.DataColumn();
			this.dataColumn8 = new System.Data.DataColumn();
			this.dataColumn9 = new System.Data.DataColumn();
			this.dataColumn10 = new System.Data.DataColumn();
			this.dataColumn11 = new System.Data.DataColumn();
			this.dataColumn12 = new System.Data.DataColumn();
			this.dataColumn13 = new System.Data.DataColumn();
			this.dataColumn14 = new System.Data.DataColumn();
			this.dataColumn15 = new System.Data.DataColumn();
			this.dataColumn16 = new System.Data.DataColumn();
			this.dataColumn17 = new System.Data.DataColumn();
			this.dataColumn18 = new System.Data.DataColumn();
			this.dataColumn19 = new System.Data.DataColumn();
			this.dataColumn20 = new System.Data.DataColumn();
			this.dataColumn21 = new System.Data.DataColumn();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtFullPathReferencingJs = new System.Windows.Forms.TextBox();
			this.btnFullPathReferencingJs = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.btnJsFullPathRefered = new System.Windows.Forms.Button();
			this.txtJsFullPathRefered = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.chkEmptyFolderHasNoFile = new System.Windows.Forms.CheckBox();
			this.chkMinifyJs = new System.Windows.Forms.CheckBox();
			this.cboSyncType = new System.Windows.Forms.ComboBox();
			this.btnDeleteEmptyRootFolderDest = new System.Windows.Forms.Button();
			this.btnRootFolderDest = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radAllowedOnlyFolder = new System.Windows.Forms.RadioButton();
			this.btnAllowedOnlyFolder = new System.Windows.Forms.Button();
			this.txtAllowedOnlyFolder = new System.Windows.Forms.TextBox();
			this.radDisallowedFolder = new System.Windows.Forms.RadioButton();
			this.btnDisallowedFolder = new System.Windows.Forms.Button();
			this.txtDisallowedFolder = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radAllowedOnlyExt = new System.Windows.Forms.RadioButton();
			this.radDisallowedExt = new System.Windows.Forms.RadioButton();
			this.txtAllowedOnlyExt = new System.Windows.Forms.TextBox();
			this.txtDisallowedExt = new System.Windows.Forms.TextBox();
			this.chkFtpUsePassive = new System.Windows.Forms.CheckBox();
			this.btnSyncNow = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.nudFtpPort = new System.Windows.Forms.NumericUpDown();
			this.txtFtpFolder = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtFtpHost = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtFtpPassword = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtFtpId = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.nudIntervalMinutes = new System.Windows.Forms.NumericUpDown();
			this.btnNow = new System.Windows.Forms.Button();
			this.dtpDateTimeAfter = new System.Windows.Forms.DateTimePicker();
			this.label6 = new System.Windows.Forms.Label();
			this.btnLogFolder = new System.Windows.Forms.Button();
			this.btnOpenRootFolderDest = new System.Windows.Forms.Button();
			this.btnOpenRootFolderSrc = new System.Windows.Forms.Button();
			this.txtLogFolder = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtRootFolderDest = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtRootFolderSrc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.sttlblMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.sttlblFullPath = new System.Windows.Forms.ToolStripStatusLabel();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.toolStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grvList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsConfig)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFtpPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIntervalMinutes)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsbtnOpen,
            this.toolStripSeparator1,
            this.tlsbtnSave,
            this.tlsbtnSaveAs,
            this.toolStripSeparator4,
            this.tlsbtnMoveUp,
            this.tlsbtnMoveDown,
            this.toolStripSeparator5,
            this.tlsbtnDelete,
            this.tlsbtnCopy,
            this.tlsbtnCut,
            this.tlsbtnPaste,
            this.toolStripSeparator2,
            this.tlsbtnHelp,
            this.toolStripSeparator3,
            this.tlsbtnClose});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(955, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tlsbtnOpen
			// 
			this.tlsbtnOpen.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnOpen.Image")));
			this.tlsbtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnOpen.Name = "tlsbtnOpen";
			this.tlsbtnOpen.Size = new System.Drawing.Size(53, 22);
			this.tlsbtnOpen.Text = "&Open";
			this.tlsbtnOpen.Click += new System.EventHandler(this.tlsbtnOpen_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tlsbtnSave
			// 
			this.tlsbtnSave.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnSave.Image")));
			this.tlsbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnSave.Name = "tlsbtnSave";
			this.tlsbtnSave.Size = new System.Drawing.Size(51, 22);
			this.tlsbtnSave.Text = "&Save";
			this.tlsbtnSave.Click += new System.EventHandler(this.tlsbtnSave_Click);
			// 
			// tlsbtnSaveAs
			// 
			this.tlsbtnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnSaveAs.Image")));
			this.tlsbtnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnSaveAs.Name = "tlsbtnSaveAs";
			this.tlsbtnSaveAs.Size = new System.Drawing.Size(66, 22);
			this.tlsbtnSaveAs.Text = "Save &As";
			this.tlsbtnSaveAs.Click += new System.EventHandler(this.tlsbtnSaveAs_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// tlsbtnMoveUp
			// 
			this.tlsbtnMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnMoveUp.Image")));
			this.tlsbtnMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnMoveUp.Name = "tlsbtnMoveUp";
			this.tlsbtnMoveUp.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnMoveUp.Text = "Move Up";
			this.tlsbtnMoveUp.ToolTipText = "Move Up";
			this.tlsbtnMoveUp.Click += new System.EventHandler(this.tlsbtnMoveUp_Click);
			// 
			// tlsbtnMoveDown
			// 
			this.tlsbtnMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnMoveDown.Image")));
			this.tlsbtnMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnMoveDown.Name = "tlsbtnMoveDown";
			this.tlsbtnMoveDown.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnMoveDown.Text = "Move Down";
			this.tlsbtnMoveDown.ToolTipText = "Move Down";
			this.tlsbtnMoveDown.Click += new System.EventHandler(this.tlsbtnMoveDown_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// tlsbtnDelete
			// 
			this.tlsbtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnDelete.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnDelete.Image")));
			this.tlsbtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnDelete.Name = "tlsbtnDelete";
			this.tlsbtnDelete.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnDelete.Text = "Delete";
			this.tlsbtnDelete.Click += new System.EventHandler(this.tlsbtnDelete_Click);
			// 
			// tlsbtnCopy
			// 
			this.tlsbtnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnCopy.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnCopy.Image")));
			this.tlsbtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnCopy.Name = "tlsbtnCopy";
			this.tlsbtnCopy.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnCopy.Text = "Copy";
			this.tlsbtnCopy.ToolTipText = "Copy";
			this.tlsbtnCopy.Click += new System.EventHandler(this.tlsbtnCopy_Click);
			// 
			// tlsbtnCut
			// 
			this.tlsbtnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnCut.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnCut.Image")));
			this.tlsbtnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnCut.Name = "tlsbtnCut";
			this.tlsbtnCut.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnCut.Text = "Cut";
			this.tlsbtnCut.ToolTipText = "Cut";
			this.tlsbtnCut.Click += new System.EventHandler(this.tlsbtnCut_Click);
			// 
			// tlsbtnPaste
			// 
			this.tlsbtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tlsbtnPaste.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnPaste.Image")));
			this.tlsbtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnPaste.Name = "tlsbtnPaste";
			this.tlsbtnPaste.Size = new System.Drawing.Size(23, 22);
			this.tlsbtnPaste.Text = "Paste";
			this.tlsbtnPaste.ToolTipText = "Paste";
			this.tlsbtnPaste.Click += new System.EventHandler(this.tlsbtnPaste_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// tlsbtnHelp
			// 
			this.tlsbtnHelp.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnHelp.Image")));
			this.tlsbtnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnHelp.Name = "tlsbtnHelp";
			this.tlsbtnHelp.Size = new System.Drawing.Size(48, 22);
			this.tlsbtnHelp.Text = "Help";
			this.tlsbtnHelp.Click += new System.EventHandler(this.tlsbtnHelp_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// tlsbtnClose
			// 
			this.tlsbtnClose.Image = ((System.Drawing.Image)(resources.GetObject("tlsbtnClose.Image")));
			this.tlsbtnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tlsbtnClose.Name = "tlsbtnClose";
			this.tlsbtnClose.Size = new System.Drawing.Size(53, 22);
			this.tlsbtnClose.Text = "&Close";
			this.tlsbtnClose.Click += new System.EventHandler(this.tlsbtnClose_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.splitContainer1);
			this.panel1.Controls.Add(this.statusStrip1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(955, 723);
			this.panel1.TabIndex = 2;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.grvList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.chkEmptyFolderHasNoFile);
			this.splitContainer1.Panel2.Controls.Add(this.chkMinifyJs);
			this.splitContainer1.Panel2.Controls.Add(this.cboSyncType);
			this.splitContainer1.Panel2.Controls.Add(this.btnDeleteEmptyRootFolderDest);
			this.splitContainer1.Panel2.Controls.Add(this.btnRootFolderDest);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.chkFtpUsePassive);
			this.splitContainer1.Panel2.Controls.Add(this.btnSyncNow);
			this.splitContainer1.Panel2.Controls.Add(this.label14);
			this.splitContainer1.Panel2.Controls.Add(this.nudFtpPort);
			this.splitContainer1.Panel2.Controls.Add(this.txtFtpFolder);
			this.splitContainer1.Panel2.Controls.Add(this.label13);
			this.splitContainer1.Panel2.Controls.Add(this.txtFtpHost);
			this.splitContainer1.Panel2.Controls.Add(this.label10);
			this.splitContainer1.Panel2.Controls.Add(this.txtName);
			this.splitContainer1.Panel2.Controls.Add(this.label9);
			this.splitContainer1.Panel2.Controls.Add(this.txtFtpPassword);
			this.splitContainer1.Panel2.Controls.Add(this.label8);
			this.splitContainer1.Panel2.Controls.Add(this.txtFtpId);
			this.splitContainer1.Panel2.Controls.Add(this.label7);
			this.splitContainer1.Panel2.Controls.Add(this.label5);
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.nudIntervalMinutes);
			this.splitContainer1.Panel2.Controls.Add(this.btnNow);
			this.splitContainer1.Panel2.Controls.Add(this.dtpDateTimeAfter);
			this.splitContainer1.Panel2.Controls.Add(this.label6);
			this.splitContainer1.Panel2.Controls.Add(this.btnLogFolder);
			this.splitContainer1.Panel2.Controls.Add(this.btnOpenRootFolderDest);
			this.splitContainer1.Panel2.Controls.Add(this.btnOpenRootFolderSrc);
			this.splitContainer1.Panel2.Controls.Add(this.txtLogFolder);
			this.splitContainer1.Panel2.Controls.Add(this.label4);
			this.splitContainer1.Panel2.Controls.Add(this.txtRootFolderDest);
			this.splitContainer1.Panel2.Controls.Add(this.label3);
			this.splitContainer1.Panel2.Controls.Add(this.txtRootFolderSrc);
			this.splitContainer1.Panel2.Controls.Add(this.label2);
			this.splitContainer1.Size = new System.Drawing.Size(955, 701);
			this.splitContainer1.SplitterDistance = 170;
			this.splitContainer1.TabIndex = 1;
			// 
			// grvList
			// 
			this.grvList.AutoGenerateColumns = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.grvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.rootFolderSrcDataGridViewTextBoxColumn,
            this.rootFolderDestDataGridViewTextBoxColumn,
            this.ftpHostDataGridViewTextBoxColumn,
            this.ftpFolderDataGridViewTextBoxColumn,
            this.ftpPortDataGridViewTextBoxColumn,
            this.logFolderDataGridViewTextBoxColumn});
			this.grvList.DataMember = "Config";
			this.grvList.DataSource = this.dsConfig;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grvList.DefaultCellStyle = dataGridViewCellStyle2;
			this.grvList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grvList.Location = new System.Drawing.Point(0, 0);
			this.grvList.Name = "grvList";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grvList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.grvList.Size = new System.Drawing.Size(955, 170);
			this.grvList.TabIndex = 0;
			// 
			// nameDataGridViewTextBoxColumn
			// 
			this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
			this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
			this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
			this.nameDataGridViewTextBoxColumn.Width = 60;
			// 
			// rootFolderSrcDataGridViewTextBoxColumn
			// 
			this.rootFolderSrcDataGridViewTextBoxColumn.DataPropertyName = "RootFolderSrc";
			this.rootFolderSrcDataGridViewTextBoxColumn.HeaderText = "RootFolderSrc";
			this.rootFolderSrcDataGridViewTextBoxColumn.Name = "rootFolderSrcDataGridViewTextBoxColumn";
			this.rootFolderSrcDataGridViewTextBoxColumn.Width = 150;
			// 
			// rootFolderDestDataGridViewTextBoxColumn
			// 
			this.rootFolderDestDataGridViewTextBoxColumn.DataPropertyName = "RootFolderDest";
			this.rootFolderDestDataGridViewTextBoxColumn.HeaderText = "RootFolderDest";
			this.rootFolderDestDataGridViewTextBoxColumn.Name = "rootFolderDestDataGridViewTextBoxColumn";
			this.rootFolderDestDataGridViewTextBoxColumn.Width = 150;
			// 
			// ftpHostDataGridViewTextBoxColumn
			// 
			this.ftpHostDataGridViewTextBoxColumn.DataPropertyName = "FtpHost";
			this.ftpHostDataGridViewTextBoxColumn.HeaderText = "FtpHost";
			this.ftpHostDataGridViewTextBoxColumn.Name = "ftpHostDataGridViewTextBoxColumn";
			this.ftpHostDataGridViewTextBoxColumn.Width = 150;
			// 
			// ftpFolderDataGridViewTextBoxColumn
			// 
			this.ftpFolderDataGridViewTextBoxColumn.DataPropertyName = "FtpFolder";
			this.ftpFolderDataGridViewTextBoxColumn.HeaderText = "FtpFolder";
			this.ftpFolderDataGridViewTextBoxColumn.Name = "ftpFolderDataGridViewTextBoxColumn";
			// 
			// ftpPortDataGridViewTextBoxColumn
			// 
			this.ftpPortDataGridViewTextBoxColumn.DataPropertyName = "FtpPort";
			this.ftpPortDataGridViewTextBoxColumn.HeaderText = "FtpPort";
			this.ftpPortDataGridViewTextBoxColumn.Name = "ftpPortDataGridViewTextBoxColumn";
			// 
			// logFolderDataGridViewTextBoxColumn
			// 
			this.logFolderDataGridViewTextBoxColumn.DataPropertyName = "LogFolder";
			this.logFolderDataGridViewTextBoxColumn.HeaderText = "LogFolder";
			this.logFolderDataGridViewTextBoxColumn.Name = "logFolderDataGridViewTextBoxColumn";
			// 
			// dsConfig
			// 
			this.dsConfig.DataSetName = "dsConfig";
			this.dsConfig.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
			// 
			// dataTable1
			// 
			this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8,
            this.dataColumn9,
            this.dataColumn10,
            this.dataColumn11,
            this.dataColumn12,
            this.dataColumn13,
            this.dataColumn14,
            this.dataColumn15,
            this.dataColumn16,
            this.dataColumn17,
            this.dataColumn18,
            this.dataColumn19,
            this.dataColumn20,
            this.dataColumn21});
			this.dataTable1.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Name"}, true)});
			this.dataTable1.PrimaryKey = new System.Data.DataColumn[] {
        this.dataColumn1};
			this.dataTable1.TableName = "Config";
			// 
			// dataColumn1
			// 
			this.dataColumn1.AllowDBNull = false;
			this.dataColumn1.Caption = "Name";
			this.dataColumn1.ColumnName = "Name";
			this.dataColumn1.DefaultValue = "";
			// 
			// dataColumn2
			// 
			this.dataColumn2.AllowDBNull = false;
			this.dataColumn2.Caption = "Source Folder";
			this.dataColumn2.ColumnName = "RootFolderSrc";
			this.dataColumn2.DefaultValue = "";
			// 
			// dataColumn3
			// 
			this.dataColumn3.AllowDBNull = false;
			this.dataColumn3.Caption = "Destination Folder";
			this.dataColumn3.ColumnName = "RootFolderDest";
			this.dataColumn3.DefaultValue = "";
			// 
			// dataColumn4
			// 
			this.dataColumn4.AllowDBNull = false;
			this.dataColumn4.Caption = "FTP Host";
			this.dataColumn4.ColumnName = "FtpHost";
			this.dataColumn4.DefaultValue = "";
			// 
			// dataColumn5
			// 
			this.dataColumn5.AllowDBNull = false;
			this.dataColumn5.Caption = "FTP Folder";
			this.dataColumn5.ColumnName = "FtpFolder";
			this.dataColumn5.DefaultValue = "";
			// 
			// dataColumn6
			// 
			this.dataColumn6.AllowDBNull = false;
			this.dataColumn6.Caption = "FTP Port";
			this.dataColumn6.ColumnName = "FtpPort";
			this.dataColumn6.DataType = typeof(int);
			this.dataColumn6.DefaultValue = 21;
			// 
			// dataColumn7
			// 
			this.dataColumn7.AllowDBNull = false;
			this.dataColumn7.Caption = "FTP Passive Mode";
			this.dataColumn7.ColumnName = "FtpUsePassive";
			this.dataColumn7.DataType = typeof(bool);
			this.dataColumn7.DefaultValue = false;
			// 
			// dataColumn8
			// 
			this.dataColumn8.AllowDBNull = false;
			this.dataColumn8.Caption = "FTP ID";
			this.dataColumn8.ColumnName = "FtpId";
			this.dataColumn8.DefaultValue = "";
			// 
			// dataColumn9
			// 
			this.dataColumn9.AllowDBNull = false;
			this.dataColumn9.Caption = "FTP Password";
			this.dataColumn9.ColumnName = "FtpPassword";
			this.dataColumn9.DefaultValue = "";
			// 
			// dataColumn10
			// 
			this.dataColumn10.AllowDBNull = false;
			this.dataColumn10.Caption = "Copy Interval";
			this.dataColumn10.ColumnName = "IntervalMinutes";
			this.dataColumn10.DataType = typeof(int);
			this.dataColumn10.DefaultValue = 60;
			// 
			// dataColumn11
			// 
			this.dataColumn11.Caption = "Allow only after this time";
			this.dataColumn11.ColumnName = "DateTimeAfter";
			this.dataColumn11.DataType = typeof(System.DateTime);
			this.dataColumn11.DateTimeMode = System.Data.DataSetDateTime.Local;
			// 
			// dataColumn12
			// 
			this.dataColumn12.AllowDBNull = false;
			this.dataColumn12.Caption = "Extension disallowed";
			this.dataColumn12.ColumnName = "DisallowedExt";
			this.dataColumn12.DefaultValue = "";
			// 
			// dataColumn13
			// 
			this.dataColumn13.AllowDBNull = false;
			this.dataColumn13.Caption = "Folder To Copy";
			this.dataColumn13.ColumnName = "DisallowedFolder";
			this.dataColumn13.DefaultValue = "";
			// 
			// dataColumn14
			// 
			this.dataColumn14.AllowDBNull = false;
			this.dataColumn14.Caption = "Log Folder";
			this.dataColumn14.ColumnName = "LogFolder";
			this.dataColumn14.DefaultValue = "";
			// 
			// dataColumn15
			// 
			this.dataColumn15.AllowDBNull = false;
			this.dataColumn15.Caption = "Extension to allow";
			this.dataColumn15.ColumnName = "AllowedOnlyExt";
			this.dataColumn15.DefaultValue = "";
			// 
			// dataColumn16
			// 
			this.dataColumn16.AllowDBNull = false;
			this.dataColumn16.Caption = "Folder To Copy";
			this.dataColumn16.ColumnName = "AllowedOnlyFolder";
			this.dataColumn16.DefaultValue = "";
			// 
			// dataColumn17
			// 
			this.dataColumn17.AllowDBNull = false;
			this.dataColumn17.Caption = "Synchronization Type";
			this.dataColumn17.ColumnName = "SyncType";
			this.dataColumn17.DefaultValue = "";
			// 
			// dataColumn18
			// 
			this.dataColumn18.AllowDBNull = false;
			this.dataColumn18.Caption = "Minify .js file";
			this.dataColumn18.ColumnName = "MinifyJs";
			this.dataColumn18.DataType = typeof(bool);
			this.dataColumn18.DefaultValue = false;
			// 
			// dataColumn19
			// 
			this.dataColumn19.AllowDBNull = false;
			this.dataColumn19.Caption = "Js Full Path Refered";
			this.dataColumn19.ColumnName = "JsFullPathRefered";
			this.dataColumn19.DefaultValue = "";
			// 
			// dataColumn20
			// 
			this.dataColumn20.AllowDBNull = false;
			this.dataColumn20.Caption = "Full Path Referencing Js";
			this.dataColumn20.ColumnName = "FullPathReferencingJs";
			this.dataColumn20.DefaultValue = "";
			// 
			// dataColumn21
			// 
			this.dataColumn21.AllowDBNull = false;
			this.dataColumn21.Caption = "Delete Empty Folder";
			this.dataColumn21.ColumnName = "EmptyFolderHasNoFile";
			this.dataColumn21.DataType = typeof(bool);
			this.dataColumn21.DefaultValue = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtFullPathReferencingJs);
			this.groupBox3.Controls.Add(this.btnFullPathReferencingJs);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.btnJsFullPathRefered);
			this.groupBox3.Controls.Add(this.txtJsFullPathRefered);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Location = new System.Drawing.Point(160, 226);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(673, 72);
			this.groupBox3.TabIndex = 38;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Append Param to Referencing Js";
			// 
			// txtFullPathReferencingJs
			// 
			this.txtFullPathReferencingJs.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.FullPathReferencingJs", true));
			this.txtFullPathReferencingJs.Location = new System.Drawing.Point(152, 18);
			this.txtFullPathReferencingJs.Name = "txtFullPathReferencingJs";
			this.txtFullPathReferencingJs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtFullPathReferencingJs.Size = new System.Drawing.Size(466, 20);
			this.txtFullPathReferencingJs.TabIndex = 36;
			// 
			// btnFullPathReferencingJs
			// 
			this.btnFullPathReferencingJs.Location = new System.Drawing.Point(621, 16);
			this.btnFullPathReferencingJs.Name = "btnFullPathReferencingJs";
			this.btnFullPathReferencingJs.Size = new System.Drawing.Size(42, 25);
			this.btnFullPathReferencingJs.TabIndex = 37;
			this.btnFullPathReferencingJs.Text = "...";
			this.btnFullPathReferencingJs.UseVisualStyleBackColor = true;
			this.btnFullPathReferencingJs.Click += new System.EventHandler(this.btnFullPathReferencingJs_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(33, 47);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(105, 13);
			this.label11.TabIndex = 35;
			this.label11.Text = "Js Full Path Refered:";
			// 
			// btnJsFullPathRefered
			// 
			this.btnJsFullPathRefered.Location = new System.Drawing.Point(621, 40);
			this.btnJsFullPathRefered.Name = "btnJsFullPathRefered";
			this.btnJsFullPathRefered.Size = new System.Drawing.Size(42, 25);
			this.btnJsFullPathRefered.TabIndex = 37;
			this.btnJsFullPathRefered.Text = "...";
			this.btnJsFullPathRefered.UseVisualStyleBackColor = true;
			this.btnJsFullPathRefered.Click += new System.EventHandler(this.btnJsFullPathRefered_Click);
			// 
			// txtJsFullPathRefered
			// 
			this.txtJsFullPathRefered.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.JsFullPathRefered", true));
			this.txtJsFullPathRefered.Location = new System.Drawing.Point(152, 42);
			this.txtJsFullPathRefered.Name = "txtJsFullPathRefered";
			this.txtJsFullPathRefered.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtJsFullPathRefered.Size = new System.Drawing.Size(466, 20);
			this.txtJsFullPathRefered.TabIndex = 36;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(13, 23);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(125, 13);
			this.label12.TabIndex = 35;
			this.label12.Text = "Full Path Referencing Js:";
			// 
			// chkEmptyFolderHasNoFile
			// 
			this.chkEmptyFolderHasNoFile.AutoSize = true;
			this.chkEmptyFolderHasNoFile.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dsConfig, "Config.EmptyFolderHasNoFile", true));
			this.chkEmptyFolderHasNoFile.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.dsConfig, "Config.EmptyFolderHasNoFile", true));
			this.chkEmptyFolderHasNoFile.Location = new System.Drawing.Point(549, 172);
			this.chkEmptyFolderHasNoFile.Name = "chkEmptyFolderHasNoFile";
			this.chkEmptyFolderHasNoFile.Size = new System.Drawing.Size(189, 17);
			this.chkEmptyFolderHasNoFile.TabIndex = 34;
			this.chkEmptyFolderHasNoFile.Text = "Empty destination folder has no file";
			this.chkEmptyFolderHasNoFile.UseVisualStyleBackColor = true;
			// 
			// chkMinifyJs
			// 
			this.chkMinifyJs.AutoSize = true;
			this.chkMinifyJs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dsConfig, "Config.MinifyJs", true));
			this.chkMinifyJs.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.dsConfig, "Config.MinifyJs", true));
			this.chkMinifyJs.Location = new System.Drawing.Point(452, 173);
			this.chkMinifyJs.Name = "chkMinifyJs";
			this.chkMinifyJs.Size = new System.Drawing.Size(82, 17);
			this.chkMinifyJs.TabIndex = 34;
			this.chkMinifyJs.Text = "Minify .js file";
			this.chkMinifyJs.UseVisualStyleBackColor = true;
			// 
			// cboSyncType
			// 
			this.cboSyncType.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.dsConfig, "Config.SyncType", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.cboSyncType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSyncType.FormattingEnabled = true;
			this.cboSyncType.Location = new System.Drawing.Point(160, 170);
			this.cboSyncType.Name = "cboSyncType";
			this.cboSyncType.Size = new System.Drawing.Size(277, 21);
			this.cboSyncType.TabIndex = 22;
			// 
			// btnDeleteEmptyRootFolderDest
			// 
			this.btnDeleteEmptyRootFolderDest.Location = new System.Drawing.Point(822, 78);
			this.btnDeleteEmptyRootFolderDest.Name = "btnDeleteEmptyRootFolderDest";
			this.btnDeleteEmptyRootFolderDest.Size = new System.Drawing.Size(83, 25);
			this.btnDeleteEmptyRootFolderDest.TabIndex = 9;
			this.btnDeleteEmptyRootFolderDest.Text = "Empty folder";
			this.btnDeleteEmptyRootFolderDest.UseVisualStyleBackColor = true;
			this.btnDeleteEmptyRootFolderDest.Click += new System.EventHandler(this.btnDeleteEmptyRootFolderDest_Click);
			// 
			// btnRootFolderDest
			// 
			this.btnRootFolderDest.Location = new System.Drawing.Point(774, 78);
			this.btnRootFolderDest.Name = "btnRootFolderDest";
			this.btnRootFolderDest.Size = new System.Drawing.Size(42, 25);
			this.btnRootFolderDest.TabIndex = 8;
			this.btnRootFolderDest.Text = "...";
			this.btnRootFolderDest.UseVisualStyleBackColor = true;
			this.btnRootFolderDest.Click += new System.EventHandler(this.btnRootFolderDest_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radAllowedOnlyFolder);
			this.groupBox2.Controls.Add(this.btnAllowedOnlyFolder);
			this.groupBox2.Controls.Add(this.txtAllowedOnlyFolder);
			this.groupBox2.Controls.Add(this.radDisallowedFolder);
			this.groupBox2.Controls.Add(this.btnDisallowedFolder);
			this.groupBox2.Controls.Add(this.txtDisallowedFolder);
			this.groupBox2.Location = new System.Drawing.Point(160, 386);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(673, 77);
			this.groupBox2.TabIndex = 29;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Folder(; Separated)";
			// 
			// radAllowedOnlyFolder
			// 
			this.radAllowedOnlyFolder.AutoSize = true;
			this.radAllowedOnlyFolder.Location = new System.Drawing.Point(11, 48);
			this.radAllowedOnlyFolder.Name = "radAllowedOnlyFolder";
			this.radAllowedOnlyFolder.Size = new System.Drawing.Size(97, 17);
			this.radAllowedOnlyFolder.TabIndex = 3;
			this.radAllowedOnlyFolder.Text = "Folder Allowed:";
			this.radAllowedOnlyFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radAllowedOnlyFolder.UseVisualStyleBackColor = true;
			// 
			// btnAllowedOnlyFolder
			// 
			this.btnAllowedOnlyFolder.Location = new System.Drawing.Point(621, 44);
			this.btnAllowedOnlyFolder.Name = "btnAllowedOnlyFolder";
			this.btnAllowedOnlyFolder.Size = new System.Drawing.Size(42, 25);
			this.btnAllowedOnlyFolder.TabIndex = 5;
			this.btnAllowedOnlyFolder.Text = "...";
			this.btnAllowedOnlyFolder.UseVisualStyleBackColor = true;
			this.btnAllowedOnlyFolder.Click += new System.EventHandler(this.btnAllowedOnlyFolder_Click);
			// 
			// txtAllowedOnlyFolder
			// 
			this.txtAllowedOnlyFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.AllowedOnlyFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtAllowedOnlyFolder.Location = new System.Drawing.Point(128, 47);
			this.txtAllowedOnlyFolder.Name = "txtAllowedOnlyFolder";
			this.txtAllowedOnlyFolder.Size = new System.Drawing.Size(490, 20);
			this.txtAllowedOnlyFolder.TabIndex = 4;
			this.txtAllowedOnlyFolder.TextChanged += new System.EventHandler(this.txtAllowedOnlyFolder_TextChanged);
			// 
			// radDisallowedFolder
			// 
			this.radDisallowedFolder.AutoSize = true;
			this.radDisallowedFolder.Checked = true;
			this.radDisallowedFolder.Location = new System.Drawing.Point(11, 20);
			this.radDisallowedFolder.Name = "radDisallowedFolder";
			this.radDisallowedFolder.Size = new System.Drawing.Size(111, 17);
			this.radDisallowedFolder.TabIndex = 0;
			this.radDisallowedFolder.TabStop = true;
			this.radDisallowedFolder.Text = "Folder Disallowed:";
			this.radDisallowedFolder.UseVisualStyleBackColor = true;
			// 
			// btnDisallowedFolder
			// 
			this.btnDisallowedFolder.Location = new System.Drawing.Point(621, 16);
			this.btnDisallowedFolder.Name = "btnDisallowedFolder";
			this.btnDisallowedFolder.Size = new System.Drawing.Size(42, 25);
			this.btnDisallowedFolder.TabIndex = 2;
			this.btnDisallowedFolder.Text = "...";
			this.btnDisallowedFolder.UseVisualStyleBackColor = true;
			this.btnDisallowedFolder.Click += new System.EventHandler(this.btnDisallowedFolder_Click);
			// 
			// txtDisallowedFolder
			// 
			this.txtDisallowedFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.DisallowedFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtDisallowedFolder.Location = new System.Drawing.Point(128, 20);
			this.txtDisallowedFolder.Name = "txtDisallowedFolder";
			this.txtDisallowedFolder.Size = new System.Drawing.Size(490, 20);
			this.txtDisallowedFolder.TabIndex = 1;
			this.txtDisallowedFolder.TextChanged += new System.EventHandler(this.txtDisallowedFolder_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radAllowedOnlyExt);
			this.groupBox1.Controls.Add(this.radDisallowedExt);
			this.groupBox1.Controls.Add(this.txtAllowedOnlyExt);
			this.groupBox1.Controls.Add(this.txtDisallowedExt);
			this.groupBox1.Location = new System.Drawing.Point(160, 307);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(606, 69);
			this.groupBox1.TabIndex = 28;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Extension(; Separated)";
			// 
			// radAllowedOnlyExt
			// 
			this.radAllowedOnlyExt.AutoSize = true;
			this.radAllowedOnlyExt.Location = new System.Drawing.Point(301, 17);
			this.radAllowedOnlyExt.Name = "radAllowedOnlyExt";
			this.radAllowedOnlyExt.Size = new System.Drawing.Size(111, 17);
			this.radAllowedOnlyExt.TabIndex = 1;
			this.radAllowedOnlyExt.Text = "Extension Allowed";
			this.radAllowedOnlyExt.UseVisualStyleBackColor = true;
			// 
			// radDisallowedExt
			// 
			this.radDisallowedExt.AutoSize = true;
			this.radDisallowedExt.Checked = true;
			this.radDisallowedExt.Location = new System.Drawing.Point(15, 17);
			this.radDisallowedExt.Name = "radDisallowedExt";
			this.radDisallowedExt.Size = new System.Drawing.Size(125, 17);
			this.radDisallowedExt.TabIndex = 0;
			this.radDisallowedExt.TabStop = true;
			this.radDisallowedExt.Text = "Extension Disallowed";
			this.radDisallowedExt.UseVisualStyleBackColor = true;
			// 
			// txtAllowedOnlyExt
			// 
			this.txtAllowedOnlyExt.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.AllowedOnlyExt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtAllowedOnlyExt.Location = new System.Drawing.Point(301, 40);
			this.txtAllowedOnlyExt.Name = "txtAllowedOnlyExt";
			this.txtAllowedOnlyExt.Size = new System.Drawing.Size(268, 20);
			this.txtAllowedOnlyExt.TabIndex = 3;
			this.txtAllowedOnlyExt.TextChanged += new System.EventHandler(this.txtAllowedOnlyExt_TextChanged);
			// 
			// txtDisallowedExt
			// 
			this.txtDisallowedExt.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.DisallowedExt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtDisallowedExt.Location = new System.Drawing.Point(11, 40);
			this.txtDisallowedExt.Name = "txtDisallowedExt";
			this.txtDisallowedExt.Size = new System.Drawing.Size(268, 20);
			this.txtDisallowedExt.TabIndex = 2;
			this.txtDisallowedExt.TextChanged += new System.EventHandler(this.txtDisallowedExt_TextChanged);
			// 
			// chkFtpUsePassive
			// 
			this.chkFtpUsePassive.AutoSize = true;
			this.chkFtpUsePassive.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.dsConfig, "Config.FtpUsePassive", true));
			this.chkFtpUsePassive.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.dsConfig, "Config.FtpUsePassive", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkFtpUsePassive.Location = new System.Drawing.Point(792, 112);
			this.chkFtpUsePassive.Name = "chkFtpUsePassive";
			this.chkFtpUsePassive.Size = new System.Drawing.Size(116, 17);
			this.chkFtpUsePassive.TabIndex = 16;
			this.chkFtpUsePassive.Text = "FTP Passive Mode";
			this.chkFtpUsePassive.UseVisualStyleBackColor = true;
			// 
			// btnSyncNow
			// 
			this.btnSyncNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSyncNow.Location = new System.Drawing.Point(854, 485);
			this.btnSyncNow.Name = "btnSyncNow";
			this.btnSyncNow.Size = new System.Drawing.Size(89, 25);
			this.btnSyncNow.TabIndex = 33;
			this.btnSyncNow.Text = "Sync Now";
			this.btnSyncNow.UseVisualStyleBackColor = true;
			this.btnSyncNow.Click += new System.EventHandler(this.btnSyncNow_Click);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(655, 113);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(52, 13);
			this.label14.TabIndex = 14;
			this.label14.Text = "FTP Port:";
			// 
			// nudFtpPort
			// 
			this.nudFtpPort.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dsConfig, "Config.FtpPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.nudFtpPort.Location = new System.Drawing.Point(718, 111);
			this.nudFtpPort.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.nudFtpPort.Name = "nudFtpPort";
			this.nudFtpPort.Size = new System.Drawing.Size(49, 20);
			this.nudFtpPort.TabIndex = 15;
			// 
			// txtFtpFolder
			// 
			this.txtFtpFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.FtpFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtFtpFolder.Location = new System.Drawing.Point(448, 111);
			this.txtFtpFolder.Name = "txtFtpFolder";
			this.txtFtpFolder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtFtpFolder.Size = new System.Drawing.Size(183, 20);
			this.txtFtpFolder.TabIndex = 13;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(385, 113);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(62, 13);
			this.label13.TabIndex = 12;
			this.label13.Text = "FTP Folder:";
			// 
			// txtFtpHost
			// 
			this.txtFtpHost.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.FtpHost", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtFtpHost.Location = new System.Drawing.Point(160, 111);
			this.txtFtpHost.Name = "txtFtpHost";
			this.txtFtpHost.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtFtpHost.Size = new System.Drawing.Size(208, 20);
			this.txtFtpHost.TabIndex = 11;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(100, 113);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(55, 13);
			this.label10.TabIndex = 10;
			this.label10.Text = "FTP Host:";
			// 
			// txtName
			// 
			this.txtName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtName.Location = new System.Drawing.Point(160, 13);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(182, 20);
			this.txtName.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(117, 16);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(38, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "Name:";
			// 
			// txtFtpPassword
			// 
			this.txtFtpPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.FtpPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtFtpPassword.Location = new System.Drawing.Point(348, 137);
			this.txtFtpPassword.Name = "txtFtpPassword";
			this.txtFtpPassword.PasswordChar = '*';
			this.txtFtpPassword.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtFtpPassword.Size = new System.Drawing.Size(89, 20);
			this.txtFtpPassword.TabIndex = 20;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(261, 140);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(79, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "FTP Password:";
			// 
			// txtFtpId
			// 
			this.txtFtpId.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.FtpId", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtFtpId.Location = new System.Drawing.Point(160, 137);
			this.txtFtpId.Name = "txtFtpId";
			this.txtFtpId.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtFtpId.Size = new System.Drawing.Size(89, 20);
			this.txtFtpId.TabIndex = 18;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(111, 140);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(44, 13);
			this.label7.TabIndex = 17;
			this.label7.Text = "FTP ID:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(43, 173);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 13);
			this.label5.TabIndex = 21;
			this.label5.Text = "Synchronization Type:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 202);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(115, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Copy Interval(Minutes):";
			// 
			// nudIntervalMinutes
			// 
			this.nudIntervalMinutes.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dsConfig, "Config.IntervalMinutes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.nudIntervalMinutes.Location = new System.Drawing.Point(160, 198);
			this.nudIntervalMinutes.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
			this.nudIntervalMinutes.Name = "nudIntervalMinutes";
			this.nudIntervalMinutes.Size = new System.Drawing.Size(49, 20);
			this.nudIntervalMinutes.TabIndex = 24;
			// 
			// btnNow
			// 
			this.btnNow.Location = new System.Drawing.Point(685, 193);
			this.btnNow.Name = "btnNow";
			this.btnNow.Size = new System.Drawing.Size(81, 25);
			this.btnNow.TabIndex = 27;
			this.btnNow.Text = "Current Time";
			this.btnNow.UseVisualStyleBackColor = true;
			this.btnNow.Click += new System.EventHandler(this.btnNow_Click);
			// 
			// dtpDateTimeAfter
			// 
			this.dtpDateTimeAfter.CustomFormat = "yyyy-MM-dd HH:mm:ss";
			this.dtpDateTimeAfter.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.dsConfig, "Config.DateTimeAfter", true));
			this.dtpDateTimeAfter.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpDateTimeAfter.Location = new System.Drawing.Point(543, 196);
			this.dtpDateTimeAfter.Name = "dtpDateTimeAfter";
			this.dtpDateTimeAfter.Size = new System.Drawing.Size(136, 20);
			this.dtpDateTimeAfter.TabIndex = 26;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(356, 200);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(169, 13);
			this.label6.TabIndex = 25;
			this.label6.Text = "Allow only Source is after this time:";
			// 
			// btnLogFolder
			// 
			this.btnLogFolder.Location = new System.Drawing.Point(726, 470);
			this.btnLogFolder.Name = "btnLogFolder";
			this.btnLogFolder.Size = new System.Drawing.Size(42, 25);
			this.btnLogFolder.TabIndex = 32;
			this.btnLogFolder.Text = "Open";
			this.btnLogFolder.UseVisualStyleBackColor = true;
			this.btnLogFolder.Click += new System.EventHandler(this.btnLogFolder_Click);
			// 
			// btnOpenRootFolderDest
			// 
			this.btnOpenRootFolderDest.Location = new System.Drawing.Point(726, 78);
			this.btnOpenRootFolderDest.Name = "btnOpenRootFolderDest";
			this.btnOpenRootFolderDest.Size = new System.Drawing.Size(42, 25);
			this.btnOpenRootFolderDest.TabIndex = 7;
			this.btnOpenRootFolderDest.Text = "Open";
			this.btnOpenRootFolderDest.UseVisualStyleBackColor = true;
			this.btnOpenRootFolderDest.Click += new System.EventHandler(this.btnOpenRootFolderDest_Click);
			// 
			// btnOpenRootFolderSrc
			// 
			this.btnOpenRootFolderSrc.Location = new System.Drawing.Point(726, 49);
			this.btnOpenRootFolderSrc.Name = "btnOpenRootFolderSrc";
			this.btnOpenRootFolderSrc.Size = new System.Drawing.Size(42, 25);
			this.btnOpenRootFolderSrc.TabIndex = 4;
			this.btnOpenRootFolderSrc.Text = "Open";
			this.btnOpenRootFolderSrc.UseVisualStyleBackColor = true;
			this.btnOpenRootFolderSrc.Click += new System.EventHandler(this.btnOpenRootFolderSrc_Click);
			// 
			// txtLogFolder
			// 
			this.txtLogFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.LogFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtLogFolder.Location = new System.Drawing.Point(160, 473);
			this.txtLogFolder.Name = "txtLogFolder";
			this.txtLogFolder.Size = new System.Drawing.Size(560, 20);
			this.txtLogFolder.TabIndex = 31;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(95, 477);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 13);
			this.label4.TabIndex = 30;
			this.label4.Text = "Log Folder:";
			// 
			// txtRootFolderDest
			// 
			this.txtRootFolderDest.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.RootFolderDest", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtRootFolderDest.Location = new System.Drawing.Point(160, 81);
			this.txtRootFolderDest.Name = "txtRootFolderDest";
			this.txtRootFolderDest.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtRootFolderDest.Size = new System.Drawing.Size(560, 20);
			this.txtRootFolderDest.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(60, 85);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Destination Folder:";
			// 
			// txtRootFolderSrc
			// 
			this.txtRootFolderSrc.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.dsConfig, "Config.RootFolderSrc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtRootFolderSrc.Location = new System.Drawing.Point(160, 52);
			this.txtRootFolderSrc.Name = "txtRootFolderSrc";
			this.txtRootFolderSrc.Size = new System.Drawing.Size(560, 20);
			this.txtRootFolderSrc.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(79, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Source Folder:";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sttlblMsg,
            this.sttlblFullPath});
			this.statusStrip1.Location = new System.Drawing.Point(0, 701);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(955, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// sttlblMsg
			// 
			this.sttlblMsg.Name = "sttlblMsg";
			this.sttlblMsg.Size = new System.Drawing.Size(470, 17);
			this.sttlblMsg.Spring = true;
			this.sttlblMsg.Text = "대기";
			this.sttlblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// sttlblFullPath
			// 
			this.sttlblFullPath.Name = "sttlblFullPath";
			this.sttlblFullPath.Size = new System.Drawing.Size(470, 17);
			this.sttlblFullPath.Spring = true;
			// 
			// dlgSave
			// 
			this.dlgSave.Filter = "DoBackup files|*.doback";
			this.dlgSave.RestoreDirectory = true;
			// 
			// dlgOpen
			// 
			this.dlgOpen.Filter = "DoBackup files|*.doback";
			this.dlgOpen.RestoreDirectory = true;
			// 
			// frmConfig
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(955, 748);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmConfig";
			this.Text = "Config";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfig_FormClosing);
			this.Load += new System.EventHandler(this.frmConfig_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grvList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsConfig)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFtpPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIntervalMinutes)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tlsbtnOpen;
		private System.Windows.Forms.ToolStripButton tlsbtnSave;
		private System.Windows.Forms.ToolStripButton tlsbtnSaveAs;
		private System.Windows.Forms.ToolStripButton tlsbtnClose;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel sttlblMsg;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton tlsbtnHelp;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView grvList;
		private System.Windows.Forms.Button btnNow;
		private System.Windows.Forms.DateTimePicker dtpDateTimeAfter;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnLogFolder;
		private System.Windows.Forms.Button btnOpenRootFolderDest;
		private System.Windows.Forms.Button btnOpenRootFolderSrc;
		private System.Windows.Forms.TextBox txtLogFolder;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtRootFolderDest;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtRootFolderSrc;
		private System.Windows.Forms.Label label2;
		private System.Data.DataSet dsConfig;
		private System.Data.DataTable dataTable1;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataColumn dataColumn7;
		private System.Data.DataColumn dataColumn8;
		private System.Data.DataColumn dataColumn9;
		private System.Data.DataColumn dataColumn10;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudIntervalMinutes;
		private System.Windows.Forms.TextBox txtFtpPassword;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtFtpId;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label9;
		private System.Data.DataColumn dataColumn11;
		private System.Windows.Forms.TextBox txtFtpHost;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtFtpFolder;
		private System.Windows.Forms.Label label13;
		private System.Data.DataColumn dataColumn12;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown nudFtpPort;
		private System.Data.DataColumn dataColumn13;
		private System.Windows.Forms.Button btnSyncNow;
		private System.Windows.Forms.ToolStripStatusLabel sttlblFullPath;
		private System.Data.DataColumn dataColumn14;
		private System.Windows.Forms.CheckBox chkFtpUsePassive;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radAllowedOnlyFolder;
		private System.Windows.Forms.Button btnAllowedOnlyFolder;
		private System.Windows.Forms.TextBox txtAllowedOnlyFolder;
		private System.Windows.Forms.RadioButton radDisallowedFolder;
		private System.Windows.Forms.Button btnDisallowedFolder;
		private System.Windows.Forms.TextBox txtDisallowedFolder;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radAllowedOnlyExt;
		private System.Windows.Forms.RadioButton radDisallowedExt;
		private System.Windows.Forms.TextBox txtAllowedOnlyExt;
		private System.Windows.Forms.TextBox txtDisallowedExt;
		private System.Data.DataColumn dataColumn15;
		private System.Data.DataColumn dataColumn16;
		private System.Windows.Forms.Button btnRootFolderDest;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton tlsbtnMoveUp;
		private System.Windows.Forms.ToolStripButton tlsbtnMoveDown;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn rootFolderSrcDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn rootFolderDestDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ftpHostDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ftpFolderDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ftpPortDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn logFolderDataGridViewTextBoxColumn;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton tlsbtnCopy;
		private System.Windows.Forms.ToolStripButton tlsbtnCut;
		private System.Windows.Forms.ToolStripButton tlsbtnPaste;
		private System.Windows.Forms.ToolStripButton tlsbtnDelete;
		private System.Windows.Forms.ComboBox cboSyncType;
		private System.Windows.Forms.Label label5;
		private System.Data.DataColumn dataColumn17;
		private System.Windows.Forms.Button btnDeleteEmptyRootFolderDest;
		private System.Windows.Forms.CheckBox chkMinifyJs;
		private System.Data.DataColumn dataColumn18;
		private System.Data.DataColumn dataColumn19;
		private System.Windows.Forms.Button btnJsFullPathRefered;
		private System.Windows.Forms.TextBox txtJsFullPathRefered;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtFullPathReferencingJs;
		private System.Windows.Forms.Button btnFullPathReferencingJs;
		private System.Windows.Forms.Label label12;
		private System.Data.DataColumn dataColumn20;
        private System.Data.DataColumn dataColumn21;
        private System.Windows.Forms.CheckBox chkEmptyFolderHasNoFile;
	}
}