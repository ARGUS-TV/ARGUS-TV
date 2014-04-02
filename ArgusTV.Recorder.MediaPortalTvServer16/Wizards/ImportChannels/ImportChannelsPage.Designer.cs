namespace ArgusTV.Recorder.MediaPortalTvServer.Wizards.ImportChannels
{
    partial class ImportChannelsPage
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
            this._recordingsTreeView = new ArgusTV.Recorder.MediaPortalTvServer.ThreeStateTreeView();
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).BeginInit();
            this.SuspendLayout();
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
            // ImportChannelsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._recordingsTreeView);
            this.Name = "ImportChannelsPage";
            this.Size = new System.Drawing.Size(597, 313);
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource _recordingsBindingSource;
        private ArgusTV.Recorder.MediaPortalTvServer.ThreeStateTreeView _recordingsTreeView;
    }
}
