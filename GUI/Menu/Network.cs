using Godot;
using System;

namespace VampireSurvivorsLike {

    public class Network : Control {

        private const int DefaultPort = 8910; // An arbitrary number.
        private const int MaxNumberOfPeers = 1; // How many people we want to have in a game

        private LineEdit address;
        private NetworkedMultiplayerENet peer;
        private Button hostButton;
        private Button joinButton;

        public override void _Ready() {
            this.address = GetNode<LineEdit>("VBoxContainer/AddressLineEdit");
            this.hostButton = GetNode<Button>("VBoxContainer/HBoxContainer/HostButton");
            this.joinButton = GetNode<Button>("VBoxContainer/HBoxContainer/JoinButton");

            this.address.Text = this.GetIpAddress();


            GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
            GetTree().Connect("connected_to_server", this, nameof(ConnectedOk));
            GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
            GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        private string GetIpAddress() {
            string ip = IP.GetLocalAddresses()[3].ToString();
            foreach (string addr in IP.GetLocalAddresses()) {
                if (addr.BeginsWith("192.168.")) {
                    ip = addr;
                    return ip;
                }
            }
            return ip;
        }

        private void PlayerConnected(int id) {
            GD.Print("Player connected: " + id);
        }

        private void PlayerDisconnected(int id) {
            GD.Print("Player disconnected: " + id);
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedOk() {
            GD.Print("ConnectedOk...");
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedFail() {
            GD.Print("ConnectedFail...");
        }

        private void ServerDisconnected() {
            GD.Print("ServerDisconnected...");
        }


        public void OnHostButtonPressed() {
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            Error err = this.peer.CreateServer(DefaultPort, MaxNumberOfPeers);

            this.GetTree().NetworkPeer = this.peer;
            this.hostButton.Disabled = true;
            this.joinButton.Disabled = true;
            GD.Print("Waiting for player??????");
        }

        public void OnJoinButtonPressed() {
            string ip = this.address.Text;
            if (!ip.IsValidIPAddress()) {
                GD.Print("IP address is invalid");
                return;
            }

            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            this.peer.CreateClient(ip, DefaultPort);
            GetTree().NetworkPeer = this.peer;
            GD.Print("Connecting...");
        }

        public void OnBackButtonPressed() {
            this.peer.CloseConnection();
            this.peer = null;
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
        }

    }

}