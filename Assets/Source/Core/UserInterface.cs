using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Source.Core.UserInterface;

namespace Assets.Source.Core
{
    /// <summary>
    ///     Class for handling text on user interface (UI)
    /// </summary>
    public class UserInterface : MonoBehaviour
    {
        public TextMeshProUGUI _playerStatsText;
        public TextMeshProUGUI _bossStatsText;
        public TextMeshProUGUI _messageBoxText;
        public TextMeshProUGUI _hintBoxText;

        public GameObject _playerStatsObject;
        public GameObject _bossStatsObject;
        public GameObject _messageBoxObject;
        public GameObject _hintBoxObject;

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


            _playerStatsText = GameObject.Find("Status Text").GetComponent<TMPro.TextMeshProUGUI>();
            _bossStatsText = GameObject.Find("Boss Status Text").GetComponent<TMPro.TextMeshProUGUI>();
            _messageBoxText = GameObject.Find("Message Text").GetComponent<TMPro.TextMeshProUGUI>();
            _hintBoxText = GameObject.Find("Hint Text").GetComponent<TMPro.TextMeshProUGUI>();

            _playerStatsObject = GameObject.Find("Status Frame");
            _bossStatsObject = GameObject.Find("Boss Status Frame");
            _messageBoxObject = GameObject.Find("Message Frame");
            _hintBoxObject = GameObject.Find("Hint Frame");

            _bossStatsObject.SetActive(false);
            _messageBoxObject.SetActive(false);
            _hintBoxObject.SetActive(false);

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
                        _hintBoxObject.SetActive(false);
                    }
                    else
                    {
                        _hintBoxObject.SetActive(true);
                        _hintBoxText.text = text;
                    }
                    break;

                case TextPosition.TopCenter:
                    if (text == "")
                    {
                        _messageBoxObject.SetActive(false);
                    }
                    else
                    {
                        _messageBoxObject.SetActive(true);
                        _messageBoxText.text = text;
                    }
                    break;

                case TextPosition.MiddleCenter:
                    if (text == "")
                    {
                        _bossStatsObject.SetActive(false);
                    }
                    else
                    {
                        _messageBoxObject.SetActive(false);
                        _bossStatsObject.SetActive(true);
                        _bossStatsText.text = text;
                    }
                    break;

                default:
                    _textComponents[(int)textPosition].text = text;
                    break;
            }
        }

    }
}
