namespace ArgusTV.WinForms
{
    partial class ThumbnailPopup
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
            this._thumbnailPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._thumbnailPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _thumbnailPictureBox
            // 
            this._thumbnailPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._thumbnailPictureBox.Location = new System.Drawing.Point(0, 0);
            this._thumbnailPictureBox.Name = "_thumbnailPictureBox";
            this._thumbnailPictureBox.Size = new System.Drawing.Size(624, 446);
            this._thumbnailPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._thumbnailPictureBox.TabIndex = 0;
            this._thumbnailPictureBox.TabStop = false;
            this._thumbnailPictureBox.Click += new System.EventHandler(this._thumbnailPictureBox_Click);
            // 
            // ThumbnailPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(624, 446);
            this.Controls.Add(this._thumbnailPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ThumbnailPopup";
            ((System.ComponentModel.ISupportInitialize)(this._thumbnailPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _thumbnailPictureBox;
    }
}