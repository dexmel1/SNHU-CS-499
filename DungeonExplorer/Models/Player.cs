using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DungeonExplorer.Models
{
    public class Player
    {
        public string Name { get; set; } = "Hero";

        // Current room name
        public string CurrentRoom { get; set; } = "Great Hall";

        // Items collected - use ObservableCollection so UI updates when items change
        public ObservableCollection<string> Inventory { get; } = new();
    }
}
