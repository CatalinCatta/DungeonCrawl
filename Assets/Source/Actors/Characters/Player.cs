using System.Linq;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;

namespace Source.Actors.Characters
{
    public class Player : Character
    {
        public bool dog;
        public Actor itemOnGround;
        public bool secretShowed;
        public bool insideSecretArea;
        public Actor companion;
        public bool map;
        public bool boss1Killed;
        public int dodgeChance;

        public override void Starter()
        {
            MaxHealth = 100;
            ActualHealth = 100;
            Damage = 1;
            ActualArmor = 0;
            MaxArmor = 0;

            ShowStats();
            UserInterface.Singleton.SetText("Look, there is a sword, let's pick it up!",
                UserInterface.TextPosition.TopCenter);
        }

        public void ShowStats()
        {
            UserInterface.Singleton.playerStatsText.text =
                $"{ActualHealth}/{MaxHealth} \n{(companion is DogCompanion ? (Damage * 2) : Damage)} \n{ActualArmor}/{MaxArmor} \n{dodgeChance}% \n{Crt}%";
        }

        protected override void OnUpdate(float deltaTime)
        {
            UserInterface.Singleton.inventor.SelectItem();

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
                UserInterface.Singleton.inventor.ShowInventory(UserInterface.Singleton.inventoryDisplay,
                    UserInterface.Singleton.viewer);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // hit
                HitEnemy();
            }

            if (!Input.GetKeyDown(KeyCode.Return)) return;
            var start = GameObject.Find("Start");
            if (start != null)
            {
                start.SetActive(false);
            }
        }

        private void PickUp()
        {
            UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopRight);

            if (itemOnGround == null || !itemOnGround.OnGround) return;
            if (itemOnGround is ItemSword)
            {
                UserInterface.Singleton.SetText("Nice! Now equip it.. Hint: Press I!",
                    UserInterface.TextPosition.TopCenter);
            }

            UserInterface.Singleton.inventor.Add(itemOnGround);
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

        public override int DefaultSpriteId => spriteId;
        public override string DefaultName => _name;

        private string _name = "Player";
        public int spriteId = 24;

        public void SetName(string newName)
        {
            _name = newName;
        }

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

                if ((wall2 == null || wall.name == this._name) || (this.companion != null &&
                                                                   (this.companion == null ||
                                                                    wall.name == this.companion.name))) continue;
                var color = wall2.color;
                color.a = secretShowed ? (float)0.5 : 1;
                wall2.color = color;
            }
        }
    }
}