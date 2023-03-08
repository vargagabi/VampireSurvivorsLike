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
        private readonly List<Item> itemNodes = new List<Item>();
        private const int maximumItemCount = 6;
        public int EquippedItemCount { get; set; }

        public override void _Ready() {
            this.itemNodes.Clear();
            this.EquippedItemCount = 0;
            foreach (PackedScene scene in this.allItems) {
                this.itemNodes.Add(scene.Instance<Item>());
            }
        }

        public void EquipOrUpgradeItem(int itemIndex) {
            if (maximumItemCount <= this.itemNodes.Count) {
                return;
            }
            if (!this.GetChildren().Contains(this.itemNodes[itemIndex])) {
                this.AddChild(this.itemNodes[itemIndex]);
                this.EquippedItemCount++;
            }
            this.itemNodes[itemIndex].Upgrade();
            this.GetParent<Player>().Gui.SetItemOnHud(itemIndex, this.itemNodes[itemIndex].Icon, this.itemNodes[itemIndex].Level);

            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                Rpc(nameof(this.PuppetEquipOrUpgradeItem), itemIndex);
            }
            // this.hud.SetItemLevel(this.equippedItems.FindIndex(i=>i.Equals(item)),item.Level);
        }

        [Puppet]
        public void PuppetEquipOrUpgradeItem(int itemIndex) {
            if (maximumItemCount <= this.itemNodes.Count) {
                return;
            }
            if (!this.GetChildren().Contains(this.itemNodes[itemIndex])) {
                this.AddChild(this.itemNodes[itemIndex]);
                this.EquippedItemCount++;
            }
            this.itemNodes[itemIndex].Upgrade();
        }


        public Array GetEquippedItems() {
            return this.GetChildren();
        }

    }

}