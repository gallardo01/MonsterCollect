using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoSkillItem : MonoBehaviour
{
    // Start is called before the first frame update

    public Button btnItem;
    public Transform[] skillType;
    public int posX { get; set; }
    public int posY { get; set; }
    public int type { get; set; }
    public int value { get; set; }  
    void Start()
    {
        btnItem.onClick.AddListener(() => onClickItem());
        skillType[type].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onClickItem()
    {
        //Debug.Log(posX + " " + posY);
        BingoSkillMatix.Instance.clickOnItem(posX,posY);

    }


}
