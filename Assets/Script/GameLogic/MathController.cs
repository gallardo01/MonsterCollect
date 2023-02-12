using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathController : Singleton<MathController>
{
    // Start is called before the first frame update

    private int[] armourPercent = {0, 10, 20, 25, 30, 35, 40, 50, 1000};
    private int[] armourPoints = {0, 15, 30, 60, 80, 100, 160, 200, 1000};

    private int[] critPercent = { 0, 5, 10, 20, 25, 30, 35, 40, 50 };
    private int[] critPoints = { 0, 200, 400, 700, 1200, 1800, 2500, 3500, 5000};

    private float[,] type = {{0, 0, 0, 0, 0 },
                             {0, 0.7f, 1.3f, 1f, 0.7f },
                             { 0, 0.7f, 0.7f, 1.3f, 0.7f },
                             { 0, 1f, 0.7f, 0.7f, 1.3f },
                             { 0, 1.3f, 1f, 0.7f, 0.7f }
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
        if (Random.Range(0, 100) < critRate)
        {
            hp = 100 + heroesData.Atk * skillDame / 100f * typeRate * 2f * (100 - armourRate) * 0.3f / 100f + heroesData.Atk * heroesData.Atk / 3000 * skillDame / 100f * typeRate * 2f * (100 - armourRate) * 0.3f / 100f;
            return (int)hp;
        } else
        {
            hp = -100 + (-heroesData.Atk) * skillDame / 100f * typeRate * (100 - armourRate) * 0.3f / 100f - heroesData.Atk * heroesData.Atk / 3000 * skillDame / 100f * typeRate * (100 - armourRate) * 0.3f / 100f; 
            return (int)hp;
        }
    }

    public int enemyHitPlayer(MyHeroes heroesData, MonsterData monsterData)
    {
        float hp;
        int armourRate = percentageArmour(heroesData.Armour);
        float typeRate = type[monsterData.Type, heroesData.Type];
        
        hp = monsterData.Atk * typeRate * (100 - armourRate) / 100f;
        return (int)hp;
    }


    private int percentageCrit(int crit)
    {
        return 5 + (crit/100);
    }

    private int percentageArmour(int armour)
    {
        int percent = 0;
        int step = 1;
        while(armour > 0)
        {
            if(armour >= (armourPercent[step] - armourPercent[step-1]) * armourPoints[step])
            {
                percent += (armourPercent[step] - armourPercent[step - 1]);
                armour -= (armourPercent[step] - armourPercent[step - 1]) * armourPoints[step];
            }
            else
            {
                percent += armour/ armourPoints[step];
                armour = 0;
                break;
            }
            step++;
        }
        if(percent >= 50)
        {
            percent = 50;
        }
        return percent;
    }
}
