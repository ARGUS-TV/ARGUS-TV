namespace ArgusTV.UI.Console
{
    partial class SelectProcessingCommandTimeForm
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
            this._commandsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._scheduleLabel = new System.Windows.Forms.Label();
            this._commandLabel = new System.Windows.Forms.Label();
            this._toRunLabel = new System.Windows.Forms.Label();
            this._recordingLabel = new System.Windows.Forms.Label();
            this._nowRadioButton = new System.Windows.Forms.RadioButton();
            this._atTimeRadioButton = new System.Windows.Forms.RadioButton();
            this._runAtTimePicker = new System.Windows.Forms.DateTimePicker();
            this._infoGroupBox = new System.Windows.Forms.GroupBox();
            this._whenGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).BeginInit();
            this._infoGroupBox.SuspendLayout();
            this._whenGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(357, 171);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 21;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(276, 171);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 20;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _scheduleLabel
            // 
            this._scheduleLabel.AutoSize = true;
            this._scheduleLabel.Location = new System.Drawing.Point(8, 19);
            this._scheduleLabel.Name = "_scheduleLabel";
            this._scheduleLabel.Size = new System.Drawing.Size(57, 13);
            this._scheduleLabel.TabIndex = 0;
            this._scheduleLabel.Text = "Command:";
            // 
            // _commandLabel
            // 
            this._commandLabel.AutoSize = true;
            this._commandLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._commandLabel.Location = new System.Drawing.Point(67, 19);
            this._commandLabel.Name = "_commandLabel";
            this._commandLabel.Size = new System.Drawing.Size(0, 13);
            this._commandLabel.TabIndex = 1;
            // 
            // _toRunLabel
            // 
            this._toRunLabel.AutoSize = true;
            this._toRunLabel.Location = new System.Drawing.Point(8, 38);
            this._toRunLabel.Name = "_toRunLabel";
            this._toRunLabel.Size = new System.Drawing.Size(59, 13);
            this._toRunLabel.TabIndex = 2;
            this._toRunLabel.Text = "Recording:";
            // 
            // _recordingLabel
            // 
            this._recordingLabel.AutoSize = true;
            this._recordingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._recordingLabel.Location = new System.Drawing.Point(67, 38);
            this._recordingLabel.Name = "_recordingLabel";
            this._recordingLabel.Size = new System.Drawing.Size(0, 13);
            this._recordingLabel.TabIndex = 3;
            // 
            // _nowRadioButton
            // 
            this._nowRadioButton.AutoSize = true;
            this._nowRadioButton.Checked = true;
            this._nowRadioButton.Location = new System.Drawing.Point(15, 19);
            this._nowRadioButton.Name = "_nowRadioButton";
            this._nowRadioButton.Size = new System.Drawing.Size(47, 17);
            this._nowRadioButton.TabIndex = 10;
            this._nowRadioButton.TabStop = true;
            this._nowRadioButton.Text = "Now";
            this._nowRadioButton.UseVisualStyleBackColor = true;
            // 
            // _atTimeRadioButton
            // 
            this._atTimeRadioButton.AutoSize = true;
            this._atTimeRadioButton.Location = new System.Drawing.Point(15, 42);
            this._atTimeRadioButton.Name = "_atTimeRadioButton";
            this._atTimeRadioButton.Size = new System.Drawing.Size(60, 17);
            this._atTimeRadioButton.TabIndex = 11;
            this._atTimeRadioButton.Text = "At time:";
            this._atTimeRadioButton.UseVisualStyleBackColor = true;
            // 
            // _runAtTimePicker
            // 
            this._runAtTimePicker.CustomFormat = "HH:mm";
            this._runAtTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._runAtTimePicker.Location = new System.Drawing.Point(77, 41);
            this._runAtTimePicker.Name = "_runAtTimePicker";
            this._runAtTimePicker.ShowUpDown = true;
            this._runAtTimePicker.Size = new System.Drawing.Size(60, 20);
            this._runAtTimePicker.TabIndex = 12;
            // 
            // _infoGroupBox
            // 
            this._infoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._infoGroupBox.Controls.Add(this._scheduleLabel);
            this._infoGroupBox.Controls.Add(this._commandLabel);
            this._infoGroupBox.Controls.Add(this._toRunLabel);
            this._infoGroupBox.Controls.Add(this._recordingLabel);
            this._infoGroupBox.Location = new System.Drawing.Point(12, 12);
            this._infoGroupBox.Name = "_infoGroupBox";
            this._infoGroupBox.Size = new System.Drawing.Size(420, 64);
            this._infoGroupBox.TabIndex = 22;
            this._infoGroupBox.TabStop = false;
            this._infoGroupBox.Text = "Details";
            // 
            // _whenGroupBox
            // 
            this._whenGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._whenGroupBox.Controls.Add(this._nowRadioButton);
            this._whenGroupBox.Controls.Add(this._atTimeRadioButton);
            this._whenGroupBox.Controls.Add(this._runAtTimePicker);
            this._whenGroupBox.Location = new System.Drawing.Point(12, 82);
            this._whenGroupBox.Name = "_whenGroupBox";
            this._whenGroupBox.Size = new System.Drawing.Size(420, 74);
            this._whenGroupBox.TabIndex = 23;
            this._whenGroupBox.TabStop = false;
            this._whenGroupBox.Text = "Run";
            // 
            // SelectProcessingCommandTimeForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(444, 204);
            this.Controls.Add(this._infoGroupBox);
            this.Controls.Add(this._whenGroupBox);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProcessingCommandTimeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule Processing Command To Run";
            this.Load += new System.EventHandler(this.SelectProcessingCommandsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).EndInit();
            this._infoGroupBox.ResumeLayout(false);
            this._infoGroupBox.PerformLayout();
            this._whenGroupBox.ResumeLayout(false);
            this._whenGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.BindingSource _commandsBindingSource;
        private System.Windows.Forms.Label _scheduleLabel;
        private System.Windows.Forms.Label _commandLabel;
        private System.Windows.Forms.Label _toRunLabel;
        private System.Windows.Forms.Label _recordingLabel;
        private System.Windows.Forms.RadioButton _nowRadioButton;
        private System.Windows.Forms.RadioButton _atTimeRadioButton;
        private System.Windows.Forms.DateTimePicker _runAtTimePicker;
        private System.Windows.Forms.GroupBox _infoGroupBox;
        private System.Windows.Forms.GroupBox _whenGroupBox;
    }
}