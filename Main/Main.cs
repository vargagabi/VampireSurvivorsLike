using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Main : Node2D {

        private const int GameTimeInMinutes = 200;
        private int minutesPassed = 0;
        public Player playerOne;
        public Player playerTwo;
        private Map map;
        private MobSpawner mobSpawner;

        private bool isConfigurationFinished = false;

        [Signal] public delegate void OnGameWin();

        public override void _Ready() {
            GD.Print("Main ready...");
            this.map = GetNode<Map>("Map");
            this.mobSpawner = GetNode<MobSpawner>("MobSpawner");

            //Create the player(s)
            this.playerOne = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn").Instance<Player>();
            this.playerOne.Gui = ResourceLoader.Load<PackedScene>("res://GUI/GUI/GUI.tscn").Instance<GUI>();
            this.playerOne.Name = "Player";
            LevelUpManagerSingleton.Instance.Player = this.playerOne;
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.GetTree().NetworkPeer != null) {
                GD.Print("hello three inside");
                this.ConfigureMultiplayer();
            }
            this.AddChild(this.playerOne);
            this.map.AddPlayer(this.playerOne);
            this.mobSpawner.PlayerOne = this.playerOne;
            this.playerOne.Connect(nameof(Player.OnPlayerDeath), this, nameof(OnPlayerDied));
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
            this.isConfigurationFinished = true;
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                Rpc(nameof(this.ConfigurationFinished));
            }

            // Connect(nameof(OnGameWin), this.GetNode("GUI"), "OnGameWon");
        }

        [Remote]
        public void ConfigurationFinished() {
            if (this.isConfigurationFinished && this.GetTree().IsNetworkServer()) {
                GD.Print("config node, send start signal");
                Rpc(nameof(this.StartGame));
            }
        }

        [RemoteSync]
        public void StartGame() {
            GD.Print("start game");
            this.GetTree().Paused = false;
        }

        private void ConfigureMultiplayer() {
            //Setup player two
            this.playerTwo = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn").Instance<Player>();

            this.SetNetworkMasters();

            this.AddChild(this.playerTwo);
            this.map.AddPlayer(this.playerTwo);
            this.mobSpawner.PlayerTwo = this.playerTwo;
            this.playerTwo.Connect(nameof(Player.OnPlayerDeath), this, nameof(OnPlayerDied));
        }

        private void SetNetworkMasters() {
            this.playerOne.SetNetworkMaster(this.GetTree().GetNetworkUniqueId());
            this.playerOne.Name = this.playerOne.GetNetworkMaster().ToString();
            this.playerTwo.SetNetworkMaster(this.GetTree().GetNetworkConnectedPeers()[0]);
            this.playerTwo.Name = this.playerTwo.GetNetworkMaster().ToString();
        }


        //Check if all players are dead, if true end game
        public void OnPlayerDied() {
            GD.Print("Someone died lol");
            if (this.playerOne.IsDead && (this.playerTwo == null || this.playerTwo.IsDead)) {
                GD.Print("Game ended, sad");
                this.GetTree().Paused = true;
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.GameLost;
                AudioPlayerSingleton.Instance.SwitchToAmbient(false);

                if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                    GD.Print("Send status to server");
                    this.RpcId(1, nameof(this.SendStatus), 100, 100);
                }

                // this.playerOne.Gui.OnGameWon();
            }
        }

        [Remote]
        public void SendStatus(int gold, int enemiesDefeated) {
            GD.Print("Receive sent status");
            if (this.GetTree().IsNetworkServer()) {
                int allGold = gold + this.playerOne.Gold;
                int enemyCount = enemiesDefeated + this.playerOne.EnemiesDefeated;
                Rpc(nameof(this.SetGameEndStatus),
                    GameStateManagerSingleton.Instance.GameState == GameStateEnum.GameWon, allGold, enemiesDefeated);
            }
        }

        [RemoteSync]
        public void SetGameEndStatus(bool victory, int enemiesDefeated, int gold) {
            this.playerOne.Gui.GameEnded(victory, enemiesDefeated, gold);
        }

        public void OnTimerTimeout() {
            this.minutesPassed++;

            if (this.minutesPassed >= GameTimeInMinutes) {
                this.EmitSignal(nameof(OnGameWin));
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.GameWon;
                AudioPlayerSingleton.Instance.SwitchToAmbient(false);
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Victory);
            }
        }

    }

}