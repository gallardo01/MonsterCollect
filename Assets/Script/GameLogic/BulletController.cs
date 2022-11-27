using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletController : MonoBehaviour
{

    private int attack;
    private int crit;
    private int type;
    private MyHeroes heroes;
    [SerializeField] int id;
    private Transform target;

    // Start is called before the first frame update


    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .01f);
        }
    }

    public void initBullet(MyHeroes myHeroes, int skill, Transform enemy)
    {
        target = enemy;
        heroes = myHeroes;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        // thunder_1
        if (collision.gameObject.tag == "Enemy" && id == 11)
        {
            returnToPool(1f);
        }
    }
    
    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
