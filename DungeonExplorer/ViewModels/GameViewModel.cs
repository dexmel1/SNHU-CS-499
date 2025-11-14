using DungeonExplorer.Models;

namespace DungeonExplorer.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private Player _player;

        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        public GameViewModel()
        {
            // Temporary initial state
            Player = new Player();
        }
    }
}
