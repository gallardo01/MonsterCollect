using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletNoTargetController : MonoBehaviour
{
    private MyHeroes heroes;
    [SerializeField] int id;
    private Transform target;
    private int skillDame;
    // Start is called before the first frame update

    private void Start()
    {

    }

    void OnDisable()
    {
    }

    void OnEnable()
    {
 
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        skillDame = dame;
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
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 3)
        {
            GameController.Instance.addExplosion(heroes, gameObject, skillDame, 5);

            //collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
            //GameController.Instance.addParticle(collision.gameObject, 5);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    IEnumerator explosion(MyHeroes data)
    {
        yield return new WaitForSeconds(0);
        GameController.Instance.addExplosion(data, gameObject, skillDame, 3);
    }

    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
