using System;
using Godot;

namespace VampireSurvivorsLike {

    public class Menu : Control {

        private CenterContainer mainMenu;
        private AnimationPlayer animationPlayer;
        private Control images;
        private Control shop;
        private Button lastButtonInFocus;
        private Control multiplayer;

        public override void _Ready() {
            mainMenu = GetNode<CenterContainer>("MainMenu");
            this.animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            this.images = GetNode<Control>("Images");
            this.shop = GetNode<Control>("Shop");
            this.lastButtonInFocus = mainMenu.GetChild(0).GetChild<Button>(0);
            this.multiplayer = this.GetChild<Control>(5);

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
        }

        public override void _Input(InputEvent @event) {
            if (@event.IsActionReleased("ui_cancel")) {
                this.mainMenu.Visible = true;
                this.shop.Visible = false;
                this.lastButtonInFocus.GrabFocus();
            }
        }

        public void OnStartButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            LevelUpManagerSingleton.Instance.Reset();
            // ItemManagerSingleton.Instance.Reset();
            GetTree().ChangeScene("res://Main/Main.tscn");
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(0);
            AudioPlayerSingleton.Instance.SwitchToAction(false);
        }

        public void OnMultiplayerButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(1);
            this.mainMenu.Visible = false;
            this.multiplayer.Visible = true;
        }

        public void OnShopButtonPressed() {
            GD.Print("Shop button pressed");
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(2);
            this.mainMenu.Visible = false;
            this.shop.Visible = true;
        }

        public void OnSettingsButtonPressed() {
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(3);
        }

        public void OnQuitButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            GetTree().Quit();
        }

        public void OnMainMenuVisibilityChanged() {
            if (this.mainMenu.Visible) {
                this.lastButtonInFocus.GrabFocus();
                AudioPlayerSingleton.Instance.SwitchToAmbient();
                GameStateManagerSingleton.Instance.GameState = GameStateEnum.Menu;
            }
        }

        public void OnBackButtonPressed() {
            AttributeManagerSingleton.Instance.Save();
            this.mainMenu.Visible = true;
            this.shop.Visible = false;
            this.multiplayer.Visible = false;
            this.lastButtonInFocus.GrabFocus();
        }


    }

}