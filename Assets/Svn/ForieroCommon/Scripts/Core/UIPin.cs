using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerPrefs = ForieroEngine.PlayerPrefs;

public class UIPin : MonoBehaviour
{
    public enum State
    {
        Pin,
        New,
        Again
    }

    public bool newPin = false;
    public Scope pinScope = Scope.Player;
    public Toggle[] toggles;
    
    State state = State.Pin;
    Scope scope = Scope.Player;     

    List<int> pin = new List<int>();
    List<int> pinAgain = new List<int>();
    List<int> pinStored = new List<int>();

    public Action OnPinCorrect;
    public Action OnPinWrong;

    public Action OnPinNew;
    public Action OnPinAgainCorrect;
    public Action OnPinAgainWrong;

    public enum Scope
    {
        Global,
        Player
    }

    private void Awake()
    {
        Init(pinScope, newPin);
    }

    public void OnDeleteClick()
    {
        switch (state)
        {
            case State.Pin:
                if (pin.Count > 0) pin.RemoveAt(pin.Count - 1);
                SetRadios(pin.Count);
                break;
            case State.New:
                if (pin.Count > 0) pin.RemoveAt(pin.Count - 1);
                SetRadios(pin.Count);
                break;
            case State.Again:
                if (pinAgain.Count > 0) pinAgain.RemoveAt(pinAgain.Count - 1);
                SetRadios(pinAgain.Count);
                break;
        }
    }

    public void OnCloseClick()
    {

    }

    public void OnNumber(int number)
    {
        switch (state)
        {
            case State.Pin:
                pin.Add(number);
                SetRadios(pin.Count);
                if (pin.Count == pinStored.Count)
                {
                    if (Matches(pin, pinStored))
                    {
                        OnPinCorrect?.Invoke();
                        Debug.Log("PIN CORRECT");
                    }
                    else
                    {
                        OnPinWrong?.Invoke();
                        Debug.Log("PIN WRONG");
                        pin.Clear();
                        SetRadios(pin.Count);
                    }
                }
                break;
            case State.New:
                pin.Add(number);
                SetRadios(pin.Count);
                if (pin.Count == pinStored.Count)
                {
                    OnPinNew?.Invoke();
                    state = State.Again;
                    SetRadios(pinAgain.Count);
                }
                break;
            case State.Again:
                pinAgain.Add(number);
                SetRadios(pinAgain.Count);
                if (pin.Count == pinAgain.Count)
                {
                    if (Matches(pin, pinAgain))
                    {
                        OnPinAgainCorrect?.Invoke();
                        Debug.Log("PIN CORRECT");
                        SavePin();
                    }
                    else
                    {
                        OnPinAgainWrong?.Invoke();
                        Debug.Log("PIN WRONG");
                        pin.Clear(); pinAgain.Clear();
                        state = State.New;
                        SetRadios(pin.Count);
                    }
                }
                break;
        }
    }

    public void Init(Scope scope, bool newPin)
    {
        pin = new List<int>();
        pinAgain = new List<int>();
        this.scope = scope;
        LoadPin();

        state = newPin ? State.New : State.Pin;
    }

    void SavePin()
    {
        switch (scope)
        {
            case Scope.Global:
                PlayerPrefs.SetBool("PIN", true);
                PlayerPrefs.SetInt("PIN1", pin[0]);
                PlayerPrefs.SetInt("PIN2", pin[1]);
                PlayerPrefs.SetInt("PIN3", pin[2]);
                PlayerPrefs.SetInt("PIN4", pin[3]);
                break;
            case Scope.Player:
                PlayerManager.player.SetBool("PIN", true);
                PlayerManager.player.SetInt("PIN1", pin[0]);
                PlayerManager.player.SetInt("PIN2", pin[1]);
                PlayerManager.player.SetInt("PIN3", pin[2]);
                PlayerManager.player.SetInt("PIN4", pin[3]);
                break;
        }
    }

    void LoadPin()
    {
        pinStored = new List<int>();
        switch (scope)
        {
            case Scope.Global:
                pinStored.Add(PlayerPrefs.GetInt("PIN1", 0));
                pinStored.Add(PlayerPrefs.GetInt("PIN2", 0));
                pinStored.Add(PlayerPrefs.GetInt("PIN3", 0));
                pinStored.Add(PlayerPrefs.GetInt("PIN4", 0));
                break;
            case Scope.Player:
                pinStored.Add(PlayerManager.player.GetInt("PIN1", 0));
                pinStored.Add(PlayerManager.player.GetInt("PIN2", 0));
                pinStored.Add(PlayerManager.player.GetInt("PIN3", 0));
                pinStored.Add(PlayerManager.player.GetInt("PIN4", 0));
                break;
        }
    }

    void SetRadios(int count)
    {
        for(int i = 0; i<toggles.Length; i++)
        {
            toggles[i].isOn = !(count <= i);
        }
    }

    bool Matches(List<int> a, List<int> b)
    {
        if (a.Count != b.Count) return false;
        if (a.Count != 4) return false;
                
        for(int i = 0; i < a.Count; i++) { if (a[i] != b[i]) return false; }

        return true;
    }
}
