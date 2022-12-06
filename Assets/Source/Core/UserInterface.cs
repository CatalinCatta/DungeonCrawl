using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Core
{
    /// <summary>
    ///     Class for handling text on user interface (UI)
    /// </summary>
    public class UserInterface : MonoBehaviour
    {
        [FormerlySerializedAs("_playerStatsText")]
        public TextMeshProUGUI playerStatsText;

        [FormerlySerializedAs("_bossStatsText")]
        public TextMeshProUGUI bossStatsText;

        [FormerlySerializedAs("_messageBoxText")]
        public TextMeshProUGUI messageBoxText;

        [FormerlySerializedAs("_hintBoxText")] public TextMeshProUGUI hintBoxText;

        [FormerlySerializedAs("_playerStatsObject")]
        public GameObject playerStatsObject;

        [FormerlySerializedAs("_bossStatsObject")]
        public GameObject bossStatsObject;

        [FormerlySerializedAs("_messageBoxObject")]
        public GameObject messageBoxObject;

        [FormerlySerializedAs("_hintBoxObject")]
        public GameObject hintBoxObject;

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

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }


            playerStatsText = GameObject.Find("Status Text").GetComponent<TextMeshProUGUI>();
            bossStatsText = GameObject.Find("Boss Status Text").GetComponent<TextMeshProUGUI>();
            messageBoxText = GameObject.Find("Message Text").GetComponent<TextMeshProUGUI>();
            hintBoxText = GameObject.Find("Hint Text").GetComponent<TextMeshProUGUI>();

            playerStatsObject = GameObject.Find("Status Frame");
            bossStatsObject = GameObject.Find("Boss Status Frame");
            messageBoxObject = GameObject.Find("Message Frame");
            hintBoxObject = GameObject.Find("Hint Frame");

            bossStatsObject.SetActive(false);
            messageBoxObject.SetActive(false);
            hintBoxObject.SetActive(false);

            Singleton = this;

            _textComponents = GetComponentsInChildren<TextMeshProUGUI>();
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

                default:
                    _textComponents[(int)textPosition].text = text;
                    break;
            }
        }
    }
}