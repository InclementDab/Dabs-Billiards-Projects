using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Glicko2;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Rating_System
{
    public class AppDataContext : DbContext
    {
        public virtual DbSet<BilliardsPlayer> Players { get; set; }
        public virtual DbSet<BilliardsMatch> Matches { get; set; }

        public AppDataContext() : base("name=PSUPlayerData")
        {
        }
    }

    public class BilliardsPlayer : GlickoPlayer, INotifyPropertyChanged
    {
        [Key]
        public int ID { get; set; }

        private int _studentID;

        [Description("PSU Student ID")]
        public int StudentID {
            get => _studentID;
            set {
                _studentID = value;
                NotifyPropertyChanged();
            }
        }

        private string _firstName;

        [Description("First Name")]
        public string FirstName { get => _firstName;
            set {
                _firstName = value;
                NotifyPropertyChanged();
            }
        }

        private string _lastName;

        [Description("Last Name")]
        public string LastName { get => _lastName;
            set {
                _lastName = value;
                NotifyPropertyChanged();
            }
        }

        public new string Name => string.Format("%1 %2", FirstName, LastName);

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property_name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }

    public class BilliardsMatch : INotifyPropertyChanged
    {
        [Key]
        public int ID { get; set; }

        public BilliardsPlayer Winner { get; set; }
        public BilliardsPlayer Loser { get; set; }

        public string Player1Data => string.Format("%1 (%2)", Winner.FirstName, WinnerEloChange);
        public string Player2Data => string.Format("%1 (%2)", Loser.FirstName, LoserEloChange);

        public int WinnerBallsPocketed { get; set; }
        public int LoserBallsPocketed { get; set; }

        public double WinnerEloChange { get; set; }
        public double LoserEloChange { get; set; }

        public void Run()
        {
            double balls_pocketed_by_winner = WinnerBallsPocketed;
            double balls_pocketed_by_loser = LoserBallsPocketed;

            if (Winner == Loser) {
                Debug.WriteLine("Cannot play against yourself!");
                return;
            }

            Debug.WriteLine("Game Score: " + balls_pocketed_by_winner + ":" + balls_pocketed_by_loser);
            if (balls_pocketed_by_winner != 8) { // loser pocketed the 8 ball to lose
                balls_pocketed_by_loser = Math.Max(0, balls_pocketed_by_loser - 2);
            }

            if (balls_pocketed_by_winner == 8) { // winner pocketed all 8 balls, they get a small bonus to their score calc
                balls_pocketed_by_winner += 2;
            }

            double denominator = Math.Max(2, balls_pocketed_by_loser + balls_pocketed_by_winner);
            double winner_result = (double)balls_pocketed_by_winner / denominator;
            double loser_result = 1 - winner_result;
            winner_result += 0.5;
            Debug.WriteLine(winner_result);
            Debug.WriteLine(loser_result);

            double winner_original = Winner.Rating;
            double loser_original = Loser.Rating;
            Debug.WriteLine(winner_original);

            Winner = GlickoCalculator.CalculateRanking(Winner, new List<GlickoOpponent>() { new GlickoOpponent(Loser, winner_result) }) as BilliardsPlayer;
            Loser = GlickoCalculator.CalculateRanking(Loser, new List<GlickoOpponent>() { new GlickoOpponent(Winner, loser_result) }) as BilliardsPlayer;

            WinnerEloChange = Winner.Rating - winner_original;
            LoserEloChange = Loser.Rating - loser_original;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string property_name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
        }
    }
}
