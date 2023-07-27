using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSnowmanController : MonoBehaviour
{
    [SerializeField] GameObject head;
    private int countBounce = 0;
    Transform playerTarget;
    GameObject empty;
    Vector3 firstPosition = new Vector3(0f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (empty.transform != null)
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, empty.transform.position, .07f);
            if (head.transform.position == empty.transform.position)
            {
                countBounce++;
                if (countBounce >= 7)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                if (countBounce % 2 == 1)
                {
                    empty.transform.position = getVector();
                } else
                {
                    empty.transform.position = firstPosition;
                }
            }
        } else if(countBounce > 0)
        {
            GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Particle_Water_4", transform.position, transform.rotation);
            particle.GetComponent<ParticleSystem>().Play();
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            EasyObjectPool.instance.ReturnObjectToPool(empty);
            empty.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void initBullet(MyHeroes myHeroes, int dame, GameObject player)
    {
        countBounce = 0;
        firstPosition = new Vector3(head.transform.position.x, head.transform.position.y, 0f);
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Particle_Water_4", transform.position, transform.rotation);
        particle.GetComponent<ParticleSystem>().Play();
        empty = EasyObjectPool.instance.GetObjectFromPool("Empty", transform.position, transform.rotation);
        this.playerTarget = player.transform;
        head.GetComponent<BulletNoTargetController>().initBullet(myHeroes, dame);
        empty.transform.position = new Vector3(transform.position.x + 0.03f, transform.position.y + -0.22f, 0f);
    }

    private Vector2 getVector()
    {
        int chance = Random.Range(1, 7);
        switch (chance)
        {
            case 1: return new Vector2(transform.position.x + 4f, transform.position.y + 0f);
            case 2: return new Vector2(transform.position.x + -4f, transform.position.y + 0f);
            case 3: return new Vector2(transform.position.x + 3f, transform.position.y + 3f);
            case 4: return new Vector2(transform.position.x + 3f, transform.position.y + -3f);
            case 5: return new Vector2(transform.position.x + -3f, transform.position.y + 3f);
            case 6: return new Vector2(transform.position.x + -3f, transform.position.y + -3f);
        }
        return Vector2.right;
    }
}
