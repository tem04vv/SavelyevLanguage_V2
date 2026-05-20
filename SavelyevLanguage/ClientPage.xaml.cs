using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private List<Client> _filteredClients;
        private int pageSize = 10;
        private int currentPage;
        private int TBAllRecords;
        public ClientPage()
        {
            InitializeComponent();

            UpdateClients();
            PageSizeComboBox.SelectedIndex = 0;
            GenderCB.SelectedIndex = 0;
            SortCB.SelectedIndex = 0;
        }

        private void UpdateClients()
        {
            var currentClient = SavelyevLanguageEntities.GetContext().Client.ToList();
            TBAllRecords = currentClient.Count;

            if (GenderCB.SelectedIndex == 1)
            {
                currentClient = currentClient.Where(g => g.GenderCode == "м").ToList();
            }
            if (GenderCB.SelectedIndex == 2)
            {
                currentClient = currentClient.Where(g => g.GenderCode == "ж").ToList();
            }

            if (SortCB.SelectedIndex == 1)
            {
                currentClient = currentClient.OrderBy(s => s.FirstName).ToList();
            }
            if (SortCB.SelectedIndex == 2)
            {
                currentClient = currentClient.OrderByDescending(s => s.LastVisitDate).ToList();
            }
            if (SortCB.SelectedIndex == 3)
            {
                currentClient = currentClient.OrderByDescending(s => s.VisitsCount).ToList();
            }

            string searchDigits = new string(SearchTB.Text.Where(char.IsDigit).ToArray());

            currentClient = currentClient
                .Where(n =>
                    n.FullName.ToLower().Contains(SearchTB.Text.ToLower()) ||
                    (!string.IsNullOrEmpty(searchDigits)) && new string(n.Phone.Where(char.IsDigit).ToArray()).Contains(searchDigits) ||
                    n.Email.ToLower().Contains(SearchTB.Text.ToLower())
                    )
                .ToList();

            //ClientListView.ItemsSource = currentClient;
            _filteredClients = currentClient;
            currentPage = 1;
            ChangePage(pageSize);
        }

        private void ChangePage(int pageSize)
        {
            PageListBox.Items.Clear();

            int totalPages = (_filteredClients.Count + pageSize - 1) / pageSize;

            for (int i = 1; i <= totalPages; i++) 
            {
                PageListBox.Items.Add(i);
            }

            PageListBox.SelectedItem = currentPage;

            var clientsPage = _filteredClients.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            ClientListView.ItemsSource = clientsPage;

            int viewedRecords = currentPage * pageSize;
            if (viewedRecords > _filteredClients.Count)
            {
                viewedRecords = _filteredClients.Count;
            }

            TBNumRecords.Text = $"{viewedRecords} из {TBAllRecords}";
        }

        private void PageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageListBox.SelectedItem is int page && page != currentPage)
            {
                currentPage = page;
                ChangePage(pageSize);
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ChangePage(pageSize);
            }
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (_filteredClients.Count + pageSize - 1) / pageSize;
            if (currentPage < totalPages)
            {
                currentPage++;
                ChangePage(pageSize);
            }
        }

        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock selected = PageSizeComboBox.SelectedItem as TextBlock;
            if (selected != null)
            {
                string selectedText = selected.Text;

                if (selectedText == "Все")
                {
                    if (_filteredClients.Count != 0)
                    {
                        pageSize = TBAllRecords;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка! Нет клиентов");
                        return;
                    }
                }
                else
                {
                    pageSize = int.Parse(selectedText);
                }

                currentPage = 1;
                ChangePage(pageSize);
                
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Client client = btn.DataContext as Client;

            if (client == null)
            {
                MessageBox.Show("Сначала выберите клиента для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // если у клиента есть посещения
            if (client.ClientService != null && client.ClientService.Any())
            {
                MessageBox.Show("Нельзя удалить клиента с посещениями", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить клиента {client.FullName}?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var context = SavelyevLanguageEntities.GetContext();
                    context.Client.Attach(client);
                    context.Client.Remove(client);
                    context.SaveChanges();
                    MessageBox.Show("Клиент успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateClients();
                    currentPage = 1;
                    ChangePage(pageSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }
    }
}
