using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class ItemDropController : MonoBehaviour
{
    private GameObject target;
    bool isActive = true;
    bool isFlyBack = false;
    private int type = 0;
    private int exp = 0;
    private int gold = 0;
    private ItemInventory itemAward;
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
    }
   
    public void setItem(ItemInventory item)
    {
        type = 3;
        itemAward = item;
    }

    private void Update()
    {
        if (target != null && isFlyBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.035f);
        }
    }

    void OnEnable()
    {
        isActive = true;
        isFlyBack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isActive)
        {
            target = collision.gameObject;
            if (type == 1)
            {
                GameController.Instance.gainExpChar(exp);
            }
            isActive = false;
            StartCoroutine(pushOut(shootFollower(collision.transform)));
        }  
        if(collision.gameObject.tag == "Player" && isFlyBack)
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
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
}
