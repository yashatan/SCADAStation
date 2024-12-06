using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using S7.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using S7.Net.Types;



namespace SCADAStationNetFrameWork
{

    public class FunctionalLab
    {
        List<TagInfo> listTags;
        List<ConnectDevice> listDevices;
        List<AlarmSetting> listAlarmSettings;
        public List<AlarmPoint> listAlarmPoints;
        public List<TagLoggingSetting> listTagLoggingSettings;
        SCADAAppConfiguration mSCADAConfiguration;
        Dictionary<int, Plc> listS7plcs;
        private IDisposable _signalR;
        public string url;
        string filePath_SCADAStationConfiguration;
        private SCADAStationConfiguration mSCADAStationConfiguration;
        public static ProjectInformation currentProjectInformation;
        public List<TrendPoint> listTrendPoints;
        public FunctionalLab()
        {


            filePath_SCADAStationConfiguration = "E:\\SCADAProject\\DemoSCADA\\DemoSCADAStation.json";
            Initialize();
        }

        public FunctionalLab(string fileName)
        {
            filePath_SCADAStationConfiguration = fileName;
            Initialize();
        }

        private void Initialize()
        {
            listS7plcs = new Dictionary<int, Plc>();
            LoadConfigFile(filePath_SCADAStationConfiguration);
            listTrendPoints = SCADAStationDbContext.Instance.TrendPoints.ToList();
            StartServer();
            SCADAHub.ClientConnected += SCADAHub_ClientConnected;
            SCADAHub.ClientDisconnected += SCADAHub_ClientDisconnected;
            SCADAHub.ClientWriteTag += SCADAHub_ClientWriteTag;
            SCADAHub.AcknowledgeAlarmPoint += SCADAHub_AcknowledegeAlarmPoint;
            SCADAHub.ClientGetTrendValue += SCADAHub_GetTrendValue;
        }

        private void SCADAHub_GetTrendValue(int tagloggingid)
        {
            List<TrendPoint> trendPoints = new List<TrendPoint>(SCADAStationDbContext.Instance.TrendPoints.Where(x => x.TagLoggingId == tagloggingid));
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.ReceiveTrendPoints(trendPoints);
        }

        private void SCADAHub_ClientConnected(string clientId)
        {
            SendSCADAConfigurationToApp();
        }

        private void SendSCADAConfigurationToApp()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            mSCADAConfiguration = new SCADAAppConfiguration();
            mSCADAConfiguration.ControlDatas = mSCADAStationConfiguration.ControlDatas;
            mSCADAConfiguration.TrendViewSettings = mSCADAStationConfiguration.TrendViewSettings;
            if (listAlarmPoints.Count > 0)
            {
                mSCADAConfiguration.CurrentAlarmPoints = listAlarmPoints;
            }
            hubContext.Clients.All.DownloadSCADAConfig(mSCADAConfiguration);
        }

        private void SCADAHub_ClientDisconnected(string clientId)
        {
            //ConnectDevice al;
            // throw new NotImplementedException();
        }

        private void SCADAHub_ClientWriteTag(int tagid, object value)
        {
            TagInfo taginfo = listTags.Where(p => p.Id == tagid).FirstOrDefault();
            if (taginfo.ConnectDevice.ConnectionType == (int)ConnectDevice.emConnectionType.emS7)
            {
                listS7plcs[taginfo.ConnectDevice.Id].Write(taginfo.MemoryAddress, value);
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString().Contains("0.50")) { continue; }
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public void StartServer()
        {
            url = $"http://{GetLocalIPAddress()}:8088/SCADA";
            //txtUrl.Text = url;
            _signalR = WebApp.Start<Startup>(url);

        }

        private void SendTagValueToClient(int tagID, int value)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.UpdateTags(tagID, value);
        }


        public void LoadConfigFile(string fileName)
        {
            Trace.WriteLine($"Starting reading file");
            mSCADAStationConfiguration = new SCADAStationConfiguration();
            mSCADAStationConfiguration = Deserialize_SCADAStationConfiguration(fileName);
            listTags = mSCADAStationConfiguration.TagInfos;
            listDevices = mSCADAStationConfiguration.ConnectDevices;
            listAlarmSettings = mSCADAStationConfiguration.AlarmSettings;
            listTagLoggingSettings = mSCADAStationConfiguration.TagLoggingSettings;
            listAlarmPoints = new List<AlarmPoint>();
            currentProjectInformation = mSCADAStationConfiguration.ProjectInformation;
            MappingTagInfo();
        }

        private void MappingTagInfo()
        {
            foreach (var taglogging in listTagLoggingSettings)
            {
                taglogging.Tag = listTags.FirstOrDefault(m => m.Id == taglogging.Tag.Id);
            }
            foreach (AlarmSetting alarmsetting in listAlarmSettings)
            {
                alarmsetting.TriggerTag = listTags.FirstOrDefault(m => m.Id == alarmsetting.TriggerTag.Id);
            }
        }

        public async Task SetupDeviceConnection(List<ConnectDevice> deviceList)
        {
            foreach (var device in deviceList)
            {
                if (device.ConnectionType == (int)ConnectDevice.emConnectionType.emS7)
                {
                    var PLC = new Plc(ConvertToCPUType(device.S7PLCType), device.Destination, (short)device.S7PLCRack, (short)device.S7PLCSlot);
                    try
                    {
                        await PLC.OpenAsync();
                        Trace.WriteLine($"Connect to the PLC {device.Name} {device.Destination} succesfully");
                        listS7plcs.Add(device.Id, PLC);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Can not connect to the PLC {device.Destination}: {ex.Message}");
                        throw;
                    }
                }

            }
            SetUpTimer();

        }

        private void SetUpTimer()
        {
            var timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            UpdateTagValue();
            UpdateTagLogging();
            if (listAlarmSettings != null && listAlarmSettings.Count > 0)
            {
                CheckAlarm();
            }
            (sender as System.Timers.Timer).Start();
        }

        public void UpdateTagValue()
        {
            foreach (var tagInfo in listTags)
            {
                var device = listS7plcs[tagInfo.ConnectDevice.Id];
                var ob1 = device.Read(tagInfo.MemoryAddress);
                int temp = Convert.ToInt16(ob1);
                if (temp != tagInfo.Value)
                {
                    tagInfo.Value = temp;
                    SendTagValueToClient(tagInfo.Id, tagInfo.Value);
                }
                Trace.WriteLine($"Read tag {tagInfo.Name} from device {tagInfo.ConnectDevice.Name} by value: {temp}");
            }
        }

        CpuType ConvertToCPUType(string type)
        {
            CpuType result = CpuType.S7200;
            switch (type)
            {
                case "S7-200":
                    result = CpuType.S7200;
                    break;
                case "S7-1200":
                    result = CpuType.S71200;
                    break;
                case "S7-1500":
                    result = CpuType.S71500;
                    break;
                default:
                    break;
            }
            return result;
        }
        public void SetupSignalR() { }

        public static SCADAStationConfiguration Deserialize_SCADAStationConfiguration(string filePath)
        {

            string jsonString = File.ReadAllText(filePath);
            var stationConfiguration = new SCADAStationConfiguration();
            stationConfiguration = JsonSerializer.Deserialize<SCADAStationConfiguration>(jsonString);
            return stationConfiguration;
        }
        public static List<TagInfo> Deserialize_Tags(string filePath)
        {

            string jsonString = File.ReadAllText(filePath);
            var listTags = new List<TagInfo>();
            listTags = JsonSerializer.Deserialize<List<TagInfo>>(jsonString);
            return listTags;
        }
        public static List<ConnectDevice> Deserialize_ConnectDevice(string filePath)
        {

            string jsonString = File.ReadAllText(filePath);
            var listDevices = new List<ConnectDevice>();
            listDevices = JsonSerializer.Deserialize<List<ConnectDevice>>(jsonString);
            return listDevices;
        }
        //bool testtagvalue;
        public async void testfunc()
        {
            //if (testtagvalue)
            //{
            //    SendTagValueToClient(11, 0);
            //    testtagvalue = false;
            //}
            //else
            //{
            //    SendTagValueToClient(11, 1);
            //    testtagvalue = true;
            //}
            //SendTagValueToClient(15, 11);
            //await SetupDeviceConnection(listDevices);
            //var alarmpoint = new AlarmPoint(listAlarmSettings.FirstOrDefault(), System.DateTime.Now) ;
            //listAlarmPoints.Add(alarmpoint);
            //OnAlarmedAdded();
            //SendAlarmPointToClient(alarmpoint);
        }

        #region Alarm
        private void AlarmTimer_Tick(object sender, EventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
           // CheckAlarm();
            (sender as System.Timers.Timer).Start();
        }
        private void SendAlarmPointToClient(AlarmPoint alarmpoint)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.UpdateAlarmPoint(alarmpoint);
        }
        private void CheckAlarm()
        {
            foreach (AlarmSetting alarmsetting in listAlarmSettings)
            {
                switch (alarmsetting.LimitMode)
                {
                    case AlarmSetting.LimiType.Higher:
                        if (alarmsetting.TriggerTag != null)
                        {
                            if (alarmsetting.TriggerTag.Value > alarmsetting.Limit)
                            {
                                if (!alarmsetting.IsAlarmed)
                                {
                                    var alarmpoint = new AlarmPoint(alarmsetting, System.DateTime.Now);
                                    listAlarmPoints.Add(alarmpoint);
                                    alarmsetting.IsAlarmed = true;
                                    OnAlarmedAdded();
                                    SendAlarmPointToClient(alarmpoint);
                                }
                            }
                            else
                            {
                                alarmsetting.IsAlarmed = false;
                            }
                        }
                        break;
                    case AlarmSetting.LimiType.Lower:
                        if (alarmsetting.TriggerTag != null)
                        {
                            if (alarmsetting.TriggerTag.Value < alarmsetting.Limit)
                            {
                                if (!alarmsetting.IsAlarmed)
                                {
                                    var alarmpoint = new AlarmPoint(alarmsetting, System.DateTime.Now);
                                    listAlarmPoints.Add(alarmpoint);
                                    alarmsetting.IsAlarmed = true;
                                    OnAlarmedAdded();
                                    SendAlarmPointToClient(alarmpoint);
                                }

                            }
                            else
                            {
                                alarmsetting.IsAlarmed = false;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private event EventHandler _AlarmAdded;
        public event EventHandler AlarmAdded
        {
            add
            {
                _AlarmAdded += value;
            }
            remove
            {
                _AlarmAdded -= value;
            }
        }

        void OnAlarmedAdded()
        {
            if (_AlarmAdded != null)
            {
                _AlarmAdded(this, new EventArgs());
            }
        }
        private void SCADAHub_AcknowledegeAlarmPoint(int alarmpointId)
        {
            listAlarmPoints.RemoveAll(m => m.Id == alarmpointId);
            OnAlarmedAdded();
        }

        public void ACKAlarmPoint(int alarmpointId)
        {
            listAlarmPoints.RemoveAll(m => m.Id == alarmpointId);
            OnAlarmedAdded();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.ACKAlarmPoint(alarmpointId);
        }
        #endregion

        #region Trend
        private void UpdateTagLogging()
        {
            foreach (TagLoggingSetting TagLogging in listTagLoggingSettings)
            {
                TagLogging.currentDuration += 0.5;
                if (TagLogging.currentDuration >= TagLogging.GetTimeCycle())
                {

                    var trendPoint = new TrendPoint(TagLogging.Id, TagLogging.Tag.Value, System.DateTime.Now);
                    SCADAStationDbContext.Instance.TrendPoints.Add(trendPoint);
                    listTrendPoints.Add(trendPoint);
                    SCADAStationDbContext.Instance.SaveChanges();
                    SendTrendPointToClient(trendPoint);
                    TagLogging.currentDuration = 0;
                    //OnAlarmedAdded();
                }
            }
        }

        public void SendTrendPointToClient(TrendPoint trendPoint)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.WriteTrendPoint(trendPoint);
        }
        #endregion
    }
}
