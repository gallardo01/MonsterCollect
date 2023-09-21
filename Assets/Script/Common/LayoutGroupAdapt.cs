using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupAdapt : MonoBehaviour
{
    [SerializeField] GameObject layoutGroup;

    // Start is called before the first frame update
    void Start()
    {
        changeScreenSize();
    }

    private void changeScreenSize()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        int width = (int) canvas.GetComponent<RectTransform>().rect.width;

        int numberOfLine = (width - 125) / 275;
        int left = (width - 275*numberOfLine + 25)/2;
        layoutGroup.GetComponent<GridLayoutGroup>().padding = new RectOffset(left, left, 0, 50);
    }
}
