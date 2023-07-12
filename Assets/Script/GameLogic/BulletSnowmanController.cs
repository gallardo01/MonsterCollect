using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSnowmanController : MonoBehaviour
{
    [SerializeField] GameObject head;
    Transform target;
    private int countBounce = 0;
    Transform playerTarget;
    GameObject empty;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (target != null)
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, target.transform.position, .05f);
            if (head.transform.position == target.position)
            {
                countBounce++;
                if (countBounce >= 6)
                {
                    EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                    gameObject.SetActive(false);
                }
                if (countBounce % 2 == 0)
                {
                    target.position = new Vector3(playerTarget.position.x * 2 - gameObject.transform.position.x, playerTarget.position.y * 2 - gameObject.transform.position.y, 0f);
                }
                else
                {
                    target.position = new Vector3(0f, 1f, 0f);
                }
            }
        } else if(countBounce > 0)
        {
            GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Particle_Water_4", transform.position, transform.rotation);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void initBullet(MyHeroes myHeroes, int dame, GameObject player)
    {
        countBounce = 0;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Particle_Water_4", transform.position, transform.rotation);
        empty = EasyObjectPool.instance.GetObjectFromPool("Empty", transform.position, transform.rotation);
        this.playerTarget = player.transform;

        head.GetComponent<BulletNoTargetController>().initBullet(myHeroes, dame);
        target.position = new Vector3(playerTarget.position.x * 2f - gameObject.transform.position.x, playerTarget.position.y * 2f - gameObject.transform.position.y, 0f);
    }

    //IEnumerator findTarget()
    //{
    //    yield return new WaitForSeconds(2f);
    //    target.position = EasyObjectPool.instance.getNearestHitPosition(player.gameObject).position;
    //    if(target == null)
    //    {
    //        StartCoroutine(findTarget());
    //    }
    //}


}
