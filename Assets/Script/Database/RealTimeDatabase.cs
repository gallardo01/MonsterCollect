using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeDatabase : Singleton<RealTimeDatabase>
{

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
        UserInformation userInfor = new UserInformation();
        UserInformation itemInformation = ItemDatabase.Instance.totalEquipmentStats();
        MyHeroes heroesInformation = HeroesDatabase.Instance.fetchMyHeroes(PlayerPrefs.GetInt("HeroesPick"));
        UserData userData = UserDatabase.Instance.getUserData();

        userInfor.Atk = heroesInformation.Atk + itemInformation.Atk + userData.Atk * 10;
        userInfor.Hp = heroesInformation.Hp + itemInformation.Hp + userData.Hp * 10;
        userInfor.Armour = heroesInformation.Armour + itemInformation.Armour + userData.Armour * 10;
        userInfor.Move = heroesInformation.Move + itemInformation.Move + userData.Move * 10;
        userInfor.Crit = heroesInformation.Crit + itemInformation.Crit + userData.Crit * 10;
        userInfor.AttackSpeed = heroesInformation.Speed + itemInformation.AttackSpeed + userData.Speed * 10;
        userInfor.ExGold = itemInformation.ExGold;
        userInfor.ExExp = itemInformation.ExExp;
        return userInfor;
    }

}
