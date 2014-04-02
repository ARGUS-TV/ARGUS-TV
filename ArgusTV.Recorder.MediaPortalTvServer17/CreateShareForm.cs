using ArgusTV.Common.Recorders.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public partial class CreateShareForm : Form
    {
        public CreateShareForm()
        {
            InitializeComponent();
        }

        private string _localPath;

        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = value; }
        }

        private void CreateShareForm_Load(object sender, EventArgs e)
        {
            _localPathLabel.Text = _localPath;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (_shareNameTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show(this, "Invalid share name, don't use special characters.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (FileUtility.CreateUncShare(_shareNameTextBox.Text.Trim(), _localPath))
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(this, "Failed to create share, name not unique?", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
