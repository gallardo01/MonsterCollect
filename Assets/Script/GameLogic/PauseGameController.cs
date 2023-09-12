using DigitalRuby.SoundManagerNamespace;
using DragonBones;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameController : MonoBehaviour
{
    [SerializeField] Image[] currentSkillImage;
    [SerializeField] Image[] currentBuffImage;
    [SerializeField] Image skillImage;
    [SerializeField] SpriteRenderer heroesImage;
    [SerializeField] TextMeshProUGUI skillText;
    [SerializeField] Button resume;
    [SerializeField] Button quit;

    private Sprite[] sprite;
    private Sprite[] skill;
    private Sprite[] heroes;
    float time;
    private void Awake()
    {
        sprite = Resources.LoadAll<Sprite>("Contents/Move");
        skill = Resources.LoadAll<Sprite>("Contents/Skill");
        heroes = Resources.LoadAll<Sprite>("UI/Icons/Monster");
    }
    // Start is called before the first frame update
    void Start()
    {
        resume.onClick.AddListener(() => resumeGame());
        quit.onClick.AddListener(() => StartCoroutine(Load()));
    }

    private void resumeGame()
    {
        Time.timeScale = time;
        gameObject.SetActive(false);
    }

    IEnumerator Load()
    {
        Time.timeScale = time;
        //SoundManagerDemo.Instance.StopAudio(1);
        AsyncOperation loadLevelOp = SceneManager.LoadSceneAsync("UI");
        while (!loadLevelOp.isDone)
        {
            // update progress bar amount to loadLevelOp.progress
            yield return null;
        }
    }

    public void initSkillData(int[] currentSkill, int[] currentBuff)
    {
        time = Time.timeScale;
        Time.timeScale = 0;
        int playerType = PlayerController.Instance.getType();
        for (int i = 0; i < 4; i++)
        {
            if (currentSkill[i + 1] > 0)
            {
                currentSkillImage[i].gameObject.SetActive(true);
                int id = currentSkill[i + 1] + (playerType - 1) * 12;
                currentSkillImage[i].sprite = sprite[id + 1];
            }
            else
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
        initHeroesData();
    }

    private void initHeroesData()
    {
        MyHeroes playerId = PlayerController.Instance.getRealData();
        skillImage.sprite = skill[playerId.Id/10 - 1];
        heroesImage.sprite = heroes[HeroesDatabase.Instance.fetchHeroesIndex(playerId.Id)];
        skillText.text = playerId.Skill;
    }
}
