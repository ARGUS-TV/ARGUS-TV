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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process;

namespace ArgusTV.UI.Notifier
{
    public partial class StatusForm : Form
    {
        private const int _maxTipTextLength = 256;

        private string _eventsServiceBaseUrl;
        private string _eventsClientId;
        private SynchronizationContext _uiSyncContext;

        private ServerStatus _serverStatus = ServerStatus.NotConnected;
        private SettingsForm _settingsForm;

        private Task _eventListenerTask;
        private CancellationTokenSource _listenerCancellationTokenSource;
        
        private Point _toolTipMousePosition;
        private StatusToolTipForm _toolTipForm;
        private bool _iconContextMenuStripIsOpen;

        public StatusForm()
        {
            InitializeComponent();
            _notifyIcon.Text = String.Empty;
            Config.Load();
            _notifyIcon.MouseMove += new MouseEventHandler(_notifyIcon_MouseMove);
            _uiSyncContext = SynchronizationContext.Current;

            _eventsClientId = String.Format("{0}-{1}-99b8cd44d1ab459cb16f199a48086588", // Unique for the Notifier!
                Dns.GetHostName(), System.Environment.GetEnvironmentVariable("SESSIONNAME"));
            StartEventListenerTask();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _activeRecordingsControl.ShowScheduleName = true;
            _upcomingProgramsControl.Sortable = true;
            _upcomingProgramsControl.ShowScheduleName = true;
            this.Text += " " + Constants.ProductVersion;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                CancelEventListenerTask();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void StartEventListenerTask()
        {
            _listenerCancellationTokenSource = new CancellationTokenSource();
            _eventListenerTask = new Task(() => ConnectAndHandleEvents(_listenerCancellationTokenSource.Token),
                _listenerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _eventListenerTask.Start();
        }

        private void CancelEventListenerTask()
        {
            try
            {
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Cancel();
                    _eventListenerTask.Wait();
                }
            }
            catch
            {
            }
            finally
            {
                if (_eventListenerTask != null)
                {
                    _eventListenerTask.Dispose();
                    _eventListenerTask = null;
                }
                if (_listenerCancellationTokenSource != null)
                {
                    _listenerCancellationTokenSource.Dispose();
                    _listenerCancellationTokenSource = null;
                }
            }
        }

        #region Tooltip / TrayIcon / Balloon

        private void _notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = Cursor.Position;

            if (SafeGetToolTipForm() == null)
            {
                if (_toolTipTimer.Enabled)
                {
                    if (Math.Abs(position.X - _toolTipMousePosition.X) > 1
                        || Math.Abs(position.Y - _toolTipMousePosition.Y) > 1)
                    {
                        _toolTipTimer.Stop();
                    }
                }
                else
                {
                    _toolTipMousePosition = position;
                    _toolTipTimer.Interval = 200;
                    _toolTipTimer.Start();
                }
            }
        }

        private void _toolTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _toolTipTimer.Stop();
                if (!_iconContextMenuStripIsOpen)
                {
                    Point position = Cursor.Position;
                    if (Math.Abs(position.X - _toolTipMousePosition.X) < 6
                        && Math.Abs(position.Y - _toolTipMousePosition.Y) < 6)
                    {
                        this.Invoke(new EventHandler(this.EnsureToolTipShown), this, EventArgs.Empty);
                        _toolTipTimer.Interval = 20;
                        _toolTipTimer.Start();
                    }
                    else
                    {
                        this.Invoke(new EventHandler(this.CloseToolTip), this, EventArgs.Empty);
                    }
                }
            }
            catch
            {
                // We don't care about any possible errors here.
            }
        }

        private StatusToolTipForm SafeGetToolTipForm()
        {
            if (_toolTipForm != null)
            {
                if (_toolTipForm.IsDisposed)
                {
                    _toolTipForm = null;
                }
                else if (!_toolTipForm.Visible)
                {
                    _toolTipForm.Dispose();
                    _toolTipForm = null;
                }
            }
            return _toolTipForm;
        }

        private void EnsureToolTipShown(object sender, EventArgs args)
        {
            if (SafeGetToolTipForm() == null)
            {
                _toolTipForm = new StatusToolTipForm();
                _toolTipForm.Show(_toolTipMousePosition, _serverStatus, _activeRecordings, _liveStreams, _upcomingRecording);
            }
        }

        private void CloseToolTip(object sender, EventArgs args)
        {
            if (SafeGetToolTipForm() != null)
            {
                if (!_toolTipForm.IsDisposed
                    && _toolTipForm.Visible)
                {
                    _toolTipForm.Close();
                    _toolTipForm.Dispose();
                    _toolTipForm = null;
                }
            }
        }

        private void SetStatusIcon(ServerStatus serverStatus)
        {
            _serverStatus = serverStatus;
            if (serverStatus == ServerStatus.Recording)
            {
                _notifyIcon.Icon = Properties.Resources.Recording;
            }
            else if (serverStatus == ServerStatus.Streaming)
            {
                _notifyIcon.Icon = Properties.Resources.Streaming;
            }
            else if (serverStatus == ServerStatus.Idle)
            {
                _notifyIcon.Icon = Properties.Resources.Idle;
            }
            else
            {
                _notifyIcon.Icon = Properties.Resources.InError;
                this.IsConnected = false;
            }
            this.Icon = _notifyIcon.Icon;
        }

        private void ShowRecordingBalloon(string title, Recording recording, bool showDescription)
        {
            if (Config.Current.ShowRecordingBalloons)
            {
                StringBuilder tipText = new StringBuilder();
                tipText.Append(recording.ProgramStartTime.ToShortTimeString());
                tipText.Append("-");
                tipText.Append(recording.ProgramStopTime.ToShortTimeString());
                tipText.Append(" ");
                tipText.Append(recording.CreateProgramTitle());
                if (showDescription)
                {
                    string description = recording.CreateCombinedDescription(false);
                    if (!String.IsNullOrEmpty(description))
                    {
                        tipText.Append(Environment.NewLine).Append(Environment.NewLine);
                        tipText.Append(description);
                    }
                }
                if (tipText.Length >= _maxTipTextLength)
                {
                    tipText.Length = _maxTipTextLength - 4;
                    tipText.Append("...");
                }
                _notifyIcon.ShowBalloonTip(Config.Current.BalloonTimeoutSeconds * 1000, title, tipText.ToString(), ToolTipIcon.Info);
            }
        }

        #endregion

        #region Opening/Closing

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Hide();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void _notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _openToolStripMenuItem_Click(sender, e);
        }

        private void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            _openToolStripMenuItem_Click(sender, e);
        }

        private void _openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _activeRecordingsControl.UpcomingPrograms = null;
            _upcomingProgramsControl.UpcomingPrograms = null;
            if (this.Visible
                && this.WindowState == FormWindowState.Normal)
            {
                this.BringToFront();
            }
            else
            {
                this.ShowInTaskbar = true;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            this.Activate();
            RefreshActiveAndUpcomingRecordings();
            RefreshStatus();
        }

        #endregion

        #region Control events

        private void _iconContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            _iconContextMenuStripIsOpen = true;
            CloseToolTip(this, EventArgs.Empty);

            if (_openManagementConsoleToolStripMenuItem != null)
            {
                _openManagementConsoleToolStripMenuItem.Enabled = Config.Current != null
                    && !String.IsNullOrEmpty(Config.Current.MmcPath)
                    && File.Exists(Config.Current.MmcPath);
                _wakeupServerToolStripMenuItem.Enabled = Config.Current != null
                    && !IsConnected
                    && !String.IsNullOrEmpty(Config.Current.ServerName)
                    && !String.IsNullOrEmpty(Config.Current.MacAddresses)
                    && !String.IsNullOrEmpty(Config.Current.IpAddress);
            }
        }

        private void _iconContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _iconContextMenuStripIsOpen = false;
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            RefreshActiveAndUpcomingRecordings();
        }

        private void _closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.ShowInTaskbar = false;
        }

        private void _openManagementConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Config.Current.MmcPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to start the Scheduler Console." + Environment.NewLine + Environment.NewLine + ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void _wakeupServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Config.Current != null)
            {
                var args = new SendWakeOnLanArgs()
                {
                    ServerName = Config.Current.ServerName,
                    Port = Config.Current.Port,
                    UserName = Config.Current.UserName,
                    Password = Config.Current.Password,
                    IpAddress = Config.Current.IpAddress,
                    MacAddresses = Config.Current.MacAddresses
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(SendWakeOnLan), args);
            }
        }

        private void _optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_settingsForm != null
                && !_settingsForm.IsDisposed)
            {
                _settingsForm.BringToFront();
            }
            else
            {
                using (_settingsForm = new SettingsForm())
                {
                    if (_settingsForm.ShowDialog(this) == DialogResult.OK)
                    {
                        this.IsConnected = false;
                        CancelEventListenerTask();
                        StartEventListenerTask();
                    }
                }
                _settingsForm = null;
            }
        }

        private void _exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelEventListenerTask();
            Application.Exit();
        }

        private void _activeRecordingsControl_GridDoubleClick(object sender, EventArgs e)
        {
            Point mousePosition = _activeRecordingsControl.PointToClient(Control.MousePosition);
            DataGridView.HitTestInfo htinfo = _activeRecordingsControl.HitTest(mousePosition.X, mousePosition.Y);
            if (htinfo.Type == DataGridViewHitTestType.Cell)
            {
                PlaySelectedRecording();
            }
        }

        private void _activeRecordingsControl_GridMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo htinfo = _activeRecordingsControl.HitTest(e.X, e.Y);
                if (htinfo.Type == DataGridViewHitTestType.Cell)
                {
                    UpcomingOrActiveProgramView upcomingProgramView = _activeRecordingsControl.Rows[htinfo.RowIndex].DataBoundItem as UpcomingOrActiveProgramView;
                    if (upcomingProgramView != null)
                    {
                        _playRecordingToolStripMenuItem.Enabled = Utility.IsVlcInstalled();
                        _activeRecordingsControl.Rows[htinfo.RowIndex].Selected = true;
                        _programContextMenuStrip.Show(_activeRecordingsControl, e.Location);
                    }
                    else
                    {
                        _playRecordingToolStripMenuItem.Enabled = false;
                    }
                }
            }
        }

        private void _programContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem == _playRecordingToolStripMenuItem)
                {
                    PlaySelectedRecording();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ARGUS TV Events

        public void OnRecordingStarted(Recording recording)
        {
            SetStatusIcon(ServerStatus.Recording);
            ShowRecordingBalloon("Recording Started", recording, true);
        }

        public void OnRecordingEnded(Recording recording)
        {
            ShowRecordingBalloon("Recording Ended", recording, false);
            OnActiveRecordingsChanged();
        }

        public void OnLiveStreamStarted(LiveStream liveStream)
        {
            OnActiveRecordingsChanged();
        }

        public void OnLiveStreamEnded(LiveStream liveStream)
        {
            OnActiveRecordingsChanged();
        }

        public bool OnUpcomingRecordingsChanged()
        {
            if (Proxies.IsInitialized
                && this.Visible
                && this.WindowState == FormWindowState.Normal)
            {
                RefreshActiveAndUpcomingRecordings();
                return true;
            }
            return false;
        }

        public void OnActiveRecordingsChanged()
        {
            if (Proxies.IsInitialized)
            {
                if (!OnUpcomingRecordingsChanged())
                {
                    RefreshStatus();
                }
            }
        }

        #endregion

        private void PlaySelectedRecording()
        {
            ActiveRecording activeRecording = GetSelectedActiveRecording();
            if (activeRecording != null)
            {
                Utility.RunVlc(activeRecording.RecordingFileName);
            }
        }

        private ActiveRecording GetSelectedActiveRecording()
        {
            ActiveRecording activeRecording = null;
            if (_activeRecordingsControl.SelectedRows.Count > 0)
            {
                UpcomingOrActiveProgramView upcomingProgramView = _activeRecordingsControl.SelectedRows[0].DataBoundItem as UpcomingOrActiveProgramView;
                if (upcomingProgramView != null)
                {
                    activeRecording = upcomingProgramView.ActiveRecording;
                }
            }
            return activeRecording;
        }

        #region Connection

        private bool _eventListenerSubscribed;
        private int _eventsErrorCount = 0;

        public bool IsConnected { get; set; }

        private void ConnectAndHandleEvents(CancellationToken cancellationToken)
        {
            for (; ; )
            {
                if (Proxies.IsInitialized)
                {
                    IList<ServiceEvent> events = null;
                    if (!_eventListenerSubscribed)
                    {
                        try
                        {
                            Proxies.CoreService.SubscribeServiceEvents(_eventsClientId, EventGroup.RecordingEvents);
                            _eventListenerSubscribed = true;
                            _eventsErrorCount = 0;

                            this.IsConnected = true;
                            _uiSyncContext.Post(s => RefreshStatus(), null);
                        }
                        catch
                        {
                        }
                    }
                    if (_eventListenerSubscribed)
                    {
                        try
                        {
                            events = Proxies.CoreService.GetServiceEvents(_eventsClientId, cancellationToken);
                            if (events == null)
                            {
                                _eventListenerSubscribed = false;
                                _uiSyncContext.Post(s => RefreshStatus(), null);
                            }
                            else
                            {
                                if (events.Count == 0)
                                {
                                    // In case of a timeout, let's refresh the general status -- to make sure we don't miss any events.
                                    _uiSyncContext.Post(s => RefreshStatus(), null);
                                }
                                ProcessEvents(events);
                            }
                        }
                        catch(Exception ex)
                        {
                            if (ex is ArgusTVNotFoundException
                                || ++_eventsErrorCount > 5)
                            {
                                _eventListenerSubscribed = false;
                                this.IsConnected = false;
                                _uiSyncContext.Post(s => SetStatusIcon(ServerStatus.NotConnected), null);
                            }
                        }
                    }
                }
                else
                {
                    _uiSyncContext.Send(s => InitializeConnectionToArgusTV(), null);
                }
                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_eventListenerSubscribed ? 0 : 10)))
                {
                    break;
                }
            }

            if (Proxies.IsInitialized
                && _eventListenerSubscribed)
            {
                try
                {
                    Proxies.CoreService.UnsubscribeServiceEvents(_eventsClientId);
                }
                catch
                {
                }
                _eventListenerSubscribed = false;
            }
        }

        private void ProcessEvents(IList<ServiceEvent> events)
        {
            foreach(var @event in events)
            {
                if (@event.Name == ServiceEventNames.UpcomingRecordingsChanged)
                {
                    _uiSyncContext.Post(s => OnUpcomingRecordingsChanged(), null);
                }
                else if (@event.Name == ServiceEventNames.ActiveRecordingsChanged)
                {
                    _uiSyncContext.Post(s => OnActiveRecordingsChanged(), null);
                }
                else if (@event.Name == ServiceEventNames.RecordingStarted)
                {
                    _uiSyncContext.Post(s => OnRecordingStarted((Recording)@event.Arguments[0]), null);
                }
                else if (@event.Name == ServiceEventNames.RecordingEnded)
                {
                    _uiSyncContext.Post(s => OnRecordingEnded((Recording)@event.Arguments[0]), null);
                }
                else if (@event.Name == ServiceEventNames.LiveStreamStarted
                    || @event.Name == ServiceEventNames.LiveStreamTuned)
                {
                    _uiSyncContext.Post(s => OnLiveStreamStarted((LiveStream)@event.Arguments[0]), null);
                }
                else if (@event.Name == ServiceEventNames.LiveStreamEnded
                    || @event.Name == ServiceEventNames.LiveStreamAborted)
                {
                    _uiSyncContext.Post(s => OnLiveStreamEnded((LiveStream)@event.Arguments[0]), null);
                }
            }
        }

        #endregion

        private void InitializeConnectionToArgusTV()
        {
            if (!String.IsNullOrEmpty(Config.Current.ServerName))
            {
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = Config.Current.ServerName;
                serverSettings.Port = Config.Current.Port;
                serverSettings.UserName = Config.Current.UserName;
                serverSettings.Password = Config.Current.Password;
                serverSettings.Transport = ServiceTransport.Https;
                this.IsConnected = Proxies.Initialize(serverSettings, false);
                if (this.IsConnected)
                { 
                    SetStatusIcon(ServerStatus.Idle);
                }
                else
                {
                    SetStatusIcon(ServerStatus.NotConnected);
                }
            }
        }

        private class SendWakeOnLanArgs
        {
            public string ServerName { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string IpAddress { get; set; }
            public string MacAddresses { get; set; }
        }

        private void SendWakeOnLan(object state)
        {
            SendWakeOnLanArgs args = state as SendWakeOnLanArgs;
            if (args != null
                && !String.IsNullOrEmpty(args.ServerName))
            {
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = args.ServerName;
                serverSettings.Port = args.Port;
                serverSettings.Transport = ServiceTransport.Https;
                serverSettings.UserName = args.UserName;
                serverSettings.Password = args.Password;
                serverSettings.WakeOnLan.IPAddress = args.IpAddress;
                serverSettings.WakeOnLan.MacAddresses = args.MacAddresses;
                serverSettings.WakeOnLan.Enabled = true;
                Proxies.Initialize(serverSettings, false);
            }
        }

        #region Refresh Status

        private IList<ActiveRecording> _activeRecordings = new List<ActiveRecording>();
        private IList<LiveStream> _liveStreams = new List<LiveStream>();
        private UpcomingRecording _upcomingRecording;

        private class RefreshStatusResult
        {
            public IList<ActiveRecording> ActiveRecordings { set; get; }
            public IList<LiveStream> LiveStreams { set; get; }
            public UpcomingRecording UpcomingRecording { set; get; }
        }

        public async void RefreshStatus()
        {
            RefreshStatusResult result = null;
            if (Proxies.IsInitialized)
            {
                await Task.Run(() =>
                {
                    var proxy = Proxies.ControlService;
                    try
                    {
                        result = new RefreshStatusResult()
                        {
                            ActiveRecordings = proxy.GetActiveRecordings(),
                            LiveStreams = proxy.GetLiveStreams(),
                            UpcomingRecording = proxy.GetNextUpcomingRecording(false)
                        };
                    }
                    catch
                    {
                        result = null;
                    }
                });
            }
            if (result == null)
            {
                _activeRecordings = new ActiveRecording[0];
                _liveStreams = new LiveStream[0];
                _upcomingRecording = null;
                SetStatusIcon(ServerStatus.NotConnected);
            }
            else
            {
                _activeRecordings = result.ActiveRecordings;
                _liveStreams = result.LiveStreams;
                _upcomingRecording = result.UpcomingRecording;
                SetStatusIcon(GetServerStatus(result.ActiveRecordings, result.LiveStreams));
            }
        }

        private static ServerStatus GetServerStatus(IList<ActiveRecording> activeRecordings, IList<LiveStream> liveStreams)
        {
            if (activeRecordings.Count > 0)
            {
                return ServerStatus.Recording;
            }
            else if (liveStreams.Count > 0)
            {
                return ServerStatus.Streaming;
            }
            return ServerStatus.Idle;
        }

        #endregion

        #region Refresh Active/Upcoming Programs

        private class RefreshProgramsResult
        {
            public IList<UpcomingRecording> AllUpcomingRecordings { set; get; }
            public UpcomingOrActiveProgramsList UpcomingRecordings { set; get; }
            public IList<ActiveRecording> ActiveRecordings { set; get; }
            public IList<LiveStream> LiveStreams { set; get; }
        }

        private async void RefreshActiveAndUpcomingRecordings()
        {
            RefreshProgramsResult result = null;
            if (Proxies.IsInitialized)
            {
                await Task.Run(() =>
                {
                    var proxy = Proxies.ControlService;
                    try
                    {
                        result = new RefreshProgramsResult()
                        {
                            AllUpcomingRecordings = proxy.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings, true),
                            ActiveRecordings = proxy.GetActiveRecordings(),
                            LiveStreams = proxy.GetLiveStreams()
                        };
                        result.UpcomingRecordings = new UpcomingOrActiveProgramsList(result.AllUpcomingRecordings);
                        result.UpcomingRecordings.RemoveActiveRecordings(result.ActiveRecordings);
                    }
                    catch
                    {
                        result = null;
                    }
                });
            }

            if (result == null)
            {
                _activeRecordingsControl.UpcomingPrograms = null;
                _upcomingProgramsControl.UpcomingPrograms = null;
                SetStatusIcon(ServerStatus.NotConnected);
            }
            else
            {
                SetStatusIcon(GetServerStatus(result.ActiveRecordings, result.LiveStreams));
                _activeRecordingsControl.UnfilteredUpcomingRecordings = new UpcomingOrActiveProgramsList(result.AllUpcomingRecordings);
                _activeRecordingsControl.UpcomingPrograms = new UpcomingOrActiveProgramsList(result.ActiveRecordings);
                _upcomingProgramsControl.UpcomingPrograms = result.UpcomingRecordings;
            }
        }

        #endregion
    }
}
