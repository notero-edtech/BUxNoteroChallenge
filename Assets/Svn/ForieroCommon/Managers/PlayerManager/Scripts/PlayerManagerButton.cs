using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerManagerButton : MonoBehaviour
{
		public PlayerManager playerManager;
		public Image buttonBackground;
		public Sprite normalButtonSprite;
		public Sprite selectedButtonSprite;
		public Text levelText;
		public Text userText;

		public void SetNormal ()
		{
				buttonBackground.sprite = normalButtonSprite;
		}

		public void SetSelected ()
		{
				buttonBackground.sprite = selectedButtonSprite;
		}

		public void OnPlayerClick ()
		{
				if (playerManager) {
						playerManager.OnPlayerClick (this);
				}
		}
}
