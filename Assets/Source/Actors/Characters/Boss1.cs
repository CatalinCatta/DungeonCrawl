using System;
using System.Collections;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;

namespace Source.Actors.Characters
{
    public class Boss1 : Character
    {
        public (int x, int y) GravePosition;

        protected override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            ActorManager.Singleton.DestroyActor(ActorManager.Singleton.GetActorAt((13, -36)));
            UserInterface.Singleton.SetText("", UserInterface.TextPosition.MiddleCenter);
        }

        private IEnumerator Start()
        {
            var timer = new WaitForSeconds(1f);
            while (true)
            {
                if (ActualHealth <= 0)
                {
                    break;
                }

                yield return timer;
                if (actualArmor == 0)
                {
                    ChangePosition();
                }

                yield return timer;
                if (actualArmor == 0)
                {
                    ChangePosition();
                }
                else
                {
                    SummonAcolytes();
                }

                yield return timer;
                if (actualArmor != 0) continue;
                ChangePosition();
                HitEnemy();
            }
        }


        protected override void Hit(Actor actor)
        {
            if (actor is Player player)
            {
                player.ApplyDamage(Damage);
            }
        }


        private static void SummonAcolytes(string drop = null)
        {
            var counter = 0;
            var random = new System.Random();

            while (true)
            {
                var position = (random.Next(14, 31), -random.Next(33, 40));
                var slot = ActorManager.Singleton.GetActorAt(position);

                if (slot != null) continue;
                if (drop != null)
                {
                    switch (counter)
                    {
                        case 0:
                            ActorManager.Singleton.Spawn<Armor>(position);
                            break;
                        case 1:
                            ActorManager.Singleton.Spawn<Meat>(position);
                            break;
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
                    switch (randomEnemy)
                    {
                        case 0:
                            ActorManager.Singleton.Spawn<Skeleton>(position);
                            break;
                        case 1:
                            ActorManager.Singleton.Spawn<Zombie>(position);
                            break;
                        case 2:
                            ActorManager.Singleton.Spawn<Grave>(position);
                            ActorManager.Singleton.Spawn<Ghost>(position);
                            ActorManager.Singleton.GetActorAt<Grave>(position).ghost =
                                ActorManager.Singleton.GetActorAt<Ghost>(position);
                            ActorManager.Singleton.GetActorAt<Ghost>(position).GravePosition = position;
                            break;
                    }

                    if (counter == 1)
                    {
                        break;
                    }
                }

                counter++;
            }
        }

        private void ChangePosition()
        {
            var values = Enum.GetValues(typeof(Direction));
            var random = new System.Random();
            var direction = (Direction)values.GetValue(random.Next(values.Length));
            TryMove(direction);
        }

        public void ShowStats()
        {
            UserInterface.Singleton.SetText($"{ActualHealth}/{MaxHealth}\n{actualArmor}/{maxArmor}",
                UserInterface.TextPosition.MiddleCenter);
        }

        public override void Starter()
        {
            ActorManager.Singleton.GetActor<Player>().boss1Killed = true;
            MaxHealth = 100;
            ActualHealth = 100;
            Damage = 20;
            actualArmor = 50;
            maxArmor = 50;

            ActorManager.Singleton.Spawn<Gate>((13, -36));
            ActorManager.Singleton.Spawn<Gate>((22, -32));

            ShowStats();
        }

        public void TrueForm()
        {
            SetSprite(169);
            SummonAcolytes("items");
            actualArmor = 0;
            ShowStats();
        }

        public override int DefaultSpriteId => 170;
        public override string DefaultName => "Boss1";
    }
}