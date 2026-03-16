using System.Windows;
using STRELA_MED.Data;

namespace STRELA_MED
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}