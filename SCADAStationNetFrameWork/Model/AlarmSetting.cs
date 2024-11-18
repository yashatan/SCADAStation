using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class AlarmSetting
    {
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

		public enum AlarmType
		{
			Warning = 0,
			Error
		}

        public enum LimiType
        {
            Lower = 0,
            Higher
        }

        private AlarmType type;

		public AlarmType Type
		{
			get { return type; }
			set { type = value; }
		}

        private LimiType limitMode;

        public LimiType LimitMode
        {
            get { return limitMode; }
            set { limitMode = value; }
        }

        public TagInfo TriggerTag { get; set; }

		public double Limit { get; set; }

        public bool IsAlarmed  { get; set; }

		public AlarmSetting() { IsAlarmed = false; }



	}
}
