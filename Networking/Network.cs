using Godot;

namespace VampireSurvivorsLike {

    public class Network : CanvasLayer {

        private const int DefaultPort = 1234; // 8910;
        private const int MaxNumberOfPeers = 1;

        private LineEdit address;
        private SpinBox port;
        private NetworkedMultiplayerENet peer;
        private Button hostButton;
        private Button joinButton;

        [Signal] public delegate void StatusClose();

        public override void _Ready() {
            GD.Print("Network ready...");
            this.address = GetNode<LineEdit>("Control/VBoxContainer/AddressLineEdit");
            this.port = GetNode<SpinBox>("Control/VBoxContainer/PortContainer/SpinBox");
            this.hostButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/HostButton");
            this.joinButton = GetNode<Button>("Control/VBoxContainer/HBoxContainer/JoinButton");
            this.address.Text = this.GetIpAddress();

            GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
            GetTree().Connect("connected_to_server", this, nameof(ConnectedOk));
            GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
            GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        private void ShowStatus(string status) {
            Label label = this.GetNode<Label>("Status/CenterContainer/VBoxContainer/Label");
            label.Text = status;
            this.GetNode<Control>("Status").Visible = true;
        }

        public void OnStatusCloseButtonPressed() {
            this.GetNode<Control>("Status").Visible = false;
            this.EmitSignal(nameof(StatusClose));
        }

        private string GetIpAddress() {
            foreach (string addr in IP.GetLocalAddresses()) {
                GD.Print(addr);
                if (addr.BeginsWith("192.168.")) {
                    return addr;
                }
            }
            return "127.0.0.1";
        }

        private void PlayerConnected(int id) {
            GD.Print("Player connected: " + id);
            this.peer.RefuseNewConnections = true;
            this.GetTree().Paused = true;
            this.GetTree().ChangeScene("res://Main/Main.tscn");
            this.GetNode<Control>("Control").Visible = false;
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
        }

        private void PlayerDisconnected(int id) {
            if (GameStateManagerSingleton.Instance.GameState == GameStateEnum.GameFinished) {
                return;
            }
            GD.Print("Player disconnected: " + id);
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
            this.ShowStatus("Player disconnected");
            this.GetTree().NetworkPeer = null;
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedOk() {
            this.GetTree().Paused = true;
            this.GetNode<Control>("Control").Visible = false;
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedFail() {
            GD.Print("Connected Fail...");
            this.peer = null;
        }

        private void ServerDisconnected() {
            if (GameStateManagerSingleton.Instance.GameState == GameStateEnum.GameFinished) {
                return;
            }
            GD.Print("ServerDisconnected...");
            this.peer = null;
            this.GetTree().ChangeScene("res://GUI/Menu/Menu.tscn");
            this.ShowStatus("Server disconnected");
        }


        public void ConnectionClosed() {
            this.peer?.CloseConnection();
            this.peer = null;
        }

        public void OnBackButtonPressed() {
            this.ConnectionClosed();
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            this.GetNode<Control>("Control").Visible = false;
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
        }

        public void OnCancelButtonPressed() {
            this.ConnectionClosed();
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
        }

        public void OnHostButtonPressed() {
            this.GetTree().NetworkPeer = null;
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            this.peer.SetBindIp(this.address.Text);
            Error err = this.peer.CreateServer((int)this.port.Value, MaxNumberOfPeers);
            int i = 0;
            while (err != Error.Ok && ++i < 9000) {
                err = this.peer.CreateServer(DefaultPort + i, MaxNumberOfPeers);
            }
            if (i > 0) {
                ShowStatus($"Failed to open server on port: {this.port.Value}\nNew port is: {DefaultPort + i}");
                this.port.Value = DefaultPort + i;
            }
            GD.Print(i);
            if (err != Error.Ok) {
                GD.Print(err);
                return;
            }

            this.GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = true;
            this.joinButton.Disabled = true;
            GD.Print("Waiting for player....");
        }

        public void OnJoinButtonPressed() {
            GD.Print("Connecting...");
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
                GD.Print(err);
                this.ShowStatus(err.ToString());
                return;
            }
            GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = true;
            this.joinButton.Disabled = true;
        }

    }

}