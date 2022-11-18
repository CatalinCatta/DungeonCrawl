using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using Assets.Source.Core;
using System;
using UnityEngine.EventSystems;

namespace DungeonCrawl.Actors.Characters
{
    public class Boss1 : Character
    {
        public (int x, int y) GravePosition;

        public override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            ActorManager.Singleton.DestroyActor(ActorManager.Singleton.GetActorAt((13, -36)));

            ActorManager.Singleton.GetActor<Player>().Boss1Killed = true;
        }

        IEnumerator Start()
        {
            WaitForSeconds timer = new WaitForSeconds(1f);
            while (true)
            {
                yield return timer;
                if (ActualArmor == 0) { ChangePosition(); }
                
                yield return timer;
                if (ActualArmor == 0)
                {
                    ChangePosition();
                }
                else
                {
                    SumonAcolites();
                }

                yield return timer;
                if (ActualArmor == 0) { ChangePosition();  HitEnemy(); }
            }
        }


        public override void Hit(Actor actor)
        {
            if (actor is Player player)
            {
                player.ApplyDamage(Damage);
            }
        }


        public void SumonAcolites(string drop = null)
        {
            int counter = 0;
            System.Random random = new System.Random();

            while (true)
            {
                (int, int) position = (random.Next(14, 31), -random.Next(33, 40));
                var slot = ActorManager.Singleton.GetActorAt(position);

                if (slot == null)
                {

                    if (drop != null)
                    {
                        if (counter == 0)
                        {
                            ActorManager.Singleton.Spawn<Armor>(position);
                        }
                        if (counter == 1)
                        {
                            ActorManager.Singleton.Spawn<Meat>(position);
                        }
                        if (counter == 2)
                        {
                            ActorManager.Singleton.Spawn<Meat>(position);
                            break;
                        }
                    }
                    else
                    {
                        var randomEnemy = random.Next(3);
                        if (randomEnemy == 0)
                        {
                            ActorManager.Singleton.Spawn<Skeleton>(position);
                        }
                        if (randomEnemy == 1)
                        {
                            ActorManager.Singleton.Spawn<Zombie>(position);
                        }
                        if (randomEnemy == 2)
                        {
                            ActorManager.Singleton.Spawn<Grave>(position);
                            ActorManager.Singleton.Spawn<Ghost>(position);
                            ActorManager.Singleton.GetActorAt<Grave>(position).ghost = ActorManager.Singleton.GetActorAt<Ghost>(position);
                            ActorManager.Singleton.GetActorAt<Ghost>(position).GravePosition = position;
                        }
                        if (counter == 1) { break; }
                    }
                    counter ++;
                }
            }

        }

        public void ChangePosition()
        {
            Array values = Enum.GetValues(typeof(Direction));
            System.Random random = new System.Random();
            var direction = (Direction)values.GetValue(random.Next(values.Length));
            TryMove(direction);

        }

        public void ShowStats()
        {
            UserInterface.Singleton.SetText("HP: " + ActualHealth + " / " + MaxHealth + "\nARMOR: " + ActualArmor + " / " + MaxArmor, UserInterface.TextPosition.TopCenter);
        }

        public override void Starter()
        {
            MaxHealth = 100;
            ActualHealth = 100;
            Damage = 20;
            ActualArmor = 50;
            MaxArmor = 50;

            ActorManager.Singleton.Spawn<Wall>((13, -36));
            ActorManager.Singleton.Spawn<Wall>((22, -32));
            
            ShowStats();
        }

        public void TrueForm()
        {
            SetSprite(169);
            SumonAcolites("items");
            ActualArmor = 0;
            ShowStats();
        }

        public override int DefaultSpriteId => 170;
        public override string DefaultName => "Boss1";
    }
}