using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveSelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(deactiveSelf());
    }

    IEnumerator deactiveSelf()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
