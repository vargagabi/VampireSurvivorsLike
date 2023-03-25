using Godot;
using System;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Main : Node2D {

        private const int GameTimeInMinutes = 10;
        private int minutesPassed = 0;
        public Player playerOne;
        public Player playerTwo;
        private Map map;
        private MobSpawner mobSpawner;
        private int levelingCounter = 0;

        //multiplayer variables
        private int level = 0;
        private int experience = 0;
        private int gold = 0;

        private bool isLevelingUp = false;
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
            AttributeManagerSingleton.Instance.SetPickupArea(
                this.playerOne.GetNode<Area2D>("PickupArea").GetChild<CollisionShape2D>(0).Shape as CircleShape2D);
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
            this.isConfigurationFinished = true;
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer()) {
                Rpc(nameof(this.ConfigurationFinished));
            }
        }

        public override void _Input(InputEvent inputEvent) {
            if (inputEvent.IsActionPressed("left_click")) {
                ItemDropManager.Instance.CreateGold(100, this.playerOne.GlobalPosition);
            }
            if (inputEvent.IsActionPressed("right_click")) {
                ItemDropManager.Instance.CreateExperienceOrb(10, this.playerOne.GlobalPosition);
            }
            if (!inputEvent.IsActionPressed("ui_cancel")) {
                return;
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(this.TogglePauseGame), !GameStateManagerSingleton.Instance.IsPaused());
                return;
            }
            this.TogglePauseGame(!GameStateManagerSingleton.Instance.IsPaused());
        }

        public override void _Process(float delta) {
            if (this.levelingCounter++ % 50 == 0 && ExpToLvl(this.experience) > this.level) {
                this.levelingCounter = 0;
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    Rpc(nameof(this.LevelUp));
                } else {
                    this.LevelUp();
                }
            }
        }

        [RemoteSync]
        public async void LevelUp() {
            if (this.isLevelingUp || GameStateManagerSingleton.Instance.GameState.Equals(GameStateEnum.Leveling)) {
                return;
            }
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Leveling;
            int levelIncreases = ExpToLvl(this.experience) - this.level;
            this.isLevelingUp = true;
            this.GetTree().Paused = true;
            this.level += levelIncreases;

            await LevelUpManagerSingleton.Instance.OnPlayerLevelUp(levelIncreases);

            this.isLevelingUp = false;
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.LevelingUpFinished));
            } else {
                this.LevelingUpFinished();
            }
            this.playerOne.Gui.SetCurrentLevel(this.level);
        }

        [RemoteSync]
        public void TogglePauseGame(bool isPaused) {
            if (GameStateManagerSingleton.Instance.GameState.Equals(GameStateEnum.Leveling)) {
                return;
            }
            GameStateManagerSingleton.Instance.GameState = isPaused ? GameStateEnum.Paused : GameStateEnum.Playing;
            this.GetTree().Paused = isPaused;
            this.playerOne.Gui.TogglePauseGame(isPaused);
        }

        [RemoteSync]
        public void IncreaseExperience(int value) {
            this.experience += value;
            this.playerOne.Gui.SetCurrentExperience(this.experience);
        }

        [Remote]
        public void LevelingUpFinished() {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                if (!this.isLevelingUp) {
                    Rpc(nameof(this.UnpauseGame));
                }
            } else {
                this.UnpauseGame();
            }
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
        }

        [RemoteSync]
        public void MultiplayerAddGold(int gold) {
            this.gold += gold;
            this.playerOne.Gui.SetGoldCount(this.gold);
        }

        /*
         * Calculates the current level depending on the experience.
         */
        public static int ExpToLvl(float exp) {
            return (int)(Math.Sqrt(exp + 4) - 2);
        }

        /*
         * Calculates the experience required to reach the level.
         */
        public static float LvlToExp(int lvl) {
            return (float)(4 * lvl + Math.Pow(lvl, 2));
        }

        [Remote]
        public void ConfigurationFinished() {
            if (this.isConfigurationFinished && this.GetTree().IsNetworkServer()) {
                Rpc(nameof(this.UnpauseGame));
            }
        }

        [RemoteSync]
        public void UnpauseGame() {
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
            if (this.playerOne.IsDead && (this.playerTwo == null || this.playerTwo.IsDead)) {
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.GameEnded), false, 0);
                } else {
                    this.GameEnded(false, 0);
                }
            }
        }

        [RemoteSync]
        public void GameEnded(bool isVictory, int gold) {
            AudioPlayerSingleton.Instance.SwitchToAmbient(false);
            this.GetTree().Paused = true;
            if (isVictory) {
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Victory);
            }
            this.playerOne.Gui.GameFinished(isVictory, gold);
        }

        public void OnTimerTimeout() {
            this.minutesPassed++;

            if (this.minutesPassed < GameTimeInMinutes ||
                (GameStateManagerSingleton.Instance.IsMultiplayer && !this.GetTree().IsNetworkServer())) {
                return;
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(this.GameEnded), true, this.gold);
            } else {
                this.GameEnded(true, this.gold);
            }
        }

    }

}