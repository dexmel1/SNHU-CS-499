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

            StatusMessage = "Your quest begins in the Great Hall...";

            MoveNorthCommand = new RelayCommand(_ => Move("north"));
            MoveSouthCommand = new RelayCommand(_ => Move("south"));
            MoveEastCommand = new RelayCommand(_ => Move("east"));
            MoveWestCommand = new RelayCommand(_ => Move("west"));
        }

        private void Move(string direction)
        {
            if (_gameService.TryMove(Player, direction, out var message))
            {
                // Notify bindings that Player (and its properties) changed
                OnPropertyChanged(nameof(Player));
            }

            StatusMessage = message;
        }
    }
}
