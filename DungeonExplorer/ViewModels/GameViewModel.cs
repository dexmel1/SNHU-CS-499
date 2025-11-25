using System.Windows.Input;
using DungeonExplorer.Models;
using DungeonExplorer.Services;

namespace DungeonExplorer.ViewModels
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }
    public class GameViewModel : BaseViewModel
    {
        private GameService _gameService;
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
        // Overlay
        public ICommand NewGameCommand { get; }
        public ICommand GoToMenuCommand { get; }

        private GameState _state;

        public GameState State
        {
            get => _state;
            set
            {
                if (SetProperty(ref _state, value))
                {
                    OnPropertyChanged(nameof(IsInMenu));
                    OnPropertyChanged(nameof(IsPlaying));
                    OnPropertyChanged(nameof(IsInGameOver));
                    OnPropertyChanged(nameof(ShowMenuOverlay));
                    OnPropertyChanged(nameof(OverlayTitle));
                    OnPropertyChanged(nameof(OverlaySubtitle));
                }
            }
        }

        public bool IsInMenu => State == GameState.Menu;
        public bool IsPlaying => State == GameState.Playing;
        public bool IsInGameOver => State == GameState.GameOver;

        // For a single overlay that covers Menu + GameOver
        public bool ShowMenuOverlay => State != GameState.Playing;

        public string OverlayTitle => State switch
        {
            GameState.Menu => "Dungeon Explorer",
            GameState.GameOver => "Game Over",
            _ => string.Empty
        };

        public string OverlaySubtitle => State switch
        {
            GameState.Menu =>
                "Welcome to Dungeon Explorer. Click 'New Game' to begin your quest. " +
                "Collect all six legendary items before entering the Dungeon.",

            GameState.GameOver =>
                $"Final Score: {Score}. Click 'New Game' to play again.",

            _ => string.Empty
        };

        public GameViewModel()
        {
            _gameService = new GameService();

            Player = new Player
            {
                Name = "Dex the Brave",
                CurrentRoom = "Great Hall"
            };

            MoveCount = 0;
            Score = 0;
            Par = 0;
            IsGameOver = false;

            // Start in Menu state
            State = GameState.Menu;

            StatusMessage = BuildStatus("Welcome to Dungeon Explorer. Click 'New Game' to begin.");
            // Movement Commands
            MoveNorthCommand = new RelayCommand(_ => Move("north"));
            MoveSouthCommand = new RelayCommand(_ => Move("south"));
            MoveEastCommand = new RelayCommand(_ => Move("east"));
            MoveWestCommand = new RelayCommand(_ => Move("west"));
            PickupItemCommand = new RelayCommand(_ => PickupItem());

            // Overlay Buttons
            NewGameCommand = new RelayCommand(_ => StartNewGame());
            GoToMenuCommand = new RelayCommand(_ => GoToMenu());
        }

        private void StartNewGame()
        {
            // New game → new service (random items, new par)
            _gameService = new GameService();

            Player = new Player
            {
                Name = "Dex the Brave",
                CurrentRoom = "Great Hall"
            };

            MoveCount = 0;
            Score = 0;
            IsGameOver = false;

            Par = _gameService.ComputePar(Player.CurrentRoom);
            UpdateCurrentRoomItem();

            State = GameState.Playing;

            StatusMessage = BuildStatus("Your quest begins in the Great Hall...");
        }

        private void GoToMenu()
        {
            // Reset basic state
            MoveCount = 0;
            Score = 0;
            IsGameOver = false;

            // Leave Player/Rooms alone; they won't be used until NewGame anyway
            State = GameState.Menu;

            StatusMessage = BuildStatus(
                "Welcome back to the main menu. Click 'New Game' to begin a new quest.");
        }

        private void Move(string direction)
        {
            if (State != GameState.Playing)
            {
                StatusMessage = BuildStatus("The game is not currently in progress. Click 'New Game' to begin.");
                return;
            }

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
                    State = GameState.GameOver;
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
            if (State != GameState.Playing)
            {
                StatusMessage = BuildStatus("The game is not currently in progress. Click 'New Game' to begin.");
                return;
            }

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
