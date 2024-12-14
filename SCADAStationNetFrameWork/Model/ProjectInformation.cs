using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADAStationNetFrameWork
{
    public class ProjectInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsNewProject {  get; set; }
        public int MainPageId { get; set; }
        public string GetDBPath()
        {
            string dbPath;
            dbPath = $"{Path.GetDirectoryName(FilePath)}\\{Name}.db";
            return dbPath;
        }
        public ProjectInformation()
        {

        }
    }
}
