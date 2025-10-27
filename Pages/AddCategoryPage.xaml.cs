using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница для добавления и редактирования категорий платежей
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал:</para>
    /// <list type="bullet">
    /// <item><description>Создание новых категорий</description></item>
    /// <item><description>Редактирование существующих категорий</description></item>
    /// <item><description>Валидация вводимых данных</description></item>
    /// <item><description>Сохранение данных в базу данных</description></item>
    /// </list>
    /// </remarks>
    public partial class AddCategoryPage : Page
    {
        /// <summary>
        /// Текущая редактируемая категория
        /// </summary>
        /// <value>
        /// Экземпляр класса <see cref="Category"/> для работы с данными категории
        /// </value>
        private Category _currentCategory = new Category();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AddCategoryPage"/>
        /// </summary>
        /// <param name="selectedCategory">
        /// Категория для редактирования. Если <c>null</c> - создаётся новая категория
        /// </param>
        /// <example>
        /// <code>
        /// // Создание страницы для новой категории
        /// var addPage = new AddCategoryPage(null);
        /// 
        /// // Создание страницы для редактирования существующей категории
        /// var editPage = new AddCategoryPage(existingCategory);
        /// </code>
        /// </example>
        /// <remarks>
        /// Конструктор инициализирует страницу и устанавливает контекст данных
        /// для привязки к элементам управления
        /// </remarks>
        public AddCategoryPage(Category selectedCategory)
        {
            InitializeComponent();
            if (selectedCategory != null)
                _currentCategory = selectedCategory;
            DataContext = _currentCategory;
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки сохранения категории
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonSaveCategory</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет следующие действия:</para>
        /// <list type="number">
        /// <item><description>Проверяет валидность данных</description></item>
        /// <item><description>Выводит сообщения об ошибках при их наличии</description></item>
        /// <item><description>Сохраняет данные в базу данных</description></item>
        /// <item><description>Возвращает на предыдущую страницу при успешном сохранении</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Возникает при ошибках обновления базы данных
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        /// Возникает при ошибках валидации сущностей Entity Framework
        /// </exception>
        /// <seealso cref="Entities.GetContext"/>
        /// <seealso cref="Entities.SaveChanges"/>
        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверка обязательного поля "Название категории"
            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
                errors.AppendLine("Укажите название категории!");

            // Если есть ошибки валидации - показать их и прервать выполнение
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Добавление новой категории в контекст данных, если это создание
            if (_currentCategory.ID == 0)
                Entities.GetContext().Category.Add(_currentCategory);

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
        /// Обрабатывает событие нажатия кнопки очистки поля названия категории
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonClean</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Метод очищает текстовое поле ввода названия категории
        /// </remarks>
        /// <example>
        /// <code>
        /// // Очистка поля ввода
        /// ButtonClean_Click(cleanButton, new RoutedEventArgs());
        /// </code>
        /// </example>
        private void ButtonClean_Click(object sender, RoutedEventArgs e) => TBCategoryName.Text = "";
    }
}