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

    public int damage;
}

[Serializable]
public class EnemySaveData
{
    public string id;
    public float posX;
    public float posY;
    public int currentHP;
    public bool isDead;

    public float deathTime;      // gameTime lúc chết
    public float respawnDelay;   // bao lâu thì hồi
}


[Serializable]
public class AnimalSaveData
{
    public string id;
    public float posX;
    public float posY;
    public int currentHP;
    public bool isDead;

    public float deathTime;
    public float respawnDelay;
}

[Serializable]
public class SaveData
{
    public string sceneName;
    public string sceneTransitionName; 

    public PlayerSaveData player;
    public EnemySaveData[] enemies;
    public AnimalSaveData[] animals;

    public float gameTime;

    public string playerName;
    public string saveTime;
    public float completionTime;
}




