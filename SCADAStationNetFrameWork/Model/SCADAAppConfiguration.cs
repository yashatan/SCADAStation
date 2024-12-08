﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class SCADAAppConfiguration
    {
        List<ControlData> controlDatas;
        public List<ControlData> ControlDatas { get { return controlDatas; } set { controlDatas = value; } }
        private List<AlarmPoint> currentAlarmPoints;
        List<TrendViewSetting> trendViewSettings;
        public List<TrendViewSetting> TrendViewSettings
        {
            get { return trendViewSettings; }
            set { trendViewSettings = value; }
        }
        List<TagLoggingSetting> tagLoggingSettings;
        public List<TagLoggingSetting> TagLoggingSettings
        {
            get { return tagLoggingSettings; }
            set { tagLoggingSettings = value; }
        }
        public List<AlarmPoint> CurrentAlarmPoints
        {
            get { return currentAlarmPoints; }
            set { currentAlarmPoints = value; }
        }
        private List<TagInfo> tagInfos;
        public List<TagInfo> TagInfos
        {
            get { return tagInfos; }
            set { tagInfos = value; }
        }
        public SCADAAppConfiguration() {
        }

    }
}
