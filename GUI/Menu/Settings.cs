using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Settings : Control {

        private string path = "CenterContainer/VBoxContainer/ScrollContainer/SettingsContainer/";
        private ScrollContainer scrollContainer;
        private PopupPanel keySetPopupPanel;
        private string actionToSet = null;

        [Signal] public delegate void BackButtonPress();

        public override void _Ready() {
            this.scrollContainer = this.GetNode<ScrollContainer>($"{this.path}../");
            this.keySetPopupPanel = this.GetNode<PopupPanel>("KeySetPopupPanel");
            this.GetSavedValues();
            GD.Print(InputMap.GetActionList("ui_old"));
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("ui_cancel") && this.Visible && !this.keySetPopupPanel.Visible) {
                this.EmitSignal(nameof(BackButtonPress));
                SettingsManager.Instance.Save();
                return;
            }
            if (!this.keySetPopupPanel.Visible || this.actionToSet == null) {
                return;
            }
            if (@event is InputEventMouseButton mouse && !mouse.IsEcho()) {
                SettingsManager.Instance.SetActionKey(this.actionToSet, mouse);
                SettingsManager.Instance.SetValue(-mouse.ButtonIndex, this.actionToSet);
                this.SetActionValues(((ButtonList)mouse.ButtonIndex) + " msb");
            } else if (@event is InputEventKey key && !key.Echo) {
                SettingsManager.Instance.SetActionKey(this.actionToSet, key);
                SettingsManager.Instance.SetValue((int)key.Scancode, this.actionToSet);
                this.SetActionValues(OS.GetScancodeString(key.Scancode));
            }
        }

        private void SetActionValues(string buttonText, string action = null) {
            GD.Print(InputMap.GetActionList("ui_hold"));
            if (action == null) {
                action = this.actionToSet.Substring(3);
                action = char.ToUpper(action[0]) + action.Substring(1);
            }
            this.GetNode<Button>($"{this.path}/{action}Button").Text =
                buttonText;
            this.keySetPopupPanel.Visible = false;
            this.actionToSet = null;
        }

        private void GetSavedValues() {
            foreach (SettingsEnum setting in Enum.GetValues(typeof(SettingsEnum))) {
                int value = SettingsManager.Instance.GetValue(setting);
                if (setting.Equals(SettingsEnum.Sound) || setting.Equals(SettingsEnum.Music)) {
                    this.SetSliderValues(value > 0, Math.Abs(value), setting.ToString());
                    continue;
                }
                string key = value >= 0 ? OS.GetScancodeString((uint)value) : ((ButtonList)value).ToString();
                string action = setting.ToString().Substring(3);
                action = char.ToUpper(action[0]) + action.Substring(1);
                this.SetActionValues(key, action);
            }
        }

        private void SetSliderValues(bool enabled, int value, string setting) {
            this.GetNode<CheckButton>($"{this.path}/{setting}CheckButton").Pressed = enabled;
            this.GetNode<HSlider>($"{this.path}/{setting}Slider").Value = value;
            this.GetNode<Label>($"{this.path}/{setting}ValueLabel").Text = value.ToString();
            this.GetNode<Label>($"{this.path}/{setting}ValueLabel").SelfModulate =
                !enabled ? new Color(0.6f, 0.6f, 0.6f) : new Color(1, 1, 1);
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
            SettingsManager.Instance.SetValue(this.GetSettingValues(SettingsEnum.Sound.ToString()),
                SettingsEnum.Sound.ToString());
        }

        public void OnMusicValueChanged(float value) {
            this.GetNode<Label>($"{this.path}/MusicValueLabel").Text =
                ((int)(value)).ToString();
            SettingsManager.Instance.SetValue(this.GetSettingValues(SettingsEnum.Music.ToString()),
                SettingsEnum.Music.ToString());
        }

        public void OnSoundButtonToggled(bool value) {
            this.ButtonToggled(value, "Sound");
        }

        public void OnMusicButtonToggled(bool value) {
            this.ButtonToggled(value, "Music");
        }

        private void OnBackButtonPressed() {
            this.Visible = false;
            this.EmitSignal(nameof(BackButtonPress));
            SettingsManager.Instance.Save();
        }

        public void OnSettingsButtonPressed() {
            this.Visible = true;
        }

        public void OnResetButtonPressed() {
            SettingsManager.Instance.Reset();
            this.GetSavedValues();
        }

        public void OnSliderMouseEntered() {
            this.scrollContainer.MouseFilter = MouseFilterEnum.Ignore;
        }

        public void OnSliderMouseExited() {
            this.scrollContainer.MouseFilter = MouseFilterEnum.Stop;
        }

        public void OnLeftSetButtonPressed() {
            this.actionToSet = "ui_left";
            this.keySetPopupPanel.PopupCentered();
        }

        public void OnRightSetButtonPressed() {
            this.actionToSet = "ui_right";
            this.keySetPopupPanel.PopupCentered();
        }

        public void OnUpSetButtonPressed() {
            this.actionToSet = "ui_up";
            this.keySetPopupPanel.PopupCentered();
        }

        public void OnDownSetButtonPressed() {
            this.actionToSet = "ui_down";
            this.keySetPopupPanel.PopupCentered();
        }

        public void OnHoldSetButtonPressed() {
            this.actionToSet = "ui_hold";
            this.keySetPopupPanel.PopupCentered();
        }

        public void OnSelectSetButtonPressed() {
            this.actionToSet = "ui_select";
            this.keySetPopupPanel.PopupCentered();
        }

    }

}