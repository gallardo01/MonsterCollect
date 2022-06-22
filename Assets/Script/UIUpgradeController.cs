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
    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        ActiveVFX(0);
        btnUpgrade.onClick.AddListener(() => ButtonUpgradeClicked());
    }
    private void InitUI()
    {
        UserData database = UserDatabase.Instance.getUserData();
        AbilitiesLevel[1].text = database.Atk.ToString();
        AbilitiesLevel[2].text = database.Hp.ToString();
        AbilitiesLevel[3].text = database.Armour.ToString();
        AbilitiesLevel[4].text = database.Speed.ToString();
        AbilitiesLevel[5].text = database.BonusExp.ToString();
        AbilitiesLevel[6].text = database.BonusGold.ToString();
        upgradePrize.text = "x"+(UserDatabase.Instance.getTotalLevel() * 1000).ToString();
        UIController.Instance.InitUI();
    }
    private void ButtonUpgradeClicked()
    {
        if (UserDatabase.Instance.reduceMoney(UserDatabase.Instance.getTotalLevel()*1000,0))
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
