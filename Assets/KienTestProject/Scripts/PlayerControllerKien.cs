using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerKien : MonoBehaviour
{
    public float speed = 10;
    public Vector2 jumpHeight = new Vector2(0,20);
    public Vector2 jumpDown = new Vector2(0, -20);

    public GameObject physic;

    private Rigidbody2D rb;
    private int direction = 1;
    private float dirY = 0;


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
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("jump");
            rb.AddForce(jumpHeight, ForceMode2D.Impulse);
            //dirY = 20;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.AddForce(jumpDown, ForceMode2D.Impulse);
            //dirY = 20;
        }
    }

    void FixedUpdate()
    {
        Vector3 tempScale = new Vector3(-direction, 1, 1);
        transform.localScale = tempScale;


        Vector3 tempVect = new Vector3(direction, 0, 0);
        tempVect = tempVect * speed * Time.deltaTime;
        //rb.MovePosition(rb.transform.position + tempVect);

        rb.AddForce(tempVect, ForceMode2D.Impulse);

    }
}
