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
    private float boundX;
    private float boundY;
    private bool canMove = true;
    private bool isAtk = false;
    private bool canHurt = true;
    private int exp = 0;
    private HeroesData data;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = new Vector3(Screen.width, Screen.height, 0);
        playerLevel = (PlayerPrefs.GetInt("Map") - 1)* 10 + 1;
        boundX = Camera.main.ScreenToWorldPoint(pos).x;
        boundY = Camera.main.ScreenToWorldPoint(pos).y;
        initStart();
        levelText.text = playerLevel.ToString();
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
        currentSpeed = data.Speed;
        currentArmour = data.Armour;
        currentAtk = data.Atk;
        bonusExp = data.XpGain;
        bonusGold = data.GoldGain;

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
        if (canMove && (transform.position.x >= -boundX && transform.position.x <= boundX) 
            && (transform.position.y >= -boundY && transform.position.y <= boundY))
        {
            transform.position += new Vector3(UltimateJoystick.GetHorizontalAxis("Movement"),
            UltimateJoystick.GetVerticalAxis("Movement"), 0).normalized * (currentSpeed / 80) * Time.deltaTime;
        }

        if (transform.position.x <= -boundX)
        {
            transform.position = new Vector3(-boundX, transform.position.y, transform.position.z);
        }
        if (transform.position.x >= boundX)
        {
            transform.position = new Vector3(boundX, transform.position.y, transform.position.z);
        }
        if (transform.position.y <= -boundY)
        {
            transform.position = new Vector3(transform.position.x, -boundY, transform.position.z);
        }
        if (transform.position.y >= boundY)
        {
            transform.position = new Vector3(transform.position.x, boundY, transform.position.z);
        }


        if (UltimateJoystick.GetHorizontalAxis("Movement") > 0 && facingRight == 0)
        {
            flip();
        } else if (UltimateJoystick.GetHorizontalAxis("Movement") < 0 && facingRight == 1)
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
        } else
        {
            if (!walk)
            {
                walk = true;
                runAnimation(1);
            }
        }
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
        } else if (pos == 3) // atk
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
        } else
        {
            runAnimation(2);
        }
    }

    private void flip()
    {
        facingRight = 1 - facingRight;
        Vector3 newScale = body.transform.localScale;
        newScale.x *= -1;
        //Vector3 newScale2 = level.gameObject.transform.localScale;
        //newScale2.x *= -1;
        //level.gameObject.transform.localScale = newScale2;
        body.transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            int enemyLv = collision.gameObject.GetComponent<MonsterController>().getLevel();
            if (playerLevel >= enemyLv) // kill
            {
                runAnimation(3);
                GameController.Instance.initEatMonster(collision.gameObject.GetComponent<MonsterController>().getLevel());
                GameController.Instance.addParticle(collision.gameObject, 1);
                collision.gameObject.GetComponent<MonsterController>().setAction(2);
            } else if(canHurt)
            {
                canHurt = false;
                StartCoroutine(setHurt(enemyLv));
                //GameController.Instance.addParticle(gameObject, 2);
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
                //GameController.Instance.addParticle(gameObject, 2);
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
        if(armour <= 15)
        {
            percent += armour;
        } 
        if(armour <= 30)
        {
            percent += (int)((armour - 15) * 0.7);
        }
        if (armour <= 50)
        {
            percent += (int)((armour - 15) * 0.5);
        }
        if(armour > 50)
        {
            percent += (int)((armour - 15) * 0.3);
        }
        return percent;
    }
}
