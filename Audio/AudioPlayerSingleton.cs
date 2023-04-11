using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VampireSurvivorsLike {

    public class AudioPlayerSingleton : Node {

        private readonly Dictionary<AudioTypeEnum, List<string>> fileMap =
            new Dictionary<AudioTypeEnum, List<string>> {
                {
                    AudioTypeEnum.Ambient, new List<string> {
                        "Ambient_1.ogg",
                        "Ambient_2.ogg",
                        "Ambient_3.ogg",
                        "Ambient_4.ogg",
                        "Ambient_5.ogg",
                        "Ambient_6.ogg",
                        "Ambient_7.ogg",
                        "Ambient_8.ogg",
                        "Ambient_9.ogg",
                        "Ambient_10.ogg",
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
                    AudioTypeEnum.Action, new List<string> {
                        "Action_1.ogg",
                        "Action_2.ogg",
                        "Action_3.ogg",
                        "Action_4.ogg",
                        "Action_5.ogg",
                    }
                }, {
                    AudioTypeEnum.Effect, new List<string> {
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
        private readonly Dictionary<AudioTypeEnum, AudioStreamPlayer> audioPlayers =
            new Dictionary<AudioTypeEnum, AudioStreamPlayer>();
        private readonly Dictionary<AudioTypeEnum, Tween> tweens =
            new Dictionary<AudioTypeEnum, Tween>();
        private readonly AudioTypeEnum[] directoryPaths =
            { AudioTypeEnum.Ambient, AudioTypeEnum.Action, AudioTypeEnum.Effect };
        private AudioTypeEnum currentlyPlaying = AudioTypeEnum.Ambient;
        private const int MinVolumeDb = -40;
        private const int MaxVolumeBd = 0;

        public static AudioPlayerSingleton Instance { get; private set; }

        private AudioPlayerSingleton() {
        }

        public override void _Ready() {
            Instance = this;
            foreach (AudioTypeEnum type in this.directoryPaths) {
                this.audioPlayers.Add(type, this.GetNode<AudioStreamPlayer>($"{type}AudioPlayer"));
                this.tweens.Add(type, this.audioPlayers[type].GetChild<Tween>(0));
                this.tweens[type].Connect("tween_completed", this, "OnTweenCompleted");
            }
            this.audioPlayers[AudioTypeEnum.Action].VolumeDb = this.audioPlayers[AudioTypeEnum.Ambient].VolumeDb =
                GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music));
            this.audioPlayers[AudioTypeEnum.Effect].VolumeDb =
                GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Sound));
            this.SwitchMusicType(AudioTypeEnum.Ambient, false);
        }

        public void SetVolume(int percent, string setting) {
            float volume = GetVolumeFromPercent(percent);
            if (setting.Equals(SettingsEnum.Music.ToString())) {
                this.audioPlayers[AudioTypeEnum.Ambient].VolumeDb = volume;
                this.audioPlayers[AudioTypeEnum.Action].VolumeDb = volume;
                if (volume <= MinVolumeDb) {
                    this.audioPlayers[AudioTypeEnum.Ambient].StreamPaused =
                        this.audioPlayers[AudioTypeEnum.Action].StreamPaused = true;
                } else {
                    this.audioPlayers[this.currentlyPlaying].StreamPaused = false;
                }
            } else if (setting.Equals(SettingsEnum.Sound.ToString())) {
                this.audioPlayers[AudioTypeEnum.Effect].VolumeDb = volume;
            }
        }

        private static int GetVolumeFromPercent(int percent) {
            return (int)(MinVolumeDb + (MaxVolumeBd - MinVolumeDb) * (percent * 0.01f));
        }

        /**
         * Continue playing track or start a new one from 'type' tracks.
         */
        private void ContinueOrPlayRandomAudio(AudioTypeEnum type, bool continueLastStream) {
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
                ResourceLoader.Load<AudioStream>($"res://Audio/{type}/{this.fileMap[type][track]}");
            this.audioPlayers[type].Play();
        }

        private void InterpolateVolume(AudioTypeEnum audioType, int finalVolume, float duration,
            Tween.TransitionType transition = Tween.TransitionType.Linear,
            Tween.EaseType easeType = Tween.EaseType.InOut) {
            this.tweens[audioType].InterpolateProperty(this.audioPlayers[audioType], "volume_db",
                this.audioPlayers[audioType].VolumeDb, finalVolume, duration, transition, easeType);
            this.tweens[audioType].Start();
        }

        public void SwitchMusicType(AudioTypeEnum type, bool continueLastStream = true) {
            foreach (Tween tween in this.tweens.Values) {
                tween.RemoveAll();
            }
            this.currentlyPlaying = type;
            if (this.audioPlayers[type].StreamPaused) {
                return;
            }
            if (type.Equals(AudioTypeEnum.Action)) {
                this.InterpolateVolume(AudioTypeEnum.Ambient, -40, 1.5f, Tween.TransitionType.Expo, Tween.EaseType.Out);
                this.InterpolateVolume(AudioTypeEnum.Action,
                    GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music)), 1,
                    Tween.TransitionType.Linear, Tween.EaseType.In);
            } else if (type.Equals(AudioTypeEnum.Ambient)) {
                this.InterpolateVolume(AudioTypeEnum.Action, -40, 3, Tween.TransitionType.Linear, Tween.EaseType.Out);
                this.InterpolateVolume(AudioTypeEnum.Ambient,
                    GetVolumeFromPercent((int)SettingsManager.Instance.GetValue(SettingsEnum.Music)), 3,
                    Tween.TransitionType.Expo, Tween.EaseType.In);
            }
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, continueLastStream);
        }

        public void PlayEffect(AudioEffectEnum effect) {
            if (this.audioPlayers[AudioTypeEnum.Effect].VolumeDb <= MinVolumeDb) {
                return;
            }
            this.audioPlayers[AudioTypeEnum.Effect].Stream =
                ResourceLoader.Load<AudioStream>($"res://Audio/Effect/{effect.ToString()}.ogg");
            this.audioPlayers[AudioTypeEnum.Effect].Play();
        }

        public void OnAudioFinished() {
            this.ContinueOrPlayRandomAudio(this.currentlyPlaying, false);
        }

        public void OnTweenCompleted(Godot.Object obj, NodePath path) {
            if (((AudioStreamPlayer)obj).VolumeDb <= MinVolumeDb) {
                ((AudioStreamPlayer)obj).StreamPaused = true;
            }
        }

    }

}