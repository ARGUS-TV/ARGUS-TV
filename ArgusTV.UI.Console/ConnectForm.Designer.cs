namespace ArgusTV.UI.Console
{
    partial class ConnectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectForm));
            this._ServerLabel = new System.Windows.Forms.Label();
            this._serverTextBox = new System.Windows.Forms.TextBox();
            this._transportLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._servicesGroupBox = new System.Windows.Forms.GroupBox();
            this._transportComboBox = new System.Windows.Forms.ComboBox();
            this._generalGroupBox = new System.Windows.Forms.GroupBox();
            this._saveAsProfileCheckBox = new System.Windows.Forms.CheckBox();
            this._profileNameTextBox = new System.Windows.Forms.TextBox();
            this._secondsLabel = new System.Windows.Forms.Label();
            this._profileNameLabel = new System.Windows.Forms.Label();
            this._wolSecondsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._allowLabel = new System.Windows.Forms.Label();
            this._useWolCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._servicesGroupBox.SuspendLayout();
            this._generalGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._wolSecondsNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // _ServerLabel
            // 
            this._ServerLabel.AutoSize = true;
            this._ServerLabel.Location = new System.Drawing.Point(8, 22);
            this._ServerLabel.Name = "_ServerLabel";
            this._ServerLabel.Size = new System.Drawing.Size(41, 13);
            this._ServerLabel.TabIndex = 0;
            this._ServerLabel.Text = "Server:";
            // 
            // _serverTextBox
            // 
            this._serverTextBox.Location = new System.Drawing.Point(69, 19);
            this._serverTextBox.Name = "_serverTextBox";
            this._serverTextBox.Size = new System.Drawing.Size(213, 20);
            this._serverTextBox.TabIndex = 1;
            this._serverTextBox.TextChanged += new System.EventHandler(this._serverTextBox_TextChanged);
            // 
            // _transportLabel
            // 
            this._transportLabel.AutoSize = true;
            this._transportLabel.Location = new System.Drawing.Point(8, 48);
            this._transportLabel.Name = "_transportLabel";
            this._transportLabel.Size = new System.Drawing.Size(55, 13);
            this._transportLabel.TabIndex = 2;
            this._transportLabel.Text = "Transport:";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(148, 219);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 100;
            this._okButton.Text = "Connect";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(229, 219);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 101;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _portNumericUpDown
            // 
            this._portNumericUpDown.Location = new System.Drawing.Point(190, 45);
            this._portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new System.Drawing.Size(92, 20);
            this._portNumericUpDown.TabIndex = 4;
            // 
            // _servicesGroupBox
            // 
            this._servicesGroupBox.Controls.Add(this._transportComboBox);
            this._servicesGroupBox.Controls.Add(this._serverTextBox);
            this._servicesGroupBox.Controls.Add(this._ServerLabel);
            this._servicesGroupBox.Controls.Add(this._portNumericUpDown);
            this._servicesGroupBox.Controls.Add(this._transportLabel);
            this._servicesGroupBox.Location = new System.Drawing.Point(12, 12);
            this._servicesGroupBox.Name = "_servicesGroupBox";
            this._servicesGroupBox.Size = new System.Drawing.Size(292, 76);
            this._servicesGroupBox.TabIndex = 1;
            this._servicesGroupBox.TabStop = false;
            this._servicesGroupBox.Text = "ARGUS TV Scheduler service";
            // 
            // _transportComboBox
            // 
            this._transportComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._transportComboBox.FormattingEnabled = true;
            this._transportComboBox.Items.AddRange(new object[] {
            "http",
            "https"});
            this._transportComboBox.Location = new System.Drawing.Point(69, 45);
            this._transportComboBox.Name = "_transportComboBox";
            this._transportComboBox.Size = new System.Drawing.Size(115, 21);
            this._transportComboBox.TabIndex = 3;
            this._transportComboBox.SelectedIndexChanged += new System.EventHandler(this._transportComboBox_SelectedIndexChanged);
            // 
            // _generalGroupBox
            // 
            this._generalGroupBox.Controls.Add(this._saveAsProfileCheckBox);
            this._generalGroupBox.Controls.Add(this._profileNameTextBox);
            this._generalGroupBox.Controls.Add(this._secondsLabel);
            this._generalGroupBox.Controls.Add(this._profileNameLabel);
            this._generalGroupBox.Controls.Add(this._wolSecondsNumericUpDown);
            this._generalGroupBox.Controls.Add(this._allowLabel);
            this._generalGroupBox.Controls.Add(this._useWolCheckBox);
            this._generalGroupBox.Location = new System.Drawing.Point(12, 94);
            this._generalGroupBox.Name = "_generalGroupBox";
            this._generalGroupBox.Size = new System.Drawing.Size(292, 114);
            this._generalGroupBox.TabIndex = 2;
            this._generalGroupBox.TabStop = false;
            this._generalGroupBox.Text = "Options";
            // 
            // _saveAsProfileCheckBox
            // 
            this._saveAsProfileCheckBox.AutoSize = true;
            this._saveAsProfileCheckBox.Location = new System.Drawing.Point(11, 65);
            this._saveAsProfileCheckBox.Name = "_saveAsProfileCheckBox";
            this._saveAsProfileCheckBox.Size = new System.Drawing.Size(213, 17);
            this._saveAsProfileCheckBox.TabIndex = 5;
            this._saveAsProfileCheckBox.Text = "Remember as named connection profile";
            this._saveAsProfileCheckBox.UseVisualStyleBackColor = true;
            this._saveAsProfileCheckBox.CheckedChanged += new System.EventHandler(this._saveAsProfileCheckBox_CheckedChanged);
            // 
            // _profileNameTextBox
            // 
            this._profileNameTextBox.Location = new System.Drawing.Point(72, 85);
            this._profileNameTextBox.Name = "_profileNameTextBox";
            this._profileNameTextBox.Size = new System.Drawing.Size(210, 20);
            this._profileNameTextBox.TabIndex = 7;
            // 
            // _secondsLabel
            // 
            this._secondsLabel.AutoSize = true;
            this._secondsLabel.Location = new System.Drawing.Point(115, 41);
            this._secondsLabel.Name = "_secondsLabel";
            this._secondsLabel.Size = new System.Drawing.Size(150, 13);
            this._secondsLabel.TabIndex = 4;
            this._secondsLabel.Text = "seconds for server to wake up";
            // 
            // _profileNameLabel
            // 
            this._profileNameLabel.AutoSize = true;
            this._profileNameLabel.Location = new System.Drawing.Point(28, 88);
            this._profileNameLabel.Name = "_profileNameLabel";
            this._profileNameLabel.Size = new System.Drawing.Size(38, 13);
            this._profileNameLabel.TabIndex = 6;
            this._profileNameLabel.Text = "Name:";
            // 
            // _wolSecondsNumericUpDown
            // 
            this._wolSecondsNumericUpDown.Location = new System.Drawing.Point(62, 39);
            this._wolSecondsNumericUpDown.Name = "_wolSecondsNumericUpDown";
            this._wolSecondsNumericUpDown.Size = new System.Drawing.Size(50, 20);
            this._wolSecondsNumericUpDown.TabIndex = 3;
            this._wolSecondsNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // _allowLabel
            // 
            this._allowLabel.AutoSize = true;
            this._allowLabel.Location = new System.Drawing.Point(28, 41);
            this._allowLabel.Name = "_allowLabel";
            this._allowLabel.Size = new System.Drawing.Size(32, 13);
            this._allowLabel.TabIndex = 2;
            this._allowLabel.Text = "Allow";
            // 
            // _useWolCheckBox
            // 
            this._useWolCheckBox.AutoSize = true;
            this._useWolCheckBox.Location = new System.Drawing.Point(11, 19);
            this._useWolCheckBox.Name = "_useWolCheckBox";
            this._useWolCheckBox.Size = new System.Drawing.Size(241, 17);
            this._useWolCheckBox.TabIndex = 1;
            this._useWolCheckBox.Text = "Use Wake-On-LAN if server can\'t be reached";
            this._useWolCheckBox.UseVisualStyleBackColor = true;
            this._useWolCheckBox.CheckedChanged += new System.EventHandler(this._useWolCheckBox_CheckedChanged);
            // 
            // ConnectForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(316, 252);
            this.Controls.Add(this._generalGroupBox);
            this.Controls.Add(this._servicesGroupBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect to ARGUS TV";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ConnectForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._servicesGroupBox.ResumeLayout(false);
            this._servicesGroupBox.PerformLayout();
            this._generalGroupBox.ResumeLayout(false);
            this._generalGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._wolSecondsNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _ServerLabel;
        private System.Windows.Forms.TextBox _serverTextBox;
        private System.Windows.Forms.Label _transportLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.NumericUpDown _portNumericUpDown;
        private System.Windows.Forms.GroupBox _servicesGroupBox;
        private System.Windows.Forms.GroupBox _generalGroupBox;
        private System.Windows.Forms.Label _secondsLabel;
        private System.Windows.Forms.NumericUpDown _wolSecondsNumericUpDown;
        private System.Windows.Forms.Label _allowLabel;
        private System.Windows.Forms.CheckBox _useWolCheckBox;
        private System.Windows.Forms.ComboBox _transportComboBox;
        private System.Windows.Forms.TextBox _profileNameTextBox;
        private System.Windows.Forms.Label _profileNameLabel;
        private System.Windows.Forms.CheckBox _saveAsProfileCheckBox;
    }
}