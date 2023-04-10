using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Menu : Control {

        private CenterContainer mainMenu;
        private AnimationPlayer animationPlayer;
        private Control images;
        private Control shop;
        private Button lastButtonInFocus;
        private Control network;
        private Control settings;

        public override void _Ready() {
            mainMenu = GetNode<CenterContainer>("MainMenu");
            this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            this.images = GetNode<Control>("Images");
            this.shop = GetNode<Control>("Shop");
            this.lastButtonInFocus = mainMenu.GetChild(0).GetChild<Button>(0);
            this.network = this.GetTree().Root.GetNode<Control>("Network/Control");
            this.settings = this.GetNode<Settings>("Settings");

            for (int i = 0; i < this.images.GetChildCount() - 1; i++) {
                Animation animation = new Animation();
                animation.Length = 20;
                animation.Loop = false;
                animation.AddTrack(Animation.TrackType.Value, 0);
                animation.AddTrack(Animation.TrackType.Value, 1);
                animation.TrackSetPath(0, this.GetPathTo(this.images.GetChild<TextureRect>(i)) + ":rect_position:x");
                animation.TrackInsertKey(0, 0, 0);
                animation.TrackInsertKey(0, animation.Length, -500);
                animation.TrackSetPath(1, this.GetPathTo(this.images.GetChild<TextureRect>(i)) + ":visible");
                animation.TrackInsertKey(1, 0, true);
                animation.TrackInsertKey(1, animation.Length, false);
                this.animationPlayer.AddAnimation(i.ToString(), animation);
                this.animationPlayer.AnimationSetNext(i.ToString(),
                    ((i + 1) % (this.images.GetChildCount() - 1)).ToString());
            }
            this.animationPlayer.Play("0");
            GetNode<AnimationPlayer>("FadeAnimationPlayer").Play("Fade");
            this.network.Connect("visibility_changed", this, nameof(this.OnNetworkVisibilityChanged));
            this.network.GetParent<Network>().Connect(nameof(Network.StatusClose), this, nameof(OnStatusClosed));
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionReleased("ui_cancel")) {
                this.mainMenu.Visible = true;
                this.shop.Visible = false;
                this.network.GetParent<Network>().OnBackButtonPressed();
                this.settings.Visible = false;
                this.lastButtonInFocus.GrabFocus();
            }
        }

        public void OnStatusClosed() {
            this.lastButtonInFocus.GrabFocus();
        }

        public void OnStartButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            LevelUpManagerSingleton.Instance.Reset();
            GetTree().ChangeScene("res://Main/Main.tscn");
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(0);
            AudioPlayerSingleton.Instance.SwitchMusicType(AudioTypeEnum.Action, false);
        }

        public void OnMultiplayerButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(1);
            this.mainMenu.Visible = false;
            this.network.Visible = true;
        }

        public void OnShopButtonPressed() {
            GD.Print("Shop button pressed");
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(2);
            this.mainMenu.Visible = false;
            this.shop.Visible = true;
        }

        public void OnSettingsButtonPressed() {
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(3);
            this.mainMenu.Visible = false;
            this.settings.Visible = true;
        }

        public void OnQuitButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            GetTree().Quit();
        }

        public void OnMainMenuVisibilityChanged() {
            if (this.mainMenu.Visible) {
                this.lastButtonInFocus.GrabFocus();
                AudioPlayerSingleton.Instance.SwitchMusicType(AudioTypeEnum.Ambient);
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.Menu;
            }
        }

        public void OnBackButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            this.mainMenu.Visible = true;
            this.shop.Visible = false;
            this.network.Visible = false;
            this.lastButtonInFocus.GrabFocus();
        }

        public void OnNetworkVisibilityChanged() {
            GD.Print("network visibility");
            if (this.network.Visible) {
                return;
            }
            this.mainMenu.Visible = true;
            this.shop.Visible = false;
            this.lastButtonInFocus.GrabFocus();
        }

    }

}