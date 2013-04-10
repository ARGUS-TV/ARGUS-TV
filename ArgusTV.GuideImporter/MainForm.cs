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
using ArgusTV.GuideImporter.Interfaces;
using System.Threading;

namespace ArgusTV.GuideImporter
{
    internal partial class MainForm : Form
    {
        private enum FormMode { Normal, Importing, RefreshingChannels };

        #region Private Members

        private Hoster _hoster;
        private bool _needToSaveChanges;
        #endregion

        public MainForm(Hoster hoster)
        {
            _hoster = hoster;
            InitializeComponent();
        }

        #region Private Methods

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " " + ArgusTV.DataContracts.Constants.ProductVersion;

            SetFormMode(FormMode.Normal);
            FillPluginComboBox();
        }

        private void FillPluginComboBox()
        {
            _pluginComboBox.Items.Clear();
            foreach(string plugin in _hoster.PluginNames)
            {
                _pluginComboBox.Items.Add(plugin);
            }
            if (_pluginComboBox.Items.Count > 0)
            {
                _pluginComboBox.SelectedIndex = 0;
            }
            DoButtonEnabling();
        }

        private void FillChannelListBoxes(bool forceChannelsReload)
        {
            _availableChannelsListBox.Items.Clear();
            _channelsToSkipListBox.Items.Clear();

            if (_pluginComboBox.SelectedIndex >= 0)
            {
                if (SelectedPlugin != null && SelectedPlugin.IsConfigured() )
                {
                    try
                    {
                        List<ImportGuideChannel> guideChannels = SelectedPlugin.GetAllImportChannels(forceChannelsReload, ProgressUpdate, FeedbackUpdate);
                        foreach (ImportGuideChannel guideChannel in guideChannels)
                        {
                            if (!SelectedPluginSetting.ChannelsToSkip.Contains(guideChannel))
                            {
                                _availableChannelsListBox.Items.Add(guideChannel);
                            }
                            else
                            { 
                                // if the ChannelName format was changed, make sure that ChannelNames in ChannelsToSkip reflect this change
                                ImportGuideChannel importGuideChannel = SelectedPluginSetting.ChannelsToSkip.Find(a => a.ExternalId == guideChannel.ExternalId);
                                if (importGuideChannel != null && !importGuideChannel.ChannelName.Equals(guideChannel.ChannelName))
                                {
                                    importGuideChannel.ChannelName = guideChannel.ChannelName;
                                    _needToSaveChanges = true;
                                }

                            }
                        }

                        foreach (ImportGuideChannel channel in SelectedPluginSetting.ChannelsToSkip)
                        {
                            _channelsToSkipListBox.Items.Add(channel);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex, "Import error");
                    }
                }                        
            }
        }

        private void DoButtonEnabling()
        {
            bool hasAvailableChannels = _availableChannelsListBox.Items.Count > 0;
            bool hasAvailableChannelSelected = hasAvailableChannels && _availableChannelsListBox.SelectedIndices.Count > 0;
            bool hasChannelsToSkip = _channelsToSkipListBox.Items.Count > 0;
            bool hasChannelsToSkipSelected = hasChannelsToSkip && _channelsToSkipListBox.SelectedItems.Count > 0;
            bool hasSelectedPlugin = SelectedPlugin != null;
            bool hasConfiguredPlugin = SelectedPlugin != null && SelectedPlugin.IsConfigured();

            _addChannelButton.Enabled = hasAvailableChannelSelected;
            _removeChannelButton.Enabled = hasChannelsToSkipSelected;
            _addAllChannelButton.Enabled = hasAvailableChannels;
            _removeAllChannelButton.Enabled = hasChannelsToSkip;
            _importButton.Enabled = hasSelectedPlugin && hasAvailableChannels;
            _configureButton.Enabled = hasSelectedPlugin;
            _refreshButton.Enabled = hasConfiguredPlugin;
            _saveButton.Enabled = _needToSaveChanges;
        }

        private void DisableAllButtons()
        {
            _addChannelButton.Enabled = false;
            _removeChannelButton.Enabled = false;
            _addAllChannelButton.Enabled = false;
            _removeAllChannelButton.Enabled = false;

            _importButton.Enabled = false;
            _refreshButton.Enabled = false;
            _saveButton.Enabled = false;
            _configureButton.Enabled = false;
        }

        private void ClearControls()
        {
            _availableChannelsListBox.Items.Clear();
            _channelsToSkipListBox.Items.Clear();
            _pluginComboBox.Items.Clear();            
        }

        private void SetFormMode(FormMode formMode)
        {
            _descriptionLabel.Visible = formMode == FormMode.Normal;
            _descriptionTextBox.Visible = formMode == FormMode.Normal;
            _importProgressBar.Visible = (formMode == FormMode.Importing || formMode == FormMode.RefreshingChannels);
            _feedBackLabel.Visible = (formMode == FormMode.Importing || formMode == FormMode.RefreshingChannels);
        }

        private void CheckNeedToSaveChanges()
        {
            CheckNeedToSaveChanges(false);
        }

        private void CheckNeedToSaveChanges(bool forceSave)
        {
            if (forceSave ||
                (_needToSaveChanges && MessageBox.Show("Do you want to save your changes ?", "Save changes", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                _hoster.SavePluginSettings();
                _needToSaveChanges = false;
            }        
        }

        private IGuideImportPlugin SelectedPlugin
        {
            get
            {
                if (_pluginComboBox.SelectedItem != null)
                {
                    return _hoster.GetPluginByName(_pluginComboBox.SelectedItem.ToString());
                }
                return null;
            }

        }

        private PluginSetting SelectedPluginSetting
        {
            get
            {
                if (_pluginComboBox.SelectedItem != null)
                {
                    return _hoster.GetPluginSetting(_pluginComboBox.SelectedItem.ToString());
                }
                return null;
            }

        }
        #endregion

        #region Event Handlers

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            if (!SelectedPlugin.IsConfigured())
            {
                MessageBox.Show("Please complete the plugin configuration of the selected plugin first.", "Incompletely configured plugin", MessageBoxButtons.OK);
                return;
            }
            SetFormMode(FormMode.RefreshingChannels);
            DisableAllButtons();
            try
            {
                _needToSaveChanges = false;
                FillChannelListBoxes(true);
            }
            catch (Exception ex)
            {
                ShowException(ex, "Error refreshing channels");
            }
            finally
            {
                SetFormMode(FormMode.Normal);
                DoButtonEnabling();
            }
        }

        private void _addChannelButton_Click(object sender, EventArgs e)
        {
            foreach (ImportGuideChannel channelToSkip in _availableChannelsListBox.SelectedItems)
            {
                if (!SelectedPluginSetting.ChannelsToSkip.Contains(channelToSkip))
                {
                    SelectedPluginSetting.ChannelsToSkip.Add(channelToSkip);
                    _needToSaveChanges = true;
                }
            }
            FillChannelListBoxes(false);
            DoButtonEnabling();
        }

        private void _removeChannelButton_Click(object sender, EventArgs e)
        {
            foreach (ImportGuideChannel channelAvailable in _channelsToSkipListBox.SelectedItems)
            {
                if (SelectedPluginSetting.ChannelsToSkip.Contains(channelAvailable))
                {
                    SelectedPluginSetting.ChannelsToSkip.Remove(channelAvailable);
                    _needToSaveChanges = true;
                }
            }
            FillChannelListBoxes(false);
            DoButtonEnabling();
        }

        private void _addAllChannelButton_Click(object sender, EventArgs e)
        {
            foreach (ImportGuideChannel channelToSkip in _availableChannelsListBox.Items)
            {
                if (!SelectedPluginSetting.ChannelsToSkip.Contains(channelToSkip))
                {
                    SelectedPluginSetting.ChannelsToSkip.Add(channelToSkip);
                    _needToSaveChanges = true;
                }
            }
            FillChannelListBoxes(false);
            DoButtonEnabling();
        }

        private void _removeAllChannelButton_Click(object sender, EventArgs e)
        {
            foreach (ImportGuideChannel channelAvailable in _channelsToSkipListBox.Items)
            {
                if (SelectedPluginSetting.ChannelsToSkip.Contains(channelAvailable))
                {
                    SelectedPluginSetting.ChannelsToSkip.Remove(channelAvailable);
                    _needToSaveChanges = true;
                }              
            }
            FillChannelListBoxes(false);
            DoButtonEnabling();
        }

        private void _pluginComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckNeedToSaveChanges();
            _descriptionTextBox.Text = SelectedPlugin.Description;
            FillChannelListBoxes(false);
            DoButtonEnabling();
        }

        private void _availableChannelsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoButtonEnabling();
        }

        private void _channelsToSkipListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoButtonEnabling();
        }

        private void _importButton_Click(object sender, EventArgs e)
        {
            CheckNeedToSaveChanges();
            if (!SelectedPlugin.IsConfigured())
            {
                MessageBox.Show("Please complete the plugin configuration of the selected plugin first.", "Incompletely configured plugin", MessageBoxButtons.OK);
                return;
            }
            
            SetFormMode(FormMode.Importing);
            DisableAllButtons();
            try
            {
                _importProgressBar.Value = 0;
                _feedBackLabel.Text = String.Empty;
                _hoster.Import(SelectedPlugin.Name, ProgressUpdate, FeedbackUpdate);
            }
            catch (Exception ex)
            {
                ShowException(ex, "Import error");
            }
            finally
            {
                Thread.Sleep(500);
                SetFormMode(FormMode.Normal);
                DoButtonEnabling();
            }
        }

        private void _configureButton_Click(object sender, EventArgs e)
        {
            CheckNeedToSaveChanges();
            if (SelectedPlugin != null)
            {
                string selectedPluginName = SelectedPlugin.Name;
                SelectedPlugin.ShowConfigurationDialog(this);                
                _hoster.ReloadPlugins();
                FillPluginComboBox();
                _pluginComboBox.SelectedItem = selectedPluginName;
            }
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            CheckNeedToSaveChanges(true);
            DoButtonEnabling();
        }

        private void ShowException(Exception ex, string title)
        {
            MessageBox.Show(String.Format("The following error occured : {0} ", ex.Message), title, MessageBoxButtons.OK);                                        
        }
        #endregion

        #region Public Methods

        public void ProgressUpdate(int percentage)
        {
            _importProgressBar.Value = percentage;
            _importProgressBar.Update();
            Application.DoEvents();
        }

        public void FeedbackUpdate(string message)
        {
            _feedBackLabel.Text = message;
            Application.DoEvents();            
        }
        #endregion
    }
}
