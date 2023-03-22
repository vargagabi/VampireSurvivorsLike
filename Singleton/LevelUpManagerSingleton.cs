using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpManagerSingleton {

        private static LevelUpManagerSingleton instance;
        public LevelUpScreen LevelUpScreen { get; set; }
        private int numberOfRewards = 4;
        private List<object> GeneratedRewards { get; set; }
        public bool IsRewardSelected { get; private set; } = false;
        public Player Player { get; set; }
        public bool OtherPlayerFinished { get; set; } = false;

        public static LevelUpManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("LevelUpManagerSingleton ready...");
                    instance = new LevelUpManagerSingleton();
                }
                return instance;
            }
        }

        public async Task OnPlayerLevelUp(int numberOfLevelUps) {
            for (int i = 0; i < numberOfLevelUps; i++) {
                this.IsRewardSelected = false;
                this.GeneratedRewards = this.GenerateRewards();
                while (!this.IsRewardSelected) {
                    await Task.Delay(250);
                }
            }
        }

        private List<object> GenerateRewards() {
            List<object> possibleRewards = new List<object>();

            possibleRewards.AddRange(this.Player.ItemManager.ItemNodes
                .Where(item => item.Level < item.MaxLevel).Select(item => item));
            possibleRewards.AddRange(AttributeManagerSingleton.Instance.GetAttributes());

            List<object> rewards = new List<object>();
            for (int i = 0; i < this.numberOfRewards; i++) {
                int index = (int)GD.RandRange(0, possibleRewards.Count);
                rewards.Add(possibleRewards[index]);
                possibleRewards.RemoveAt(index);
            }
            this.LevelUpScreen.SetRewards(rewards);
            return rewards;
        }

        public void OnRewardSelected(int index) {
            object reward = this.GeneratedRewards[index];
            if (reward is Item item) {
                this.Player.ItemManager.EquipOrUpgradeItem(
                    this.Player.ItemManager.ItemNodes.FindIndex(val => val.Id == item.Id));
            } else if (reward is Attribute attribute) {
                attribute.Increase();
            }
            this.IsRewardSelected = true;
        }

        public void Reset() {
            GD.Print("LevelUpManager reset...");
        }

    }

}