using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using WebProject.Models;

namespace WebProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<userNewField>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(databaseCreator != null)
                {
                    if(!databaseCreator.CanConnect())
                    {
                        databaseCreator.Create();
                    }
                    if(!databaseCreator.HasTables())
                    {
                        databaseCreator.CreateTables();
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            

            }
        }
    }
}
