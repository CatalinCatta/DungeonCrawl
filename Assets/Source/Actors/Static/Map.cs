namespace DungeonCrawl.Actors.Static
{
    public class Map : Actor
    {
        public override int DefaultSpriteId => 751;
        public override string DefaultName => "Map";

        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}