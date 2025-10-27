using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница управления категориями платежей
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал для работы с категориями:</para>
    /// <list type="bullet">
    /// <item><description>Просмотр списка всех категорий</description></item>
    /// <item><description>Добавление новых категорий</description></item>
    /// <item><description>Редактирование существующих категорий</description></item>
    /// <item><description>Удаление категорий</description></item>
    /// <item><description>Автоматическое обновление данных при отображении страницы</description></item>
    /// </list>
    /// </remarks>
    public partial class CategoryTabPage : Page
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CategoryTabPage"/>
        /// </summary>
        /// <example>
        /// <code>
        /// // Создание страницы управления категориями
        /// CategoryTabPage categoryPage = new CategoryTabPage();
        /// NavigationService.Navigate(categoryPage);
        /// </code>
        /// </example>
        /// <remarks>
        /// Конструктор загружает данные категорий из базы данных и подписывается на событие изменения видимости страницы
        /// </remarks>
        public CategoryTabPage()
        {
            InitializeComponent();
            DataGridCategory.ItemsSource = Entities.GetContext().Category.ToList();
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
                DataGridCategory.ItemsSource = Entities.GetContext().Category.ToList();
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки добавления категории
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonAdd</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу добавления новой категории
        /// </remarks>
        /// <seealso cref="AddCategoryPage"/>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddCategoryPage(null));

        /// <summary>
        /// Обрабатывает событие нажатия кнопки удаления категорий
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonDel</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет следующие действия:</para>
        /// <list type="number">
        /// <item><description>Проверяет наличие выбранных категорий для удаления</description></item>
        /// <item><description>Запрашивает подтверждение удаления у пользователя</description></item>
        /// <item><description>Выполняет удаление выбранных категорий из базы данных</description></item>
        /// <item><description>Обновляет отображение DataGrid после удаления</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Возникает при ошибках обновления базы данных, например, при нарушении внешних ключей
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Возникает при проблемах с контекстом данных Entity Framework
        /// </exception>
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var categoriesForRemoving = DataGridCategory.SelectedItems.Cast<Category>().ToList();

            if (categoriesForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите категории для удаления");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {categoriesForRemoving.Count} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Category.RemoveRange(categoriesForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    DataGridCategory.ItemsSource = Entities.GetContext().Category.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки редактирования категории
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonEdit</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет переход на страницу редактирования выбранной категории.</para>
        /// <para>Для определения редактируемой категории используется DataContext кнопки, 
        /// который должен быть привязан к объекту <see cref="Category"/>.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Редактирование категории через кнопку в DataGrid
        /// ButtonEdit_Click(editButton, new RoutedEventArgs());
        /// </code>
        /// </example>
        /// <seealso cref="AddCategoryPage"/>
        private void ButtonEdit_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddCategoryPage((sender as Button).DataContext as Category));
    }
}