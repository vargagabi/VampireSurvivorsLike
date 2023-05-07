using Godot;

namespace VampireSurvivorsLike {

    public class Network : CanvasLayer {

        private const int DefaultPort = 1234;

        private LineEdit address;
        private SpinBox port;
        private NetworkedMultiplayerENet peer;
        private Button hostButton;
        private Button joinButton;

        [Signal] public delegate void StatusClose();
        [Signal] public delegate void BackButtonPressed();

        public override void _Ready() {
            this.address = this.GetNode<LineEdit>("Control/VBoxContainer/AddressLineEdit");
            this.port = this.GetNode<SpinBox>("Control/VBoxContainer/PortContainer/SpinBox");
            this.hostButton = this.GetNode<Button>("Control/VBoxContainer/HBoxContainer/HostButton");
            this.joinButton = this.GetNode<Button>("Control/VBoxContainer/HBoxContainer/JoinButton");
            this.address.Text = GetIpAddress();

            this.GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
            this.GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
            this.GetTree().Connect("connected_to_server", this, nameof(ConnectedOk));
            this.GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
            this.GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        private void ShowStatus(string status) {
            this.GetNode<Label>("Status/CenterContainer/VBoxContainer/Label").Text = status;
            this.GetNode<Control>("Status").Visible = true;
        }

        private static string GetIpAddress() {
            foreach (string addr in IP.GetLocalAddresses()) {
                if (addr.BeginsWith("192.168.")) {
                    return addr;
                }
            }
            return "127.0.0.1";
        }

        private void PlayerConnected(int id) {
            this.peer.RefuseNewConnections = true;
            this.GetTree().Paused = true;
            this.GetTree().ChangeScene("res://Main/Main.tscn");
            this.GetNode<Control>("Control").Visible = false;
            this.GetNode<Control>("Status").Visible = false;
            this.hostButton.Disabled = this.joinButton.Disabled = false;
        }

        private void PlayerDisconnected(int id) {
            if (GameStateManagerSingleton.Instance.GameState == GameStateEnum.GameFinished) {
                return;
            }
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
            this.ShowStatus("Player disconnected");
            this.GetTree().NetworkPeer = null;
        }

        private void ConnectedOk() {
            this.GetTree().Paused = true;
            this.GetNode<Control>("Control").Visible = false;
            this.GetNode<Control>("Status").Visible = false;
        }

        private void ConnectedFail() {
            this.peer = null;
        }

        private void ServerDisconnected() {
            if (GameStateManagerSingleton.Instance.GameState == GameStateEnum.GameFinished) {
                return;
            }
            this.peer = null;
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
            this.ShowStatus("Server disconnected");
        }


        public void ConnectionClosed() {
            this.peer?.CloseConnection();
            this.peer = null;
        }

        public void OnHostButtonPressed() {
            this.GetTree().NetworkPeer = null;
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            this.peer.SetBindIp(this.address.Text);
            Error err = this.peer.CreateServer((int)this.port.Value, 1);
            int i = 0;
            while (err != Error.Ok && ++i < 50000) {
                err = this.peer.CreateServer(DefaultPort + i, 1);
            }
            if (err != Error.Ok) {
                this.ShowStatus($"Error: {err}");
                return;
            }
            if (i > 0) {
                this.ShowStatus($"Failed to open server on port: {this.port.Value}\nNew port is: {DefaultPort + i}");
                this.port.Value = DefaultPort + i;
            }

            this.GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = this.joinButton.Disabled = true;
        }

        public void OnJoinButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            string ip = this.address.Text;
            if (!ip.IsValidIPAddress()) {
                this.ShowStatus("Invalid ip address");
                return;
            }
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            Error err = this.peer.CreateClient(ip, (int)this.port.Value);
            if (err != Error.Ok) {
                this.ShowStatus(err.ToString());
                return;
            }
            this.GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = this.joinButton.Disabled = true;
        }

        public void OnBackButtonPressed() {
            this.ConnectionClosed();
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            this.GetNode<Control>("Control").Visible = false;
            this.hostButton.Disabled = this.joinButton.Disabled = false;
            this.EmitSignal(nameof(BackButtonPressed));
        }

        public void OnCancelButtonPressed() {
            this.ConnectionClosed();
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            this.hostButton.Disabled = this.joinButton.Disabled = false;
        }

        public void OnStatusCloseButtonPressed() {
            this.GetNode<Control>("Status").Visible = false;
            this.EmitSignal(nameof(StatusClose));
        }

    }

}