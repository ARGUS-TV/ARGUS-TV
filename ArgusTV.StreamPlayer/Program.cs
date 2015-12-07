using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Microsoft.Win32;

using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;

namespace ArgusTV.StreamPlayer
{
    static class Program
    {
        private static int _checkInterval = 20 * 1000;

        private static AutoResetEvent _playerExitedEvent = new AutoResetEvent(false);
        private static KeepAliveThread _keepAliveThread;

        static void Main(string[] args)
        {
            try
            {
#if DEBUG
                Thread.Sleep(5000);
                _checkInterval = 3000;
#endif
                if (args.Length == 8)
                {
                    string playerExe = args[0];
                    bool isLiveStream = args[6] == "L";
                    string rtspUrl = args[7];

                    Process liveStreamPlayerProcess = null;

                    if (rtspUrl.Contains("://"))
                    {
                        liveStreamPlayerProcess = new Process();
                        liveStreamPlayerProcess.StartInfo.FileName = playerExe;
                        liveStreamPlayerProcess.StartInfo.Arguments = rtspUrl;
                        liveStreamPlayerProcess.StartInfo.CreateNoWindow = true;
                        liveStreamPlayerProcess.EnableRaisingEvents = true;
                        liveStreamPlayerProcess.Exited += new EventHandler(livePlayerProcess_Exited);
                        liveStreamPlayerProcess.Start();
                    }

                    ServerSettings serverSettings = new ServerSettings();
                    serverSettings.ServerName = args[1];
                    serverSettings.Port = Int32.Parse(args[2]);
                    serverSettings.Transport = (ServiceTransport)Enum.Parse(typeof(ServiceTransport), args[3]);
                    serverSettings.UserName = args[4];
                    serverSettings.Password = args[5];

                    Proxies.Initialize(serverSettings, false);
                    if (Proxies.IsInitialized)
                    {
                        if (isLiveStream)
                        {
                            PlayLiveStream(rtspUrl, liveStreamPlayerProcess);
                        }
                        else
                        {
                            PlayRecordingStream(rtspUrl, liveStreamPlayerProcess);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect to the ARGUS TV Scheduler service!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (liveStreamPlayerProcess != null
                        && !liveStreamPlayerProcess.HasExited)
                    {
                        liveStreamPlayerProcess.CloseMainWindow();
                        liveStreamPlayerProcess.WaitForExit(1000);
                        if (!liveStreamPlayerProcess.HasExited)
                        {
                            liveStreamPlayerProcess.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _playerExitedEvent.Close();
            }
        }

        private static void PlayLiveStream(string rtspUrl, Process liveStreamPlayerProcess)
        {
            LiveStream liveStream = Proxies.ControlService.GetLiveStreamByRtspUrl(rtspUrl).Result;
            if (liveStream != null)
            {
                if (liveStreamPlayerProcess == null)
                {
                    _keepAliveThread = new KeepAliveThread();
                    _keepAliveThread.LiveStream = liveStream;
                    _keepAliveThread.Start();

                    string name = String.IsNullOrEmpty(liveStream.TimeshiftFile) ? liveStream.RtspUrl : liveStream.TimeshiftFile;

                    MessageBox.Show("Can't play stream " + name + " in VLC." + Environment.NewLine + Environment.NewLine
                        + "Press OK to stop the stream.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    _keepAliveThread.Stop(true);
                }
                else
                {
                    bool exited = false;
                    while (!exited)
                    {
                        if (!Proxies.ControlService.KeepLiveStreamAlive(liveStream).Result)
                        {
                            break;
                        }
                        exited = _playerExitedEvent.WaitOne(_checkInterval, false);
                    }
                }

                Proxies.ControlService.StopLiveStream(liveStream).Wait();
            }
            else
            {
                MessageBox.Show("Failed to find live stream for " + rtspUrl, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void PlayRecordingStream(string rtspUrl, Process liveStreamPlayerProcess)
        {
            if (liveStreamPlayerProcess == null)
            {
                MessageBox.Show("Can't play stream " + rtspUrl + " in VLC." + Environment.NewLine + Environment.NewLine
                    + "Press OK to stop the stream.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                _playerExitedEvent.WaitOne(Timeout.Infinite, false);
            }
            Proxies.ControlService.StopRecordingStream(rtspUrl);
        }

        #region Private Methods

        private static void livePlayerProcess_Exited(object sender, EventArgs e)
        {
            _playerExitedEvent.Set();
        }

        #endregion
    }
}
