using Godot;

namespace VampireSurvivorsLike {

    public class Gun : Weapon {

        private int NumberOfBullets { get; set; }
        private PackedScene bullet;
        private int piercing = 1;
        private float bulletSpeed = 200;
        private uint bulletsShot = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.SetIcon();
            this.Level = 0;
            this.MaxLevel = 8;
            this.Counter = 0;
            this.AttackSpeed = 100;
            this.Damage = 5;
            this.NumberOfBullets = 1;
            this.bullet = (PackedScene)ResourceLoader.Load("res://Items/Weapons/Gun/Bullet.tscn");
            this.player = this.GetParent().GetParent<Player>();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (!GameStateManagerSingleton.Instance.IsMultiplayer || this.IsNetworkMaster()) {
                this.Counter++;
                if (this.Counter % this.AttackSpeed == 0) {
                    this.Shoot();
                }
            }
        }

        private void Shoot() {
            for (int i = 0; i < this.NumberOfBullets; i++) {
                Bullet bulletInst = (Bullet)this.bullet.Instance();
                bulletInst.SetNetworkMaster(this.GetParent().GetNetworkMaster());
                bulletInst.Speed = this.bulletSpeed;
                bulletInst.Name = this.GetParent().Name + this.bulletsShot++;
                bulletInst.Piercing = this.piercing;
                bulletInst.Damage = this.Damage;
                bulletInst.Direction = ((Vector2)this.player.Get("Direction"))
                    .Rotated((i * Mathf.Pi / 12) - (Mathf.Pi / 12) * (this.NumberOfBullets - 1) / 2.0f).Normalized();
                bulletInst.GlobalPosition =
                    this.player.GlobalPosition + ((Vector2)this.player.Get("Direction")).Normalized() * 10;
                bulletInst.Visible = true;
                AddChild(bulletInst, true);
                bulletInst.SetAsToplevel(true);
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    Rpc(nameof(Gun.PuppetShoot), bulletInst.Direction, bulletInst.GlobalPosition, bulletInst.Name);
                }
            }
        }

        [Puppet]
        public void PuppetShoot(Vector2 direction, Vector2 position, string name) {
            Bullet bulletInst = (Bullet)this.bullet.Instance();
            bulletInst.SetNetworkMaster(this.GetParent().GetNetworkMaster());
            bulletInst.Speed = this.bulletSpeed;
            bulletInst.Modulate = new Color(0, 0, 1f);
            bulletInst.Name = name;
            bulletInst.Piercing = this.piercing;
            bulletInst.Damage = this.Damage;
            bulletInst.Direction = direction;
            bulletInst.GlobalPosition = position;
            bulletInst.Visible = true;
            AddChild(bulletInst, true);
            bulletInst.SetAsToplevel(true);
        }

        public override void Upgrade() {
            this.Level++;
            switch (this.Level) {
                case 1: break;
                case 2:
                    this.AttackSpeed -= 20;
                    break;
                case 3:
                    this.NumberOfBullets++;
                    break;
                case 4:
                    this.piercing++;
                    break;
                case 5:
                    this.bulletSpeed += 500;
                    break;
                case 6:
                    this.NumberOfBullets++;
                    break;
                case 7:
                    this.piercing++;
                    break;
                case 8:
                    this.AttackSpeed -= 10;
                    break;
            }
        }

        public override string UpgradeMessage() {
            switch (this.Level) {
                case 0: return "Gun: a gun that at higher levels can shoot multiple bullets piercing multiple enemies";
                case 1: return "Gun: Increase Attack Speed Of Gun";
                case 2: return "Gun: Increase Number Of Bullets Of Gun By 1";
                case 3: return "Gun: Increase Piercing Of Bullets By 1";
                case 4: return "Gun: Increase Bullet Speed";
                case 5: return "Gun: Increase Number Of Bullets By 1";
                case 6: return "Gun: Increase Piercing Of Bullets By 1";
                case 7: return "Gun: Increase Attack Speed And Number Of Bullets";
                default: return "Gun: No more upgrades.";
            }
        }

        public override void SetIcon() {
            this.Icon = ResourceLoader.Load("res://MyPixelArts/images/GunIcon.png") as Texture;
        }

        public override string ToString() {
            switch (this.Level) {
                case 0: return "Gun: a gun that at higher levels can shoot multiple bullets piercing multiple enemies";
                case 1: return "Gun: Increase Attack Speed Of Gun";
                case 2: return "Gun: Increase Number Of Bullets Of Gun By 1";
                case 3: return "Gun: Increase Piercing Of Bullets By 1";
                case 4: return "Gun: Increase Bullet Speed";
                case 5: return "Gun: Increase Number Of Bullets By 1";
                case 6: return "Gun: Increase Piercing Of Bullets By 1";
                case 7: return "Gun: Increase Attack Speed And Number Of Bullets";
                default: return "Gun: No more upgrades.";
            }
        }

    }

}