using DungeonCrawl.Core;
using UnityEngine;
using Assets.Source.Core;
using DungeonCrawl.Actors.Characters;
using DungeonCrawl.Actors.Static;
using System.Linq;

namespace DungeonCrawl.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        public (int x, int y) Position
        {
            get => _position;
            set
            {
                _position = value;
                transform.position = new Vector3(value.x, value.y, Z);
                if (this is Player) { CameraController.Singleton.Position = value; }
            }
        }

        private (int x, int y) _position;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            SetSprite(DefaultSpriteId);

            if (this is Character character) { character.Starter(); }
        }

        private void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        public void SetSprite(int id)
        {
            _spriteRenderer.sprite = ActorManager.Singleton.GetSprite(id);
        }

        public virtual void TryMove(Direction direction)
        {
            var vector = direction.ToVector();
            (int x, int y) targetPosition = (Position.x + vector.x, Position.y + vector.y);

            var actorAtTargetPosition = ActorManager.Singleton.GetActorAt(targetPosition);

            if (this is Player playerTemp)
            {
                if (playerTemp.companion)
                {
                    playerTemp.companion.Position = Position;
                }
                UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopRight);
            }

            if (actorAtTargetPosition == null)
            {
                if (this is Player player)
                {
                    if (player.SecretShowed && !player.InsideSecretArea) {  player.SecretSwitch(); }

                    if (targetPosition == (22, -33) && !player.Boss1Killed)
                    {
                        ActorManager.Singleton.Spawn<Boss1>((22, -36));
                    }
                    // No obstacle found, just move
                }
                Position = targetPosition;
            }
            else
            {
                if (actorAtTargetPosition.OnCollision(this))
                {
                    // Allowed to move
                    Position = targetPosition; 
                    
                    if (actorAtTargetPosition.OnGround && this is Player player2)
                    {
                        // Allowed to pick up
                        UserInterface.Singleton.SetText("Press E to pick up", UserInterface.TextPosition.TopRight);
                        player2.ItemOnGround = actorAtTargetPosition;
                    }

                }

                else if (this.OnCollision(actorAtTargetPosition))
                {
                    Position = targetPosition;
                }

                else if (this is Player player)
                {
                    if (actorAtTargetPosition is Dor dor && player.inventory.Remove("Key"))
                    {
                        ActorManager.Singleton.DestroyActor(dor);
                        UserInterface.Singleton.SetText("Over the game u will find hp potions, use them wisely!", UserInterface.TextPosition.TopCenter);
                        Position = targetPosition;
                    }
                    if (actorAtTargetPosition is HidenFloor)
                    {
                        if (!player.SecretShowed && !player.InsideSecretArea) { player.SecretSwitch(); }
                        Position = targetPosition;
                    }
                    if (actorAtTargetPosition is HidenSecret)
                    {
                        player.InsideSecretArea = true;

                        Position = (50, -9);
                        UserInterface.Singleton.SetText("WOW!!", UserInterface.TextPosition.TopCenter);
                        
                        foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>())
                        {
                            var wall2 = wall.GetComponent<Renderer>();
                            Color color = wall2.material.color;
                            color.a = 1;
                            wall2.material.color = color;
                        }


                    }
                    if (actorAtTargetPosition is Dor2)
                    {
                        Position = (12, -13);
                        UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                        player.InsideSecretArea = false;
                        
                        foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => !obj.name.Contains("Hiden")))
                        {
                            var wall2 = wall.GetComponent<Renderer>();
                            if (wall2 != null && wall.name != this.name && wall.name != player.companion.name)
                            {
                                Color color = wall2.material.color;
                                color.a = (float)0.5;
                                wall2.material.color = color;
                            }
                        }

                    }

                }
            }
        }

        /// <summary>
        ///     Invoked whenever another actor attempts to walk on the same position
        ///     this actor is placed.
        /// </summary>
        /// <param name="anotherActor"></param>
        /// <returns>true if actor can walk on this position, false if not</returns>
        public virtual bool OnCollision(Actor anotherActor)
        {
            // All actors are passable by default
            return false;
        }

        /// <summary>
        ///     Invoked every animation frame, can be used for movement, character logic, etc
        /// </summary>
        /// <param name="deltaTime">Time (in seconds) since the last animation frame</param>
        protected virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        ///     Can this actor be detected with ActorManager.GetActorAt()? Should be false for purely cosmetic actors
        /// </summary>
        public virtual bool Detectable => true;

        public virtual bool OnGround => false;

        /// <summary>
        ///     Z position of this Actor (0 by default)
        /// </summary>
        public virtual int Z => 0;

        /// <summary>
        ///     Id of the default sprite of this actor type
        /// </summary>
        public abstract int DefaultSpriteId { get; }

        /// <summary>
        ///     Default name assigned to this actor type
        /// </summary>
        public abstract string DefaultName { get; }
    }
}