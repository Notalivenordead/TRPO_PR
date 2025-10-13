using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _222_Busin.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

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

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService?.Navigate(new ChangePassPage());
        }

        // Капча
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
