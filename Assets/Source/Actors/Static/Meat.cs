namespace DungeonCrawl.Actors.Static
{
    public class Meat : Actor
    {
        public override int DefaultSpriteId => 800;
        public override string DefaultName => "Meat";

        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
        public override int Z => -1;
    }
}
