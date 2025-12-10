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
}
