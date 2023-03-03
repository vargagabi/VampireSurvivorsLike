using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Main : Node2D {

        private const int GameTimeInMinutes = 2;
        private int minutesPassed = 0;

        [Signal] public delegate void OnGameWin();

        public override void _Ready() {
            GD.Print("Main ready...");
            Connect(nameof(OnGameWin), this.GetNode("GUI"), "OnGameWon");
            this.GetTree().Paused = false;
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Playing;
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

        public void Save() {
        }

        public void Load() {
        }

    }

}