using Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Retro_Gaming_Konzole.Pages
{
    /// <summary>
    /// Interaction logic for DataTablePage.xaml
    /// </summary>
    public partial class DataTablePage : Page
    {   
        public ObservableCollection<RetroConsole> retroConsoles { get; set; }
        MainWindow mainWindow;

        public DataTablePage()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            retroConsoles = mainWindow.retroConsoles;
            DataContext = this;
        }

        public void selectOrDeselectAll()
        {
            
            int num = ConsolesDataGrid.Items.Count;

            for(int i = 0; i < num; ++i)
            {
                ConsolesDataGrid.SelectedIndex = i;

                this.CheckBox_Checked(this, new RoutedEventArgs());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            retroConsoles[ConsolesDataGrid.SelectedIndex].IsSelected = !retroConsoles[ConsolesDataGrid.SelectedIndex].IsSelected;
        }
    }
}
