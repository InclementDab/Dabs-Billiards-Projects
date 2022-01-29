using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Collections.Specialized;
using Glicko2;
using System.Text.RegularExpressions;

namespace Rating_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public App App = Application.Current as App;

        protected MainWindowViewModel m_MainWindowViewModel => DataContext as MainWindowViewModel;

        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            Debug.WriteLine("Loading Database...");
            foreach (BilliardsPlayer billiards_player in App.Database.Players) {
                m_MainWindowViewModel.BilliardsPlayers.Add(billiards_player);
            }

            Debug.WriteLine("Loaded " + m_MainWindowViewModel.BilliardsPlayers.Count() + " players");
            Debug.WriteLine("Loaded Database");

            // Update DB
            m_MainWindowViewModel.BilliardsPlayers.CollectionChanged += BilliardsPlayers_CollectionChanged;
        }

        private void BilliardsPlayers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (object entity in e.NewItems) {
                        App.Database.Players.Add(entity as BilliardsPlayer);
                    }
                    break;
            }

            Debug.WriteLine("Saving Changed");
            App.Database.SaveChanges();
        }

        private void PlayGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWindowViewModel.StudentIDBox == string.Empty) {
                Debug.WriteLine("Cannot Enter a student with an ID of 0");
                return;
            }

            m_MainWindowViewModel.BilliardsPlayers.Add(new BilliardsPlayer() { FirstName = "Tyler", LastName = "Paul", StudentID = int.Parse(m_MainWindowViewModel.StudentIDBox) });
        }

        private void PreviewStudentIDInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _studentIDBox;
        public string StudentIDBox { get => _studentIDBox;
            set {
                _studentIDBox = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<BilliardsPlayer> BilliardsPlayers { get; set; } = new ObservableCollection<BilliardsPlayer>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property_name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
