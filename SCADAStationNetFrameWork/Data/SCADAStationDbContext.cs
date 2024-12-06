using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SCADAStationNetFrameWork
{
    public class SCADAStationDbContext : DbContext
    {
        public DbSet<TrendPoint> TrendPoints { get; set; }
        public SCADAStationDbContext(DbConnection connection)
                    : base(connection, true)
        {
        }
        private SCADAStationDbContext() : base(new SQLiteConnection($"Data Source={FunctionalLab.currentProjectInformation.GetDBPath()};New=False;Compress=True;UTF8Encoding=True"), true)
        {

        }
        private static SCADAStationDbContext instance;
        public static SCADAStationDbContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SCADAStationDbContext();
                }
                return instance;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

    }
}


