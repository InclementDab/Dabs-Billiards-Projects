using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rating_System
{
    /// <summary>
    /// Interaction logic for CreatePlayerWindow.xaml
    /// </summary>
    public partial class CreatePlayerWindow : Window
    {
        protected ObservableCollection<BilliardsPlayer> m_BilliardsPlayers;

        public CreatePlayerWindow(ObservableCollection<BilliardsPlayer> player_list)
        {
            InitializeComponent();
            m_BilliardsPlayers = player_list;
        }

        private void PreviewStudentIDInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            m_BilliardsPlayers.Add(new BilliardsPlayer() {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                StudentID = int.Parse(StudentIDTextBox.Text)
            });

            Close();
        }
    }
}
