using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница для добавления и редактирования платежей
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал:</para>
    /// <list type="bullet">
    /// <item><description>Создание новых платежей</description></item>
    /// <item><description>Редактирование существующих платежей</description></item>
    /// <item><description>Валидация всех обязательных полей</description></item>
    /// <item><description>Загрузка списков категорий и пользователей</description></item>
    /// <item><description>Сохранение данных в базу данных</description></item>
    /// </list>
    /// </remarks>
    public partial class AddPaymentPage : Page
    {
        /// <summary>
        /// Текущий редактируемый платеж
        /// </summary>
        /// <value>
        /// Экземпляр класса <see cref="Payment"/> для работы с данными платежа
        /// </value>
        private Payment _currentPayment = new Payment();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AddPaymentPage"/>
        /// </summary>
        /// <param name="selectedPayment">
        /// Платеж для редактирования. Если <c>null</c> - создаётся новый платеж
        /// </param>
        /// <example>
        /// <code>
        /// // Создание страницы для нового платежа
        /// var addPage = new AddPaymentPage(null);
        /// 
        /// // Создание страницы для редактирования существующего платежа
        /// var editPage = new AddPaymentPage(existingPayment);
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Конструктор выполняет:</para>
        /// <list type="number">
        /// <item><description>Загрузку списка категорий из базы данных</description></item>
        /// <item><description>Загрузку списка пользователей из базы данных</description></item>
        /// <item><description>Установку текущей даты для новых платежей</description></item>
        /// <item><description>Настройку контекста данных для привязки</description></item>
        /// </list>
        /// </remarks>
        public AddPaymentPage(Payment selectedPayment)
        {
            InitializeComponent();

            // Загрузка данных для выпадающих списков
            CBCategory.ItemsSource = Entities.GetContext().Category.ToList();
            CBUser.ItemsSource = Entities.GetContext().User.ToList();

            if (selectedPayment != null)
                _currentPayment = selectedPayment;

            DataContext = _currentPayment;

            // Установка текущей даты для новых платежей
            if (_currentPayment.ID == 0)
                _currentPayment.Date = DateTime.Now;
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки сохранения платежа
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonSave</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет комплексную проверку данных:</para>
        /// <list type="bullet">
        /// <item><description>Проверяет выбор пользователя</description></item>
        /// <item><description>Проверяет выбор категории</description></item>
        /// <item><description>Проверяет наличие названия платежа</description></item>
        /// <item><description>Проверяет указание даты</description></item>
        /// <item><description>Проверяет корректность количества и цены</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Возникает при ошибках обновления базы данных
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        /// Возникает при ошибках валидации сущностей Entity Framework
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Возникает при отсутствии необходимых данных
        /// </exception>
        /// <seealso cref="Entities.GetContext"/>
        /// <seealso cref="Entities.SaveChanges"/>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация обязательных полей
            if (_currentPayment.UserID == 0)
                errors.AppendLine("Выберите пользователя!");
            if (_currentPayment.CategoryID == 0)
                errors.AppendLine("Выберите категорию!");
            if (string.IsNullOrWhiteSpace(_currentPayment.Name))
                errors.AppendLine("Укажите название платежа!");
            if (_currentPayment.Date == null)
                errors.AppendLine("Укажите дату!");
            if (_currentPayment.Num <= 0)
                errors.AppendLine("Количество должно быть больше 0!");
            if (_currentPayment.Price <= 0)
                errors.AppendLine("Цена должна быть больше 0!");

            // Если есть ошибки валидации - показать их и прервать выполнение
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Добавление нового платежа в контекст данных, если это создание
            if (_currentPayment.ID == 0)
                Entities.GetContext().Payment.Add(_currentPayment);

            try
            {
                // Сохранение изменений в базе данных
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки очистки полей формы
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonClean</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод очищает все поля ввода на форме:</para>
        /// <list type="bullet">
        /// <item><description>Название платежа</description></item>
        /// <item><description>Количество</description></item>
        /// <item><description>Цену</description></item>
        /// <item><description>Дату (устанавливает текущую)</description></item>
        /// <item><description>Выбор категории</description></item>
        /// <item><description>Выбор пользователя</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Очистка всех полей формы
        /// ButtonClean_Click(cleanButton, new RoutedEventArgs());
        /// </code>
        /// </example>
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBPaymentName.Text = "";
            TBCount.Text = "";
            TBPrice.Text = "";
            DPDate.SelectedDate = DateTime.Now;
            CBCategory.SelectedIndex = -1;
            CBUser.SelectedIndex = -1;
        }
    }
}