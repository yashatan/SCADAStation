using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class AlarmPoint
    {
        private static int currentId = 0;
        private int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private AlarmSetting.AlarmType type;

        public AlarmSetting.AlarmType Type
        {
            get { return type; }
            set { type = value; }
        }

        private DateTime timeStamp;

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        public AlarmPoint()
        {
            
        }

        public AlarmPoint(AlarmSetting alarmsetting, DateTime timestamp)
        {
            this.Id = ++currentId;
            this.name = alarmsetting.Name;
            this.text = alarmsetting.Text;
            this.type = alarmsetting.Type; 
            this.timeStamp = timestamp;
        }
    }
}
