namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels
{
    partial class FinishPage
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
            this._finishPanel = new System.Windows.Forms.Panel();
            this._exportingFileLabel = new System.Windows.Forms.Label();
            this._exportProgressBar = new System.Windows.Forms.ProgressBar();
            this._destinationLabel = new System.Windows.Forms.Label();
            this._exportLabel = new System.Windows.Forms.Label();
            this._exportHeaderLabel = new System.Windows.Forms.Label();
            this._finishLabel = new System.Windows.Forms.Label();
            this._destinationPanel = new System.Windows.Forms.Panel();
            this._importChannelsOnlyRadioButton = new System.Windows.Forms.RadioButton();
            this._importInNamedGroupRadioButton = new System.Windows.Forms.RadioButton();
            this._importChannelsAndGroupsRadioButton = new System.Windows.Forms.RadioButton();
            this._groupNameTextBox = new System.Windows.Forms.TextBox();
            this._noWorkPanel = new System.Windows.Forms.Panel();
            this._noWorkLabel = new System.Windows.Forms.Label();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._finishPanel.SuspendLayout();
            this._destinationPanel.SuspendLayout();
            this._noWorkPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _finishPanel
            // 
            this._finishPanel.Controls.Add(this._exportingFileLabel);
            this._finishPanel.Controls.Add(this._exportProgressBar);
            this._finishPanel.Controls.Add(this._destinationLabel);
            this._finishPanel.Controls.Add(this._exportLabel);
            this._finishPanel.Controls.Add(this._exportHeaderLabel);
            this._finishPanel.Controls.Add(this._finishLabel);
            this._finishPanel.Controls.Add(this._destinationPanel);
            this._finishPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._finishPanel.Location = new System.Drawing.Point(0, 0);
            this._finishPanel.Name = "_finishPanel";
            this._finishPanel.Size = new System.Drawing.Size(597, 313);
            this._finishPanel.TabIndex = 0;
            // 
            // _exportingFileLabel
            // 
            this._exportingFileLabel.AutoEllipsis = true;
            this._exportingFileLabel.Location = new System.Drawing.Point(78, 178);
            this._exportingFileLabel.Name = "_exportingFileLabel";
            this._exportingFileLabel.Size = new System.Drawing.Size(438, 23);
            this._exportingFileLabel.TabIndex = 10;
            // 
            // _exportProgressBar
            // 
            this._exportProgressBar.Location = new System.Drawing.Point(81, 155);
            this._exportProgressBar.Name = "_exportProgressBar";
            this._exportProgressBar.Size = new System.Drawing.Size(435, 18);
            this._exportProgressBar.TabIndex = 9;
            this._exportProgressBar.Visible = false;
            // 
            // _destinationLabel
            // 
            this._destinationLabel.AutoSize = true;
            this._destinationLabel.Location = new System.Drawing.Point(12, 58);
            this._destinationLabel.Name = "_destinationLabel";
            this._destinationLabel.Size = new System.Drawing.Size(46, 13);
            this._destinationLabel.TabIndex = 4;
            this._destinationLabel.Text = "Options:";
            // 
            // _exportLabel
            // 
            this._exportLabel.AutoSize = true;
            this._exportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._exportLabel.Location = new System.Drawing.Point(78, 36);
            this._exportLabel.Name = "_exportLabel";
            this._exportLabel.Size = new System.Drawing.Size(69, 13);
            this._exportLabel.TabIndex = 3;
            this._exportLabel.Text = "0 channels";
            // 
            // _exportHeaderLabel
            // 
            this._exportHeaderLabel.AutoSize = true;
            this._exportHeaderLabel.Location = new System.Drawing.Point(12, 36);
            this._exportHeaderLabel.Name = "_exportHeaderLabel";
            this._exportHeaderLabel.Size = new System.Drawing.Size(39, 13);
            this._exportHeaderLabel.TabIndex = 2;
            this._exportHeaderLabel.Text = "Import:";
            // 
            // _finishLabel
            // 
            this._finishLabel.AutoSize = true;
            this._finishLabel.Location = new System.Drawing.Point(12, 12);
            this._finishLabel.Name = "_finishLabel";
            this._finishLabel.Size = new System.Drawing.Size(420, 13);
            this._finishLabel.TabIndex = 1;
            this._finishLabel.Text = "Select options and hit finish to import the selected channels and close the impor" +
                "t wizard.";
            // 
            // _destinationPanel
            // 
            this._destinationPanel.Controls.Add(this._groupNameTextBox);
            this._destinationPanel.Controls.Add(this._importChannelsOnlyRadioButton);
            this._destinationPanel.Controls.Add(this._importInNamedGroupRadioButton);
            this._destinationPanel.Controls.Add(this._importChannelsAndGroupsRadioButton);
            this._destinationPanel.Location = new System.Drawing.Point(81, 53);
            this._destinationPanel.Name = "_destinationPanel";
            this._destinationPanel.Size = new System.Drawing.Size(435, 96);
            this._destinationPanel.TabIndex = 11;
            // 
            // _importChannelsOnlyRadioButton
            // 
            this._importChannelsOnlyRadioButton.AutoSize = true;
            this._importChannelsOnlyRadioButton.Location = new System.Drawing.Point(0, 49);
            this._importChannelsOnlyRadioButton.Name = "_importChannelsOnlyRadioButton";
            this._importChannelsOnlyRadioButton.Size = new System.Drawing.Size(338, 17);
            this._importChannelsOnlyRadioButton.TabIndex = 13;
            this._importChannelsOnlyRadioButton.TabStop = true;
            this._importChannelsOnlyRadioButton.Text = "Import channels only (no groups will be created in ARGUS TV)";
            this._importChannelsOnlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // _importInNamedGroupRadioButton
            // 
            this._importInNamedGroupRadioButton.AutoSize = true;
            this._importInNamedGroupRadioButton.Location = new System.Drawing.Point(0, 26);
            this._importInNamedGroupRadioButton.Name = "_importInNamedGroupRadioButton";
            this._importInNamedGroupRadioButton.Size = new System.Drawing.Size(203, 17);
            this._importInNamedGroupRadioButton.TabIndex = 11;
            this._importInNamedGroupRadioButton.Text = "Import all channels in the same group:";
            this._importInNamedGroupRadioButton.UseVisualStyleBackColor = true;
            this._importInNamedGroupRadioButton.CheckedChanged += new System.EventHandler(this._importInNamedGroupRadioButton_CheckedChanged);
            // 
            // _importChannelsAndGroupsRadioButton
            // 
            this._importChannelsAndGroupsRadioButton.AutoSize = true;
            this._importChannelsAndGroupsRadioButton.Checked = true;
            this._importChannelsAndGroupsRadioButton.Location = new System.Drawing.Point(0, 3);
            this._importChannelsAndGroupsRadioButton.Name = "_importChannelsAndGroupsRadioButton";
            this._importChannelsAndGroupsRadioButton.Size = new System.Drawing.Size(180, 17);
            this._importChannelsAndGroupsRadioButton.TabIndex = 10;
            this._importChannelsAndGroupsRadioButton.TabStop = true;
            this._importChannelsAndGroupsRadioButton.Text = "Import both channels and groups";
            this._importChannelsAndGroupsRadioButton.UseVisualStyleBackColor = true;
            // 
            // _groupNameTextBox
            // 
            this._groupNameTextBox.Enabled = false;
            this._groupNameTextBox.Location = new System.Drawing.Point(213, 25);
            this._groupNameTextBox.Name = "_groupNameTextBox";
            this._groupNameTextBox.Size = new System.Drawing.Size(150, 20);
            this._groupNameTextBox.TabIndex = 12;
            this._groupNameTextBox.Text = "Imported Channels";
            // 
            // _noWorkPanel
            // 
            this._noWorkPanel.Controls.Add(this._noWorkLabel);
            this._noWorkPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._noWorkPanel.Location = new System.Drawing.Point(0, 0);
            this._noWorkPanel.Name = "_noWorkPanel";
            this._noWorkPanel.Size = new System.Drawing.Size(597, 313);
            this._noWorkPanel.TabIndex = 0;
            // 
            // _noWorkLabel
            // 
            this._noWorkLabel.AutoSize = true;
            this._noWorkLabel.Location = new System.Drawing.Point(12, 12);
            this._noWorkLabel.Name = "_noWorkLabel";
            this._noWorkLabel.Size = new System.Drawing.Size(328, 13);
            this._noWorkLabel.TabIndex = 0;
            this._noWorkLabel.Text = "No channels where selected for import. Hit finish to close the wizard.";
            // 
            // _folderBrowserDialog
            // 
            this._folderBrowserDialog.Description = "Select destination folder to export recordings to.";
            // 
            // FinishPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._finishPanel);
            this.Controls.Add(this._noWorkPanel);
            this.Name = "FinishPage";
            this.Size = new System.Drawing.Size(597, 313);
            this._finishPanel.ResumeLayout(false);
            this._finishPanel.PerformLayout();
            this._destinationPanel.ResumeLayout(false);
            this._destinationPanel.PerformLayout();
            this._noWorkPanel.ResumeLayout(false);
            this._noWorkPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _finishPanel;
        private System.Windows.Forms.Panel _noWorkPanel;
        private System.Windows.Forms.Label _noWorkLabel;
        private System.Windows.Forms.Label _exportHeaderLabel;
        private System.Windows.Forms.Label _finishLabel;
        private System.Windows.Forms.Label _exportLabel;
        private System.Windows.Forms.Label _destinationLabel;
        private System.Windows.Forms.TextBox _groupNameTextBox;
        private System.Windows.Forms.ProgressBar _exportProgressBar;
        private System.Windows.Forms.Label _exportingFileLabel;
        private System.Windows.Forms.Panel _destinationPanel;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.RadioButton _importChannelsAndGroupsRadioButton;
        private System.Windows.Forms.RadioButton _importChannelsOnlyRadioButton;
        private System.Windows.Forms.RadioButton _importInNamedGroupRadioButton;

    }
}
