using DungeonCrawl.Core;
using System.Collections.Generic;
using UnityEngine;
using Assets.Source.Core;
using DungeonCrawl.Actors.Static;
using System;
using UnityEngine.UI;
using System.Collections;


namespace DungeonCrawl.Actors.Characters
{
    public abstract class Character : Actor
    {
        public int MaxHealth { get; set; }
        public int ActualHealth { get; set; }
        public int Damage { get; set; }
        public int MaxArmor = 0;
        public int ActualArmor = 0;
        public int Crit = 1;

        public void ApplyDamage(int damage, string curse = null, string criticalDmg = null)
        {
            var HitImg = this.HitAnimation(damage, criticalDmg, curse);
            //if (!(this is Player))
            
            var actor = this.gameObject;

            var audio = actor.GetComponent(typeof(AudioSource)) as AudioSource;
            
            if(!audio.isPlaying)
            {
                audio.Play();
            }

            // }

            if (this.ActualArmor > 0 && curse == null)
            {
                this.ActualArmor -= damage / 2;

                if (this is Boss1 b1 && ActualArmor <= 0)
                {
                    b1.TrueForm();
                }
            }
            else
            {
                this.ActualHealth -= damage;
            }

            if (this is Player player)
            {
                player.ShowStats();
            }

            if (this is Boss1 boss1)
            {
                boss1.ShowStats();
            }

            if (ActualHealth <= 0)
            {
                // Die
                Drop();
                OnDeath();

                if (!(this is Player))
                {
                    audio.clip = Resources.Load($"Audio/{this.name}-die") as AudioClip;
                    audio.Play();

                    StartCoroutine(this.DeathSound());

                }
                else
                {
                    ActorManager.Singleton.DestroyActor(this);
                }
            }

            StartCoroutine(waiter_not_that_waiter_just_waiter(HitImg));
        }


        private GameObject HitAnimation(int dmg, string criticalHit, string Curse)
        {
            var NewObj1 = new GameObject();
            NewObj1.transform.position = new Vector3(this.Position.x, this.Position.y, -5);
            NewObj1.transform.parent = this.gameObject.transform;

            var NewImage = NewObj1.AddComponent<SpriteRenderer>();
            NewImage.sprite = criticalHit == "2" ? ActorManager.Singleton.GetSprite(553) : Curse == "Curse" ? ActorManager.Singleton.GetSprite(609) : ActorManager.Singleton.GetSprite(551);
            NewImage.color = criticalHit == "2" ? Color.red : Curse == "Curse" ? Color.black : Color.grey;


            var NewObj2 = new GameObject();
            NewObj2.transform.position = new Vector3(this.Position.x, this.Position.y, -6);
            NewObj2.transform.parent = NewObj1.gameObject.transform;

            var Damage = NewObj2.AddComponent<TextMesh>();
            Damage.text = dmg.ToString();
            Damage.fontSize = 8;

            return NewObj1;
        }


        IEnumerator waiter_not_that_waiter_just_waiter(GameObject NewObj)
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(NewObj);
        }

        IEnumerator DeathSound()
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
        public override int Z => -2;

        public abstract void Starter();

        public void HitEnemy()
        {
            List<Actor> actors = new List<Actor>
            {
                ActorManager.Singleton.GetActorAt((Position.x, Position.y + 1)),
                ActorManager.Singleton.GetActorAt((Position.x, Position.y - 1)),
                ActorManager.Singleton.GetActorAt((Position.x - 1, Position.y)),
                ActorManager.Singleton.GetActorAt((Position.x + 1, Position.y))
            };

            foreach(Actor actor in actors)
            {
                Hit(actor);
            }
        }

        public abstract void Hit(Actor actor);

        public virtual void Drop()
        {
            var rand = new System.Random();
            var dropRate = rand.Next(100);
            if (dropRate < 20) { ActorManager.Singleton.Spawn<Heal>(Position); }
            if (dropRate == 20) { ActorManager.Singleton.Spawn<Meat>(Position); }
        }
    }
}
