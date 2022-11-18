using UnityEngine;
using UnityEngine.UI;
using Assets.Source.Core;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using System.Linq;
using System;

namespace DungeonCrawl.Actors.Characters
{
    public class Player : Character
    {
        public bool Dog = false;
        public GameObject inventoryDisplay;
        public Inventory inventory = new Inventory();
        public Actor ItemOnGround;
        public bool SecretShowed = false;
        public bool InsideSecretArea = false;
        public Actor companion;
        public bool Map = false;
        public bool Boss1Killed = false;
        public int Crit = 0;
        public int DodgeChance = 0;

        public override void Starter()
        {
            inventoryDisplay = GameObject.Find("Inventory");
            if (inventoryDisplay != null) { inventoryDisplay.SetActive(false); }
            MaxHealth = 100;
            ActualHealth = 100;
            Damage = 1;
            ActualArmor = 0;
            MaxArmor = 0;

            ShowStats();
            UserInterface.Singleton.SetText("Look, there is a sword, let's pick it up!", UserInterface.TextPosition.TopCenter);
        }

        public void ShowStats()
        {
            UserInterface.Singleton._playerStatsText.text = $"{ActualHealth}/{MaxHealth} \n{(companion is DogCompanion ? (Damage * 2) : Damage)} \n{ActualArmor}/{MaxArmor} \n{DodgeChance}% \n{Crit}%";
        }

        protected override void OnUpdate(float deltaTime)
        {
            inventory.selectItem();

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
                inventory.ShowInventory(inventoryDisplay);
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
        }

        public void PickUp()
        {
            UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopRight);

            if (ItemOnGround.OnGround && ItemOnGround != null)
            {
                
                if (ItemOnGround is Sword) { UserInterface.Singleton.SetText("Nice! Now equip it.. Hint: Press I!", UserInterface.TextPosition.TopCenter); }
                inventory.Add(ItemOnGround);
                ActorManager.Singleton.DestroyActor(ItemOnGround);
            }

        }

        public override void Hit(Actor actor)
        {
            if (actor is Character enemy)
            {
                System.Random random = new System.Random();
                int dmg = companion is DogCompanion ? Damage * 2 : Damage;
                dmg = random.Next(100) < Crit ? dmg * 2 : dmg;

                enemy.ApplyDamage(dmg);
            }
        }

        public override bool OnCollision(Actor anotherActor)
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
        public override void Drop() { }

        public void SecretSwitch()
        {
            SecretShowed = !SecretShowed;
            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Hiden")))
            {
                var wall2 = wall.GetComponent<Renderer>();
                Color color = wall2.material.color;
                color.a = SecretShowed ? 1 : 0;
                wall2.material.color = color;
            }

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => !obj.name.Contains("Hiden")))
            {
                var wall2 = wall.GetComponent<Renderer>();

                if (wall2 != null && wall.name != this.name && wall.name != this.companion.name)
                {
                    Color color = wall2.material.color;
                    color.a = SecretShowed ? (float)0.5 : 1;
                    wall2.material.color = color;
                }
            }

        }

    }
}
