using System;
using Source.Actors;
using Source.Actors.Characters;
using Source.Core;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private bool _onOff;
    private readonly Items[] _inventoryList = new Items[12];

    public void Add(Actor item)
    {
        for (var i = 0; i < 12; i++)
        {
            if (_inventoryList[i] == null || _inventoryList[i].SpriteId != item.DefaultSpriteId) continue;
            _inventoryList[i].Count++;
            var tempCount = GameObject.Find($"Counter_{i}");
            if (tempCount != null)
            {
                tempCount.GetComponent<Text>().text = _inventoryList[i].Count.ToString();
            }

            return;
        }

        var index = Array.IndexOf(_inventoryList, null);
        var slot = GameObject.Find($"Slot_{index}");
        var count = GameObject.Find($"Counter_{index}");
        if (slot != null)
        {
            slot.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(item.DefaultSpriteId);
            count.GetComponent<Text>().text = "1";
        }

        _inventoryList[index] = new Items(item.DefaultSpriteId, 1, item.DefaultName);
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
                if (_inventoryList[i] != null)
                {
                    if (_inventoryList[i].Name == "Sword")
                    {
                        UserInterface.Singleton.SetText(
                            "To use an item u will ned to press the corresponding number(1.. 2.. 3..)",
                            UserInterface.TextPosition.TopCenter);
                    }

                    if (_inventoryList[i].Name == "Key")
                    {
                        UserInterface.Singleton.SetText(
                            "A key.. I wonder what is that for.. let's try hitting that dor with the face",
                            UserInterface.TextPosition.TopCenter);
                    }

                    item.GetComponent<Image>().sprite = ActorManager.Singleton.GetSprite(_inventoryList[i].SpriteId);
                    count.GetComponent<Text>().text = _inventoryList[i].Count.ToString();
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
            if (_inventoryList[i] == null || _inventoryList[i].Name != itemName) continue;
            var count = GameObject.Find($"Counter_{i}");
            _inventoryList[i].Count--;

            if (_inventoryList[i].Count == 0)
            {
                _inventoryList[i] = null;

                var slot = GameObject.Find($"Slot_{i}");

                if (slot == null) return true;
                slot.GetComponent<Image>().sprite = null;
                count.GetComponent<Text>().text = "";
            }
            else
            {
                if (count != null)
                {
                    count.GetComponent<Text>().text = _inventoryList[i].Count.ToString();
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
            if (_inventoryList[i] != null)
            {
                Consume(_inventoryList[i]);
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
            case "Heal":
                UserInterface.Singleton.SetText("", UserInterface.TextPosition.TopCenter);
                player.Heal(10);
                player.ShowStats();
                Remove(itemName);
                break;
            case "Sword":
                player.Damage = 2;
                Remove(itemName);
                GameObject.Find("Sword Holder").GetComponent<Image>().sprite =
                    ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Sword Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
                break;
            case "Meat":
                player.Heal(player.MaxHealth);
                player.ShowStats();
                Remove(itemName);
                break;
            case "Dog":
                player.ShowStats();
                Remove(itemName);
                GameObject.Find("Companion Holder").GetComponent<Image>().sprite =
                    ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Companion Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.companion = ActorManager.Singleton.Spawn<DogCompanion>(player.Position);
                break;
            case "Map":
                Remove(itemName);
                player.map = true;
                break;
            case "Armor":
                player.actualArmor = 50;
                player.maxArmor = 50;
                Remove(itemName);
                GameObject.Find("Armor Holder").GetComponent<Image>().sprite =
                    ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Armor Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
                player.SetSprite(29);
                break;
            case "Ring1":
                player.Crt = 20;
                Remove(itemName);
                GameObject.Find("Ring Holder").GetComponent<Image>().sprite =
                    ActorManager.Singleton.GetSprite(item.SpriteId);
                GameObject.Find("Ring Holder").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                player.ShowStats();
                break;
        }
    }
}