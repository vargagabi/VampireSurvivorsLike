using Godot;
using System;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Main : Node2D {

        private const int GameTimeInMinutes = 1;
        private int minutesPassed = 0;
        private Player playerOne;
        private Player playerTwo;
        private Map map;
        private MobSpawner mobSpawner;
        private int levelingCounter = 0;

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
            if (this.levelingCounter++ % 25 != 0) {
                return;
            }
            this.levelingCounter = 0;
            if (ExpToLvl(this.experience) > this.level && !this.playerOne.IsDead) {
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
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.playerOne.IsDead) {
                this.Rpc(nameof(this.LevelingUpFinished));
                this.isLevelingUp = false;
                return;
            }

            await LevelUpManagerSingleton.Instance.OnPlayerLevelUp(levelIncreases);

            this.isLevelingUp = false;
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.LevelingUpFinished));
            } else {
                this.LevelingUpFinished();
            }
            this.playerOne.Gui.SetCurrentLevel(this.level);
            this.playerOne.Gui.SetCurrentExperience((int)(100 * (this.experience - LvlToExp(this.level))
                                                          / (LvlToExp(this.level + 1) - LvlToExp(this.level))));
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
            this.playerOne.Gui.SetCurrentExperience((int)(100 * (this.experience - LvlToExp(this.level))
                                                          / (LvlToExp(this.level + 1) - LvlToExp(this.level))));
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
        public void AddGold(int gold) {
            this.gold += gold;
            this.playerOne.Gui.SetGoldCount(this.gold);
        }

        /*
         * Calculates the current level depending on the experience.
         */
        private static int ExpToLvl(float exp) {
            return (int)(Math.Sqrt(exp + 4) - 2);
        }

        /*
         * Calculates the experience required to reach the level.
         */
        private static float LvlToExp(int lvl) {
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
            if (!this.playerOne.IsDead || (this.playerTwo != null && !this.playerTwo.IsDead)) {
                return;
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.GameEnded), false, 0);
            } else {
                this.GameEnded(false, 0);
            }
        }

        [RemoteSync]
        public void GameEnded(bool isVictory, int goldCount) {
            this.GetTree().Paused = true;
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.GameFinished;
            AudioPlayerSingleton.Instance.SwitchToAmbient(false);
            if (isVictory) {
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Victory);
                AttributeManagerSingleton.Instance.Gold += goldCount;
                AttributeManagerSingleton.Instance.Save();
            }
            this.playerOne.Gui.GameFinished(isVictory, goldCount);
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