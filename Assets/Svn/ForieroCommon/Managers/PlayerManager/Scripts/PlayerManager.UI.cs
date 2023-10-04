using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class PlayerManager : MonoBehaviour
{
    List<PlayerManagerButton> playerButtons = new List<PlayerManagerButton>();

    PlayerManagerButton AddButton(string name, int levels)
    {
        PlayerManagerButton b = Instantiate(PREFAB_user).GetComponent<PlayerManagerButton>();
        b.transform.SetParent(userContainer);
        RectTransform rt = b.transform as RectTransform;
        rt.SetSize(new Vector2(userContainer.GetSize().x, rt.GetSize().y));
        rt.anchoredPosition = new Vector2(0, -userContainer.GetSize().y);
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        b.transform.localScale = Vector3.one;
        b.playerManager = this;

        b.levelText.text = levels.ToString();
        b.userText.text = name;

        userContainer.SetSize(new Vector2(userContainer.GetSize().x, userContainer.GetSize().y + rt.GetSize().y));

        playerButtons.Add(b);

        return b;
    }

    public void InitPlayerButtons()
    {
        if (playerButtons.Count > 0)
        {
            for (int i = 0; i < playerButtons.Count; i++)
            {
                Destroy(playerButtons[i].gameObject);
            }
        }

        playButton.interactable = false;

        playerButtons = new List<PlayerManagerButton>();

        userContainer.SetSize(new Vector2(userContainer.GetSize().x, 0));

        for (int i = 0; i < Players.players.Count; i++)
        {
            PlayerManagerButton button = AddButton(Players.players[i].name, Players.players[i].GetInt("CHECK_POINT"));
            if (Players.players[i].selected)
            {
                button.SetSelected();
                playButton.interactable = true;
            }
            else
            {
                button.SetNormal();
            }
        }

        FocusSelectedPlayer();
    }

    public void FocusSelectedPlayer()
    {
        if (Players.selectedPlayer == null)
            return;

        for (int i = 0; i < playerButtons.Count; i++)
        {
            if (Players.selectedPlayer.name == playerButtons[i].userText.text)
            {
                scrollRect.verticalNormalizedPosition = 1f - (float)(i + 1) / (float)playerButtons.Count();
            }
        }
    }
    
    public void OnScrollRectChange() { /*Debug.Log (scrollRect.normalizedPosition); */ }
    public void OnPlayerInputChange() { newUserInputField.text = newUserInputField.text.RemoveNewLine(); }
    public void OnPlayerInputEnd() { }
    public void OnAddClick()
    {
        newUserInputField.text = newUserInputField.text.Trim();

        if (string.IsNullOrEmpty(newUserInputField.text))
        {
            newUserInputField.image.color = Color.red;
            return;
        }
        else
        {
            newUserInputField.image.color = Color.white;
        }

        for (int i = 0; i < Players.players.Count; i++)
        {
            if (Players.players[i].name == newUserInputField.text)
            {
                newUserInputField.image.color = Color.red;
                return;
            }
        }

        AddPlayer(newUserInputField.text);

        PlayerManagerButton button = AddButton(newUserInputField.text, 0);

        Players.selectedPlayer = Players.GetPlayer(newUserInputField.text);

        _player = Players.selectedPlayer;

        playButton.interactable = true;

        foreach (PlayerManagerButton b in playerButtons)
        {
            if (b == button)
                b.SetSelected();
            else
                b.SetNormal();
        }

        newUserInputField.text = "";

        FocusSelectedPlayer();

        EventSystem.current.SetSelectedGameObject(playButton.gameObject, null);

        if (!PlayerManager.autoSave) Save();
    }

    public void OnRemoveClick()
    {
        if (player == null)
            return;

        confirmPanel.gameObject.SetActive(true);
        confirmText.text = "Delete user : " + player.name;
        confirmClick = ConfirmClick.User;
    }

    public void OnPlayClick()
    {
        if (SceneSettings.WasSceneOnceLoaded(SceneSettings.LoadEnum.Command, "INTRO", Players.selectedPlayer.guid))
        {
            SceneSettings.LoadSceneByCommand("NEXT");
        }
        else
        {
            SceneSettings.LoadSceneByCommand("INTRO", Players.selectedPlayer.guid);
        }
        SM.StopAllMusic(0.2f);
    }

    public void OnPlayerClick(PlayerManagerButton button)
    {
        playButton.interactable = true;
        Players.selectedPlayer = Players.GetPlayer(button.userText.text);
        _player = Players.selectedPlayer;
        foreach (PlayerManagerButton b in playerButtons)
        {
            if (b == button)
                b.SetSelected();
            else
                b.SetNormal();
        }

        if (!PlayerManager.autoSave) Save();
    }

    public enum ConfirmClick
    {
        None,
        User,
        AllUsers
    }

    ConfirmClick confirmClick = ConfirmClick.None;

    public void OnResetClick()
    {
        confirmPanel.gameObject.SetActive(true);
        confirmText.text = "Delete all users?";
        confirmClick = ConfirmClick.AllUsers;
    }

    public void OnResetYesClick()
    {
        switch (confirmClick)
        {
            case ConfirmClick.AllUsers:
                PlayerManager.Reset();
                InitPlayerButtons();
                break;
            case ConfirmClick.User:
                if (player != null)
                {
                    RemovePlayer(player.name);
                    foreach (PlayerManagerButton b in playerButtons)
                    {
                        b.SetNormal();
                    }
                    playButton.interactable = false;
                    InitPlayerButtons();
                }
                break;
        }

        confirmPanel.gameObject.SetActive(false);
    }

    public void OnResetNoClick()
    {
        confirmPanel.gameObject.SetActive(false);
    }

    public void OnReferralsClick()
    {
        referralsPanel.gameObject.SetActive(true);
    }
}
