using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour

{
    [SerializeField] private TMP_Text _text;

    private float _time = 0;

    private void Update()
    {
        _time += Time.deltaTime;
        _text.text = GetTimeText();
    }

    public string GetTimeText()
    {
        int secs = (int)_time % 60;
        int mins = (int)_time / 60;
        string secsString = secs >= 10 ? secs + "" : "0" + secs;
        string minsString = mins >= 10 ? mins + "" : "0" + mins;
        return minsString + ":" + secsString;
    }
}