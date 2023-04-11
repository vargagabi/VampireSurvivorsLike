
namespace VampireSurvivorsLike {

    public class GameStateManagerSingleton {

        private static GameStateManagerSingleton instance;
        public bool IsMultiplayer = false;

        private GameStateManagerSingleton() {
        }


        public static GameStateManagerSingleton Instance => instance ?? (instance = new GameStateManagerSingleton());

        public GameStateEnum GameState { get; set; }

        public bool IsPaused() {
            return this.GameState.Equals(GameStateEnum.Paused);
        }

    }

}