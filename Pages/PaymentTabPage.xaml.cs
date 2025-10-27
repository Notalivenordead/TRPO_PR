using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница управления платежами
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал для работы с платежами:</para>
    /// <list type="bullet">
    /// <item><description>Просмотр списка всех платежей</description></item>
    /// <item><description>Добавление новых платежей</description></item>
    /// <item><description>Редактирование существующих платежей</description></item>
    /// <item><description>Удаление платежей</description></item>
    /// <item><description>Автоматическое обновление данных при отображении страницы</description></item>
    /// </list>
    /// </remarks>
    public partial class PaymentTabPage : Page
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PaymentTabPage"/>
        /// </summary>
        /// <example>
        /// <code>
        /// // Создание страницы управления платежами
        /// PaymentTabPage paymentPage = new PaymentTabPage();
        /// NavigationService.Navigate(paymentPage);
        /// </code>
        /// </example>
        /// <remarks>
        /// Конструктор загружает данные платежей из базы данных и подписывается на событие изменения видимости страницы
        /// </remarks>
        public PaymentTabPage()
        {
            InitializeComponent();
            DataGridPayment.ItemsSource = Entities.GetContext().Payment.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        /// <summary>
        /// Обрабатывает событие изменения видимости страницы
        /// </summary>
        /// <param name="sender">Источник события - текущая страница</param>
        /// <param name="e">Данные события <see cref="DependencyPropertyChangedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет обновление данных при каждом отображении страницы:</para>
        /// <list type="number">
        /// <item><description>Перезагружает все отслеживаемые сущности Entity Framework</description></item>
        /// <item><description>Обновляет источник данных DataGrid актуальными данными из базы</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Entities.GetContext"/>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridPayment.ItemsSource = Entities.GetContext().Payment.ToList();
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки добавления платежа
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonAdd</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу добавления нового платежа
        /// </remarks>
        /// <seealso cref="AddPaymentPage"/>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddPaymentPage(null));

        /// <summary>
        /// Обрабатывает событие нажатия кнопки удаления платежей
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonDel</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет следующие действия:</para>
        /// <list type="number">
        /// <item><description>Проверяет наличие выбранных платежей для удаления</description></item>
        /// <item><description>Запрашивает подтверждение удаления у пользователя</description></item>
        /// <item><description>Выполняет удаление выбранных платежей из базы данных</description></item>
        /// <item><description>Обновляет отображение DataGrid после удаления</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Возникает при ошибках обновления базы данных
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Возникает при проблемах с контекстом данных Entity Framework
        /// </exception>
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var paymentsForRemoving = DataGridPayment.SelectedItems.Cast<Payment>().ToList();

            if (paymentsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите платежи для удаления");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {paymentsForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Payment.RemoveRange(paymentsForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    DataGridPayment.ItemsSource = Entities.GetContext().Payment.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки редактирования платежа
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonEdit</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет переход на страницу редактирования выбранного платежа.</para>
        /// <para>Для определения редактируемого платежа используется DataContext кнопки, 
        /// который должен быть привязан к объекту <see cref="Payment"/>.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Редактирование платежа через кнопку в DataGrid
        /// ButtonEdit_Click(editButton, new RoutedEventArgs());
        /// </code>
        /// </example>
        /// <seealso cref="AddPaymentPage"/>
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddPaymentPage((sender as Button).DataContext as Payment));
    }
}