using UniRx;

namespace FusionCore.Test.Models
{
    public class GameModel
    {
        public GameModel()
        {
            CurrentGameState = new ReactiveProperty<GameState>();
        }
        
        public IReactiveProperty<GameState> CurrentGameState { get; }
    }
}
