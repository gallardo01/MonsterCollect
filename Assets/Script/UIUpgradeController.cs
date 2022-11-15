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

        //for (int i = 0; i < 9; i++)
        //{
        //    btnToolTip[i].onClick.AddListener(() => openToolTip(i));

        //}


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

        upgradePrize.text = "x" + (UserDatabase.Instance.getTotalLevel() * 1000).ToString();
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
        StartCoroutine(closeAllToolTip(id));  
    }


    IEnumerator closeAllToolTip(int id)
    {
        yield return new WaitForSeconds(3f);
 
            toolTip[id].SetActive(false);

    }

    private void ButtonUpgradeClicked()
    {
        if (UserDatabase.Instance.reduceMoney(UserDatabase.Instance.getTotalLevel() * 1000, 0))
        {

            int result = Random.Range(1, 10);
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

}
