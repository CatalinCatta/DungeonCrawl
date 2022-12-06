using System.Linq;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Actors.Characters
{
    public class Player : Character
    {
        [FormerlySerializedAs("Dog")] public bool dog;
        private static GameObject _inventoryDisplay;
        public Inventory inventory;
        [FormerlySerializedAs("ItemOnGround")] public Actor itemOnGround;
        [FormerlySerializedAs("SecretShowed")] public bool secretShowed;

        [FormerlySerializedAs("InsideSecretArea")]
        public bool insideSecretArea;

        public Actor companion;
        [FormerlySerializedAs("Map")] public bool map;
        [FormerlySerializedAs("Boss1Killed")] public bool boss1Killed;
        [FormerlySerializedAs("DodgeChance")] public int dodgeChance;
        private GameObject _viewer;

        public override void Starter()
        {
            _inventoryDisplay = GameObject.Find("Inventory");
            _viewer = GameObject.Find("Viewer");
            inventory = _inventoryDisplay.GetComponent<Inventory>();

            if (_inventoryDisplay != null)
            {
                _inventoryDisplay.SetActive(false);
            }

            MaxHealth = 100;
            ActualHealth = 100;
            Damage = 1;
            actualArmor = 0;
            maxArmor = 0;

            ShowStats();
            UserInterface.Singleton.SetText("Look, there is a sword, let's pick it up!",
                UserInterface.TextPosition.TopCenter);
        }

        public void ShowStats()
        {
            UserInterface.Singleton.playerStatsText.text =
                $"{ActualHealth}/{MaxHealth} \n{(companion is DogCompanion ? (Damage * 2) : Damage)} \n{actualArmor}/{maxArmor} \n{dodgeChance}% \n{Crt}%";
        }

        protected override void OnUpdate(float deltaTime)
        {
            inventory.SelectItem();

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Move up
                TryMove(Direction.Up);
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Move down
                TryMove(Direction.Down);
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Move left
                TryMove(Direction.Left);
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Move right
                TryMove(Direction.Right);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                // pickUp
                PickUp();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                // inventory
                inventory.ShowInventory(_inventoryDisplay, _viewer);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // hit
                HitEnemy();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                var start = GameObject.Find("Start");
                if (start != null)
                {
                    start.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                // save
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                // load
            }
        }

        private void PickUp()
        {
            UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopRight);

            if (itemOnGround == null || !itemOnGround.OnGround) return;
            if (itemOnGround is Sword)
            {
                UserInterface.Singleton.SetText("Nice! Now equip it.. Hint: Press I!",
                    UserInterface.TextPosition.TopCenter);
            }

            inventory.Add(itemOnGround);
            ActorManager.Singleton.DestroyActor(itemOnGround);
        }

        protected override void Hit(Actor actor)
        {
            if (!(actor is Character enemy)) return;
            var random = new System.Random();

            var crt = random.Next(100) < Crt ? 2 : 1;
            var damage = companion is DogCompanion ? (Damage * crt * 2) : (Damage * crt);

            enemy.ApplyDamage(damage, null, crt.ToString());
        }

        protected override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            ActualHealth = 0;
            ShowStats();
        }

        public override int DefaultSpriteId => 24;
        public override string DefaultName => "Player";

        protected override void Drop()
        {
        }

        public void SecretSwitch()
        {
            secretShowed = !secretShowed;
            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Hidden")))
            {
                var wall2 = wall.GetComponent<SpriteRenderer>();
                var color = wall2.color;
                color.a = secretShowed ? 1 : 0;
                wall2.color = color;
            }

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>()
                         .Where(obj => !obj.name.Contains("Hidden")))
            {
                var wall2 = wall.GetComponent<SpriteRenderer>();

                if ((wall2 == null || wall.name == this.name) || (this.companion != null &&
                                                                  (this.companion == null ||
                                                                   wall.name == this.companion.name))) continue;
                var color = wall2.color;
                color.a = secretShowed ? (float)0.5 : 1;
                wall2.color = color;
            }
        }
    }
}