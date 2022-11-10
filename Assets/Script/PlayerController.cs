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

    public GameObject runSmoke;
    public GameObject SmokePos;

    private int id;
    private int currentHp;
    private float currentSpeed;
    private int currentArmour;
    private int currentAtk;
    private int bonusExp;
    private int bonusGold;

    private int facingRight = 1;
    private bool walk = true;
    private int playerLevel = 1;
    private bool canMove = true;
    private bool isAtk = false;
    private bool canHurt = true;
    private int exp = 0;
    private HeroesData data;
    float timeSmoke = 0;
    public float timeSmokeWait = 1f;
    private bool isPause = false;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = new Vector3(Screen.width, Screen.height, 0);
        if (!PlayerPrefs.HasKey("Map"))
        {
            PlayerPrefs.SetInt("Map", 1);
        }
        playerLevel = (PlayerPrefs.GetInt("Map") - 1) * 10 + 1;
        initStart();
        levelText.text = playerLevel.ToString();

        attackMonster();
    }
    public Transform getPosition()
    {
        return gameObject.transform;
    }
    public void initStart()
    {
        levelText.text = playerLevel.ToString();
        int heroesId = 11;
        data = HeroesDatabase.Instance.fetchHeroesData(heroesId);
        currentHp = data.Hp;
        currentSpeed = data.Speed / 10;
        currentArmour = data.Armour;
        currentAtk = data.Atk;
        bonusExp = data.Crit;
        bonusGold = data.Spell;

        hpText.text = currentHp.ToString();
        hpBar.transform.localScale = new Vector3(1f, 1f, 1f);
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
            UltimateJoystick.GetVerticalAxis("Movement"), 0).normalized * (currentSpeed / 80) * Time.deltaTime;
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
                //var smoke = Instantiate(runSmoke,new Vector3(SmokePos.transform.position.x, SmokePos.transform.position.y, SmokePos.transform.position.z), SmokePos.transform.rotation);
                timeSmoke = 0;

            }
            else
            {
                timeSmoke += Time.deltaTime;
            }
        }
    }
  
    private void attackMonster()
    {
        StartCoroutine(normalAttack());
    }

    private IEnumerator normalAttack()
    {
        yield return new WaitForSeconds(1.5f);
        if (isPause == false)
        {
            string bulletText = "Electric_1";
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.rotation);
                Vector2 vector = shootFollower(shootTarget);
                float angle = calAngle(shootTarget, vector);
                projectileNormal.transform.Rotate(0, 0, angle + 90);
                projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
        }
        StartCoroutine(normalAttack());
    }

    private IEnumerator thunder_1()
    {
        yield return new WaitForSeconds(1.5f);
        if (isPause == false)
        {
            string bulletText = "Electric_1";
            Transform shootTarget = EasyObjectPool.instance.getNearestHitPosition(gameObject);
            if (shootTarget != null)
            {
                GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool(bulletText, locate.transform.position,
    shootTarget.rotation);
                Vector2 vector = shootFollower(shootTarget);
                float angle = calAngle(shootTarget, vector);
                projectileNormal.transform.Rotate(0, 0, angle + 90);

                projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
        }
        StartCoroutine(normalAttack());
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (canHurt)
            {
                int enemyLv = collision.gameObject.GetComponent<MonsterController>().getLevel();
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                collision.gameObject.GetComponent<MonsterController>().setAction(1);
            }

        }
        if (collision.gameObject.tag == "Boss")
        {
            if (canHurt)
            {
                int enemyLv = collision.gameObject.GetComponent<BossController>().getLevel();
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                collision.gameObject.GetComponent<BossController>().setAction(1);
            }
        }
    }
    IEnumerator setHurt(int level)
    {
        int dame = 50 + (level % 10) * 40 + (level / 10) * 100;
        int percent = calculateArmourReduce(currentArmour);
        dame = dame * (100 - percent) / 100;
        reduceHealth(dame);
        body.GetComponent<Animator>().SetTrigger("hurt");
        yield return new WaitForSeconds(1f);
        canHurt = true;
    }
    private void reduceHealth(int amount)
    {
        string floatingText = "FloatingText";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
        particle.GetComponent<FloatingText>().disableObject(amount);

        currentHp -= amount;

        if (currentHp <= 0)
        {
            currentHp = 0;
            // dead

        }
        else
        {
            hpText.text = currentHp.ToString();
            float per = currentHp / data.Hp;
            hpBar.transform.localScale = new Vector3(per, 1f, 1f);
        }
    }
    private int calculateArmourReduce(int armour)
    {
        int percent = 0;
        if (armour <= 15)
        {
            percent += armour;
        }
        if (armour <= 30)
        {
            percent += (int)((armour - 15) * 0.7);
        }
        if (armour <= 50)
        {
            percent += (int)((armour - 15) * 0.5);
        }
        if (armour > 50)
        {
            percent += (int)((armour - 15) * 0.3);
        }
        return percent;
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
        Vector2 diference = Vector2.zero;

        if (target.x > pos.x)
            diference = target - pos;
        else
            diference = pos - target;

        return Vector2.Angle(Vector2.right, diference);
    }
}
