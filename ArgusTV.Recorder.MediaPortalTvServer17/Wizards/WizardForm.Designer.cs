namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards
{
    partial class WizardForm<TContext>
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
            this._headerTitleLabel = new System.Windows.Forms.Label();
            this._wizardLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._buttonsPanel = new System.Windows.Forms.Panel();
            this._backButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._nextButton = new System.Windows.Forms.Button();
            this._finishButton = new System.Windows.Forms.Button();
            this._headerPanel = new System.Windows.Forms.Panel();
            this._headerSeparatorPanel = new System.Windows.Forms.Panel();
            this._headerPictureBox = new System.Windows.Forms.PictureBox();
            this._headerInfoLabel = new System.Windows.Forms.Label();
            this._pagesPanel = new System.Windows.Forms.Panel();
            this._wizardLayoutPanel.SuspendLayout();
            this._buttonsPanel.SuspendLayout();
            this._headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._headerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _headerTitleLabel
            // 
            this._headerTitleLabel.AutoSize = true;
            this._headerTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._headerTitleLabel.Location = new System.Drawing.Point(12, 7);
            this._headerTitleLabel.Name = "_headerTitleLabel";
            this._headerTitleLabel.Size = new System.Drawing.Size(32, 13);
            this._headerTitleLabel.TabIndex = 1;
            this._headerTitleLabel.Text = "Title";
            // 
            // _wizardLayoutPanel
            // 
            this._wizardLayoutPanel.ColumnCount = 1;
            this._wizardLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._wizardLayoutPanel.Controls.Add(this._buttonsPanel, 0, 2);
            this._wizardLayoutPanel.Controls.Add(this._headerPanel, 0, 0);
            this._wizardLayoutPanel.Controls.Add(this._pagesPanel, 0, 1);
            this._wizardLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._wizardLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._wizardLayoutPanel.Name = "_wizardLayoutPanel";
            this._wizardLayoutPanel.RowCount = 3;
            this._wizardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this._wizardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._wizardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this._wizardLayoutPanel.Size = new System.Drawing.Size(617, 412);
            this._wizardLayoutPanel.TabIndex = 0;
            // 
            // _buttonsPanel
            // 
            this._buttonsPanel.Controls.Add(this._backButton);
            this._buttonsPanel.Controls.Add(this._cancelButton);
            this._buttonsPanel.Controls.Add(this._nextButton);
            this._buttonsPanel.Controls.Add(this._finishButton);
            this._buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonsPanel.Location = new System.Drawing.Point(6, 381);
            this._buttonsPanel.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this._buttonsPanel.Name = "_buttonsPanel";
            this._buttonsPanel.Size = new System.Drawing.Size(605, 28);
            this._buttonsPanel.TabIndex = 1;
            // 
            // _backButton
            // 
            this._backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._backButton.Location = new System.Drawing.Point(356, 1);
            this._backButton.Name = "_backButton";
            this._backButton.Size = new System.Drawing.Size(75, 23);
            this._backButton.TabIndex = 0;
            this._backButton.Text = "< Back";
            this._backButton.UseVisualStyleBackColor = true;
            this._backButton.Click += new System.EventHandler(this._backButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(530, 1);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _nextButton
            // 
            this._nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._nextButton.Location = new System.Drawing.Point(437, 1);
            this._nextButton.Name = "_nextButton";
            this._nextButton.Size = new System.Drawing.Size(75, 23);
            this._nextButton.TabIndex = 1;
            this._nextButton.Text = "Next >";
            this._nextButton.UseVisualStyleBackColor = true;
            this._nextButton.Click += new System.EventHandler(this._nextButton_Click);
            // 
            // _finishButton
            // 
            this._finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._finishButton.Location = new System.Drawing.Point(437, 1);
            this._finishButton.Name = "_finishButton";
            this._finishButton.Size = new System.Drawing.Size(75, 23);
            this._finishButton.TabIndex = 2;
            this._finishButton.Text = "Finish";
            this._finishButton.UseVisualStyleBackColor = true;
            this._finishButton.Click += new System.EventHandler(this._finishButton_Click);
            // 
            // _headerPanel
            // 
            this._headerPanel.BackColor = System.Drawing.Color.White;
            this._headerPanel.Controls.Add(this._headerSeparatorPanel);
            this._headerPanel.Controls.Add(this._headerPictureBox);
            this._headerPanel.Controls.Add(this._headerInfoLabel);
            this._headerPanel.Controls.Add(this._headerTitleLabel);
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Margin = new System.Windows.Forms.Padding(0);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(617, 57);
            this._headerPanel.TabIndex = 2;
            // 
            // _headerSeparatorPanel
            // 
            this._headerSeparatorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._headerSeparatorPanel.BackColor = System.Drawing.Color.DarkGray;
            this._headerSeparatorPanel.Location = new System.Drawing.Point(0, 55);
            this._headerSeparatorPanel.Name = "_headerSeparatorPanel";
            this._headerSeparatorPanel.Size = new System.Drawing.Size(617, 1);
            this._headerSeparatorPanel.TabIndex = 0;
            // 
            // _headerPictureBox
            // 
            this._headerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._headerPictureBox.Location = new System.Drawing.Point(467, 0);
            this._headerPictureBox.Name = "_headerPictureBox";
            this._headerPictureBox.Size = new System.Drawing.Size(150, 57);
            this._headerPictureBox.TabIndex = 3;
            this._headerPictureBox.TabStop = false;
            // 
            // _headerInfoLabel
            // 
            this._headerInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._headerInfoLabel.Location = new System.Drawing.Point(21, 24);
            this._headerInfoLabel.Name = "_headerInfoLabel";
            this._headerInfoLabel.Size = new System.Drawing.Size(445, 30);
            this._headerInfoLabel.TabIndex = 2;
            this._headerInfoLabel.Text = "Information";
            // 
            // _pagesPanel
            // 
            this._pagesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pagesPanel.Location = new System.Drawing.Point(6, 63);
            this._pagesPanel.Margin = new System.Windows.Forms.Padding(6);
            this._pagesPanel.Name = "_pagesPanel";
            this._pagesPanel.Size = new System.Drawing.Size(605, 309);
            this._pagesPanel.TabIndex = 3;
            // 
            // WizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(617, 412);
            this.Controls.Add(this._wizardLayoutPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "WizardForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wizard";
            this.Load += new System.EventHandler(this.WizardForm_Load);
            this._wizardLayoutPanel.ResumeLayout(false);
            this._buttonsPanel.ResumeLayout(false);
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._headerPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _wizardLayoutPanel;
        private System.Windows.Forms.Panel _buttonsPanel;
        private System.Windows.Forms.Button _nextButton;
        private System.Windows.Forms.Button _backButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _finishButton;
        private System.Windows.Forms.Panel _headerPanel;
        private System.Windows.Forms.Panel _headerSeparatorPanel;
        private System.Windows.Forms.Label _headerInfoLabel;
        private System.Windows.Forms.PictureBox _headerPictureBox;
        private System.Windows.Forms.Panel _pagesPanel;
        private System.Windows.Forms.Label _headerTitleLabel;
    }
}