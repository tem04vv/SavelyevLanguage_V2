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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private List _filteredClients;
        private int count_Clients_On_Page = 10;
        private int currentPage;
        public ClientPage()
        {
            InitializeComponent();

            var currentClient = SavelyevLanguageEntities.GetContext().Client.ToList();
            ClientListView.ItemsSource = currentClient;
            ChangePage();
        }

        private void ChangePage()
        {
            PageListBox.Items.Clear();

            int totalPages = (currentClient.Count)
        }

        private void PageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
