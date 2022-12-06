namespace Source.Core
{
    public class Items
    {
        public readonly int SpriteId;
        public int Count;
        public readonly string Name;

        public Items(int sprite, int count, string name)
        {
            SpriteId = sprite;
            Count = count;
            Name = name;
        }
    }
}