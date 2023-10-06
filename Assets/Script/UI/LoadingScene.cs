using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelBusters.EssentialKit;

public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        Debug.Log("Waiting for cloud data...");
        yield return new WaitUntil(SyncService.Instance.HasRetrievedCloudData);
        yield return new WaitForSeconds(1f);
        if (SyncService.Instance.getSynchronizeStatus())
        {
            if (SyncService.Instance.getCloudStatus() == false)
            {
                for (int i = 0; i < 20; i++)
                {
                    CloudServices.Synchronize();
                    yield return new WaitForSeconds(0.5f);
                    if (SyncService.Instance.getCloudStatus())
                    {
                        break;
                    }
                }
            }
        } else
        {
            for (int i = 0; i < 10; i++)
            {
                CloudServices.Synchronize();
                yield return new WaitForSeconds(0.5f);
                if (SyncService.Instance.getCloudStatus())
                {
                    break;
                }
            }
        }
        ItemDatabase.Instance.LoadData();
        UserDatabase.Instance.LoadData();
        HeroesDatabase.Instance.LoadData();

        yield return new WaitForSeconds(1f);
        if (HeroesDatabase.Instance.returnCurrentHeroes() > 0)
        {
            AsyncOperation loadLevelOp = SceneManager.LoadSceneAsync("UI");
            while (!loadLevelOp.isDone)
            {
                // update progress bar amount to loadLevelOp.progress
                yield return null;
            }
        } else
        {
            AsyncOperation loadLevelOp = SceneManager.LoadSceneAsync("Tutorial");
            while (!loadLevelOp.isDone)
            {
                // update progress bar amount to loadLevelOp.progress
                yield return null;
            }
        }
    }
}
