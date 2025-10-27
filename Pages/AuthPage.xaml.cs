using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    /// <summary>
    /// Страница аутентификации пользователей в системе
    /// </summary>
    /// <remarks>
    /// <para>Страница предоставляет функционал:</para>
    /// <list type="bullet">
    /// <item><description>Авторизация пользователей по логину и паролю</description></item>
    /// <item><description>Защита от брутфорс-атак с помощью CAPTCHA</description></item>
    /// <item><description>Хеширование паролей с использованием SHA1</description></item>
    /// <item><description>Перенаправление пользователей по ролям</description></item>
    /// <item><description>Регистрация новых пользователей</description></item>
    /// <item><description>Восстановление пароля</description></item>
    /// </list>
    /// </remarks>
    public partial class AuthPage : Page
    {
        /// <summary>
        /// Счетчик неудачных попыток входа
        /// </summary>
        /// <value>
        /// Количество последовательных неудачных попыток аутентификации
        /// </value>
        private int failedAttempts = 0;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AuthPage"/>
        /// </summary>
        /// <example>
        /// <code>
        /// // Создание страницы аутентификации
        /// AuthPage authPage = new AuthPage();
        /// NavigationService.Navigate(authPage);
        /// </code>
        /// </example>
        public AuthPage() => InitializeComponent();

        /// <summary>
        /// Вычисляет хеш SHA1 для указанного пароля
        /// </summary>
        /// <param name="password">Пароль для хеширования</param>
        /// <returns>Хеш-строка в шестнадцатеричном формате</returns>
        /// <remarks>
        /// <para>Метод использует алгоритм SHA1 для создания безопасного хеша пароля.</para>
        /// <para>Хеш представляется в виде строки шестнадцатеричных символов в верхнем регистре.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// string passwordHash = AuthPage.GetHash("myPassword123");
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
        /// Обрабатывает событие нажатия кнопки входа в систему
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonEnter</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Процесс аутентификации включает следующие шаги:</para>
        /// <list type="number">
        /// <item><description>Проверка заполнения обязательных полей</description></item>
        /// <item><description>Валидация CAPTCHA при необходимости</description></item>
        /// <item><description>Хеширование введенного пароля</description></item>
        /// <item><description>Поиск пользователя в базе данных</description></item>
        /// <item><description>Перенаправление по ролям при успешной аутентификации</description></item>
        /// </list>
        /// <para>После 3 неудачных попыток активируется CAPTCHA для защиты от брутфорс-атак.</para>
        /// </remarks>
        /// <exception cref="System.Data.Entity.Core.EntityException">
        /// Возникает при ошибках доступа к базе данных
        /// </exception>
        /// <exception cref="CryptographicException">
        /// Возникает при ошибках в процессе хеширования пароля
        /// </exception>
        /// <seealso cref="GetHash"/>
        /// <seealso cref="CaptchaSwitch"/>
        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            if (captcha.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(captchaInput.Text) || captchaInput.Text != captcha.Text)
                {
                    MessageBox.Show("Неверная капча");
                    CaptchaChange();
                    return;
                }
            }

            string hashedPassword = GetHash(PasswordBox.Password);
            var db = Entities.GetContext();
            var user = db.User.AsQueryable().AsNoTracking()
                .FirstOrDefault(u => u.Login == TextBoxLogin.Text && u.Password == hashedPassword);

            if (user == null)
            {
                failedAttempts++;
                MessageBox.Show("Пользователь с такими данными не найден!");
                if (failedAttempts >= 3 && captcha.Visibility != Visibility.Visible)
                {
                    CaptchaChange();
                    CaptchaSwitch(true);
                }
                else if (failedAttempts >= 3)
                {
                    CaptchaChange();
                }
                return;
            }

            // Успешная авторизация
            failedAttempts = 0;
            MessageBox.Show("Пользователь успешно найден!");

            switch (user.Role)
            {
                case "User":
                    NavigationService?.Navigate(new Pages.UserPage());
                    break;
                case "Admin":
                case "Administrator":
                    NavigationService?.Navigate(new Pages.AdminPage());
                    break;
                default:
                    NavigationService?.Navigate(new Pages.UserPage());
                    break;
            }
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки регистрации
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonReg</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу регистрации новых пользователей
        /// </remarks>
        /// <seealso cref="RegPage"/>
        private void ButtonReg_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new RegPage());

        /// <summary>
        /// Обрабатывает событие нажатия кнопки смены пароля
        /// </summary>
        /// <param name="sender">Источник события - кнопка ButtonChangePassword</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// Выполняет переход на страницу восстановления пароля
        /// </remarks>
        /// <seealso cref="ChangePassPage"/>
        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ChangePassPage());

        /// <summary>
        /// Включает или отключает отображение CAPTCHA
        /// </summary>
        /// <param name="visible">
        /// <c>true</c> - показать CAPTCHA, <c>false</c> - скрыть CAPTCHA
        /// </param>
        /// <remarks>
        /// <para>При включении CAPTCHA:</para>
        /// <list type="bullet">
        /// <item><description>Показывает элементы CAPTCHA</description></item>
        /// <item><description>Блокирует поля ввода логина и пароля</description></item>
        /// <item><description>Блокирует кнопки навигации</description></item>
        /// </list>
        /// <para>При отключении CAPTCHA восстанавливает нормальный режим работы формы.</para>
        /// </remarks>
        public void CaptchaSwitch(bool visible)
        {
            if (visible)
            {
                captcha.Visibility = Visibility.Visible;
                captchaInput.Visibility = Visibility.Visible;
                submitCaptcha.Visibility = Visibility.Visible;
                labelCaptcha.Visibility = Visibility.Visible;

                TextBoxLogin.IsEnabled = false;
                PasswordBox.IsEnabled = false;
                ButtonEnter.IsEnabled = false;
                ButtonReg.IsEnabled = false;
                ButtonChangePassword.IsEnabled = false;
            }
            else
            {
                captcha.Visibility = Visibility.Collapsed;
                captchaInput.Visibility = Visibility.Collapsed;
                submitCaptcha.Visibility = Visibility.Collapsed;
                labelCaptcha.Visibility = Visibility.Collapsed;

                TextBoxLogin.IsEnabled = true;
                PasswordBox.IsEnabled = true;
                ButtonEnter.IsEnabled = true;
                ButtonReg.IsEnabled = true;
                ButtonChangePassword.IsEnabled = true;
            }
        }

        /// <summary>
        /// Генерирует новую случайную CAPTCHA
        /// </summary>
        /// <remarks>
        /// <para>CAPTCHA состоит из 6 случайных символов, которые могут быть:</para>
        /// <list type="bullet">
        /// <item><description>Прописные латинские буквы (A-Z)</description></item>
        /// <item><description>Строчные латинские буквы (a-z)</description></item>
        /// <item><description>Цифры (0-9)</description></item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Генерация новой CAPTCHA
        /// CaptchaChange();
        /// string currentCaptcha = captcha.Text; // Например: "A1b2C3"
        /// </code>
        /// </example>
        public void CaptchaChange()
        {
            const string allowchar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var r = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < 6; i++)
                sb.Append(allowchar[r.Next(allowchar.Length)]);
            captcha.Text = sb.ToString();
            captchaInput.Text = string.Empty;
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки подтверждения CAPTCHA
        /// </summary>
        /// <param name="sender">Источник события - кнопка submitCaptcha</param>
        /// <param name="e">Данные события <see cref="RoutedEventArgs"/></param>
        /// <remarks>
        /// <para>Метод проверяет корректность введенной CAPTCHA:</para>
        /// <list type="number">
        /// <item><description>При неверной CAPTCHA - показывает ошибку и генерирует новую</description></item>
        /// <item><description>При верной CAPTCHA - отключает проверку и сбрасывает счетчик попыток</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="CaptchaSwitch"/>
        /// <seealso cref="CaptchaChange"/>
        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Капча введена неверно", "Ошибка");
                CaptchaChange();
                return;
            }

            MessageBox.Show("Капча введена успешно");
            CaptchaSwitch(false);
            failedAttempts = 0;
        }
    }
}