using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletOfBossComtroller : MonoBehaviour
{

    private float speed = 5f;
    private Transform target;
    private Vector3 direction;
    private int type = 0;
    private MonsterData monsterData;

    private Vector3[] dir = {
        new Vector3(1, 0.6f, 0),
        new Vector3(1, -0.6f, 0),
        new Vector3(0, -1, 0),
        new Vector3(-1, -0.6f, 0),
        new Vector3(-1, 0.6f, 0),
        new Vector3(0, 1, 0),
    };



    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position += direction * speed *Time.deltaTime;
        }
    }

    public void initBullet(Transform Target, Vector3 Direction, float Speed, int type, MonsterData monsterData)
    {
        target = Target;
        speed = Speed;
        direction = Direction;
        this.type = type;
        this.monsterData = monsterData;
        //direction = Vector3.Normalize(target.position -Source.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().setPlayerHurt(monsterData, type);

            if (type == 1) // root bullet
            {
                speed = 0f;
                transform.position = collision.transform.position;

                StartCoroutine( returnToPool(1f));

            }
            else if (type == 2) // ice spike
            {


            }
            else
            {
                GameController.Instance.addParticle(collision.gameObject, 1);
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.tag == "Boss")
        {
            if (type == 2) // ice spike
            {
                int ran = 0;

                for (int i = 0; i < 6; i++)
                {
                    GameObject iceFrag = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_fragment", transform.position, transform.rotation);
                    //dir[i] = Quaternion.Euler(ran, ran, 0) * dir[i];

                    Vector3 temp = new Vector3(dir[i].x * Mathf.Cos(ran) + dir[i].y * Mathf.Sin(ran), -dir[i].x * Mathf.Sin(ran) + dir[i].y * Mathf.Cos(ran), 0);
                    Debug.Log(Vector3.Normalize(temp));

                    Vector3 direction = Vector3.Normalize(temp);
                    iceFrag.GetComponent<BulletOfBossComtroller>().initBullet(target, direction, 5f, 0, monsterData);

                    float angle = calAngle(temp, direction);
                    iceFrag.transform.Rotate(0, 0, angle);
                }

                GameController.Instance.addParticle(collision.gameObject, 1);
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);

                
            }
        }
    }



    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    private float calAngle(Vector3 en, Vector2 vector)
    {
        Vector2 cur = new Vector2(-1, 0);
        float y = gameObject.transform.position.y;
        float n = en.y;
        float angle = 0;

        if (y <= n)
        {
            Debug.Log("if");
            angle = 2 * AngleTo(cur, vector);
        }

        else
        {
            Debug.Log("else");

            angle = -2 * AngleTo(cur, vector);

        }

        return angle;
    }

    private float AngleTo(Vector2 pos, Vector2 target)
    {
        Vector2 diference;
        if (target.x > pos.x)
            diference = target - pos;
        else
            diference = pos - target;
        return Vector2.Angle(Vector2.right, diference);
    }

}
