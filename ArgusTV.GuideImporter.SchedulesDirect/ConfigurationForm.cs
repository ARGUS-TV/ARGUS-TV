/*
 *	Copyright (C) 2007-2014 ARGUS TV
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

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            ConfigInstance.Current.PluginName = _pluginNameTextBox.Text;
            ConfigInstance.Current.SDUserName = _sdUserNameTextBox.Text;
            ConfigInstance.Current.SDPassword = _sdPasswordTextBox.Text;
            if (_channelNameFormat.SelectedItem != null)
            {
                ConfigInstance.Current.ChannelNameFormat = _channelNameFormat.SelectedItem.ToString();
            }
            else
            {
                ConfigInstance.Current.ChannelNameFormat = _channelNameFormat.Text;
            }
            ConfigInstance.Current.NrOfDaysToImport = (int)_nrDaysNumericUpDown.Value;
            ConfigInstance.Current.ProxyUserName = _proxyUserNameTextBox.Text;
            ConfigInstance.Current.ProxyPassword = _proxyPasswordTextBox.Text;
            if (_updatesAllDays.Checked)
            {
                ConfigInstance.Current.UpdateMode = Config.UpDateMode.AllDays;
            }
            else
            {
                ConfigInstance.Current.UpdateMode = Config.UpDateMode.NextDayOnly;            
            }
            ConfigInstance.Current.UpdateChannelNames = _updateChannelNamesCheckBox.Checked;            
            ConfigInstance.Save();

            this.Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _channelNameFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool shouldWarn = _channelNameFormat.SelectedItem.ToString() != ConfigInstance.Current.ChannelNameFormat;
            _warningLabel.Visible = shouldWarn;
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            _pluginNameTextBox.Text = ConfigInstance.Current.PluginName;
            _sdUserNameTextBox.Text = ConfigInstance.Current.SDUserName;
            _sdPasswordTextBox.Text = ConfigInstance.Current.SDPassword;
            _nrDaysNumericUpDown.Value = (decimal)ConfigInstance.Current.NrOfDaysToImport;
            _proxyUserNameTextBox.Text = ConfigInstance.Current.ProxyUserName;
            _proxyPasswordTextBox.Text = ConfigInstance.Current.ProxyPassword;

            // ChannelNameFormat
            bool selectionIsSpecificItem = true;
            foreach (string format in ConfigInstance.Current.ChannelNameFormats)
            {
                _channelNameFormat.Items.Add(format);
                if (format.Equals(ConfigInstance.Current.ChannelNameFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    selectionIsSpecificItem = false;
                }
            }
            if (selectionIsSpecificItem)
            {
                _channelNameFormat.Items.Add(ConfigInstance.Current.ChannelNameFormat);
            }
            _channelNameFormat.SelectedItem = ConfigInstance.Current.ChannelNameFormat;

            // UpdateMode
            if (ConfigInstance.Current.UpdateMode == Config.UpDateMode.AllDays)
            {
                _updatesAllDays.Checked = true;
            }
            else
            {
                _updatesNextDayOnly.Checked = true;
            }
            _updateChannelNamesCheckBox.Checked = ConfigInstance.Current.UpdateChannelNames;


            _warningLabel.Visible = false;
        }

        #region Validation

        private void _pluginNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_pluginNameTextBox, e, "Please enter a pluginName");
        }

        private void _sdUserNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_sdUserNameTextBox, e, "Please enter your schedulesDirect username");
        }

        private void _sdPasswordTextBox_Validating(object sender, CancelEventArgs e)
        {
            DoTextBoxValidation(_sdPasswordTextBox, e, "Please enter your schedulesDirect password");
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
