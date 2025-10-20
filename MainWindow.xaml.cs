using _222_Busin.Pages;
using System;
using System.ComponentModel;
using System.Windows;

namespace _222_Busin
{
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            SwitchToTheme1();
            LoadAuthPage();
        }

        private void LoadAuthPage()
        {
            MainFrame.Navigate(new AuthPage());
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isDarkTheme)
            {
                SwitchToTheme1();
                ThemeButton.Content = "Тёмная тема";
                _isDarkTheme = false;
            }
            else
            {
                SwitchToTheme2();
                ThemeButton.Content = "Светлая тема";
                _isDarkTheme = true;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }
    }
}
