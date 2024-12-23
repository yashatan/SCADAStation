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
using EasyModbus;
using System.Timers;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using S7.Net.Types;
using System.Runtime.Serialization.Formatters.Binary;

using Opc.Ua;
using Opc.Ua.Client;
using System.Diagnostics.Eventing.Reader;



namespace SCADAStationNetFrameWork
{

    public class FunctionalLab
    {
        public List<TagInfo> listTags;
        public List<ConnectDevice> listDevices;
        public List<AlarmSetting> listAlarmSettings;
        public List<AlarmPoint> listAlarmPoints;
        public List<TagLoggingSetting> listTagLoggingSettings;
        public List<ControlDevice> listcontrolDevices;
        public List<ClientItem> listClient;
        SCADAAppConfiguration mSCADAConfiguration;
        Dictionary<int, ControlDevice> DictionaryControlDevices;
        private IDisposable _signalR;
        public string url;
        string filePath_SCADAStationConfiguration;
        private SCADAStationConfiguration mSCADAStationConfiguration;
        public static ProjectInformation currentProjectInformation;
        public List<TrendPoint> listTrendPoints;
        public bool LoadFileStatus {  get; set; }
        public FunctionalLab()
        {
            filePath_SCADAStationConfiguration = "C:\\Users\\Admin\\Work\\DemoSCADA\\DemoSCADAStation.json";
            //filePath_SCADAStationConfiguration = "E:\\SCADAProject\\DemoSCADA\\DemoSCADAStation.json";
            Initialize();
        }

        public FunctionalLab(string fileName)
        {
            filePath_SCADAStationConfiguration = fileName;
            Initialize();
        }

        private void Initialize()
        {
            DictionaryControlDevices = new Dictionary<int, ControlDevice>();
            listClient = new List<ClientItem>();
            LoadFileStatus = LoadConfigFile(filePath_SCADAStationConfiguration);
            if (LoadFileStatus)
            {
                listTrendPoints = SCADAStationDbContext.Instance.TrendPoints.ToList();
                StartServer();
                SCADAHub.ClientConnected += SCADAHub_ClientConnected;
                SCADAHub.ClientDisconnected += SCADAHub_ClientDisconnected;
                SCADAHub.ClientWriteTag += SCADAHub_ClientWriteTag;
                SCADAHub.AcknowledgeAlarmPoint += SCADAHub_AcknowledegeAlarmPoint;
                SCADAHub.ClientGetTrendPoints += SCADAHub_GetTrendPoints;
                SCADAHub.ClientNameChanged += SCADAHub_ClientNameChanged;
            }

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
            await SetupDeviceConnection();
            //var alarmpoint = new AlarmPoint(listAlarmSettings.FirstOrDefault(), System.DateTime.Now) ;
            //listAlarmPoints.Add(alarmpoint);
            //OnAlarmedAdded();
            //SendAlarmPointToClient(alarmpoint);
        }
        public async void testfunc2()
        {
            //TagInfo taginfo = listTags.Where(p => p.Name == "testopcwrite").FirstOrDefault();
            //if (listControlDevices.ContainsKey(taginfo.ConnectDevice.Id))
            //{
            //    var device = listControlDevices[taginfo.ConnectDevice.Id];
            //    if (device != null)
            //    {
            //        device.WriteTag(taginfo, Convert);
            //    }
            //}
        }
        #region SCADA
        public bool LoadConfigFile(string fileName)
        {
            Trace.WriteLine($"Starting reading file");
            mSCADAStationConfiguration = new SCADAStationConfiguration();
            mSCADAStationConfiguration = Deserialize_SCADAStationConfiguration(fileName);
            if ( mSCADAStationConfiguration == null)
            {
                return false;
            }
            listTags = mSCADAStationConfiguration.TagInfos;
            listDevices = mSCADAStationConfiguration.ConnectDevices;
            listAlarmSettings = mSCADAStationConfiguration.AlarmSettings;
            listTagLoggingSettings = mSCADAStationConfiguration.TagLoggingSettings;
            listAlarmPoints = new List<AlarmPoint>();
            currentProjectInformation = mSCADAStationConfiguration.ProjectInformation;

            listcontrolDevices = new List<ControlDevice>();
            foreach (var device in listDevices)
            {
                var controldevice = new ControlDevice(device);
                listcontrolDevices.Add(controldevice);
            }
            MappingTagInfo();
            return true;
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

        public static SCADAStationConfiguration Deserialize_SCADAStationConfiguration(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var stationConfiguration = new SCADAStationConfiguration();
                stationConfiguration = JsonSerializer.Deserialize<SCADAStationConfiguration>(jsonString);
                return stationConfiguration;
            }
            catch (Exception e)
            {
                MessageBox.Show("Configuration file is invalid. Plesae check again");
                return null;
            }

        }
        //public static List<TagInfo> Deserialize_Tags(string filePath)
        //{

        //    string jsonString = File.ReadAllText(filePath);
        //    var listTags = new List<TagInfo>();
        //    listTags = JsonSerializer.Deserialize<List<TagInfo>>(jsonString);
        //    return listTags;
        //}
        //public static List<ConnectDevice> Deserialize_ConnectDevice(string filePath)
        //{

        //    string jsonString = File.ReadAllText(filePath);
        //    var listDevices = new List<ConnectDevice>();
        //    listDevices = JsonSerializer.Deserialize<List<ConnectDevice>>(jsonString);
        //    return listDevices;
        //}
        #endregion

        #region PLC
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

        public async Task SetupDeviceConnection()
        {
            foreach (var controlDevice in listcontrolDevices)
            {
                await controlDevice.Connect();

                if (controlDevice.ConnectionStatus == "Connected")
                {
                    DictionaryControlDevices.Add(controlDevice.DeviceInfo.Id, controlDevice);
                    OnDeviceUpdated();
                    if (controlDevice.DeviceInfo.ConnectionType == (int)ConnectDevice.emConnectionType.emOPCUA)
                    {
                        controlDevice.SubscribeTags(listTags);
                    }
                }
            }
            if (DictionaryControlDevices.Count > 0)
            {
                SetUpTimer();
            }
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
                if (DictionaryControlDevices.ContainsKey(tagInfo.ConnectDevice.Id))
                {
                    var device = DictionaryControlDevices[tagInfo.ConnectDevice.Id];
                    if (device != null)
                    {
                        long temp = tagInfo.Data;
                        device.ReadTag(tagInfo);
                        if ((temp != tagInfo.Data) || tagInfo.ConnectDevice.ConnectionType == (int)ConnectDevice.emConnectionType.emOPCUA)
                        {
                            SendTagValueToClient(tagInfo.Id, tagInfo.Data);
                            OnTagUpdated();
                        }
                    }
                }


            }
        }
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj); return ms.ToArray();
            }
        }
        #endregion

        #region Alarm
        private void SendAlarmPointToClient(AlarmPoint alarmpoint)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.UpdateAlarmPoint(alarmpoint);
        }
        private void CheckAlarm()
        {
            foreach (AlarmSetting alarmsetting in listAlarmSettings)
            {
                int value = 0;
                if (alarmsetting.TriggerTag.Type == TagInfo.TagType.eReal)
                {
                    var temp = Convert.ToSingle(alarmsetting.TriggerTag.Value);
                    value = Convert.ToInt32(temp);
                }
                else if (alarmsetting.TriggerTag.Type == TagInfo.TagType.eDouble)
                {
                    var temp = Convert.ToDouble(alarmsetting.TriggerTag.Value);
                    value = Convert.ToInt32(temp);

                }
                else
                {
                    value = Convert.ToInt32(alarmsetting.TriggerTag.Value);
                }
                switch (alarmsetting.LimitMode)
                {
                    case AlarmSetting.LimiType.Higher:
                        if (alarmsetting.TriggerTag != null)
                        {

                            if (value > alarmsetting.Limit)
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
                            if (value < alarmsetting.Limit)
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
        #endregion

        #region Trend
        private void UpdateTagLogging()
        {
            foreach (TagLoggingSetting TagLogging in listTagLoggingSettings)
            {
                TagLogging.currentDuration += 0.5;
                if (TagLogging.currentDuration >= TagLogging.GetTimeCycle())
                {
                    double value = 0;
                    if (TagLogging.Tag.Type == TagInfo.TagType.eReal)
                    {
                        var temp = Convert.ToSingle(TagLogging.Tag.Value);
                        value = (double) temp;
                    }
                    else if (TagLogging.Tag.Type == TagInfo.TagType.eDouble)
                    {
                        var temp = Convert.ToDouble(TagLogging.Tag.Value);
                        value = temp;

                    }
                    else
                    {
                        value = (double) Convert.ToInt32(TagLogging.Tag.Value);
                    }
                    var trendPoint = new TrendPoint(TagLogging.Id, value, System.DateTime.Now);
                    SCADAStationDbContext.Instance.TrendPoints.Add(trendPoint);
                    listTrendPoints.Add(trendPoint);
                    SCADAStationDbContext.Instance.SaveChanges();
                    SendTrendPointToClient(trendPoint);
                    TagLogging.currentDuration = 0;
                    OnAlarmedAdded();
                }
            }
        }

        public void SendTrendPointToClient(TrendPoint trendPoint)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.WriteTrendPoint(trendPoint);
        }
        #endregion

        #region SignalR
        public void ACKAlarmPoint(int alarmpointId)
        {
            listAlarmPoints.RemoveAll(m => m.Id == alarmpointId);
            OnAlarmedAdded();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.ACKAlarmPoint(alarmpointId);
        }
        private void SendTagValueToClient(int tagID, long value)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.UpdateTags(tagID, value);
        }
        public void StartServer()
        {
            url = $"http://{GetLocalIPAddress()}:8088/{currentProjectInformation.Name}/signalr";
            var connect_url = $"http://{GetLocalIPAddress()}:8088/{currentProjectInformation.Name}";
            _signalR = WebApp.Start<Startup>(connect_url);

        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString().Contains("0.50")) { continue; }
                    if (ip.ToString().Contains("172.")) { continue; }
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        private void SCADAHub_ClientWriteTag(int tagid, object value)
        {
            TagInfo taginfo = listTags.Where(p => p.Id == tagid).FirstOrDefault();
            if (DictionaryControlDevices.ContainsKey(taginfo.ConnectDevice.Id))
            {
                var device = DictionaryControlDevices[taginfo.ConnectDevice.Id];
                if (device != null)
                {
                    device.WriteTag(taginfo, value);
                }
            }


        }
        private void SendSCADAConfigurationToApp()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            mSCADAConfiguration = new SCADAAppConfiguration();
            mSCADAConfiguration.SCADAPages = mSCADAStationConfiguration.SCADAPages;
            mSCADAConfiguration.TagInfos = mSCADAStationConfiguration.TagInfos;
            mSCADAConfiguration.TrendViewSettings = mSCADAStationConfiguration.TrendViewSettings;
            mSCADAConfiguration.TagLoggingSettings = mSCADAStationConfiguration.TagLoggingSettings;
            if (listAlarmPoints.Count > 0)
            {
                mSCADAConfiguration.CurrentAlarmPoints = listAlarmPoints;
            }

            hubContext.Clients.All.DownloadSCADAConfig(mSCADAConfiguration);
            foreach (var tag in listTags)
            {
                SendTagValueToClient(tag.Id, tag.Data);
            }
        }
        private void SCADAHub_GetTrendPoints(int tagloggingid)
        {
            List<TrendPoint> trendPointsById = new List<TrendPoint>(SCADAStationDbContext.Instance.TrendPoints.Where(x => x.TagLoggingId == tagloggingid).ToList());
            var trendPoints = new List<TrendPoint>(trendPointsById.Skip(Math.Max(0, trendPointsById.Count - 60)).ToList());
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SCADAHub>();
            hubContext.Clients.All.WriteCurrentTrendPoints(trendPoints);
        }

        private void SCADAHub_ClientConnected(string clientId)
        {
            listClient.Add(new ClientItem(clientId, clientId));
            SendSCADAConfigurationToApp();
        }
        private void SCADAHub_ClientDisconnected(string clientId)
        {
            //listClient.Add(new ClientItem(clientId, clientId));
        }

        private void SCADAHub_ClientNameChanged(string clientId, string newName)
        {
            var client = listClient.FirstOrDefault(p=>p.ConnectionID == clientId);
            if (client != null)
            {
                client.Name = newName;
                OnNewClientConnected();
            }
        }
        #endregion

        #region Event
        private event EventHandler _TagUpdated;
        public event EventHandler TagUpdated
        {
            add
            {
                _TagUpdated += value;
            }
            remove
            {
                _TagUpdated -= value;
            }
        }

        void OnTagUpdated()
        {
            if (_TagUpdated != null)
            {
                _TagUpdated(this, new EventArgs());
            }
        }

        private event EventHandler _DeviceUpdated;
        public event EventHandler DeviceUpdated
        {
            add
            {
                _DeviceUpdated += value;
            }
            remove
            {
                _DeviceUpdated -= value;
            }
        }

        void OnDeviceUpdated()
        {
            if (_DeviceUpdated != null)
            {
                _DeviceUpdated(this, new EventArgs());
            }
        }

        private event EventHandler _NewClientConnected;
        public event EventHandler NewClientConnected
        {
            add
            {
                _NewClientConnected += value;
            }
            remove
            {
                _DeviceUpdated -= value;
            }
        }

        void OnNewClientConnected()
        {
            if (_NewClientConnected != null)
            {
                _NewClientConnected(this, new EventArgs());
            }
        }
        #endregion
    }
}
