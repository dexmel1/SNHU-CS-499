using System.Collections.Generic;

namespace DungeonExplorer.Models
{
    public class Room
    {
        public string Name { get; set; } = string.Empty;

        // e.g., "north" -> "Kitchen"
        public Dictionary<string, string> Exits { get; set; } = new();

        // Optional item in this room
        public string? Item { get; set; }
    }
}
