using System.Configuration;
using System.Data.Entity;
using DataModeling;

namespace DatabaseMigration
{
    class Program
    {
        static void Main()
        {
            IDatabaseInitializer<EntityContext> initializer = new EntityContextInitializer();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["EntityContext"];
            using (EntityContext context = new EntityContext(settings.ConnectionString))
            {
                initializer.InitializeDatabase(context);
            }
        }
    }
}
