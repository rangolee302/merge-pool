using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    void Reset()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void SetScore(int score)
    {
        _text.text = score.ToString();
    }
}
