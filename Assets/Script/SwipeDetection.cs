using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    public GameObject joystick;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touch = Input.GetTouch(0).position;
            joystick.transform.position = touch;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touch = Input.mousePosition;
            joystick.GetComponent<RectTransform>().anchoredPosition = touch;
        }
    }

}
