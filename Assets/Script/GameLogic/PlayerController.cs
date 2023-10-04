using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;
using UnityEngine.UI;
using DigitalRuby.SoundManagerNamespace;
using MiniJSON;
using Newtonsoft.Json;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] GameObject body;
    //[SerializeField] TextMeshPro levelText;
    [SerializeField] Image typeImage;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] GameObject hpBar;

    [SerializeField] GameObject locate;
    [SerializeField] int idPick;

    [SerializeField] GameObject line_hp;
    [SerializeField] GameObject line;

    public GameObject SmokePos;
    private int currentHp;
    private GameObject joystick;
    private int facingRight = 1;
    private bool walk = true;
    private int playerLevel = 1;
    private bool canMove = true;
    private bool isAtk = false;
    private bool canHurt = true;
    private static MyHeroes data;
    private MyHeroes realData;

    float timeSmoke = 0;
    private float timeSmokeWait = 1f;
    private bool isPause = false;
    private bool isActiveNonRepeat = true;
    private GameObject gameObjectNonRepeat;
    private float bonusSpeed = 1f;
    private int bonusAtk = 1;
    private bool revivePenguin = true;

    // 1.Atk 2.Hp 3.Armour 4.Move 5.Crit 6.Speed 7.SuperEffective 8.Gold 9.Exp 10. Healing
    private int[] bonusPoints = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] bonusPointsExtra = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] buffLevel = { 0, 0, 0, 0, 0, 0, 0 };
    private int cacheSpeed;
    private bool isPlayerRooted = false;
    private List<SkillInGame> totalSkill = new List<SkillInGame>();
    private void Awake()
    {
        joystick = GameObject.FindGameObjectWithTag("JoyStick");
    }
    void Start()
    {
        //Vector3 pos = new Vector3(Screen.width, Screen.height, 0);
        if (!PlayerPrefs.HasKey("Map"))
        {
            PlayerPrefs.SetInt("Map", 1);
        }
        initStart();
    }
    private int getSkillData(int id)
    {
        for (int i = 0; i < totalSkill.Count; i++)
        {
            if (id == totalSkill[i].data.Id)
            {
                return i;
            }
        }
        return -1;
    }
    public Transform getPosition()
    {
        return gameObject.transform;
    }
    public void setPosition()
    {
        gameObject.transform.position = new Vector3(0f, 0f, 0f);
    }
    public void initStart()
    {
        // pick con nao?
        data = HeroesDatabase.Instance.fetchMyHeroesData(idPick);
        realData = HeroesDatabase.Instance.fetchMyHeroesData(idPick);
        // get real data
        UserInformation realDataUser = RealTimeDatabase.Instance.getData();

        realData.Atk = realDataUser.Atk;
        realData.Crit = realDataUser.Crit;
        realData.Hp = realDataUser.Hp;
        realData.Armour = realDataUser.Armour;
        realData.Speed = realDataUser.AttackSpeed;
        realData.Move = realDataUser.Move;

        data.Atk = realDataUser.Atk;
        data.Crit = realDataUser.Crit;
        data.Hp = realDataUser.Hp;
        data.Armour = realDataUser.Armour;
        data.Speed = realDataUser.AttackSpeed;
        data.Move = realDataUser.Move;

        bonusPointsExtra[8] = realDataUser.ExGold / 15;
        bonusPointsExtra[9] = realDataUser.ExExp / 15;
        if (data.Id / 10 == 1)
        {
            bonusPointsExtra[8] += 20;
        } else if (data.Id / 10 == 2)
        {
            bonusPointsExtra[9] += 20;
        }
        bonusPoints[8] = bonusPointsExtra[8];
        bonusPoints[9] = bonusPointsExtra[9];

        currentHp = realData.Hp;
        hpText.text = currentHp.ToString();
        hpBar.GetComponent<Slider>().value = 1f;
        typeImage.sprite = Resources.Load<Sprite>("UI/Icons/Type/" + data.Type);
        int typeHeroes = (data.Type - 1) * 12 + 1;
        for (int i = typeHeroes; i < typeHeroes + 6; i++)
        {
            SkillInGame skill = SkillDatabase.Instance.fetchSkillIngame(i);
            totalSkill.Add(skill);
        }
        calculateSkillDame();
        attackMonster(totalSkill[0]);

        for (int i = 0; i < currentHp / 400; i++)
        {
            GameObject line_obj = Instantiate(line, line_hp.transform.position, line_hp.transform.rotation);
            line_obj.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
            line_obj.transform.SetParent(line_hp.transform);
            line_obj.transform.localPosition = new Vector3(0f, 0f, 0f);
            line_obj.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    // 1.Atk 2.Hp 3.Armour 4.Move 5.Crit 6.Speed 7.SuperEffective 8.Gold 9.Exp 
    private void updatePlayerData()
    {
        realData.Atk = data.Atk * (100 + bonusPoints[1] + bonusPointsExtra[1]) / 100;
        int cacheHp = realData.Hp;
        realData.Hp = data.Hp * (100 + bonusPoints[2]) / 100;
        realData.Armour = data.Armour * (100 + bonusPoints[3]) / 100;
        realData.Move = data.Move * (100 + bonusPoints[4]) / 100;
        realData.Crit = data.Crit * (100 + bonusPoints[5]) / 100;
        realData.Speed = data.Speed * (100 + bonusPoints[6]) / 100;
        if (realData.Hp - cacheHp > 0)
        {
            int percentHeal = (realData.Hp - cacheHp) * 100 / realData.Hp;
            healPlayer(percentHeal);
        }
        //Debug.Log(JsonConvert.SerializeObject(realData, Formatting.Indented));
    }
    public MyHeroes getRealData()
    {
        return realData;
    }
    public int getBonusPoints(int i)
    {
        return bonusPoints[i];
    }
    public void gainLv(int lv)
    {
        SoundManagerDemo.Instance.playOneShot(6);
        playerLevel = lv;
        //StartCoroutine(runVFX());
        // Need turn on VFX
    }
    public Transform returnObj()
    {
        return this.gameObject.transform;
    }
    public int getLevel()
    {
        return playerLevel;
    }
    public int getType()
    {
        return data.Type;
    }

    // Update is called once per frame;
    void Update()
    {
        if (canMove)
        {
            transform.position += new Vector3(UltimateJoystick.GetHorizontalAxis("Movement"),
            UltimateJoystick.GetVerticalAxis("Movement"), 0).normalized * (float)(realData.Move * bonusSpeed / 590f) * Time.deltaTime;
        }
        if (UltimateJoystick.GetHorizontalAxis("Movement") > 0 && facingRight == 0)
        {
            flip();
        }
        else if (UltimateJoystick.GetHorizontalAxis("Movement") < 0 && facingRight == 1)
        {
            flip();
        }
        if (UltimateJoystick.GetHorizontalAxis("Movement") != 0 || UltimateJoystick.GetVerticalAxis("Movement") != 0)
        {
            if (walk)
            {
                walk = false;
                runAnimation(2);
            }
        }
        else
        {
            if (!walk)
            {
                walk = true;
                runAnimation(1);
            }
        }
    }
    public void disablePlayer()
    {
        this.enabled = false;
    }
    private void FixedUpdate()
    {
        if (UltimateJoystick.GetHorizontalAxis("Movement") != 0 || UltimateJoystick.GetVerticalAxis("Movement") != 0)
        {
            if (timeSmoke > timeSmokeWait) // only check for space bar if we last fired longer than the cooldown time
            {
                GameObject smoke = EasyObjectPool.instance.GetObjectFromPool("Smoke", new Vector3(SmokePos.transform.position.x, SmokePos.transform.position.y, SmokePos.transform.position.z), SmokePos.transform.rotation);
                smoke.GetComponent<SmokeDisable>().disableSelf();
                timeSmoke = 0;
            }
            else
            {
                timeSmoke += Time.deltaTime;
            }
        }
    }

    public void setDataSkill(int[] currentSkill, int[] skillLv)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (currentSkill[i] > 0)
            {
                int idSkill = currentSkill[i] - 1;
                totalSkill[idSkill].data.Level = skillLv[i];
                if (totalSkill[idSkill].isTrigger == false || totalSkill[idSkill].data.Id == 18 || totalSkill[idSkill].data.Id == 27)
                {
                    attackMonster(totalSkill[idSkill]);
                }
            }
        }
        calculateSkillDame();
    }
    private void calculateSkillDame()
    {
        for (int i = 0; i < totalSkill.Count; i++)
        {
            totalSkill[i].powerGame = totalSkill[i].data.Power * (90 + 10 * totalSkill[i].data.Level) / 100;
            totalSkill[i].timerGame = totalSkill[i].data.Timer - (float)(totalSkill[i].data.Level - 1) * totalSkill[i].data.Upgrade / 1000;
        }
    }

    //buff
    public void setDataBuff(int[] currentBuff, int[] buffLv)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (currentBuff[i] > 0)
            {
                int idSkill = currentBuff[i] - 6;
                buffLevel[idSkill] = buffLv[i];
            }
        }
        for (int i = 1; i <= 6; i++)
        {
            if (buffLevel[i] > 0)
            {
                int skillId = (data.Type - 1) * 12 + 6 + i;
                SkillData skillData = SkillDatabase.Instance.fetchSkillIndex(skillId);
                bonusPoints[skillData.Timer] = skillData.Power * (100 + (buffLevel[i] - 1) * 50) / 100 + bonusPointsExtra[skillData.Timer];
            }
        }
        updatePlayerData();
    }
    public void increasePointExtra()
    {
        if (bonusPointsExtra[1] < 100)
        {
            bonusPointsExtra[1]++;
            updatePlayerData();
        }
    }
    public void attackMonster(SkillInGame skill)
    {
        int id = getSkillData(skill.data.Id);
        string bulletText = StaticInfo.typeText[realData.Type] + "_" + (skill.data.Id % 12);
        int skillId = skill.data.Id;
        totalSkill[id].isTrigger = true;
        if (skillId % 12 == 1)
        {
            StartCoroutine(normalAttack(bulletText, id));
        }
        else if (skillId == 14 || skillId == 26)
        {
            StartCoroutine(throwAWeb(bulletText, id));
        }
        else if (skillId == 15 || skillId == 3)
        {
            StartCoroutine(thunder_2(bulletText, id));
        }
        else if (skillId == 16 || skillId == 6 || skillId == 40)
        {
            StartCoroutine(bombFly(bulletText, id));
        }
        else if (skillId == 17 || skillId == 2)
        {
            StartCoroutine(bounceAroundEnemy(bulletText, id));
        }
        else if (skillId == 18 || skillId == 27)
        {
            forceField(bulletText, id);
        }
        else if (skillId == 4 || skillId == 28) // meteor
        {
            StartCoroutine(meteorFalling(bulletText, id));
        }
        else if (skillId == 5) // bomb
        {
            StartCoroutine(bombTrigger(bulletText, id));
        }
        else if (skillId == 29)
        {
            StartCoroutine(snowmanTrigger(bulletText, id));
        }
        else if (skillId == 30 || skillId == 42)
        {
            StartCoroutine(reflectBullet(bulletText, id));
        }
        else if (skillId == 38)
        {
            StartCoroutine(rootGrassTree(bulletText, id));
        } 
        else if(skillId == 39)
        {
            StartCoroutine(grassFlyAround(bulletText, id));
        } 
        else if(skillId == 41)
        {
            StartCoroutine(spawnSpikeTree(bulletText, id));
        }
    }
    IEnumerator disableObject(float timer, GameObject obj)
    {
        yield return new WaitForSeconds(timer);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }
    private IEnumerator normalAttack(string bulletText, int id)
    {
        int bonusTimer = realData.Speed / 100;
        if (realData.Id/10 == 8)
        {
            int percent = currentHp * 100 / realData.Hp;
            bonusTimer += 50 * (100 - percent) / 100;
        }
        if (bonusTimer > 100) bonusTimer = 100;
        yield return new WaitForSeconds(totalSkill[id].timerGame * (100 - bonusTimer) / 100 + 0.4f);
        if (isPause == false)
        {
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                SoundManagerDemo.Instance.playOneShot(2);
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, shootTarget.rotation);
                projectileNormal.GetComponent<BulletController>().initBullet(realData, 5, totalSkill[id].powerGame, shootTarget);
                Vector2 vector = shootFollower(shootTarget);
                float angle = calAngle(shootTarget, vector);
                projectileNormal.transform.Rotate(0, 0, angle + 90);
                if (totalSkill[id].data.Level >= 6)
                {
                    yield return new WaitForSeconds(0.1f);
                    GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, shootTarget.rotation);
                    projectileNormal1.GetComponent<BulletController>().initBullet(realData, 5, totalSkill[id].powerGame, shootTarget);
                    projectileNormal1.transform.Rotate(0, 0, angle + 90);
                }
            }
        }
        StartCoroutine(normalAttack(bulletText, id));
    }
    private IEnumerator snowmanTrigger(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        createSnowman(bulletText, id);
        if (totalSkill[id].data.Level >= 3)
        {
            yield return new WaitForSeconds(0.2f);
            createSnowman(bulletText, id);
        }
        if (totalSkill[id].data.Level >= 6)
        {
            yield return new WaitForSeconds(0.2f);
            createSnowman(bulletText, id);
        }
        StartCoroutine(snowmanTrigger(bulletText, id));
    }
    private void createSnowman(string bulletText, int id)
    {
        Vector2 position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        Transform pos = EasyObjectPool.instance.getRandomTargetPosition();
        if (pos != null)
        {
            position.x = pos.position.x;
            position.y = pos.position.y;
        }
        GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, position,
gameObject.transform.rotation);
        projectileNormal.GetComponent<BulletSnowmanController>().initBullet(realData, totalSkill[id].powerGame, gameObject);
    }

    private IEnumerator bombTrigger(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        createBombFalling(bulletText, id);
        if (totalSkill[id].data.Level >= 3)
        {
            yield return new WaitForSeconds(0.2f);
            createBombFalling(bulletText, id);
        }
        if (totalSkill[id].data.Level >= 6)
        {
            yield return new WaitForSeconds(0.2f);
            createBombFalling(bulletText, id);
        }
        StartCoroutine(bombTrigger(bulletText, id));
    }
    private void createBombFalling(string bulletText, int id)
    {
        GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, gameObject.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0),
gameObject.transform.rotation);
        projectileNormal.GetComponent<BulletBombController>().initBullet(realData, totalSkill[id].powerGame);
    }
    private IEnumerator meteorFalling(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        createMeteorFalling(bulletText, id);
        if (totalSkill[id].data.Level >= 3)
        {
            yield return new WaitForSeconds(0.2f);
            createMeteorFalling(bulletText, id);
        }
        if (totalSkill[id].data.Level >= 6)
        {
            yield return new WaitForSeconds(0.2f);
            createMeteorFalling(bulletText, id);
        }
        StartCoroutine(meteorFalling(bulletText, id));
    }
    private void createMeteorFalling(string bulletText, int id)
    {
        GameObject shootTarget = EasyObjectPool.instance.GetObjectFromPool("Shadow", locate.transform.position,
locate.transform.rotation);
        Transform pos = EasyObjectPool.instance.getRandomTargetPosition();
        if (pos != null)
        {
            shootTarget.transform.position = pos.transform.position;
        }
        else
        {
            shootTarget.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        }
        GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
shootTarget.transform.rotation);
        projectileNormal.transform.position = shootTarget.transform.position + new Vector3(Random.Range(-3f, 3f), 10f, 0);
        projectileNormal.GetComponent<BulletOnGroundController>().initBullet(realData, 1, totalSkill[id].powerGame, shootTarget.transform);
    }
    private IEnumerator throwAWeb(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        if (isPause == false)
        {
            Transform shootTargetObj = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            GameObject shootTarget = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
    locate.transform.rotation);
            StartCoroutine(disableObject(2f, shootTarget));

            if (shootTargetObj != null)
            {
                shootTarget.transform.position = shootTargetObj.position;
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.transform.rotation);
                StartCoroutine(disableObject(3f, projectileNormal));

                projectileNormal.GetComponent<BulletController>().initBullet(realData, 1, totalSkill[id].powerGame, shootTarget.transform);
                if (totalSkill[id].data.Level >= 3)
                {
                    GameObject shootTarget1 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
locate.transform.rotation);
                    StartCoroutine(disableObject(3f, shootTarget1));
                    shootTarget1.transform.position = shootTargetObj.position + new Vector3(-1f, 0f, 0f);
                    GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
        shootTarget1.transform.rotation);
                    StartCoroutine(disableObject(3f, projectileNormal1));
                    projectileNormal1.GetComponent<BulletController>().initBullet(realData, 1, totalSkill[id].powerGame, shootTarget1.transform);
                }
                if (totalSkill[id].data.Level >= 6)
                {
                    GameObject shootTarget2 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
locate.transform.rotation);
                    StartCoroutine(disableObject(3f, shootTarget2));
                    shootTarget2.transform.position = shootTargetObj.position + new Vector3(1f, 0f, 0f);
                    GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
        shootTarget2.transform.rotation);
                    StartCoroutine(disableObject(3f, projectileNormal1));
                    projectileNormal1.GetComponent<BulletController>().initBullet(realData, 1, totalSkill[id].powerGame, shootTarget2.transform);
                }
            }
        }
        StartCoroutine(throwAWeb(bulletText, id));
    }
    private IEnumerator spawnSpikeTree(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        if (isPause == false)
        {
            GameObject shootTarget = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
locate.transform.rotation);
            shootTarget.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, shootTarget.transform.position,
shootTarget.transform.rotation);
            projectileNormal.GetComponent<BulletHitBulletController>().initBullet(realData, totalSkill[id].powerGame);
            if (totalSkill[id].data.Level >= 3)
            {
                GameObject shootTarget1 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position, locate.transform.rotation);
                shootTarget1.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, shootTarget1.transform.position, shootTarget1.transform.rotation);
                projectileNormal1.GetComponent<BulletHitBulletController>().initBullet(realData, totalSkill[id].powerGame);
            }
            if (totalSkill[id].data.Level >= 6)
            {
                GameObject shootTarget2 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position, locate.transform.rotation);
                shootTarget2.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                GameObject projectileNormal2 = EasyObjectPool.instance.GetObjectFromPool(bulletText, shootTarget2.transform.position, shootTarget2.transform.rotation);
                projectileNormal2.GetComponent<BulletHitBulletController>().initBullet(realData, totalSkill[id].powerGame);
            }

        }
        StartCoroutine(spawnSpikeTree(bulletText, id));
    }
    private IEnumerator rootGrassTree(string bulletText, int id)
    {
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        if (isPause == false)
        {
            GameObject shootTarget = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
locate.transform.rotation);
            shootTarget.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool("Grass_2", shootTarget.transform.position,
shootTarget.transform.rotation);
            projectileNormal.GetComponent<BulletRootController>().initBullet(realData, totalSkill[id].powerGame);
            if (totalSkill[id].data.Level >= 3)
            {
                GameObject shootTarget1 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position, locate.transform.rotation);
                shootTarget1.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool("Grass_2", shootTarget1.transform.position, shootTarget1.transform.rotation);
                projectileNormal1.GetComponent<BulletRootController>().initBullet(realData, totalSkill[id].powerGame);
            }
            if (totalSkill[id].data.Level >= 6)
            {
                GameObject shootTarget2 = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position, locate.transform.rotation);
                shootTarget2.transform.position = this.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                GameObject projectileNormal2 = EasyObjectPool.instance.GetObjectFromPool("Grass_2", shootTarget2.transform.position, shootTarget2.transform.rotation);
                projectileNormal2.GetComponent<BulletRootController>().initBullet(realData, totalSkill[id].powerGame);
            }

        }
        StartCoroutine(rootGrassTree(bulletText, id));
    }
    private IEnumerator thunder_2(string bulletText, int id)
    {
        if (isPause == false)
        {
            if (totalSkill[id].data.Level < 3)
            {
                StartCoroutine(thunder_2_clone(bulletText, 0, id));
            }
            else if (totalSkill[id].data.Level >= 3 && totalSkill[id].data.Level < 6)
            {
                StartCoroutine(thunder_2_clone(bulletText, 0, id));
                StartCoroutine(thunder_2_clone(bulletText, 3, id));
            }
            else
            {
                StartCoroutine(thunder_2_clone(bulletText, 0, id));
                StartCoroutine(thunder_2_clone(bulletText, 2, id));
                StartCoroutine(thunder_2_clone(bulletText, 4, id));
            }
        }
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        StartCoroutine(thunder_2(bulletText, id));
    }
    private IEnumerator thunder_2_clone(string bulletText, int circle, int id)
    {
        GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, new Vector3(20f, 20f, 0f),
gameObject.transform.rotation);
        projectileNormal.GetComponent<BulletFlyAround>().initBullet(realData, circle, totalSkill[id].powerGame, gameObject.transform);
        yield return new WaitForSeconds(3.5f);
        EasyObjectPool.instance.ReturnObjectToPool(projectileNormal);
        projectileNormal.SetActive(false);
    }
    private IEnumerator grassFlyAround(string bulletText, int id)
    {
        if (isPause == false)
        {
            for(int i = 0; i < totalSkill[id].data.Level + 3; i++)
            {
                StartCoroutine(grassFlyAroundAction(bulletText, i, id));
            }
        }
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        StartCoroutine(grassFlyAround(bulletText, id));
    }
    private IEnumerator grassFlyAroundAction(string bulletText, int circle, int id)
    {
        GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, new Vector3(20f, 20f, 0f),
gameObject.transform.rotation);
        projectileNormal.GetComponent<BulletFlyAround>().initBullet(realData, circle, totalSkill[id].powerGame, gameObject.transform);
        yield return new WaitForSeconds(0f);
    }

    private IEnumerator reflectBullet(string bulletText, int id)
    {
        if (isPause == false)
        {
            Vector2 vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector2 vector2 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector2 vector3 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            vector = vector.normalized;
            vector2 = vector2.normalized;
            vector3 = vector3.normalized;
            int speedBomb = 500;

            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
            projectileNormal.GetComponent<BulletBouncingController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
            projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * speedBomb);
            if (totalSkill[id].data.Level >= 3)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletBouncingController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector2 * speedBomb);
            }
            if (totalSkill[id].data.Level >= 6)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletBouncingController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector3 * speedBomb);
            }
        }
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        StartCoroutine(reflectBullet(bulletText, id));
    }

    private IEnumerator bombFly(string bulletText, int id)
    {
        if (isPause == false)
        {
            Vector2 vector = new Vector2(-1f, 2f);
            Vector2 vector2 = new Vector2(1f, -2f);
            Vector2 vector3 = new Vector2(-1f, -2f);
            Vector2 vector4 = new Vector2(1f, 2f);
            Vector2 vector5 = new Vector2(0f, 2f);
            Vector2 vector6 = new Vector2(0f, -2f);

            int speedBomb = 100;
            if (totalSkill[id].data.Id == 6)
            {
                speedBomb = 60;
            }
            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
            projectileNormal.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
            projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * speedBomb);
            if (totalSkill[id].data.Level > 1)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector2 * speedBomb);
            }
            if (totalSkill[id].data.Level > 2)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector3 * speedBomb);
            }
            if (totalSkill[id].data.Level > 3)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector4 * speedBomb);
            }
            if (totalSkill[id].data.Level == 6)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector5 * speedBomb);
                GameObject projectileNormal2 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal2.GetComponent<BulletNoTargetController>().initBullet(realData, 3, totalSkill[id].powerGame, gameObject.transform);
                projectileNormal2.GetComponent<Rigidbody2D>().AddForce(vector6 * speedBomb);
            }
        }
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        StartCoroutine(bombFly(bulletText, id));
    }
    private IEnumerator bounceAroundEnemy(string bulletText, int id)
    {
        if (isPause == false)
        {
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.rotation);
                projectileNormal.GetComponent<BulletController>().initBullet(realData, 4, totalSkill[id].powerGame, shootTarget);
            }
            if (totalSkill[id].data.Level >= 6)
            {
                yield return new WaitForSeconds(0.5f);
                Transform shootTarget2 = EasyObjectPool.instance.getNearestHitPosition(gameObject);
                if (shootTarget2 != null)
                {
                    GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
        shootTarget2.rotation);
                    projectileNormal1.GetComponent<BulletController>().initBullet(realData, 4, totalSkill[id].powerGame, shootTarget2);
                }
            }
        }
        yield return new WaitForSeconds(totalSkill[id].timerGame);
        StartCoroutine(bounceAroundEnemy(bulletText, id));
    }
    private void forceField(string bulletText, int id)
    {
        if (isPause == false)
        {
            int size = totalSkill[id].data.Level;
            if (isActiveNonRepeat)
            {
                isActiveNonRepeat = false;
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, gameObject.transform.position,
    gameObject.transform.rotation);
                //projectileNormal.transform.parent = gameObject.transform;
                //projectileNormal.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                projectileNormal.transform.position = gameObject.transform.position;

                gameObjectNonRepeat = projectileNormal;
                float scaleNumber = 0.7f + (size - 1) * 0.1f;
                projectileNormal.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
                projectileNormal.GetComponent<BulletOnStayController>().initBullet(realData, size, totalSkill[id].powerGame, gameObject);
            }
            else
            {
                float scaleNumber = 0.7f + (size - 1) * 0.1f;             
                gameObjectNonRepeat.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
                gameObjectNonRepeat.GetComponent<BulletOnStayController>().initBullet(realData, size, totalSkill[id].powerGame, gameObject);
            }
        }
    }

    IEnumerator reActiveObject(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        obj.SetActive(true);
        reActiveObject(obj);
    }
    IEnumerator returnToPoolEmpty(GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }
    private float giveRandomFloatNumber(float x, float y)
    {
        if (Random.Range(1, 9) % 2 == 0)
        {
            return Random.Range(x, y);
        }
        return Random.Range(-y, -x);
    }
    private void runAnimation(int pos)
    {
        //idle
        if (pos == 1 && isAtk == false)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
        }
        //move
        else if (pos == 2 && isAtk == false)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");
        }
        else if (pos == 3) // atk
        {
            isAtk = true;
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("Attack", 0.5f, 1);
            StartCoroutine(replayAnimation());
        }
        else if (pos == 4) // dead
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("die", 1f, 1);
            StartCoroutine(deadAnimation());
        }
    }
    IEnumerator deadAnimation()
    {
        canMove = false;
        isPause = true;
        gameObject.tag = "Wall";
        joystick.SetActive(false);
        GameController.Instance.updateEndGameInformation();
        yield return new WaitForSeconds(0.99f);
        GetComponent<DragonBones.UnityArmatureComponent>().animation.Stop();
        yield return new WaitForSeconds(2f);
        if (realData.Id / 10 == 3 && revivePenguin == true)
        {
            revivePenguin = false;
            revivePlayer();
        }
        else
        {
            GameFlowController.Instance.userDeath();
        }
    }
    IEnumerator replayAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        isAtk = false;
        if (walk)
        {
            runAnimation(1);
        }
        else
        {
            runAnimation(2);
        }
    }

    private void flip()
    {
        facingRight = 1 - facingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;

        Vector3 newScale1 = hpBar.GetComponent<RectTransform>().localScale;
        newScale1.x *= -1;
        hpBar.GetComponent<RectTransform>().localScale = newScale1;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (canHurt && currentHp > 0)
            {
                MonsterData enemyLv = collision.gameObject.GetComponent<MonsterController>().getLevel();
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                collision.gameObject.GetComponent<MonsterController>().setAction(1);
            }
        }
        if (collision.gameObject.tag == "Boss")
        {
            if (canHurt && currentHp > 0)
            {
                MonsterData enemyLv = collision.gameObject.GetComponent<BossController>().getLevel();
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                collision.gameObject.GetComponent<BossController>().setAction(1);
            }
        }
    }
    IEnumerator setHurt(MonsterData monsterData)
    {
        int dame = MathController.Instance.enemyHitPlayer(data, monsterData);
        reduceHealth(dame);
        body.GetComponent<Animator>().SetTrigger("hurt");
        yield return new WaitForSeconds(0.5f);
        canHurt = true;
    }
    IEnumerator setHurtPercent(MonsterData monsterData, int percent)
    {
        int dame = MathController.Instance.enemyHitPlayer(data, monsterData);
        dame = dame * percent / 100;
        reduceHealth(dame);
        body.GetComponent<Animator>().SetTrigger("hurt");
        yield return new WaitForSeconds(0.7f);
        canHurt = true;
    }
    public void setPlayerHurt(MonsterData monsterData, int status)
    {
        // 1 root
        if (status == 1)
        {
            rootPlayer();
        }
        // 2 bi dau
        else if (status == 2)
        {
            if (canHurt)
            {
                canHurt = false;
                StartCoroutine(setHurt(monsterData));
            }
        }
        else if (status == 3) //slow Speed
        {
            StartCoroutine(slowPlayer(70));
        }
        else if (status == 4) //slow Speed
        {
            rootPlayer();
        }
        else if (status > 10) // chi gay x % dame
        {
            if (canHurt)
            {
                canHurt = false;
                StartCoroutine(setHurtPercent(monsterData, status));
            }
        }
        else // 3 vua stun vua mat mau
        {
            if (canHurt)
            {
                canHurt = false;
                StartCoroutine(setHurt(monsterData));
            }
        }
    }
    private void reduceHealth(int amount)
    {
        string floatingText = "FloatingText";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
        particle.GetComponent<FloatingText>().playerHealth(amount);

        currentHp -= amount;

        if (currentHp <= 0)
        {
            currentHp = 0;
            //dead
            GameController.Instance.setSpawn(false);
            canHurt = false;
            //StopAllCoroutines();
            runAnimation(4);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(false);
            }
        }
        float per = (float)currentHp / realData.Hp;
        hpText.text = currentHp.ToString();
        hpBar.GetComponent<Slider>().value = per;
        calculateBonusSpeed();
    }
    private void calculateBonusSpeed()
    {
        if (realData.Id / 10 == 10)
        {
            float percent = (float) currentHp / realData.Hp;
            bonusSpeed = 1f + 0.5f * (1f - percent);
        }
    }
    public void setPlayerNormal()
    {
        realData.Move = cacheSpeed;

    }
    public void revivePlayer()
    {
        canMove = true;
        joystick.SetActive(true);
        isPause = false;
        gameObject.tag = "Player";
        currentHp = realData.Hp;
        hpBar.GetComponent<Slider>().value = 1f;
        StartCoroutine(cannotHurtDuringRevive());
        hpText.text = currentHp.ToString();
        GameController.Instance.setSpawn(true);
        UltimateJoystick.ResetJoystick("Movement");
        runAnimation(1);
        calculateBonusSpeed();
    }
    IEnumerator cannotHurtDuringRevive()
    {
        canHurt = false;
        yield return new WaitForSeconds(3f);
        canHurt = true;
    }
    public void healPlayer(int amount)
    {
        int healHp = amount * realData.Hp * (100 + bonusPoints[10]) / 10000;

        if (currentHp + healHp >= realData.Hp)
        {
            currentHp = realData.Hp;
            healHp = realData.Hp - currentHp;
        }
        if (healHp > 0)
        {
            GameObject floatText = EasyObjectPool.instance.GetObjectFromPool("FloatingText", transform.position, transform.rotation);
            floatText.GetComponent<FloatingText>().healPlayer(healHp);
            currentHp += healHp;
        }
        float per = (float)currentHp / realData.Hp;
        hpText.text = currentHp.ToString();
        hpBar.GetComponent<Slider>().value = per;
        calculateBonusSpeed();
    }
    private Vector2 shootFollower(Transform en)
    {
        Vector2 vector = new Vector2(-gameObject.transform.position.x + en.transform.position.x, -gameObject.transform.position.y + en.transform.position.y);
        vector = vector.normalized;
        return vector;
    }
    private float calAngle(Transform en, Vector2 vector)
    {
        Vector2 cur = new Vector2(-1, 0);
        float y = gameObject.transform.position.y;
        float n = en.transform.position.y;
        float angle = 0;

        if (y <= n)
            angle = 2 * AngleTo(cur, vector);
        else
            angle = -2 * AngleTo(cur, vector);

        return angle;
    }
    private float AngleTo(Vector2 pos, Vector2 target)
    {
        Vector2 diference;
        if (target.x > pos.x)
            diference = target - pos;
        else
            diference = pos - target;
        return Vector2.Angle(Vector2.right, diference);
    }
    public void rootPlayer()
    {
        if (isPlayerRooted == false)
        {
            isPlayerRooted = true;
            StartCoroutine(slowSpeed(100));
        }
    }
    private IEnumerator slowSpeed(int percent)
    {
        if (realData.Move > 0)
        {
            int cacheSpeed = realData.Move;
            realData.Move = (100 - percent) * realData.Move / 100;
            yield return new WaitForSeconds(1f);
            realData.Move = cacheSpeed;
            isPlayerRooted = false;
        }
    }
    private IEnumerator slowPlayer(int percent)
    {
        if (isPlayerRooted == false)
        {
            isPlayerRooted = true;
            cacheSpeed = realData.Move;
            realData.Move = (100 - percent) * realData.Move / 100;
            Debug.Log("Move " + realData.Move);
            yield return new WaitForSeconds(3f);
            realData.Move = cacheSpeed;
            isPlayerRooted = false;
        }
    }
}
