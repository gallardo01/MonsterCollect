using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MarchingBytes;
using TMPro;
using DG.Tweening;
using Cinemachine;
public class GameController : Singleton<GameController>
{
    [SerializeField] GameObject expBar;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject bossController;

    [SerializeField] GameObject pickAbilityPanel;
    [SerializeField] TextMeshProUGUI goldText;

    [SerializeField] Image iconImage;
    private int exp = 0;
    private int playerLevel = 1;
    private bool isSpawn = true;

    private int stage = 1;
    private int enemyLv = 1;
    private int countEnemy = 1;

    private int goldAward = 0;
    private List<ItemInventory> itemAward = new List<ItemInventory>();
    private int playerType;
    private int[] currentSkill = { 0, 1, 0, 0, 0 };
    private int[] skillLevel = { 0, 1, 0, 0, 0 };
    private int[] currentBuff = { 0, 0, 0, 0, 0 };
    private int[] buffLevel = { 0, 0, 0, 0, 0 };
    private int[] type = { 0, 0, 0 };
    private int[] availableOption = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private GameObject[] waypoints1;
    private bool isBossSpawn = true;
    [SerializeField] GameObject camera;
    private bool bossDead = false;
    [SerializeField] GameObject fillImage;
    [SerializeField] GameObject bossIcon;
    [SerializeField] TextMeshProUGUI bossHp;
    [SerializeField] GameObject warningBoss;
    [SerializeField] GameObject fadeScreen;
    [SerializeField] GameObject bossTileMaps;
    [SerializeField] GameObject gameTileMaps;
    [SerializeField] GameObject bossPosition;

    private int[] numberCreep = { 0, 40, 60, 70, 90, 105, 130, 150, 170, 200};

    private int idCurrentBoss = 50;


    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(testScreen());
    }
    
    IEnumerator testScreen()
    {
        yield return new WaitForSeconds(0f);
        //int heroesPick = PlayerPrefs.GetInt("HeroesPick");
        int heroesPick = 11;
        GameObject pl = Instantiate(Resources.Load("Prefabs/Heroes_Game/No." + heroesPick) as GameObject);
        pl.transform.position = new Vector3(0f, 0f, 0f);
        camera.GetComponent<CinemachineVirtualCamera>().Follow = pl.transform;
        // get stage    
        string route = "Route1";
        waypoints1 = GameObject.FindGameObjectsWithTag(route);
        System.Array.Sort(waypoints1, CompareObNames);
        enemyLv = 1;
        initInfo();
        playerLevel = PlayerController.Instance.getLevel();
        levelText.text = playerLevel.ToString();
        //spawn crep
        StartCoroutine(addEnemyFirstScene());
        //StartCoroutine(spawnBoss());
    }

    public void updateGold(int gold)
    {
        goldAward += gold * (100 + PlayerController.Instance.getBonusPoints(8))/100;
        goldText.text = goldAward.ToString();
        string floatingText = "FloatingText";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(floatingText, goldText.transform.position - new Vector3(0f, 0.7f, 0f), transform.rotation);
        particle.transform.SetParent(goldText.transform);
        particle.GetComponent<FloatingText>().showGold(gold);
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
        if (isSpawn && isBossSpawn)
        {
            addEnemy();
            if (enemyLv < 10 && countEnemy >= numberCreep[enemyLv])
            {
                enemyLv++;
            }
        }
        if (enemyLv >= 10)
        {
            isSpawn = false;
            StartCoroutine(spawnBoss());
        }
        else
        {
            StartCoroutine(respawnEnemyDuringTime());
        }
    }
    IEnumerator spawnBoss()
    {
        yield return new WaitForSeconds(1f);
        if(EasyObjectPool.instance.getObjAvailable() == false)
        {
            isBossSpawn = false;
            StartCoroutine(bossAnimationShowUp());
        }
        else
        {
            StartCoroutine(spawnBoss());
        }
    }
    public bool getBossSpawn()
    {
        return isBossSpawn;
    }
    IEnumerator bossAnimationShowUp()
    {
        warningBoss.SetActive(true);
        yield return new WaitForSeconds(3f);
        fadeScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        //set up thong tin truoc khi danh boss
        PlayerController.Instance.setPosition();
        bossTileMaps.SetActive(true);
        camera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9;
        yield return new WaitForSeconds(1.5f);
        // bat dau danh boss
        fadeScreen.SetActive(false);
        GameObject warning = EasyObjectPool.instance.GetObjectFromPool("Warning", bossPosition.transform.position, transform.rotation);
        yield return new WaitForSeconds(2f);
        warningBoss.SetActive(false);
        string enemyType = "Enemy" + (idCurrentBoss);
        GameObject boss = EasyObjectPool.instance.GetObjectFromPool(enemyType, warning.transform.position, transform.rotation);
        boss.GetComponent<BossController>().initInfo(idCurrentBoss);
        boss.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
        warning.SetActive(false);
        fillImage.SetActive(true);
        bossIcon.SetActive(true);
        bossHp.gameObject.SetActive(true);
        bossHp.text = BossController.Instance.getLevel().Hp.ToString();
        expBar.GetComponent<Slider>().value = 1f;
    }
    public void updateHpBoss(int number, int maxHp)
    {
        bossHp.text = number.ToString();
        expBar.GetComponent<Slider>().value = (float) number/maxHp;
    }
    private void initInfo()
    {
        expBar.GetComponent<Slider>().value = 0;
        updateGold(0);
    }
    public void endGame()
    {
        StartCoroutine(endGameFlow());
    }
    IEnumerator endGameFlow()
    {
        PlayerController.Instance.disablePlayer();
        bossDead = true;
        yield return new WaitForSeconds(5f);
        updateEndGameInformation();
        GameFlowController.Instance.gameOver();
    }
    public void setSpawn(bool logic)
    {
        isSpawn = logic;
    }
    public void addEnemy()
    {
        if (isSpawn)
        {
            StartCoroutine(addEnemyNow());
        }
    }
    private IEnumerator addEnemyNow()
    {
        int pos = Random.Range(0, waypoints1.Length);
        GameObject warning = EasyObjectPool.instance.GetObjectFromPool("Warning", waypoints1[pos].transform.position, waypoints1[pos].transform.rotation);
        yield return new WaitForSeconds(1.5f);
        EasyObjectPool.instance.ReturnObjectToPool(warning);
        warning.SetActive(false);
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
        GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, waypoints1[pos].transform.position, waypoints1[pos].transform.rotation);
        enemy.GetComponent<MonsterController>().initDataWaypoints(enemyId, countEnemy);
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
    public void addItemToDb(ItemInventory item)
    {
        itemAward.Add(item);
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
        updateProgressBar(exp >= playerLevel * 800);
    }
    private void updateProgressBar(bool levelUp)
    {
        //expBar.GetComponent<Slider>().value = progres;
        if (levelUp)
        {
            exp -= playerLevel * 800;
            playerLevel++;
            levelText.text = playerLevel.ToString();
            PlayerController.Instance.gainLv(playerLevel);
            updateColorText();
        }
        float progres = (float)exp / (float)(playerLevel * 800);
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
            if (currentSkill[i] > 0 && skillLevel[i] <= 5)
            {
                availableOption[currentSkill[i]] = 1;
            } else if (currentSkill[i] > 0 && skillLevel[i] == 6)
            {
                availableOption[currentSkill[i]] = 2;
            }
        }
        if (countSkill > 0)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (availableOption[i] != 2)
                {
                    availableOption[i] = 1;
                }
            }
        }
        int countBuff = 0;
        for (int i = 1; i <= 4; i++)
        {
            if (currentBuff[i] == 0) countBuff++;
            if (currentBuff[i] > 0 && buffLevel[i] <= 5)
            {
                availableOption[currentBuff[i]] = 1;
            }
            else if (currentSkill[i] > 0 && skillLevel[i] == 6)
            {
                availableOption[currentBuff[i]] = 2;
            }
        }
        if (countBuff > 0)
        {
            for (int j = 7; j <= 12; j++)
            {
                if (availableOption[j] != 2)
                {
                    availableOption[j] = 1;
                }
            }
        }
        for (int i = 1; i <= 12; i++)
        {
            if (availableOption[i] == 1)
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
        pickAbilityPanel.GetComponent<PickAbilityController>().initSkillData(currentSkill, skillLevel, currentBuff, buffLevel, type);
    }
    public void pickSkill(int id)
    {
        Time.timeScale = 1;
        // heal
        if (id == -1)
        {
            PlayerController.Instance.healPlayer(100);
        }
        else if (id == -2) // gold
        {
            int bonusGold = Random.Range(0, 200);
            if(Random.Range(0,10) == 9)
            {
                bonusGold += Random.Range(0, 800);
            }
            updateGold(bonusGold);
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

    public void addParticleDefault(GameObject obj, int type)
    {
        string par = "Particle_" + StaticInfo.typeText[type] + "_1";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        particle.transform.position = obj.transform.position;
        StartCoroutine(disableParticle(particle));
    }

    public void addExplosion(MyHeroes heroes, GameObject obj, int skillDame, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        particle.GetComponent<ExplosionController>().initData(heroes, skillDame);
        StartCoroutine(disableParticle(particle));
    }
    public void addExplosionText(MyHeroes heroes, GameObject obj, int skillDame, string objExplosion)
    {
        string par = objExplosion;
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
    public void addAwardToInventory(int gold, ItemInventory item)
    {
        goldAward += gold;
        itemAward.Add(item);
    }
    public void updateEndGameInformation()
    {
        int progress = countEnemy * 100 / 200  - 10;
        if (bossDead) progress = 100;
        GameFlowController.Instance.initData(progress, stage, goldAward, itemAward);
    }
    private int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

}
