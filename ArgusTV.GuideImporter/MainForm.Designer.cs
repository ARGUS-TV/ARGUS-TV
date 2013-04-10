namespace ArgusTV.GuideImporter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._importerLabel = new System.Windows.Forms.Label();
            this._pluginComboBox = new System.Windows.Forms.ComboBox();
            this._importButton = new System.Windows.Forms.Button();
            this._availableChannelsLabel = new System.Windows.Forms.Label();
            this._channelsToSkipLabel = new System.Windows.Forms.Label();
            this._availableChannelsListBox = new System.Windows.Forms.ListBox();
            this._addChannelButton = new System.Windows.Forms.Button();
            this._removeChannelButton = new System.Windows.Forms.Button();
            this._channelsToSkipListBox = new System.Windows.Forms.ListBox();
            this._removeAllChannelButton = new System.Windows.Forms.Button();
            this._mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._availablePluginPanel = new System.Windows.Forms.Panel();
            this._feedBackLabel = new System.Windows.Forms.Label();
            this._configureButton = new System.Windows.Forms.Button();
            this._importProgressBar = new System.Windows.Forms.ProgressBar();
            this._descriptionTextBox = new System.Windows.Forms.TextBox();
            this._descriptionLabel = new System.Windows.Forms.Label();
            this._buttonPanel = new System.Windows.Forms.Panel();
            this._addAllChannelButton = new System.Windows.Forms.Button();
            this._availableChannelsPanel = new System.Windows.Forms.Panel();
            this._refreshButton = new System.Windows.Forms.Button();
            this._skipChannelsPanel = new System.Windows.Forms.Panel();
            this._saveButton = new System.Windows.Forms.Button();
            this._mainTableLayoutPanel.SuspendLayout();
            this._availablePluginPanel.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this._availableChannelsPanel.SuspendLayout();
            this._skipChannelsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _importerLabel
            // 
            this._importerLabel.AutoSize = true;
            this._importerLabel.Location = new System.Drawing.Point(0, 3);
            this._importerLabel.Name = "_importerLabel";
            this._importerLabel.Size = new System.Drawing.Size(79, 13);
            this._importerLabel.TabIndex = 0;
            this._importerLabel.Text = "Importer plugin:";
            // 
            // _pluginComboBox
            // 
            this._pluginComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pluginComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._pluginComboBox.FormattingEnabled = true;
            this._pluginComboBox.Location = new System.Drawing.Point(88, 0);
            this._pluginComboBox.Name = "_pluginComboBox";
            this._pluginComboBox.Size = new System.Drawing.Size(455, 21);
            this._pluginComboBox.TabIndex = 1;
            this._pluginComboBox.SelectedIndexChanged += new System.EventHandler(this._pluginComboBox_SelectedIndexChanged);
            // 
            // _importButton
            // 
            this._importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._importButton.Location = new System.Drawing.Point(549, -1);
            this._importButton.Name = "_importButton";
            this._importButton.Size = new System.Drawing.Size(75, 23);
            this._importButton.TabIndex = 4;
            this._importButton.Text = "Import";
            this._importButton.UseVisualStyleBackColor = true;
            this._importButton.Click += new System.EventHandler(this._importButton_Click);
            // 
            // _availableChannelsLabel
            // 
            this._availableChannelsLabel.AutoSize = true;
            this._availableChannelsLabel.Location = new System.Drawing.Point(3, 70);
            this._availableChannelsLabel.Name = "_availableChannelsLabel";
            this._availableChannelsLabel.Size = new System.Drawing.Size(99, 13);
            this._availableChannelsLabel.TabIndex = 5;
            this._availableChannelsLabel.Text = "Available channels:";
            // 
            // _channelsToSkipLabel
            // 
            this._channelsToSkipLabel.AutoSize = true;
            this._channelsToSkipLabel.Location = new System.Drawing.Point(394, 70);
            this._channelsToSkipLabel.Name = "_channelsToSkipLabel";
            this._channelsToSkipLabel.Size = new System.Drawing.Size(88, 13);
            this._channelsToSkipLabel.TabIndex = 6;
            this._channelsToSkipLabel.Text = "Channels to skip:";
            // 
            // _availableChannelsListBox
            // 
            this._availableChannelsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._availableChannelsListBox.DisplayMember = "ChannelName";
            this._availableChannelsListBox.FormattingEnabled = true;
            this._availableChannelsListBox.Location = new System.Drawing.Point(0, 0);
            this._availableChannelsListBox.Name = "_availableChannelsListBox";
            this._availableChannelsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this._availableChannelsListBox.Size = new System.Drawing.Size(235, 277);
            this._availableChannelsListBox.Sorted = true;
            this._availableChannelsListBox.TabIndex = 7;
            this._availableChannelsListBox.SelectedIndexChanged += new System.EventHandler(this._availableChannelsListBox_SelectedIndexChanged);
            // 
            // _addChannelButton
            // 
            this._addChannelButton.Enabled = false;
            this._addChannelButton.Location = new System.Drawing.Point(40, 0);
            this._addChannelButton.Name = "_addChannelButton";
            this._addChannelButton.Size = new System.Drawing.Size(73, 23);
            this._addChannelButton.TabIndex = 104;
            this._addChannelButton.Text = ">";
            this._addChannelButton.UseVisualStyleBackColor = true;
            this._addChannelButton.Click += new System.EventHandler(this._addChannelButton_Click);
            // 
            // _removeChannelButton
            // 
            this._removeChannelButton.Enabled = false;
            this._removeChannelButton.Location = new System.Drawing.Point(40, 31);
            this._removeChannelButton.Name = "_removeChannelButton";
            this._removeChannelButton.Size = new System.Drawing.Size(73, 23);
            this._removeChannelButton.TabIndex = 103;
            this._removeChannelButton.Text = "<";
            this._removeChannelButton.UseVisualStyleBackColor = true;
            this._removeChannelButton.Click += new System.EventHandler(this._removeChannelButton_Click);
            // 
            // _channelsToSkipListBox
            // 
            this._channelsToSkipListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._channelsToSkipListBox.DisplayMember = "ChannelName";
            this._channelsToSkipListBox.FormattingEnabled = true;
            this._channelsToSkipListBox.Location = new System.Drawing.Point(0, 0);
            this._channelsToSkipListBox.Name = "_channelsToSkipListBox";
            this._channelsToSkipListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this._channelsToSkipListBox.Size = new System.Drawing.Size(236, 277);
            this._channelsToSkipListBox.Sorted = true;
            this._channelsToSkipListBox.TabIndex = 107;
            this._channelsToSkipListBox.SelectedIndexChanged += new System.EventHandler(this._channelsToSkipListBox_SelectedIndexChanged);
            // 
            // _removeAllChannelButton
            // 
            this._removeAllChannelButton.Enabled = false;
            this._removeAllChannelButton.Location = new System.Drawing.Point(40, 104);
            this._removeAllChannelButton.Name = "_removeAllChannelButton";
            this._removeAllChannelButton.Size = new System.Drawing.Size(73, 23);
            this._removeAllChannelButton.TabIndex = 108;
            this._removeAllChannelButton.Text = "<<";
            this._removeAllChannelButton.UseVisualStyleBackColor = true;
            this._removeAllChannelButton.Click += new System.EventHandler(this._removeAllChannelButton_Click);
            // 
            // _mainTableLayoutPanel
            // 
            this._mainTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mainTableLayoutPanel.ColumnCount = 3;
            this._mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this._mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._mainTableLayoutPanel.Controls.Add(this._availablePluginPanel, 0, 0);
            this._mainTableLayoutPanel.Controls.Add(this._availableChannelsLabel, 0, 1);
            this._mainTableLayoutPanel.Controls.Add(this._channelsToSkipLabel, 2, 1);
            this._mainTableLayoutPanel.Controls.Add(this._buttonPanel, 1, 2);
            this._mainTableLayoutPanel.Controls.Add(this._availableChannelsPanel, 0, 2);
            this._mainTableLayoutPanel.Controls.Add(this._skipChannelsPanel, 2, 2);
            this._mainTableLayoutPanel.Location = new System.Drawing.Point(4, 3);
            this._mainTableLayoutPanel.Name = "_mainTableLayoutPanel";
            this._mainTableLayoutPanel.RowCount = 3;
            this._mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this._mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this._mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._mainTableLayoutPanel.Size = new System.Drawing.Size(633, 409);
            this._mainTableLayoutPanel.TabIndex = 1;
            // 
            // _availablePluginPanel
            // 
            this._mainTableLayoutPanel.SetColumnSpan(this._availablePluginPanel, 3);
            this._availablePluginPanel.Controls.Add(this._feedBackLabel);
            this._availablePluginPanel.Controls.Add(this._configureButton);
            this._availablePluginPanel.Controls.Add(this._importProgressBar);
            this._availablePluginPanel.Controls.Add(this._descriptionTextBox);
            this._availablePluginPanel.Controls.Add(this._descriptionLabel);
            this._availablePluginPanel.Controls.Add(this._pluginComboBox);
            this._availablePluginPanel.Controls.Add(this._importerLabel);
            this._availablePluginPanel.Controls.Add(this._importButton);
            this._availablePluginPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availablePluginPanel.Location = new System.Drawing.Point(3, 3);
            this._availablePluginPanel.Name = "_availablePluginPanel";
            this._availablePluginPanel.Size = new System.Drawing.Size(627, 64);
            this._availablePluginPanel.TabIndex = 0;
            // 
            // _feedBackLabel
            // 
            this._feedBackLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._feedBackLabel.AutoSize = true;
            this._feedBackLabel.Location = new System.Drawing.Point(87, 44);
            this._feedBackLabel.Name = "_feedBackLabel";
            this._feedBackLabel.Size = new System.Drawing.Size(0, 13);
            this._feedBackLabel.TabIndex = 110;
            // 
            // _configureButton
            // 
            this._configureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._configureButton.Location = new System.Drawing.Point(549, 26);
            this._configureButton.Name = "_configureButton";
            this._configureButton.Size = new System.Drawing.Size(75, 23);
            this._configureButton.TabIndex = 8;
            this._configureButton.Text = "Configure";
            this._configureButton.UseVisualStyleBackColor = true;
            this._configureButton.Click += new System.EventHandler(this._configureButton_Click);
            // 
            // _importProgressBar
            // 
            this._importProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._importProgressBar.Location = new System.Drawing.Point(88, 29);
            this._importProgressBar.Name = "_importProgressBar";
            this._importProgressBar.Size = new System.Drawing.Size(455, 10);
            this._importProgressBar.Step = 1;
            this._importProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._importProgressBar.TabIndex = 7;
            this._importProgressBar.Value = 1;
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._descriptionTextBox.Enabled = false;
            this._descriptionTextBox.Location = new System.Drawing.Point(88, 29);
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.Size = new System.Drawing.Size(455, 20);
            this._descriptionTextBox.TabIndex = 6;
            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.AutoSize = true;
            this._descriptionLabel.Location = new System.Drawing.Point(0, 33);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new System.Drawing.Size(63, 13);
            this._descriptionLabel.TabIndex = 5;
            this._descriptionLabel.Text = "Description:";
            // 
            // _buttonPanel
            // 
            this._buttonPanel.Controls.Add(this._addAllChannelButton);
            this._buttonPanel.Controls.Add(this._removeAllChannelButton);
            this._buttonPanel.Controls.Add(this._removeChannelButton);
            this._buttonPanel.Controls.Add(this._addChannelButton);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonPanel.Location = new System.Drawing.Point(244, 88);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(144, 318);
            this._buttonPanel.TabIndex = 108;
            // 
            // _addAllChannelButton
            // 
            this._addAllChannelButton.Enabled = false;
            this._addAllChannelButton.Location = new System.Drawing.Point(40, 75);
            this._addAllChannelButton.Name = "_addAllChannelButton";
            this._addAllChannelButton.Size = new System.Drawing.Size(73, 23);
            this._addAllChannelButton.TabIndex = 109;
            this._addAllChannelButton.Text = ">>";
            this._addAllChannelButton.UseVisualStyleBackColor = true;
            this._addAllChannelButton.Click += new System.EventHandler(this._addAllChannelButton_Click);
            // 
            // _availableChannelsPanel
            // 
            this._availableChannelsPanel.Controls.Add(this._refreshButton);
            this._availableChannelsPanel.Controls.Add(this._availableChannelsListBox);
            this._availableChannelsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableChannelsPanel.Location = new System.Drawing.Point(3, 88);
            this._availableChannelsPanel.Name = "_availableChannelsPanel";
            this._availableChannelsPanel.Size = new System.Drawing.Size(235, 318);
            this._availableChannelsPanel.TabIndex = 109;
            // 
            // _refreshButton
            // 
            this._refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._refreshButton.Enabled = false;
            this._refreshButton.Location = new System.Drawing.Point(0, 285);
            this._refreshButton.MinimumSize = new System.Drawing.Size(72, 23);
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(102, 23);
            this._refreshButton.TabIndex = 109;
            this._refreshButton.Text = "Refresh Channels";
            this._refreshButton.UseVisualStyleBackColor = true;
            this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
            // 
            // _skipChannelsPanel
            // 
            this._skipChannelsPanel.Controls.Add(this._saveButton);
            this._skipChannelsPanel.Controls.Add(this._channelsToSkipListBox);
            this._skipChannelsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._skipChannelsPanel.Location = new System.Drawing.Point(394, 88);
            this._skipChannelsPanel.Name = "_skipChannelsPanel";
            this._skipChannelsPanel.Size = new System.Drawing.Size(236, 318);
            this._skipChannelsPanel.TabIndex = 110;
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._saveButton.Enabled = false;
            this._saveButton.Location = new System.Drawing.Point(0, 285);
            this._saveButton.MinimumSize = new System.Drawing.Size(72, 23);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(102, 23);
            this._saveButton.TabIndex = 110;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 417);
            this.Controls.Add(this._mainTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(658, 453);
            this.Name = "MainForm";
            this.Text = "ARGUS TV - Guide Importer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._mainTableLayoutPanel.ResumeLayout(false);
            this._mainTableLayoutPanel.PerformLayout();
            this._availablePluginPanel.ResumeLayout(false);
            this._availablePluginPanel.PerformLayout();
            this._buttonPanel.ResumeLayout(false);
            this._availableChannelsPanel.ResumeLayout(false);
            this._skipChannelsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _pluginComboBox;
        private System.Windows.Forms.Label _importerLabel;
        private System.Windows.Forms.Label _availableChannelsLabel;
        private System.Windows.Forms.Button _importButton;
        private System.Windows.Forms.ListBox _availableChannelsListBox;
        private System.Windows.Forms.Label _channelsToSkipLabel;
        private System.Windows.Forms.Button _addChannelButton;
        private System.Windows.Forms.Button _removeChannelButton;
        private System.Windows.Forms.ListBox _channelsToSkipListBox;
        private System.Windows.Forms.Button _removeAllChannelButton;
        private System.Windows.Forms.TableLayoutPanel _mainTableLayoutPanel;
        private System.Windows.Forms.Panel _availablePluginPanel;
        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.Panel _availableChannelsPanel;
        private System.Windows.Forms.Panel _skipChannelsPanel;
        private System.Windows.Forms.Button _refreshButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Label _descriptionLabel;
        private System.Windows.Forms.TextBox _descriptionTextBox;
        private System.Windows.Forms.ProgressBar _importProgressBar;
        private System.Windows.Forms.Button _addAllChannelButton;
        private System.Windows.Forms.Button _configureButton;
        private System.Windows.Forms.Label _feedBackLabel;
    }
}

