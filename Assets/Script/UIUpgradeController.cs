using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIUpgradeController : MonoBehaviour
{
    public GameObject[] Abilities;
    public GameObject[] AbilitiesVFX;
    public TextMeshProUGUI[] AbilitiesLevel;
    public Button btnUpgrade;
    public TextMeshProUGUI upgradePrize;
    public Button[] btnToolTip;
    public GameObject[] toolTip;
    public Button btnBlank;
    public TextMeshProUGUI[] txtAlibityDetail;

    private bool IsUpdated;
    private int maxLevel = 10;

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        ActiveVFX(0);
        btnUpgrade.onClick.AddListener(() => ButtonUpgradeClicked());

        btnToolTip[0].onClick.AddListener(() => openToolTip(0));
        btnToolTip[1].onClick.AddListener(() => openToolTip(1));
        btnToolTip[2].onClick.AddListener(() => openToolTip(2));
        btnToolTip[3].onClick.AddListener(() => openToolTip(3));
        btnToolTip[4].onClick.AddListener(() => openToolTip(4));
        btnToolTip[5].onClick.AddListener(() => openToolTip(5));
        btnToolTip[6].onClick.AddListener(() => openToolTip(6));
        btnToolTip[7].onClick.AddListener(() => openToolTip(7));
        btnToolTip[8].onClick.AddListener(() => openToolTip(8));

        btnBlank.onClick.AddListener(() => closeAllToolTip());
        setupTxtAlibityDetail();
        IsUpdated = false;

    }
    private void InitUI()
    {
        UserData database = UserDatabase.Instance.getUserData();
        AbilitiesLevel[1].text = database.Atk.ToString();
        AbilitiesLevel[2].text = database.Hp.ToString();
        AbilitiesLevel[3].text = database.Armour.ToString();
        AbilitiesLevel[4].text = database.Move.ToString();
        AbilitiesLevel[5].text = database.Crit.ToString();
        AbilitiesLevel[6].text = database.Speed.ToString();
        AbilitiesLevel[7].text = database.Equipment.ToString();
        AbilitiesLevel[8].text = database.ExtraGold.ToString();
        AbilitiesLevel[9].text = database.ExtraExp.ToString();

        if (canUpgrade())
        {
            upgradePrize.text = "x" + (UserDatabase.Instance.getTotalLevel() * 1000).ToString();

        }
        else
        {
            btnUpgrade.gameObject.SetActive(false);
        }
        UIController.Instance.InitUI();
    }

    private void openToolTip(int id)
    {
        for (int i = 0; i < 9; i++)
        {
            if (i == id)
            {
                toolTip[i].SetActive(true);
            }
            else
            {
                toolTip[i].SetActive(false);
            }
        }
        //StartCoroutine(closeAllToolTip(3f));  
    }


    //IEnumerator closeAllToolTip(float time)
    //{
    //    Debug.Log("close");
    //    yield return new WaitForSeconds(time);
    //    for (int i = 0; i < 9; i++)
    //    {
    //        toolTip[i].SetActive(false);

    //    }

    //}

    private void closeAllToolTip()
    {

        for (int i = 0; i < 9; i++)
        {
            toolTip[i].SetActive(false);

        }

    }

    private void ButtonUpgradeClicked()
    {
        if (!IsUpdated && canUpgrade())
        {
            if (UserDatabase.Instance.reduceMoney(UserDatabase.Instance.getTotalLevel() * 1000, 0))
            {
                UserData database = UserDatabase.Instance.getUserData();

                int[] arr =
                {
                    0,
                    database.Atk,
                    database.Hp,
                    database.Armour,
                    database.Move,
                    database.Crit,
                    database.Speed,
                    database.Equipment,
                    database.ExtraGold,
                    database.ExtraExp
                };

                int result;

                do 
                {
                    result = Random.Range(1, 10);
                } while (arr[result] >= maxLevel) ;

                // Cộng ngầm 
                UserDatabase.Instance.gainLevel(result);
                //Play anim
                StartCoroutine(replayAnimation(result));
                //StartCoroutine(replayAnimation2(result));

            }
            else
            {
                Debug.Log("Out of money");
            }

            closeAllToolTip();
            setupTxtAlibityDetail();
            IsUpdated = true;

        }
    }


    private bool canUpgrade()
    {
        UserData database = UserDatabase.Instance.getUserData();
        if (database.Atk < maxLevel || database.Armour < maxLevel | database.Hp < maxLevel | database.Move < maxLevel | database.Speed < maxLevel || database.Crit < maxLevel || database.ExtraExp < maxLevel || database.ExtraGold < maxLevel || database.Equipment < maxLevel)
        {
            return true;
        }
        return false;
    }
    IEnumerator replayAnimation(int result)
    {
        for (int i = 1; i <= 10; i++)
        {
            int rand = Random.Range(1, 10);
            ActiveVFX(rand);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 1; i <= 10; i++)
        {
            int rand = Random.Range(1, 10);
            ActiveVFX(rand);
            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 0; i < 5; i++)
        {
            ActiveVFX(result);
            yield return new WaitForSeconds(0.15f);
            AbilitiesVFX[result].SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }
        InitUI();
        IsUpdated = false;

    }


    void ActiveVFX(int item)
    {
        for (int i = 1; i <= 9; i++)
        {
            AbilitiesVFX[i].SetActive(false);
        }
        if (item > 0)
        {
            AbilitiesVFX[item].SetActive(true);
        }
    }

    void setupTxtAlibityDetail()
    {
        UserData database = UserDatabase.Instance.getUserData();
        txtAlibityDetail[0].text = "Power\nAtk + " + database.Atk * StaticInfo.userUpdateBase[0];
        txtAlibityDetail[1].text = "Strength\nHp + " + database.Hp * StaticInfo.userUpdateBase[1];
        txtAlibityDetail[2].text = "Block\nArmour + " + database.Armour * StaticInfo.userUpdateBase[2];
        txtAlibityDetail[3].text = "Boost\nMovement + " + database.Move * StaticInfo.userUpdateBase[3];
        txtAlibityDetail[4].text = "Dexterous\nCrit + " + database.Crit * StaticInfo.userUpdateBase[4];
        txtAlibityDetail[5].text = "Agile\nSpeed + " + database.Speed * StaticInfo.userUpdateBase[5];
        txtAlibityDetail[6].text = "Intelligence\nEquipment + " + database.Equipment * StaticInfo.userUpdateBase[6] +"%";
        txtAlibityDetail[7].text = "Glory\nExtra Gold + " + database.ExtraGold * StaticInfo.userUpdateBase[7] + "%";
        txtAlibityDetail[8].text = "Inspire\nExtra Exp + " + database.ExtraExp * StaticInfo.userUpdateBase[8] + "%";
    }

}
