using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VampireSurvivorsLike {

    public class AudioPlayerSingleton : Node {

        private static AudioPlayerSingleton instance;


        private readonly Dictionary<string, List<string>> fileMap =
            new Dictionary<string, List<string>> {
                {
                    "Ambient", new List<string> {
                        "Ambient_1.ogg",
                        "Ambient_10.ogg",
                        "Ambient_2.ogg",
                        "Ambient_3.ogg",
                        "Ambient_4.ogg",
                        "Ambient_5.ogg",
                        "Ambient_6.ogg",
                        "Ambient_7.ogg",
                        "Ambient_8.ogg",
                        "Ambient_9.ogg",
                        "Light_Ambient_1.ogg",
                        "Light_Ambient_2.ogg",
                        "Light_Ambient_3.ogg",
                        "Light_Ambient_4.ogg",
                        "Light_Ambient_5.ogg",
                        "Night_Ambient_1.ogg",
                        "Night_Ambient_2.ogg",
                        "Night_Ambient_3.ogg",
                        "Night_Ambient_4.ogg",
                        "Night_Ambient_5.ogg",
                    }
                }, {
                    "Action", new List<string> {
                        "Action_1.ogg",
                        "Action_2.ogg",
                        "Action_3.ogg",
                        "Action_4.ogg",
                        "Action_5.ogg",
                    }
                }, {
                    "Effect", new List<string> {
                        "Complete.ogg",
                        "Death.ogg",
                        "EnemyDeath.ogg",
                        "EnemyHit.ogg",
                        "PlayerHit.ogg",
                        "Strange.ogg",
                        "Victory.ogg",
                    }
                }
            };
        private readonly Dictionary<string, AudioStreamPlayer> audioPlayers =
            new Dictionary<string, AudioStreamPlayer>();
        private readonly Dictionary<string, Tween> tweens =
            new Dictionary<string, Tween>();
        private readonly string[] directoryPaths = { "Ambient", "Action", "Effect" };
        private string currentlyPlaying = "";
        private int minVolumeDb = -40;
        private int maxVolumeBd = 0;

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

        public override void _Ready() {
            Instance = this;

            foreach (string type in this.directoryPaths) {
                this.audioPlayers.Add(type, GetNode<AudioStreamPlayer>($"{type}AudioPlayer"));
                this.tweens.Add(type, this.audioPlayers[type].GetChild<Tween>(0));
                this.tweens[type].Connect("tween_completed", this, "OnTweenCompleted");
            }

            this.audioPlayers["Ambient"].VolumeDb =
                this.GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music));
            this.audioPlayers["Action"].VolumeDb =
                this.GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music));
            this.audioPlayers["Effect"].VolumeDb =
                this.GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Sound));
        }

        public void SetVolume(int percent, string setting) {
            float volume = this.minVolumeDb + (this.maxVolumeBd - this.minVolumeDb) * (percent * 0.01f);
            if (volume <= this.minVolumeDb) {
                volume = -80;
            }
            if (setting.Equals(SettingsEnum.Music.ToString())) {
                this.audioPlayers["Ambient"].VolumeDb = volume;
                this.audioPlayers["Action"].VolumeDb = volume;
                if (!this.currentlyPlaying.Empty()) {
                    this.ContinueOrPlayRandomAudio(this.currentlyPlaying, true);
                }
            } else if (setting.Equals(SettingsEnum.Sound.ToString())) {
                this.audioPlayers["Effect"].VolumeDb = volume;
            }
        }

        private int GetVolumeFromPercent(int percent) {
            return (int)(this.minVolumeDb + (this.maxVolumeBd - this.minVolumeDb) * (percent * 0.01f));
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
                this.audioPlayers[type].StreamPaused = false;
                return;
            }
            GD.Randomize();
            int track = (int)GD.RandRange(0, this.fileMap[type].Count);
            this.audioPlayers[type].Stream =
                ResourceLoader.Load<AudioStream>($"res://Audio/{type}/{this.fileMap[type][track]}");
            this.audioPlayers[type].Play();
        }

        private void RemoveTweens() {
            foreach (Tween tween in this.tweens.Values) {
                tween.RemoveAll();
            }
        }

        private void InterpolateVolume(string audioType, int finalVolume, float duration,
            Tween.TransitionType transition = Tween.TransitionType.Linear,
            Tween.EaseType easeType = Tween.EaseType.InOut) {
            this.tweens[audioType].InterpolateProperty(this.audioPlayers[audioType], "volume_db",
                this.audioPlayers[audioType].VolumeDb, finalVolume, duration, transition, easeType);
            this.tweens[audioType].Start();
        }

        public void SwitchToAmbient(bool continueLastStream = true) {
            GD.Print("Play ambient music");
            this.RemoveTweens();
            this.currentlyPlaying = "Ambient";

            if (this.audioPlayers["Ambient"].StreamPaused) {
                this.audioPlayers["Ambient"].StreamPaused = false;
            }
            this.InterpolateVolume("Action", -40, 3, Tween.TransitionType.Linear, Tween.EaseType.Out);
            this.InterpolateVolume("Ambient",
                this.GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music)), 3,
                Tween.TransitionType.Expo, Tween.EaseType.In);

            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, continueLastStream);
        }

        public void SwitchToAction(bool continueLastStream = true) {
            GD.Print("Play action music");
            this.RemoveTweens();
            this.currentlyPlaying = "Action";

            if (this.audioPlayers["Action"].StreamPaused) {
                this.audioPlayers["Action"].StreamPaused = false;
            }
            this.InterpolateVolume("Ambient", -40, 1.5f, Tween.TransitionType.Expo, Tween.EaseType.Out);
            this.InterpolateVolume("Action",
                this.GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music)), 1,
                Tween.TransitionType.Linear, Tween.EaseType.In);

            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, continueLastStream);
        }

        public void PlayEffect(AudioEffectEnum effect) {
            GD.Print($"Play {effect} effect");
            this.audioPlayers["Effect"].Stream =
                ResourceLoader.Load<AudioStream>($"res://Audio/Effect/{effect.ToString()}.ogg");
            this.audioPlayers["Effect"].Play();
        }

        public void OnAudioFinished() {
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, false);
        }

        public void OnTweenCompleted(Godot.Object obj, NodePath path) {
            if (((AudioStreamPlayer)obj).VolumeDb <= this.minVolumeDb) {
                ((AudioStreamPlayer)obj).StreamPaused = true;
            }
        }

    }

}