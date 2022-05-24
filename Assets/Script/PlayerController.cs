using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] TextMeshPro level;
    [SerializeField] GameObject bar;

    private int hp;
    private float speed = 100;
    private int facingRight = 1;
    private bool walk = true;

    private float boundX;
    private float boundY;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = new Vector3(Screen.width, Screen.height, 0);

        boundX = Camera.main.ScreenToWorldPoint(pos).x;
        boundY = Camera.main.ScreenToWorldPoint(pos).y;
    }

    private void getHp()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.x >= -boundX && transform.position.x <= boundX) && (transform.position.y >= -boundY && transform.position.y <= boundY))
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
        if (pos == 1)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
        }
        //move
        else if (pos == 2)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");
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
}
