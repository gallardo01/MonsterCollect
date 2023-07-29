using DG.Tweening;
using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BulletBouncingController : MonoBehaviour
{
    private float width = 6f;
    private float height = 6f;

    public int type;
    Transform target;
    MyHeroes heroes;
    int damePercent;
    int bounce = 0;
    private bool isReflect = true;
    Transform direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (transform.position.x > (width + target.position.x) && bounce > 0 && isReflect)
            {
                isReflect = false;
                StartCoroutine(resumeReflect());
                bounce--;
                if (bounce <= 0)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Vector2 vector = new Vector2(-1f, Random.Range(-2f, 2f));
                direction = EasyObjectPool.instance.getRandomTargetPosition();
                if (direction != null)
                {
                    vector = calculateVector(this.transform, direction);
                }
                vector = vector.normalized;
                GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
            else if (transform.position.x < (-width + target.position.x) && bounce > 0 && isReflect)
            {
                isReflect = false;
                StartCoroutine(resumeReflect());
                bounce--;
                if (bounce <= 0)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }

                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Vector2 vector = new Vector2(1f, Random.Range(-2f, 2f));
                direction = EasyObjectPool.instance.getRandomTargetPosition();
                if (direction != null)
                {
                    vector = calculateVector(this.transform, direction);
                }
                vector = vector.normalized;
                GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
            else if (transform.position.y > (height + target.position.y) && bounce > 0 && isReflect)
            {
                isReflect = false;
                StartCoroutine(resumeReflect());
                bounce--;
                if (bounce <= 0)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Vector2 vector = new Vector2(Random.Range(-2f, 2f), -1f);
                direction = EasyObjectPool.instance.getRandomTargetPosition();
                if (direction != null)
                {
                    vector = calculateVector(this.transform, direction);
                }
                vector = vector.normalized;
                GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
            else if (transform.position.y < (-height + target.position.y) && bounce > 0 && isReflect)
            {
                isReflect = false;
                StartCoroutine(resumeReflect());
                bounce--;
                if (bounce <= 0)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Vector2 vector = new Vector2(Random.Range(-2f, 2f), 1f);
                direction = EasyObjectPool.instance.getRandomTargetPosition();
                if (direction != null)
                {
                    vector = calculateVector(this.transform, direction);
                }
                vector = vector.normalized;
                GetComponent<Rigidbody2D>().AddForce(vector * 500);
            }
        }
    }

    IEnumerator resumeReflect()
    {
        yield return new WaitForSeconds(0.05f);
        isReflect = true;
    } 

    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        target = enemy;
        heroes = myHeroes;
        type = heroes.Type;
        damePercent = dame;
        isReflect = true;
        bounce = 4;
        if (GameController.Instance.getBossSpawn())
        {
            height = 6f;
            width = height * Screen.width / Screen.height;
        } else
        {
            height = 9f;
            width = height * Screen.width / Screen.height;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss"))
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, damePercent);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, damePercent);
            }
            GameController.Instance.addParticleDefault(collision.gameObject, type);
        }
    }

    private Vector2 calculateVector(Transform a, Transform b)
    {
        return new Vector2(b.position.x - a.position.x, b.position.y - a.position.y);
    }

}
