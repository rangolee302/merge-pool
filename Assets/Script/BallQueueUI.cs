using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class BallQueueUI : MonoBehaviour
{
    [SerializeField] RectTransform _panel;
    [SerializeField] Image _ballUIPrefab;
    [SerializeField] Sprite[] _levelSprite;
    [SerializeField] TMP_Text _ballNumberText;
    Queue<Image> _uiList;

    void Reset()
    {
        _panel = GetComponent<RectTransform>();
    }

    void Start()
    {
        _uiList = new Queue<Image>();
    }

    void CreateBall(Ball ball)
    {
        Image ballImage = Instantiate(_ballUIPrefab, _panel);
        if (ball.level > 0 && (ball.level - 1) < _levelSprite.Length)
        {
            ballImage.color = ball.color;
            ballImage.transform.GetChild(0).GetComponent<Image>().sprite = _levelSprite[ball.level - 1];
        }

        RectTransform rect = ballImage.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(-_uiList.Count * rect.sizeDelta.x, 0);
        _uiList.Enqueue(ballImage);
        UpdateNumber(_uiList.Count);
    }

    public void RemoveBall()
    {
        Image ball = _uiList.Dequeue();
        UpdateNumber(_uiList.Count);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            ball.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.3f)
        );
        foreach (var item in _uiList)
        {
            RectTransform rect = item.GetComponent<RectTransform>();
            sequence.Join(
                rect.DOAnchorPosX(rect.anchoredPosition.x + 80, 0.3f)
            );
        }
        Destroy(ball.gameObject);
    }

    void UpdateNumber(int number)
    {
        if (_ballNumberText == null)
        {
            return;
        }
        _ballNumberText.text = number.ToString();
    }

    public void UpdateQueue(Queue<Ball> queue)
    {
        foreach (var item in queue)
        {
            CreateBall(item);
        }
    }
}
