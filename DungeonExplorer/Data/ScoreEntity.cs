using System;

namespace DungeonExplorer.Data
{
    public class ScoreEntity
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public PlayerEntity? Player { get; set; }

        public int Points { get; set; }
        public int Moves { get; set; }
        public int Par { get; set; }
        public bool Won { get; set; }
        public int ItemsCollected { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
