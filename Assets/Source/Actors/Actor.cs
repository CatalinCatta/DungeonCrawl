using System.Linq;
using Source.Actors.Characters;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;

namespace Source.Actors
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
                if (this is Player)
                {
                    CameraController.Singleton.Position = value;
                }
            }
        }

        private (int x, int y) _position;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            SetSprite(DefaultSpriteId);

            if (this is Character character)
            {
                character.Starter();
            }
        }

        private void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        public void SetSprite(int id)
        {
            _spriteRenderer.sprite = ActorManager.Singleton.GetSprite(id);
        }

        protected virtual void TryMove(Direction direction)
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
                playerTemp.itemOnGround = null;
            }

            if (actorAtTargetPosition == null)
            {
                if (this is Player player)
                {
                    if (player.secretShowed && !player.insideSecretArea)
                    {
                        player.SecretSwitch();
                    }

                    if (targetPosition == (22, -33) && !player.boss1Killed)
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

                    if (!actorAtTargetPosition.OnGround || !(this is Player player2)) return;
                    // Allowed to pick up
                    UserInterface.Singleton.SetText("Press E to pick up", UserInterface.TextPosition.TopRight);
                    player2.itemOnGround = actorAtTargetPosition;
                }

                else if (this.OnCollision(actorAtTargetPosition))
                {
                    Position = targetPosition;
                }

                else if (this is Player player)
                {
                    switch (actorAtTargetPosition)
                    {
                        case Dor dor when player.inventory.Remove("Key"):
                            ActorManager.Singleton.DestroyActor(dor);
                            UserInterface.Singleton.SetText("Over the game u will find hp potions, use them wisely!",
                                UserInterface.TextPosition.TopCenter);
                            Position = targetPosition;
                            break;
                        case HiddenFloor _:
                        {
                            if (!player.secretShowed && !player.insideSecretArea)
                            {
                                player.SecretSwitch();
                            }

                            Position = targetPosition;
                            break;
                        }
                        case HiddenSecret _:
                        {
                            player.insideSecretArea = true;

                            Position = (50, -9);
                            UserInterface.Singleton.SetText("WOW!!", UserInterface.TextPosition.TopCenter);

                            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>())
                            {
                                var wall2 = wall.GetComponent<SpriteRenderer>();
                                if (wall2 == null) continue;
                                var color = wall2.color;
                                color.a = 1;
                                wall2.color = color;
                            }

                            break;
                        }
                        case Dor2 _:
                        {
                            Position = (12, -13);
                            UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                            player.insideSecretArea = false;

                            foreach (var wall in Resources.FindObjectsOfTypeAll<GameObject>()
                                         .Where(obj => !obj.name.Contains("Hidden")))
                            {
                                var wall2 = wall.GetComponent<SpriteRenderer>();
                                if ((wall2 == null || wall.name == this.name) || (player.companion != null &&
                                        (player.companion == null ||
                                         wall.name == player.companion.name)))
                                    continue;
                                var color = wall2.color;
                                color.a = (float)0.5;
                                wall2.color = color;
                            }

                            break;
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
        protected virtual bool OnCollision(Actor anotherActor)
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
        protected virtual int Z => 0;

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