using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFinal;

namespace WPFinal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("Views/Themes/LightTheme.xaml", UriKind.Relative);
            ((App)Application.Current).ChangeTheme(uri);
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("Views/Themes/DarkTheme.xaml", UriKind.Relative);
            ((App)Application.Current).ChangeTheme(uri);
        }

        private void StrangeTheme_Click(Object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("Views/Themes/StrangeTheme.xaml", UriKind.Relative);
            ((App)Application.Current).ChangeTheme(uri);
        }
    }
}