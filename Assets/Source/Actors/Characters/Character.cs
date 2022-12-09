using System.Collections;
using System.Collections.Generic;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Source.Actors.Characters
{
    public abstract class Character : Actor
    {
        public int MaxHealth { get; protected set; }
        public int ActualHealth { get; set; }
        public int Damage { get; set; }
        public int MaxArmor { get; set; }
        public int ActualArmor { get; set; }
        public int Crt { get; set; }

        public void ApplyDamage(int hit, string curse = null, string criticalDmg = null)
        {
            var hitDmg = HitAnimation(hit, criticalDmg, curse);
            //if (!(this is Player))

            var actor = gameObject;

            var audioSource = actor.GetComponent(typeof(AudioSource)) as AudioSource;

            if (audioSource != null)
            {
                audioSource.Play();
            }

            // }

            if (ActualArmor > 0 && curse == null)
            {
                ActualArmor -= hit / 2;

                if (this is EnemyBoss1 b1 && ActualArmor <= 0)
                {
                    b1.TrueForm();
                }
            }
            else
            {
                ActualHealth -= hit;
            }

            if (this is Player player)
            {
                player.ShowStats();
            }

            if (this is EnemyBoss1 boss1)
            {
                boss1.ShowStats();
            }

            if (ActualHealth <= 0)
            {
                // Die
                Drop();
                OnDeath();

                if (!(this is Player) && audioSource != null)
                {
                    audioSource.clip = Resources.Load($"Audio/{DefaultName.Replace("Enemy", "")}-die") as AudioClip;
                    audioSource.Play();

                    StartCoroutine(DeathSound());
                }
                else
                {
                    ActorManager.Singleton.DestroyActor(this);
                }
            }

            StartCoroutine(waiter_not_that_waiter_just_waiter(hitDmg));
        }


        private GameObject HitAnimation(int dmg, string criticalHit, string curse)
        {
            var newObj1 = new GameObject
            {
                transform =
                {
                    position = new Vector3(this.Position.x, this.Position.y, -5),
                    parent = this.gameObject.transform
                }
            };

            var newImage = newObj1.AddComponent<SpriteRenderer>();
            newImage.sprite = criticalHit == "2" ? ActorManager.Singleton.GetSprite(553) :
                curse == "Curse" ? ActorManager.Singleton.GetSprite(609) : ActorManager.Singleton.GetSprite(551);
            newImage.color = criticalHit == "2" ? Color.red : curse == "Curse" ? Color.black : Color.grey;


            var newObj2 = new GameObject
            {
                transform =
                {
                    position = new Vector3(this.Position.x, this.Position.y, -6),
                    parent = newObj1.gameObject.transform
                }
            };

            var hit = newObj2.AddComponent<TextMesh>();
            hit.text = dmg.ToString();
            hit.fontSize = 8;

            return newObj1;
        }


        private static IEnumerator waiter_not_that_waiter_just_waiter(Object newObj)
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(newObj);
        }

        private IEnumerator DeathSound()
        {
            this.Position = (1000, 1000);
            yield return new WaitForSeconds(3f);
            ActorManager.Singleton.DestroyActor(this);
        }


        public void Heal(int hp)
        {
            ActualHealth += hp;
            if (ActualHealth > MaxHealth)
            {
                ActualHealth = MaxHealth;
            }
        }

        protected abstract void OnDeath();

        /// <summary>
        ///     All characters are drawn "above" floor etc
        /// </summary>
        protected override int Z => -2;

        public abstract void Starter();

        protected void HitEnemy()
        {
            var actors = new List<Actor>
            {
                ActorManager.Singleton.GetActorAt((Position.x, Position.y + 1)),
                ActorManager.Singleton.GetActorAt((Position.x, Position.y - 1)),
                ActorManager.Singleton.GetActorAt((Position.x - 1, Position.y)),
                ActorManager.Singleton.GetActorAt((Position.x + 1, Position.y))
            };

            foreach (var actor in actors)
            {
                Hit(actor);
            }
        }

        protected abstract void Hit(Actor actor);

        protected virtual void Drop()
        {
            var rand = new System.Random();
            var dropRate = rand.Next(100);
            if (dropRate < 20)
            {
                ActorManager.Singleton.Spawn<ItemHeal>(Position);
            }

            if (dropRate == 20)
            {
                ActorManager.Singleton.Spawn<ItemMeat>(Position);
            }
        }
    }
}