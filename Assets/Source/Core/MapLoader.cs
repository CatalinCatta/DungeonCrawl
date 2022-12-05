using DungeonCrawl.Actors.Characters;
using DungeonCrawl.Actors.Static;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.Rendering.Universal;

namespace DungeonCrawl.Core
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
        public static void LoadMap(int id)
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
                    SpawnActor(character, (x, -y));

                    if (character == 'P') { playerCoord = (x, -y); }
                }
            }
            // Set default camera size and position
            CameraController.Singleton.Size = 5;
            CameraController.Singleton.Position = playerCoord;
            SetUp();

        }

        private static void SpawnActor(char c, (int x, int y) position)
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
                    ActorManager.Singleton.Spawn<Key>(position);
                    break;
                case 'P':
                    ActorManager.Singleton.Spawn<Player>(position);
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'S':
                    ActorManager.Singleton.Spawn<Skeleton>(position);
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case '}':
                    ActorManager.Singleton.Spawn<StairsDown>(position);
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'G':
                    ActorManager.Singleton.Spawn<Grave>(position);
                    ActorManager.Singleton.Spawn<Ghost>(position);
                    ActorManager.Singleton.GetActorAt<Grave>(position).ghost = ActorManager.Singleton.GetActorAt<Ghost>(position);
                    ActorManager.Singleton.GetActorAt<Ghost>(position).GravePosition = position;
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'Z':
                    ActorManager.Singleton.Spawn<Zombie>(position);
                    ActorManager.Singleton.Spawn<Floor>(position);
                    break;
                case 'H':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    ActorManager.Singleton.Spawn<Heal>(position);
                    break;
                case 'D':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    ActorManager.Singleton.Spawn<Dor>(position);
                    break;
                case '^':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    ActorManager.Singleton.Spawn<Sword>(position);
                    break;
                case '+':
                    ActorManager.Singleton.Spawn<HidenFloor>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '`':
                    ActorManager.Singleton.Spawn<HidenFloor>(position);
                    break;
                case '-':
                    ActorManager.Singleton.Spawn<HidenWall1>(position);
                    break;
                case '=':
                    ActorManager.Singleton.Spawn<HidenWall1>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '$':
                    ActorManager.Singleton.Spawn<HidenWall1>(position);
                    ActorManager.Singleton.Spawn<HidenFloor>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '!':
                    ActorManager.Singleton.Spawn<HidenWall1>(position);
                    ActorManager.Singleton.Spawn<HidenFloor>(position);
                    break;
                case '|':
                    ActorManager.Singleton.Spawn<HidenWall2>(position);
                    break;
                case '*':
                    ActorManager.Singleton.Spawn<HidenWall2>(position);
                    ActorManager.Singleton.Spawn<Wall>(position);
                    break;
                case '?':
                    ActorManager.Singleton.Spawn<HidenSecret>(position);
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
                    ActorManager.Singleton.Spawn<Floor2>(position);
                    ActorManager.Singleton.Spawn<Dog>(position);
                    break;
                case 'B':
                    ActorManager.Singleton.Spawn<Floor2>(position);
                    ActorManager.Singleton.Spawn<Meat>(position);
                    break;
                case 'M':
                    ActorManager.Singleton.Spawn<Floor2>(position);
                    ActorManager.Singleton.Spawn<Map>(position);
                    break;
                case 'L':
                    ActorManager.Singleton.Spawn<Wall2>(position);
                    break;
                case 'R':
                    ActorManager.Singleton.Spawn<Dor2>(position);
                    break;
                case 'r':
                    ActorManager.Singleton.Spawn<Floor>(position);
                    ActorManager.Singleton.Spawn<Ring1>(position);
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

            var playerLight = GameObject.Find("Player").AddComponent(typeof(Light2D)) as Light2D;
            playerLight.pointLightOuterRadius = 5;
            playerLight.shadowsEnabled = true;

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Wall")))
            {
                var wallShade = wall.AddComponent(typeof(ShadowCaster2D)) as ShadowCaster2D;

                wallShade.selfShadows = true;
            }

            foreach (var torch in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Torch")))
            {
                var _ = torch.AddComponent(typeof(LightFlickerEffect)) as LightFlickerEffect;
                var torchLight = torch.AddComponent(typeof(Light2D)) as Light2D;
                torchLight.pointLightOuterRadius = 5;
                torchLight.pointLightInnerRadius = 3;
                torchLight.shadowsEnabled = true;
                torchLight.shadowIntensity = 0.9f;
                torchLight.color = Color.yellow;
            }

            foreach (var floor in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "HidenFloor"))
            {
                floor.GetComponent<SpriteRenderer>().color = Color.green;
            }

            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Contains("Hiden")))
            {
                var wall2 = wall.GetComponent<SpriteRenderer>();
                Color color = wall2.color;
                color.a = 0;
                wall2.color = color;

            }

        }
    }
}
