using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace RIP
{
    class AppContext: DbContext
    {
        public AppContext()
            : base("RIPDBEntities")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
    }
}
