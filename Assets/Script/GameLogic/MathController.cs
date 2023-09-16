using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathController : Singleton<MathController>
{
    // Start is called before the first frame update

    private int[] armourPercent = {20, 40, 60, 85, 120, 175, 200};

    private float[,] type = {{0, 0, 0, 0, 0 },
                             { 0, 1f, 1f, 0.7f, 1.3f },
                             { 0, 1f, 1f, 1.3f, 0.7f },
                             { 0, 1.3f, 0.7f, 1f, 1f },
                             { 0, 0.7f, 1.3f, 1f, 1f },
    };

    private void Start()
    {
   
    }

    public int playerHitEnemy(MyHeroes heroesData, MonsterData monsterData, int skillDame)
    {
        float hp;
        int critRate = percentageCrit(heroesData.Crit);
        int armourRate = percentageArmour(monsterData.Armour);
        float typeRate = type[heroesData.Type, monsterData.Type];
        if (heroesData.Id / 10 == 5 && typeRate == 1.3f)
        {
            typeRate = 1.8f;
        }
        if(heroesData.Id / 10 == 12)
        {
            typeRate = 1.3f;
        }
        if (typeRate > 1)
        {
            typeRate += (float)PlayerController.Instance.getBonusPoints(7) / 100;
        }
        if (heroesData.Id / 10 == 9)
        {
            critRate = 100;
        }
        if (Random.Range(0, 100) < critRate)
        {
            hp = (heroesData.Atk / 3)  * (skillDame / 100f) * typeRate * 2f * (100 - armourRate)/ 100f;
            if (heroesData.Id / 10 == 7)
            {
                hp *= 2;
            }
            //return 2000;
            return (int)hp;
        } else
        {
            hp = -(heroesData.Atk / 3) * (skillDame / 100f) * typeRate * (100 - armourRate)/ 100f;
            return (int)hp;
            //return 2000;
        }
    }
    public int playerHitEnemy(MyHeroes heroesData, MonsterData monsterData, int skillDame, int bonusPercent)
    {
        float hp;
        int critRate = percentageCrit(heroesData.Crit);
        int armourRate = percentageArmour(monsterData.Armour);
        float typeRate = type[heroesData.Type, monsterData.Type];
        if (heroesData.Id / 10 == 5 && typeRate == 1.3f)
        {
            typeRate = 1.8f;
        }
        if (typeRate > 1)
        {
            typeRate += (float)PlayerController.Instance.getBonusPoints(7) / 100;
        }
        if (heroesData.Id / 10 == 9)
        {
            critRate = 100;
        }

        if (Random.Range(0, 100) < critRate)
        {
            hp = (heroesData.Atk / 3) * (skillDame / 100f) * typeRate * 2f * (100 - armourRate + bonusPercent) / 100f;
            return (int)hp;
        }
        else
        {
            hp = -(heroesData.Atk / 3) * (skillDame / 100f) * typeRate * (100 - armourRate + bonusPercent) / 100f;
            return (int)hp;
        }
    }

    public int getTypeValue(MyHeroes heroesData, MonsterData monsterData)
    {
        float typeRate = type[heroesData.Type, monsterData.Type];
        if (typeRate == 1f)
        {
            return 0;
        } else if (typeRate > 1f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int enemyHitPlayer(MyHeroes heroesData, MonsterData monsterData)
    {
        float hp;
        int armourRate = percentageArmour(heroesData.Armour);
        float typeRate = type[heroesData.Type, monsterData.Type];
        hp = (monsterData.Atk / 2) * typeRate * (100 - armourRate) / 100f;
        return (int)hp;
    }

    private int percentageCrit(int crit)
    {
        return 5 + (crit/85);
    }

    private int percentageArmour(int armour)
    {
        int percent = armour / 35;
        if (percent > 75)
        {
            percent = 75;
        }
        return percent;
    }
}
