using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница для добавления и редактирования пользователей
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал:</para>
    /// <list type="bullet">
    /// <item><description>Создание новых пользователей</description></item>
    /// <item><description>Редактирование существующих пользователей</description></item>
    /// <item><description>Валидация данных пользователя</description></item>
    /// <item><description>Хеширование паролей с использованием SHA1</description></item>
    /// <item><description>Проверка сложности пароля</description></item>
    /// <item><description>Сохранение данных в базу данных</description></item>
    /// </list>
    /// </remarks>
    public partial class AddUserPage : Page
    {
        /// <summary>
        /// Текущий редактируемый пользователь
        /// </summary>
        /// <value>
        /// Экземпляр класса <see cref="User"/> для работы с данными пользователя
        /// </value>
        private User _currentUser = new User();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AddUserPage"/>
        /// </summary>
        /// <param name="selectedUser">
        /// Пользователь для редактирования. Если <c>null</c> - создаётся новый пользователь
        /// </param>
        /// <example>
        /// <code>
        /// // Создание страницы для нового пользователя
        /// var addPage = new AddUserPage(null);
        /// 
        /// // Создание страницы для редактирования существующего пользователя
        /// var editPage = new AddUserPage(existingUser);
        /// </code>
        /// </example>
        /// <remarks>
        /// Конструктор инициализирует страницу и устанавливает контекст данных.
        /// Для существующих пользователей предварительно заполняется поле роли.
        /// </remarks>
        public AddUserPage(User selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
                _currentUser = selectedUser;
            DataContext = _currentUser;
            if (!string.IsNullOrWhiteSpace(_currentUser.Role))
            {
                cmbRole.Text = _currentUser.Role;
            }
        }

        /// <summary>
        /// Вычисляет хеш SHA1 для указанного пароля
        /// </summary>
        /// <param name="password">Пароль для хеширования</param>
        /// <returns>Хеш-строка в шестнадцатеричном формате</returns>
        /// <remarks>
        /// <para>Метод использует алгоритм SHA1 для создания хеша пароля.</para>
        /// <para>Хеш представляется в виде строки шестнадцатеричных символов.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// string passwordHash = AddUserPage.GetHash("myPassword123");
        /// // Возвращает хеш вида "A9993E364706816ABA3E25717850C26C9CD0D89D"
        /// </code>
        /// </example>
        /// <seealso cref="SHA1"/>
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки сохранения пользователя
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonSave</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет следующие проверки:</para>
        /// <list type="number">
        /// <item><description>Проверяет обязательные поля (логин, пароль, роль, ФИО)</description></item>
        /// <item><description>Для новых пользователей или при изменении пароля проверяет сложность пароля</description></item>
        /// <item><description>Хеширует пароль перед сохранением</description></item>
        /// <item><description>Сохраняет данные в базу данных</description></item>
        /// </list>
        /// <para>Требования к паролю:</para>
        /// <list type="bullet">
        /// <item><description>Минимум 6 символов</description></item>
        /// <item><description>Только английские буквы и цифры</description></item>
        /// <item><description>Хотя бы одна цифра</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Возникает при ошибках обновления базы данных
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        /// Возникает при ошибках валидации сущностей Entity Framework
        /// </exception>
        /// <exception cref="CryptographicException">
        /// Возникает при ошибках в процессе хеширования пароля
        /// </exception>
        /// <seealso cref="GetHash"/>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Text)) errors.AppendLine("Укажите пароль!");
            if (string.IsNullOrWhiteSpace(cmbRole.Text)) errors.AppendLine("Выберите роль!");
            else _currentUser.Role = cmbRole.Text;
            if (string.IsNullOrWhiteSpace(_currentUser.FIO)) errors.AppendLine("Укажите ФИО");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка пароля только для новых пользователей или при изменении пароля
            if (_currentUser.ID == 0 || !string.IsNullOrEmpty(TBPass.Text))
            {
                string password = TBPass.Text;

                if (password.Length < 6)
                {
                    MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                    return;
                }

                bool en = true;
                bool number = false;
                foreach (char c in password)
                {
                    if (c >= '0' && c <= '9') number = true;
                    else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))) en = false;
                }

                if (!en)
                {
                    MessageBox.Show("Используйте только английскую раскладку!");
                    return;
                }

                if (!number)
                {
                    MessageBox.Show("Добавьте хотя бы одну цифру!");
                    return;
                }

                // Хешируем пароль перед сохранением
                _currentUser.Password = GetHash(password);
            }

            if (_currentUser.ID == 0)
                Entities.GetContext().User.Add(_currentUser);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        /// <item><description>Логин</description></item>
        /// <item><description>Пароль</description></item>
        /// <item><description>Роль</description></item>
        /// <item><description>ФИО</description></item>
        /// <item><description>Фото</description></item>
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
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = -1;
            TBFio.Text = "";
            TBPhoto.Text = "";
        }
    }
}