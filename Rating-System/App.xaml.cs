using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Glicko2;
using System.Diagnostics;

namespace Rating_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 


    public partial class App : Application
    {
        // Player Database
        public AppDataContext Database { get; protected set; } = new AppDataContext();

        void ReportMatch(BilliardsMatch match)
        {
            double balls_pocketed_by_winner = match.WinnerBallsPocketed;
            double balls_pocketed_by_loser = match.LoserBallsPocketed;

            if (match.Winner == match.Loser) {
                Debug.WriteLine("Cannot play against yourself!");
                return;
            }

            Debug.WriteLine("Game Score: " + balls_pocketed_by_winner + ":" + balls_pocketed_by_loser);
            if (balls_pocketed_by_winner != 8) { // loser pocketed the 8 ball to lose
                balls_pocketed_by_loser = Math.Max(0, balls_pocketed_by_loser - 2);
            }

            if (balls_pocketed_by_winner == 8) {
                balls_pocketed_by_winner += 2;
            }

            double denominator = Math.Max(2, balls_pocketed_by_loser + balls_pocketed_by_winner);
            double winner_result = (double)balls_pocketed_by_winner / denominator;
            double loser_result = 1 - winner_result;
            winner_result += 0.5;
            Debug.WriteLine(winner_result);
            Debug.WriteLine(loser_result);

            double winner_original = match.Winner.Rating;
            double loser_original = match.Loser.Rating;
            Debug.WriteLine(winner_original);

            match.Winner = GlickoCalculator.CalculateRanking(match.Winner, new List<GlickoOpponent>() { new GlickoOpponent(match.Loser, winner_result) }) as BilliardsPlayer;
            match.Loser = GlickoCalculator.CalculateRanking(match.Loser, new List<GlickoOpponent>() { new GlickoOpponent(match.Winner, loser_result) }) as BilliardsPlayer;

            Debug.WriteLine("Winner Change: " + (match.Winner.Rating - winner_original));
            Debug.WriteLine("Loser Change: " + (match.Loser.Rating - loser_original));
        }
    }
}
