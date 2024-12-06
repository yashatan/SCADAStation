using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class SCADAProjectInformation
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }

        public ICollection<TrendSetting> TrendSettings { get; set; }
    }
}
