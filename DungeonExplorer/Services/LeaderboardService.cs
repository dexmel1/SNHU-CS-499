using System;
using System.Linq;
using System.Collections.Generic;
using DungeonExplorer.Data;

namespace DungeonExplorer.Services
{
    public class LeaderboardEntry
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Moves { get; set; }
        public int Par { get; set; }
        public bool Won { get; set; }
        public int ItemsCollected { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LeaderboardService
    {
        public LeaderboardService()
        {
            using var db = new GameDbContext();
            db.Database.EnsureCreated();
        }

        public void SaveScore(string playerName, int points, int moves, int par, bool won, int itemsCollected)
        {
            using var db = new GameDbContext();

            // Find existing player or create new
            var player = db.Players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                player = new PlayerEntity { Name = playerName };
                db.Players.Add(player);
                db.SaveChanges();
            }

            var score = new ScoreEntity
            {
                PlayerId = player.Id,
                Points = points,
                Moves = moves,
                Par = par,
                Won = won,
                ItemsCollected = itemsCollected,
                CreatedAt = DateTime.UtcNow
            };

            db.Scores.Add(score);
            db.SaveChanges();
        }

        public List<LeaderboardEntry> GetTopScores(int count = 10)
        {
            using var db = new GameDbContext();

            return db.Scores
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.Moves)
                .Take(count)
                .Select(s => new LeaderboardEntry
                {
                    PlayerName = s.Player != null ? s.Player.Name : string.Empty,
                    Points = s.Points,
                    Moves = s.Moves,
                    Par = s.Par,
                    Won = s.Won,
                    ItemsCollected = s.ItemsCollected,
                    CreatedAt = s.CreatedAt
                })
                .ToList();
        }

        public List<LeaderboardEntry> GetScoresForPlayer(string playerName, int count = 10)
        {
            using var db = new GameDbContext();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                // no specific player – just fall back to global top scores
                return GetTopScores(count);
            }

            return db.Scores
                .Where(s => s.Player != null && s.Player.Name == playerName)
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.Moves)
                .Take(count)
                .Select(s => new LeaderboardEntry
                {
                    PlayerName = s.Player != null ? s.Player.Name : string.Empty,
                    Points = s.Points,
                    Moves = s.Moves,
                    Par = s.Par,
                    Won = s.Won,
                    ItemsCollected = s.ItemsCollected,
                    CreatedAt = s.CreatedAt
                })
                .ToList();
        }

    }
}
