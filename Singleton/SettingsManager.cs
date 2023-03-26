using System;
using Godot;
using System.Collections.Generic;

namespace VampireSurvivorsLike {

    public class SettingsManager : Node {

        private static SettingsManager instance;
        private Dictionary<string, int> settings = new Dictionary<string, int>();

        private const string savePath = "user://settings.cfg";


        private SettingsManager() {
        }

        static SettingsManager() {
        }

        public static SettingsManager Instance {
            get => instance;
            private set => instance = value;
        }

        public override void _Ready() {
            Instance = this;
            this.Load();
        }

        private void Load() {
            ConfigFile file = new ConfigFile();
            Error err = file.Load(savePath);
            if (err.Equals(Error.FileNotFound)) {
                this.Reset();
                return;
            }
            if (err != Error.Ok) {
                GD.Print(err);
                return;
            }
            foreach (string key in file.GetSectionKeys("Settings")) {
                if (file.GetValue("Settings", key) is int intValue) {
                    this.settings.Add(key, intValue);
                }
            }
        }

        public void Save() {
            ConfigFile file = new ConfigFile();
            foreach (KeyValuePair<string, int> value in this.settings) {
                file.SetValue("Settings", value.Key, value.Value);
            }
            file.Save(savePath);
        }

        public void Reset() {
            this.settings = this.GetDefaultSettings();
            this.Save();
        }

        // if its negative its disabled
        private Dictionary<string, int> GetDefaultSettings() {
            return new Dictionary<string, int> {
                { SettingsEnum.Sound.ToString(), 100 },
                { SettingsEnum.Music.ToString(), 100 },
            };
        }

        public void SetValue(string setting, int value) {
            this.settings[setting] = value;
            if (setting.Equals(SettingsEnum.Music.ToString())) {
                AudioPlayerSingleton.Instance.SetVolume(value < 0 ? 0 : value);
            }
        }

        public int GetValue(SettingsEnum setting) {
            return this.settings[setting.ToString()];
        }

    }

}