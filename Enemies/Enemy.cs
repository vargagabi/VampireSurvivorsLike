using System;
using Godot;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Enemy : KinematicBody2D {

        public static int MobCount = 0;
        public int SpawnRate { get; protected set; }
        public float SpawnDistance { get; protected set; }
        public int Strength { get; set; }
        public Vector2 SpawnTime { get; protected set; }
        protected int Health;
        protected int ExpValue;
        protected float Speed;

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        protected AnimatedSprite AnimatedSprite { get; set; }

        private readonly PackedScene experienceOrb =
            ResourceLoader.Load<PackedScene>("res://ItemDrops/ExpOrbs/ExpOrb.tscn");
        private readonly PackedScene gold = ResourceLoader.Load<PackedScene>("res://ItemDrops/Gold/Gold.tscn");
        private uint expOrbCount = 0;
        private uint goldCount = 0;

        public override void _Ready() {
            base._Ready();
            GD.Randomize();
            this.AnimatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.AnimatedSprite.Connect("animation_finished", this, nameof(this.OnAnimationFinished));
            this.AnimationPlay(EnemyAnimationsEnum.Walk);
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                Timer timer = new Timer();
                timer.OneShot = false;
                timer.WaitTime = 3;
                timer.Autostart = true;
                timer.Connect("timeout", this, nameof(OnTimerTimeout));
                this.AddChild(timer);
            }
        }

        protected Vector2 GetTargetPosition() {
            if (this.PlayerTwo == null) {
                return this.PlayerOne.GlobalPosition;
            }
            if (this.PlayerOne.IsDead) {
                return this.PlayerTwo.GlobalPosition;
            }
            if (this.PlayerTwo.IsDead) {
                return this.PlayerOne.GlobalPosition;
            }
            return this.GlobalPosition.DistanceTo(this.PlayerOne.GlobalPosition) <
                   this.GlobalPosition.DistanceTo(this.PlayerTwo.GlobalPosition)
                ? this.PlayerOne.GlobalPosition
                : this.PlayerTwo.GlobalPosition;
        }


        protected void AnimationPlay(EnemyAnimationsEnum enemyAnimations) {
            if (this.Health <= 0 && enemyAnimations != EnemyAnimationsEnum.Death) {
                return;
            }
            this.AnimatedSprite.Play(enemyAnimations.ToString());
        }

        public void OnAnimationFinished() {
            if (this.AnimatedSprite.Animation != EnemyAnimationsEnum.Death.ToString()) {
                return;
            }
            this.QueueFree();
            if (new Random().Next(0, 4) == 0) {
                this.CreateExperienceOrb(this.ExpValue, this.GlobalPosition);
            }
            if (new Random().Next(0, 5) == 0) {
                this.CreateGold(new Random().Next(1, 10), this.GlobalPosition);
            }
        }

        /*
         * When receiving damage reduce the health by damage amount.
         * If the health is less than or equals 0 remove the enemy and add an
         * instance of an ExpOrb at its place.
         */
        public void OnHit(int damage, Weapon weapon) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.SyncTakeDamage), damage);
            } else {
                this.SyncTakeDamage(damage);
            }
            if (this.Health > 0) {
                return;
            }
            if (weapon is Aura aura) {
                this.ExpValue += (int)(this.ExpValue * aura.BonusExperience);
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.SyncOnDeath), this.ExpValue, this.GlobalPosition);
            } else {
                this.SyncOnDeath(this.ExpValue, this.GlobalPosition);
            }
        }

        [RemoteSync]
        private void SyncTakeDamage(int damage) {
            if (this.Health <= 0) {
                return;
            }
            AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.EnemyHit);
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.24f, 0.24f), damage,
                this.GetParent().GetParent());
            this.Health -= damage;
        }

        [RemoteSync]
        public void SyncOnDeath(int experience, Vector2 position) {
            this.CollisionMask = 0;
            this.CollisionLayer = 0;
            this.Health = 0;
            this.GlobalPosition = position;
            this.ExpValue = experience;
            this.AnimationPlay(EnemyAnimationsEnum.Death);
            if (this is Skeleton) {
                this.GetTree().Root.GetNode<Main>("Main").DefeatedEnemyPoints += 2;
            } else if (this is Slime) {
                this.GetTree().Root.GetNode<Main>("Main").DefeatedEnemyPoints += 1;
            }
        }

        [Puppet]
        protected void SetPuppetPosition(Vector2 globalPosition) {
            if (this.Health <= 0) {
                return;
            }
            this.GlobalPosition = globalPosition;
        }

        private void CreateExperienceOrb(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            drop.Name = $"Exp{this.expOrbCount++}";
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropEnum.ExperienceOrb);
            parent.AddChild(drop, true);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(PuppetCreateExperienceOrb), drop.Name, value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateExperienceOrb(string name, int value, int direction, int distance,
            Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.experienceOrb.Instance<ItemDrop>();
            drop.Name = name;
            drop.Init(globalPosition, value, direction, distance, ItemDropEnum.ExperienceOrb);
            parent.AddChild(drop, true);
        }

        private void CreateGold(int value, Vector2 globalPosition) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            drop.Name = $"Exp{this.goldCount++}";
            int direction = (int)GD.RandRange(0, 360);
            int distance = (int)GD.RandRange(30, 40);
            drop.Init(globalPosition, value, direction, distance, ItemDropEnum.Gold);
            parent.AddChild(drop, true);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(PuppetCreateGold), drop.Name, value, direction, distance, globalPosition);
            }
        }

        [Puppet]
        public void PuppetCreateGold(string name, int value, int direction, int distance, Vector2 globalPosition) {
            Node parent = this.GetTree().Root.GetNode("Main");
            ItemDrop drop = this.gold.Instance<ItemDrop>();
            drop.Name = name;
            drop.Init(globalPosition, value, direction, distance, ItemDropEnum.Gold);
            parent.AddChild(drop, true);
        }

        public void OnTimerTimeout() {
            this.RpcUnreliable(nameof(this.SetPuppetPosition), this.GlobalPosition);
        }

    }

}