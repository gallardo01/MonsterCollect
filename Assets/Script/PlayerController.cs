using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] TextMeshPro levelText;
    [SerializeField] GameObject bar;
    [SerializeField] int id;

    private int hp = 1000;
    private float speed = 100;
    private int facingRight = 1;
    private bool walk = true;
    private int level = 25;
    private float boundX;
    private float boundY;
    private bool canMove = true;
    private bool isAtk = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = new Vector3(Screen.width, Screen.height, 0);

        boundX = Camera.main.ScreenToWorldPoint(pos).x;
        boundY = Camera.main.ScreenToWorldPoint(pos).y;
    }

    public void init(int val)
    {
        level = val;
        levelText.text = val.ToString();
        
    }

    public int getLevel()
    {
        return level;
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
        Debug.Log("Run Animation: " + pos);
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
            if (level >= enemyLv) // kill
            {
                runAnimation(3);
                collision.gameObject.GetComponent<MonsterController>().setAction(2);
            } else
            {
                collision.gameObject.GetComponent<MonsterController>().setAction(1);
            }

        }
    }
}
