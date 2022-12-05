namespace DungeonCrawl.Actors.Static
{
    public class Dog : Actor
    {
        public override int DefaultSpriteId => 366;
        public override string DefaultName => "Dog";

        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}