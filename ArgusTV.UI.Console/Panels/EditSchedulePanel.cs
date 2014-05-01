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
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using ArgusTV.UI.Process;
using ArgusTV.UI.Process.EditSchedule;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.Console.Panels
{
    public partial class EditSchedulePanel : ContentPanel
    {
        private const string _onDateLabelText = "On date:";
        private const string _fromDateLabelText = "From date:";

        private EditScheduleModel _model;
        private EditScheduleController _controller;

        private int _standardGeneralHeight;
        private const int _generalHeightNoCommandsDelta = 26;
        
        private DateTime _timePickerReferenceDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private int _allowedTimeFrame = 3 * 3600;
 
        public EditSchedulePanel()
        {
            InitializeComponent();
            _standardGeneralHeight = _generalGroupBox.Height;
        }

        public override string Title
        {
            get { return "Edit Schedule"; }
        }

        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        private bool _forceManualSchedule;

        public bool ForceManualSchedule
        {
            get { return _forceManualSchedule; }
            set { _forceManualSchedule = value; }
        }

        private bool _inPanelLoad;

        private void EditSchedulePanel_Load(object sender, EventArgs e)
        {
            _inPanelLoad = true;

            try
            {
                MainForm.SetMenuMode(MainMenuMode.SaveCancel);

                if (_schedule.ScheduleType == ScheduleType.Recording)
                {
                    SetGeneralGroupBoxHeight(_standardGeneralHeight);
                }
                else
                {
                    SetGeneralGroupBoxHeight(_standardGeneralHeight - _generalHeightNoCommandsDelta);
                }

                _model = new EditScheduleModel();
                _controller = new EditScheduleController(_model);
                _controller.Initialize(MainForm.GuideProxy, MainForm.SchedulerProxy, _schedule, _forceManualSchedule, "All Channels", "Default");

                if (_schedule.ScheduleType == ScheduleType.Recording)
                {
                    _formatsBindingSource.DataSource = _model.RecordingFormats;
                    _formatsBindingSource.ResetBindings(false);
                    _fileFormatComboBox.SelectedValue = _schedule.RecordingFileFormatId.HasValue ? _schedule.RecordingFileFormatId.Value : Guid.Empty;
                }

                _titleRuleTypeComboBox.SelectedIndex = TitleRuleTypeIndex.Equals;
                _subTitleRuleTypeComboBox.SelectedIndex = TitleRuleTypeIndex.Equals;
                _episodeNumberRuleTypeComboBox.SelectedIndex = TitleRuleTypeIndex.Equals;

                _manualScheduleGroupBox.Visible = _model.IsManual;
                _rulesTableLayoutPanel.Visible = !_model.IsManual;

                _schedulePriorityControl.SchedulePriority = _model.Schedule.SchedulePriority;

                _preRecDateTimePicker.MaxDate = _timePickerReferenceDate.AddSeconds(_allowedTimeFrame);
                _preRecDateTimePicker.MinDate = _timePickerReferenceDate;
                _postRecDateTimePicker.MaxDate = _timePickerReferenceDate.AddSeconds(_allowedTimeFrame);
                _postRecDateTimePicker.MinDate = _timePickerReferenceDate;

                if (_model.Schedule.PreRecordSeconds.HasValue)
                {
                    _preRecDateTimePicker.Value = _preRecDateTimePicker.MinDate.AddSeconds(Math.Min(_model.Schedule.PreRecordSeconds.Value, _allowedTimeFrame));
                    _preRecDateTimePicker.Checked = true;
                }
                else
                {
                    Utility.SetDateTimePickerValue(MainForm, _preRecDateTimePicker, ConfigurationKey.Scheduler.PreRecordsSeconds);
                    _preRecDateTimePicker.Checked = false;
                }

                if (_model.Schedule.PostRecordSeconds.HasValue)
                {
                    _postRecDateTimePicker.Value = _preRecDateTimePicker.MinDate.AddSeconds(Math.Min(_model.Schedule.PostRecordSeconds.Value, _allowedTimeFrame));
                    _postRecDateTimePicker.Checked = true;
                }
                else
                {
                    Utility.SetDateTimePickerValue(MainForm, _postRecDateTimePicker, ConfigurationKey.Scheduler.PostRecordsSeconds);
                    _postRecDateTimePicker.Checked = false;
                }

                UpdateKeepMethodControl();
                UpdateProcesingCommandsControl();

                if (_model.IsManual)
                {
                    _manualChannelGroupComboBox.DataSource = _model.ChannelGroups;
                    _manualChannelGroupComboBox.DisplayMember = "GroupName";
                    _manualChannelGroupComboBox.ValueMember = "ChannelGroupId";

                    _manualDatePicker.Value = DateTime.Today;
                    _manualTimePicker.Value = DateTime.Now;
                    _manualDurationDateTimePicker.Value = DateTime.Today.AddHours(1);
                }
                else
                {
                    _channelGroupCombobox.DataSource = _model.ChannelGroups;
                    _channelGroupCombobox.DisplayMember = "GroupName";
                    _channelGroupCombobox.ValueMember = "ChannelGroupId";

                    _aroundDateTimePicker.Value = DateTime.Today.AddHours(12);

                    foreach (string category in _model.Categories)
                    {
                        _categoryComboBox.Items.Add(category);
                    }

                    _betweenFromDateTimePicker.Value = DateTime.Today;
                    _betweenToDateTimePicker.Value = DateTime.Today.Add(new TimeSpan(23, 59, 0));
                }

                ShowRulesInUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _inPanelLoad = false;

            string name = _model.Schedule.Name;
            _upcomingProgramsControl.ScheduleType = _model.Schedule.ScheduleType;
            RefreshUpcoming();
            _nameTextBox.Text = name;
        }

        private void SetGeneralGroupBoxHeight(int newHeight)
        {
            if (_generalGroupBox.Height != newHeight)
            {
                int diff = newHeight - _generalGroupBox.Height;
                _manualScheduleGroupBox.Top += diff;
                _rulesTableLayoutPanel.Top += diff;
                _programsLabel.Top += diff;
                _upcomingProgramsControl.Top += diff;
                _upcomingProgramsControl.Height -= diff;
                _generalGroupBox.Height = newHeight;
                _processingTextBox.Visible = (diff > 0);
                _selectCommandsButton.Visible = (diff > 0);
                _fileFormatLabel.Visible = (diff > 0);
                _fileFormatComboBox.Visible = (diff > 0);
            }
        }

        public override void OnClosed()
        {
            MainForm.SetMenuMode(MainMenuMode.Normal);
            _upcomingProgramsControl.UpcomingPrograms = null;
        }

        private void UpdateKeepMethodControl()
        {
            if (_model.Schedule.ScheduleType == ScheduleType.Recording)
            {
                _keepUntilControl.SetKeepUntil(_model.Schedule.KeepUntilMode, _model.Schedule.KeepUntilValue);
            }
            else
            {
                _keepLabel.Visible = false;
                _keepUntilControl.Visible = false;
            }
        }

        private void UpdateProcesingCommandsControl()
        {
            _processingTextBox.Text = _model.ProcessingCommandsText;
        }

        [Obfuscation(Exclude = true)]
        private class ChannelListItem
        {
            private Channel _channel;

            public ChannelListItem(Channel channel)
            {
                _channel = channel;
            }

            public Guid ChannelId
            {
                get { return _channel == null ? Guid.Empty : _channel.ChannelId; }
            }

            public String ChannelDisplayName
            {
                get { return _channel == null ? String.Empty : _channel.DisplayName; }
            }

            public override string ToString()
            {
                return _channel == null ? String.Empty : _channel.DisplayName;
            }
        }

        private ChannelListItem FindChannelItem(ComboBox channelComboBox, Guid channelId)
        {
            foreach (ChannelListItem item in channelComboBox.Items)
            {
                if (item.ChannelId == channelId)
                {
                    return item;
                }
            }
            return null;
        }

        private void ShowRulesInUI()
        {
            _onDateDateTimePicker.Checked = false;
            _descriptionOrAnyComboBox.SelectedIndex = 0;

            int typeIndex;
            _titleTextBox.Text = _controller.GetTitleRuleText(out typeIndex);
            _titleRuleTypeComboBox.SelectedIndex = typeIndex;

            _subTitleTextBox.Text = _controller.GetSubTitleRuleText(out typeIndex);
            _subTitleRuleTypeComboBox.SelectedIndex = typeIndex;

            _episodeNumberTextBox.Text = _controller.GetEpisodeNumberRuleText(out typeIndex);
            _episodeNumberRuleTypeComboBox.SelectedIndex = typeIndex;

            bool isProgramInfo;
            _descriptionOrAnyTextBox.Text = _controller.GetDescriptionOrProgramInfoRuleText(out isProgramInfo);
            _descriptionOrAnyComboBox.SelectedIndex = isProgramInfo ? 0 : 1;

            foreach (ScheduleRule rule in _model.Schedule.Rules)
            {
                switch (rule.Type)
                {
                    case ScheduleRuleType.NotOnChannels:
                        if (!_manualScheduleGroupBox.Visible)
                        {
                            goto case ScheduleRuleType.Channels;
                        }
                        break;

                    case ScheduleRuleType.Channels:
                        _tvChannelsListBox.Items.Clear();
                        foreach (Guid channelId in rule.Arguments)
                        {
                            ChannelListItem item = FindChannelItem(_channelComboBox, channelId);
                            if (item == null
                                && _model.AllChannels.ContainsKey(channelId))
                            {
                                item = new ChannelListItem(_model.AllChannels[channelId]);
                            }
                            if (item != null)
                            {
                                _tvChannelsListBox.Items.Add(item);
                            }
                        }
                        if (_manualScheduleGroupBox.Visible
                            && rule.Arguments.Count > 0)
                        {
                            Guid channelId = (Guid)rule.Arguments[0];
                            SelectGroupWithChannel(_manualChannelGroupComboBox, channelId);
                            _manualChannelComboBox.SelectedItem = FindChannelItem(_manualChannelComboBox, channelId);
                        }
                        _notInChannelsRadioButton.Checked = (rule.Type == ScheduleRuleType.NotOnChannels);
                        break;

                    case ScheduleRuleType.OnDate:
                        _onDateLabel.Text = _onDateLabelText;
                        _manualDateTimeLabel.Text = _onDateLabelText;
                        _onDateDateTimePicker.Checked = true;
                        _onDateDateTimePicker.Value = (DateTime)rule.Arguments[0];
                        _scheduleDaysOfWeekControl.ScheduleDaysOfWeek = ScheduleDaysOfWeek.None;
                        break;

                    case ScheduleRuleType.DaysOfWeek:
                        _onDateLabel.Text = _fromDateLabelText;
                        _manualDateTimeLabel.Text = _fromDateLabelText;
                        ScheduleDaysOfWeek scheduleDaysOfWeek = (ScheduleDaysOfWeek)rule.Arguments[0];
                        _scheduleDaysOfWeekControl.ScheduleDaysOfWeek = scheduleDaysOfWeek;
                        _manualScheduleDaysOfWeekControl.ScheduleDaysOfWeek = scheduleDaysOfWeek;
                        if (rule.Arguments.Count > 1)
                        {
                            _onDateDateTimePicker.Checked = true;
                            _onDateDateTimePicker.Value = (DateTime)rule.Arguments[1];
                        }
                        else
                        {
                            _onDateDateTimePicker.Checked = false;
                            _onDateDateTimePicker.Value = DateTime.Today;
                        }
                        break;

                    case ScheduleRuleType.AroundTime:
                        _aroundCheckBox.Checked = true;
                        ScheduleTime aroundTime = (ScheduleTime)rule.Arguments[0];
                        _aroundDateTimePicker.Value = DateTime.Today.AddTicks(aroundTime.Ticks);
                        break;

                    case ScheduleRuleType.StartingBetween:
                        _betweenCheckBox.Checked = true;
                        ScheduleTime fromTime = (ScheduleTime)rule.Arguments[0];
                        ScheduleTime toTime = (ScheduleTime)rule.Arguments[1];
                        _betweenFromDateTimePicker.Value = DateTime.Today.AddTicks(fromTime.Ticks);
                        _betweenToDateTimePicker.Value = DateTime.Today.AddTicks(toTime.Ticks);
                        break;

                    case ScheduleRuleType.CategoryEquals:
                    case ScheduleRuleType.CategoryDoesNotEqual:
                        foreach (string category in rule.Arguments)
                        {
                            _categoriesListBox.Items.Add(category);
                        }
                        _notInCategoriesRadioButton.Checked = (rule.Type == ScheduleRuleType.CategoryDoesNotEqual);
                        break;

                    case ScheduleRuleType.DirectedBy:
                        foreach (string director in rule.Arguments)
                        {
                            _directorsListBox.Items.Add(director);
                        }
                        break;

                    case ScheduleRuleType.WithActor:
                        foreach (string actor in rule.Arguments)
                        {
                            _actorsListBox.Items.Add(actor);
                        }
                        break;

                    case ScheduleRuleType.NewEpisodesOnly:
                        _newEpisodesOnlyCheckBox.Checked = (bool)rule.Arguments[0];
                        break;

                    case ScheduleRuleType.NewTitlesOnly:
                        _newTitlesOnlyCheckBox.Checked = (bool)rule.Arguments[0];
                        break;

                    case ScheduleRuleType.SkipRepeats:
                        _skipRepeatsCheckBox.Checked = (bool)rule.Arguments[0];
                        break;

                    case ScheduleRuleType.ManualSchedule:
                        DateTime manualDateTime = (DateTime)rule.Arguments[0];
                        ScheduleTime manualDuration = (ScheduleTime)rule.Arguments[1];
                        _manualDatePicker.Value = manualDateTime.Date;
                        _manualTimePicker.Value = DateTime.Today.Add(manualDateTime.TimeOfDay);
                        _manualDurationDateTimePicker.Value = DateTime.Today.AddTicks(manualDuration.Ticks);
                        break;
                }
            }

            RefreshListBoxAllLabels();
        }

        private void SelectGroupWithChannel(ComboBox groupComboBox, Guid channelId)
        {
            try
            {
                foreach (ChannelGroup channelGroup in groupComboBox.Items)
                {
                    _controller.EnsureChannelsByGroup(MainForm.SchedulerProxy, channelGroup.ChannelGroupId);
                    foreach (Channel channel in _model.ChannelsByGroup[channelGroup.ChannelGroupId])
                    {
                        if (channel.ChannelId == channelId)
                        {
                            groupComboBox.SelectedItem = channelGroup;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                groupComboBox.SelectedItem = null;
            }
        }

        private void UpdateRules()
        {
            if (_model.IsManual)
            {
                ChannelListItem channelItem = _manualChannelComboBox.SelectedItem as ChannelListItem;
                if (channelItem != null 
                    && channelItem.ChannelId != Guid.Empty
                    && (_manualDurationDateTimePicker.Value.Hour != 0 || _manualDurationDateTimePicker.Value.Minute != 0))
                {
                    DateTime startTime = _manualDatePicker.Value.Date.Add(
                        new TimeSpan(_manualTimePicker.Value.Hour, _manualTimePicker.Value.Minute, 0));
                    TimeSpan duration =
                        new TimeSpan(_manualDurationDateTimePicker.Value.Hour, _manualDurationDateTimePicker.Value.Minute, 0);

                    ScheduleDaysOfWeek daysOfWeek = _manualScheduleDaysOfWeekControl.ScheduleDaysOfWeek;
                    _controller.UpdateManualSchedule(channelItem.ChannelId, channelItem.ChannelDisplayName, startTime, duration, daysOfWeek);
                    _nameTextBox.Text = _model.Schedule.Name;
                }
            }
            else
            {
                List<ScheduleRule> rules = new List<ScheduleRule>();

                _controller.AppendTitleRule(rules, _titleRuleTypeComboBox.SelectedIndex, _titleTextBox.Text);
                _controller.AppendSubTitleRule(rules, _subTitleRuleTypeComboBox.SelectedIndex, _subTitleTextBox.Text);
                _controller.AppendEpisodeNumberRule(rules, _episodeNumberRuleTypeComboBox.SelectedIndex, _episodeNumberTextBox.Text);

                if (_descriptionOrAnyComboBox.SelectedIndex == 1)
                {
                    _controller.AppendDescriptionRule(rules, _descriptionOrAnyTextBox.Text);
                }
                else
                {
                    _controller.AppendProgramInfoRule(rules, _descriptionOrAnyTextBox.Text);
                }
                _controller.AppendOnDateAndDaysOfWeekRule(rules, _scheduleDaysOfWeekControl.ScheduleDaysOfWeek,
                    _onDateDateTimePicker.Checked ? _onDateDateTimePicker.Value : (DateTime?)null);
                _controller.AppendAroundTimeRule(rules, _aroundCheckBox.Checked ? _aroundDateTimePicker.Value : (DateTime?)null);
                _controller.AppendStartingBetweenRule(rules, _betweenCheckBox.Checked, _betweenFromDateTimePicker.Value, _betweenToDateTimePicker.Value);
                _controller.AppendCategoriesRule(rules, _notInCategoriesRadioButton.Checked, _categoriesListBox.Items);
                _controller.AppendDirectedByRule(rules, _directorsListBox.Items);
                _controller.AppendWithActorRule(rules, _actorsListBox.Items);
                _controller.AppendNewEpisodesOnlyRule(rules, _newEpisodesOnlyCheckBox.Checked);
                _controller.AppendNewTitlesOnlyRule(rules, _newTitlesOnlyCheckBox.Checked);
                _controller.AppendSkipRepeatsRule(rules, _skipRepeatsCheckBox.Checked);

                List<Guid> channelIds = new List<Guid>();
                foreach (ChannelListItem item in _tvChannelsListBox.Items)
                {
                    channelIds.Add(item.ChannelId);
                }
                _controller.AppendChannelsRule(rules, _notInChannelsRadioButton.Checked, channelIds);

                _model.Schedule.Rules = rules;
            }
        }

        private void RefreshUpcoming()
        {
            if (_model.Schedule != null
                && !_inPanelLoad)
            {
                RefreshListBoxAllLabels();
                UpdateRules();

                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _controller.RefreshUpcomingPrograms(MainForm.SchedulerProxy);
                    RefreshPrograms(_model.UpcomingPrograms);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void RefreshPrograms(IEnumerable<UpcomingProgram> upcomingPrograms)
        {
            _upcomingProgramsControl.UpcomingPrograms = new UpcomingOrActiveProgramsList(upcomingPrograms);
        }

        private void RefreshListBoxAllLabels()
        {
            _allChannelsLabel.Visible = (_tvChannelsListBox.Items.Count == 0);
            _allCategoriesLabel.Visible = (_categoriesListBox.Items.Count == 0);
        }

        public override void OnSave()
        {
            // For manual schedules, be sure we read the name first.
            string name = _nameTextBox.Text.Trim();

            UpdateRules();

            if (_model.Schedule.Rules.Count == 0)
            {
                MessageBox.Show(this, "No rules are defined.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (name.Length == 0)
            {
                MessageBox.Show(this, "You must enter a name for the schedule.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                _model.Schedule.Name = name;
                _model.Schedule.SchedulePriority = _schedulePriorityControl.SchedulePriority;
                _model.Schedule.PreRecordSeconds = _preRecDateTimePicker.Checked ?
                    (int?)_preRecDateTimePicker.Value.TimeOfDay.TotalSeconds : null;
                _model.Schedule.PostRecordSeconds = _postRecDateTimePicker.Checked ?
                    (int?)_postRecDateTimePicker.Value.TimeOfDay.TotalSeconds : null;

                if (_fileFormatComboBox.Visible)
                {
                    Guid formatId = (Guid)_fileFormatComboBox.SelectedValue;
                    _schedule.RecordingFileFormatId = (formatId == Guid.Empty) ? null : (Guid?)formatId;
                }

                if (_keepUntilControl.Visible)
                {
                    _model.Schedule.KeepUntilMode = _keepUntilControl.KeepUntilMode;
                    _model.Schedule.KeepUntilValue = _keepUntilControl.KeepUntilValue;
                }

                try
                {
                    _controller.SaveSchedule(MainForm.SchedulerProxy);
                    ClosePanel(DialogResult.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _historyButton_Click(object sender, EventArgs e)
        {
            ScheduleHistoryPanel panel = new ScheduleHistoryPanel();
            panel.Schedule = _model.Schedule;
            panel.OpenPanel(this);
        }

        private void _titleRuleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _subTitleRuleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }
        
        private void _episodeNumberRuleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _titleTextBox_Leave(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _subTitleTextBox_Leave(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _episodeNumberTextBox_Leave(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _descriptionTextBox_Leave(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _channelsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetRadioButtonStrikeout(_inChannelsRadioButton);
            SetRadioButtonStrikeout(_notInChannelsRadioButton);
            RefreshUpcoming();
        }

        private void _categoriesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetRadioButtonStrikeout(_inCategoriesRadioButton);
            SetRadioButtonStrikeout(_notInCategoriesRadioButton);
            RefreshUpcoming();
        }

        private void SetRadioButtonStrikeout(RadioButton radioButton)
        {
            radioButton.Font = new Font(radioButton.Font, radioButton.Checked ? FontStyle.Regular : FontStyle.Strikeout);
        }

        private void _scheduleDaysOfWeekControl_ScheduleDaysOfWeekChanged(object sender, EventArgs e)
        {
            if (_scheduleDaysOfWeekControl.ScheduleDaysOfWeek == ScheduleDaysOfWeek.None)
            {
                _onDateLabel.Text = _onDateLabelText;
            }
            else
            {
                _onDateLabel.Text = _fromDateLabelText;
            }
            RefreshUpcoming();
        }

        private void _betweenDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _channelGroupCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChannelsComboBoxForGroup(_channelGroupCombobox, _channelComboBox, false);
        }

        private void _manualChannelGroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChannelsComboBoxForGroup(_manualChannelGroupComboBox, _manualChannelComboBox, true);
            RefreshUpcoming();
        }

        private void UpdateChannelsComboBoxForGroup(ComboBox channelGroupCombobox, ComboBox channelComboBox, bool addEmptyItem)
        {
            try
            {
                ChannelGroup channelGroup = channelGroupCombobox.SelectedItem as ChannelGroup;
                Guid channelGroupId = channelGroup.ChannelGroupId;
                _controller.EnsureChannelsByGroup(MainForm.SchedulerProxy, channelGroupId);
                channelComboBox.Items.Clear();
                if (addEmptyItem)
                {
                    channelComboBox.Items.Add(new ChannelListItem(null));
                }
                foreach (Channel channel in _model.ChannelsByGroup[channelGroupId])
                {
                    channelComboBox.Items.Add(new ChannelListItem(channel));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                channelComboBox.Items.Clear();
            }
        }

        private void _addChannelButton_Click(object sender, EventArgs e)
        {
            ChannelListItem item = _channelComboBox.SelectedItem as ChannelListItem;
            if (item != null)
            {
                if (!_tvChannelsListBox.Items.Contains(item))
                {
                    _tvChannelsListBox.Items.Add(item);
                    _channelComboBox.SelectedIndex = -1;
                    RefreshUpcoming();
                }
            }
        }

        private void _addAllChannelsButton_Click(object sender, EventArgs e)
        {
            foreach (ChannelListItem item in _channelComboBox.Items)
            {
                if (!_tvChannelsListBox.Items.Contains(item))
                {
                    _tvChannelsListBox.Items.Add(item);
                }
            }
            _channelComboBox.SelectedIndex = -1;
            RefreshUpcoming();
        }

        private void _removeChannelButton_Click(object sender, EventArgs e)
        {
            if (_tvChannelsListBox.SelectedItems.Count > 0)
            {
                ArrayList selectedItems = new ArrayList(_tvChannelsListBox.SelectedItems);
                foreach (ChannelListItem item in selectedItems)
                {
                    _tvChannelsListBox.Items.Remove(item);
                }
                RefreshUpcoming();
            }
        }

        private void _addCategoryButton_Click(object sender, EventArgs e)
        {
            string category = _categoryComboBox.Text.Trim();
            if (!String.IsNullOrEmpty(category))
            {
                if (!_categoriesListBox.Items.Contains(category))
                {
                    _categoriesListBox.Items.Add(category);
                    _categoryComboBox.Text = String.Empty;
                    RefreshUpcoming();
                }
            }
        }

        private void _addAllCategoriesButton_Click(object sender, EventArgs e)
        {
            foreach (string category in _categoryComboBox.Items)
            {
                if (!_categoriesListBox.Items.Contains(category))
                {
                    _categoriesListBox.Items.Add(category);
                    _categoryComboBox.Text = String.Empty;
                }
            }
            RefreshUpcoming();
        }

        private void _removeCategoryButton_Click(object sender, EventArgs e)
        {
            if (_categoriesListBox.SelectedItems.Count > 0)
            {
                ArrayList selectedCategories = new ArrayList(_categoriesListBox.SelectedItems);
                foreach (string category in selectedCategories)
                {
                    _categoriesListBox.Items.Remove(category);
                }
                RefreshUpcoming();
            }
        }

        private void _addDirectorButton_Click(object sender, EventArgs e)
        {
            string director = _directorTextBox.Text.Trim();
            if (!String.IsNullOrEmpty(director))
            {
                if (!_directorsListBox.Items.Contains(director))
                {
                    _directorsListBox.Items.Add(director);
                    _directorTextBox.Text = String.Empty;
                    RefreshUpcoming();
                }
            }
        }

        private void _removeDirectorButton_Click(object sender, EventArgs e)
        {
            if (_directorsListBox.SelectedItem != null)
            {
                _directorsListBox.Items.Remove(_directorsListBox.SelectedItem);
                RefreshUpcoming();
            }
        }

        private void _addActorButton_Click(object sender, EventArgs e)
        {
            string actor = _actorTextBox.Text.Trim();
            if (!String.IsNullOrEmpty(actor))
            {
                if (!_actorsListBox.Items.Contains(actor))
                {
                    _actorsListBox.Items.Add(actor);
                    _actorTextBox.Text = String.Empty;
                    RefreshUpcoming();
                }
            }
        }

        private void _removeActorButton_Click(object sender, EventArgs e)
        {
            if (_actorsListBox.SelectedItem != null)
            {
                _actorsListBox.Items.Remove(_actorsListBox.SelectedItem);
                RefreshUpcoming();
            }
        }

        private void _onDateDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _aroundDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _manualScheduleDaysOfWeekControl_ScheduleDaysOfWeekChanged(object sender, EventArgs e)
        {
            if (_manualScheduleDaysOfWeekControl.ScheduleDaysOfWeek == ScheduleDaysOfWeek.None)
            {
                _manualDateTimeLabel.Text = _onDateLabelText;
            }
            else
            {
                _manualDateTimeLabel.Text = _fromDateLabelText;
            }
            RefreshUpcoming();
        }

        private void _manualDurationDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _manualTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _manualChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _manualDatePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _skipRepeatsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private void _descriptionOrAnyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcoming();
        }

        private bool _inNewEpisodeOrTitleCheckedChanged;

        private void _newEpisodesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_inNewEpisodeOrTitleCheckedChanged)
            {
                try
                {
                    _inNewEpisodeOrTitleCheckedChanged = true;
                    _newTitlesOnlyCheckBox.Checked = false;
                    RefreshUpcoming();
                }
                finally
                {
                    _inNewEpisodeOrTitleCheckedChanged = false;
                }
            }
        }

        private void _newTitlesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_inNewEpisodeOrTitleCheckedChanged)
            {
                try
                {
                    _inNewEpisodeOrTitleCheckedChanged = true;
                    _newEpisodesOnlyCheckBox.Checked = false;
                    RefreshUpcoming();
                }
                finally
                {
                    _inNewEpisodeOrTitleCheckedChanged = false;
                }
            }
        }

        private bool _inTimeCheckedChanging;

        private void _aroundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_inTimeCheckedChanging)
            {
                try
                {
                    _inTimeCheckedChanging = true;
                    _aroundDateTimePicker.Enabled = _aroundCheckBox.Checked;
                    _betweenFromDateTimePicker.Enabled = false;
                    _betweenToDateTimePicker.Enabled = false;
                    _betweenCheckBox.Checked = false;
                    RefreshUpcoming();
                }
                finally
                {
                    _inTimeCheckedChanging = false;
                }
            }
        }

        private void _betweenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_inTimeCheckedChanging)
            {
                try
                {
                    _inTimeCheckedChanging = true;
                    _betweenFromDateTimePicker.Enabled = _betweenCheckBox.Checked;
                    _betweenToDateTimePicker.Enabled = _betweenCheckBox.Checked;
                    _aroundDateTimePicker.Enabled = false;
                    _aroundCheckBox.Checked = false;
                    RefreshUpcoming();
                }
                finally
                {
                    _inTimeCheckedChanging = false;
                }
            }
        }

        private void _selectCommandsButton_Click(object sender, EventArgs e)
        {
            SelectProcessingCommandsForm form = new SelectProcessingCommandsForm();
            form.Schedule = _model.Schedule;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                UpdateProcesingCommandsControl();
            }
        }

        private void _titleHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowLogicHelp(_titleRuleTypeComboBox.SelectedIndex == TitleRuleTypeIndex.Contains);
        }

        private void _subTitleHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowLogicHelp(_subTitleRuleTypeComboBox.SelectedIndex == TitleRuleTypeIndex.Contains);
        }

        private void _episodeNumberHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowLogicHelp(false);
        }

        private void _programInfoHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowLogicHelp(true);
        }

        private void ShowLogicHelp(bool isContains)
        {
            if (isContains)
            {
                MessageBox.Show(this, "You can use the AND, OR and NOT logical operators. Examples:" + Environment.NewLine + Environment.NewLine
                        + "  James AND Bond" + Environment.NewLine
                        + "  Avengers NOT New" + Environment.NewLine
                        + "  CSI AND Miami OR New York" + Environment.NewLine
                        + "  global warming OR climate change AND Gore OR Obama" + Environment.NewLine + Environment.NewLine
                        + "Operators must be in capitals and OR has higher priority than AND.",
                        "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, "You can use the OR logical operator. Examples:" + Environment.NewLine + Environment.NewLine
                        + "  Homer OR Bart" + Environment.NewLine
                        + "  The Simpsons OR Simpsons" + Environment.NewLine + Environment.NewLine
                        + "OR operator must be in capitals.",
                        "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
