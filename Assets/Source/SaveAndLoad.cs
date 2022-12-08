using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Source.Actors;
using Source.Actors.Characters;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Source
{
    public static class SavingObject
    {
        public static void Save(Inventory inventory)
        {
            var json = new StringBuilder();
            var player = ActorManager.Singleton.GetActor<Player>();
            var serializedPlayer = new SerializedPlayer()
            {
                name = player.DefaultName,
                x = player.Position.x,
                y = player.Position.y,
                actualHealth = player.ActualHealth,
                actualArmor = player.ActualArmor,
                spriteId = player.DefaultSpriteId,
                secretShowed = player.secretShowed,
                insideSecretArea = player.insideSecretArea,
                boss1Killed = player.boss1Killed
            };
            json.Append(JsonUtility.ToJson(serializedPlayer));

            foreach (var item in inventory.InventoryList.Where(obj => obj != null))
            {
                var serializedInventoryItems = new SerializedInventoryItems()
                {
                    spriteId = item.SpriteId,
                    count = item.Count,
                    name = item.Name
                };
                json.Append(JsonUtility.ToJson(serializedInventoryItems));
            }

            foreach (var item in inventory.EquippedItems.Where(obj => obj != null))
            {
                var serializedInventoryItems = new SerializedInventoryItems()
                {
                    spriteId = item.SpriteId,
                    count = item.Count,
                    name = "Equipped" + item.Name
                };
                json.Append(JsonUtility.ToJson(serializedInventoryItems));
            }

            foreach (var actor in ActorManager.Singleton.GetAllActors()
                         .Where(obj =>
                             (obj.DefaultName.Contains("Item") && !obj.DefaultName.Contains("Inventory")) ||
                             (obj.DefaultName.Contains("Enemy") && !obj.DefaultName.Contains("Ghost"))))
            {
                var saveObject = new SerializedCharacter()
                {
                    name = actor.DefaultName,
                    x = actor.Position.x,
                    y = actor.Position.y,
                    spriteId = actor.DefaultSpriteId
                };
                if (actor.DefaultName.Contains("Enemy") && actor is Character enemy)
                {
                    saveObject.actualHealth = enemy.ActualHealth;
                    saveObject.actualArmor = enemy.ActualArmor;
                }

                json.Append(JsonUtility.ToJson(saveObject));
            }

            File.WriteAllText(Path.GetFullPath(@"Save/Save1.json"), json.ToString());
        }

        public static void Load()
        {
            ActorManager.Singleton.DestroyAllActors();
            MapLoader.LoadMap(1, "empty");
            using var r = new StreamReader(Path.GetFullPath(@"Save/Save1.json"));
            var json = r.ReadToEnd();
            var text = Regex.Split(json, "}");
            foreach (var obj in text)
            {
                if (obj == string.Empty)
                {
                    break;
                }

                if ((obj.Contains("Item") && !obj.Contains("Inventory")) || obj.Contains("Enemy"))
                {
                    var item = JsonUtility.FromJson<SerializedCharacter>(obj + "}");
                    Actor actor = item.name switch
                    {
                        "EnemyBoss1" => ActorManager.Singleton.Spawn<EnemyBoss1>((item.x, item.y)),
                        "EnemyGrave" => SpawnGrave(item.x, item.y),
                        "EnemySkeleton" => ActorManager.Singleton.Spawn<EnemySkeleton>((item.x, item.y)),
                        "EnemyZombie" => ActorManager.Singleton.Spawn<EnemyZombie>((item.x, item.y)),
                        "ItemArmor" => ActorManager.Singleton.Spawn<ItemArmor>((item.x, item.y)),
                        "ItemDog" => ActorManager.Singleton.Spawn<ItemDog>((item.x, item.y)),
                        "ItemDor" => ActorManager.Singleton.Spawn<ItemDor>((item.x, item.y)),
                        "ItemHeal" => ActorManager.Singleton.Spawn<ItemHeal>((item.x, item.y)),
                        "ItemKey" => ActorManager.Singleton.Spawn<ItemKey>((item.x, item.y)),
                        "ItemMap" => ActorManager.Singleton.Spawn<ItemMap>((item.x, item.y)),
                        "ItemMeat" => ActorManager.Singleton.Spawn<ItemMeat>((item.x, item.y)),
                        "ItemRing1" => ActorManager.Singleton.Spawn<ItemRing1>((item.x, item.y)),
                        "ItemSword" => ActorManager.Singleton.Spawn<ItemSword>((item.x, item.y)),
                        _ => null
                    };

                    if (actor == null || !(actor is Character enemy)) continue;
                    enemy.ActualHealth = item.actualHealth;
                    enemy.ActualArmor = item.actualArmor;
                    enemy.SetSprite(item.spriteId);
                    if (enemy is EnemyBoss1 boss1)
                    {
                        boss1.ShowStats();
                    }
                }
                else if (obj.Contains("Inventory"))
                {
                    var item = JsonUtility.FromJson<SerializedInventoryItems>(obj + "}");
                    UserInterface.Singleton.inventor.LoadItem(new Items(item.spriteId, item.count, item.name));
                }
                else
                {
                    var item = JsonUtility.FromJson<SerializedPlayer>(obj + "}");
                    var player = ActorManager.Singleton.Spawn<Player>((item.x, item.y));

                    player.SetName(item.name);
                    player.ActualHealth = item.actualHealth;
                    player.ActualArmor = item.actualArmor;
                    player.SetSprite(item.spriteId);
                    player.secretShowed = item.secretShowed;
                    player.insideSecretArea = item.insideSecretArea;
                    player.boss1Killed = item.boss1Killed;
                    var playerLight = player.gameObject.AddComponent(typeof(Light2D)) as Light2D;
                    if (playerLight == null) continue;
                    playerLight.pointLightOuterRadius = 5;
                    playerLight.shadowsEnabled = true;
                    player.ShowStats();
                }
            }
        }

        private static EnemyGrave SpawnGrave(int x, int y)
        {
            var grave = ActorManager.Singleton.Spawn<EnemyGrave>((x, y));
            var ghost = ActorManager.Singleton.Spawn<EnemyGhost>((x, y));
            grave.enemyGhost = ghost;
            ghost.GravePosition = (x, y);
            return grave;
        }

        [Serializable]
        public class SerializedActor
        {
            public string name;
            public int spriteId;
        }

        [Serializable]
        public class SerializedCharacter : SerializedActor
        {
            public int actualHealth;
            public int actualArmor;
            public int x;
            public int y;
        }

        [Serializable]
        public class SerializedPlayer : SerializedCharacter
        {
            public bool secretShowed;
            public bool insideSecretArea;
            public bool boss1Killed;
        }


        [Serializable]
        public class SerializedInventoryItems : SerializedActor
        {
            public int count;
        }
    }
}