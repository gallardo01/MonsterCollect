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
            head.transform.position = Vector3.MoveTowards(head.transform.position, empty.transform.position, .1f);
            if (head.transform.position == empty.transform.position)
            {
                countBounce++;
                if (countBounce >= 6)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                if (countBounce % 2 == 0)
                {
                    empty.transform.position = new Vector3(playerTarget.position.x * 2 - gameObject.transform.position.x, playerTarget.position.y * 2 - gameObject.transform.position.y, 0f);
                }
                else
                {
                    empty.transform.position = firstPosition;
                }
            }
        } else if(countBounce > 0)
        {
            GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Particle_Water_4", transform.position, transform.rotation);
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
        empty = EasyObjectPool.instance.GetObjectFromPool("Empty", transform.position, transform.rotation);
        this.playerTarget = player.transform;
        head.GetComponent<BulletNoTargetController>().initBullet(myHeroes, dame);
        empty.transform.position = new Vector3(playerTarget.position.x * 2f - gameObject.transform.position.x, playerTarget.position.y * 2f - gameObject.transform.position.y, 0f);
    }


}
