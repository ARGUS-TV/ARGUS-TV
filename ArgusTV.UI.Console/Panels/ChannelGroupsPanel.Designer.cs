namespace ArgusTV.UI.Console.Panels
{
    partial class ChannelGroupsPanel
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
            this._channelsDataGridView = new System.Windows.Forms.DataGridView();
            this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._channelsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._moveUpButton = new System.Windows.Forms.Button();
            this._moveDownButton = new System.Windows.Forms.Button();
            this._moveBottomButton = new System.Windows.Forms.Button();
            this._moveTopButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._createNewButton = new System.Windows.Forms.Button();
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._groupsTreeView = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this._sortByLcnButton = new System.Windows.Forms.Button();
            this._removeChannelButton = new System.Windows.Forms.Button();
            this._addChannelButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).BeginInit();
            this._tableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _channelsDataGridView
            // 
            this._channelsDataGridView.AllowUserToAddRows = false;
            this._channelsDataGridView.AllowUserToDeleteRows = false;
            this._channelsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._channelsDataGridView.AutoGenerateColumns = false;
            this._channelsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._channelsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._channelsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._channelsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.displayNameDataGridViewTextBoxColumn});
            this._channelsDataGridView.DataSource = this._channelsBindingSource;
            this._channelsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._channelsDataGridView.GridColor = System.Drawing.Color.White;
            this._channelsDataGridView.Location = new System.Drawing.Point(699, 42);
            this._channelsDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this._channelsDataGridView.Name = "_channelsDataGridView";
            this._channelsDataGridView.ReadOnly = true;
            this._channelsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._channelsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._channelsDataGridView.Size = new System.Drawing.Size(371, 635);
            this._channelsDataGridView.StandardTab = true;
            this._channelsDataGridView.TabIndex = 2;
            this._channelsDataGridView.SelectionChanged += new System.EventHandler(this._channelsDataGridView_SelectionChanged);
            this._channelsDataGridView.DoubleClick += new System.EventHandler(this._channelsDataGridView_DoubleClick);
            // 
            // displayNameDataGridViewTextBoxColumn
            // 
            this.displayNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "CombinedDisplayName";
            this.displayNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
            this.displayNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.displayNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _channelsBindingSource
            // 
            this._channelsBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.Channel>);
            this._channelsBindingSource.Sort = "";
            // 
            // _moveUpButton
            // 
            this._moveUpButton.Enabled = false;
            this._moveUpButton.Location = new System.Drawing.Point(0, 211);
            this._moveUpButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._moveUpButton.Name = "_moveUpButton";
            this._moveUpButton.Size = new System.Drawing.Size(135, 35);
            this._moveUpButton.TabIndex = 93;
            this._moveUpButton.Text = "Move Up";
            this._moveUpButton.UseVisualStyleBackColor = true;
            this._moveUpButton.Click += new System.EventHandler(this._moveUpButton_Click);
            // 
            // _moveDownButton
            // 
            this._moveDownButton.Enabled = false;
            this._moveDownButton.Location = new System.Drawing.Point(0, 255);
            this._moveDownButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._moveDownButton.Name = "_moveDownButton";
            this._moveDownButton.Size = new System.Drawing.Size(135, 35);
            this._moveDownButton.TabIndex = 94;
            this._moveDownButton.Text = "Move Down";
            this._moveDownButton.UseVisualStyleBackColor = true;
            this._moveDownButton.Click += new System.EventHandler(this._moveDownButton_Click);
            // 
            // _moveBottomButton
            // 
            this._moveBottomButton.Enabled = false;
            this._moveBottomButton.Location = new System.Drawing.Point(0, 300);
            this._moveBottomButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._moveBottomButton.Name = "_moveBottomButton";
            this._moveBottomButton.Size = new System.Drawing.Size(135, 35);
            this._moveBottomButton.TabIndex = 95;
            this._moveBottomButton.Text = "To Bottom";
            this._moveBottomButton.UseVisualStyleBackColor = true;
            this._moveBottomButton.Click += new System.EventHandler(this._moveBottomButton_Click);
            // 
            // _moveTopButton
            // 
            this._moveTopButton.Enabled = false;
            this._moveTopButton.Location = new System.Drawing.Point(0, 166);
            this._moveTopButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._moveTopButton.Name = "_moveTopButton";
            this._moveTopButton.Size = new System.Drawing.Size(135, 35);
            this._moveTopButton.TabIndex = 92;
            this._moveTopButton.Text = "To Top";
            this._moveTopButton.UseVisualStyleBackColor = true;
            this._moveTopButton.Click += new System.EventHandler(this._moveTopButton_Click);
            // 
            // _deleteButton
            // 
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(0, 434);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 97;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _createNewButton
            // 
            this._createNewButton.Location = new System.Drawing.Point(0, 389);
            this._createNewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(135, 35);
            this._createNewButton.TabIndex = 96;
            this._createNewButton.Text = "Create New";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 3;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this._tableLayoutPanel.Controls.Add(this._groupsTreeView, 0, 1);
            this._tableLayoutPanel.Controls.Add(this._channelsDataGridView, 2, 1);
            this._tableLayoutPanel.Controls.Add(this.panel1, 1, 1);
            this._tableLayoutPanel.Controls.Add(this._channelTypeComboBox, 0, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(1070, 677);
            this._tableLayoutPanel.TabIndex = 100;
            // 
            // _groupsTreeView
            // 
            this._groupsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupsTreeView.CheckBoxes = true;
            this._groupsTreeView.HideSelection = false;
            this._groupsTreeView.ItemHeight = 18;
            this._groupsTreeView.LabelEdit = true;
            this._groupsTreeView.Location = new System.Drawing.Point(0, 42);
            this._groupsTreeView.Margin = new System.Windows.Forms.Padding(0);
            this._groupsTreeView.Name = "_groupsTreeView";
            this._groupsTreeView.Size = new System.Drawing.Size(555, 635);
            this._groupsTreeView.TabIndex = 0;
            this._groupsTreeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this._groupsTreeView_BeforeLabelEdit);
            this._groupsTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this._groupsTreeView_AfterLabelEdit);
            this._groupsTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this._groupsTreeView_AfterCollapse);
            this._groupsTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._groupsTreeView_BeforeExpand);
            this._groupsTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this._groupsTreeView_AfterExpand);
            this._groupsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._groupsTreeView_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._sortByLcnButton);
            this.panel1.Controls.Add(this._removeChannelButton);
            this.panel1.Controls.Add(this._createNewButton);
            this.panel1.Controls.Add(this._deleteButton);
            this.panel1.Controls.Add(this._addChannelButton);
            this.panel1.Controls.Add(this._moveTopButton);
            this.panel1.Controls.Add(this._moveBottomButton);
            this.panel1.Controls.Add(this._moveUpButton);
            this.panel1.Controls.Add(this._moveDownButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(559, 47);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(136, 625);
            this.panel1.TabIndex = 1;
            // 
            // _sortByLcnButton
            // 
            this._sortByLcnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._sortByLcnButton.Location = new System.Drawing.Point(1, 523);
            this._sortByLcnButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._sortByLcnButton.Name = "_sortByLcnButton";
            this._sortByLcnButton.Size = new System.Drawing.Size(135, 35);
            this._sortByLcnButton.TabIndex = 98;
            this._sortByLcnButton.Text = "Sort By LCN";
            this._sortByLcnButton.UseVisualStyleBackColor = true;
            this._sortByLcnButton.Click += new System.EventHandler(this._sortByLcnButton_Click);
            // 
            // _removeChannelButton
            // 
            this._removeChannelButton.Enabled = false;
            this._removeChannelButton.Location = new System.Drawing.Point(0, 77);
            this._removeChannelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._removeChannelButton.Name = "_removeChannelButton";
            this._removeChannelButton.Size = new System.Drawing.Size(135, 35);
            this._removeChannelButton.TabIndex = 91;
            this._removeChannelButton.Text = ">>";
            this._removeChannelButton.UseVisualStyleBackColor = true;
            this._removeChannelButton.Click += new System.EventHandler(this._removeChannelButton_Click);
            // 
            // _addChannelButton
            // 
            this._addChannelButton.Enabled = false;
            this._addChannelButton.Location = new System.Drawing.Point(0, 32);
            this._addChannelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._addChannelButton.Name = "_addChannelButton";
            this._addChannelButton.Size = new System.Drawing.Size(135, 35);
            this._addChannelButton.TabIndex = 90;
            this._addChannelButton.Text = "<<";
            this._addChannelButton.UseVisualStyleBackColor = true;
            this._addChannelButton.Click += new System.EventHandler(this._addChannelButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television Channel Groups",
            "Radio Channel Groups"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(0, 0);
            this._channelTypeComboBox.Margin = new System.Windows.Forms.Padding(0);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(373, 28);
            this._channelTypeComboBox.TabIndex = 104;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // ChannelGroupsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "ChannelGroupsPanel";
            this.Size = new System.Drawing.Size(1070, 677);
            this.Load += new System.EventHandler(this.ChannelsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).EndInit();
            this._tableLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _channelsDataGridView;
        private System.Windows.Forms.BindingSource _channelsBindingSource;
        private System.Windows.Forms.Button _moveUpButton;
        private System.Windows.Forms.Button _moveDownButton;
        private System.Windows.Forms.Button _moveBottomButton;
        private System.Windows.Forms.Button _moveTopButton;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.TreeView _groupsTreeView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _addChannelButton;
        private System.Windows.Forms.Button _removeChannelButton;
        private System.Windows.Forms.Button _sortByLcnButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
    }
}
