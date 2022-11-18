using UnityEngine;
using DungeonCrawl.Actors;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DungeonCrawl.Actors.Characters;
using Assets.Source.Core;
using DungeonCrawl.Actors.Static;   

namespace DungeonCrawl.Core
{
    public class Inventory : MonoBehaviour
    {
        private bool onOff = false;
        private Items[] inventoryList = new Items[12];

        public void Add(Actor item)
        {
            for (int i = 0; i < 12; i++)
            {
                if (inventoryList[i] != null && inventoryList[i].SpriteId == item.DefaultSpriteId)
                {
                    inventoryList[i].Count++;
                    var temp_count = GameObject.Find($"Counter_{i}");
                    if (temp_count != null) { temp_count.GetComponent<Text>().text = inventoryList[i].Count.ToString(); }
                    return;
                }
                
            }
            
            int index = Array.IndexOf(inventoryList, null);
            var slot = GameObject.Find($"Slot_{index}");
            var count = GameObject.Find($"Counter_{index}");
            if (slot != null)
            {
                slot.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.DefaultSpriteId);
                count.GetComponent<Text>().text = "1";
            }
            inventoryList[index] = new Items(item.DefaultSpriteId, 1, item.DefaultName);

        }

        public void ShowInventory(GameObject display)
        {
            if (onOff)
            {
                display.SetActive(false);
                onOff = false;
            }
            else
            {
                display.SetActive(true);
                onOff = true;
                for (int i = 0; i < 12; i++)
                {
                    var item = GameObject.Find($"Slot_{i}");
                    var count = GameObject.Find($"Counter_{i}");
                    if (inventoryList[i] != null)
                    {
                        if (inventoryList[i].Name == "Sword")
                        {
                            UserInterface.Singleton.SetText("To use an item u will ned to press the corensponding number(1.. 2.. 3..)", UserInterface.TextPosition.TopCenter);
                        }
                        if (inventoryList[i].Name == "Key")
                        {
                            UserInterface.Singleton.SetText("A key.. I wonder what is that for.. let's try hiting that dor with the face", UserInterface.TextPosition.TopCenter);
                        }
                        item.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(inventoryList[i].SpriteId);
                        count.GetComponent<Text>().text = inventoryList[i].Count.ToString();
                    }
                    else
                    {
                        item.GetComponent<Image>().sprite = null;
                        count.GetComponent<Text>().text = "";
                    }
                }
            }
        }

        public bool Remove(string name)
        {
            for (int i = 0; i < 12; i++)
            {
                if (inventoryList[i] != null && inventoryList[i].Name == name)
                {
                    var count = GameObject.Find($"Counter_{i}");
                    inventoryList[i].Count--;

                    if (inventoryList[i].Count == 0)
                    {
                        inventoryList[i] = null;

                        var slot = GameObject.Find($"Slot_{i}");

                        if (slot != null) 
                        { 
                            slot.GetComponent<Image>().sprite = null;
                            count.GetComponent<Text>().text = "";
                        }
                    }
                    else
                    {
                        if (count != null) { count.GetComponent<Text>().text = inventoryList[i].Count.ToString(); }
                    }
                    return true;

                }
            }
            return false;
        }
        
        public void selectItem()
        {
            if (onOff)
            {
                KeyCode[] inv =
                {
                    KeyCode.Alpha1,
                    KeyCode.Alpha2,
                    KeyCode.Alpha3,
                    KeyCode.Alpha4,
                    KeyCode.Alpha5,
                    KeyCode.Alpha6,
                    KeyCode.Alpha7,
                    KeyCode.Alpha8,
                    KeyCode.Alpha9,
                    KeyCode.Alpha0,
                    KeyCode.Minus,
                    KeyCode.Equals,
                };
                for (int i = 0; i < 12; i++)
                {

                    if (Input.GetKeyDown(inv[i]))
                    {
                        // Move up
                        if (inventoryList[i] != null) { Consume(inventoryList[i]); }
                        break;
                    }
                }
            }
        }

        private void Consume(Items item)
        {
            string name = item.Name;
            Player player = ActorManager.Singleton.GetActor<Player>();
            if (name == "Heal")
            {
                UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                player.Heal(10);
                player.ShowStats();
                Remove(name);
            }

            if (name == "Sword")
            {
                player.Damage = 2;
                Remove(name);
                GameObject.Find("Sword Holder").GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Sword Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
            }

            if (name == "Meat")
            {
                player.Heal(player.MaxHealth);
                player.ShowStats();
                Remove(name);
            }

            if (name == "Dog")
            {
                player.ShowStats();
                Remove(name);
                GameObject.Find("Companion Holder").GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Companion Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.companion = ActorManager.Singleton.Spawn<DogCompanion>(player.Position);
            }

            if (name == "Map")
            {
                Remove(name);
                player.Map = true;
            }
            

            if (name == "Armor")
            {
                player.ActualArmor = 50;
                player.MaxArmor = 50;
                Remove(name);
                GameObject.Find("Armor Holder").GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Armor Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
                player.SetSprite(29);
            }

            if (name == "Ring1")
            {
                player.Crit = 20;
                Remove(name);
                GameObject.Find("Ring Holder").GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Ring Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
            }

        }
    }
}
