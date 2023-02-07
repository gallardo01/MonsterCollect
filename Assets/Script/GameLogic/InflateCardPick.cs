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
        if ((card - 1) % 12 < 6)
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
        nameSkill.text = StaticInfo.skillName[card];
        textSkill.text = StaticInfo.skillContent[card];
        for(int i = 0; i < 5; i++)
        {
            stars[i].SetActive(false);
        }
        starsAnimation[level].GetComponent<Animator>().SetBool("isFade", true);
    }
}
