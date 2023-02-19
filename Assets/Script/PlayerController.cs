using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] GameObject body;
    [SerializeField] TextMeshPro levelText;
    [SerializeField] GameObject particle;

    [SerializeField] TextMeshPro hpText;
    [SerializeField] GameObject hpBar;

    [SerializeField] GameObject locate;
    [SerializeField] GameObject joystick;

    public GameObject runSmoke;
    public GameObject SmokePos;
    private int currentHp;
    private int bonusExp;
    private int bonusGold;

    private int facingRight = 1;
    private bool walk = true;
    private int playerLevel = 1;
    private bool canMove = true;
    private bool isAtk = false;
    private bool canHurt = true;
    private int exp = 0;
    private MyHeroes data;
    private MyHeroes realData;

    float timeSmoke = 0;
    public float timeSmokeWait = 1f;
    private bool isPause = false;
    private bool isActiveNonRepeat = true;
    private GameObject gameObjectNonRepeat;
    private int[] thunderType = { 1, 0, 0, 0, 0, 0 };
    private int[] grassType = { 1, 0, 0, 0, 0, 0 };
    private int[] waterType = { 1, 0, 0, 0, 0, 0 };
    private int[] fireType = { 1, 0, 0, 0, 0, 0 };

    public int[] dameSkill = { 0, 0, 0, 0, 0, 0 };
    public int[] skillLevel = { 1, 0, 0, 0, 0, 0 };
    private float[] timer = { 0, 0, 0, 0, 0, 0 };

    // 1.Atk 2.Hp 3.Armour 4.Move 5.Crit 6.Speed 7.SuperEffective 8.Gold 9.Exp 
    public int[] bonusPoints = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] buffLevel = { 0, 0, 0, 0, 0, 0, 0 };
    // Start is called before the first frame update
    void Start()
    {
        //Vector3 pos = new Vector3(Screen.width, Screen.height, 0);
        if (!PlayerPrefs.HasKey("Map"))
        {
            PlayerPrefs.SetInt("Map", 1);
        }
        playerLevel = (PlayerPrefs.GetInt("Map") - 1) * 10 + 1;
        initStart();
        levelText.text = playerLevel.ToString();
        updatePlayerData();
        for (int i = 0; i <= 4; i++) {
            attackMonster(i);
        }
    }
    public Transform getPosition()
    {
        return gameObject.transform;
    }
    public void initStart()
    {
        levelText.text = playerLevel.ToString();
        // pick con nao?
        int heroesId = 10;
        data = HeroesDatabase.Instance.fetchMyData(heroesId);
        realData = data;
        currentHp = data.Hp;

        hpText.text = currentHp.ToString();
        hpBar.transform.localScale = new Vector3(1f, 1f, 1f);

        for (int i = 0; i < 6; i++)
        {
            timer[i] = SkillDatabase.Instance.fetchSkillIndex(i + 1 + (realData.Type - 1) * 12).Timer;
            dameSkill[i] = SkillDatabase.Instance.fetchSkillIndex(i + 1 + (realData.Type - 1) * 12).Power;
        }
    }
    // 1.Atk 2.Hp 3.Armour 4.Move 5.Crit 6.Speed 7.SuperEffective 8.Gold 9.Exp 
    private void updatePlayerData()
    {
        realData.Atk = realData.Atk * (100 + bonusPoints[1]) / 100;
        realData.Hp = realData.Hp * (100 + bonusPoints[2]) / 100;
        realData.Armour = realData.Armour * (100 + bonusPoints[3]) / 100;
        realData.Move = realData.Move * (100 + bonusPoints[4]) / 100;
        realData.Crit = realData.Crit * (100 + bonusPoints[5]) / 100;
        realData.Speed = realData.Speed * (100 + bonusPoints[6]) / 100;
    }
    public void gainLv(int lv)
    {
        playerLevel = lv;
        levelText.text = playerLevel.ToString();
        StartCoroutine(runVFX());
    }

    IEnumerator runVFX()
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(1f);
        particle.SetActive(false);
    }
    public int getLevel()
    {
        return playerLevel;
    }
    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.position += new Vector3(UltimateJoystick.GetHorizontalAxis("Movement"),
            UltimateJoystick.GetVerticalAxis("Movement"), 0).normalized * (realData.Speed / 400) * Time.deltaTime;
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

    public void setDataSkill(int[] typeRaw, int[] skillLv)
    {
        for(int i = 1; i <= 4; i++)
        {
            if (typeRaw[i] > 0)
            {
                int idSkill = typeRaw[i] - 1;
                thunderType[idSkill] = 1;
                skillLevel[idSkill] = skillLv[i];
                if (timer[idSkill] == 0)
                {
                    attackMonster(idSkill);
                }
            }
        }
        for(int i = 0; i < 6; i++)
        {
            int skillId = (data.Type - 1) * 12 + i + 1;
            SkillData skillData = SkillDatabase.Instance.fetchSkillIndex(skillId);

            dameSkill[i] = skillData.Power * (80 + 20*skillLevel[i]) / 100;
            timer[i] = skillData.Timer - (float)(skillLevel[i] - 1) * skillData.Upgrade/1000;
        }
    }

    //buff
    public void setDataBuff(int[] typeRaw, int[] buffLv)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (typeRaw[i] > 0)
            {
                int idSkill = typeRaw[i] - 6;
                Debug.Log(idSkill + "  " + buffLevel[i]);
                buffLevel[idSkill] = buffLv[i];
            }
        }
        for (int i = 1; i <= 6; i++)
        {
            if (buffLevel[i] > 0)
            {
                int skillId = i + 6;
                SkillData skillData = SkillDatabase.Instance.fetchSkillIndex(skillId);
                bonusPoints[skillData.Timer] = skillData.Power * (100 + (buffLevel[i] - 1) * 50) / 100;
            }
        }
        updatePlayerData();
    }

    public void attackMonster(int id)
    {
        if(id == 0)
        {
            StartCoroutine(normalAttack());
        }
        else if(id == 1)
        {
            StartCoroutine(thunder_1());
        }
        else if(id == 2)
        {
            StartCoroutine(thunder_2());
        }
        else if(id == 3)
        {
            StartCoroutine(thunder_3());
        }
        else if(id == 4)
        {
            StartCoroutine(thunder_4());
        }
        else if(id == 5)
        {
            StartCoroutine(thunder_5());
        }
    }
    IEnumerator disableObject(float timer, GameObject obj)
    {
        yield return new WaitForSeconds(timer);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }
    private IEnumerator normalAttack()
    {
        int bonusTimer = realData.Speed / 100;
        if (bonusTimer > 50) bonusTimer = 50;
        yield return new WaitForSeconds(timer[0] * (100 - bonusTimer)/100);
        if (isPause == false && thunderType[0] > 0)
        {
            string bulletText = "Electric_1";
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, shootTarget.rotation);
                projectileNormal.GetComponent<BulletController>().initBullet(realData, 1, dameSkill[0], shootTarget);
                Vector2 vector = shootFollower(shootTarget);
                float angle = calAngle(shootTarget, vector);
                projectileNormal.transform.Rotate(0, 0, angle + 90);
                if (skillLevel[0] == 5)
                {
                    yield return new WaitForSeconds(0.1f);
                    GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, shootTarget.rotation);
                    projectileNormal1.GetComponent<BulletController>().initBullet(realData, 1, dameSkill[0], shootTarget);
                    projectileNormal1.transform.Rotate(0, 0, angle + 90);

                }
            }
        }
        StartCoroutine(normalAttack());
    }
    private IEnumerator thunder_1()
    {
        if (isPause == false && thunderType[1] > 0)
        {
            string bulletText = "Electric_2";
            Transform shootTargetObj = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            GameObject shootTarget = EasyObjectPool.instance.GetObjectFromPool("Empty", locate.transform.position,
    locate.transform.rotation);
            shootTarget.transform.position = shootTargetObj.position + new Vector3(giveRandomFloatNumber(0.3f, 1f), giveRandomFloatNumber(0.3f, 1f), 0);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.transform.rotation);
                projectileNormal.GetComponent<BulletController>().initBullet(realData, 1, dameSkill[1], shootTarget.transform);
            }
            StartCoroutine(disableObject(5f, shootTarget));
        }
        yield return new WaitForSeconds(timer[1]);
        StartCoroutine(thunder_1());
    }
    private IEnumerator thunder_2()
    {
        if (isPause == false && thunderType[2] > 0)
        {
            string bulletText = "Electric_3";

            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
gameObject.transform.rotation);
            projectileNormal.GetComponent<BulletFlyAround>().initBullet(realData, 1, dameSkill[2], gameObject.transform);
            yield return new WaitForSeconds(3.5f);
            EasyObjectPool.instance.ReturnObjectToPool(projectileNormal);
            projectileNormal.SetActive(false);
        }
        yield return new WaitForSeconds(timer[2]);
        StartCoroutine(thunder_2());
    }
    private IEnumerator thunder_3()
    {
        if (isPause == false && thunderType[3] > 0)
        {
            string bulletText = "Electric_4";

            Vector2 vector = new Vector2(-1f, 2f);
            Vector2 vector2 = new Vector2(1f, -2f);
            Vector2 vector3 = new Vector2(-1f, -2f);
            Vector2 vector4 = new Vector2(1f, 2f);

            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
            projectileNormal.GetComponent<BulletNoTargetController>().initBullet(realData, 3, dameSkill[3], gameObject.transform);
            projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * 100);
            if (skillLevel[3] > 1)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, dameSkill[3], gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector2 * 100);
            } 
            if(skillLevel[3] > 2)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, dameSkill[3], gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector3 * 100);
            }
            if (skillLevel[3] > 3)
            {
                GameObject projectileNormal1 = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position, locate.transform.rotation);
                projectileNormal1.GetComponent<BulletNoTargetController>().initBullet(realData, 3, dameSkill[3], gameObject.transform);
                projectileNormal1.GetComponent<Rigidbody2D>().AddForce(vector4 * 100);
            }
        }
        yield return new WaitForSeconds(timer[3]);
        StartCoroutine(thunder_3());
    }
    private IEnumerator thunder_4()
    {
        if (isPause == false && thunderType[4] > 0)
        {
            string bulletText = "Electric_5";
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.rotation);
                projectileNormal.GetComponent<BulletController>().initBullet(realData, 4, dameSkill[4], shootTarget);
            }
        }
        yield return new WaitForSeconds(timer[4]);
        StartCoroutine(thunder_4());
    }
    private IEnumerator thunder_5()
    {
        if (isPause == false && thunderType[5] > 0)
        {
            int size = skillLevel[5];
            string bulletText = "Electric_6";
            if (isActiveNonRepeat)
            {
                isActiveNonRepeat = false;
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, gameObject.transform.position,
    gameObject.transform.rotation);
                projectileNormal.transform.parent = gameObject.transform;
                projectileNormal.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                gameObjectNonRepeat = projectileNormal;
                float scaleNumber = 0.7f + (size - 1) * 0.2f;
                projectileNormal.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
                projectileNormal.GetComponent<BulletOnStayController>().initBullet(realData, size, dameSkill[5], gameObject);
            }
            else
            {
                float scaleNumber = 0.7f + (size - 1) * 0.2f;
                gameObjectNonRepeat.transform.localScale = new Vector3(scaleNumber, scaleNumber, scaleNumber);
                gameObjectNonRepeat.GetComponent<BulletOnStayController>().initBullet(realData, size, dameSkill[5], gameObject);
            }
        }
        yield return new WaitForSeconds(timer[5]);
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
    public void reviveUser()
    {

    }
    IEnumerator deadAnimation()
    {
        canMove = false;
        joystick.SetActive(false);
        yield return new WaitForSeconds(0.99f);
        GetComponent<DragonBones.UnityArmatureComponent>().animation.Stop();
        yield return new WaitForSeconds(2f);
        GameFlowController.Instance.userDeath();
        this.enabled = false;
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
        Vector3 newScale = body.transform.localScale;
        newScale.x *= -1;
        body.transform.localScale = newScale;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (canHurt)
            {
                MonsterData enemyLv = collision.gameObject.GetComponent<MonsterController>().getLevel();
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                collision.gameObject.GetComponent<MonsterController>().setAction(1);
            }

        }
        if (collision.gameObject.tag == "Boss")
        {
            if (canHurt)
            {
                //int enemyLv = collision.gameObject.GetComponent<BossController>().getLevel();
                canHurt = false;
                //StartCoroutine(setHurt(enemyLv));
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
            StopAllCoroutines();
            runAnimation(4);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(false);
            }
        }
        hpText.text = currentHp.ToString();
        float per = (float)currentHp / data.Hp;
        hpText.text = currentHp.ToString();
        hpBar.transform.localScale = new Vector3(per, 1f, 1f);
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
}
