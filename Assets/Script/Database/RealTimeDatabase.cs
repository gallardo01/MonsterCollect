using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeDatabase : Singleton<RealTimeDatabase>
{
    private UserInformation userRealData = new UserInformation();
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public UserInformation getUserInformation()
    {
        UserInformation itemInformation = ItemDatabase.Instance.totalEquipmentStats();
        MyHeroes heroesInformation = HeroesDatabase.Instance.fetchMyHeroes(PlayerPrefs.GetInt("HeroesPick"));
        UserData userData = UserDatabase.Instance.getUserData();

        itemInformation.Atk = (int) heroesInformation.Atk + itemInformation.Atk + userData.Atk * 20;
        itemInformation.Hp = (int) heroesInformation.Hp + itemInformation.Hp + userData.Hp * 20;
        itemInformation.Armour = (int) heroesInformation.Armour + itemInformation.Armour + userData.Armour * 20;
        itemInformation.Move = (int) heroesInformation.Move + itemInformation.Move + userData.Move * 20;
        itemInformation.Crit = (int) heroesInformation.Crit + itemInformation.Crit + userData.Crit * 20;
        itemInformation.AttackSpeed = (int)heroesInformation.Speed + itemInformation.AttackSpeed + userData.Speed * 20;
        itemInformation.ExGold = (int)itemInformation.ExGold;
        itemInformation.ExExp = (int)itemInformation.ExExp;

        int totalBonus = 0;
        for (int i = 1; i <= 4; i++)
        {
            if(ItemDatabase.Instance.getBonusItemType(i) >= 3)
            {
                if (heroesInformation.Type == i)
                {
                    totalBonus += 5;
                }
            }
            if(ItemDatabase.Instance.getBonusItemType(i) == 6)
            {
                totalBonus += 5;
            }
        }
        if(totalBonus > 0)
        {
            itemInformation.Atk = itemInformation.Atk * (totalBonus + 100) / 100;
            itemInformation.Hp = itemInformation.Hp * (totalBonus + 100) / 100;
            itemInformation.Armour = itemInformation.Armour * (totalBonus + 100) / 100;
            itemInformation.Move = itemInformation.Move * (totalBonus + 100) / 100;
            itemInformation.Crit = itemInformation.Crit * (totalBonus + 100) / 100;
            itemInformation.AttackSpeed = itemInformation.AttackSpeed * (totalBonus + 100) / 100;
            itemInformation.ExGold = itemInformation.ExGold * (totalBonus + 100) / 100;
            itemInformation.ExExp = itemInformation.ExExp * (totalBonus + 100) / 100;
        }
        userRealData = itemInformation;
        return itemInformation;
    }

    public UserInformation getData()
    {
        return (UserInformation)userRealData.Clone();
    }
}
