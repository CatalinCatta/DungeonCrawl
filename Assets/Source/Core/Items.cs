using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public int SpriteId;
    public int Count;
    public string Name;

    public Items(int sprite, int count, string name)
    {
        SpriteId = sprite;
        Count = count;
        Name = name;
    }
}
