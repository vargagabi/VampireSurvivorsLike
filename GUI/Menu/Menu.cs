using Godot;

namespace VampireSurvivorsLike {

    public class Menu : Control {

        private CenterContainer mainMenu;
        private Shop shop;
        private Button lastButtonInFocus;
        private Control network;
        private Control settings;

        public override void _Ready() {
            AnimationPlayer animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
            Control images = this.GetNode<Control>("Images");
            this.mainMenu = this.GetNode<CenterContainer>("MainMenu");
            this.shop = this.GetNode<Shop>("Shop");
            this.network = this.GetTree().Root.GetNode<Control>("Network/Control");
            this.settings = this.GetNode<Settings>("Settings");
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(0);

            for (int i = 0; i < images.GetChildCount() - 1; i++) {
                Animation animation = new Animation();
                animation.Length = 20;
                animation.Loop = false;
                animation.AddTrack(Animation.TrackType.Value, 0);
                animation.AddTrack(Animation.TrackType.Value, 1);
                animation.TrackSetPath(0, this.GetPathTo(images.GetChild<TextureRect>(i)) + ":rect_position:x");
                animation.TrackInsertKey(0, 0, 0);
                animation.TrackInsertKey(0, animation.Length, -500);
                animation.TrackSetPath(1, this.GetPathTo(images.GetChild<TextureRect>(i)) + ":visible");
                animation.TrackInsertKey(1, 0, true);
                animation.TrackInsertKey(1, animation.Length, false);
                animationPlayer.AddAnimation(i.ToString(), animation);
                animationPlayer.AnimationSetNext(i.ToString(),
                    ((i + 1) % (images.GetChildCount() - 1)).ToString());
            }
            animationPlayer.Play("0");
            this.GetNode<AnimationPlayer>("FadeAnimationPlayer").Play("Fade");
            this.network.GetParent<Network>().Connect(nameof(Network.StatusClose), this, nameof(this.OnStatusClosed));
            this.network.GetParent<Network>()
                .Connect(nameof(Network.BackButtonPressed), this, nameof(this.OnBackButtonPressed));
        }

        public override void _Input(InputEvent @event) {
            if (!@event.IsActionPressed("ui_cancel")) {
                return;
            }
            this.mainMenu.Visible = true;
            this.shop.OnBackButtonPressed();
            this.network.GetParent<Network>().OnBackButtonPressed();
            this.settings.Visible = false;
            this.lastButtonInFocus.GrabFocus();
        }

        public void OnStatusClosed() {
            this.lastButtonInFocus.GrabFocus();
        }

        public void OnStartButtonPressed() {
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(0);
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            this.GetTree().ChangeScene("res://Main/Main.tscn");
            AudioPlayerSingleton.Instance.SwitchMusicType(AudioTypeEnum.Action, false);
        }

        public void OnMultiplayerButtonPressed() {
            this.lastButtonInFocus = this.mainMenu.GetChild(0).GetChild<Button>(1);
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            this.mainMenu.Visible = false;
            this.network.Visible = true;
        }

        public void OnShopButtonPressed() {
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
            this.GetTree().Quit();
        }

        public void OnMainMenuVisibilityChanged() {
            if (!this.mainMenu.Visible) {
                return;
            }
            this.lastButtonInFocus.GrabFocus();
            AudioPlayerSingleton.Instance.SwitchMusicType(AudioTypeEnum.Ambient);
            GameStateManagerSingleton.Instance.GameState = GameStateEnum.Menu;
        }

        public void OnBackButtonPressed() {
            this.mainMenu.Visible = true;
        }

    }

}