using UnityEngine;
using System.Collections;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using Assets.Source.Core;
using System;

namespace DungeonCrawl.Actors.Characters
{
    public class Grave : Character
    {
        public Ghost ghost;

        public override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            if (ghost != null && ghost.ActualHealth > 0) { ghost.ApplyDamage(Damage); }
        }

        public override void Drop()
        {

        }

        public override void Hit(Actor actor)
        {

        }

        public override void Starter()
        {
            MaxHealth = 20;
            ActualHealth = 20;
            Damage = 999;
        }

        public override int DefaultSpriteId => 672;
        public override string DefaultName => "Grave";
    }
}