namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    partial class ScanRecordingFoldersPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanRecordingFoldersPage));
            this._processingPanel = new System.Windows.Forms.Panel();
            this._busyPictureBox = new System.Windows.Forms.PictureBox();
            this._processingLabel = new System.Windows.Forms.Label();
            this._processingBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._processingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _processingPanel
            // 
            this._processingPanel.Controls.Add(this._busyPictureBox);
            this._processingPanel.Controls.Add(this._processingLabel);
            this._processingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._processingPanel.Location = new System.Drawing.Point(0, 0);
            this._processingPanel.Name = "_processingPanel";
            this._processingPanel.Size = new System.Drawing.Size(597, 313);
            this._processingPanel.TabIndex = 12;
            // 
            // _busyPictureBox
            // 
            this._busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._busyPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("_busyPictureBox.Image")));
            this._busyPictureBox.Location = new System.Drawing.Point(290, 143);
            this._busyPictureBox.Name = "_busyPictureBox";
            this._busyPictureBox.Size = new System.Drawing.Size(16, 16);
            this._busyPictureBox.TabIndex = 3;
            this._busyPictureBox.TabStop = false;
            // 
            // _processingLabel
            // 
            this._processingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._processingLabel.AutoSize = true;
            this._processingLabel.Location = new System.Drawing.Point(261, 125);
            this._processingLabel.Name = "_processingLabel";
            this._processingLabel.Size = new System.Drawing.Size(82, 13);
            this._processingLabel.TabIndex = 0;
            this._processingLabel.Text = "Scanning files...";
            // 
            // _processingBackgroundWorker
            // 
            this._processingBackgroundWorker.WorkerSupportsCancellation = true;
            // 
            // ScanRecordingFoldersPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._processingPanel);
            this.Name = "ScanRecordingFoldersPage";
            this.Size = new System.Drawing.Size(597, 313);
            this._processingPanel.ResumeLayout(false);
            this._processingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _processingPanel;
        private System.Windows.Forms.Label _processingLabel;
        private System.ComponentModel.BackgroundWorker _processingBackgroundWorker;
        private System.Windows.Forms.PictureBox _busyPictureBox;
    }
}
