namespace ArgusTV.GuideImporter.SchedulesDirect
{
    partial class ConfigurationForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this._generalGroupBox = new System.Windows.Forms.GroupBox();
            this._pluginNameTextBox = new System.Windows.Forms.TextBox();
            this._pluginNameLabel = new System.Windows.Forms.Label();
            this._sdPasswordLabel = new System.Windows.Forms.Label();
            this._sdUserNameLabel = new System.Windows.Forms.Label();
            this._sdPasswordTextBox = new System.Windows.Forms.TextBox();
            this._sdUserNameTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._cancelButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this._proxyGroupBox = new System.Windows.Forms.GroupBox();
            this._proxyPasswordLabel = new System.Windows.Forms.Label();
            this._proxyUserNameTextBox = new System.Windows.Forms.TextBox();
            this._proxyUserNameLabel = new System.Windows.Forms.Label();
            this._proxyPasswordTextBox = new System.Windows.Forms.TextBox();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._updatesAllDays = new System.Windows.Forms.RadioButton();
            this._updatesNextDayOnly = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this._channelNameFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._nrDaysNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._nrDaysLabel = new System.Windows.Forms.Label();
            this._warningLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._updateChannelNamesCheckBox = new System.Windows.Forms.CheckBox();
            this._generalGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this._proxyGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nrDaysNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // _generalGroupBox
            // 
            this._generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._generalGroupBox.Controls.Add(this._pluginNameTextBox);
            this._generalGroupBox.Controls.Add(this._pluginNameLabel);
            this._generalGroupBox.Controls.Add(this._sdPasswordLabel);
            this._generalGroupBox.Controls.Add(this._sdUserNameLabel);
            this._generalGroupBox.Controls.Add(this._sdPasswordTextBox);
            this._generalGroupBox.Controls.Add(this._sdUserNameTextBox);
            this._generalGroupBox.Controls.Add(this.pictureBox1);
            this._generalGroupBox.Location = new System.Drawing.Point(11, 8);
            this._generalGroupBox.Name = "_generalGroupBox";
            this._generalGroupBox.Size = new System.Drawing.Size(540, 78);
            this._generalGroupBox.TabIndex = 15;
            this._generalGroupBox.TabStop = false;
            this._generalGroupBox.Text = "General";
            // 
            // _pluginNameTextBox
            // 
            this._pluginNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pluginNameTextBox.Location = new System.Drawing.Point(102, 19);
            this._pluginNameTextBox.Name = "_pluginNameTextBox";
            this._pluginNameTextBox.Size = new System.Drawing.Size(340, 20);
            this._pluginNameTextBox.TabIndex = 1;
            this._pluginNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._pluginNameTextBox_Validating);
            // 
            // _pluginNameLabel
            // 
            this._pluginNameLabel.AutoSize = true;
            this._pluginNameLabel.Location = new System.Drawing.Point(16, 22);
            this._pluginNameLabel.Name = "_pluginNameLabel";
            this._pluginNameLabel.Size = new System.Drawing.Size(71, 13);
            this._pluginNameLabel.TabIndex = 0;
            this._pluginNameLabel.Text = "Plugin name :";
            // 
            // _sdPasswordLabel
            // 
            this._sdPasswordLabel.AutoSize = true;
            this._sdPasswordLabel.Location = new System.Drawing.Point(242, 50);
            this._sdPasswordLabel.Name = "_sdPasswordLabel";
            this._sdPasswordLabel.Size = new System.Drawing.Size(59, 13);
            this._sdPasswordLabel.TabIndex = 4;
            this._sdPasswordLabel.Text = "Password :";
            // 
            // _sdUserNameLabel
            // 
            this._sdUserNameLabel.AutoSize = true;
            this._sdUserNameLabel.Location = new System.Drawing.Point(16, 50);
            this._sdUserNameLabel.Name = "_sdUserNameLabel";
            this._sdUserNameLabel.Size = new System.Drawing.Size(61, 13);
            this._sdUserNameLabel.TabIndex = 5;
            this._sdUserNameLabel.Text = "Username :";
            // 
            // _sdPasswordTextBox
            // 
            this._sdPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._sdPasswordTextBox.Location = new System.Drawing.Point(324, 47);
            this._sdPasswordTextBox.Name = "_sdPasswordTextBox";
            this._sdPasswordTextBox.Size = new System.Drawing.Size(118, 20);
            this._sdPasswordTextBox.TabIndex = 4;
            this._sdPasswordTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._sdPasswordTextBox_Validating);
            // 
            // _sdUserNameTextBox
            // 
            this._sdUserNameTextBox.AcceptsReturn = true;
            this._sdUserNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._sdUserNameTextBox.Location = new System.Drawing.Point(102, 47);
            this._sdUserNameTextBox.Name = "_sdUserNameTextBox";
            this._sdUserNameTextBox.Size = new System.Drawing.Size(118, 20);
            this._sdUserNameTextBox.TabIndex = 2;
            this._sdUserNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._sdUserNameTextBox_Validating);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::ArgusTV.GuideImporter.SchedulesDirect.Properties.Resources.SchedulesDirect;
            this.pictureBox1.Location = new System.Drawing.Point(462, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 43);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.CausesValidation = false;
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(476, 259);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 12;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._saveButton.Location = new System.Drawing.Point(395, 259);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 11;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _proxyGroupBox
            // 
            this._proxyGroupBox.Controls.Add(this._proxyPasswordLabel);
            this._proxyGroupBox.Controls.Add(this._proxyUserNameTextBox);
            this._proxyGroupBox.Controls.Add(this._proxyUserNameLabel);
            this._proxyGroupBox.Controls.Add(this._proxyPasswordTextBox);
            this._proxyGroupBox.Location = new System.Drawing.Point(11, 306);
            this._proxyGroupBox.Name = "_proxyGroupBox";
            this._proxyGroupBox.Size = new System.Drawing.Size(539, 60);
            this._proxyGroupBox.TabIndex = 16;
            this._proxyGroupBox.TabStop = false;
            this._proxyGroupBox.Text = "Proxy";
            // 
            // _proxyPasswordLabel
            // 
            this._proxyPasswordLabel.AutoSize = true;
            this._proxyPasswordLabel.Location = new System.Drawing.Point(242, 31);
            this._proxyPasswordLabel.Name = "_proxyPasswordLabel";
            this._proxyPasswordLabel.Size = new System.Drawing.Size(59, 13);
            this._proxyPasswordLabel.TabIndex = 13;
            this._proxyPasswordLabel.Text = "Password :";
            // 
            // _proxyUserNameTextBox
            // 
            this._proxyUserNameTextBox.AcceptsReturn = true;
            this._proxyUserNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._proxyUserNameTextBox.Location = new System.Drawing.Point(102, 28);
            this._proxyUserNameTextBox.Name = "_proxyUserNameTextBox";
            this._proxyUserNameTextBox.Size = new System.Drawing.Size(118, 20);
            this._proxyUserNameTextBox.TabIndex = 11;
            // 
            // _proxyUserNameLabel
            // 
            this._proxyUserNameLabel.AutoSize = true;
            this._proxyUserNameLabel.Location = new System.Drawing.Point(16, 31);
            this._proxyUserNameLabel.Name = "_proxyUserNameLabel";
            this._proxyUserNameLabel.Size = new System.Drawing.Size(61, 13);
            this._proxyUserNameLabel.TabIndex = 14;
            this._proxyUserNameLabel.Text = "Username :";
            // 
            // _proxyPasswordTextBox
            // 
            this._proxyPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._proxyPasswordTextBox.Location = new System.Drawing.Point(324, 28);
            this._proxyPasswordTextBox.Name = "_proxyPasswordTextBox";
            this._proxyPasswordTextBox.Size = new System.Drawing.Size(118, 20);
            this._proxyPasswordTextBox.TabIndex = 12;
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._updateChannelNamesCheckBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this._updatesAllDays);
            this.groupBox1.Controls.Add(this._updatesNextDayOnly);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._channelNameFormat);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._nrDaysNumericUpDown);
            this.groupBox1.Controls.Add(this._nrDaysLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 139);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // _updatesAllDays
            // 
            this._updatesAllDays.AutoSize = true;
            this._updatesAllDays.Location = new System.Drawing.Point(171, 104);
            this._updatesAllDays.Name = "_updatesAllDays";
            this._updatesAllDays.Size = new System.Drawing.Size(61, 17);
            this._updatesAllDays.TabIndex = 19;
            this._updatesAllDays.TabStop = true;
            this._updatesAllDays.Text = "All days";
            this._updatesAllDays.UseVisualStyleBackColor = true;
            // 
            // _updatesNextDayOnly
            // 
            this._updatesNextDayOnly.AutoSize = true;
            this._updatesNextDayOnly.Location = new System.Drawing.Point(238, 104);
            this._updatesNextDayOnly.Name = "_updatesNextDayOnly";
            this._updatesNextDayOnly.Size = new System.Drawing.Size(89, 17);
            this._updatesNextDayOnly.TabIndex = 18;
            this._updatesNextDayOnly.TabStop = true;
            this._updatesNextDayOnly.Text = "Next day only";
            this._updatesNextDayOnly.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Check for guidedata updates :";
            // 
            // _channelNameFormat
            // 
            this._channelNameFormat.FormattingEnabled = true;
            this._channelNameFormat.Location = new System.Drawing.Point(171, 22);
            this._channelNameFormat.Name = "_channelNameFormat";
            this._channelNameFormat.Size = new System.Drawing.Size(285, 21);
            this._channelNameFormat.TabIndex = 16;
            this._channelNameFormat.SelectedIndexChanged += new System.EventHandler(this._channelNameFormat_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "ChannelName format :";
            // 
            // _nrDaysNumericUpDown
            // 
            this._nrDaysNumericUpDown.Location = new System.Drawing.Point(171, 76);
            this._nrDaysNumericUpDown.Maximum = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this._nrDaysNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nrDaysNumericUpDown.Name = "_nrDaysNumericUpDown";
            this._nrDaysNumericUpDown.Size = new System.Drawing.Size(49, 20);
            this._nrDaysNumericUpDown.TabIndex = 14;
            this._nrDaysNumericUpDown.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // _nrDaysLabel
            // 
            this._nrDaysLabel.AutoSize = true;
            this._nrDaysLabel.Location = new System.Drawing.Point(15, 78);
            this._nrDaysLabel.Name = "_nrDaysLabel";
            this._nrDaysLabel.Size = new System.Drawing.Size(130, 13);
            this._nrDaysLabel.TabIndex = 13;
            this._nrDaysLabel.Text = "Number of days to import :";
            // 
            // _warningLabel
            // 
            this._warningLabel.AutoSize = true;
            this._warningLabel.ForeColor = System.Drawing.Color.Red;
            this._warningLabel.Location = new System.Drawing.Point(27, 233);
            this._warningLabel.Name = "_warningLabel";
            this._warningLabel.Size = new System.Drawing.Size(491, 13);
            this._warningLabel.TabIndex = 17;
            this._warningLabel.Text = "Once this config is saved, click the refresh channels button to reflect the chann" +
                "elName format change.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(212, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Update channelnames in ARGUS TV:";
            // 
            // _updateChannelNamesCheckBox
            // 
            this._updateChannelNamesCheckBox.AutoSize = true;
            this._updateChannelNamesCheckBox.Location = new System.Drawing.Point(234, 52);
            this._updateChannelNamesCheckBox.Name = "_updateChannelNamesCheckBox";
            this._updateChannelNamesCheckBox.Size = new System.Drawing.Size(15, 14);
            this._updateChannelNamesCheckBox.TabIndex = 21;
            this._updateChannelNamesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AcceptButton = this._saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(562, 294);
            this.Controls.Add(this._warningLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._proxyGroupBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._saveButton);
            this.Controls.Add(this._generalGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SchedulesDirect - Configuration";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            this._generalGroupBox.ResumeLayout(false);
            this._generalGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this._proxyGroupBox.ResumeLayout(false);
            this._proxyGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nrDaysNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _generalGroupBox;
        private System.Windows.Forms.TextBox _pluginNameTextBox;
        private System.Windows.Forms.Label _pluginNameLabel;
        private System.Windows.Forms.Label _sdPasswordLabel;
        private System.Windows.Forms.Label _sdUserNameLabel;
        private System.Windows.Forms.TextBox _sdPasswordTextBox;
        private System.Windows.Forms.TextBox _sdUserNameTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.GroupBox _proxyGroupBox;
        private System.Windows.Forms.Label _proxyPasswordLabel;
        private System.Windows.Forms.TextBox _proxyUserNameTextBox;
        private System.Windows.Forms.Label _proxyUserNameLabel;
        private System.Windows.Forms.TextBox _proxyPasswordTextBox;
        private System.Windows.Forms.ErrorProvider _errorProvider;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label _nrDaysLabel;
        private System.Windows.Forms.NumericUpDown _nrDaysNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _channelNameFormat;
        private System.Windows.Forms.Label _warningLabel;
        private System.Windows.Forms.RadioButton _updatesAllDays;
        private System.Windows.Forms.RadioButton _updatesNextDayOnly;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox _updateChannelNamesCheckBox;
        private System.Windows.Forms.Label label3;
    }
}