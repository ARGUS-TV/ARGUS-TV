/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.ServiceModel;
using System.Net.Security;
using System.IO;
using System.Threading;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;


namespace ArgusTV.UI.Notifier
{
    public partial class StatusForm : Form
    {
        #region Private members

        private const int _retryTcpConnectionMinutes = 10;
        private const int _maxTipTextLength = 256;
        private string _eventsServiceBaseUrl;

        private ServerStatus _serverStatus = ServerStatus.NotConnected;
        private SettingsForm _settingsForm;
        
        private Point _toolTipMousePosition;
        private StatusToolTipForm _toolTipForm;
        private bool _iconContextMenuStripIsOpen;

        #endregion

        public StatusForm()
        {
            InitializeComponent();
            _notifyIcon.Text = String.Empty;
            Config.Load();
            _notifyIcon.MouseMove += new MouseEventHandler(_notifyIcon_MouseMove);
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

        internal string EventsServiceBaseUrl
        {
            get { return _eventsServiceBaseUrl; }
            set { _eventsServiceBaseUrl = value; }
        }

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
                    && !String.IsNullOrEmpty(Config.Current.TcpServerName)
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
                SendWakeOnLanArgs args = new SendWakeOnLanArgs()
                {
                    TcpServerName = Config.Current.TcpServerName,
                    TcpPort = Config.Current.TcpPort,
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
                        EnsureConnection();
                    }
                }
                _settingsForm = null;
            }
        }

        private void _exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.IsConnected
                && this.ServiceTransport == ServiceTransport.NetTcp)
            {
                try
                {
                    using (CoreServiceAgent coreAgent = new CoreServiceAgent())
                    {
                        coreAgent.RemoveEventListener(_eventsServiceBaseUrl);
                    }
                }
                catch
                {
                }
            }
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
            RefreshStatusNow();
            ShowRecordingBalloon("Recording Ended", recording, false);
        }

        public void OnLiveStreamStarted(LiveStream liveStream)
        {
            RefreshStatusNow();
        }

        public void OnLiveStreamEnded(LiveStream liveStream)
        {
            RefreshStatusNow();
        }

        public bool OnUpcomingRecordingsChanged()
        {
            if (this.IsConnected
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
            if (this.IsConnected)
            {
                if (!OnUpcomingRecordingsChanged())
                {
                    RefreshStatus();
                }
            }
        }
        #endregion

        #region Privates

        private void RefreshStatusNow()
        {
            _refreshTimer.Stop();
            _refreshTimer.Interval = 10;
            _refreshTimer.Start();
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
            try
            {
                EnsureConnection();
                OnActiveRecordingsChanged();
            }
            catch
            {
            }
            finally
            {
#if DEBUG
                _refreshTimer.Interval = 10000;
#else
                _refreshTimer.Interval = Math.Max(2000, Config.Current.PollIntervalSeconds * 1000);
#endif
                _refreshTimer.Start();
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _activeRecordingsControl.ShowScheduleName = true;
            _upcomingProgramsControl.Sortable = true;
            _upcomingProgramsControl.ShowScheduleName = true;
            this.Text += " " + Constants.ProductVersion;
            EnsureConnection();
            _refreshTimer.Interval = 500;
            _refreshTimer.Start();
        }

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
        #endregion

        #region Connection

        private object _connectionLock = new object();
        private bool _isConnected;
        private ServiceTransport _serviceTransport;

        public bool IsConnected
        {
            get
            {
                lock (_connectionLock)
                {
                    return _isConnected;
                }
            }
            set
            {
                lock (_connectionLock)
                {
                    _isConnected = value;
                }
            }
        }

        public ServiceTransport ServiceTransport
        {
            get
            {
                lock (_connectionLock)
                {
                    return _serviceTransport;
                }
            }
        }

        private void EnsureConnection()
        {
            lock (_connectionLock)
            {
                if (!_connectionBackgroundWorker.IsBusy)
                {
                    _connectionBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private DateTime _nextTcpConnectionAttemptTime = DateTime.MinValue;

        #region BackgroundWorker

        private void _connectionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {            
            bool retryTcpConnection =
                (this.ServiceTransport == ServiceTransport.BinaryHttps
                && !String.IsNullOrEmpty(Config.Current.TcpServerName)
                && DateTime.Now > _nextTcpConnectionAttemptTime);
            if (!this.IsConnected
                || retryTcpConnection)
            {
                e.Result = false;

                try
                {
                    if (!String.IsNullOrEmpty(Config.Current.TcpServerName))
                    {
                        ServerSettings serverSettings = new ServerSettings();
                        serverSettings.ServerName = Config.Current.TcpServerName;
                        serverSettings.Port = Config.Current.TcpPort;
                        serverSettings.Transport = ServiceTransport.NetTcp;

                        ServiceChannelFactories.Initialize(serverSettings, true);

                        using (CoreServiceAgent coreAgent = new CoreServiceAgent())
                        {
                            coreAgent.EnsureEventListener(EventGroup.RecordingEvents, _eventsServiceBaseUrl, Constants.EventListenerApiVersion);
                        }
                        lock (_connectionLock)
                        {
                            _serviceTransport = serverSettings.Transport;
                            _isConnected = true;
                        }
                        _nextTcpConnectionAttemptTime = DateTime.MaxValue;
                        e.Result = true;
                    }
                    else
                    {
                        ConnectOverHttps();
                        e.Result = true;
                    }
                }
                catch
                {
                    try
                    {
                        ConnectOverHttps();
                        e.Result = true;
                    }
                    catch
                    {
                        this.IsConnected = false;
                    }
                }
            }
            else
            {
                e.Result = this.IsConnected;
            }
        }

        private void _connectionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null
                && (bool)e.Result)
            {
                RefreshStatus();
            }
            else
            {
                SetStatusIcon(ServerStatus.NotConnected);
            }
        }
        #endregion

        private void ConnectOverHttps()
        {
            if (!String.IsNullOrEmpty(Config.Current.HttpsServerName))
            {
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = Config.Current.HttpsServerName;
                serverSettings.Port = Config.Current.HttpsPort;
                serverSettings.UserName = Config.Current.UserName;
                serverSettings.Password = Config.Current.Password;
                serverSettings.Transport = ServiceTransport.BinaryHttps;
                if (ServiceChannelFactories.Initialize(serverSettings, false))
                {
                    lock (_connectionLock)
                    {
                        _serviceTransport = serverSettings.Transport;
                        _isConnected = true;
                        _nextTcpConnectionAttemptTime = DateTime.Now.AddMinutes(_retryTcpConnectionMinutes);
                    }
                }
            }
        }

        private class SendWakeOnLanArgs
        {
            public string TcpServerName { get; set; }
            public int TcpPort { get; set; }
            public string IpAddress { get; set; }
            public string MacAddresses { get; set; }
        }

        private void SendWakeOnLan(object state)
        {
            SendWakeOnLanArgs args = state as SendWakeOnLanArgs;
            if (args != null
                && !String.IsNullOrEmpty(args.TcpServerName))
            {
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = args.TcpServerName;
                serverSettings.Port = args.TcpPort;
                serverSettings.Transport = ServiceTransport.NetTcp;

                serverSettings.WakeOnLan.IPAddress = args.IpAddress;
                serverSettings.WakeOnLan.MacAddresses = args.MacAddresses;
                serverSettings.WakeOnLan.Enabled = true;

                ServiceChannelFactories.Initialize(serverSettings, false);
            }
        }

        #endregion

        #region Refresh Status

        private ActiveRecording[] _activeRecordings = new ActiveRecording[0];
        private LiveStream[] _liveStreams = new LiveStream[0];
        private UpcomingRecording _upcomingRecording;

        private class RefreshStatusResult
        {
            public ActiveRecording[] ActiveRecordings { set; get; }

            public LiveStream[] LiveStreams { set; get; }

            public UpcomingRecording UpcomingRecording { set; get; }
        }

        public void RefreshStatus()
        {
            if (!_refreshStatusBackgroundWorker.IsBusy)
            {
                _refreshStatusBackgroundWorker.RunWorkerAsync();
            }
        }

        private void _refreshStatusBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.IsConnected)
            {
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    RefreshStatusResult result = new RefreshStatusResult();
                    result.ActiveRecordings = tvControlAgent.GetActiveRecordings();
                    result.LiveStreams = tvControlAgent.GetLiveStreams();
                    result.UpcomingRecording = tvControlAgent.GetNextUpcomingRecording(false);
                    e.Result = result;
                }
            }
        }

        private void _refreshStatusBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null
                || e.Cancelled
                || e.Result == null)
            {
                _activeRecordings = new ActiveRecording[0];
                _liveStreams = new LiveStream[0];
                _upcomingRecording = null;
                SetStatusIcon(ServerStatus.NotConnected);
            }
            else
            {
                RefreshStatusResult result = (RefreshStatusResult)e.Result;
                _activeRecordings = result.ActiveRecordings;
                _liveStreams = result.LiveStreams;
                _upcomingRecording = result.UpcomingRecording;
                SetStatusIcon(GetServerStatus(result.ActiveRecordings, result.LiveStreams));
            }
        }

        private static ServerStatus GetServerStatus(ActiveRecording[] activeRecordings, LiveStream[] liveStreams)
        {
            if (activeRecordings.Length > 0)
            {
                return ServerStatus.Recording;
            }
            else if (liveStreams.Length > 0)
            {
                return ServerStatus.Streaming;
            }
            return ServerStatus.Idle;
        }

        #endregion

        #region Refresh Active/Upcoming Programs

        private class RefreshProgramsResult
        {
            public UpcomingRecording[] AllUpcomingRecordings { set; get; }

            public UpcomingOrActiveProgramsList UpcomingRecordings { set; get; }

            public ActiveRecording[] ActiveRecordings { set; get; }

            public LiveStream[] LiveStreams { set; get; }
        }

        private void RefreshActiveAndUpcomingRecordings()
        {
            if (!_refreshProgramsBackgroundWorker.IsBusy)
            {
                _refreshProgramsBackgroundWorker.RunWorkerAsync();
            }
        }

        private void _refreshProgramsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshProgramsResult result = null;
            //DateTime startTime = DateTime.Now;

            if (this.IsConnected)
            {
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    result = new RefreshProgramsResult();

                    result.AllUpcomingRecordings = tvControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings, true);
                    result.ActiveRecordings = tvControlAgent.GetActiveRecordings();
                    result.LiveStreams = tvControlAgent.GetLiveStreams();
                    result.UpcomingRecordings = new UpcomingOrActiveProgramsList(result.AllUpcomingRecordings);
                    result.UpcomingRecordings.RemoveActiveRecordings(result.ActiveRecordings);
                }
            }

            //Utility.EnsureMinimumTime(startTime, 250);

            e.Result = result;
        }

        private void _refreshProgramsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null
                || e.Cancelled)
            {
                _activeRecordingsControl.UpcomingPrograms = null;
                _upcomingProgramsControl.UpcomingPrograms = null;
                SetStatusIcon(ServerStatus.NotConnected);
            }
            else if (e.Result != null)
            {
                RefreshProgramsResult result = (RefreshProgramsResult)e.Result;
                SetStatusIcon(GetServerStatus(result.ActiveRecordings, result.LiveStreams));
                _activeRecordingsControl.UnfilteredUpcomingRecordings = new UpcomingOrActiveProgramsList(result.AllUpcomingRecordings);
                _activeRecordingsControl.UpcomingPrograms = new UpcomingOrActiveProgramsList(result.ActiveRecordings);
                _upcomingProgramsControl.UpcomingPrograms = result.UpcomingRecordings;
            }
        }

        #endregion

    }
}
