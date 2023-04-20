using System;
using Godot;

namespace VampireSurvivorsLike.ItemDrops {

    public enum ItemDropsEnum { Gold, ExperienceOrb }

    public class ItemDropManager : Node2D {

        private static ItemDropManager instance;
        public static ItemDropManager Instance {
            get => instance;
            private set { instance = value; }
        }

        private readonly PackedScene experienceOrb =
            ResourceLoader.Load<PackedScene>("res://ItemDrops/ExpOrbs/ExpOrb.tscn");
        private readonly PackedScene gold = ResourceLoader.Load<PackedScene>("res://ItemDrops/Gold/Gold.tscn");
        private uint expOrbCount = 0;
        private uint goldCount = 0;

        public override void _Ready() {
            Instance = this;
        }

        public void CreateExperienceOrb(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            drop.Name = $"Exp{this.expOrbCount++}";
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.AddChild(drop, true);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(PuppetCreateExperienceOrb), drop.Name, value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateExperienceOrb(string name, int value, int direction, int distance,
            Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            drop.Name = name;
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.AddChild(drop, true);
        }

        public void CreateGold(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            drop.Name = $"Exp{this.goldCount++}";
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.Gold);
            parent.AddChild(drop, true);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(PuppetCreateGold), drop.Name, value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateGold(string name, int value, int direction, int distance, Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            drop.Name = name;
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.Gold);
            parent.AddChild(drop, true);
        }

    }

}