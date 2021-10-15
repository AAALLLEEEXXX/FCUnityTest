using FusionCore.Test.Generic;

namespace FusionCore.Test.Models
{
    public class GameModel
    {
        public GameModel()
        {
            CurrentGameState = new SubscriptionProperty<GameState>();
        }
        
        public IReadOnlySubscriptionProperty<GameState> CurrentGameState { get; }
    }
}
