using System.Collections.Generic;
using DungeonExplorer.Models;

namespace DungeonExplorer.Services
{
    public class GameService
    {
        public Dictionary<string, Room> Rooms { get; } = new();

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
    }
}
