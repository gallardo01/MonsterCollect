using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletController : MonoBehaviour
{
    private MyHeroes heroes;
    [SerializeField] int id;
    public Transform target;
    // Start is called before the first frame update
    private int bounce = 4;
    private float speed = 0.16f;
    private int damePercent;

    private void Start()
    {
       
    }

    void Update()
    {
        if (target != null && (target.tag == "Enemy"))
        {
            if (id != 1 && target.gameObject.GetComponent<MonsterController>().getIsDead() == false)
            {
                if (target != null && target.gameObject.activeInHierarchy && target.gameObject.GetComponent<MonsterController>().getIsDead() == false && bounce > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
                } else
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
            } else if (target.gameObject.GetComponent<MonsterController>().getIsDead() == true)
            {
                if (id == 4)
                {
                    target = EasyObjectPool.instance.getNearestExcludeGameObjectPosition(target.gameObject);
                }
                else
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
            } else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
            }
        } else if(target != null && (target.tag == "Boss" || target.tag == "Empty"))
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }
        else
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
    }

    void OnEnable()
    {
        if (id == 1)
        {
            StartCoroutine(disableCollider());
        }
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
    IEnumerator thunder_2()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(explosion(heroes));
        StartCoroutine(returnToPool(1.3f));
    }

    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        damePercent = dame;
        target = enemy;
        heroes = myHeroes;
        if(id == 4)
        {
            bounce = 4;
            speed = 0.16f;
        }

        if (skill == 1)
        {
            StartCoroutine(thunder_2());
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" && id != 1)
        {
            //GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 1 )
        {
            //collision.gameObject.GetComponent<MonsterController>().stopRunning();
        }
        else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 4) { 
            if (bounce > 0) {
                if (collision.gameObject.tag == "Enemy")
                {
                    collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, damePercent);
                } else
                {
                    collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, damePercent);
                }
                GameController.Instance.addParticle(collision.gameObject, 4);
                target = EasyObjectPool.instance.getNearestExcludeGameObjectPosition(target.gameObject);
                //isNextTarget = false;
                bounce--;
                if (target == null || target.gameObject.activeInHierarchy == false)
                {
                    bounce = 0;
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
            } else
            {
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }
        else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id != 1 && id != 4)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, damePercent);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, damePercent);
            }
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    IEnumerator explosion(MyHeroes data)
    {
        yield return new WaitForSeconds(0);
        GameController.Instance.addExplosion(data, gameObject, damePercent, 3);
    }

    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
