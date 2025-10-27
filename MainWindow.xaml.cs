using _222_Busin.Pages;
using System;
using System.ComponentModel;
using System.Windows;

namespace _222_Busin
{
    /// <summary>
    /// Главное окно приложения для учёта платежей
    /// </summary>
    /// <remarks>
    /// <para>Окно предоставляет функционал:</para>
    /// <list type="bullet">
    /// <item><description>Навигация между страницами</description></item>
    /// <item><description>Переключение между светлой и тёмной темами</description></item>
    /// <item><description>Отображение текущего времени</description></item>
    /// <item><description>Подтверждение закрытия приложения</description></item>
    /// </list>
    /// </remarks>
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = false;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainWindow"/>
        /// </summary>
        /// <example>
        /// <code>
        /// var mainWindow = new MainWindow();
        /// mainWindow.Show();
        /// </code>
        /// </example>
        public MainWindow()
        {
            InitializeComponent();
            SwitchToTheme1();
            LoadAuthPage();
        }

        /// <summary>
        /// Загружает страницу авторизации в основной фрейм
        /// </summary>
        /// <remarks>
        /// Метод используется для первоначальной загрузки страницы аутентификации
        /// при запуске приложения
        /// </remarks>
        private void LoadAuthPage()
        {
            MainFrame.Navigate(new AuthPage());
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки смены темы
        /// </summary>
        /// <param name="sender">Источник события - кнопка ThemeButton</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <example>
        /// <code>
        /// ThemeButton_Click(themeButton, new RoutedEventArgs());
        /// </code>
        /// </example>
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

        /// <summary>
        /// Переключает на первую тему (светлая тема)
        /// </summary>
        /// <remarks>
        /// Загружает ресурсы из файла Styles1.xaml и применяет их ко всему приложению
        /// </remarks>
        /// <exception cref="System.IO.IOException">
        /// Возникает при невозможности загрузить файл Styles1.xaml
        /// </exception>
        private void SwitchToTheme1()
        {
            var uri = new Uri("Styles1.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        /// <summary>
        /// Переключает на вторую тему (тёмная тема)
        /// </summary>
        /// <remarks>
        /// Загружает ресурсы из файла Styles2.xaml и применяет их ко всему приложению
        /// </remarks>
        /// <exception cref="System.IO.IOException">
        /// Возникает при невозможности загрузить файл Styles2.xaml
        /// </exception>
        private void SwitchToTheme2()
        {
            var uri = new Uri("Styles2.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        /// <summary>
        /// Обрабатывает событие загрузки окна
        /// </summary>
        /// <param name="sender">Источник события - главное окно</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Инициализирует таймер для обновления отображения текущего времени
        /// </remarks>
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

        /// <summary>
        /// Обрабатывает событие закрытия окна
        /// </summary>
        /// <param name="sender">Источник события - главное окно</param>
        /// <param name="e">Данные события <see cref="CancelEventArgs"/></param>
        /// <remarks>
        /// Запрашивает подтверждение пользователя перед закрытием приложения
        /// </remarks>
        /// <example>
        /// <code>
        /// // При закрытии окна показывается диалог подтверждения
        /// </code>
        /// </example>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки "Назад"
        /// </summary>
        /// <param name="sender">Источник события - кнопка BackButton</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Возвращает на предыдущую страницу в навигации, если это возможно
        /// </remarks>
        /// <seealso cref="Frame.GoBack"/>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        /// <summary>
        /// Получает или устанавливает значение, указывающее текущую тему
        /// </summary>
        /// <value>
        /// <c>true</c> если активна тёмная тема, <c>false</c> если светлая тема
        /// </value>
        /// <permission cref="System.Security.Permissions.UIPermission">
        /// Требуется для изменения визуальных стилей приложения
        /// </permission>
        private bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set { _isDarkTheme = value; }
        }
    }
}