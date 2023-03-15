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

        public override void _Ready() {
            GD.Print("ItemDropManager ready...");
            Instance = this;
        }

        public void CreateExperienceOrb(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.CallDeferred("add_child", drop);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(PuppetCreateExperienceOrb), value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateExperienceOrb(int value, int direction, int distance, Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.CallDeferred("add_child", drop);
        }

        public void CreateGold(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.CallDeferred("add_child", drop);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(PuppetCreateGold), value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateGold(int value, int direction, int distance, Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            drop.Init(globalPosition, value, direction, distance, ItemDropsEnum.ExperienceOrb);
            parent.CallDeferred("add_child", drop);
        }

    }

}