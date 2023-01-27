using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class ItemManagerSingleton {

        private static ItemManagerSingleton instance;

        private readonly List<PackedScene> allItems = new List<PackedScene>() {
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Gun/Gun.tscn"),
            ResourceLoader.Load<PackedScene>("res://Items/Weapons/Aura/Aura.tscn"),
        };
        private readonly List<Item> unequippedItems = new List<Item>();
        private const int maximumItemCount = 6;
        public int EquippedItemCount { get; set; }
        private List<Item> equippedItems = new List<Item>();


        public Player Player { get; set; }
        public HUD Hud { get; set; }

        public static ItemManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("ItemManagerSingleton ready...");
                    instance = new ItemManagerSingleton();
                }
                return instance;
            }
        }

        public void OnPlayerReady(Player player) {
            this.Player = player;
        }

        private ItemManagerSingleton() {
        }

        public void Reset() {
            this.unequippedItems.Clear();
            this.equippedItems.Clear();
            this.EquippedItemCount = 0;
            foreach (PackedScene scene in this.allItems) {
                this.unequippedItems.Add(scene.Instance<Item>());
            }
        }

        public void EquipOrUpgradeItem(Item item) {
            if (maximumItemCount <= this.equippedItems.Count || this.Player == null) {
                return;
            }
            if (!this.equippedItems.Contains(item)) {
                this.Player.AddChild(item);
                this.equippedItems.Add(item);
                this.unequippedItems.Remove(item);
                this.Hud.AddItem(item);
                this.EquippedItemCount++;
            }
            item.Upgrade();
            this.Hud.SetItemLevel(this.equippedItems.FindIndex(i=>i.Equals(item)),item.Level);
        }

        public List<Item> GetEquippedItems() {
            return this.equippedItems;
        }

        public List<Item> GetUnequippedItems() {
            return this.unequippedItems;
        }

    }

}