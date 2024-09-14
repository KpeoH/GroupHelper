using System.Configuration;
using System.Data;
using System.Windows;
using WPFinal;
using WPFinal.ViewModels;

namespace WPFinal
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public void ChangeTheme(Uri uri)
        {
            ResourceDictionary theme = new ResourceDictionary { Source = uri };
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(theme);
        }


    }
}
