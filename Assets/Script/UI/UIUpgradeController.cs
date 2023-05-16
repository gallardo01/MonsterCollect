using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEditor;

public class UIUpgradeController : MonoBehaviour
{
    public GameObject[] Abilities;
    public GameObject[] AbilitiesVFX;
    public TextMeshProUGUI[] AbilitiesLevel;
    public Button btnUpgrade;
    public TextMeshProUGUI upgradePrize;
    public Button[] btnToolTip;
    public GameObject toolTip;
    public Button btnBlank;
    public TextMeshProUGUI txtAlibityDetail;

    private bool IsUpdated;
    private int maxLevel = 20;
    private int requireGold = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        IsUpdated = false;
        InitUI();
        updateRequireGold();
    }
    private void updateRequireGold()
    {
        requireGold = UserDatabase.Instance.getTotalLevel()*300 + 300;
        upgradePrize.text = requireGold.ToString();
        if (UserDatabase.Instance.getTotalLevel() < 180 && requireGold <= UserDatabase.Instance.getUserData().Gold)
        {
            btnUpgrade.interactable = true;
        }
        else
        {
            btnUpgrade.interactable = false;
        }
        UIController.Instance.InitUI();
    }
    private void InitUI()
    {
        UserData database = UserDatabase.Instance.getUserData();

        AbilitiesLevel[1].text = database.Atk < 10 ? database.Atk.ToString() : "MAX";
        AbilitiesLevel[2].text = database.Hp < 10 ? database.Hp.ToString() : "MAX";
        AbilitiesLevel[3].text = database.Armour < 10 ? database.Armour.ToString() : "MAX";
        AbilitiesLevel[4].text = database.Move < 10 ? database.Move.ToString() : "MAX";
        AbilitiesLevel[5].text = database.Crit < 10 ? database.Crit.ToString() : "MAX";
        AbilitiesLevel[6].text = database.Speed < 10 ? database.Speed.ToString() : "MAX";
        AbilitiesLevel[7].text = database.Equipment < 10 ? database.Equipment.ToString() : "MAX";
        AbilitiesLevel[8].text = database.ExtraGold < 10 ? database.ExtraGold.ToString() : "MAX";
        AbilitiesLevel[9].text = database.ExtraExp < 10 ? database.ExtraExp.ToString(): "MAX";
        ItemStatus(database);
    }

    private void ItemStatus(UserData database)
    {
        Abilities[1].GetComponent<Animator>().SetTrigger(database.Atk < 1 ? "Lock" : "Unlock");
        Abilities[2].GetComponent<Animator>().SetTrigger(database.Hp < 1 ? "Lock" : "Unlock");
        Abilities[3].GetComponent<Animator>().SetTrigger(database.Armour < 1 ? "Lock" : "Unlock");
        Abilities[4].GetComponent<Animator>().SetTrigger(database.Move < 1 ? "Lock" : "Unlock");
        Abilities[5].GetComponent<Animator>().SetTrigger(database.Crit < 1 ? "Lock" : "Unlock");
        Abilities[6].GetComponent<Animator>().SetTrigger(database.Speed < 1 ? "Lock" : "Unlock");
        Abilities[7].GetComponent<Animator>().SetTrigger(database.Equipment < 1 ? "Lock" : "Unlock");
        Abilities[8].GetComponent<Animator>().SetTrigger(database.ExtraGold < 1 ? "Lock" : "Unlock");
        Abilities[9].GetComponent<Animator>().SetTrigger(database.ExtraExp < 1 ? "Lock" : "Unlock");
    }
    private void openToolTip(int id)
    {
        id++;
        toolTip.SetActive(true);
        Vector3 pos = new Vector3(Abilities[id].transform.position.x, Abilities[id].transform.position.y - 300f, 0);
        toolTip.transform.position = Abilities[id].transform.position;
        setupTxtAlibityDetail(id);
    }

    private void closeAllToolTip()
    {
        toolTip.SetActive(false);
    }

    private void ButtonUpgradeClicked()
    {
        if (!IsUpdated && UserDatabase.Instance.getTotalLevel() < 180)
        {
            btnUpgrade.interactable = false;
            if (requireGold <= UserDatabase.Instance.getUserData().Gold)
            {
                UserData database = UserDatabase.Instance.getUserData();
                UserDatabase.Instance.reduceMoney(requireGold, 0);
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
            }
            else
            {
                Debug.Log("Out of money");
            }
            closeAllToolTip();
            IsUpdated = true;
            updateRequireGold();
        }
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
    void setupTxtAlibityDetail(int num)
    {
        UserData database = UserDatabase.Instance.getUserData();
        if(num == 1) { txtAlibityDetail.text = "Power\nAtk + " + database.Atk * StaticInfo.userUpdateBase[0]; }
        else if(num == 2) { txtAlibityDetail.text = "Strength\nHp + " + database.Hp * StaticInfo.userUpdateBase[1]; }
        else if (num == 3) { txtAlibityDetail.text = "Block\nArmour + " + database.Armour * StaticInfo.userUpdateBase[2]; }
        else if (num == 4) { txtAlibityDetail.text = "Boost\nMovement + " + database.Move * StaticInfo.userUpdateBase[3]; }
        else if (num == 5) { txtAlibityDetail.text = "Dexterous\nCrit + " + database.Crit * StaticInfo.userUpdateBase[4]; }
        else if (num == 6) { txtAlibityDetail.text = "Agile\nAtk Speed + " + database.Speed * StaticInfo.userUpdateBase[5]; }
        else if (num == 7) { txtAlibityDetail.text = "Intelligence\nEquipment + " + database.Equipment * StaticInfo.userUpdateBase[6] + "%"; }
        else if (num == 8) { txtAlibityDetail.text = "Glory\nExtra Gold + " + database.ExtraGold * StaticInfo.userUpdateBase[7] + "%"; }
        else if (num == 9) { txtAlibityDetail.text = "Inspire\nExtra Exp + " + database.ExtraExp * StaticInfo.userUpdateBase[8] + "%"; }
    }

}
