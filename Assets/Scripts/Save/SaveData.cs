using System;

[Serializable]
public class PlayerSaveData
{
    public float posX;
    public float posY;

    public int level;
    public int exp;
    public int currentHP;
    public int maxHP;
    public int expToNextLevel;
}

[Serializable]
public class EnemySaveData
{
    public string id;
    public float posX;
    public float posY;
    public int currentHP;
    public bool isDead;
}

[Serializable]
public class AnimalSaveData
{
    public string id;
    public float posX;
    public float posY;
    public int currentHP;
    public bool isDead;
}

[Serializable]
public class SaveData
{
    public string sceneName;

    public PlayerSaveData player;

    public EnemySaveData[] enemies;
    public AnimalSaveData[] animals;

    public float gameTime;

    // 👇 METADATA CHO SAVE UI
    public string playerName;
    public string saveTime; // DateTime.Now.ToString()
    public float completionTime; // thời gian hoàn thành game
}



