using Godot;

namespace VampireSurvivorsLike {

    public class GameStateManagerSingleton {

        private static GameStateManagerSingleton instance;
        private GameStateEnum gameState;
        public bool IsMultiplayer { get; set; } = false;

        private int Gold { get; set; } = 0;
        private int DefeatedEnemies { get; set; } = 0;

        private GameStateManagerSingleton() {
        }

        static GameStateManagerSingleton() {
        }

        public static GameStateManagerSingleton Instance {
            get => instance ?? (instance = new GameStateManagerSingleton());
        }

        public GameStateEnum GameState {
            get => this.gameState;
            set {
                this.gameState = value;
                // GD.Print($"Game state: {this.gameState}");
            }
        }

    }

}