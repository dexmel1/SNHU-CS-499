using System.Windows.Input;
using DungeonExplorer.Models;
using DungeonExplorer.Services;

namespace DungeonExplorer.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly GameService _gameService;
        private Player _player;
        private string _statusMessage = string.Empty;

        private string? _currentRoomItem;

        public string? CurrentRoomItem
        {
            get => _currentRoomItem;
            set => SetProperty(ref _currentRoomItem, value);
        }

        public ICommand PickupItemCommand { get; }

        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Movement commands
        public ICommand MoveNorthCommand { get; }
        public ICommand MoveSouthCommand { get; }
        public ICommand MoveEastCommand { get; }
        public ICommand MoveWestCommand { get; }

        public GameViewModel()
        {
            _gameService = new GameService();

            Player = new Player
            {
                Name = "Dex the Brave",
                CurrentRoom = "Great Hall"
            };

            // Initialize item in the starting room (if any)
            UpdateCurrentRoomItem();

            StatusMessage = "Your quest begins in the Great Hall...";

            MoveNorthCommand = new RelayCommand(_ => Move("north"));
            MoveSouthCommand = new RelayCommand(_ => Move("south"));
            MoveEastCommand = new RelayCommand(_ => Move("east"));
            MoveWestCommand = new RelayCommand(_ => Move("west"));

            PickupItemCommand = new RelayCommand(_ => PickupItem());
        }

        private void Move(string direction)
        {
            if (_gameService.TryMove(Player, direction, out var message))
            {
                // Notify bindings that Player changed (for CurrentRoom)
                OnPropertyChanged(nameof(Player));
                // Update room item after moving
                UpdateCurrentRoomItem();
            }

            StatusMessage = message;
        }

        private void UpdateCurrentRoomItem()
        {
            var room = _gameService.GetCurrentRoom(Player);
            CurrentRoomItem = room?.Item;
        }

        private void PickupItem()
        {
            if (_gameService.TryPickupItem(Player, out var message))
            {
                // Inventory changed -> notify UI
                OnPropertyChanged(nameof(Player));
                // Room no longer has the item
                UpdateCurrentRoomItem();
            }

            StatusMessage = message;
        }
    }
}
