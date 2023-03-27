using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InflateCardPick : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Image glowImage;
    [SerializeField] Image headerImage;
    [SerializeField] TextMeshProUGUI nameSkill;
    [SerializeField] Image iconSkill;
    [SerializeField] TextMeshProUGUI textSkill;
    [SerializeField] GameObject[] stars;
    [SerializeField] GameObject[] starsAnimation;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initCard(int card, int level)
    {
        if ((card - 1) % 12 < 6 && card > 0)
        {
            cardImage.sprite = Resources.Load<Sprite>("UI/Background/Skill");
            headerImage.sprite = Resources.Load<Sprite>("UI/Background/Header_Skill");
            glowImage.color = Color.magenta;
        } else
        {
            cardImage.sprite = Resources.Load<Sprite>("UI/Background/Buff");
            headerImage.sprite = Resources.Load<Sprite>("UI/Background/Header_Buff");
            glowImage.color = Color.cyan;
        }
        iconSkill.sprite = Resources.Load<Sprite>("Contents/Move/" + card.ToString());
        if ((card - 1) % 12 < 6 && card > 0)
        {
            SkillData data = SkillDatabase.Instance.fetchSkillIndex(card);
            nameSkill.text = data.Skill;
            if (level == 0)
            {
                textSkill.text = data.Content;
            }
            else
            {
                textSkill.text = data.Second;
            }
        } else if ((card - 1) % 12 >= 6 && card > 0)
        {
            SkillData data = SkillDatabase.Instance.fetchSkillIndex(card);
            nameSkill.text = data.Skill;
            int percent = data.Power * (100 + (level) * 50) / 100;
            textSkill.text = data.Content + " +" + percent + "%";
        }
        else if(card == -1)
        {
            nameSkill.text = "Max Potion";
            textSkill.text = "Restore full HP immediately";
        } else if(card == -2)
        {
            nameSkill.text = "Gold";
            textSkill.text = "Gain random 1-1000 gold";
        }
        for (int i = 0; i < 5; i++)
        {
            stars[i].SetActive(false);
        }
        if (level == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                stars[i].SetActive(true);
                stars[i].GetComponent<Image>().color = Color.red;
                starsAnimation[i].GetComponent<Animator>().SetBool("isFade", true);
            }
        }
        else
        {
            for (int i = 0; i <= level; i++)
            {
                stars[i].SetActive(true);
                stars[i].GetComponent<Image>().color = Color.white;
                starsAnimation[i].GetComponent<Animator>().SetBool("isFade", false);
            }
            starsAnimation[level].GetComponent<Animator>().SetBool("isFade", true);
        }
    }
}
