namespace ArgusTV.UI.Console
{
    partial class SplashScreenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreenForm));
            this._processingLabel = new System.Windows.Forms.Label();
            this._busyPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _processingLabel
            // 
            this._processingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._processingLabel.AutoSize = true;
            this._processingLabel.BackColor = System.Drawing.Color.Transparent;
            this._processingLabel.Location = new System.Drawing.Point(185, 103);
            this._processingLabel.Name = "_processingLabel";
            this._processingLabel.Size = new System.Drawing.Size(70, 13);
            this._processingLabel.TabIndex = 12;
            this._processingLabel.Text = "Connecting...";
            // 
            // _busyPictureBox
            // 
            this._busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._busyPictureBox.BackColor = System.Drawing.Color.Transparent;
            this._busyPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("_busyPictureBox.Image")));
            this._busyPictureBox.Location = new System.Drawing.Point(207, 120);
            this._busyPictureBox.Name = "_busyPictureBox";
            this._busyPictureBox.Size = new System.Drawing.Size(16, 16);
            this._busyPictureBox.TabIndex = 13;
            this._busyPictureBox.TabStop = false;
            // 
            // SplashScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(435, 166);
            this.Controls.Add(this._processingLabel);
            this.Controls.Add(this._busyPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreenForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _processingLabel;
        private System.Windows.Forms.PictureBox _busyPictureBox;
    }
}