using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;
using UnityEngine.UIElements;
using DG.Tweening;

public class BossController : Singleton<BossController>
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
    //boss 3
    float timeSmoke = 0;
    public float timeSmokeWait = 0.5f;
    // boss 5
    private Vector3 bossDirection;
    private GameObject SmokePos;
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
        //initInfo(90);

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
            if (heroes == null)
            {
                heroes = PlayerController.Instance.getRealData();
            }
            int dame;
            int type;
            if (heroes.Id / 10 == 6)
            {
                dame = MathController.Instance.playerHitEnemy(heroes, monsterData, damePercent, bonusLowHpPercent());
                type = MathController.Instance.getTypeValue(heroes, monsterData);
            }
            else
            {
                dame = MathController.Instance.playerHitEnemy(heroes, monsterData, damePercent);
                type = MathController.Instance.getTypeValue(heroes, monsterData);
            }
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
                GameController.Instance.endGame();
                UserDatabase.Instance.setLevelMap(monsterData.Id / 10 + 1);
            }
            GameController.Instance.updateHpBoss(currentHp, monsterData.Hp);
            disableObject();
            string floatingText = "FloatingText";
            GameObject floatText = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
            floatText.GetComponent<FloatingText>().disableObject(dame, type);
        }
    }
    private int bonusLowHpPercent()
    {
        int percentHp = currentHp * 100 / monsterData.Hp;
        if (percentHp >= 60 && percentHp <= 80)
        {
            return 10;
        }
        else if (percentHp >= 50 && percentHp <= 60)
        {
            return 20;
        }
        else if (percentHp >= 35 && percentHp <= 50)
        {
            return 30;
        }
        else if (percentHp >= 20 && percentHp <= 35)
        {
            return 40;
        }
        else if (percentHp < 20)
        {
            return 20;
        }
        return 0;
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
        yield return new WaitForSeconds(0.5f);
        isMove = true;
        StartCoroutine(replayAnimation());
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        if (wayMove == 6 || wayMove == 5 || wayMove ==4)
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

    private void Move()
    {
        if (isMove)
        {
            // lao vao nguoi choi
            if (wayMove == 1)
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
            // move to game object
            else if (wayMove == 4)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                bossTargetGlobal.transform.position,
                (moveSpeed / 4 * 30) * Time.deltaTime);
            }
            // move to 
            else if (wayMove == 5)
            {
                transform.position += bossDirection * moveSpeed * Time.deltaTime;
            }
            //move to vector
            else if (wayMove == 6)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                bossDirection, moveSpeed * Time.deltaTime);
            }

        }

    }

    IEnumerator castSkill()
    {
        if (monsterData.Id == 10)
        {
            //GameObject bossTarget = EasyObjectPool.instance.GetObjectFromPool("Boss_1_Target", transform.position, transform.rotation);
            //bossTargetGlobal = bossTarget;
            //bossTarget.SetActive(false);

            //GameObject bossJumpOut = EasyObjectPool.instance.GetObjectFromPool("boss_1_jump_out", transform.position, transform.rotation);
            //bossJumpOut.SetActive(false);

            //GameObject bossJumpIn = EasyObjectPool.instance.GetObjectFromPool("boss_1_jump_in", transform.position, transform.rotation);
            //bossJumpIn.GetComponent<BulletOfBossController>().initBullet(player, new Vector3(0, 0, 0), 0, 8, monsterData);
            //bossJumpIn.SetActive(false);

            //6s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);

            //8s nhay
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(2f);


            moveSpeed = 4;
            yield return new WaitForSeconds(1.5f);
            moveSpeed = 2;
            //jump
            //wayMove = 2;
            //bossJumpOut.transform.position = transform.position;
            //bossJumpOut.SetActive(true);

            //yield return new WaitForSeconds(1f);
            //playerLastPos = player.position;
            //bossJumpOut.SetActive(false);

            //bossTarget.transform.position = new Vector3(playerLastPos.x, playerLastPos.y -, playerLastPos.z);
            //bossTarget.gameObject.SetActive(true);
            //yield return new WaitForSeconds(1f);

            //wayMove = 3;
            //yield return new WaitForSeconds(0.3f);
            //bossTarget.gameObject.SetActive(false);
            //bossJumpIn.SetActive(true);
            //bossJumpIn.transform.position = bossTarget.transform.position;
            //yield return new WaitForSeconds(0.7f);
            //bossJumpIn.SetActive(false);

            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            //ban dan

            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(2f);

            moveSpeed = 0;

            Vector3 temp = player.position - transform.position;
            temp = Vector3.Normalize(temp);
            float angle1 = calAngle(temp);

            for (int i = 0; i < 2; i++)
            {
                faceToPlayer();
                runAnimation(3);

                GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_1", transform.position, transform.rotation);
                fireBullet.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 1.5f, 0, monsterData);
                fireBullet.transform.Rotate(0, 0, angle1);

                for (int j = -3; j < 4; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_1", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, j, 15);
                }

                yield return new WaitForSeconds(1f);

                faceToPlayer();
                runAnimation(3);

                for (int j = -3; j < 4; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_1", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, (float)(j + 0.5), 15);
                }

                yield return new WaitForSeconds(1f);
            }

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;

        }
        else if (monsterData.Id == 20)
        {
            //runAnimation(2);

            //wayMove = 1;
            //moveSpeed = 2f;

            //for (int i = 0; i < 5; i++)
            //{
            //    GameObject fileGround = EasyObjectPool.instance.GetObjectFromPool("Particle_Fire_2", transform.position, transform.rotation);
            //    fileGround.GetComponent<BulletOfBossController>().initBullet(player, new Vector3(0,0,0), 0f, 8, monsterData);
            //    StartCoroutine(returnObjectToPool(fileGround, 5f));
            //    yield return new WaitForSeconds(1f);
            //}

            //for (int i = 0; i < 10; i++)
            //{
            //    moveSpeed -= 0.2f;
            //    yield return new WaitForSeconds(0.1f);

            //}

            //runAnimation(1);

            //isMove = false;

            //for (int i = 0; i < 4; i++)
            //{
            //    flip();
            //    yield return new WaitForSeconds(0.75f);
            //}

            ////phut lua

            //playerLastPos = player.position;

            //for (int i = 0; i < 5; i++)
            //{
            //    faceToPlayer();
            //    runAnimation(3);
            //    GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_fire_boss", transform.position, transform.rotation);

            //    Vector3 direction = Vector3.Normalize(player.position - transform.position);
            //    fireBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 5f, 0, monsterData);


            //    float angle = calAngle(player.transform, direction);
            //    fireBullet.transform.Rotate(0, 0, angle);

            //    yield return new WaitForSeconds(1f);

            //}

            //isMove = true;


            //lao vao ben canh
            wayMove = 1;
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            SmokePos = this.transform.Find("SmokePos").transform.gameObject;
            wayMove = 6;
            if (transform.localScale.x < 0)
            {
                bossDirection = new Vector3(player.position.x - 0.5f, player.position.y, player.position.z);
            }
            else
            {
                bossDirection = new Vector3(player.position.x + 0.5f, player.position.y, player.position.z);

            }

            moveSpeed = 7;
            yield return new WaitForSeconds(1f);
            moveSpeed = 0;
            wayMove = 1;

            //phun lua
            faceToPlayer();
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("attack");
            yield return new WaitForSeconds(0.2f);
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Stop("attack");

            Vector3 temp = player.position - transform.position;
            GameObject fireBulletF = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_2_fire", transform.position, transform.rotation);
            //fireBulletF.transform.parent = transform.Find("FirePos");
            fireBulletF.transform.position = transform.Find("FirePos").position;
            if (transform.localScale.x < 0)
            {
                fireBulletF.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                fireBulletF.transform.localScale = new Vector3(-1, 1, 1);
            }
            fireBulletF.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 0f, 8, monsterData);
            yield return new WaitForSeconds(2f);

            //6s idle
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);

            //ban 8 huong
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(2f);
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            //Vector3 temp = player.position - transform.position;
            temp = Vector3.Normalize(temp);
            float angle1 = calAngle(temp);

            for (int i = 0; i < 4; i++)
            {
                runAnimation(5);

                GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_2", transform.position, transform.rotation);
                fireBullet.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 1.5f, 0, monsterData);
                fireBullet.transform.Rotate(0, 0, angle1);

                for (int j = 1; j < 8; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_2", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, j, 45);
                }

                yield return new WaitForSeconds(0.5f);

                for (int j = 1; j < 9; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_2", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, (float)(j + 0.5), 45);
                }

                yield return new WaitForSeconds(0.5f);
            }

            moveSpeed = 0;
            yield return new WaitForSeconds(3f);

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;
        }

        else if (monsterData.Id == 30)
        {
            //runAnimation(2);
            //wayMove = 1;

            //moveSpeed = 1;

            //yield return new WaitForSeconds(2f);
            //int chance = Random.Range(0, 10);

            //if (chance <= rate && !isCast)
            //{
            //    isCast = true;
            //    runAnimation(3);
            //    GameObject fellow = EasyObjectPool.instance.GetObjectFromPool("Enemy14", transform.position, transform.rotation);
            //    fellow.GetComponent<MonsterController>().initData(14, false);
            //    fellow.GetComponent<MonsterController>().triggerWaypoints();

            //    if (isRage())
            //    {
            //        moveSpeed = 0;
            //        yield return new WaitForSeconds(1f);

            //        runAnimation(3);
            //        GameObject silkBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_silk_boss", transform.position, transform.rotation);

            //        Vector3 direction = Vector3.Normalize(player.position - transform.position);
            //        silkBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 15f, 1, monsterData);

            //        yield return new WaitForSeconds(1f);

            //        moveSpeed = 1;


            //    }

            //    isCast = false;
            //}


            //6s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);

            //dung lai ban dan

            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            Vector3 temp = player.position - transform.position;
            temp = Vector3.Normalize(temp);
            float angle1 = calAngle(temp);

            for (int i = 0; i < 1; i++)
            {
                runAnimation(5);

                GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                fireBullet.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 1.5f, 0, monsterData);
                fireBullet.transform.Rotate(0, 0, angle1);

                GameObject fireBullet05 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                rotateBullet(fireBullet05, temp, angle1, (float)(0.3), 24);


                for (int j = 1; j < 15; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, j, 24);

                    GameObject fireBullet2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                    rotateBullet(fireBullet2, temp, angle1, (float)(j + 0.3), 24);

                    yield return new WaitForSeconds(0.01f);
                }

                //for (int j = 1; j < 16; j++)
                //{
                //    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                //    rotateBullet(fireBullet1, temp, angle1, (float)(j + 0.3), 24);
                //}

                yield return new WaitForSeconds(0.5f);
            }


            //lao vao tuong
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);
            wayMove = 5;
            bossDirection = Vector3.Normalize(player.position - transform.position);
            SmokePos = this.transform.Find("SmokePos").transform.gameObject;
            moveSpeed = 7f;
            yield return new WaitForSeconds(2f);
            moveSpeed = 0;
            yield return new WaitForSeconds(1f);

            moveSpeed = 2;
            yield return new WaitForSeconds(2f);
            // de quai con
            moveSpeed = 0;

            float tempFellow = 1f;
                runAnimation(3);
                GameObject fellow = EasyObjectPool.instance.GetObjectFromPool("Enemy28", new Vector3(transform.position.x + tempFellow, transform.position.y + tempFellow, transform.position.z), transform.rotation);
                fellow.GetComponent<MonsterController>().initData(28, false);
                fellow.GetComponent<MonsterController>().triggerWaypoints();

                GameObject fellow1 = EasyObjectPool.instance.GetObjectFromPool("Enemy28", new Vector3(transform.position.x - tempFellow, transform.position.y - tempFellow, transform.position.z), transform.rotation);
                fellow1.GetComponent<MonsterController>().initData(28, false);
                fellow1.GetComponent<MonsterController>().triggerWaypoints();

                GameObject fellow2 = EasyObjectPool.instance.GetObjectFromPool("Enemy28", new Vector3(transform.position.x + tempFellow, transform.position.y - tempFellow, transform.position.z), transform.rotation);
                fellow2.GetComponent<MonsterController>().initData(28, false);
                fellow2.GetComponent<MonsterController>().triggerWaypoints();

                GameObject fellow3 = EasyObjectPool.instance.GetObjectFromPool("Enemy28", new Vector3(transform.position.x - tempFellow, transform.position.y + tempFellow, transform.position.z), transform.rotation);
                fellow3.GetComponent<MonsterController>().initData(28, false);
                fellow3.GetComponent<MonsterController>().triggerWaypoints();


            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;

        }
        else if (monsterData.Id == 40)
        {
            GameObject bossTarget = EasyObjectPool.instance.GetObjectFromPool("Boss_4_Target", transform.position, transform.rotation);
            bossTargetGlobal = bossTarget;
            bossTarget.SetActive(false);

            SmokePos = this.transform.Find("SmokePos").transform.gameObject;

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

            moveSpeed = 0;
            runAnimation(1);

            yield return new WaitForSeconds(1f);
            playerLastPos = player.position;

            bossTarget.transform.position = new Vector3(playerLastPos.x, playerLastPos.y, playerLastPos.z);
            bossTarget.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);

            runAnimation(3);
            GameObject iceSpiked = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTarget.transform.position, transform.rotation);

            iceSpiked.GetComponent<BulletOfBossController>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
            bossTarget.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);
            moveSpeed = 1;

            wayMove = 4;
            faceToỌbject(bossTarget.transform);

            yield return new WaitForSeconds(2f);

            runAnimation(2);
            wayMove = 1;
            yield return new WaitForSeconds(3f);

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
            GameObject iceSpiked1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTarget.transform.position, transform.rotation);
            iceSpiked1.GetComponent<BulletOfBossController>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
            bossTarget.gameObject.SetActive(false);

            for (int i = 0; i < 6; i++)
            {
                GameObject iceSpikedSub = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_boss", bossTargetSub[i].transform.position, transform.rotation);
                iceSpikedSub.GetComponent<BulletOfBossController>().initBullet(player, Vector3.zero, 0f, 2, monsterData);
                bossTargetSub[i].gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(2f);
            moveSpeed = 1;

            wayMove = 4;
            faceToỌbject(bossTarget.transform);

            yield return new WaitForSeconds(2f);

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;
        }
        else if (monsterData.Id == 50)
        {

            //yield return new WaitForSeconds(2f);
            //int chance = Random.Range(0, 5);
            //if (chance <= rate && !isCast)
            //{
            //    isCast = true;
            //    wayMove = 5;
            //    moveSpeed = 0;
            //    runAnimation(3);
            //    yield return new WaitForSeconds(1f);
            //    runAnimation(2);


            //    for (int i = 0; i < 4; i++)
            //    {
            //        bossDirection = Vector3.Normalize(player.position - transform.position);
            //        moveSpeed = 4f;
            //        yield return new WaitForSeconds(1f);
            //        moveSpeed = 0;
            //        yield return new WaitForSeconds(1f);
            //    }



            //    moveSpeed = 1;
            //    wayMove = 1;

            //    isCast = false;
            //}

            SmokePos = this.transform.Find("SmokePos").transform.gameObject;

            // 2s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            // 6s duoi theo nguoi choi
            wayMove = 1;
            runAnimation(2);
            moveSpeed = 1.7f;
            yield return new WaitForSeconds(6f);

            //dung lai ban dan
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            Vector3 temp = player.position - transform.position;
            temp = Vector3.Normalize(temp);
            float angle1 = calAngle(temp);

            for (int i = 0; i < 1; i++)
            {
                runAnimation(5);

                GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                fireBullet.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 1.5f, 0, monsterData);
                fireBullet.transform.Rotate(0, 0, angle1);

                GameObject fireBullet05 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                rotateBullet(fireBullet05, temp, angle1, (float)(0.3), 24);


                for (int j = 1; j < 15; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, j, 24);

                    GameObject fireBullet2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_3", transform.position, transform.rotation);
                    rotateBullet(fireBullet2, temp, angle1, (float)(j + 0.3), 24);

                    yield return new WaitForSeconds(0.01f);
                }

                yield return new WaitForSeconds(0.5f);
            }

            //3s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);

            //lao ve huong nguoi choi
            wayMove = 5;
            runAnimation(2);
            for (int i = 0; i < 4; i++)
            {
                faceToPlayer();
                bossDirection = Vector3.Normalize(player.position - transform.position);
                moveSpeed = 4f;
                yield return new WaitForSeconds(1f);
                moveSpeed = 0;
                yield return new WaitForSeconds(1f);
            }

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;
        }

        else if (monsterData.Id == 60)
        {

            //wayMove = 1;

            //yield return new WaitForSeconds(2f);
            //int chance = Random.Range(0, 5);
            //if (chance <= rate && !isCast)
            //{
            //    isCast = true;
            //    moveSpeed = 0;
            //    runAnimation(3);

            //    GameObject windFog = EasyObjectPool.instance.GetObjectFromPool("Boss_6_Target", transform.position, transform.rotation);
            //    Vector3 directionFog = Vector3.Normalize(player.position - transform.position);
            //    windFog.GetComponent<BulletOfBossController>().initBullet(player, directionFog, 6f, 3, monsterData);


            //    // ban dan
            //    for (int i = 0; i < 5; i++)
            //    {
            //        faceToPlayer();
            //        runAnimation(3);
            //        GameObject windBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_wind_boss", transform.position, transform.rotation);
            //        Vector3 direction = Vector3.Normalize(player.position - transform.position);
            //        windBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 5f, 0, monsterData);

            //        float angle = calAngle(player.transform, direction);
            //        windBullet.transform.Rotate(0, 0, angle);

            //        yield return new WaitForSeconds(1f);
            //    }

            //    moveSpeed = 1;
            //    isCast = false;
            //}

            ////3s idle
            //wayMove = 1;
            //runAnimation(1);
            //moveSpeed = 0;
            //yield return new WaitForSeconds(3f);

            //dung lai ban dan
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            // ban dan
            for (int i = 0; i < 5; i++)
            {
                faceToPlayer();
                runAnimation(3);
                GameObject windBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_wind_boss", transform.position, transform.rotation);
                Vector3 direction = Vector3.Normalize(player.position - transform.position);
                windBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 5f, 10, monsterData);

                float angle = calAngle(player.transform, direction);
                windBullet.transform.Rotate(0, 0, angle);

                yield return new WaitForSeconds(1f);
            }

            //ban lam cham
            yield return new WaitForSeconds(2f);
            runAnimation(3);
            GameObject windFog = EasyObjectPool.instance.GetObjectFromPool("Boss_6_Target", transform.position, transform.rotation);
            Vector3 directionFog = Vector3.Normalize(player.position - transform.position);
            windFog.GetComponent<BulletOfBossController>().initBullet(player, directionFog, 6f, 3, monsterData);

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;

        }

        else if (monsterData.Id == 70)
        {
            //4s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(2f);
            //dung lai ban dan

            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            //Vector3 temp = player.position - transform.position;
            //temp = Vector3.Normalize(temp);
            //float angle1 = calAngle(temp);

            for (int i = 0; i < 5; i++)
            {
                faceToPlayer();
                runAnimation(3);
                GameObject windBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_7a", transform.position, transform.rotation);
                Vector3 direction = Vector3.Normalize(player.position - transform.position);
                windBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 5f, 11, monsterData);

                //float angle = calAngle(player.transform, direction);
                //windBullet.transform.Rotate(0, 0, angle);

                yield return new WaitForSeconds(1f);
            }

            //4s idle
            wayMove = 1;
            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(2f);

            //dung lai ban dan

            runAnimation(1);
            moveSpeed = 0;
            yield return new WaitForSeconds(2f);

            Vector3 temp = player.position - transform.position;
            temp = Vector3.Normalize(temp);
            float angle1 = calAngle(temp);

            for (int i = 0; i < 1; i++)
            {
                runAnimation(5);

                GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_7", transform.position, transform.rotation);
                fireBullet.GetComponent<BulletOfBossController>().initBulletNoTarget(temp, 1.5f, 0, monsterData);
                fireBullet.transform.Rotate(0, 0, angle1);

                GameObject fireBullet05 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_7", transform.position, transform.rotation);
                rotateBullet(fireBullet05, temp, angle1, (float)(0.3), 24);


                for (int j = 1; j < 15; j++)
                {
                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_7", transform.position, transform.rotation);
                    rotateBullet(fireBullet1, temp, angle1, j, 24);

                    GameObject fireBullet2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_boss_7", transform.position, transform.rotation);
                    rotateBullet(fireBullet2, temp, angle1, (float)(j + 0.3), 24);

                    yield return new WaitForSeconds(0.01f);
                }

                yield return new WaitForSeconds(0.5f);
                runAnimation(1);

            }

            //lao vao tuong
            runAnimation(2);
            moveSpeed = 2;
            yield return new WaitForSeconds(3f);
            moveSpeed = 0;
            yield return new WaitForSeconds(3f);
            wayMove = 5;
            bossDirection = Vector3.Normalize(player.position - transform.position);
            SmokePos = this.transform.Find("SmokePos").transform.gameObject;
            moveSpeed = 7f;
            yield return new WaitForSeconds(2f);
            moveSpeed = 0;
            yield return new WaitForSeconds(1f);

            moveSpeed = 2;
            yield return new WaitForSeconds(2f);

            if (Time.timeScale < 1.5f) Time.timeScale += 0.1f;

        }

        else if (monsterData.Id == 80)
        {
            wayMove = 1;
            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                GameObject fireRoundBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_fire_boss_2", transform.position, transform.rotation);
                fireRoundBullet.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 5f, 6, monsterData, 0f);

                GameObject fireRoundBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_fire_boss_2", transform.position, transform.rotation);
                fireRoundBullet1.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 5f, 6, monsterData, 2f);

                GameObject fireRoundBullet2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_fire_boss_2", transform.position, transform.rotation);
                fireRoundBullet2.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 5f, 6, monsterData, 4f);

                yield return new WaitForSeconds(5f);

                fireRoundBullet.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 6f, 0, monsterData, 0f);
                yield return new WaitForSeconds(.5f);

                fireRoundBullet1.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 6f, 0, monsterData, 0f);
                yield return new WaitForSeconds(.5f);

                fireRoundBullet2.GetComponent<BulletOfBossController>().initBullet(player, transform, Vector3.Normalize(player.position - transform.position), 6f, 0, monsterData, 0f);
                yield return new WaitForSeconds(.5f);


                isCast = false;
            }

        }

        else if (monsterData.Id == 90)
        {
            wayMove = 1;
            yield return new WaitForSeconds(2f);
            int chance = Random.Range(0, 5);
            if (chance <= rate && !isCast)
            {
                isCast = true;

                //phut 3 la

                for (int i = 0; i < 3; i++)
                {
                    faceToPlayer();
                    runAnimation(3);

                    GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_grass_boss", transform.position, transform.rotation);
                    Vector3 direction = Vector3.Normalize(player.position - transform.position);
                    fireBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 4f, 0, monsterData);
                    float angle = calAngle(player.transform, direction);
                    fireBullet.transform.Rotate(0, 0, angle);

                    GameObject fireBullet1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_grass_boss", transform.position, transform.rotation);
                    Vector3 direction1 = Vector3.Normalize(player.position - transform.position + new Vector3(3f, 0, 0));
                    fireBullet1.GetComponent<BulletOfBossController>().initBullet(player, direction1, 4f, 0, monsterData);
                    float angle1 = calAngle(player.transform, direction1);
                    fireBullet1.transform.Rotate(0, 0, angle1);

                    GameObject fireBullet2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_grass_boss", transform.position, transform.rotation);
                    Vector3 direction2 = Vector3.Normalize(player.position - transform.position + new Vector3(-6f, 0, 0));
                    fireBullet2.GetComponent<BulletOfBossController>().initBullet(player, direction2, 4f, 0, monsterData);
                    float angle2 = calAngle(player.transform, direction2);
                    fireBullet2.transform.Rotate(0, 0, angle2);

                    yield return new WaitForSeconds(1f);

                }

                //GameObject fireBullet = EasyObjectPool.instance.GetObjectFromPool("Bullet_grass_boss_2", transform.position, transform.rotation);
                //Vector3 direction = Vector3.Normalize(player.position - transform.position);
                //fireBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 4f, 7, monsterData);


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

                explosionBullet.GetComponent<BulletOfBossController>().initBullet(player, direction, 6f, 5, monsterData);

                //GameObject explosionBullet_1 = EasyObjectPool.instance.GetObjectFromPool("Bullet_explosion_boss", transform.position, transform.rotation);
                //Vector3 direction_1 = Vector3.Normalize(player.position - transform.position) + new Vector3(0.4f, Random.Range(0, 0.3f), 0);
                //explosionBullet_1.GetComponent<BulletOfBossController>().initBullet(player, direction_1, 6f, 0, monsterData);

                //GameObject explosionBullet_2 = EasyObjectPool.instance.GetObjectFromPool("Bullet_explosion_boss", transform.position, transform.rotation);
                //Vector3 direction_2 = Vector3.Normalize(player.position - transform.position) + new Vector3(-0.4f, Random.Range(0, 0.3f), 0); ;
                //explosionBullet_2.GetComponent<BulletOfBossController>().initBullet(player, direction_2, 6f, 0, monsterData);
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
        if (Random.Range(0, 4) % 2 == 0)
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
        else if (pos == 5)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("attack");

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
        GameObject goldObj = EasyObjectPool.instance.GetObjectFromPool("Gold", transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), transform.rotation);
        goldObj.GetComponent<ItemDropController>().setGold(Random.Range(100, 100 + monsterLv * 10));

        GameObject itemObj = EasyObjectPool.instance.GetObjectFromPool("Item", transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), transform.rotation);
        itemObj.GetComponent<ItemDropController>().setItem();

        if (Random.Range(0, 100) < 50)
        {
            GameObject itemObj2 = EasyObjectPool.instance.GetObjectFromPool("Item", transform.position + new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), 0f), transform.rotation);
            itemObj2.GetComponent<ItemDropController>().setItem();
        }
        if (Random.Range(0, 100) < 30)
        {
            GameObject itemObj3 = EasyObjectPool.instance.GetObjectFromPool("Item", transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), transform.rotation);
            itemObj3.GetComponent<ItemDropController>().setItem();
        }
    }

    private float calAngle(Transform en, Vector2 vector)
    {
        Vector2 cur = new Vector2(1, 0);
        float y = gameObject.transform.position.y;
        float n = en.transform.position.y;
        float angle = 0;

        if (y <= n)
            angle = 2 * AngleTo(cur, vector);
        else
            angle = -2 * AngleTo(cur, vector);

        return angle;
    }

    private float calAngle(Vector2 vector)
    {

        Vector2 cur = new Vector2(1, 0);
        float y = vector.y;
        float n = 0;
        float angle = 0;
        if (y >= n)
            angle = Vector2.Angle(cur, vector);
        else
            angle = -Vector2.Angle(cur, vector);

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

    private void rotateBullet(GameObject bullet, Vector3 dir, float angle, float shape, float dif)
    {
        Vector3 direction = Quaternion.AngleAxis(dif * shape, Vector3.forward) * dir;
        direction = Vector3.Normalize(direction);
        bullet.GetComponent<BulletOfBossController>().initBulletNoTarget(direction, 1.5f, 0, monsterData);
        bullet.transform.Rotate(0, 0, angle + shape * dif);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            runAnimation(2);
            wayMove = 1;
            moveSpeed = 0;
        }
    }
}
