using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class ItemDropController : MonoBehaviour
{
    [SerializeField] GameObject goldBar;
    private GameObject target;
    bool isActive = true;
    bool isFlyBack = false;
    private int type = 0;
    private int exp = 0;
    private int gold = 0;
    private int percent = 0;
    private ItemInventory itemAward;
    private float speed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setExp(int ex)
    {
        type = 1;
        exp = ex;
    }

    public void setGold(int g)
    {
        type = 2;
        gold = g;
        target = GameObject.FindWithTag("GoldBar");
        speed = 1f;
    }
   
    public void setItem()
    {
        type = 3;
        itemAward = ItemDatabase.Instance.dropItem();
    }

    public void setHp(int p)
    {
        type = 4;
        percent = p;
    }

    public void setMagnet()
    {
        type = 5;
    }
    private void Update()
    {
        if (target != null && isFlyBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }

    }

    void OnEnable()
    {
        isActive = true;
        isFlyBack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isActive && type != 5 && type != 2)
        {
            target = collision.gameObject;
            StartCoroutine(pushOut(shootFollower(collision.transform)));

            if (type == 1)
            {
                GameController.Instance.gainExpChar(exp);
            }
            if (type == 4)
            {
                PlayerController.Instance.healPlayer(percent);
            }
            if (type == 3)
            {
                GameController.Instance.addItemToDb(itemAward);
            }
            isActive = false;
        }
        else if (collision.gameObject.tag == "Player" && isActive && type == 5)
        {
            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Reward");
            foreach (GameObject respawn in respawns)
            {
                respawn.GetComponent<ItemDropController>().activeAction();
            }
            StartCoroutine(pushOutMagnet(shootFollower(collision.transform)));
        }
        else if (collision.gameObject.tag == "Player" && isActive && type == 2)
        {
            target = GameObject.FindWithTag("GoldBar");
            speed = 1f;
            isFlyBack = true;
        }
        else if (collision.gameObject.tag == "Player" && (isFlyBack) && type != 2)
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "GoldBar" && isFlyBack)
        {
            GameController.Instance.updateGold(gold);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void activeAction()
    {
        if (isActive && type != 5)
        {
            if (type == 2)
            {
                target = GameObject.FindWithTag("GoldBar");
                isFlyBack = true;
            }
            else
            {
                target = GameObject.FindWithTag("Player"); ;
                StartCoroutine(pushOut(shootFollower(target.transform)));
            }
            if (type == 1)
            {
                GameController.Instance.gainExpChar(exp);
            }
            if (type == 4)
            {
                PlayerController.Instance.healPlayer(percent);
            }
            isActive = false;
        }
    }

    private Vector2 shootFollower(Transform en)
    {
        Vector2 vector = new Vector2(gameObject.transform.position.x - en.transform.position.x, gameObject.transform.position.y - en.transform.position.y);
        vector = vector.normalized;
        return vector;
    }
    IEnumerator pushOut(Vector2 vector)
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(vector * 350);
        yield return new WaitForSeconds(0.25f);
        isFlyBack = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    IEnumerator pushOutMagnet(Vector2 vector)
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(vector * 250);
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

}
