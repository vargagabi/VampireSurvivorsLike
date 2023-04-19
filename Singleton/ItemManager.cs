using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class ItemManager : Node2D {

        public readonly List<Item> ItemNodes = new List<Item>();
        private readonly List<PackedScene> allItems = new List<PackedScene> {
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Gun/Gun.tscn"),
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Aura/Aura.tscn"),
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/FireRing/FireRing.tscn")
        };
        public const int MaximumItemCount = 6;
        private int EquippedItemCount { get; set; }

        public override void _Ready() {
            this.ItemNodes.Clear();
            this.EquippedItemCount = 0;
            for (int i = 0; i < this.allItems.Count; i++) {
                this.ItemNodes.Add(this.allItems[i].Instance<Item>());
                this.ItemNodes[i].Id = i;
            }
        }

        public void EquipOrUpgradeItem(int itemIndex) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                Rpc(nameof(this.SyncEquipOrUpgradeItem), itemIndex);
            } else {
                this.SyncEquipOrUpgradeItem(itemIndex);
            }
            this.GetParent<Player>().Gui
                .SetItemOnHud(this.ItemNodes[itemIndex].Icon, this.ItemNodes[itemIndex].Level);
        }

        [PuppetSync]
        public void SyncEquipOrUpgradeItem(int itemIndex) {
            if (MaximumItemCount <= this.ItemNodes.Count) {
                return;
            }
            if (!this.GetChildren().Contains(this.ItemNodes[itemIndex])) {
                this.AddChild(this.ItemNodes[itemIndex]);
                this.EquippedItemCount++;
            }
            this.ItemNodes[itemIndex].Upgrade();
            this.ItemNodes[itemIndex].SetNetworkMaster(this.GetTree().GetRpcSenderId());
        }

    }

}