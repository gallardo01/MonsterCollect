using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MarchingBytes;
using TMPro;
using DG.Tweening;

public class GameController : Singleton<GameController>
{
    [SerializeField] Button btnQuit;

    [SerializeField] Canvas result;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject player;
    [SerializeField] GameObject controller;

    [SerializeField] GameObject expBar;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject bossController;

    [SerializeField] GameController deadPanel;
    [SerializeField] Button revive;
    [SerializeField] Button cancelRevive;

    [SerializeField] GameObject pickAbilityPanel;
    [SerializeField] TextMeshProUGUI goldText;
    private int exp = 0;
    private int playerLevel = 1;
    private bool isSpawn = true;

    private int enemyLv = 1;
    private int countEnemy = 1;

    private int goldAward = 0;
    private List<ItemInventory> itemAward = new List<ItemInventory>();
    private int playerType = 1;
    private int[] currentSkill = { 0, 1, 0, 0, 0 };
    private int[] skillLevel = { 0, 1, 0, 0, 0 };
    private int[] currentBuff = { 0, 0, 0, 0, 0 };
    private int[] buffLevel = { 0, 0, 0, 0, 0 };
    private int[] type = { 0, 0, 0 };
    private int[] availableOption = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private bool isBossSpawn = true;
    // Start is called before the first frame update
    void Start()
    {
        // get stage
        enemyLv = 1;
        initInfo();
        playerLevel = PlayerController.Instance.getLevel();
        //addBoss();
        levelText.text = playerLevel.ToString();
        //btnQuit.onClick.AddListener(quitGame);

        //spawn crep
        StartCoroutine(addEnemyFirstScene());
        //addEnemy();
        //addBoss();
    }

    public void updateGold(int gold)
    {
        goldAward += gold * (100 + PlayerController.Instance.getBonusPoints(8))/100;
        goldText.text = goldAward.ToString();
        string floatingText = "FloatingText";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(floatingText, goldText.transform.position - new Vector3(0f, 0.5f, 0f), transform.rotation);
        particle.transform.SetParent(goldText.transform);
        particle.GetComponent<FloatingText>().showGold(gold);
    }

    private void tweenText()
    {
        

    }

    public IEnumerator addEnemyFirstScene()
    {
        for (int i = 0; i < 5; i++)
        {
            addEnemy();
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(respawnEnemyDuringTime());
    }

    public int getEnemyLv()
    {
        return enemyLv;
    }
    private IEnumerator respawnEnemyDuringTime()
    {
        yield return new WaitForSeconds(1.5f);
        if (isSpawn)
        {
            addEnemy();
            if (countEnemy % 20 == 0)
            {
                enemyLv++;
            }
        }
        if (enemyLv >= 10)
        {
            if (isBossSpawn)
            {
                isBossSpawn = false;
                addBoss();
            }
        }
        else
        {
            StartCoroutine(respawnEnemyDuringTime());
        }
    }

    private void initInfo()
    {
        expBar.GetComponent<Slider>().value = 0;
        updateGold(0);
    }

    private void addBoss()
    {
        string enemyType = "Enemy" + (20);
        GameObject boss = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);

        boss.GetComponent<BossController>().initInfo(20);
        boss.transform.localPosition = new Vector3(5, 5, 5);
        boss.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    public void setSpawn(bool logic)
    {
        isSpawn = logic;
    }
    public void addEnemy()
    {
        if (isSpawn)
        {
            countEnemy++;
            int chance = Random.Range(0, 14);
            int enemyId;
            if (chance < 13)
            {
                enemyId = enemyLv;
            }
            else
            {
                enemyId = enemyLv + chance % 10;
            }
            if (enemyId > 9)
            {
                enemyId = 9;
            }
            string enemyType = "Enemy" + enemyId;
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().initData(enemyId);
        }
    }
    public IEnumerator respawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(1, 4);
        if (chance == 1)
        {
            addEnemy();
        }
    }
    private void updateColorText()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in gos)
        {
            go.GetComponent<MonsterController>().setColor();
        }
    }
    public void gainExpChar(int num)
    {
        exp += num;
        updateProgressBar(exp >= playerLevel * 1000);
    }

    private void updateProgressBar(bool levelUp)
    {
        //expBar.GetComponent<Slider>().value = progres;
        if (levelUp)
        {
            exp -= playerLevel * 1000;
            playerLevel++;
            levelText.text = playerLevel.ToString();
            PlayerController.Instance.gainLv(playerLevel);
            updateColorText();
        }
        float progres = (float)exp / (float)(playerLevel * 1000);
        StartCoroutine(animationprogressBar(expBar.GetComponent<Slider>().value, progres, levelUp));
    }
    private void pickSkillLevelUp()
    {
        Time.timeScale = 0;
        List<int> chosenSkill = new List<int>();
        for (int i = 1; i <= 12; i++)
        {
            availableOption[i] = 0;
        }
        int countSkill = 0;
        for (int i = 1; i <= 4; i++)
        {
            if (currentSkill[i] == 0)
            {
                countSkill++;
            }
            if (currentSkill[i] > 0 && skillLevel[i] < 5)
            {
                availableOption[currentSkill[i]] = 1;
            }
        }
        if (countSkill > 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                availableOption[i] = 1;
            }
        }
        int countBuff = 0;
        for (int i = 1; i <= 4; i++)
        {
            if (currentBuff[i] == 0) countBuff++;
            if (currentBuff[i] > 0 && buffLevel[i] < 5)
            {
                availableOption[currentBuff[i]] = 1;
            }
        }
        if (countBuff > 0)
        {
            for (int j = 7; j <= 12; j++)
            {
                availableOption[j] = 1;
            }
        }
        for (int i = 1; i <= 12; i++)
        {
            if (availableOption[i] > 0)
            {
                chosenSkill.Add(i);
            }
        }
        chosenSkill.Add(-1);
        chosenSkill.Add(-2);
        if (chosenSkill.Count <= 3)
        {
            for (int i = 0; i < chosenSkill.Count; i++)
            {
                type[i] = chosenSkill[i];
            }
        }
        else
        {
            int count = 0;
            while (count < 3)
            {
                int ran = Random.Range(0, chosenSkill.Count);
                type[count] = chosenSkill[ran];
                chosenSkill.RemoveAt(ran);
                count++;
            }
        }
        pickAbilityPanel.SetActive(true);
        pickAbilityPanel.GetComponent<PickAbilityController>().initSkillData(currentSkill, skillLevel, currentBuff, buffLevel, type, playerType);
    }

    public void pickSkill(int id)
    {
        Time.timeScale = 1;
        // heal
        if (id == -1)
        {

        }
        else if (id == -2) // gold
        {

        }
        else if (id > 0) // skill 
        {
            for (int i = 1; i <= 4; i++)
            {
                if (currentSkill[i] == id)
                {
                    skillLevel[i]++;
                    PlayerController.Instance.setDataSkill(currentSkill, skillLevel);
                    return;
                }
                if (currentBuff[i] == id)
                {
                    buffLevel[i]++;
                    PlayerController.Instance.setDataBuff(currentBuff, buffLevel);
                    return;
                }
            }
            if (id % 12 > 0 && id % 12 <= 6)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (currentSkill[i] == 0)
                    {
                        currentSkill[i] = id;
                        skillLevel[i] = 1;
                        PlayerController.Instance.setDataSkill(currentSkill, skillLevel);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (currentBuff[i] == 0)
                    {
                        currentBuff[i] = id;
                        buffLevel[i] = 1;
                        PlayerController.Instance.setDataBuff(currentBuff, buffLevel);
                        return;
                    }
                }
            }
        }
    }

    private IEnumerator animationprogressBar(float current, float last, bool lvUp)
    {
        if (lvUp)
        {
            while (current < 1)
            {
                current += 0.05f;
                if (current >= 1)
                {
                    current = 1;
                    expBar.GetComponent<Slider>().value = current;
                    pickSkillLevelUp();
                }
                expBar.GetComponent<Slider>().value = current;
                yield return new WaitForSeconds(0.03f);
            }
            current = 0f;
            expBar.GetComponent<Slider>().value = current;
        }
        while (current < last)
        {
            current += 0.05f;
            if (current >= last)
            {
                current = last;
            }
            expBar.GetComponent<Slider>().value = current;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public int getLevel()
    {
        return playerLevel;
    }

    public void addParticle(GameObject obj, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        if (index == 2 || index == 4)
        {
            particle.transform.SetParent(obj.transform);
        }
        StartCoroutine(disableParticle(particle));
    }

    public void addExplosion(MyHeroes heroes, GameObject obj, int skillDame, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        particle.GetComponent<ExplosionController>().initData(heroes, skillDame);
        StartCoroutine(disableParticle(particle));
    }

    IEnumerator disableParticle(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }

    void quitGame()
    {
        controller.SetActive(false);
        canvas.SetActive(false);
        player.SetActive(false);
        result.gameObject.SetActive(true);
    }

    public void addAwardToInventory(int gold, ItemInventory item)
    {
        goldAward += gold;
        itemAward.Add(item);
    }
}
