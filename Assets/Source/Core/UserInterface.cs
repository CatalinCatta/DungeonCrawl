using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Core
{
    /// <summary>
    ///     Class for handling text on user interface (UI)
    /// </summary>
    public class UserInterface : MonoBehaviour
    {
        public TextMeshProUGUI playerStatsText;
        public TextMeshProUGUI bossStatsText;
        public TextMeshProUGUI messageBoxText;
        public TextMeshProUGUI hintBoxText;
        public GameObject playerStatsObject;
        public GameObject bossStatsObject;
        public GameObject messageBoxObject;
        public GameObject hintBoxObject;
        public GameObject inventoryDisplay;
        public Inventory inventor;
        public GameObject viewer;
        public GameObject saveLoad;

        public enum TextPosition : byte
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        /// <summary>
        ///     User Interface singleton
        /// </summary>
        public static UserInterface Singleton { get; private set; }

        private TextMeshProUGUI[] _textComponents;

        public void ShowInventoryDisplay(bool show = true)
        {
            inventoryDisplay.SetActive(show);
        }

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            inventoryDisplay = GameObject.Find("Inventory");
            inventor = inventoryDisplay.GetComponent<Inventory>();
            viewer = GameObject.Find("Viewer");
            if (inventoryDisplay != null)
            {
                inventoryDisplay.SetActive(false);
            }

            playerStatsText = GameObject.Find("Status Text").GetComponent<TextMeshProUGUI>();
            bossStatsText = GameObject.Find("Boss Status Text").GetComponent<TextMeshProUGUI>();
            messageBoxText = GameObject.Find("Message Text").GetComponent<TextMeshProUGUI>();
            hintBoxText = GameObject.Find("Hint Text").GetComponent<TextMeshProUGUI>();

            saveLoad = GameObject.Find("Save/Load Frame");
            playerStatsObject = GameObject.Find("Status Frame");
            bossStatsObject = GameObject.Find("Boss Status Frame");
            messageBoxObject = GameObject.Find("Message Frame");
            hintBoxObject = GameObject.Find("Hint Frame");

            bossStatsObject.SetActive(false);
            messageBoxObject.SetActive(false);
            hintBoxObject.SetActive(false);
            saveLoad.SetActive(false);

            Singleton = this;

            _textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        }

        public void Update()
        {
            OnUpdate();
        }

        private IEnumerator SaveAnimation(string iconName)
        {
            saveLoad.SetActive(true);
            var opusIconName = iconName == "Save" ? "Load" : "Save";
            var opusIcon = GameObject.Find($"{opusIconName} Sign");
            opusIcon.SetActive(false);
            var icon = GameObject.Find($"{iconName} Sign");
            GameObject.Find("Save/Load Text").GetComponent<TextMeshProUGUI>().text =
                iconName == "Save" ? "Game saved" : "Game loaded";
            var initialSpriteName = icon.GetComponent<Image>().sprite.name.Replace("(Clone)", "");
            var timer = 0;
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                timer++;
                if (timer == 5)
                {
                    icon.GetComponent<Image>().sprite =
                        ActorManager.Singleton.GetSprite(
                            int.Parse(initialSpriteName.Substring(initialSpriteName.Length - 3)));
                }

                if (timer == 10)
                {
                    break;
                }

                var spriteName = icon.GetComponent<Image>().sprite.name.Replace("(Clone)", "");
                Debug.Log(spriteName.Substring(spriteName.Length - 3));
                icon.GetComponent<Image>().sprite =
                    ActorManager.Singleton.GetSprite(int.Parse(spriteName.Substring(spriteName.Length - 3)) + 1);
            }

            icon.GetComponent<Image>().sprite =
                ActorManager.Singleton.GetSprite(int.Parse(initialSpriteName.Substring(initialSpriteName.Length - 3)));
            opusIcon.SetActive(true);
            saveLoad.SetActive(false);
        }

        private void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                StartCoroutine(SaveAnimation("Load"));
                SavingObject.Load();
            }

            if (!Input.GetKeyDown(KeyCode.F5)) return;
            StartCoroutine(SaveAnimation("Save"));
            SavingObject.Save(Singleton.inventor);
        }

        /// <summary>
        ///     Changes text at given screen position
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textPosition"></param>
        public void SetText(string text, TextPosition textPosition)
        {
            switch (textPosition)
            {
                case TextPosition.TopRight:
                    if (text == "")
                    {
                        hintBoxObject.SetActive(false);
                    }
                    else
                    {
                        hintBoxObject.SetActive(true);
                        hintBoxText.text = text;
                    }

                    break;

                case TextPosition.TopCenter:
                    if (text == "")
                    {
                        messageBoxObject.SetActive(false);
                    }
                    else
                    {
                        messageBoxObject.SetActive(true);
                        messageBoxText.text = text;
                    }

                    break;

                case TextPosition.MiddleCenter:
                    if (text == "")
                    {
                        bossStatsObject.SetActive(false);
                    }
                    else
                    {
                        messageBoxObject.SetActive(false);
                        bossStatsObject.SetActive(true);
                        bossStatsText.text = text;
                    }

                    break;

                case TextPosition.TopLeft:
                case TextPosition.MiddleLeft:
                case TextPosition.MiddleRight:
                case TextPosition.BottomLeft:
                case TextPosition.BottomCenter:
                case TextPosition.BottomRight:
                default:
                    _textComponents[(int)textPosition].text = text;
                    break;
            }
        }
    }
}