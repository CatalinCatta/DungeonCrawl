namespace DungeonCrawl.Actors.Static
{
    public class Sword : Actor
    {
        public override int DefaultSpriteId => 371;
        public override string DefaultName => "Sword";

        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }


    }
}
