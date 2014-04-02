namespace ArgusTV.Recorder.MediaPortalTvServer
{
    partial class CreateShareForm
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
            this._pathLabel = new System.Windows.Forms.Label();
            this._localPathLabel = new System.Windows.Forms.Label();
            this._shareLabel = new System.Windows.Forms.Label();
            this._shareNameTextBox = new System.Windows.Forms.TextBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _pathLabel
            // 
            this._pathLabel.AutoSize = true;
            this._pathLabel.Location = new System.Drawing.Point(12, 15);
            this._pathLabel.Name = "_pathLabel";
            this._pathLabel.Size = new System.Drawing.Size(60, 13);
            this._pathLabel.TabIndex = 0;
            this._pathLabel.Text = "Local path:";
            // 
            // _localPathLabel
            // 
            this._localPathLabel.AutoEllipsis = true;
            this._localPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._localPathLabel.Location = new System.Drawing.Point(85, 15);
            this._localPathLabel.Name = "_localPathLabel";
            this._localPathLabel.Size = new System.Drawing.Size(297, 20);
            this._localPathLabel.TabIndex = 1;
            // 
            // _shareLabel
            // 
            this._shareLabel.AutoSize = true;
            this._shareLabel.Location = new System.Drawing.Point(12, 41);
            this._shareLabel.Name = "_shareLabel";
            this._shareLabel.Size = new System.Drawing.Size(67, 13);
            this._shareLabel.TabIndex = 2;
            this._shareLabel.Text = "Share name:";
            // 
            // _shareNameTextBox
            // 
            this._shareNameTextBox.Location = new System.Drawing.Point(85, 38);
            this._shareNameTextBox.Name = "_shareNameTextBox";
            this._shareNameTextBox.Size = new System.Drawing.Size(297, 20);
            this._shareNameTextBox.TabIndex = 3;
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(226, 72);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 4;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(307, 72);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 5;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // CreateShareForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(394, 104);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._shareNameTextBox);
            this.Controls.Add(this._shareLabel);
            this.Controls.Add(this._localPathLabel);
            this.Controls.Add(this._pathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateShareForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create UNC Share";
            this.Load += new System.EventHandler(this.CreateShareForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _pathLabel;
        private System.Windows.Forms.Label _localPathLabel;
        private System.Windows.Forms.Label _shareLabel;
        private System.Windows.Forms.TextBox _shareNameTextBox;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}