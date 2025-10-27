using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница изменения пароля пользователя
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал для безопасной смены пароля:</para>
    /// <list type="bullet">
    /// <item><description>Проверка подлинности пользователя по логину и старому паролю</description></item>
    /// <item><description>Валидация нового пароля на соответствие требованиям безопасности</description></item>
    /// <item><description>Хеширование паролей с использованием алгоритма SHA1</description></item>
    /// <item><description>Подтверждение нового пароля для предотвращения опечаток</description></item>
    /// </list>
    /// </remarks>
    public partial class ChangePassPage : Page
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChangePassPage"/>
        /// </summary>
        /// <example>
        /// <code>
        /// // Создание страницы смены пароля
        /// ChangePassPage changePassPage = new ChangePassPage();
        /// NavigationService.Navigate(changePassPage);
        /// </code>
        /// </example>
        public ChangePassPage() => InitializeComponent();

        /// <summary>
        /// Вычисляет хеш SHA1 для указанной строки
        /// </summary>
        /// <param name="s">Входная строка для хеширования</param>
        /// <returns>Хеш-строка в шестнадцатеричном формате</returns>
        /// <remarks>
        /// <para>Метод использует алгоритм SHA1 для создания безопасного хеша.</para>
        /// <para>Результат представляется в виде строки шестнадцатеричных символов в верхнем регистре.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// string hash = GetHash("myPassword123");
        /// // Возвращает хеш вида "A9993E364706816ABA3E25717850C26C9CD0D89D"
        /// </code>
        /// </example>
        /// <seealso cref="SHA1"/>
        private string GetHash(string s)
        {
            using (var sha = SHA1.Create())
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(s)).Select(x => x.ToString("X2")));
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки изменения пароля
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonChange</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод выполняет многоуровневую проверку данных:</para>
        /// <list type="number">
        /// <item><description>Проверяет заполнение всех обязательных полей</description></item>
        /// <item><description>Проверяет существование пользователя с указанным логином</description></item>
        /// <item><description>Сверяет старый пароль с сохраненным в базе данных</description></item>
        /// <item><description>Проверяет минимальную длину нового пароля (6 символов)</description></item>
        /// <item><description>Сравнивает новый пароль и его подтверждение</description></item>
        /// <item><description>Сохраняет новый хешированный пароль в базу данных</description></item>
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
        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения всех обязательных полей
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                string.IsNullOrWhiteSpace(TxtOldPass.Password) ||
                string.IsNullOrWhiteSpace(TxtNewPass.Password) ||
                string.IsNullOrWhiteSpace(TxtNewPassRepeat.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var db = Entities.GetContext();
            var user = db.User.FirstOrDefault(x => x.Login == TxtLogin.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            // Проверка корректности старого пароля
            if (user.Password != GetHash(TxtOldPass.Password))
            {
                MessageBox.Show("Старый пароль неверен!");
                return;
            }

            // Проверка минимальной длины нового пароля
            if (TxtNewPass.Password.Length < 6)
            {
                MessageBox.Show("Минимальная длина пароля — 6 символов!");
                return;
            }

            // Подтверждение нового пароля
            if (TxtNewPass.Password != TxtNewPassRepeat.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // Сохранение нового хешированного пароля
            user.Password = GetHash(TxtNewPass.Password);
            db.SaveChanges();
            MessageBox.Show("Пароль успешно изменён!");
            NavigationService?.GoBack();
        }
    }
}