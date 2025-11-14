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
            Player = new Player
            {
                Name = "Dex the Brave",
                CurrentRoom = "Great Hall"
            };

            Player.Inventory.Add("Torch");
            Player.Inventory.Add("Shield");
        }

    }
}
