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
        public HUD Hud { get; set; }
        private int numberOfRewards = 4;
        private List<object> GeneratedRewards { get; set; }
        public bool CurrentlyRewardSelecting { get; private set; }
        private bool IsRewardSelected { get; set; } = false;

        public EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public static LevelUpManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("LevelUpManagerSingleton ready...");
                    instance = new LevelUpManagerSingleton();
                }
                return instance;
            }
        }

        private void Init() {
        }

        public async Task OnLevelUp(int numberOfLevelUps) {
            this.CurrentlyRewardSelecting = true;

            for (int i = 0; i < numberOfLevelUps; i++) {
                this.IsRewardSelected = false;
                this.GeneratedRewards = this.GenerateRewards();
                while (!this.IsRewardSelected) {
                    await Task.Delay(250);
                }
            }


            this.CurrentlyRewardSelecting = false;
        }

        private List<object> GenerateRewards() {
            List<object> possibleRewards = new List<object>();
            possibleRewards.AddRange(ItemManagerSingleton.Instance.GetEquippedItems()
                .Where(item => item.Level < item.MaxLevel).Select(item => item));
            possibleRewards.AddRange(ItemManagerSingleton.Instance.GetUnequippedItems());
            possibleRewards.AddRange(AttributeManagerSingleton.Instance.GetAttributes());

            List<object> rewards = new List<object>();
            for (int i = 0; i < this.numberOfRewards; i++) {
                int index = (int)GD.RandRange(0, possibleRewards.Count);
                rewards.Add(possibleRewards[index]);
                possibleRewards.RemoveAt(index);
            }
            this.LevelUpScreen.SetRewards(rewards.Select(reward => reward.ToString()).ToList());
            return rewards;
        }

        public void OnRewardSelected(int index) {
            object reward = this.GeneratedRewards[index];
            if (reward is Item item) {
                ItemManagerSingleton.Instance.EquipOrUpgradeItem(item);
            } else if (reward is Attribute attribute) {
                attribute.Increase();
                if (attribute.ToString().Contains("max health")) {
                    this.Hud.SetHealthLabel(attribute.GetCurrentValue());
                }
            }
            this.IsRewardSelected = true;
        }

        public void Reset() {
            GD.Print("LevelUpManager reset...");
        }

    }

}