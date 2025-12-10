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

    public int armorPhysical;
    public int armorMagic;
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
    public bool flipX;
    public int currentHP;
    public bool hasHealth;
}

[Serializable]
public class SaveData
{
    public string sceneName;

    public PlayerSaveData player;

    public EnemySaveData[] enemies;
    public AnimalSaveData[] animals;
}
