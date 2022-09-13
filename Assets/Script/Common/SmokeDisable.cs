using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class SmokeDisable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(disableSelf());
    }

    public void disableSelf()
    {
        StartCoroutine(disable());
    }

    private IEnumerator disable()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
    }
}
