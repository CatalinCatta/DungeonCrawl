using System.Collections.Generic;
using System.Linq;
using Source.Actors;
using Source.Actors.Characters;
using UnityEngine;
using UnityEngine.U2D;

namespace Source.Core
{
    /// <summary>
    ///     Main class for Actor management - spawning, destroying, detecting at positions, etc
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        /// <summary>
        ///     ActorManager singleton
        /// </summary>
        public static ActorManager Singleton { get; private set; }

        private SpriteAtlas _spriteAtlas;
        private HashSet<Actor> _allActors;

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            Singleton = this;

            _allActors = new HashSet<Actor>();
            _spriteAtlas = Resources.Load<SpriteAtlas>("Spritesheet");
        }

        /// <summary>
        ///     Returns actor present at given position (returns null if no actor is present)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Actor GetActorAt((int x, int y) position)
        {
            return _allActors.FirstOrDefault(actor => actor.Detectable && actor.Position == position);
        }

        /// <summary>
        ///     Returns actor of specific subclass present at given position (returns null if no actor is present)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public T GetActorAt<T>((int x, int y) position) where T : Actor
        {
            return _allActors.FirstOrDefault(actor =>
                actor.Detectable && actor is T && actor.Position == position) as T;
        }


        public T GetActor<T>() where T : Actor
        {
            return _allActors.FirstOrDefault(actor => actor.Detectable && actor is T) as T;
        }

        public IEnumerable<Actor> GetAllActors() => _allActors;

        public IEnumerable<T> GetAllActors<T>()
        {
            //var aa = _allActors.Select(actor => actor.Detectable && actor is T).ToList();
            var bb = _allActors.Where(actor => actor.Detectable && actor is T).ToList();

            var a = new List<T>();

            foreach (var b in bb)
            {
                if (b is T t)
                {
                    a.Add(t);
                }
            }

            return a;
        }

        /// <summary>
        ///     Unregisters given actor (use when killing/destroying)
        /// </summary>
        /// <param name="actor"></param>
        public void DestroyActor(Actor actor)
        {
            _allActors.Remove(actor);
            Destroy(actor.gameObject);
        }

        /// <summary>
        ///     Used for cleaning up the scene before loading a new map
        /// </summary>
        public void DestroyAllActors()
        {
            var actors = _allActors.ToArray();
            UserInterface.Singleton.ShowInventoryDisplay();
            UserInterface.Singleton.inventor.Clear();
            UserInterface.Singleton.ShowInventoryDisplay(false);

            foreach (var actor in actors)
                DestroyActor(actor);
        }

        /// <summary>
        ///     Returns sprite with given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sprite GetSprite(int id)
        {
            return _spriteAtlas.GetSprite($"kenney_transparent_{id}");
        }

        /// <summary>
        ///     Spawns given Actor type at given position
        /// </summary>
        /// <typeparam name="T">Actor type</typeparam>
        /// <param name="position">Position</param>
        /// <param name="actorName">Actor's name (optional)</param>
        /// <returns></returns>
        public T Spawn<T>((int x, int y) position, string actorName = null) where T : Actor
        {
            return Spawn<T>(position.x, position.y, actorName);
        }

        /// <summary>
        ///     Spawns given Actor type at given position
        /// </summary>
        /// <typeparam name="T">Actor type</typeparam>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="actorName">Actor's name (optional)</param>
        /// <returns></returns>
        private T Spawn<T>(int x, int y, string actorName = null) where T : Actor
        {
            var go = new GameObject();
            go.AddComponent<SpriteRenderer>();

            var component = go.AddComponent<T>();

            go.name = actorName ?? component.DefaultName;
            component.Position = (x, y);

            if (typeof(T).IsSubclassOf(typeof(Character)))
            {
                var audioSource = go.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load($"Audio/{component.DefaultName.Replace("Enemy", "")}-hit") as AudioClip;
            }

            _allActors.Add(component);

            return component;
        }
    }
}