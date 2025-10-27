using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Главная страница администратора с навигацией по разделам системы
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет интерфейс для доступа ко всем основным разделам системы:</para>
    /// <list type="bullet">
    /// <item><description>Управление пользователями</description></item>
    /// <item><description>Управление категориями платежей</description></item>
    /// <item><description>Просмотр и управление платежами</description></item>
    /// <item><description>Аналитика и диаграммы</description></item>
    /// </list>
    /// <para>Каждая кнопка на странице обеспечивает переход к соответствующему функциональному модулю.</para>
    /// </remarks>
    public partial class AdminPage : Page
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AdminPage"/>
        /// </summary>
        /// <example>
        /// <code>
        /// // Создание и отображение страницы администратора
        /// AdminPage adminPage = new AdminPage();
        /// NavigationService.Navigate(adminPage);
        /// </code>
        /// </example>
        /// <remarks>
        /// Конструктор выполняет базовую инициализацию компонентов страницы
        /// </remarks>
        public AdminPage() => InitializeComponent();

        /// <summary>
        /// Обрабатывает событие нажатия кнопки перехода на вкладку пользователей
        /// </summary>
        /// <param name="sender">Источник события - кнопка BtnTab1</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу <see cref="UsersTabPage"/> для управления пользователями системы
        /// </remarks>
        /// <seealso cref="UsersTabPage"/>
        private void BtnTab1_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());

        /// <summary>
        /// Обрабатывает событие нажатия кнопки перехода на вкладку категорий
        /// </summary>
        /// <param name="sender">Источник события - кнопка BtnTab2</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу <see cref="CategoryTabPage"/> для управления категориями платежей
        /// </remarks>
        /// <seealso cref="CategoryTabPage"/>
        private void BtnTab2_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new CategoryTabPage());

        /// <summary>
        /// Обрабатывает событие нажатия кнопки перехода на вкладку платежей
        /// </summary>
        /// <param name="sender">Источник события - кнопка BtnTab3</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу <see cref="PaymentTabPage"/> для просмотра и управления платежами
        /// </remarks>
        /// <seealso cref="PaymentTabPage"/>
        private void BtnTab3_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new PaymentTabPage());

        /// <summary>
        /// Обрабатывает событие нажатия кнопки перехода на вкладку диаграмм
        /// </summary>
        /// <param name="sender">Источник события - кнопка BtnTab4</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу <see cref="DiagrammPage"/> для просмотра аналитики и визуализации данных
        /// </remarks>
        /// <seealso cref="DiagrammPage"/>
        private void BtnTab4_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagrammPage());
    }
}