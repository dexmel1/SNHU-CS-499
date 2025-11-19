using System.Collections.Generic;
using DungeonExplorer.Models;

namespace DungeonExplorer.Services
{
    public class GameService
    {
        public Dictionary<string, Room> Rooms { get; } = new();

        private static readonly HashSet<string> RequiredItems = new()
        {
            "Shield",
            "Armor",
            "Sword",
            "Bow",
            "Torch",
            "Arrows"
        };

        public int RequiredItemCount => RequiredItems.Count;

        private readonly Random _random = new();

        public GameService()
        {
            InitializeRooms();
            RandomlyPlaceItems();
        }

        private void InitializeRooms()
        {
            Rooms["Great Hall"] = new Room
            {
                Name = "Great Hall",
                Exits = new Dictionary<string, string>
                {
                    ["west"] = "West Tower",
                    ["east"] = "East Tower",
                    ["south"] = "Dungeon"
                }
            };

            Rooms["East Tower"] = new Room
            {
                Name = "East Tower",
                Exits = new Dictionary<string, string>
                {
                    ["west"] = "Great Hall",
                    ["south"] = "Barracks"
                },
            };

            Rooms["Barracks"] = new Room
            {
                Name = "Barracks",
                Exits = new Dictionary<string, string>
                {
                    ["west"] = "Dungeon",
                    ["north"] = "East Tower",
                    ["south"] = "Library"
                },
            };

            Rooms["Library"] = new Room
            {
                Name = "Library",
                Exits = new Dictionary<string, string>
                {
                    ["west"] = "Bedroom",
                    ["north"] = "Barracks"
                }
            };

            Rooms["Bedroom"] = new Room
            {
                Name = "Bedroom",
                Exits = new Dictionary<string, string>
                {
                    ["west"] = "Stables",
                    ["east"] = "Library",
                    ["north"] = "Dungeon"
                },
            };

            Rooms["Stables"] = new Room
            {
                Name = "Stables",
                Exits = new Dictionary<string, string>
                {
                    ["north"] = "Kitchen",
                    ["east"] = "Bedroom"
                },
            };

            Rooms["Kitchen"] = new Room
            {
                Name = "Kitchen",
                Exits = new Dictionary<string, string>
                {
                    ["north"] = "West Tower",
                    ["south"] = "Stables",
                    ["east"] = "Dungeon"
                },
            };

            Rooms["West Tower"] = new Room
            {
                Name = "West Tower",
                Exits = new Dictionary<string, string>
                {
                    ["south"] = "Kitchen",
                    ["east"] = "Great Hall"
                },
            };

            Rooms["Dungeon"] = new Room
            {
                Name = "Dungeon",
                Exits = new Dictionary<string, string>
                {
                    ["north"] = "Great Hall",
                    ["east"] = "Barracks",
                    ["south"] = "Bedroom",
                    ["west"] = "Kitchen"
                }
            };
        }

        private void RandomlyPlaceItems()
        {
            // Rooms eligible to hold items (exclude Dungeon)
            var candidateRooms = Rooms.Values
                .Where(r => !string.Equals(r.Name, "Dungeon", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Clear any existing items
            foreach (var room in candidateRooms)
            {
                room.Item = null;
            }

            // We have 6 required items
            var items = RequiredItems.ToList();

            // Shuffle rooms
            for (int i = candidateRooms.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (candidateRooms[i], candidateRooms[j]) = (candidateRooms[j], candidateRooms[i]);
            }

            // Assign each item to a different room
            int itemCount = Math.Min(items.Count, candidateRooms.Count);
            for (int i = 0; i < itemCount; i++)
            {
                candidateRooms[i].Item = items[i];
            }
        }

        public bool TryMove(Player player, string direction, out string message)
        {
            if (!Rooms.TryGetValue(player.CurrentRoom, out var currentRoom))
            {
                message = "You are in an unknown area.";
                return false;
            }

            direction = direction.ToLowerInvariant();

            if (currentRoom.Exits.TryGetValue(direction, out var nextRoomName))
            {
                player.CurrentRoom = nextRoomName;
                message = $"You move {direction} to the {nextRoomName}.";
                return true;
            }

            message = "You can't go that way.";
            return false;
        }

        public Room? GetCurrentRoom(Player player)
        {
            Rooms.TryGetValue(player.CurrentRoom, out var room);
            return room;
        }

        public bool TryPickupItem(Player player, out string message)
        {
            var room = GetCurrentRoom(player);
            if (room == null)
            {
                message = "You cannot tell where you are.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(room.Item))
            {
                message = "There is nothing here to pick up.";
                return false;
            }

            var itemName = room.Item;

            // Already in inventory?
            if (player.Inventory.Contains(itemName))
            {
                message = $"You already picked up the {itemName}.";
                return false;
            }

            // Add to inventory and remove from room
            player.Inventory.Add(itemName);
            room.Item = null;

            message = $"You pick up the {itemName} and add it to your inventory.";
            return true;
        }

        public bool ResolveDungeonBattle(Player player, out string message)
        {
            // Check if player has all required items
            var inventorySet = new HashSet<string>(player.Inventory);

            bool hasAllItems = RequiredItems.IsSubsetOf(inventorySet);

            if (!hasAllItems)
            {
                message =
                    "You step into the Dungeon... Azog descends upon you!\n" +
                    "Without all six legendary items, you are overwhelmed and defeated.";
                return false; // lose
            }

            message =
                "You step into the Dungeon... Azog charges!\n" +
                "You blind him with the Torch and block with your Shield.\n" +
                "Your Armor holds as you strike with your Sword.\n" +
                "You slow his retreat with your Bow and finish him with a volley of Arrows.\n" +
                "Azog is defeated. The castle is safe!";
            return true; // win
        }

        private Dictionary<string, int> BfsDistances(string startRoomName)
        {
            var distances = new Dictionary<string, int>();
            var queue = new Queue<string>();

            distances[startRoomName] = 0;
            queue.Enqueue(startRoomName);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentDist = distances[current];

                if (!Rooms.TryGetValue(current, out var room))
                    continue;

                foreach (var nextRoomName in room.Exits.Values)
                {
                    if (!distances.ContainsKey(nextRoomName))
                    {
                        distances[nextRoomName] = currentDist + 1;
                        queue.Enqueue(nextRoomName);
                    }
                }
            }

            return distances;
        }

        private static int GetDistance(Dictionary<string, int> distMap, string roomName, int defaultValue)
        {
            return distMap.TryGetValue(roomName, out var d) ? d : defaultValue;
        }

        public int ComputePar(string startRoomName)
        {
            const int INF = int.MaxValue / 4;
            const string dungeonRoomName = "Dungeon";

            // Identify item rooms from current layout
            var itemRooms = new List<string>();

            foreach (var room in Rooms.Values)
            {
                if (!string.IsNullOrWhiteSpace(room.Item) && RequiredItems.Contains(room.Item!))
                {
                    itemRooms.Add(room.Name);
                }
            }

            int m = itemRooms.Count;
            if (m == 0)
            {
                // No items → trivial par
                return 0;
            }

            // BFS from start
            var distFromStart = BfsDistances(startRoomName);

            // BFS from each item room
            var bfsFromItems = new Dictionary<string, Dictionary<string, int>>();
            foreach (var roomName in itemRooms)
            {
                bfsFromItems[roomName] = BfsDistances(roomName);
            }

            // Build distance tables
            var distStartToItem = new int[m];
            var distItemToDungeon = new int[m];
            var distItems = new int[m, m];

            for (int i = 0; i < m; i++)
            {
                distStartToItem[i] = GetDistance(distFromStart, itemRooms[i], INF);

                var fromItem = bfsFromItems[itemRooms[i]];
                distItemToDungeon[i] = GetDistance(fromItem, dungeonRoomName, INF);

                for (int j = 0; j < m; j++)
                {
                    if (i == j)
                    {
                        distItems[i, j] = 0;
                        continue;
                    }

                    distItems[i, j] = GetDistance(fromItem, itemRooms[j], INF);
                }
            }

            int fullMask = (1 << m) - 1;
            var dp = new int[1 << m, m];

            // Initialize dp with INF
            for (int mask = 0; mask <= fullMask; mask++)
            {
                for (int i = 0; i < m; i++)
                {
                    dp[mask, i] = INF;
                }
            }

            // Base cases: starting from Start → item i
            for (int i = 0; i < m; i++)
            {
                dp[1 << i, i] = distStartToItem[i];
            }

            // Held–Karp DP
            for (int mask = 0; mask <= fullMask; mask++)
            {
                for (int i = 0; i < m; i++)
                {
                    if ((mask & (1 << i)) == 0)
                        continue;

                    int prevMask = mask ^ (1 << i);
                    if (prevMask == 0)
                        continue;

                    for (int j = 0; j < m; j++)
                    {
                        if ((prevMask & (1 << j)) == 0)
                            continue;

                        int prevDist = dp[prevMask, j];
                        int step = distItems[j, i];

                        if (prevDist >= INF || step >= INF)
                            continue;

                        int candidate = prevDist + step;
                        if (candidate < dp[mask, i])
                        {
                            dp[mask, i] = candidate;
                        }
                    }
                }
            }

            // Finish at Dungeon
            int best = INF;
            for (int i = 0; i < m; i++)
            {
                int route = dp[fullMask, i] + distItemToDungeon[i];
                if (route < best)
                {
                    best = route;
                }
            }

            if (best >= INF)
            {
                // Fallback if something is disconnected
                return 0;
            }

            return best;
        }



    }
}
