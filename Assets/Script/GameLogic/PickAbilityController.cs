using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PickAbilityController : MonoBehaviour
{
    [SerializeField] Image[] currentSkillImage;
    [SerializeField] Image[] currentBuffImage;
    [SerializeField] GameObject[] cardChosen;
    [SerializeField] Button[] pickerSkill;
    private int[] typeSkill;
    private Sprite[] sprite;
    // Start is called before the first frame update
    void Start()
    {
        pickerSkill[0].onClick.AddListener(() => onClickSkill(0));
        pickerSkill[1].onClick.AddListener(() => onClickSkill(1));
        pickerSkill[2].onClick.AddListener(() => onClickSkill(2));
    }

    private void Awake()
    {
        sprite = Resources.LoadAll<Sprite>("Contents/Move");
    }
    private void onClickSkill(int id)
    {
        GameController.Instance.pickSkill(typeSkill[id]);
        gameObject.SetActive(false);
    }

    public void initSkillData(int[] currentSkill, int[] levelSkill, int[] currentBuff, int[] levelBuff, int[] type)
    {
        int playerType = PlayerController.Instance.getType();
        typeSkill = type;
        for(int i = 0; i < 4; i++)
        {
            if (currentSkill[i + 1] > 0) {
                currentSkillImage[i].gameObject.SetActive(true);
                int id = currentSkill[i + 1] + (playerType - 1) * 12;
                currentSkillImage[i].sprite = sprite[id + 1];
            } else
            {
                currentSkillImage[i].gameObject.SetActive(false);
            }
            if (currentBuff[i + 1] > 0)
            {
                currentBuffImage[i].gameObject.SetActive(true);
                int id = currentBuff[i + 1] + (playerType - 1) * 12;
                currentBuffImage[i].sprite = sprite[id + 1];
            }
            else
            {
                currentBuffImage[i].gameObject.SetActive(false);
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if(type[i] != 0)
            {
                cardChosen[i].SetActive(true);
                cardChosen[i].GetComponent<InflateCardPick>().initCard(type[i], returnLvCard(currentSkill, levelSkill, currentBuff, levelBuff, type[i]));
            } else
            {
                cardChosen[i].SetActive(false);
            }
        }
    }

    private int returnLvCard(int[] currentSkill, int[] levelSkill, int[] currentBuff, int[] levelBuff, int id)
    {
        for(int i = 1; i <= 4; i++)
        {
            if(id == currentSkill[i])
            {
                return levelSkill[i];
            }
            if(id == currentBuff[i])
            {
                return levelBuff[i];
            }
        }
        return 0;
    }
}
