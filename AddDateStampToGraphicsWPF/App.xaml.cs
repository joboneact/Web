using System.Windows;

namespace AddDateToGraphicWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
