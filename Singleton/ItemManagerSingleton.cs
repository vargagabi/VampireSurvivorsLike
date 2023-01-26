using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class ItemManagerSingleton {

        private static ItemManagerSingleton instance;

        private readonly List<PackedScene> allItems = new List<PackedScene>();
        private readonly List<Item> unequippedItems = new List<Item>();
        private const int maximumItemCount = 6;
        private List<Item> equippedItems = new List<Item>();

        // public Player Player { get; set; }
        public HUD Hud { get; set; }

        public static ItemManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("ItemManagerSingleton ready...");
                    instance = new ItemManagerSingleton();
                    InitInstance();
                }
                return instance;
            }
        }

        private static void InitInstance() {
            instance.allItems.Add(ResourceLoader.Load<PackedScene>("res://Items/Weapons/Gun/Gun.tscn"));
            instance.allItems.Add(ResourceLoader.Load<PackedScene>("res://Items/Weapons/Aura/Aura.tscn"));
            ResetItemsStatic();
        }

        private ItemManagerSingleton() {
        }

        private static void ResetItemsStatic() {
            instance.unequippedItems.Clear();
            instance.equippedItems = new List<Item>();
            foreach (PackedScene scene in instance.allItems) {
                instance.unequippedItems.Add(scene.Instance<Item>());
            }
        }

        public void ResetItems() {
            this.unequippedItems.Clear();
            this.equippedItems = new List<Item>();
            foreach (PackedScene scene in this.allItems) {
                this.unequippedItems.Add(scene.Instance<Item>());
            }
        }

        public void EquipItem(Item item) {
            // if (maximumItemCount <= this.equippedItems.Count || this.Player == null) {
                // return;
            // }
            this.equippedItems.Add(item);
            this.unequippedItems.Remove(item);
            item.Upgrade();
            // this.Player.AddChild(item);
        }

        public List<Item> GetEquippedItems() {
            return this.equippedItems;
        }

        public List<Item> GetUnequippedItems() {
            return this.unequippedItems;
        }

    }

}