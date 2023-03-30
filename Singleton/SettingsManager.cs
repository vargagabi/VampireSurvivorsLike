using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace VampireSurvivorsLike {

    public class SettingsManager : Node {

        private static SettingsManager instance;
        private Dictionary<string, object> settings = new Dictionary<string, object>();

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
            foreach (KeyValuePair<string, object> value in this.settings) {
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
            foreach (KeyValuePair<string, object> setting in
                     this.settings.Where(setting => setting.Key.Contains('_'))) {
                if (!(setting.Value is KeyList)) {
                    continue;
                }
                InputEventKey keyPress = new InputEventKey();
                keyPress.Scancode = (uint)OS.FindScancodeFromString(setting.Value.ToString());
                this.SetActionKey(setting.Key, keyPress);
            }
        }

        // negative audio value means disabled
        private Dictionary<string, object> GetDefaultSettings() {
            return new Dictionary<string, object> {
                { SettingsEnum.Sound.ToString(), 100 },
                { SettingsEnum.Music.ToString(), 100 },
                { SettingsEnum.ui_up.ToString(), KeyList.W },
                { SettingsEnum.ui_down.ToString(), KeyList.S },
                { SettingsEnum.ui_left.ToString(), KeyList.A },
                { SettingsEnum.ui_right.ToString(), KeyList.D },
                { SettingsEnum.ui_hold.ToString(), KeyList.Shift },
                { SettingsEnum.ui_accept.ToString(), KeyList.Space }
            };
        }

        public void SetActionKey(string action, InputEvent @event) {
            InputMap.ActionEraseEvents(action);
            InputMap.ActionAddEvent(action, @event);
        }

        public void SetValue(string setting, object value) {
            this.settings[setting] = value;
            if (value is int intValue && (setting.Equals(SettingsEnum.Music.ToString()) ||
                                          setting.Equals(SettingsEnum.Sound.ToString()))) {
                AudioPlayerSingleton.Instance.SetVolume(Math.Max(0, intValue), setting);
            }
        }

        public object GetValue(SettingsEnum setting) {
            return this.settings[setting.ToString()];
        }

    }

}