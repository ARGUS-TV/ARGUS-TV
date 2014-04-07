namespace ArgusTV.UI.Console.Panels
{
    partial class RecordingFormatsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._formatsDataGridView = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._formatsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._createNewButton = new System.Windows.Forms.Button();
            this._commandGroupBox = new System.Windows.Forms.GroupBox();
            this._resetCurrentToDefaultButton = new System.Windows.Forms.Button();
            this._helpRecFormatLinkLabel = new System.Windows.Forms.LinkLabel();
            this._exampleTextBox = new System.Windows.Forms.TextBox();
            this._formatTextBox = new System.Windows.Forms.TextBox();
            this._commandLabel = new System.Windows.Forms.Label();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this._nameLabel = new System.Windows.Forms.Label();
            this._recordingFormatGroupBox = new System.Windows.Forms.GroupBox();
            this._recRadioExampleTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._recRadioFormatTextBox = new System.Windows.Forms.TextBox();
            this._recOneTimeExampleTextBox = new System.Windows.Forms.TextBox();
            this._recSeriesExampleTextBox = new System.Windows.Forms.TextBox();
            this._recOneTimeLabel = new System.Windows.Forms.Label();
            this._resetRecFormatButton = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this._recOneTimeFormatTextBox = new System.Windows.Forms.TextBox();
            this._recFormatInfolabel = new System.Windows.Forms.Label();
            this._recSeriesFormatTextBox = new System.Windows.Forms.TextBox();
            this._recSeriesFormatLabel = new System.Windows.Forms.Label();
            this._customLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._formatsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._formatsBindingSource)).BeginInit();
            this._commandGroupBox.SuspendLayout();
            this._recordingFormatGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _formatsDataGridView
            // 
            this._formatsDataGridView.AllowUserToAddRows = false;
            this._formatsDataGridView.AllowUserToDeleteRows = false;
            this._formatsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._formatsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._formatsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._formatsDataGridView.AutoGenerateColumns = false;
            this._formatsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._formatsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._formatsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._formatsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn});
            this._formatsDataGridView.DataSource = this._formatsBindingSource;
            this._formatsDataGridView.GridColor = System.Drawing.Color.White;
            this._formatsDataGridView.Location = new System.Drawing.Point(0, 366);
            this._formatsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._formatsDataGridView.MultiSelect = false;
            this._formatsDataGridView.Name = "_formatsDataGridView";
            this._formatsDataGridView.ReadOnly = true;
            this._formatsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._formatsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._formatsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._formatsDataGridView.Size = new System.Drawing.Size(1020, 326);
            this._formatsDataGridView.TabIndex = 2;
            this._formatsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._formatsDataGridView_DataError);
            this._formatsDataGridView.SelectionChanged += new System.EventHandler(this._formatsDataGridView_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _formatsBindingSource
            // 
            this._formatsBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.RecordingFileFormat>);
            this._formatsBindingSource.Sort = "";
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(1029, 448);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 4;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _createNewButton
            // 
            this._createNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._createNewButton.Location = new System.Drawing.Point(1029, 403);
            this._createNewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(135, 35);
            this._createNewButton.TabIndex = 3;
            this._createNewButton.Text = "Create New";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _commandGroupBox
            // 
            this._commandGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._commandGroupBox.Controls.Add(this._resetCurrentToDefaultButton);
            this._commandGroupBox.Controls.Add(this._helpRecFormatLinkLabel);
            this._commandGroupBox.Controls.Add(this._exampleTextBox);
            this._commandGroupBox.Controls.Add(this._formatTextBox);
            this._commandGroupBox.Controls.Add(this._commandLabel);
            this._commandGroupBox.Controls.Add(this._nameTextBox);
            this._commandGroupBox.Controls.Add(this._nameLabel);
            this._commandGroupBox.Location = new System.Drawing.Point(0, 702);
            this._commandGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandGroupBox.Name = "_commandGroupBox";
            this._commandGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandGroupBox.Size = new System.Drawing.Size(1020, 194);
            this._commandGroupBox.TabIndex = 10;
            this._commandGroupBox.TabStop = false;
            this._commandGroupBox.Text = "Recording File Name Format";
            // 
            // _resetCurrentToDefaultButton
            // 
            this._resetCurrentToDefaultButton.Location = new System.Drawing.Point(111, 146);
            this._resetCurrentToDefaultButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._resetCurrentToDefaultButton.Name = "_resetCurrentToDefaultButton";
            this._resetCurrentToDefaultButton.Size = new System.Drawing.Size(150, 35);
            this._resetCurrentToDefaultButton.TabIndex = 11;
            this._resetCurrentToDefaultButton.Text = "Reset to Default";
            this._resetCurrentToDefaultButton.UseVisualStyleBackColor = true;
            this._resetCurrentToDefaultButton.Click += new System.EventHandler(this._resetCurrentToDefaultButton_Click);
            // 
            // _helpRecFormatLinkLabel
            // 
            this._helpRecFormatLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._helpRecFormatLinkLabel.AutoSize = true;
            this._helpRecFormatLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._helpRecFormatLinkLabel.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this._helpRecFormatLinkLabel.Location = new System.Drawing.Point(968, 42);
            this._helpRecFormatLinkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._helpRecFormatLinkLabel.Name = "_helpRecFormatLinkLabel";
            this._helpRecFormatLinkLabel.Size = new System.Drawing.Size(42, 20);
            this._helpRecFormatLinkLabel.TabIndex = 4;
            this._helpRecFormatLinkLabel.TabStop = true;
            this._helpRecFormatLinkLabel.Text = "Help";
            this._helpRecFormatLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._helpRecFormatLinkLabel_LinkClicked);
            // 
            // _exampleTextBox
            // 
            this._exampleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._exampleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this._exampleTextBox.ForeColor = System.Drawing.Color.DimGray;
            this._exampleTextBox.Location = new System.Drawing.Point(111, 109);
            this._exampleTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._exampleTextBox.Name = "_exampleTextBox";
            this._exampleTextBox.ReadOnly = true;
            this._exampleTextBox.Size = new System.Drawing.Size(898, 23);
            this._exampleTextBox.TabIndex = 5;
            this._exampleTextBox.TabStop = false;
            // 
            // _formatTextBox
            // 
            this._formatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._formatTextBox.Location = new System.Drawing.Point(111, 69);
            this._formatTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._formatTextBox.Name = "_formatTextBox";
            this._formatTextBox.Size = new System.Drawing.Size(898, 26);
            this._formatTextBox.TabIndex = 3;
            this._formatTextBox.TextChanged += new System.EventHandler(this._formatTextBox_TextChanged);
            // 
            // _commandLabel
            // 
            this._commandLabel.AutoSize = true;
            this._commandLabel.Location = new System.Drawing.Point(12, 74);
            this._commandLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._commandLabel.Name = "_commandLabel";
            this._commandLabel.Size = new System.Drawing.Size(64, 20);
            this._commandLabel.TabIndex = 2;
            this._commandLabel.Text = "Format:";
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Location = new System.Drawing.Point(111, 29);
            this._nameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(448, 26);
            this._nameTextBox.TabIndex = 1;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Location = new System.Drawing.Point(12, 34);
            this._nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(55, 20);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "Name:";
            // 
            // _recordingFormatGroupBox
            // 
            this._recordingFormatGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recordingFormatGroupBox.Controls.Add(this._recRadioExampleTextBox);
            this._recordingFormatGroupBox.Controls.Add(this.label1);
            this._recordingFormatGroupBox.Controls.Add(this._recRadioFormatTextBox);
            this._recordingFormatGroupBox.Controls.Add(this._recOneTimeExampleTextBox);
            this._recordingFormatGroupBox.Controls.Add(this._recSeriesExampleTextBox);
            this._recordingFormatGroupBox.Controls.Add(this._recOneTimeLabel);
            this._recordingFormatGroupBox.Controls.Add(this._resetRecFormatButton);
            this._recordingFormatGroupBox.Controls.Add(this.linkLabel1);
            this._recordingFormatGroupBox.Controls.Add(this._recOneTimeFormatTextBox);
            this._recordingFormatGroupBox.Controls.Add(this._recFormatInfolabel);
            this._recordingFormatGroupBox.Controls.Add(this._recSeriesFormatTextBox);
            this._recordingFormatGroupBox.Controls.Add(this._recSeriesFormatLabel);
            this._recordingFormatGroupBox.Location = new System.Drawing.Point(0, 0);
            this._recordingFormatGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recordingFormatGroupBox.Name = "_recordingFormatGroupBox";
            this._recordingFormatGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recordingFormatGroupBox.Size = new System.Drawing.Size(1020, 325);
            this._recordingFormatGroupBox.TabIndex = 0;
            this._recordingFormatGroupBox.TabStop = false;
            this._recordingFormatGroupBox.Text = "Default Recordings File Name Formats";
            // 
            // _recRadioExampleTextBox
            // 
            this._recRadioExampleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recRadioExampleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this._recRadioExampleTextBox.ForeColor = System.Drawing.Color.DimGray;
            this._recRadioExampleTextBox.Location = new System.Drawing.Point(96, 237);
            this._recRadioExampleTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recRadioExampleTextBox.Name = "_recRadioExampleTextBox";
            this._recRadioExampleTextBox.ReadOnly = true;
            this._recRadioExampleTextBox.Size = new System.Drawing.Size(913, 23);
            this._recRadioExampleTextBox.TabIndex = 10;
            this._recRadioExampleTextBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 206);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Radio:";
            // 
            // _recRadioFormatTextBox
            // 
            this._recRadioFormatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recRadioFormatTextBox.Location = new System.Drawing.Point(96, 202);
            this._recRadioFormatTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recRadioFormatTextBox.Name = "_recRadioFormatTextBox";
            this._recRadioFormatTextBox.Size = new System.Drawing.Size(913, 26);
            this._recRadioFormatTextBox.TabIndex = 9;
            this._recRadioFormatTextBox.TextChanged += new System.EventHandler(this._recRadioFormatTextBox_TextChanged);
            // 
            // _recOneTimeExampleTextBox
            // 
            this._recOneTimeExampleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recOneTimeExampleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this._recOneTimeExampleTextBox.ForeColor = System.Drawing.Color.DimGray;
            this._recOneTimeExampleTextBox.Location = new System.Drawing.Point(96, 165);
            this._recOneTimeExampleTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recOneTimeExampleTextBox.Name = "_recOneTimeExampleTextBox";
            this._recOneTimeExampleTextBox.ReadOnly = true;
            this._recOneTimeExampleTextBox.Size = new System.Drawing.Size(913, 23);
            this._recOneTimeExampleTextBox.TabIndex = 7;
            this._recOneTimeExampleTextBox.TabStop = false;
            // 
            // _recSeriesExampleTextBox
            // 
            this._recSeriesExampleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recSeriesExampleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this._recSeriesExampleTextBox.ForeColor = System.Drawing.Color.DimGray;
            this._recSeriesExampleTextBox.Location = new System.Drawing.Point(96, 92);
            this._recSeriesExampleTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recSeriesExampleTextBox.Name = "_recSeriesExampleTextBox";
            this._recSeriesExampleTextBox.ReadOnly = true;
            this._recSeriesExampleTextBox.Size = new System.Drawing.Size(913, 23);
            this._recSeriesExampleTextBox.TabIndex = 4;
            this._recSeriesExampleTextBox.TabStop = false;
            // 
            // _recOneTimeLabel
            // 
            this._recOneTimeLabel.AutoSize = true;
            this._recOneTimeLabel.Location = new System.Drawing.Point(9, 134);
            this._recOneTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._recOneTimeLabel.Name = "_recOneTimeLabel";
            this._recOneTimeLabel.Size = new System.Drawing.Size(78, 20);
            this._recOneTimeLabel.TabIndex = 5;
            this._recOneTimeLabel.Text = "One-time:";
            // 
            // _resetRecFormatButton
            // 
            this._resetRecFormatButton.Location = new System.Drawing.Point(96, 274);
            this._resetRecFormatButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._resetRecFormatButton.Name = "_resetRecFormatButton";
            this._resetRecFormatButton.Size = new System.Drawing.Size(150, 35);
            this._resetRecFormatButton.TabIndex = 11;
            this._resetRecFormatButton.Text = "Reset to Defaults";
            this._resetRecFormatButton.UseVisualStyleBackColor = true;
            this._resetRecFormatButton.Click += new System.EventHandler(this._resetRecFormatButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this.linkLabel1.Location = new System.Drawing.Point(968, 29);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(42, 20);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._helpRecFormatLinkLabel_LinkClicked);
            // 
            // _recOneTimeFormatTextBox
            // 
            this._recOneTimeFormatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recOneTimeFormatTextBox.Location = new System.Drawing.Point(96, 129);
            this._recOneTimeFormatTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recOneTimeFormatTextBox.Name = "_recOneTimeFormatTextBox";
            this._recOneTimeFormatTextBox.Size = new System.Drawing.Size(913, 26);
            this._recOneTimeFormatTextBox.TabIndex = 6;
            this._recOneTimeFormatTextBox.TextChanged += new System.EventHandler(this._recOneTimeFormatTextBox_TextChanged);
            // 
            // _recFormatInfolabel
            // 
            this._recFormatInfolabel.AutoSize = true;
            this._recFormatInfolabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recFormatInfolabel.ForeColor = System.Drawing.Color.DimGray;
            this._recFormatInfolabel.Location = new System.Drawing.Point(9, 29);
            this._recFormatInfolabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._recFormatInfolabel.Name = "_recFormatInfolabel";
            this._recFormatInfolabel.Size = new System.Drawing.Size(536, 20);
            this._recFormatInfolabel.TabIndex = 0;
            this._recFormatInfolabel.Text = "Recording file names (and folders) will be defined using these formats:";
            // 
            // _recSeriesFormatTextBox
            // 
            this._recSeriesFormatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recSeriesFormatTextBox.Location = new System.Drawing.Point(96, 57);
            this._recSeriesFormatTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recSeriesFormatTextBox.Name = "_recSeriesFormatTextBox";
            this._recSeriesFormatTextBox.Size = new System.Drawing.Size(913, 26);
            this._recSeriesFormatTextBox.TabIndex = 3;
            this._recSeriesFormatTextBox.TextChanged += new System.EventHandler(this._recSeriesFormatTextBox_TextChanged);
            // 
            // _recSeriesFormatLabel
            // 
            this._recSeriesFormatLabel.AutoSize = true;
            this._recSeriesFormatLabel.Location = new System.Drawing.Point(9, 62);
            this._recSeriesFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._recSeriesFormatLabel.Name = "_recSeriesFormatLabel";
            this._recSeriesFormatLabel.Size = new System.Drawing.Size(58, 20);
            this._recSeriesFormatLabel.TabIndex = 2;
            this._recSeriesFormatLabel.Text = "Series:";
            // 
            // _customLabel
            // 
            this._customLabel.AutoSize = true;
            this._customLabel.Location = new System.Drawing.Point(-2, 340);
            this._customLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._customLabel.Name = "_customLabel";
            this._customLabel.Size = new System.Drawing.Size(279, 20);
            this._customLabel.TabIndex = 1;
            this._customLabel.Text = "Custom Recording File Name Formats";
            // 
            // RecordingFormatsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._customLabel);
            this.Controls.Add(this._recordingFormatGroupBox);
            this.Controls.Add(this._formatsDataGridView);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._createNewButton);
            this.Controls.Add(this._commandGroupBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "RecordingFormatsPanel";
            this.Size = new System.Drawing.Size(1164, 895);
            this.Load += new System.EventHandler(this.RecordingFormatsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._formatsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._formatsBindingSource)).EndInit();
            this._commandGroupBox.ResumeLayout(false);
            this._commandGroupBox.PerformLayout();
            this._recordingFormatGroupBox.ResumeLayout(false);
            this._recordingFormatGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _formatsDataGridView;
        private System.Windows.Forms.BindingSource _formatsBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.GroupBox _commandGroupBox;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Label _nameLabel;
        private System.Windows.Forms.TextBox _formatTextBox;
        private System.Windows.Forms.Label _commandLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.TextBox _exampleTextBox;
        private System.Windows.Forms.LinkLabel _helpRecFormatLinkLabel;
        private System.Windows.Forms.GroupBox _recordingFormatGroupBox;
        private System.Windows.Forms.TextBox _recOneTimeExampleTextBox;
        private System.Windows.Forms.TextBox _recSeriesExampleTextBox;
        private System.Windows.Forms.Label _recOneTimeLabel;
        private System.Windows.Forms.Button _resetRecFormatButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox _recOneTimeFormatTextBox;
        private System.Windows.Forms.Label _recFormatInfolabel;
        private System.Windows.Forms.TextBox _recSeriesFormatTextBox;
        private System.Windows.Forms.Label _recSeriesFormatLabel;
        private System.Windows.Forms.Label _customLabel;
        private System.Windows.Forms.Button _resetCurrentToDefaultButton;
        private System.Windows.Forms.TextBox _recRadioExampleTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _recRadioFormatTextBox;
    }
}
