using System;
using System.Collections.Generic;

namespace DungeonExplorer.Data
{
    public class PlayerEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ScoreEntity> Scores { get; set; } = new List<ScoreEntity>();
    }
}
