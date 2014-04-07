namespace ArgusTV.UI.MediaPortal
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this._ServerLabel = new System.Windows.Forms.Label();
            this._serverTextBox = new System.Windows.Forms.TextBox();
            this._transportLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._testButton = new System.Windows.Forms.Button();
            this._servicesGroupBox = new System.Windows.Forms.GroupBox();
            this._generalGroupBox = new System.Windows.Forms.GroupBox();
            this._disableRadioCheckBox = new System.Windows.Forms.CheckBox();
            this._standbyInfoLabel = new System.Windows.Forms.Label();
            this._noStandbyWhenNotHomeCheckBox = new System.Windows.Forms.CheckBox();
            this._secondsLabel = new System.Windows.Forms.Label();
            this._wolSecondsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._allowLabel = new System.Windows.Forms.Label();
            this._useWolCheckBox = new System.Windows.Forms.CheckBox();
            this._playRecordingsOverRtspCheckBox = new System.Windows.Forms.CheckBox();
            this._streamingModeCheckBox = new System.Windows.Forms.CheckBox();
            this._preferRtspForLiveTVCheckBox = new System.Windows.Forms.CheckBox();
            this._streamingModeGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._servicesGroupBox.SuspendLayout();
            this._generalGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._wolSecondsNumericUpDown)).BeginInit();
            this._streamingModeGroupBox.SuspendLayout();
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
            this._serverTextBox.Size = new System.Drawing.Size(196, 20);
            this._serverTextBox.TabIndex = 1;
            // 
            // _transportLabel
            // 
            this._transportLabel.AutoSize = true;
            this._transportLabel.Location = new System.Drawing.Point(8, 48);
            this._transportLabel.Name = "_transportLabel";
            this._transportLabel.Size = new System.Drawing.Size(49, 13);
            this._transportLabel.TabIndex = 3;
            this._transportLabel.Text = "http port:";
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(134, 358);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 90;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(215, 358);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 91;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _portNumericUpDown
            // 
            this._portNumericUpDown.Location = new System.Drawing.Point(69, 45);
            this._portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new System.Drawing.Size(81, 20);
            this._portNumericUpDown.TabIndex = 9;
            // 
            // _testButton
            // 
            this._testButton.Location = new System.Drawing.Point(164, 43);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new System.Drawing.Size(101, 23);
            this._testButton.TabIndex = 10;
            this._testButton.Text = "Test Connection";
            this._testButton.UseVisualStyleBackColor = true;
            this._testButton.Click += new System.EventHandler(this._testButton_Click);
            // 
            // _servicesGroupBox
            // 
            this._servicesGroupBox.Controls.Add(this._ServerLabel);
            this._servicesGroupBox.Controls.Add(this._testButton);
            this._servicesGroupBox.Controls.Add(this._serverTextBox);
            this._servicesGroupBox.Controls.Add(this._portNumericUpDown);
            this._servicesGroupBox.Controls.Add(this._transportLabel);
            this._servicesGroupBox.Location = new System.Drawing.Point(12, 12);
            this._servicesGroupBox.Name = "_servicesGroupBox";
            this._servicesGroupBox.Size = new System.Drawing.Size(278, 76);
            this._servicesGroupBox.TabIndex = 0;
            this._servicesGroupBox.TabStop = false;
            this._servicesGroupBox.Text = "ARGUS TV Scheduler service";
            // 
            // _generalGroupBox
            // 
            this._generalGroupBox.Controls.Add(this._disableRadioCheckBox);
            this._generalGroupBox.Controls.Add(this._standbyInfoLabel);
            this._generalGroupBox.Controls.Add(this._noStandbyWhenNotHomeCheckBox);
            this._generalGroupBox.Controls.Add(this._secondsLabel);
            this._generalGroupBox.Controls.Add(this._wolSecondsNumericUpDown);
            this._generalGroupBox.Controls.Add(this._allowLabel);
            this._generalGroupBox.Controls.Add(this._useWolCheckBox);
            this._generalGroupBox.Location = new System.Drawing.Point(12, 94);
            this._generalGroupBox.Name = "_generalGroupBox";
            this._generalGroupBox.Size = new System.Drawing.Size(278, 163);
            this._generalGroupBox.TabIndex = 10;
            this._generalGroupBox.TabStop = false;
            this._generalGroupBox.Text = "Options";
            // 
            // _disableRadioCheckBox
            // 
            this._disableRadioCheckBox.AutoSize = true;
            this._disableRadioCheckBox.Location = new System.Drawing.Point(12, 135);
            this._disableRadioCheckBox.Name = "_disableRadioCheckBox";
            this._disableRadioCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._disableRadioCheckBox.Size = new System.Drawing.Size(118, 17);
            this._disableRadioCheckBox.TabIndex = 8;
            this._disableRadioCheckBox.Text = "Disable radio plugin";
            this._disableRadioCheckBox.UseVisualStyleBackColor = true;
            // 
            // _standbyInfoLabel
            // 
            this._standbyInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._standbyInfoLabel.ForeColor = System.Drawing.Color.DimGray;
            this._standbyInfoLabel.Location = new System.Drawing.Point(7, 84);
            this._standbyInfoLabel.Name = "_standbyInfoLabel";
            this._standbyInfoLabel.Size = new System.Drawing.Size(265, 44);
            this._standbyInfoLabel.TabIndex = 7;
            this._standbyInfoLabel.Text = "If checked, the server and client will be kept alive when you\'re not on the home " +
    "screen.                        (no need for the powerscheduler plugin)";
            // 
            // _noStandbyWhenNotHomeCheckBox
            // 
            this._noStandbyWhenNotHomeCheckBox.AutoSize = true;
            this._noStandbyWhenNotHomeCheckBox.Location = new System.Drawing.Point(11, 66);
            this._noStandbyWhenNotHomeCheckBox.Name = "_noStandbyWhenNotHomeCheckBox";
            this._noStandbyWhenNotHomeCheckBox.Size = new System.Drawing.Size(260, 17);
            this._noStandbyWhenNotHomeCheckBox.TabIndex = 6;
            this._noStandbyWhenNotHomeCheckBox.Text = "Only allow client to enter standby on home screen";
            this._noStandbyWhenNotHomeCheckBox.UseVisualStyleBackColor = true;
            // 
            // _secondsLabel
            // 
            this._secondsLabel.AutoSize = true;
            this._secondsLabel.Location = new System.Drawing.Point(115, 41);
            this._secondsLabel.Name = "_secondsLabel";
            this._secondsLabel.Size = new System.Drawing.Size(150, 13);
            this._secondsLabel.TabIndex = 5;
            this._secondsLabel.Text = "seconds for server to wake up";
            // 
            // _wolSecondsNumericUpDown
            // 
            this._wolSecondsNumericUpDown.Location = new System.Drawing.Point(62, 39);
            this._wolSecondsNumericUpDown.Name = "_wolSecondsNumericUpDown";
            this._wolSecondsNumericUpDown.Size = new System.Drawing.Size(50, 20);
            this._wolSecondsNumericUpDown.TabIndex = 4;
            this._wolSecondsNumericUpDown.Value = new decimal(new int[] {
            10,
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
            this._allowLabel.TabIndex = 3;
            this._allowLabel.Text = "Allow";
            // 
            // _useWolCheckBox
            // 
            this._useWolCheckBox.AutoSize = true;
            this._useWolCheckBox.Location = new System.Drawing.Point(11, 19);
            this._useWolCheckBox.Name = "_useWolCheckBox";
            this._useWolCheckBox.Size = new System.Drawing.Size(241, 17);
            this._useWolCheckBox.TabIndex = 2;
            this._useWolCheckBox.Text = "Use Wake-On-LAN if server can\'t be reached";
            this._useWolCheckBox.UseVisualStyleBackColor = true;
            this._useWolCheckBox.CheckedChanged += new System.EventHandler(this._useWolCheckBox_CheckedChanged);
            // 
            // _playRecordingsOverRtspCheckBox
            // 
            this._playRecordingsOverRtspCheckBox.AutoSize = true;
            this._playRecordingsOverRtspCheckBox.Location = new System.Drawing.Point(30, 65);
            this._playRecordingsOverRtspCheckBox.Name = "_playRecordingsOverRtspCheckBox";
            this._playRecordingsOverRtspCheckBox.Size = new System.Drawing.Size(201, 17);
            this._playRecordingsOverRtspCheckBox.TabIndex = 9;
            this._playRecordingsOverRtspCheckBox.Text = "Prefer RTSP streaming for recordings";
            this._playRecordingsOverRtspCheckBox.UseVisualStyleBackColor = true;
            // 
            // _streamingModeCheckBox
            // 
            this._streamingModeCheckBox.AutoSize = true;
            this._streamingModeCheckBox.Location = new System.Drawing.Point(10, 19);
            this._streamingModeCheckBox.Name = "_streamingModeCheckBox";
            this._streamingModeCheckBox.Size = new System.Drawing.Size(155, 17);
            this._streamingModeCheckBox.TabIndex = 11;
            this._streamingModeCheckBox.Text = "Auto select (recommended)";
            this._streamingModeCheckBox.UseVisualStyleBackColor = true;
            this._streamingModeCheckBox.CheckedChanged += new System.EventHandler(this._streamingModeCheckBox_CheckedChanged);
            // 
            // _preferRtspForLiveTVCheckBox
            // 
            this._preferRtspForLiveTVCheckBox.AutoSize = true;
            this._preferRtspForLiveTVCheckBox.Location = new System.Drawing.Point(30, 42);
            this._preferRtspForLiveTVCheckBox.Name = "_preferRtspForLiveTVCheckBox";
            this._preferRtspForLiveTVCheckBox.Size = new System.Drawing.Size(185, 17);
            this._preferRtspForLiveTVCheckBox.TabIndex = 8;
            this._preferRtspForLiveTVCheckBox.Text = "Prefer RTSP streaming for live TV";
            this._preferRtspForLiveTVCheckBox.UseVisualStyleBackColor = true;
            // 
            // _streamingModeGroupBox
            // 
            this._streamingModeGroupBox.Controls.Add(this._playRecordingsOverRtspCheckBox);
            this._streamingModeGroupBox.Controls.Add(this._streamingModeCheckBox);
            this._streamingModeGroupBox.Controls.Add(this._preferRtspForLiveTVCheckBox);
            this._streamingModeGroupBox.Location = new System.Drawing.Point(13, 263);
            this._streamingModeGroupBox.Name = "_streamingModeGroupBox";
            this._streamingModeGroupBox.Size = new System.Drawing.Size(277, 89);
            this._streamingModeGroupBox.TabIndex = 12;
            this._streamingModeGroupBox.TabStop = false;
            this._streamingModeGroupBox.Text = "Streaming mode";
            // 
            // SetupForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(302, 391);
            this.Controls.Add(this._streamingModeGroupBox);
            this.Controls.Add(this._generalGroupBox);
            this.Controls.Add(this._servicesGroupBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ARGUS TV";
            this.Load += new System.EventHandler(this.ConnectForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._servicesGroupBox.ResumeLayout(false);
            this._servicesGroupBox.PerformLayout();
            this._generalGroupBox.ResumeLayout(false);
            this._generalGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._wolSecondsNumericUpDown)).EndInit();
            this._streamingModeGroupBox.ResumeLayout(false);
            this._streamingModeGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _ServerLabel;
        private System.Windows.Forms.TextBox _serverTextBox;
        private System.Windows.Forms.Label _transportLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.NumericUpDown _portNumericUpDown;
        private System.Windows.Forms.Button _testButton;
        private System.Windows.Forms.GroupBox _servicesGroupBox;
        private System.Windows.Forms.GroupBox _generalGroupBox;
        private System.Windows.Forms.CheckBox _useWolCheckBox;
        private System.Windows.Forms.CheckBox _noStandbyWhenNotHomeCheckBox;
        private System.Windows.Forms.Label _secondsLabel;
        private System.Windows.Forms.NumericUpDown _wolSecondsNumericUpDown;
        private System.Windows.Forms.Label _allowLabel;
        private System.Windows.Forms.Label _standbyInfoLabel;
        private System.Windows.Forms.CheckBox _preferRtspForLiveTVCheckBox;
        private System.Windows.Forms.CheckBox _playRecordingsOverRtspCheckBox;
        private System.Windows.Forms.CheckBox _streamingModeCheckBox;
        private System.Windows.Forms.GroupBox _streamingModeGroupBox;
        private System.Windows.Forms.CheckBox _disableRadioCheckBox;
    }
}
