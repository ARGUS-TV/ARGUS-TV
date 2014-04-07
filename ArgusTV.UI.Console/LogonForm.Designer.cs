namespace ArgusTV.UI.Console
{
    partial class LogonForm
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
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._userNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._savePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(180, 93);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 11;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(99, 93);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 10;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _userNameTextBox
            // 
            this._userNameTextBox.Location = new System.Drawing.Point(70, 12);
            this._userNameTextBox.Name = "_userNameTextBox";
            this._userNameTextBox.Size = new System.Drawing.Size(185, 20);
            this._userNameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Location = new System.Drawing.Point(70, 38);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(185, 20);
            this._passwordTextBox.TabIndex = 3;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _savePasswordCheckBox
            // 
            this._savePasswordCheckBox.AutoSize = true;
            this._savePasswordCheckBox.Location = new System.Drawing.Point(70, 64);
            this._savePasswordCheckBox.Name = "_savePasswordCheckBox";
            this._savePasswordCheckBox.Size = new System.Drawing.Size(99, 17);
            this._savePasswordCheckBox.TabIndex = 4;
            this._savePasswordCheckBox.Text = "Save password";
            this._savePasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // LogonForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(267, 126);
            this.Controls.Add(this._savePasswordCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._passwordTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._userNameTextBox);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogonForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log On to ARGUS TV";
            this.Load += new System.EventHandler(this.LogonForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.TextBox _userNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.CheckBox _savePasswordCheckBox;
    }
}