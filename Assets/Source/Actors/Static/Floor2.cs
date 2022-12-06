namespace Source.Actors.Static
{
    public class Floor2 : Actor
    {
        public override int DefaultSpriteId => 16;
        public override string DefaultName => "Floor2";

        public override bool Detectable => false;

        protected override int Z => 1;
    }
}