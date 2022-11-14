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


        btnToolTip[0].onClick.AddListener(() => openToolTip_0());
        btnToolTip[1].onClick.AddListener(() => openToolTip_1());
        btnToolTip[2].onClick.AddListener(() => openToolTip_2());



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
        upgradePrize.text = "x" + (UserDatabase.Instance.getTotalLevel() * 1000).ToString();
        UIController.Instance.InitUI();
    }

    private void openToolTip_0()
    {

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                toolTip[i].SetActive(true);
            }
            else
            {
                toolTip[i].SetActive(false);
            }
        }
    }
    private void openToolTip_1()
    {


        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
            {
                toolTip[i].SetActive(true);
            }
            else
            {
                toolTip[i].SetActive(false);
            }
        }
    }

    private void openToolTip_2()
    {


        for (int i = 0; i < 3; i++)
        {
            if (i == 2)
            {

                toolTip[i].SetActive(true);
            }
            else
            {

                toolTip[i].SetActive(false);
            }
        }
    }

    private void closeAllToolTip()
    {
        for (int i = 0; i < 3; i++)
        {


            toolTip[i].SetActive(false);

        }
    }

    private void ButtonUpgradeClicked()
    {
        if (UserDatabase.Instance.reduceMoney(UserDatabase.Instance.getTotalLevel() * 1000, 0))
        {

            int result = Random.Range(1, 7);
            // Cộng ngầm 
            UserDatabase.Instance.gainLevel(result);
            //Play anim
            StartCoroutine(replayAnimation(result));
        }
        else
        {
            Debug.Log("Out of money");
        }
    }
    IEnumerator replayAnimation(int result)
    {
        for (int i = 1; i <= 20; i++)
        {
            int rand = Random.Range(1, 7);
            ActiveVFX(rand);
            yield return new WaitForSeconds(0.1f);
        }
        ActiveVFX(result);
        InitUI();
    }
    void ActiveVFX(int item)
    {
        for (int i = 1; i <= 6; i++)
        {
            AbilitiesVFX[i].SetActive(false);
        }
        if (item > 0)
        {
            AbilitiesVFX[item].SetActive(true);
        }
    }
}
