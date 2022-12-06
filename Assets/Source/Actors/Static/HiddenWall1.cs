namespace Source.Actors.Static
{
    public class HiddenWall1 : Actor
    {
        public override int DefaultSpriteId => 529;
        public override string DefaultName => "HiddenWall1";

        protected override int Z => -1;
    }
}