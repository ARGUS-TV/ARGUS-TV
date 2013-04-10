namespace ArgusTV.GuideImporter.ClickFinder
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
            this._pluginNameLabel = new System.Windows.Forms.Label();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._pluginNameTextBox = new System.Windows.Forms.TextBox();
            this._connectionStringLabel = new System.Windows.Forms.Label();
            this.TvUptodatePathLabel = new System.Windows.Forms.Label();
            this._launchClickFinderBeforeImportCheckBox = new System.Windows.Forms.CheckBox();
            this._connectionStringTextBox = new System.Windows.Forms.TextBox();
            this._tvUpToDatePathTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._browseButton = new System.Windows.Forms.Button();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this._shortDescriptionRadioButton = new System.Windows.Forms.RadioButton();
            this._longDescriptionRadioButton = new System.Windows.Forms.RadioButton();
            this._generalGroupBox = new System.Windows.Forms.GroupBox();
            this._testConnectionButton = new System.Windows.Forms.Button();
            this._contentGroupBox = new System.Windows.Forms.GroupBox();
            this._defaultButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this._generalGroupBox.SuspendLayout();
            this._contentGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pluginNameLabel
            // 
            this._pluginNameLabel.AutoSize = true;
            this._pluginNameLabel.Location = new System.Drawing.Point(9, 22);
            this._pluginNameLabel.Name = "_pluginNameLabel";
            this._pluginNameLabel.Size = new System.Drawing.Size(71, 13);
            this._pluginNameLabel.TabIndex = 0;
            this._pluginNameLabel.Text = "Plugin name :";
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._saveButton.Location = new System.Drawing.Point(398, 239);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 8;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.CausesValidation = false;
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(479, 239);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _pluginNameTextBox
            // 
            this._pluginNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pluginNameTextBox.Location = new System.Drawing.Point(123, 19);
            this._pluginNameTextBox.Name = "_pluginNameTextBox";
            this._pluginNameTextBox.Size = new System.Drawing.Size(321, 20);
            this._pluginNameTextBox.TabIndex = 1;
            this._pluginNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._pluginNameTextBox_Validating);
            // 
            // _connectionStringLabel
            // 
            this._connectionStringLabel.AutoSize = true;
            this._connectionStringLabel.Location = new System.Drawing.Point(9, 87);
            this._connectionStringLabel.Name = "_connectionStringLabel";
            this._connectionStringLabel.Size = new System.Drawing.Size(95, 13);
            this._connectionStringLabel.TabIndex = 4;
            this._connectionStringLabel.Text = "Connection string :";
            // 
            // TvUptodatePathLabel
            // 
            this.TvUptodatePathLabel.AutoSize = true;
            this.TvUptodatePathLabel.Location = new System.Drawing.Point(9, 54);
            this.TvUptodatePathLabel.Name = "TvUptodatePathLabel";
            this.TvUptodatePathLabel.Size = new System.Drawing.Size(107, 13);
            this.TvUptodatePathLabel.TabIndex = 5;
            this.TvUptodatePathLabel.Text = "Path to TvUptodate :";
            // 
            // _launchClickFinderBeforeImportCheckBox
            // 
            this._launchClickFinderBeforeImportCheckBox.AutoSize = true;
            this._launchClickFinderBeforeImportCheckBox.Location = new System.Drawing.Point(12, 120);
            this._launchClickFinderBeforeImportCheckBox.Name = "_launchClickFinderBeforeImportCheckBox";
            this._launchClickFinderBeforeImportCheckBox.Size = new System.Drawing.Size(236, 17);
            this._launchClickFinderBeforeImportCheckBox.TabIndex = 5;
            this._launchClickFinderBeforeImportCheckBox.Text = "Launch ClickFinder before starting the import";
            this._launchClickFinderBeforeImportCheckBox.UseVisualStyleBackColor = true;
            // 
            // _connectionStringTextBox
            // 
            this._connectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._connectionStringTextBox.Location = new System.Drawing.Point(123, 84);
            this._connectionStringTextBox.Name = "_connectionStringTextBox";
            this._connectionStringTextBox.Size = new System.Drawing.Size(321, 20);
            this._connectionStringTextBox.TabIndex = 4;
            this._connectionStringTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._connectionStringTextBox_Validating);
            // 
            // _tvUpToDatePathTextBox
            // 
            this._tvUpToDatePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tvUpToDatePathTextBox.Location = new System.Drawing.Point(123, 51);
            this._tvUpToDatePathTextBox.Name = "_tvUpToDatePathTextBox";
            this._tvUpToDatePathTextBox.Size = new System.Drawing.Size(285, 20);
            this._tvUpToDatePathTextBox.TabIndex = 2;
            this._tvUpToDatePathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._tvUpToDatePathTextBox_Validating);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::ArgusTV.GuideImporter.ClickFinder.Properties.Resources.tvmovie4_infografik;
            this.pictureBox1.Location = new System.Drawing.Point(470, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 52);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "tvUptodate.exe";
            // 
            // _browseButton
            // 
            this._browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseButton.Location = new System.Drawing.Point(417, 49);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(26, 23);
            this._browseButton.TabIndex = 3;
            this._browseButton.Text = "...";
            this._browseButton.UseVisualStyleBackColor = true;
            this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Description field :";
            // 
            // _shortDescriptionRadioButton
            // 
            this._shortDescriptionRadioButton.AutoSize = true;
            this._shortDescriptionRadioButton.Location = new System.Drawing.Point(233, 19);
            this._shortDescriptionRadioButton.Name = "_shortDescriptionRadioButton";
            this._shortDescriptionRadioButton.Size = new System.Drawing.Size(107, 17);
            this._shortDescriptionRadioButton.TabIndex = 7;
            this._shortDescriptionRadioButton.TabStop = true;
            this._shortDescriptionRadioButton.Text = "Use short version";
            this._shortDescriptionRadioButton.UseVisualStyleBackColor = true;
            // 
            // _longDescriptionRadioButton
            // 
            this._longDescriptionRadioButton.AutoSize = true;
            this._longDescriptionRadioButton.Location = new System.Drawing.Point(123, 19);
            this._longDescriptionRadioButton.Name = "_longDescriptionRadioButton";
            this._longDescriptionRadioButton.Size = new System.Drawing.Size(104, 17);
            this._longDescriptionRadioButton.TabIndex = 6;
            this._longDescriptionRadioButton.TabStop = true;
            this._longDescriptionRadioButton.Text = "Use long version";
            this._longDescriptionRadioButton.UseVisualStyleBackColor = true;
            // 
            // _generalGroupBox
            // 
            this._generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._generalGroupBox.Controls.Add(this._testConnectionButton);
            this._generalGroupBox.Controls.Add(this._pluginNameTextBox);
            this._generalGroupBox.Controls.Add(this._pluginNameLabel);
            this._generalGroupBox.Controls.Add(this._connectionStringLabel);
            this._generalGroupBox.Controls.Add(this.TvUptodatePathLabel);
            this._generalGroupBox.Controls.Add(this._launchClickFinderBeforeImportCheckBox);
            this._generalGroupBox.Controls.Add(this._browseButton);
            this._generalGroupBox.Controls.Add(this._connectionStringTextBox);
            this._generalGroupBox.Controls.Add(this._tvUpToDatePathTextBox);
            this._generalGroupBox.Controls.Add(this.pictureBox1);
            this._generalGroupBox.Location = new System.Drawing.Point(10, 12);
            this._generalGroupBox.Name = "_generalGroupBox";
            this._generalGroupBox.Size = new System.Drawing.Size(546, 155);
            this._generalGroupBox.TabIndex = 14;
            this._generalGroupBox.TabStop = false;
            this._generalGroupBox.Text = "General";
            // 
            // _testConnectionButton
            // 
            this._testConnectionButton.Location = new System.Drawing.Point(464, 81);
            this._testConnectionButton.Name = "_testConnectionButton";
            this._testConnectionButton.Size = new System.Drawing.Size(61, 23);
            this._testConnectionButton.TabIndex = 11;
            this._testConnectionButton.Text = "Test";
            this._testConnectionButton.UseVisualStyleBackColor = true;
            this._testConnectionButton.Click += new System.EventHandler(this._testConnectionButton_Click);
            // 
            // _contentGroupBox
            // 
            this._contentGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._contentGroupBox.Controls.Add(this.label1);
            this._contentGroupBox.Controls.Add(this._longDescriptionRadioButton);
            this._contentGroupBox.Controls.Add(this._shortDescriptionRadioButton);
            this._contentGroupBox.Location = new System.Drawing.Point(10, 177);
            this._contentGroupBox.Name = "_contentGroupBox";
            this._contentGroupBox.Size = new System.Drawing.Size(546, 49);
            this._contentGroupBox.TabIndex = 15;
            this._contentGroupBox.TabStop = false;
            this._contentGroupBox.Text = "Content";
            // 
            // _defaultButton
            // 
            this._defaultButton.Location = new System.Drawing.Point(10, 239);
            this._defaultButton.Name = "_defaultButton";
            this._defaultButton.Size = new System.Drawing.Size(75, 23);
            this._defaultButton.TabIndex = 16;
            this._defaultButton.Text = "Set Default";
            this._defaultButton.UseVisualStyleBackColor = true;
            this._defaultButton.Click += new System.EventHandler(this._defaultButton_Click);
            // 
            // ConfigurationForm
            // 
            this.AcceptButton = this._saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(568, 274);
            this.Controls.Add(this._defaultButton);
            this.Controls.Add(this._contentGroupBox);
            this.Controls.Add(this._generalGroupBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._saveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(578, 297);
            this.Name = "ConfigurationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClickFinderPlugin - Configuration";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this._generalGroupBox.ResumeLayout(false);
            this._generalGroupBox.PerformLayout();
            this._contentGroupBox.ResumeLayout(false);
            this._contentGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _pluginNameLabel;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TextBox _pluginNameTextBox;
        private System.Windows.Forms.Label _connectionStringLabel;
        private System.Windows.Forms.Label TvUptodatePathLabel;
        private System.Windows.Forms.CheckBox _launchClickFinderBeforeImportCheckBox;
        private System.Windows.Forms.TextBox _connectionStringTextBox;
        private System.Windows.Forms.TextBox _tvUpToDatePathTextBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.ErrorProvider _errorProvider;
        private System.Windows.Forms.RadioButton _longDescriptionRadioButton;
        private System.Windows.Forms.RadioButton _shortDescriptionRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox _generalGroupBox;
        private System.Windows.Forms.GroupBox _contentGroupBox;
        private System.Windows.Forms.Button _testConnectionButton;
        private System.Windows.Forms.Button _defaultButton;
    }
}