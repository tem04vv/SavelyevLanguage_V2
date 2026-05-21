using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SavelyevLanguage
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentClient;
        private string _selectedPhotoPath;
        public AddEditPage(Client client = null)
        {
            InitializeComponent();
            _currentClient = client;

            if (_currentClient != null)
            {
                ID_TextBox.Visibility = Visibility.Visible;
                ID_TextBox.IsEnabled = false;
                ID_TextBox.Text = _currentClient.ID.ToString();
                FirstName_TextBox.Text = _currentClient.FirstName.ToString();
                LastName_TextBox.Text = _currentClient.LastName.ToString();
                Patronymic_TextBox.Text = _currentClient.Patronymic.ToString();
                Email_TextBox.Text = _currentClient.Email.ToString();
                Phone_TextBox.Text = _currentClient.Phone.ToString();
                Birthday_DatePicker.SelectedDate = _currentClient.Birthday;
                if (_currentClient.GenderCode == "ж")
                {
                    FemaleRB.IsChecked = true;
                }
                else
                {
                    MaleRB.IsChecked = true;
                }

                if (!string.IsNullOrEmpty(_currentClient.PhotoPath))
                {
                    ClientPhoto.Source = new BitmapImage(new Uri(_currentClient.PhotoPath, UriKind.Relative));
                }
            }
            else
            {
                ID_TextBlock.Visibility = Visibility.Collapsed;
                ID_TextBox.Visibility = Visibility.Collapsed;
                ClientPhoto.Source = new BitmapImage(new Uri("res/picture.png", UriKind.Relative));
            }
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(dlg.FileName);
                string clientsFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Клиенты");
                System.IO.Directory.CreateDirectory(clientsFolder);
                string fullPath = System.IO.Path.Combine(clientsFolder, fileName);

                System.IO.File.Copy(dlg.FileName, fullPath, true);

                _selectedPhotoPath = "Клиенты\\" + fileName;
                ClientPhoto.Source = new BitmapImage(new Uri(fullPath));
            }
        }
        private bool IsValidName(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsValidPhone(string phone)
        {
            foreach (char c in phone)
            {
                if (!char.IsDigit(c) && c != '+' && c != '-' && c != '(' && c != ')' && c != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(FirstName_TextBox.Text))
                errors.AppendLine("Заполните фамилию!");
            else
            {
                if (FirstName_TextBox.Text.Length > 50)
                    errors.AppendLine("Фамилия не может быть длиннее 50 символов!");
                if (!IsValidName(FirstName_TextBox.Text))
                    errors.AppendLine("Фамилия может содержать только буквы, пробел и дефис!");
            }

            if (string.IsNullOrWhiteSpace(LastName_TextBox.Text))
                errors.AppendLine("Заполните имя!");
            else
            {
                if (LastName_TextBox.Text.Length > 50)
                    errors.AppendLine("Имя не может быть длиннее 50 символов!");
                if (!IsValidName(LastName_TextBox.Text))
                    errors.AppendLine("Имя может содержать только буквы, пробел и дефис!");
            }

            if (string.IsNullOrWhiteSpace(Patronymic_TextBox.Text))
                errors.AppendLine("Заполните отчество!");
            else
            {
                if (Patronymic_TextBox.Text.Length > 50)
                    errors.AppendLine("Отчество не может быть длиннее 50 символов!");
                if (!IsValidName(Patronymic_TextBox.Text))
                    errors.AppendLine("Отчество может содержать только буквы, пробел и дефис!");
            }

            if (string.IsNullOrWhiteSpace(Email_TextBox.Text))
                errors.AppendLine("Заполните email!");
            else
            {
                if (Email_TextBox.Text.Length > 50)
                    errors.AppendLine("Email не может быть длиннее 50 символов!");
                if (Email_TextBox.Text.Any(c => c >= 'а' && c <= 'я' || c >= 'А' && c <= 'Я'))
                    errors.AppendLine("Email не должен содержать русские буквы!");
                try
                {
                    var addr = new System.Net.Mail.MailAddress(Email_TextBox.Text);
                    if (addr.Address != Email_TextBox.Text)
                        throw new Exception();
                }
                catch
                {
                    errors.AppendLine("Введите корректный email!");
                }
            }

            if (string.IsNullOrWhiteSpace(Phone_TextBox.Text))
                errors.AppendLine("Заполните телефон!");
            else
            {
                int digitCount = Phone_TextBox.Text.Count(char.IsDigit);

                if (digitCount < 9)
                    errors.AppendLine("Телефон должен содержать не менее 9 цифр!");
                if (digitCount > 11)
                    errors.AppendLine("Телефон должен содержать не более 11 цифр!");
                if (!IsValidPhone(Phone_TextBox.Text))
                    errors.AppendLine("Телефон может содержать только цифры и символы: + - ( ) пробел");
            }

            if (Birthday_DatePicker.SelectedDate == null)
                errors.AppendLine("Выберите дату рождения!");

            if (FemaleRB.IsChecked == false && MaleRB.IsChecked == false)
                errors.AppendLine("Выберите пол!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            var context = SavelyevLanguageEntities.GetContext();

            if (_currentClient == null)
            {
                _currentClient = new Client();
                _currentClient.RegistrationDate = DateTime.Now;
                context.Client.Add(_currentClient);
            }
            _currentClient.FirstName = FirstName_TextBox.Text;
            _currentClient.LastName = LastName_TextBox.Text;
            _currentClient.Patronymic = Patronymic_TextBox.Text;
            _currentClient.Email = Email_TextBox.Text;
            _currentClient.Phone = Phone_TextBox.Text;
            _currentClient.Birthday = Birthday_DatePicker.SelectedDate.Value;
            if (FemaleRB.IsChecked == true)
            {
                _currentClient.GenderCode = "ж";
            }
            else
            {
                _currentClient.GenderCode = "м";
            }

            if (string.IsNullOrEmpty(_selectedPhotoPath))
            {
                _currentClient.PhotoPath = "res/picture.png"; 
            }
            else
            {
                _currentClient.PhotoPath = _selectedPhotoPath;
            }

            try
            {
                context.SaveChanges();
                MessageBox.Show("Сохранение прошло успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
