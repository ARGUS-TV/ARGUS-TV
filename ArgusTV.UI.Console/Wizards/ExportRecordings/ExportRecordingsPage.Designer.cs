namespace ArgusTV.UI.Console.Wizards.ExportRecordings
{
    partial class ExportRecordingsPage
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
            this.components = new System.ComponentModel.Container();
            this._recordingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._noneButton = new System.Windows.Forms.Button();
            this._allButton = new System.Windows.Forms.Button();
            this._recordingsTreeView = new ArgusTV.UI.Console.Controls.ThreeStateTreeView();
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _noneButton
            // 
            this._noneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._noneButton.Location = new System.Drawing.Point(81, 290);
            this._noneButton.Name = "_noneButton";
            this._noneButton.Size = new System.Drawing.Size(75, 23);
            this._noneButton.TabIndex = 15;
            this._noneButton.Text = "Select None";
            this._noneButton.UseVisualStyleBackColor = true;
            this._noneButton.Click += new System.EventHandler(this._noneButton_Click);
            // 
            // _allButton
            // 
            this._allButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._allButton.Location = new System.Drawing.Point(0, 290);
            this._allButton.Name = "_allButton";
            this._allButton.Size = new System.Drawing.Size(75, 23);
            this._allButton.TabIndex = 14;
            this._allButton.Text = "Select All";
            this._allButton.UseVisualStyleBackColor = true;
            this._allButton.Click += new System.EventHandler(this._allButton_Click);
            // 
            // _recordingsTreeView
            // 
            this._recordingsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._recordingsTreeView.HideSelection = false;
            this._recordingsTreeView.ItemHeight = 18;
            this._recordingsTreeView.Location = new System.Drawing.Point(0, 0);
            this._recordingsTreeView.Name = "_recordingsTreeView";
            this._recordingsTreeView.Size = new System.Drawing.Size(597, 286);
            this._recordingsTreeView.TabIndex = 0;
            this._recordingsTreeView.BeforeNodeStateChangeByUser += new System.EventHandler<System.Windows.Forms.TreeViewEventArgs>(this._recordingsTreeView_BeforeNodeStateChangeByUser);
            this._recordingsTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._recordingsTreeView_BeforeExpand);
            // 
            // ExportRecordingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._recordingsTreeView);
            this.Controls.Add(this._noneButton);
            this.Controls.Add(this._allButton);
            this.Name = "ExportRecordingsPage";
            this.Size = new System.Drawing.Size(597, 313);
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource _recordingsBindingSource;
        private System.Windows.Forms.Button _noneButton;
        private System.Windows.Forms.Button _allButton;
        private ArgusTV.UI.Console.Controls.ThreeStateTreeView _recordingsTreeView;
    }
}
