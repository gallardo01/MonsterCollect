﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshPro text;

    private void Awake()
    {
        text.gameObject.transform.position = new Vector3(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f), 0f);
    }

    public void disableObject(int dame)
    {
        int actualDame = Mathf.Abs(dame);
        if (dame > 0)
        {
            text.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
        }
        text.text = actualDame.ToString();
        StartCoroutine(disableFloating());
    }

    private IEnumerator disableFloating()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
