using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public GameObject[] gameObjects;
    private void Start()
    {
        gameObjects[0].GetComponent<Animator>().SetTrigger("Attack");

    }
    public void Click()
    {
    }
    IEnumerator startone()
    {
        for (var i = 0; i < 2; ++i)
        {
            gameObjects[i].GetComponent<Animator>().SetTrigger("Attack");
            Debug.Log("mew1");
        }
        yield return new WaitForSeconds(0.1f);
        for (var i = 2; i < 4; ++i)
        {
            gameObjects[i].GetComponent<Animator>().SetTrigger("Attack");
            Debug.Log("mew2");
        }
        yield return new WaitForSeconds(0.1f);
        for (var i = 4; i < 6; ++i)
        {
            gameObjects[i].GetComponent<Animator>().SetTrigger("Attack");
            Debug.Log("mew3");
        }
    }
}
