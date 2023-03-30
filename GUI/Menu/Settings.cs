using System;
using System.Linq;
using Godot;

namespace VampireSurvivorsLike {

    public class Settings : Control {

        private string path = "CenterContainer/VBoxContainer/ScrollContainer/SettingsContainer/";
        private ScrollContainer scrollContainer;
        private PopupPanel keySetPopupPanel;
        private string actionToSet = null;

        [Signal] public delegate void BackButtonPress();

        public override void _Ready() {
            this.Connect("visibility_changed", this, nameof(this.OnVisibilityChanged));
            this.scrollContainer = this.GetNode<ScrollContainer>($"{this.path}../");
            this.keySetPopupPanel = this.GetNode<PopupPanel>("KeySetPopupPanel");

            this.CreateControlSettings();
            this.GetSavedValues();
        }

        private void CreateControlSettings() {
            GridContainer container = this.GetNode<GridContainer>(this.path);
            bool firstRow = true;
            foreach (string setting in Enum.GetNames(typeof(SettingsEnum)).Where(s => s.Contains('_'))) {
                Label label = new Label();
                label.SizeFlagsHorizontal = label.SizeFlagsVertical = (int)SizeFlags.Fill;
                label.Align = Label.AlignEnum.Left;
                label.Valign = Label.VAlign.Center;
                label.Text = setting.Substring(3);
                container.AddChild(label);
                Control control = new Control();
                control.SizeFlagsHorizontal = control.SizeFlagsVertical = (int)SizeFlags.Fill;
                container.AddChild(control);
                Button button = new Button();
                button.Name = setting + "Button";
                button.SizeFlagsHorizontal = button.SizeFlagsVertical = (int)SizeFlags.Fill;
                container.AddChild(button);
                container.AddChild(control.Duplicate());
                button.Connect("pressed", this, $"On{char.ToUpper(setting[3])}{setting.Substring(4)}SetButtonPressed");
                if (firstRow) {
                    button.FocusNeighbourTop = new NodePath("../MusicCheckButton");
                    firstRow = false;
                }
            }
        }

        public void OnVisibilityChanged() {
            this.GetNode<Button>("HBoxContainer/BackButton").GrabFocus();
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionPressed("ui_cancel") && this.Visible) {
                if (this.keySetPopupPanel.Visible) {
                    this.GetTree().SetInputAsHandled();
                } else {
                    SettingsManager.Instance.Save();
                }
                return;
            }
            if (!(@event is InputEventKey key) || key.Echo || !this.keySetPopupPanel.Visible ||
                this.actionToSet == null) {
                return;
            }

            this.GetTree().SetInputAsHandled();
            if (!this.actionToSet.Equals(SettingsEnum.ui_accept.ToString()) &&
                !this.actionToSet.Equals(SettingsEnum.ui_hold.ToString()) &&
                (key.Scancode.Equals((int)KeyList.Escape) ||
                 ((int)key.Scancode).Equals((int)SettingsManager.Instance.GetValue(SettingsEnum.ui_accept)))) {
                this.keySetPopupPanel.GetChild<Label>(0).Text =
                    $"Can't assign the {OS.GetScancodeString(key.Scancode)} button to a direction action.\nPress another button.";
                return;
            }
            this.keySetPopupPanel.GetChild<Label>(0).Text = "Press a button.";
            SettingsManager.Instance.SetActionKey(this.actionToSet, key);
            SettingsManager.Instance.SetValue(this.actionToSet, (int)key.Scancode);
            this.SetActionValues(OS.GetScancodeString(key.Scancode));
        }

        private void SetActionValues(string buttonText, string action = null) {
            if (action == null) {
                action = this.actionToSet;
            }

            this.GetNode<Button>($"{this.path}/{action}Button").Text = buttonText;
            this.keySetPopupPanel.Visible = false;
            this.actionToSet = null;
        }

        private void GetSavedValues() {
            foreach (SettingsEnum setting in Enum.GetValues(typeof(SettingsEnum))) {
                int value = (int)SettingsManager.Instance.GetValue(setting);
                if (setting.Equals(SettingsEnum.Sound) || setting.Equals(SettingsEnum.Music)) {
                    this.SetSliderValues(value > 0, Math.Abs(value), setting.ToString());
                    continue;
                }
                string key = value >= 0 ? OS.GetScancodeString((uint)value) : ((ButtonList)value).ToString();
                string action = setting.ToString();
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
            SettingsManager.Instance.SetValue(button, this.GetSettingValues(button));
        }

        public void OnSoundValueChanged(float value) {
            this.GetNode<Label>($"{this.path}/SoundValueLabel").Text =
                ((int)(value)).ToString();
            SettingsManager.Instance.SetValue(SettingsEnum.Sound.ToString(),
                this.GetSettingValues(SettingsEnum.Sound.ToString()));
        }

        public void OnMusicValueChanged(float value) {
            this.GetNode<Label>($"{this.path}/MusicValueLabel").Text =
                ((int)(value)).ToString();
            SettingsManager.Instance.SetValue(SettingsEnum.Music.ToString(),
                this.GetSettingValues(SettingsEnum.Music.ToString()));
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

        public void OnAcceptSetButtonPressed() {
            this.actionToSet = "ui_select";
            this.keySetPopupPanel.PopupCentered();
        }

    }

}