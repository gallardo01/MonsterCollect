using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitBulletController : MonoBehaviour
{
    private Transform target;
    private MyHeroes heroes;
    private int skillDame;


    public void initBullet(MyHeroes myHeroes, int dame)
    {
        GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
        par.GetComponent<ParticleSystem>().Play();
        skillDame = dame;
        heroes = myHeroes;
        StartCoroutine(hitEnemy());
    }

    IEnumerator hitEnemy()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 vector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            target = EasyObjectPool.instance.getRandomTargetPosition();
            if(target != null)
            {
                vector = shootFollower(target);
            } else
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            vector = vector.normalized;
            float angle = calAngle(target, vector);

            GameObject projectileNormal = EasyObjectPool.instance.GetObjectFromPool("Grass_7", transform.position, transform.rotation);
            projectileNormal.GetComponent<BulletNoTargetController>().initBullet(heroes, skillDame);
            projectileNormal.GetComponent<Rigidbody2D>().AddForce(vector * 350f);
            projectileNormal.transform.Rotate(0, 0, angle - 90);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
        par.GetComponent<ParticleSystem>().Play();
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    private float calAngle(Transform en, Vector2 vector)
    {
        Vector2 cur = new Vector2(-1, 0);
        float y = gameObject.transform.position.y;
        float n = en.transform.position.y;
        float angle = 0;

        if (y <= n)
            angle = 2 * AngleTo(cur, vector);
        else
            angle = -2 * AngleTo(cur, vector);

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
    private Vector2 shootFollower(Transform en)
    {
        Vector2 vector = new Vector2(-gameObject.transform.position.x + en.transform.position.x, -gameObject.transform.position.y + en.transform.position.y);
        vector = vector.normalized;
        return vector;
    }
}
