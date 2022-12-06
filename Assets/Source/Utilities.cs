using System;

namespace Source
{
    public enum Direction : byte
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class Utilities
    {
        public static (int x, int y) ToVector(this Direction dir)
        {
            return dir switch
            {
                Direction.Up => (0, 1),
                Direction.Down => (0, -1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            };
        }
    }
}