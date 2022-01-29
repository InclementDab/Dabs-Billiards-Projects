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

    public class BilliardsMatch
    {
        [Key]
        public int ID { get; set; }

        public BilliardsPlayer Winner { get; set; }
        public BilliardsPlayer Loser { get; set; }

        public int WinnerBallsPocketed { get; set; }
        public int LoserBallsPocketed { get; set; }
    }
}
