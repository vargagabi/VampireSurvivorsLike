using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

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
            foreach (string key in file.GetSectionKeys("Controls")) {
                if (file.GetValue("Controls", key) is int intValue) {
                    this.settings.Add(key, intValue);
                    if (intValue < 0) {
                        InputEventMouseButton mouse = new InputEventMouseButton();
                        mouse.ButtonIndex = Math.Abs(intValue);
                        this.SetActionKey(key, mouse);
                    } else {
                        InputEventKey eventKey = new InputEventKey();
                        eventKey.Scancode = (uint)intValue;
                        this.SetActionKey(key, eventKey);
                    }
                }
            }
            foreach (string key in file.GetSectionKeys("Audio")) {
                if (file.GetValue("Audio", key) is int intValue) {
                    this.settings.Add(key, intValue);
                }
            }
        }

        public void Save() {
            ConfigFile file = new ConfigFile();
            foreach (KeyValuePair<string, int> value in this.settings) {
                if (value.Key.Equals(SettingsEnum.Sound.ToString()) ||
                    value.Key.Equals(SettingsEnum.Music.ToString())) {
                    file.SetValue("Audio", value.Key, value.Value);
                } else {
                    file.SetValue("Controls", value.Key, value.Value);
                }
            }
            file.Save(savePath);
        }

        public void Reset() {
            this.settings = this.GetDefaultSettings();
            this.Save();
            foreach (var setting in this.settings.Where(setting => setting.Key.Contains('_'))) {
                if (setting.Value < 0) {
                    InputEventMouseButton mouse = new InputEventMouseButton();
                    mouse.ButtonIndex = Math.Abs(setting.Value);
                    mouse.Pressed = true;
                    this.SetActionKey(setting.Key, mouse);
                } else {
                    InputEventKey eventKey = new InputEventKey();
                    eventKey.PhysicalScancode = (uint)setting.Value;
                    eventKey.Pressed = true;
                    this.SetActionKey(setting.Key, eventKey);
                }
            }
        }

        // if its negative its disabled
        private Dictionary<string, int> GetDefaultSettings() {
            return new Dictionary<string, int> {
                { SettingsEnum.Sound.ToString(), 100 },
                { SettingsEnum.Music.ToString(), 100 },
                { SettingsEnum.ui_up.ToString(), 119 },
                { SettingsEnum.ui_down.ToString(), 115 },
                { SettingsEnum.ui_left.ToString(), 97 },
                { SettingsEnum.ui_right.ToString(), 100 },
                { SettingsEnum.ui_hold.ToString(), 32 },
                { SettingsEnum.ui_select.ToString(), 32 },
            };
        }

        public void SetActionKey(string action, InputEvent @event) {
            InputMap.ActionEraseEvents(action);
            InputMap.ActionAddEvent(action, @event);
        }

        public void SetValue(int value, string setting) {
            this.settings[setting] = value;
            if (setting.Equals(SettingsEnum.Music.ToString()) || setting.Equals(SettingsEnum.Sound.ToString())) {
                AudioPlayerSingleton.Instance.SetVolume(value < 0 ? 0 : value, setting);
            }
        }

        public int GetValue(SettingsEnum setting) {
            return this.settings[setting.ToString()];
        }

    }

}