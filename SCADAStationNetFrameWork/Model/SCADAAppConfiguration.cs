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

        public List<AlarmPoint> CurrentAlarmPoints
        {
            get { return currentAlarmPoints; }
            set { currentAlarmPoints = value; }
        }

        public SCADAAppConfiguration() {
        }

    }
}
