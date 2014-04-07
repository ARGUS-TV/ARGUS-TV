namespace ArgusTV.UI.Console.Wizards.ExportRecordings
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
            this._directoryNameComboBox = new System.Windows.Forms.ComboBox();
            this._createDirectoriesCheckBox = new System.Windows.Forms.CheckBox();
            this._destinationTextBox = new System.Windows.Forms.TextBox();
            this._browseDestinationButton = new System.Windows.Forms.Button();
            this._deleteRecordingsCheckBox = new System.Windows.Forms.CheckBox();
            this._exportMetadataCheckBox = new System.Windows.Forms.CheckBox();
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
            this._destinationLabel.Size = new System.Drawing.Size(63, 13);
            this._destinationLabel.TabIndex = 4;
            this._destinationLabel.Text = "Destination:";
            // 
            // _exportLabel
            // 
            this._exportLabel.AutoSize = true;
            this._exportLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._exportLabel.Location = new System.Drawing.Point(78, 36);
            this._exportLabel.Name = "_exportLabel";
            this._exportLabel.Size = new System.Drawing.Size(77, 13);
            this._exportLabel.TabIndex = 3;
            this._exportLabel.Text = "0 recordings";
            // 
            // _exportHeaderLabel
            // 
            this._exportHeaderLabel.AutoSize = true;
            this._exportHeaderLabel.Location = new System.Drawing.Point(12, 36);
            this._exportHeaderLabel.Name = "_exportHeaderLabel";
            this._exportHeaderLabel.Size = new System.Drawing.Size(40, 13);
            this._exportHeaderLabel.TabIndex = 2;
            this._exportHeaderLabel.Text = "Export:";
            // 
            // _finishLabel
            // 
            this._finishLabel.AutoSize = true;
            this._finishLabel.Location = new System.Drawing.Point(12, 12);
            this._finishLabel.Name = "_finishLabel";
            this._finishLabel.Size = new System.Drawing.Size(445, 13);
            this._finishLabel.TabIndex = 1;
            this._finishLabel.Text = "Select destination and hit finish to export the selected recordings and close the" +
                " export wizard.";
            // 
            // _destinationPanel
            // 
            this._destinationPanel.Controls.Add(this._directoryNameComboBox);
            this._destinationPanel.Controls.Add(this._createDirectoriesCheckBox);
            this._destinationPanel.Controls.Add(this._destinationTextBox);
            this._destinationPanel.Controls.Add(this._browseDestinationButton);
            this._destinationPanel.Controls.Add(this._deleteRecordingsCheckBox);
            this._destinationPanel.Controls.Add(this._exportMetadataCheckBox);
            this._destinationPanel.Location = new System.Drawing.Point(81, 53);
            this._destinationPanel.Name = "_destinationPanel";
            this._destinationPanel.Size = new System.Drawing.Size(435, 96);
            this._destinationPanel.TabIndex = 11;
            // 
            // _directoryNameComboBox
            // 
            this._directoryNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._directoryNameComboBox.Enabled = false;
            this._directoryNameComboBox.FormattingEnabled = true;
            this._directoryNameComboBox.Items.AddRange(new object[] {
            "Title",
            "Schedule",
            "Channel",
            "Date"});
            this._directoryNameComboBox.Location = new System.Drawing.Point(223, 26);
            this._directoryNameComboBox.Name = "_directoryNameComboBox";
            this._directoryNameComboBox.Size = new System.Drawing.Size(80, 21);
            this._directoryNameComboBox.TabIndex = 8;
            // 
            // _createDirectoriesCheckBox
            // 
            this._createDirectoriesCheckBox.AutoSize = true;
            this._createDirectoriesCheckBox.Location = new System.Drawing.Point(0, 29);
            this._createDirectoriesCheckBox.Name = "_createDirectoriesCheckBox";
            this._createDirectoriesCheckBox.Size = new System.Drawing.Size(222, 17);
            this._createDirectoriesCheckBox.TabIndex = 7;
            this._createDirectoriesCheckBox.Text = "Create subdirectories in destination folder:";
            this._createDirectoriesCheckBox.UseVisualStyleBackColor = true;
            this._createDirectoriesCheckBox.CheckedChanged += new System.EventHandler(this._createDirectoriesCheckBox_CheckedChanged);
            // 
            // _destinationTextBox
            // 
            this._destinationTextBox.Location = new System.Drawing.Point(0, 2);
            this._destinationTextBox.Name = "_destinationTextBox";
            this._destinationTextBox.Size = new System.Drawing.Size(356, 20);
            this._destinationTextBox.TabIndex = 5;
            // 
            // _browseDestinationButton
            // 
            this._browseDestinationButton.Location = new System.Drawing.Point(360, 0);
            this._browseDestinationButton.Name = "_browseDestinationButton";
            this._browseDestinationButton.Size = new System.Drawing.Size(75, 23);
            this._browseDestinationButton.TabIndex = 6;
            this._browseDestinationButton.Text = "Browse...";
            this._browseDestinationButton.UseVisualStyleBackColor = true;
            this._browseDestinationButton.Click += new System.EventHandler(this._browseDestinationButton_Click);
            // 
            // _deleteRecordingsCheckBox
            // 
            this._deleteRecordingsCheckBox.AutoSize = true;
            this._deleteRecordingsCheckBox.Location = new System.Drawing.Point(0, 75);
            this._deleteRecordingsCheckBox.Name = "_deleteRecordingsCheckBox";
            this._deleteRecordingsCheckBox.Size = new System.Drawing.Size(266, 17);
            this._deleteRecordingsCheckBox.TabIndex = 10;
            this._deleteRecordingsCheckBox.Text = "Delete recordings from ARGUS TV after export";
            this._deleteRecordingsCheckBox.UseVisualStyleBackColor = true;
            // 
            // _exportMetadataCheckBox
            // 
            this._exportMetadataCheckBox.AutoSize = true;
            this._exportMetadataCheckBox.Location = new System.Drawing.Point(0, 52);
            this._exportMetadataCheckBox.Name = "_exportMetadataCheckBox";
            this._exportMetadataCheckBox.Size = new System.Drawing.Size(227, 17);
            this._exportMetadataCheckBox.TabIndex = 9;
            this._exportMetadataCheckBox.Text = "Also export metadata in separate .arg file";
            this._exportMetadataCheckBox.UseVisualStyleBackColor = true;
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
            this._noWorkLabel.Size = new System.Drawing.Size(335, 13);
            this._noWorkLabel.TabIndex = 0;
            this._noWorkLabel.Text = "No recordings where selected for export. Hit finish to close the wizard.";
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
        private System.Windows.Forms.Button _browseDestinationButton;
        private System.Windows.Forms.TextBox _destinationTextBox;
        private System.Windows.Forms.CheckBox _deleteRecordingsCheckBox;
        private System.Windows.Forms.CheckBox _exportMetadataCheckBox;
        private System.Windows.Forms.ProgressBar _exportProgressBar;
        private System.Windows.Forms.Label _exportingFileLabel;
        private System.Windows.Forms.Panel _destinationPanel;
        private System.Windows.Forms.CheckBox _createDirectoriesCheckBox;
        private System.Windows.Forms.ComboBox _directoryNameComboBox;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;

    }
}
