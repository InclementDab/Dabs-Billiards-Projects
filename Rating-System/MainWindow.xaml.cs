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

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            CreatePlayerWindow create_player_window = new CreatePlayerWindow(m_MainWindowViewModel.BilliardsPlayers);
            create_player_window.Show();
        }

        private void PreviewStudentIDInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void ReportMatch_Click(object sender, RoutedEventArgs e)
        {
            BilliardsMatch match = new BilliardsMatch() {
                Winner = m_MainWindowViewModel.BilliardsPlayers.Where(p => p.StudentID == int.Parse(m_MainWindowViewModel.WinnerIDBox)).First(),
                Loser = m_MainWindowViewModel.BilliardsPlayers.Where(p => p.StudentID == int.Parse(m_MainWindowViewModel.LoserIDBox)).First(),
                WinnerBallsPocketed = int.Parse(m_MainWindowViewModel.WinnerBallsBox),
                LoserBallsPocketed = int.Parse(m_MainWindowViewModel.LoserBallsBox),
            };

            match.Run();
            m_MainWindowViewModel.BilliardsMatches.Add(match);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _winnerIDBox;
        public string WinnerIDBox { 
            get => _winnerIDBox;
            set { 
                _winnerIDBox = value;
                NotifyPropertyChanged();
            } 
        }        
        
        private string _winnerBallsBox;
        public string WinnerBallsBox { 
            get => _winnerBallsBox;
            set {
                _winnerBallsBox = value;
                NotifyPropertyChanged();
            } 
        }        
        
        private string _loserIDBox;
        public string LoserIDBox { 
            get => _loserIDBox;
            set {
                _loserIDBox = value;
                NotifyPropertyChanged();
            } 
        }        
        
        private string _loserBallsBox;
        public string LoserBallsBox { 
            get => _loserBallsBox;
            set {
                _loserBallsBox = value;
                NotifyPropertyChanged();
            } 
        }

        public ObservableCollection<BilliardsPlayer> BilliardsPlayers { get; set; } = new ObservableCollection<BilliardsPlayer>();
        public ObservableCollection<BilliardsMatch> BilliardsMatches { get; set; } = new ObservableCollection<BilliardsMatch>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property_name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
