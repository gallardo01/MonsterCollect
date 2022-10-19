using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletController : MonoBehaviour
{

    private int attack;
    private int crit;
    private int type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initBullet(int attack, int crit, int type)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            StartCoroutine(disableObject());
        }
        //if (collision.gameObject.tag == "Enemy")
        //{
        //    //collision.gameObject.GetComponent<MonsterController>().
        //    StartCoroutine(disableObject());
        //}
    }

    IEnumerator disableObject()
    {
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }

}
