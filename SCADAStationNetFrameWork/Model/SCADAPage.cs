using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADAStationNetFrameWork;

namespace SCADAStationNetFrameWork
{
    public class SCADAPage : BaseSCADAPage
    {
        public List<ControlData> ControlDatas { get; set; }
        public SCADAPage()
        {
        }
        public SCADAPage(string name)
        {
            Name = name;
            PageType = 0;
        }
    }
}
