using System;
using Source.Actors;
using Source.Actors.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Core
{
    public class Inventory : MonoBehaviour
    {
        private bool _onOff;
        public Items[] InventoryList = new Items[12];
        public readonly Items[] EquippedItems = new Items[7];

        private readonly int[] _defaultSprites = new[]
        {
            32,
            323,
            330,
            79,
            231,
            407,
            38
        };

        public void Clear()
        {
            InventoryList = new Items[12];
            for (var i = 0; i < 12; i++)
            {
                var item = GameObject.Find($"Slot_{i}");
                var count = GameObject.Find($"Counter_{i}");
                if (InventoryList[i] == null) continue;
                item.GetComponent<Image>().sprite = null;
                count.GetComponent<Text>().text = "";
            }

            for (var i = 0; i < _defaultSprites.Length; i++)
            {
                var count = GameObject.Find($"Item Frame_{i}").transform.GetChild(0).GetChild(0).gameObject;
                count.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(_defaultSprites[i]);
                count.GetComponent<Image>().color = new Color32(128, 128, 128, 150);
            }
        }

        public void LoadItem(Items item)
        {
            if (item.Name.Contains("Equipped"))
            {
                UserInterface.Singleton.ShowInventoryDisplay();
                Consume(new Items(item.SpriteId, item.Count, item.Name.Replace("Equipped", "")));
                UserInterface.Singleton.ShowInventoryDisplay(false);
                return;
            }

            var index = Array.IndexOf(InventoryList, null);
            var slot = GameObject.Find($"Slot_{index}");
            var count = GameObject.Find($"Counter_{index}");
            if (slot != null)
            {
                slot.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.SpriteId);
                count.GetComponent<Text>().text = item.Count.ToString();
            }

            InventoryList[index] = item;
        }

        public void Add(Actor item)
        {
            for (var i = 0; i < 12; i++)
            {
                if (InventoryList[i] == null || InventoryList[i].SpriteId != item.DefaultSpriteId) continue;
                InventoryList[i].Count++;
                var tempCount = GameObject.Find($"Counter_{i}");
                if (tempCount != null)
                {
                    tempCount.GetComponent<Text>().text = InventoryList[i].Count.ToString();
                }

                return;
            }

            var index = Array.IndexOf(InventoryList, null);
            var slot = GameObject.Find($"Slot_{index}");
            var count = GameObject.Find($"Counter_{index}");
            if (slot != null)
            {
                slot.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.DefaultSpriteId);
                count.GetComponent<Text>().text = "1";
            }

            InventoryList[index] = new Items(item.DefaultSpriteId, 1, $"Inventory{item.DefaultName}");
        }

        public void ShowInventory(GameObject display, GameObject viewer)
        {
            if (_onOff)
            {
                display.SetActive(false);
                _onOff = false;
                viewer.SetActive(true);
            }
            else
            {
                viewer.SetActive(false);
                display.SetActive(true);
                _onOff = true;
                for (var i = 0; i < 12; i++)
                {
                    var item = GameObject.Find($"Slot_{i}");
                    var count = GameObject.Find($"Counter_{i}");
                    if (InventoryList[i] != null)
                    {
                        if (InventoryList[i].Name == "InventoryItemSword")
                        {
                            UserInterface.Singleton.SetText(
                                "To use an item u will ned to press the corresponding number(1.. 2.. 3..)",
                                UserInterface.TextPosition.TopCenter);
                        }

                        if (InventoryList[i].Name == "InventoryItemKey")
                        {
                            UserInterface.Singleton.SetText(
                                "A key.. I wonder what is that for.. let's try hitting that dor with the face",
                                UserInterface.TextPosition.TopCenter);
                        }

                        item.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(InventoryList[i].SpriteId);
                        count.GetComponent<Text>().text = InventoryList[i].Count.ToString();
                    }
                    else
                    {
                        item.GetComponent<Image>().sprite = null;
                        count.GetComponent<Text>().text = "";
                    }
                }
            }
        }

        public bool Remove(string itemName)
        {
            for (var i = 0; i < 12; i++)
            {
                if (InventoryList[i] == null || InventoryList[i].Name != itemName) continue;
                var count = GameObject.Find($"Counter_{i}");
                InventoryList[i].Count--;

                if (InventoryList[i].Count == 0)
                {
                    InventoryList[i] = null;

                    var slot = GameObject.Find($"Slot_{i}");

                    if (slot == null) return true;
                    slot.GetComponent<Image>().sprite = null;
                    count.GetComponent<Text>().text = "";
                }
                else
                {
                    if (count != null)
                    {
                        count.GetComponent<Text>().text = InventoryList[i].Count.ToString();
                    }
                }

                return true;
            }

            return false;
        }

        public void SelectItem()
        {
            if (!_onOff) return;
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
            for (var i = 0; i < 12; i++)
            {
                if (!Input.GetKeyDown(inv[i])) continue;
                // Move up
                if (InventoryList[i] != null)
                {
                    Consume(InventoryList[i]);
                }

                break;
            }
        }

        private void Consume(Items item)
        {
            var itemName = item.Name;
            var player = ActorManager.Singleton.GetActor<Player>();

            switch (itemName)
            {
                case "InventoryItemHeal":
                    UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                    player.Heal(10);
                    CleanCure();
                    player.ShowStats();
                    Remove(itemName);
                    break;
                case "InventoryItemSword":
                    player.Damage = 2;
                    GameObject.Find("Sword Holder").GetComponent<Image>().sprite =
                        ActorManager.Singleton.GetSprite(item.SpriteId);
                    GameObject.Find("Sword Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    player.ShowStats();
                    EquippedItems[1] = item;
                    Remove(itemName);
                    break;
                case "InventoryItemMeat":
                    player.Heal(player.MaxHealth);
                    CleanCure();
                    player.ShowStats();
                    Remove(itemName);
                    break;
                case "InventoryItemDog":
                    player.ShowStats();
                    GameObject.Find("Companion Holder").GetComponent<Image>().sprite =
                        ActorManager.Singleton.GetSprite(item.SpriteId);
                    GameObject.Find("Companion Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    player.companion = ActorManager.Singleton.Spawn<DogCompanion>(player.Position);
                    EquippedItems[6] = item;
                    player.ShowStats();
                    Remove(itemName);
                    break;
                case "InventoryItemMap":
                    player.map = true;
                    Remove(itemName);
                    break;
                case "InventoryItemArmor":
                    player.ActualArmor = 50;
                    player.MaxArmor = 50;
                    GameObject.Find("Armor Holder").GetComponent<Image>().sprite =
                        ActorManager.Singleton.GetSprite(item.SpriteId);
                    GameObject.Find("Armor Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    player.ShowStats();
                    player.SetSprite(29);
                    player.spriteId = 29;
                    EquippedItems[4] = item;
                    Remove(itemName);
                    break;
                case "InventoryItemRing1":
                    player.Crt = 20;
                    GameObject.Find("Ring Holder").GetComponent<Image>().sprite =
                        ActorManager.Singleton.GetSprite(item.SpriteId);
                    GameObject.Find("Ring Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    player.ShowStats();
                    EquippedItems[3] = item;
                    Remove(itemName);
                    break;
            }
        }

        private static void CleanCure()
        {
            var ghosts = ActorManager.Singleton.GetAllActors<EnemyGhost>();
            foreach (var ghost in ghosts)
            {
                ghost.curse = 0;
            }
        }
    }
}