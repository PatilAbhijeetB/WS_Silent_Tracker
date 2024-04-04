using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emageia.Workshiftly.Entity;
using Microsoft.EntityFrameworkCore;

namespace Emageia.Workshiftly.Domain.Concrete.ORM
{
    public class WorkshiftlyContext : DbContext
    { 

        public DbSet<ActivedWindow> ActiveWindows { get; set; }
        public DbSet<AppData> AppDatas { get; set; } 
        public DbSet<CompanyProject> CompanyProjects { get; set; }
       

    }

}
