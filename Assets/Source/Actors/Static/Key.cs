namespace DungeonCrawl.Actors.Static
{
    public class Key : Actor
    {

        public override int DefaultSpriteId => 559;
        public override string DefaultName => "Key";
        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        public override int Z => -1;
            
    }
}
