/*
 *	Copyright (C) 2007-2013 ARGUS TV
 *	http://www.argus-tv.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter.ClickFinder
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();
        }

        #region Button Events

        private void _browseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _tvUpToDatePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void _testConnectionButton_Click(object sender, EventArgs e)
        {
            if (_connectionStringTextBox.Text.Length > 0)
            {
                try
                {
                    if (DbReader.IsValidConnectionString(_connectionStringTextBox.Text))
                    {
                        MessageBox.Show("Your connectionstring is valid.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Your connectionstring is NOT valid !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Exception : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a connectionString !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _defaultButton_Click(object sender, EventArgs e)
        {
            ConfigInstance.LoadDefault();
            ShowCurrentConfig();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            if (DoValidation())
            {
                ConfigInstance.Current.PluginName = _pluginNameTextBox.Text;
                ConfigInstance.Current.TvUptodatePath = _tvUpToDatePathTextBox.Text;
                ConfigInstance.Current.ClickFinderConnectionString = _connectionStringTextBox.Text;
                ConfigInstance.Current.LaunchClickFinderBeforeImport = _launchClickFinderBeforeImportCheckBox.Checked;
                ConfigInstance.Current.UseShortDescription = _shortDescriptionRadioButton.Checked;
                ConfigInstance.Save();

                this.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("http://www.tvmovie.de");
            Process.Start(processStartInfo);
        }
        #endregion

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            ShowCurrentConfig();
        }

        private void ShowCurrentConfig()
        {
            _pluginNameTextBox.Text = ConfigInstance.Current.PluginName;
            _tvUpToDatePathTextBox.Text = ConfigInstance.Current.TvUptodatePath;
            _connectionStringTextBox.Text = ConfigInstance.Current.ClickFinderConnectionString;
            _launchClickFinderBeforeImportCheckBox.Checked = ConfigInstance.Current.LaunchClickFinderBeforeImport;
            _shortDescriptionRadioButton.Checked = ConfigInstance.Current.UseShortDescription;
            _longDescriptionRadioButton.Checked = !ConfigInstance.Current.UseShortDescription;
        }

        #region Validation

        private bool DoValidation()
        { 
            // Path to TvUpToDate.exe
            if (_tvUpToDatePathTextBox.Text.Length == 0)
            {
                _errorProvider.SetError(_tvUpToDatePathTextBox, "Please enter the full path to clickFinders tvUptodate.exe");
                return false;
            }
            if (!File.Exists(_tvUpToDatePathTextBox.Text))
            {
                _errorProvider.SetError(_tvUpToDatePathTextBox, "Invalid path to clickFinders tvUptodate.exe, file cannot be found");
                return false;
            }
            _errorProvider.SetError(_tvUpToDatePathTextBox, null);

            if (!DbReader.SafeIsValidConnectionString(_connectionStringTextBox.Text))
            {
                _errorProvider.SetError(_connectionStringTextBox, "Invalid connectionstring.");
                return false;            
            }
            return true;
        }

        private void _pluginNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_pluginNameTextBox, e, "Please enter a pluginName");
        }

        private void _tvUpToDatePathTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_tvUpToDatePathTextBox, e, "Please enter the path to TvUpToDate.exe");
        }

        private void _connectionStringTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_connectionStringTextBox, e, "Please enter a valid connection string.");
        }

        private void DoTextBoxValidation(TextBox textBox, CancelEventArgs e, string errorMessage)
        {
            if (textBox.Text.Length == 0)
            {
                _errorProvider.SetError(textBox, errorMessage);
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(textBox, null);
            }
        }
        #endregion
    }
}
