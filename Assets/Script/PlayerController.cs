using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] GameObject body;
    [SerializeField] TextMeshPro levelText;
    [SerializeField] GameObject bar;
    
    private int id;
    private int hp = 1000;
    private float speed = 100;
    private int armour = 5;
    private int atk = 10;
    private int bonusExp = 5;
    private int bonusGold = 10;

    private int facingRight = 1;
    private bool walk = true;
    private int playerLevel = 1;
    private float boundX;
    private float boundY;
    private bool canMove = true;
    private bool isAtk = false;
    private bool canHurt = true;
    private int exp = 0;

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

    public void initStart()
    {
        levelText.text = playerLevel.ToString();
        int heroesId = 11;
        HeroesData data = HeroesDatabase.Instance.fetchHeroesData(heroesId);
        hp = data.Hp;
        speed = data.Speed;
        armour = data.Armour;
        atk = data.Atk;
        bonusExp = data.XpGain;
        bonusGold = data.GoldGain;
    }

    public void gainLv(int lv)
    {
        playerLevel = lv;
        levelText.text = playerLevel.ToString();
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
            UltimateJoystick.GetVerticalAxis("Movement"), 0).normalized * (speed / 80) * Time.deltaTime;
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
                StartCoroutine(setHurt());
                GameController.Instance.addParticle(gameObject, 2);
                collision.gameObject.GetComponent<MonsterController>().setAction(1);
            }

        }
    }
    IEnumerator setHurt()
    {
        yield return new WaitForSeconds(1f);
        canHurt = true;
    }
}
