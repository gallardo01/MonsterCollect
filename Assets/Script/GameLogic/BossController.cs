using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private int facingRight = 1;
    private float x = 0f, y = 0f;
    public float moveSpeed = 1f;
    private int waypointIndex = 0;
    private Vector2[] waypoints;
    private int wayMove = 1;
    private BoxCollider2D boxCollider2D;
    private bool isMove = true;
    private bool isCast = false;
    private Vector3 playerLastPos;
    public TextMeshPro level;
    private Transform player;
    private MonsterData monsterData;

    private int currentHp;
    // boss 1
    private float rate;
    private GameObject bossTargetGlobal;
    private bool isDead = false;
    //boss 2

    // boss 5
    private Vector3 bossDirection;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;

        //if (monsterData.Id == 10)
        //{
        //    bossTarget = GameObject.FindWithTag("Target_boss_1").transform;

        //}
        //else if (monsterData.Id == 40)
        //{
        //    bossTarget.gameObject = EasyObjectPool.instance.GetObjectFromPool("Gold", transform.position * 1.05f, transform.rotation);

        //}
        //bossTarget.gameObject.SetActive(false);

        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

        //boss 1
        rate = 2f;


        //boss 2


    }

    void Start()
    {
        initInfo(100);

        waypoints = new Vector2[]
        {
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
        };
        x = transform.position.x;
        y = transform.position.y;
        //init();
        runAnimation(2);
        StartCoroutine(castSkill());
    }

    public MonsterData getLevel()
    {
        return monsterData;
    }

    public void initInfo(int id)
    {
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        currentHp = monsterData.Hp;
        level.text = "Lv." + id.ToString();
    }

    public bool isRage()
    {
        return currentHp <= monsterData.Hp / 3;

    }
    public void enemyHurt(MyHeroes heroes, int damePercent)
    {
        if (!isDead)
        {
            int dame = MathController.Instance.playerHitEnemy(heroes, monsterData, damePercent);
            int typeValue = MathController.Instance.getTypeValue(heroes, monsterData);
            int actualDame = Mathf.Abs(dame);
            currentHp -= actualDame;
            //Debug.Log(currentHp);
            if (currentHp <= 0)
            {
                //GameController.Instance.initEatMonster(heroes.Level);
                // drop item
                dropItemController(monsterData.Id);
                isMove = false;
                setAction(2);
                isDead = true;
            }
            disableObject();
            string floatingText = "FloatingText";
            GameObject floatText = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
            floatText.GetComponent<FloatingText>().disableObject(dame, typeValue);
        }
    }

    public bool getIsDead()
    {
        return isDead;
    }

    public void setAction(int action)
    {
        // attack back
        if (action == 1)
        {
            runAnimation(3);
            isMove = false;
            StartCoroutine(setMove());
        }
        else if (action == 2) // dead
        {
            runAnimation(4);
            isMove = false;
        }
    }

    IEnumerator setMove()
    {
        yield return new WaitForSeconds(1f);
        isMove = true;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (isMove)
        {
            // lao vao nguoi choi
            if(wayMove == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                player.position,
                (moveSpeed / 4 * 3) * Time.deltaTime);

                //if (transform.position.x == waypoints[waypointIndex].x && transform.position.y == waypoints[waypointIndex].y)
                //{
                //    isMove = false;
                //    GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
                //    StartCoroutine(castSkill());
                //}

                faceToPlayer();
            }
            //nhay ra khoi man hinh
            else if (wayMove == 2)
            {
                boxCollider2D.enabled = false;
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x, 20f, 0f), (moveSpeed * 30) * Time.deltaTime);
            }
            //roi tu ngoai man hinh vao
            else if (wayMove == 3)
            {   
                boxCollider2D.enabled = true;
                transform.position = Vector2.MoveTowards(transform.position,
                        playerLastPos,
                        (moveSpeed * 50) * Time.deltaTime);
            }
            else if (wayMove == 4)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                bossTargetGlobal.transform.position,
                (moveSpeed / 4 * 30) * Time.deltaTime);
            }
            else if (wayMove == 5)
            {
                transform.position += bossDirection * moveSpeed * Time.deltaTime;
            } 

        }

    }

    IEnumerator castSkill()
    {
        if (monsterData.Id == 10)
        {
            GameObject bossTarget = EasyObjectPool.instance.GetObjectFromPool("Boss_1_Target", transform.position, transform.rotation);
            bossTargetGlobal = bossTarget;
            bossTarget.SetActive(false);

            if (isRage())
            {
                rate = 4f;
            }
            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 10);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                wayMove = 2;

                yield return new WaitForSeconds(1f);
                playerLastPos = player.position;

                bossTarget.transform.position = new Vector3(playerLastPos.x, playerLastPos.y - 1f, playerLastPos.z);
                bossTarget.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                wayMove = 3;
                bossTarget.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);
                wayMove = 1;
                isCast = false;
            }
        }
        else if (monsterData.Id == 20)
        {
            runAnimation(2);

            wayMove = 1;
            moveSpeed = 2f;


            for (int i = 0; i < 5; i++)
            {
                GameObject fileGround = EasyObjectPool.instance.GetObjectFromPool("Particle_Fire_2", transform.position, transform.rotation);
                StartCoroutine(returnObjectToPool(fileGround, 5f));
                yield return new WaitForSeconds(1f);
            }

            for (int i = 0; i < 10; i++)
            {
                moveSpeed -= 0.2f;
                yield return new WaitForSeconds(0.1f);

            }

            runAnimation(1);

            isMove = false;

            for (int i = 0; i < 4; i++)
            {
                flip();
                yield return new WaitForSeconds(0.75f);
            }

            //phut lua

            playerLastPos = player.position;

            for (int i = 0; i < 5; i++)
            {
                faceToPlayer();
                runAnimation(3);
                GameObject fileBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_fire_boss", transform.position, transform.rotation);

                Vector3 direction = Vector3.Normalize(player.position - transform.position);
                fileBullet.GetComponent<BulletOfBossComtroller>().initBullet(player, direction, 5f, 0, monsterData);


                float angle = calAngle(player.transform, direction);
                fileBullet.transform.Rotate(0, 0, angle);

                yield return new WaitForSeconds(1f);

            }


            isMove = true;

        }

        else if (monsterData.Id == 30)
        {
            runAnimation(2);
            wayMove = 1;

            moveSpeed = 1;

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 10);

            if (chance <= rate && !isCast)
            {
                isCast = true;

                runAnimation(3);

                GameObject fellow = EasyObjectPool.instance.GetObjectFromPool("Enemy14", transform.position, transform.rotation);
                fellow.GetComponent<MonsterController>().initData(14, false);
                fellow.GetComponent<MonsterController>().triggerWaypoints();


                if (isRage())
                {
                    moveSpeed = 0;
                    yield return new WaitForSeconds(1f);

                    runAnimation(3);
                    GameObject silkBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_silk_boss", transform.position, transform.rotation);

                    Vector3 direction = Vector3.Normalize(player.position - transform.position);
                    silkBullet.GetComponent<BulletOfBossComtroller>().initBullet(player, direction, 15f, 1, monsterData);

                    yield return new WaitForSeconds(1f);

                    moveSpeed = 1;


                }

                isCast = false;
            }
        }
        else if (monsterData.Id == 40)
        {
            GameObject bossTarget = EasyObjectPool.instance.GetObjectFromPool("Boss_4_Target", transform.position, transform.rotation);
            bossTargetGlobal = bossTarget;
            bossTarget.SetActive(false);

            List<GameObject> bossTargetSub = new List<GameObject>();

            for (int i = 0; i < 6; i++)
            {
                bossTargetSub.Add(EasyObjectPool.instance.GetObjectFromPool("Boss_4_Target", transform.position, transform.rotation));
            }

            for (int i = 0; i < 6; i++)
            {
                bossTargetSub[i].SetActive(false);
            }

            runAnimation(2);
            wayMove = 1;

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                if (isRage())
                {
                    moveSpeed = 0;
                    runAnimation(1);

                    yield return new WaitForSeconds(1f);
                    playerLastPos = player.position;

                    bossTarget.transform.position = new Vector3(playerLastPos.x, playerLastPos.y, playerLastPos.z);
                    bossTarget.gameObject.SetActive(true);

                    float mulNum = 3f;

                    bossTargetSub[1].transform.position = new Vector3(playerLastPos.x, playerLastPos.y + mulNum, playerLastPos.z);
                    bossTargetSub[2].transform.position = new Vector3(playerLastPos.x + mulNum, playerLastPos.y + mulNum * 0.6f, playerLastPos.z);
                    bossTargetSub[3].transform.position = new Vector3(playerLastPos.x + mulNum, playerLastPos.y - mulNum * 0.6f, playerLastPos.z);
                    bossTargetSub[4].transform.position = new Vector3(playerLastPos.x, playerLastPos.y - mulNum, playerLastPos.z);
                    bossTargetSub[5].transform.position = new Vector3(playerLastPos.x - mulNum, playerLastPos.y - mulNum * 0.6f, playerLastPos.z);
                    bossTargetSub[0].transform.position = new Vector3(playerLastPos.x - mulNum, playerLastPos.y + mulNum * 0.6f, playerLastPos.z);

                    for (int i = 0; i < 6; i++)
                    {
                        bossTargetSub[i].SetActive(true);
                    }
                    yield return new WaitForSeconds(2f);

                    runAnimation(3);
                    GameObject iceSpiked = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTarget.transform.position, transform.rotation);
                    iceSpiked.GetComponent<BulletOfBossComtroller>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
                    bossTarget.gameObject.SetActive(false);

                    for (int i = 0; i < 6; i++)
                    {
                        GameObject iceSpikedSub = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTargetSub[i].transform.position, transform.rotation);
                        iceSpikedSub.GetComponent<BulletOfBossComtroller>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
                        bossTargetSub[i].gameObject.SetActive(false);
                    }

                    yield return new WaitForSeconds(2f);
                    moveSpeed = 1;

                    wayMove = 4;
                    faceToỌbject(bossTarget.transform);

                    yield return new WaitForSeconds(2f);


                }
                else
                {
                    moveSpeed = 0;
                    runAnimation(1);

                    yield return new WaitForSeconds(1f);
                    playerLastPos = player.position;

                    bossTarget.transform.position = new Vector3(playerLastPos.x, playerLastPos.y, playerLastPos.z);
                    bossTarget.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2f);

                    runAnimation(3);
                    GameObject iceSpiked = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTarget.transform.position, transform.rotation);

                    iceSpiked.GetComponent<BulletOfBossComtroller>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
                    bossTarget.gameObject.SetActive(false);

                    yield return new WaitForSeconds(2f);
                    moveSpeed = 1;

                    wayMove = 4;
                    faceToỌbject(bossTarget.transform);

                    yield return new WaitForSeconds(2f);
                }


                isCast = false;
            }
        }
        else if (monsterData.Id == 50)
        {

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;
                wayMove = 5;
                moveSpeed = 0;
                runAnimation(3);
                yield return new WaitForSeconds(1f);
                runAnimation(2);


                for (int i = 0; i < 4; i++)
                {
                    bossDirection = Vector3.Normalize(player.position - transform.position);
                    moveSpeed = 5f;
                    yield return new WaitForSeconds(1f);
                    moveSpeed = 0;
                    yield return new WaitForSeconds(1f);
                }



                moveSpeed = 1;
                wayMove = 1;

                isCast = false;
            }
        }

        else if (monsterData.Id == 60)
        {

            wayMove = 1;

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;
                moveSpeed = 0;
                runAnimation(3);

                GameObject windFog = EasyObjectPool.instance.GetObjectFromPool("Boss_6_Target", transform.position, transform.rotation);
                Vector3 directionFog = Vector3.Normalize(player.position - transform.position);
                windFog.GetComponent<BulletOfBossComtroller>().initBullet(player, directionFog, 6f, 3, monsterData);


                // ban dan
                for (int i = 0; i < 5; i++)
                {
                    faceToPlayer();
                    runAnimation(3);
                    GameObject windBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_wind_boss", transform.position, transform.rotation);
                    Vector3 direction = Vector3.Normalize(player.position - transform.position);
                    windBullet.GetComponent<BulletOfBossComtroller>().initBullet(player, direction, 5f, 0, monsterData);

                    float angle = calAngle(player.transform, direction);
                    windBullet.transform.Rotate(0, 0, angle);

                    yield return new WaitForSeconds(1f);
                }

                moveSpeed = 1;
                isCast = false;
            }
        }

        else if (monsterData.Id == 70)
        {
            wayMove = 1;

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                // ban dan
                moveSpeed = 0;
         
                faceToPlayer();
                runAnimation(3);

                GameObject waterBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_water_boss", transform.position, transform.rotation);
                Vector3 direction = Vector3.Normalize(player.position - transform.position);
                waterBullet.GetComponent<BulletOfBossComtroller>().initBullet(player, direction, 6f, 4, monsterData);

                GameObject waterBullet_1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_water_boss", transform.position, transform.rotation);
                Vector3 direction_1 = Vector3.Normalize(player.position - transform.position) + new Vector3(0.4f,Random.Range(0, 0.3f),0);
                waterBullet_1.GetComponent<BulletOfBossComtroller>().initBullet(player, direction_1, 6f, 4, monsterData);

                GameObject waterBullet_2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_water_boss", transform.position, transform.rotation);
                Vector3 direction_2 = Vector3.Normalize(player.position - transform.position) + new Vector3(-0.4f, Random.Range(0, 0.3f), 0); ;
                waterBullet_2.GetComponent<BulletOfBossComtroller>().initBullet(player, direction_2, 6f, 4, monsterData);


                moveSpeed = 1;
                isCast = false;
            }

        }


        else if (monsterData.Id == 100)
        {
            wayMove = 1;

            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                // ban dan
                moveSpeed = 0;

                faceToPlayer();
                runAnimation(3);


                GameObject explosionBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_explosion_boss", transform.position, transform.rotation);
                Vector3 direction = Vector3.Normalize(player.position - transform.position);

                explosionBullet.GetComponent<BulletOfBossComtroller>().initBullet(player, direction, 6f, 5, monsterData);

                //GameObject explosionBullet_1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_explosion_boss", transform.position, transform.rotation);
                //Vector3 direction_1 = Vector3.Normalize(player.position - transform.position) + new Vector3(0.4f, Random.Range(0, 0.3f), 0);
                //explosionBullet_1.GetComponent<BulletOfBossComtroller>().initBullet(player, direction_1, 6f, 0, monsterData);

                //GameObject explosionBullet_2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_explosion_boss", transform.position, transform.rotation);
                //Vector3 direction_2 = Vector3.Normalize(player.position - transform.position) + new Vector3(-0.4f, Random.Range(0, 0.3f), 0); ;
                //explosionBullet_2.GetComponent<BulletOfBossComtroller>().initBullet(player, direction_2, 6f, 0, monsterData);




                moveSpeed = 1;
                isCast = false;
            }

        }

        StartCoroutine(castSkill());

    }


    IEnumerator returnObjectToPool(GameObject gameObject, float timer)
    {

        yield return new WaitForSeconds(timer);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);

    }


    void init()
    {
        if(Random.Range(0,4) % 2 == 0)
        {
            flip();
        }
        waypoints[0] = new Vector2(transform.position.x, transform.position.y);
        waypoints[1] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
        waypoints[2] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
        waypoints[3] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
    }

    private void flip()
    {
        facingRight = 1 - facingRight;
        Vector3 newScale = gameObject.transform.localScale;
        newScale.x *= -1;
        Vector3 newScale2 = level.gameObject.transform.localScale;
        newScale2.x *= -1;
        level.gameObject.transform.localScale = newScale2;
        gameObject.transform.localScale = newScale;
    }

    private void runAnimation(int pos)
    {
        //idle
        if (pos == 1)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
        }
        //move
        else if (pos == 2)
        {

            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");

        }
        //attack
        else if (pos == 3)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("attack", 0.5f, 1);
            StartCoroutine(replayAnimation());
        }
        // die
        else if (pos == 4)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("die", 1f, 1);
            StartCoroutine(disableObject());
        }
    }

    void faceToPlayer()
    {
        if (transform.position.x < player.position.x && facingRight == 0)
        {
            flip();
        }
        else if (transform.position.x > player.position.x && facingRight == 1)
        {
            flip();
        }
    }
    void faceToỌbject(Transform Obj)
    {
        if (transform.position.x < Obj.position.x && facingRight == 0)
        {
            flip();
        }
        else if (transform.position.x > Obj.position.x && facingRight == 1)
        {
            flip();
        }
    }


    IEnumerator replayAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        if (isMove)
        {
            runAnimation(2);
        }
        else
        {
            runAnimation(1);
        }
    }

    IEnumerator disableObject()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    private void dropItemController(int monsterLv)
    {
        GameObject goldObj = EasyObjectPool.instance.GetObjectFromPool("Gold", transform.position * 1.05f, transform.rotation);
        goldObj.GetComponent<ItemDropController>().setGold(Random.Range(100, 100 + monsterLv * 10));

        GameObject hpObj = EasyObjectPool.instance.GetObjectFromPool("Item", transform.position * 1.05f, transform.rotation);
        hpObj.GetComponent<ItemDropController>().setItem();
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
