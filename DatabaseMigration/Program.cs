using System.Configuration;
using System.Data.Entity;
using DataModeling.DataModel;

namespace DatabaseMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabaseInitializer<EntityContext> initializer = new EntityContextInitializer();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["EntityContext"];
            EntityContext context = new EntityContext(settings.ConnectionString);
            initializer.InitializeDatabase(context);
        }
    }
}
