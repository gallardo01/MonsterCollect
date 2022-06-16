using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIMainMenuController : MonoBehaviour
{

    public RectTransform Maps;
    public TextMeshProUGUI mapName;
    public TextMeshProUGUI mapDescription;
    public GameObject locker;

    public Button leftBtn;
    public Button rightBtn;

    private int currentMap = 1;
    private int currentStage = 3;

    private bool isClick = true;


    // Start is called before the first frame update
    void Start()
    {
        selectMap(currentMap);

        leftBtn.onClick.AddListener(() => leftButton());
        rightBtn.onClick.AddListener(() => rightButton());

    }

    private void leftButton()
    {
        if (isClick)
        {
            isClick = false;

            currentMap--;
            if (currentMap == 0)
            {
                currentMap = 10;
            }
            selectMap(currentMap);
        }
    }

    private void rightButton()
    {
        if (isClick)
        {
            isClick = false;

            currentMap++;
            if (currentMap == 11)
            {
                currentMap = 1;
            }
            selectMap(currentMap);

        }
    }

    private void selectMap(int index)
    {
        Maps.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Maps/map_" + index);
        locker.SetActive(false);
        Maps.GetComponent<Image>().color = Color.white;


        if (index > currentStage)
        {
            Maps.GetComponent<Image>().color = new Color(152f / 255f, 152f / 255f, 152f / 255f);
            locker.SetActive(true);
        }


        mapName.text = StaticInfo.mapName[index];
        //mapDescription.text = "/10";


        StartCoroutine(animationMap());
        StartCoroutine(animationText());


    }

    IEnumerator animationMap()
    {
        Maps.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.07f);
        yield return new WaitForSeconds(0.1f);
        Maps.DOScale(new Vector3(1f, 1f, 1f), 0.07f);
    }

    IEnumerator animationText()
    {
        mapName.GetComponent<RectTransform>().DOScale(new Vector3(0f, 0f, 0f), 0.1f);
        mapDescription.GetComponent<RectTransform>().DOScale(new Vector3(0f, 0f, 0f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        mapName.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.1f);
        mapDescription.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.1f);


        yield return new WaitForSeconds(0.3f);
        isClick = true;

    }

    
}
