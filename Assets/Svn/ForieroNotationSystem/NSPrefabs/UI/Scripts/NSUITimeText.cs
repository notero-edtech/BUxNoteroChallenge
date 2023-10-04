/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Text;
using ForieroEngine.Music.NotationSystem;
using TMPro;
using UnityEngine;

public class NSUITimeText : MonoBehaviour
{
    public enum TimeEnum { StringBuilder, TimeSpan }
    public TimeEnum timeEnum = TimeEnum.TimeSpan;
    public TextMeshProUGUI tmp;

    private readonly char[] _numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private readonly StringBuilder _sb = new StringBuilder("99:99.9", 7);
    private int _f = 0;
    private int _fLast = -1;
    private int _ss = 0;
    private int _mm = 0;
    
    private void Update()
    {
        _f = Mathf.FloorToInt(Mathf.Repeat(NSPlayback.Time.time * 10, 10));
        if (_f == _fLast) return;
        
        switch (timeEnum)
        {
            case TimeEnum.StringBuilder:
                _sb[6] = _numbers[_f];
                
                _ss =  Mathf.FloorToInt(Mathf.Repeat(NSPlayback.Time.time, 60));
                _sb[4] = _numbers[_ss % 10];
                _sb[3] = _numbers[_ss / 10];
                
                _mm = Mathf.FloorToInt(NSPlayback.Time.time / 60);
                _sb[1] = _numbers[_mm % 10];
                _sb[0] = _numbers[_mm / 10];
                
                tmp.SetText(_sb);
                break;
            case TimeEnum.TimeSpan:
                tmp.text = TimeSpan.FromSeconds(NSPlayback.Time.time).ToString(@"mm\:ss\:f");
                break;
        }
        
        _fLast = _f;
    }
}
