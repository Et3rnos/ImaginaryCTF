using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace iCTF_Shared_Resources
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseMySql(SharedConfiguration.connectionString,
                    new MySqlServerVersion(new Version(5, 7)));

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
