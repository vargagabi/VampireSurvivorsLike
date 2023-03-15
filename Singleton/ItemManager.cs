using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class ItemManager : Node2D {

        private readonly List<PackedScene> allItems = new List<PackedScene>() {
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Gun/Gun.tscn"),
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Aura/Aura.tscn"),
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/FireRing/FireRing.tscn")
        };
        public List<Item> ItemNodes { get; } = new List<Item>();
        private const int maximumItemCount = 6;
        public int EquippedItemCount { get; set; }

        public override void _Ready() {
            this.ItemNodes.Clear();
            this.EquippedItemCount = 0;
            foreach (PackedScene scene in this.allItems) {
                this.ItemNodes.Add(scene.Instance<Item>());
            }
        }

        public void EquipOrUpgradeItem(int itemIndex) {
            if (maximumItemCount <= this.ItemNodes.Count) {
                return;
            }
            if (!this.GetChildren().Contains(this.ItemNodes[itemIndex])) {
                this.AddChild(this.ItemNodes[itemIndex]);
                this.EquippedItemCount++;
            }
            this.ItemNodes[itemIndex].Upgrade();
            this.GetParent<Player>().Gui
                .SetItemOnHud(itemIndex, this.ItemNodes[itemIndex].Icon, this.ItemNodes[itemIndex].Level);

            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                this.ItemNodes[itemIndex].SetNetworkMaster(this.GetTree().GetNetworkUniqueId());
                Rpc(nameof(this.PuppetEquipOrUpgradeItem), itemIndex);
            }

            // this.hud.SetItemLevel(this.equippedItems.FindIndex(i=>i.Equals(item)),item.Level);
        }

        [Puppet]
        public void PuppetEquipOrUpgradeItem(int itemIndex) {
            if (maximumItemCount <= this.ItemNodes.Count) {
                return;
            }
            if (!this.GetChildren().Contains(this.ItemNodes[itemIndex])) {
                this.AddChild(this.ItemNodes[itemIndex]);
                this.EquippedItemCount++;
            }
            this.ItemNodes[itemIndex].Upgrade();
            this.ItemNodes[itemIndex].SetNetworkMaster(this.GetTree().GetRpcSenderId());
        }


        public Array GetEquippedItems() {
            return this.GetChildren();
        }

    }

}