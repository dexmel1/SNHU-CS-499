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

        private string BuildStatus(string coreMessage)
        {
            int totalItems = _gameService.RequiredItemCount;
            int collected = Player.Inventory.Count;

            return $"{coreMessage}\nItems: {collected}/{totalItems} | Moves: {MoveCount} | Par: {Par} | Score: {Score}";
        }

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

        private bool _isGameOver;

        public bool IsGameOver
        {
            get => _isGameOver;
            set => SetProperty(ref _isGameOver, value);
        }

        private int _moveCount;
        public int MoveCount
        {
            get => _moveCount;
            set => SetProperty(ref _moveCount, value);
        }

        private int _par;
        public int Par
        {
            get => _par;
            set => SetProperty(ref _par, value);
        }

        private int _score;
        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
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

            // Compute par based on current layout (start, items, dungeon)
            Par = _gameService.ComputePar(Player.CurrentRoom);

            MoveCount = 0;
            Score = 0;

            StatusMessage = BuildStatus("Your quest begins in the Great Hall...");

            MoveNorthCommand = new RelayCommand(_ => Move("north"));
            MoveSouthCommand = new RelayCommand(_ => Move("south"));
            MoveEastCommand = new RelayCommand(_ => Move("east"));
            MoveWestCommand = new RelayCommand(_ => Move("west"));

            PickupItemCommand = new RelayCommand(_ => PickupItem());

            UpdateCurrentRoomItem();
        }


        private void Move(string direction)
        {
            if (IsGameOver)
            {
                StatusMessage = BuildStatus("The game is over. Restart the game to play again.");
                return;
            }

            if (_gameService.TryMove(Player, direction, out var message))
            {
                // Increment move counter
                MoveCount = MoveCount + 1;

                // Notify bindings that Player changed (for CurrentRoom)
                OnPropertyChanged(nameof(Player));
                // Update room item after moving
                UpdateCurrentRoomItem();

                // If player entered the Dungeon, resolve win/lose
                if (Player.CurrentRoom == "Dungeon")
                {
                    bool won = _gameService.ResolveDungeonBattle(Player, out var battleMessage);

                    // Apply move penalty if over par
                    if (Par > 0 && MoveCount > Par)
                    {
                        int over = MoveCount - Par;
                        int penalty = over * 5;
                        Score -= penalty;
                        battleMessage += $"\nYou were {over} moves over par (-{penalty} points).";
                    }

                    battleMessage += $"\nFinal Score: {Score}";
                    StatusMessage = BuildStatus(battleMessage);

                    IsGameOver = true;
                    return;
                }
            }

            // If player didn't early-return due to Dungeon resolution, show normal movement message
            StatusMessage = BuildStatus(message);
        }


        private void UpdateCurrentRoomItem()
        {
            var room = _gameService.GetCurrentRoom(Player);
            CurrentRoomItem = room?.Item;
        }

        private void PickupItem()
        {
            if (IsGameOver)
            {
                StatusMessage = BuildStatus("The game is over. Restart the game to play again.");
                return;
            }

            if (_gameService.TryPickupItem(Player, out var message))
            {
                // Inventory changed -> notify UI
                OnPropertyChanged(nameof(Player));
                // Room no longer has the item
                UpdateCurrentRoomItem();

                // Scoring: +100 per item
                Score = Score + 100;
            }

            StatusMessage = BuildStatus(message);
        }
    }
}
