using Godot;
using System;
using System.Collections.Generic;

namespace VampireSurvivorsLike {

    public class Network : Control {

        private const int DefaultPort = 8910; // An arbitrary number.
        private const int MaxNumberOfPeers = 1; // How many people we want to have in a game

        private LineEdit address;
        private NetworkedMultiplayerENet peer;
        private Button hostButton;
        private Button joinButton;
        
        public List<string> peers = new List<string>();
        
        private Label status;

        public override void _Ready() {
            this.address = GetNode<LineEdit>("VBoxContainer/AddressLineEdit");
            this.hostButton = GetNode<Button>("VBoxContainer/HBoxContainer/HostButton");
            this.joinButton = GetNode<Button>("VBoxContainer/HBoxContainer/JoinButton");
            this.address.Text = this.GetIpAddress();
            
            this.status = GetNode<Label>("Status");

            GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
            GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
            GetTree().Connect("connected_to_server", this, nameof(ConnectedOk));
            GetTree().Connect("connection_failed", this, nameof(ConnectedFail));
            GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        private string GetIpAddress() {
            foreach (string addr in IP.GetLocalAddresses()) {
                if (addr.BeginsWith("192.168.")) {
                    return addr;
                }
            }
            return "127.0.0.1";
        }

        private void AddStatus(string msg) {
            this.status.Text = this.status.Text + "\n" + msg;
        }

        private void PlayerConnected(int id) {
            GD.Print("Player connected: " + id);
            this.AddStatus("Player connected: " + id);
            this.GetTree().NetworkPeer.RefuseNewConnections = true;
            this.peers.Add(id.ToString());
            this.AddStatus(string.Join(", ", this.peers.ToArray()));
            this.GetTree().Paused = true;
            this.GetTree().ChangeScene("res://Main/Main.tscn");
            
            
        }

        private void PlayerDisconnected(int id) {
            GD.Print("Player disconnected: " + id);
            this.AddStatus("Player disconnected: " + id);
            this.GetTree().NetworkPeer.RefuseNewConnections = false;
            this.peers.Remove(id.ToString());
            this.AddStatus(string.Join(", ", this.peers.ToArray()));
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedOk() {
            GD.Print("Connected Ok...");
            this.AddStatus("Connected Ok...");
            this.GetTree().Paused = true;
        }

        // Callback from SceneTree, only for clients (not server).
        private void ConnectedFail() {
            GD.Print("Connected Fail...");
            this.AddStatus("Connected Fail...");
            this.GetTree().NetworkPeer = null;
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
        }

        private void ServerDisconnected() {
            GD.Print("ServerDisconnected...");
            this.AddStatus("ServerDisconnected...");
        }


        public void OnHostButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            GD.Print("Waiting for player....");
            this.AddStatus("Waiting for player....");
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            Error err = this.peer.CreateServer(DefaultPort, MaxNumberOfPeers);
            if (err != Error.Ok) {
                GD.Print(err);
                return;
            }
            this.GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = true;
            this.joinButton.Disabled = true;
            
            this.peers.Clear();
            this.peers.Add(this.peer.GetUniqueId() + " (ME)");
            this.AddStatus(string.Join(", ", this.peers.ToArray()));
        }

        public void OnJoinButtonPressed() {
            GameStateManagerSingleton.Instance.IsMultiplayer = true;
            GD.Print("Connecting...");
            this.AddStatus("Connecting...");
            string ip = this.address.Text;
            if (!ip.IsValidIPAddress()) {
                GD.Print("IP address is invalid");
                return;
            }
            this.peer = new NetworkedMultiplayerENet();
            this.peer.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
            this.peer.CreateClient(ip, DefaultPort);
            GetTree().NetworkPeer = this.peer;

            this.hostButton.Disabled = true;
            this.joinButton.Disabled = true;
            
            this.peers.Clear();
            this.peers.Add(this.peer.GetUniqueId() + " (ME)");
            this.AddStatus(string.Join(", ", this.peers.ToArray()));
        }

        private void ConnectionClosed() {
            if (this.peer != null) {
                this.peer.CloseConnection();
                this.peer = null;
            }
            this.GetTree().NetworkPeer = null;
        }

        public void OnBackButtonPressed() {
            this.ConnectionClosed();
            this.hostButton.Disabled = false;
            this.joinButton.Disabled = false;
            GameStateManagerSingleton.Instance.IsMultiplayer = false;
        }

    }

}