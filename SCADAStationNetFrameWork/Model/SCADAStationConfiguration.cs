using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class SCADAStationConfiguration
    {
        List<ControlData> controlDatas;
        public List<ControlData> ControlDatas
        {
            get { return controlDatas; }
            set { controlDatas = value; }
        }
        List<TagInfo> tagInfos;
        public List<TagInfo> TagInfos
        {
            get { return tagInfos; }
            set { tagInfos = value; }
        }
        List<ConnectDevice> connectDevices;
        public List<ConnectDevice> ConnectDevices
        {
            get { return connectDevices; }
            set { connectDevices = value; }
        }
        List<AlarmSetting> alarmSettings;
        public List<AlarmSetting> AlarmSettings
        {
            get { return alarmSettings; }
            set { alarmSettings = value; }
        }
        public SCADAStationConfiguration()
        {

        }
        public void SetControlDatas(List<ControlData> controlDatas) { this.ControlDatas = controlDatas; }
        public void SetTagInfos(List<TagInfo> tagInfos) { this.TagInfos = tagInfos; }
        public void SetConnectDevices(List<ConnectDevice> connectDevices) { this.ConnectDevices = connectDevices; }
        public void SetAlarmSettings(List<AlarmSetting> alarmSettings) { this.AlarmSettings = alarmSettings; }
    }
}
