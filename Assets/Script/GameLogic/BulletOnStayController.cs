using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletOnStayController : MonoBehaviour
{
    private MyHeroes heroes;
    [SerializeField] int id;
    // Start is called before the first frame update
    float SavedTime = 0f;
    float DelayTime = 1f;
    private GameObject playerObj;
    private int size = 1;
    private int skillDame;
    //private void Start()
    //{
    //    //StartCoroutine(disableCollider());
    //}

    void OnDisable()
    {
    }

    void OnEnable()
    {
        StartCoroutine(hurtEnemyAround());
    }

    IEnumerator hurtEnemyAround()
    {
        yield return new WaitForSeconds(1.5f);
        EasyObjectPool.instance.getAllObjectInPosition(playerObj, size, heroes, skillDame);
        StartCoroutine(hurtEnemyAround());
    }

    public void initBullet(MyHeroes myHeroes, int s, int dame, GameObject obj)
    {
        skillDame = dame;
        heroes = myHeroes;
        size = s;
        playerObj = obj;
    }

    public MyHeroes getHeroes()
    {
        return heroes;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "Wall")
        //{
        //    GameController.Instance.addParticle(collision.gameObject, 1);
        //    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        //    gameObject.SetActive(false);
        //}
        //if (collision.gameObject.tag == "Enemy" && id == 3)
        //{
        //    GameController.Instance.addExplosion(heroes, gameObject, 5);

        //    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        //    gameObject.SetActive(false);
        //}
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, skillDame);
            GameController.Instance.addParticle(collision.gameObject, 4);
        }
        if (collision.gameObject.tag == "Boss")
        {
            collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, skillDame);
            GameController.Instance.addParticle(collision.gameObject, 4);
        }

    }

    //private void OnTriggerStay2D(Collider2D[] collision)
    //{
    //    if ((Time.time - SavedTime) > DelayTime)
    //    {
    //        SavedTime = Time.time;
    //        foreach (Collider2D c in collision)
    //        {
    //            if (c.gameObject.tag == "Enemy")
    //            {
    //                Debug.Log("BBB");
    //                c.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
    //                GameController.Instance.addParticle(c.gameObject, 4);
    //            }
    //        }
    //    }
    //}

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
