using Godot;

namespace VampireSurvivorsLike {

    public class Gun : Weapon {

        private int NumberOfBullets { get; set; }
        private PackedScene bullet;
        private int piercing = 1;
        private float bulletSpeed = 400;
        private uint bulletsShot = 0;
        private Player player;

        public Gun() {
            this.Level = 0;
            this.MaxLevel = 8;
            this.Icon = ResourceLoader.Load("res://MyPixelArts/images/GunIcon.png") as Texture;
            this.AttackSpeed = 100;
            this.Damage = 5;
            this.NumberOfBullets = 1;
            this.bullet = (PackedScene)ResourceLoader.Load("res://Items/Weapons/Gun/Bullet.tscn");
            this.player = this.GetParent().GetParent<Player>();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (this.Counter++ > this.AttackSpeed) {
                this.Shoot();
                this.Counter = 0;
            }
        }

        private void Shoot() {
            for (int i = 0; i < this.NumberOfBullets; i++) {
                string name = this.GetParent().Name + this.bulletsShot++;
                Vector2 direction = this.player.Direction
                    .Rotated((i * Mathf.Pi / 12) - (Mathf.Pi / 12) * (this.NumberOfBullets - 1) / 2.0f).Normalized();
                Vector2 position = this.player.GlobalPosition + new Vector2(0, -10) + this.player.Direction * 10;
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    this.Rpc(nameof(this.SyncShoot), direction, position, name);
                } else {
                    this.SyncShoot(direction, position, name);
                }
            }
        }

        [PuppetSync]
        public void SyncShoot(Vector2 direction, Vector2 position, string name) {
            Bullet bulletInst = this.bullet.Instance<Bullet>();
            bulletInst.SetNetworkMaster(this.GetParent().GetNetworkMaster());
            bulletInst.Init(direction, this.bulletSpeed, this.Damage, this.piercing, name, position);
            this.AddChild(bulletInst, true);
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