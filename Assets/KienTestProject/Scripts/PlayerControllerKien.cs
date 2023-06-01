using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerControllerKien : MonoBehaviour
{
    public float speed = 10;
    public Vector2 jumpHeight = new Vector2(0,20);
    public Vector2 jumpDown = new Vector2(0, -20);
    public float boostSpeed = 1;

    public GameObject physic;
    public GameObject swordUp;
    public GameObject swordDown;

    private Rigidbody2D rb;
    private int direction = 1;
    private int lastDirection = -1;

    private bool canJump = true;
    private bool canBoost = true;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = -1;

            if (direction == lastDirection && canBoost && canJump)
            {
                StartCoroutine(boostPlayer());
            }
            lastDirection = direction;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;

            if (direction == lastDirection && canBoost && canJump)
            {
                StartCoroutine(boostPlayer());
            }
            lastDirection = direction;

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            Jump();
            //dirY = 20;
        }
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    rb.AddForce(jumpDown, ForceMode2D.Impulse);
        //    //dirY = 20;
        //}
    }

    void FixedUpdate()
    {
        Vector3 tempScale = new Vector3(-direction, 1, 1);
        transform.localScale = tempScale;

        Vector3 tempVect = new Vector3(direction, 0, 0);
        tempVect = tempVect * speed * Time.deltaTime * boostSpeed;
        //rb.MovePosition(rb.transform.position + tempVect);

        rb.AddForce(tempVect, ForceMode2D.Impulse);
    }


    IEnumerator boostPlayer()
    {
        Debug.Log("player boost");
        canBoost = false;
        boostSpeed = 2;
        yield return new WaitForSeconds(0.5f);
        boostSpeed = 1;
        yield return new WaitForSeconds(1.5f);

        canBoost = true;
    }




    public void Jump()
    {
        if (canJump)
        {
            canJump = false;
            rb.AddForce(jumpHeight, ForceMode2D.Impulse);

        }
        else
        {
            rb.AddForce(jumpDown, ForceMode2D.Impulse);
            swordUp.SetActive(false);
            swordDown.SetActive(true);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("player bi danh");
            rb.AddForce(jumpHeight, ForceMode2D.Impulse);

        }
        if (collision.gameObject.tag == "Route1")
        {
            canJump = true;
            swordUp.SetActive(true);
            swordDown.SetActive(false);
        }
        
    }
}
