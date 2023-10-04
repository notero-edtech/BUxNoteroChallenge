using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class Game
{
    public static class Players
    {
        public static PlayerData plauer = new PlayerData();
        public static List<PlayerData> players = new List<PlayerData>();
    }
    
    [Serializable]
    public class PlayerData : AbstractGameData<PlayerData>
    {
        public string name = "";
        public Sprite avatarSprite = null;
        public Texture2D avatarTexture = null;

        public override void Init(){
        
        }
    }
}
