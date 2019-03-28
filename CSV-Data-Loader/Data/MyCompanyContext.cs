using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSV_Data_Loader.Models
{
    public class MyCompanyContext : DbContext
    {
        public MyCompanyContext (DbContextOptions<MyCompanyContext> options)
            : base(options)
        {
        }

        public DbSet<CSV_Data_Loader.Models.Person> Person { get; set; }
        public DbSet<CSV_Data_Loader.Models.Product> Product { get; set; }
    }
}
