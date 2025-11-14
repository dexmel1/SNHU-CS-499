using System.Collections.Generic;

namespace DungeonExplorer.Models
{
    public class Player
    {
        public string Name { get; set; } = "Hero";

        // Current room name
        public string CurrentRoom { get; set; } = "Great Hall";

        // Items collected
        public List<string> Inventory { get; set; } = new();
    }
}
