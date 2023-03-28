using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Settings : Control {

        private string path = "CenterContainer/VBoxContainer/SettingsContainer/";

        public override void _Ready() {
            this.SetValues();
        }

        private void SetValues() {
            int soundValue = SettingsManager.Instance.GetValue(SettingsEnum.Sound);
            int musicValue = SettingsManager.Instance.GetValue(SettingsEnum.Music);
            this.SetSettingValues(soundValue > 0, Math.Abs(soundValue), SettingsEnum.Sound.ToString());
            this.SetSettingValues(musicValue > 0, Math.Abs(musicValue), SettingsEnum.Music.ToString());
        }

        private void SetSettingValues(bool enabled, int value, string setting) {
            this.GetNode<CheckButton>($"{this.path}/{setting}CheckButton").Pressed = enabled;
            this.GetNode<HSlider>($"{this.path}/{setting}Slider").Value = value;
            this.GetNode<Label>($"{this.path}/{setting}ValueLabel").Text = value.ToString();
        }

        private int GetSettingValues(string setting) {
            int value = (int)this.GetNode<HSlider>($"{this.path}/{setting}Slider").Value;
            if (!this.GetNode<CheckButton>($"{this.path}/{setting}CheckButton").Pressed) {
                value *= -1;
            }
            return value;
        }

        private void ButtonToggled(bool value, string button) {
            if (!button.Equals("Music") && !button.Equals("Sound")) {
                return;
            }
            this.GetNode<HSlider>($"{this.path}/{button}Slider").Editable = value;
            this.GetNode<Label>($"{this.path}/{button}ValueLabel").SelfModulate =
                !value ? new Color(0.6f, 0.6f, 0.6f) : new Color(1, 1, 1);
            SettingsManager.Instance.SetValue(this.GetSettingValues(button), button);
        }

        public void OnSoundValueChanged(float value) {
            this.GetNode<Label>($"{this.path}/SoundValueLabel").Text =
                ((int)(value)).ToString();
            SettingsManager.Instance.SetValue(this.GetSettingValues(SettingsEnum.Sound.ToString()), SettingsEnum.Sound.ToString());
        }

        public void OnMusicValueChanged(float value) {
            this.GetNode<Label>($"{this.path}/MusicValueLabel").Text =
                ((int)(value)).ToString();
            SettingsManager.Instance.SetValue(this.GetSettingValues(SettingsEnum.Music.ToString()), SettingsEnum.Music.ToString());
        }

        public void OnSoundButtonToggled(bool value) {
            this.ButtonToggled(value, "Sound");
        }

        public void OnMusicButtonToggled(bool value) {
            this.ButtonToggled(value, "Music");
        }

        public void OnBackButtonPressed() {
            foreach (string setting in Enum.GetNames(typeof(SettingsEnum))) {
                GD.Print(setting);
                SettingsManager.Instance.SetValue(this.GetSettingValues(setting), setting);
            }
            SettingsManager.Instance.Save();
        }

        public void OnResetButtonPressed() {
            SettingsManager.Instance.Reset();
            this.SetValues();
        }

    }

}