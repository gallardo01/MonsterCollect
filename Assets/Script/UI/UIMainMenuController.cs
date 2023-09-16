using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using DigitalRuby.SoundManagerNamespace;

public class UIMainMenuController : MonoBehaviour
{

    public Transform Maps;
    public TextMeshProUGUI mapName;
    public TextMeshProUGUI mapDescription;
    public GameObject locker;

    public Button leftBtn;
    public Button rightBtn;
    public Button playGame;

    private int currentMap = 1;
    private int currentStage = 1;

    private bool isClick = true;
    private Sprite[] itemsSprite;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Map"))
        {
            PlayerPrefs.SetInt("Map", 1);
        }
        currentStage = UserDatabase.Instance.getUserData().Level;
        currentMap = PlayerPrefs.GetInt("Map");
        selectMap(currentMap);

        leftBtn.onClick.AddListener(() => leftButton());
        rightBtn.onClick.AddListener(() => rightButton());
        playGame.onClick.AddListener(() => playGameAction());
    }

    private void playGameAction()
    {
        if (currentMap > UserDatabase.Instance.getUserData().Level)
        {
            PlayerPrefs.SetInt("Map", currentStage);
        }
        else
        {
            PlayerPrefs.SetInt("Map", currentMap);
        }

        SceneManager.LoadScene(2);
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
        itemsSprite = Resources.LoadAll<Sprite>("Contents/Icon/Island");

        Maps.GetComponent<SpriteRenderer>().sprite = itemsSprite[index-1];
        locker.SetActive(false);
        foreach (Transform child in Maps.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject monster = Instantiate(Resources.Load("Prefabs/Monster/Stage" + index.ToString()) as GameObject, Maps.transform);
        monster.transform.localPosition = new Vector3(4, -1.2f, 0);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");


        if (index > currentStage)
        {
            locker.SetActive(true);
        } else
        {
            currentStage = index;
        }

        mapName.text = StaticInfo.mapName[index];
        mapDescription.text = StaticInfo.mapType[index];
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
