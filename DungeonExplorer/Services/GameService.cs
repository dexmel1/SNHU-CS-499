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


        public GameService()
        {
            InitializeRooms();
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
                Item = "Shield"
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
                Item = "Armor"
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
                Item = "Sword"
            };

            Rooms["Stables"] = new Room
            {
                Name = "Stables",
                Exits = new Dictionary<string, string>
                {
                    ["north"] = "Kitchen",
                    ["east"] = "Bedroom"
                },
                Item = "Bow"
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
                Item = "Torch"
            };

            Rooms["West Tower"] = new Room
            {
                Name = "West Tower",
                Exits = new Dictionary<string, string>
                {
                    ["south"] = "Kitchen",
                    ["east"] = "Great Hall"
                },
                Item = "Arrows"
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


    }
}
