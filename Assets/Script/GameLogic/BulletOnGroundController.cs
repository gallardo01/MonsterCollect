using DG.Tweening.Core.Easing;
using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOnGroundController : MonoBehaviour
{
    Transform target;
    MyHeroes heroes;
    int damePercent;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .2f);
        }
        if (transform.position == target.position)
        {
            explosion(heroes, "Particle_Fire_2");
            EasyObjectPool.instance.ReturnObjectToPool(target.gameObject);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            target.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }


    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        target = enemy;
        heroes = myHeroes;
        damePercent = dame;
    }
    private void explosion(MyHeroes data, string particle)
    {
        GameController.Instance.addExplosionText(data, gameObject, damePercent, particle);
    }

}
