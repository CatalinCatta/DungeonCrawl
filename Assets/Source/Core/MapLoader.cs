using System;
using System.Linq;
using System.Text.RegularExpressions;
using Materials;
using Source.Actors.Characters;
using Source.Actors.Static;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Source.Core
{
    /// <summary>
    ///     MapLoader is used for constructing maps from txt files
    /// </summary>
    public static class MapLoader
    {
        /// <summary>
        ///     Constructs map from txt file and spawns actors at appropriate positions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loaded"></param>
        public static void LoadMap(int id, string loaded = null)
        {
            var lines = Regex.Split(Resources.Load<TextAsset>($"map_{id}").text, "\r\n|\r|\n");

            // Read map size from the first line
            var split = lines[0].Split(' ');
            var width = int.Parse(split[0]);
            var height = int.Parse(split[1]);

            (int x, int y) playerCoord = (width / 2, -height / 2);

            // Create actors
            for (var y = 0; y < height; y++)
            {
                var line = lines[y + 1];
                for (var x = 0; x < width; x++)
                {
                    var character = line[x];
                    SpawnActor(character, (x, -y), loaded);

                    if (character == 'P')
                    {
                        playerCoord = (x, -y);
                    }
                }
            }

            // Set default camera size and position
            CameraController.Singleton.Size = 5;
            CameraController.Singleton.Position = playerCoord;
            SetUp();
        }

        private static void SpawnActor(char c, (int x, int y) position, string loaded)
        {
            switch (c)
            {
                case '#':
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '.':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'K':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemKey>(position);
                    }

                    break;
                case 'P':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<Player>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'S':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<EnemySkeleton>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case '}':
                    ActorManager.Singleton.Spawn<StairsDown>(position);
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'G':
                    if (loaded == null)
                    {
                        var grave = ActorManager.Singleton.Spawn<EnemyGrave>(position);
                        var ghost = ActorManager.Singleton.Spawn<EnemyGhost>(position);
                        grave.enemyGhost = ghost;
                        ghost.GravePosition = position;
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'Z':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<EnemyZombie>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'H':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemHeal>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'D':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemDor>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case '^':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemSword>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case '+':
                    ActorManager.Singleton.Spawn<HiddenFloor>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '`':
                    ActorManager.Singleton.Spawn<HiddenFloor>(position);
                    break;
                case '-':
                    ActorManager.Singleton.Spawn<HiddenWall1>(position);
                    break;
                case '=':
                    ActorManager.Singleton.Spawn<HiddenWall1>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '$':
                    ActorManager.Singleton.Spawn<HiddenWall1>(position);
                    ActorManager.Singleton.Spawn<HiddenFloor>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '!':
                    ActorManager.Singleton.Spawn<HiddenWall1>(position);
                    ActorManager.Singleton.Spawn<HiddenFloor>(position);
                    break;
                case '|':
                    ActorManager.Singleton.Spawn<HiddenWall2>(position);
                    break;
                case '*':
                    ActorManager.Singleton.Spawn<HiddenWall2>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '?':
                    ActorManager.Singleton.Spawn<HiddenSecret>(position);
                    break;
                case '/':
                    ActorManager.Singleton.Spawn<LeftRoof>(position);
                    break;
                case '\\':
                    ActorManager.Singleton.Spawn<RightRoof>(position);
                    break;
                case '_':
                    ActorManager.Singleton.Spawn<CenterRoof>(position);
                    break;
                case '%':
                    ActorManager.Singleton.Spawn<Floor2>(position);
                    break;
                case 'C':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemDog>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor2>(position);
                    break;
                case 'B':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemMeat>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor2>(position);
                    break;
                case 'M':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemMap>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor2>(position);
                    break;
                case 'L':
                    ActorManager.Singleton.Spawn<Wall2>(position);
                    break;
                case 'R':
                    ActorManager.Singleton.Spawn<Dor2>(position);
                    break;
                case 'r':
                    if (loaded == null)
                    {
                        ActorManager.Singleton.Spawn<ItemRing1>(position);
                    }

                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'T':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    ActorManager.Singleton.Spawn<Torch>(position);
                    break;
                case 's':
                    ActorManager.Singleton.Spawn<SpiderWeb>(position);
                    break;
                case 'W':
                    ActorManager.Singleton.Spawn<Window>(position);
                    break;
                case ' ':
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetUp()
        {
            try
            {
                var playerLight = GameObject.Find("Player").AddComponent(typeof(Light2D)) as Light2D;
                playerLight.pointLightOuterRadius = 5;
                playerLight.shadowsEnabled = true;
            }
            catch (NullReferenceException)
            {
                Debug.Log("Player not found or lights already on");
            }

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Wall")))
            {
                var wallShade = wall.AddComponent(typeof(ShadowCaster2D)) as ShadowCaster2D;

                if (wallShade != null) wallShade.selfShadows = true;
            }

            foreach (var torch in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Torch")))
            {
                var _ = torch.AddComponent(typeof(LightFlickerEffect)) as LightFlickerEffect;
                var torchLight = torch.AddComponent(typeof(Light2D)) as Light2D;
                if (torchLight == null) continue;
                torchLight.pointLightOuterRadius = 5;
                torchLight.pointLightInnerRadius = 3;
                torchLight.shadowsEnabled = true;
                torchLight.shadowIntensity = 0.9f;
                torchLight.color = Color.yellow;
            }

            foreach (var floor in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "HiddenFloor"))
            {
                floor.GetComponent<SpriteRenderer>().color = Color.green;
            }

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Hidden")))
            {
                var wall2 = wall.GetComponent<SpriteRenderer>();
                var color = wall2.color;
                color.a = 0;
                wall2.color = color;
            }
        }
    }
}