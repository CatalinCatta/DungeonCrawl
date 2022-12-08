namespace Source.Actors.Static
{
    public class ItemDog : Actor
    {
        public override int DefaultSpriteId => 366;
        public override string DefaultName => "ItemDog";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}