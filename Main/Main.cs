using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Main : Node2D {

        private const int GameTimeInMinutes = 20;
        private int minutesPassed = 0;
        private Player playerOne;
        private Player playerTwo;
        private Map map;

        [Signal] public delegate void OnGameWin();

        public override void _Ready() {
            GD.Print("Main ready...");
            this.map = GetNode<Map>("Map");
            
            //Create the player(s)
            this.playerOne = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn").Instance<Player>();
            this.playerOne.Name = "Player";
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.GetTree().NetworkPeer != null) {
                GD.Print("hello three inside");
                this.ConfigureMultiplayer();
            }
            this.AddChild(this.playerOne);
            this.map.AddPlayer(this.playerOne);


            // Connect(nameof(OnGameWin), this.GetNode("GUI"), "OnGameWon");
            this.GetTree().Paused = false;
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
            GD.Print(string.Join(", ", this.GetChildren()));
        }


        private void ConfigureMultiplayer() {
            this.GetTree().Paused = true;

            //Setup player two
            this.playerTwo = ResourceLoader.Load<PackedScene>("res://Player/Player.tscn").Instance<Player>();
            
            this.SetNetworkMasters();
            
            this.AddChild(this.playerTwo);
            this.map.AddPlayer(this.playerTwo);
        }

        private void SetNetworkMasters() {
            this.playerOne.SetNetworkMaster(this.GetTree().GetNetworkUniqueId());
            this.playerOne.Name = this.playerOne.GetNetworkMaster().ToString();
            this.playerTwo.SetNetworkMaster(this.GetTree().GetNetworkConnectedPeers()[0]);
            this.playerTwo.Name = this.playerTwo.GetNetworkMaster().ToString();
        }


        public void OnTimerTimeout() {
            this.minutesPassed++;

            if (this.minutesPassed >= GameTimeInMinutes) {
                this.EmitSignal(nameof(OnGameWin));
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.GameEnd;
                AudioPlayerSingleton.Instance.SwitchToAmbient(false);
                AudioPlayerSingleton.Instance.PlayEffect(AudioPlayerSingleton.EffectEnum.Victory);
            }
        }

    }

}