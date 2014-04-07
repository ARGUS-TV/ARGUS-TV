namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
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
            this._importLabel = new System.Windows.Forms.Label();
            this._moveLabel = new System.Windows.Forms.Label();
            this._cleanupLabel = new System.Windows.Forms.Label();
            this._moveHeaderLabel = new System.Windows.Forms.Label();
            this._cleanupHeaderLabel = new System.Windows.Forms.Label();
            this._importHeaderLabel = new System.Windows.Forms.Label();
            this._finishLabel = new System.Windows.Forms.Label();
            this._noWorkPanel = new System.Windows.Forms.Panel();
            this._noWorkLabel = new System.Windows.Forms.Label();
            this._finishPanel.SuspendLayout();
            this._noWorkPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _finishPanel
            // 
            this._finishPanel.Controls.Add(this._importLabel);
            this._finishPanel.Controls.Add(this._moveLabel);
            this._finishPanel.Controls.Add(this._cleanupLabel);
            this._finishPanel.Controls.Add(this._moveHeaderLabel);
            this._finishPanel.Controls.Add(this._cleanupHeaderLabel);
            this._finishPanel.Controls.Add(this._importHeaderLabel);
            this._finishPanel.Controls.Add(this._finishLabel);
            this._finishPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._finishPanel.Location = new System.Drawing.Point(0, 0);
            this._finishPanel.Name = "_finishPanel";
            this._finishPanel.Size = new System.Drawing.Size(597, 313);
            this._finishPanel.TabIndex = 0;
            // 
            // _importLabel
            // 
            this._importLabel.AutoSize = true;
            this._importLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._importLabel.Location = new System.Drawing.Point(70, 36);
            this._importLabel.Name = "_importLabel";
            this._importLabel.Size = new System.Drawing.Size(104, 13);
            this._importLabel.TabIndex = 7;
            this._importLabel.Text = "0 new recordings";
            // 
            // _moveLabel
            // 
            this._moveLabel.AutoSize = true;
            this._moveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._moveLabel.Location = new System.Drawing.Point(70, 56);
            this._moveLabel.Name = "_moveLabel";
            this._moveLabel.Size = new System.Drawing.Size(118, 13);
            this._moveLabel.TabIndex = 6;
            this._moveLabel.Text = "0 moved recordings";
            // 
            // _cleanupLabel
            // 
            this._cleanupLabel.AutoSize = true;
            this._cleanupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cleanupLabel.Location = new System.Drawing.Point(70, 76);
            this._cleanupLabel.Name = "_cleanupLabel";
            this._cleanupLabel.Size = new System.Drawing.Size(122, 13);
            this._cleanupLabel.TabIndex = 5;
            this._cleanupLabel.Text = "0 missing recordings";
            // 
            // _moveHeaderLabel
            // 
            this._moveHeaderLabel.AutoSize = true;
            this._moveHeaderLabel.Location = new System.Drawing.Point(12, 56);
            this._moveHeaderLabel.Name = "_moveHeaderLabel";
            this._moveHeaderLabel.Size = new System.Drawing.Size(37, 13);
            this._moveHeaderLabel.TabIndex = 4;
            this._moveHeaderLabel.Text = "Move:";
            // 
            // _cleanupHeaderLabel
            // 
            this._cleanupHeaderLabel.AutoSize = true;
            this._cleanupHeaderLabel.Location = new System.Drawing.Point(12, 76);
            this._cleanupHeaderLabel.Name = "_cleanupHeaderLabel";
            this._cleanupHeaderLabel.Size = new System.Drawing.Size(52, 13);
            this._cleanupHeaderLabel.TabIndex = 3;
            this._cleanupHeaderLabel.Text = "Clean up:";
            // 
            // _importHeaderLabel
            // 
            this._importHeaderLabel.AutoSize = true;
            this._importHeaderLabel.Location = new System.Drawing.Point(12, 36);
            this._importHeaderLabel.Name = "_importHeaderLabel";
            this._importHeaderLabel.Size = new System.Drawing.Size(39, 13);
            this._importHeaderLabel.TabIndex = 2;
            this._importHeaderLabel.Text = "Import:";
            // 
            // _finishLabel
            // 
            this._finishLabel.AutoSize = true;
            this._finishLabel.Location = new System.Drawing.Point(12, 12);
            this._finishLabel.Name = "_finishLabel";
            this._finishLabel.Size = new System.Drawing.Size(271, 13);
            this._finishLabel.TabIndex = 1;
            this._finishLabel.Text = "Hit finish to close the import/synchronization wizard and:";
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
            this._noWorkLabel.Size = new System.Drawing.Size(455, 13);
            this._noWorkLabel.TabIndex = 0;
            this._noWorkLabel.Text = "No new, moved or missing recordings where detected or selected. Hit finish to clo" +
                "se the wizard.";
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
            this._noWorkPanel.ResumeLayout(false);
            this._noWorkPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _finishPanel;
        private System.Windows.Forms.Panel _noWorkPanel;
        private System.Windows.Forms.Label _noWorkLabel;
        private System.Windows.Forms.Label _cleanupLabel;
        private System.Windows.Forms.Label _moveHeaderLabel;
        private System.Windows.Forms.Label _cleanupHeaderLabel;
        private System.Windows.Forms.Label _importHeaderLabel;
        private System.Windows.Forms.Label _finishLabel;
        private System.Windows.Forms.Label _importLabel;
        private System.Windows.Forms.Label _moveLabel;

    }
}
