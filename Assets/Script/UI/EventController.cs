using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EventController : MonoBehaviour
{
    [SerializeField] Button delete;
    // Start is called before the first frame update
    void Start()
    {
        delete.onClick.AddListener(() => deleteData());   
    }

    private void deleteData()
    {
        UserDatabase.Instance.deleteData();
        ItemDatabase.Instance.deleteData();
        HeroesDatabase.Instance.deleteData();
        PlayerPrefs.DeleteAll();
    }
}
