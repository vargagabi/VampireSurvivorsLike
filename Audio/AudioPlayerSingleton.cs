using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class AudioPlayerSingleton : Node {

        private static AudioPlayerSingleton instance;

        public enum EffectEnum {

            Complete,
            Death,
            Strange,
            Victory

        }

        private System.Collections.Generic.Dictionary<string, List<string>> fileMap =
            new System.Collections.Generic.Dictionary<string, List<string>>();
        private System.Collections.Generic.Dictionary<string, AudioStreamPlayer> audioPlayers =
            new System.Collections.Generic.Dictionary<string, AudioStreamPlayer>();
        private System.Collections.Generic.Dictionary<string, Tween> tweens =
            new System.Collections.Generic.Dictionary<string, Tween>();
        private readonly string[] directoryPaths = { "Ambient", "Action", "Effect" };
        private string currentlyPlaying = "";

        private int typeInt = 0;

        public static AudioPlayerSingleton Instance {
            get => instance;
            set {
                GD.Print("AudioPlayer ready...");
                instance = value;
            }
        }


        private AudioPlayerSingleton() {
        }

        static AudioPlayerSingleton() {
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("num_1")) {
                this.SwitchToAmbient();
            } else if (@event.IsActionPressed("num_2")) {
                this.SwitchToAction();
            } else if (@event.IsActionPressed("num_3")) {
                this.PlayEffect(EffectEnum.Death);
            } else if (@event.IsActionPressed("ui_right")) {
                // this.ContinueOrPlayRandomAudio(this.currentlyPlaying);
            } else if (@event.IsActionPressed("ui_up")) {
                // this.audioPlayers[this.currentlyPlaying].VolumeDb += 5;
                GD.Print(this.audioPlayers[this.currentlyPlaying].VolumeDb);
            } else if (@event.IsActionPressed("ui_down")) {
                // this.audioPlayers[this.currentlyPlaying].VolumeDb -= 5;
                GD.Print(this.audioPlayers[this.currentlyPlaying].VolumeDb);
            } else if (@event.IsActionPressed("ui_left")) {
                // this.typeInt = (this.typeInt + 1) % this.directoryPaths.Length;
                // this.currentlyPlaying = this.directoryPaths[this.typeInt];
                GD.Print($"Type set to {this.directoryPaths[this.typeInt]}");
            } else if (@event.IsActionPressed("num_4")) {
                // this.getStreamData();
            }
        }


        public override void _Ready() {
            Instance = this;

            foreach (string type in this.directoryPaths) {
                this.audioPlayers.Add(type, GetNode<AudioStreamPlayer>($"{type}AudioPlayer"));
                this.tweens.Add(type, this.audioPlayers[type].GetChild<Tween>(0));
                this.tweens[type].Connect("tween_completed", this, "OnTweenCompleted");
            }

            this.audioPlayers["Ambient"].VolumeDb = -40;
            this.audioPlayers["Action"].VolumeDb = -40;
            this.audioPlayers["Effect"].VolumeDb = 0;
            this.GetAudioFiles();
        }

        private void getStreamData() {
            GD.Print("---");
            foreach (var player in this.audioPlayers) {
                String name = "noname";
                float pos = 0;
                if (player.Value.Stream != null) {
                    name = player.Value.Stream.ResourcePath;
                    pos = player.Value.GetPlaybackPosition() / player.Value.Stream.GetLength();
                }
                GD.Print(
                    $"{player.Key}: {player.Value.Playing} | {!player.Value.StreamPaused} | {player.Value.VolumeDb} | {pos} | {name}");
            }
        }


        private void GetAudioFiles() {
            Directory directory = new Directory();
            foreach (string audioType in this.directoryPaths) {
                directory.Open("res://Audio/music/" + audioType);
                directory.ListDirBegin(true, true);
                this.fileMap.Add(audioType, new List<string>());
                while (true) {
                    string file = directory.GetNext();
                    if (file.Equals("")) {
                        directory.ListDirEnd();
                        break;
                    }
                    if (!file.EndsWith(".import")) {
                        this.fileMap[audioType].Add(file);
                    }
                }
            }
        }


        /**
         * Continue playing track or start a new one from 'type' tracks.
         */
        private void ContinueOrPlayRandomAudio(String type, bool continueLastStream) {
            if (!this.directoryPaths.Contains(type)) {
                throw new ArgumentException($"Audio type {type} does not exist.");
            }
            if (continueLastStream && this.audioPlayers[type].Stream != null &&
                this.audioPlayers[type].GetPlaybackPosition() <
                this.audioPlayers[type].Stream.GetLength()) {
                return;
            }
            GD.Randomize();
            int track = (int)GD.RandRange(0, this.fileMap[type].Count);
            this.audioPlayers[type].Stream =
                ResourceLoader.Load<AudioStream>($"res://Audio/music/{type}/{this.fileMap[type][track]}");
            // GD.Print($"Now Playing {this.fileMap[type][track]}");
            this.audioPlayers[type].Play();
        }

        private void RemoveTweens() {
            foreach (Tween tween in this.tweens.Values) {
                tween.RemoveAll();
            }
        }

        public void InterpolateVolume(string audioType, int finalVolume, float duration,
            Tween.TransitionType transition = Tween.TransitionType.Linear,
            Tween.EaseType easeType = Tween.EaseType.InOut) {
            this.tweens[audioType].InterpolateProperty(this.audioPlayers[audioType], "volume_db",
                this.audioPlayers[audioType].VolumeDb, finalVolume, duration, transition, easeType);
            this.tweens[audioType].Start();
        }

        public void SwitchToAmbient(bool continueLastStream = true) {
            // GD.Print("Play ambient method");
            this.RemoveTweens();
            if (this.currentlyPlaying.Equals("Ambient")) {
                return;
            }

            if (this.audioPlayers["Ambient"].StreamPaused) {
                this.audioPlayers["Ambient"].StreamPaused = false;
            }
            this.InterpolateVolume("Action", -40, 3, Tween.TransitionType.Linear, Tween.EaseType.Out);
            this.InterpolateVolume("Ambient", -15, 3, Tween.TransitionType.Expo, Tween.EaseType.In);

            this.currentlyPlaying = "Ambient";
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, continueLastStream);
        }

        public void SwitchToAction(bool continueLastStream = true) {
            // GD.Print("Play action method");
            this.RemoveTweens();
            if (this.currentlyPlaying.Equals("Action")) {
                return;
            }

            if (this.audioPlayers["Action"].StreamPaused) {
                this.audioPlayers["Action"].StreamPaused = false;
            }
            this.InterpolateVolume("Ambient", -40, 1.5f, Tween.TransitionType.Expo, Tween.EaseType.Out);
            this.InterpolateVolume("Action", -5, 1, Tween.TransitionType.Linear, Tween.EaseType.In);

            this.currentlyPlaying = "Action";
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, continueLastStream);
        }

        public void PlayEffect(EffectEnum effect) {
            // GD.Print("Play effect method");
            this.audioPlayers["Effect"].Stream =
                ResourceLoader.Load<AudioStream>($"res://Audio/music/Effect/{effect.ToString()}.ogg");
            this.audioPlayers["Effect"].Play();
        }

        //Signals:
        public void OnAudioFinished() {
            // GD.Print("On audio finished signal");
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, false);
        }

        public void OnTweenCompleted(Godot.Object obj, NodePath path) {
            // GD.Print("\nTween completed");
            if (((AudioStreamPlayer)obj).VolumeDb <= -40) {
                ((AudioStreamPlayer)obj).StreamPaused = true;
            }
            // this.getStreamData();
        }

    }

}