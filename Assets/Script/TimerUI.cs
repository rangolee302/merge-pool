using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace MergeBalls
{
    [RequireComponent(typeof(Image))]
    public class TimerUI : MonoBehaviour
    {
        Image _image;
        Timer _timer;
        [SerializeField] TMP_Text _text;
        Color _startColor = Color.white;
        Color _endColor = new Color(64, 64, 64);

        void OnEnable()
        {
            _image = GetComponent<Image>();
        }

        public void SetTimer(Timer timer)
        {
            _timer = timer;
        }

        void Update()
        {
            if (_timer != null)
            {
                _image.fillAmount = _timer.time / _timer.limit;
            }
            if (_text != null)
            {
                _text.text = Mathf.RoundToInt(_timer.time).ToString();
                _text.color = Color.Lerp(_startColor, _endColor, _timer.time / _timer.limit);
            }
        }

    }

}
