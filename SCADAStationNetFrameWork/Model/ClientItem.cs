using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class ClientItem
    {
        public ClientItem()
        {
            
        }

        public ClientItem(string name, string connectionid)
        {
            Name = name;
            ConnectionID = connectionid;
        }

        public string Name { get; set; }
        public string ConnectionID { get; set; }
    }
}
