using System;
using System.Windows;

namespace _222_Busin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SwitchToTheme1();
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeToggle.IsChecked == true)
            {
                SwitchToTheme2();
                ThemeToggle.Content = "Светлая тема";
            }
            else
            {
                SwitchToTheme1();
                ThemeToggle.Content = "Тёмная тема";
            }
        }

        private void SwitchToTheme1()
        {
            var uri = new Uri("Styles1.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        private void SwitchToTheme2()
        {
            var uri = new Uri("Styles2.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1),
                IsEnabled = true
            };
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно ? ", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                PaymentWindow.Close();
        }
    }
}
