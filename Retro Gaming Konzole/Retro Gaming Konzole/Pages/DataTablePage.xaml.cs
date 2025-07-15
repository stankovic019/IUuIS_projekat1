using Domain.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Retro_Gaming_Konzole.Pages
{
    /// <summary>
    /// Interaction logic for DataTablePage.xaml
    /// </summary>
    public partial class DataTablePage : Page
    {
        public ObservableCollection<RetroConsole> retroConsoles { get; set; }
        public bool IsAdmin { get; set; }
        public bool allSelected { get; set; } = false;
        MainWindow mainWindow;

        public DataTablePage()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            retroConsoles = mainWindow.retroConsoles;
            DataContext = this;
            IsAdmin = mainWindow.user.role == Domain.Enums.UserRole.Admin ? true : false;
            ResetSelection();
        }

        public void selectAll()
        {
            if (!IsAdmin) return;

            foreach (var console in retroConsoles)
            {
                console.IsSelected = true;
            }

            allSelected = true;

        }

        public void deleteSelection()
        {
            if (!IsAdmin) return;

            var selected = retroConsoles.Where(c => c.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("No rows selected.");
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete {selected.Count} item(s)?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var item in selected)
                {
                    retroConsoles.Remove(item);
                }
            }
        }

        public void deselectAll()
        {
            if (!IsAdmin) return;

            foreach (var console in retroConsoles)
            {
                console.IsSelected = false;
            }

            allSelected = false;

        }

        public void ResetSelection()
        {
            foreach (var console in retroConsoles)
            {
                console.IsSelected = false;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.Tag is RetroConsole selectedConsole)
            {
                mainWindow.hyperLink(selectedConsole);
            }

        }
    }
}
