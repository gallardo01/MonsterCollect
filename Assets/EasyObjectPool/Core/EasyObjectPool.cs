﻿/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MarchingBytes {

	[System.Serializable]
	public class PoolInfo {
		public string poolName;
		public GameObject prefab;
		public int poolSize;
		public bool fixedSize;
	}

	class Pool {
		private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

		private bool fixedSize;
		private GameObject poolObjectPrefab;
		private int poolSize;
		private string poolName;
		
		public Pool(string poolName, GameObject poolObjectPrefab, int initialCount, bool fixedSize) {
			this.poolName = poolName;
			this.poolObjectPrefab = poolObjectPrefab;
			this.poolSize = initialCount;
			this.fixedSize = fixedSize;
			//populate the pool
			for(int index = 0; index < initialCount; index++) {
				AddObjectToPool(NewObjectInstance());
			}
		}

		//o(1)
		private void AddObjectToPool(PoolObject po) {
			//add to pool
			po.gameObject.SetActive(false);
			availableObjStack.Push(po);
			po.isPooled = true;
		}
		
		private PoolObject NewObjectInstance() {
			GameObject go = (GameObject)GameObject.Instantiate(poolObjectPrefab);
			PoolObject po = go.GetComponent<PoolObject>();
			if(po == null) {
				po = go.AddComponent<PoolObject>();
			}
			//set name
			po.poolName = poolName;
			return po;
		}

		//o(1)
		public GameObject NextAvailableObject(Vector3 position, Quaternion rotation) {
			PoolObject po = null;
			if(availableObjStack.Count > 0) {
				po = availableObjStack.Pop();
			} else if(fixedSize == false) {
				//increment size var, this is for info purpose only
				poolSize++;
				//Debug.Log(string.Format("Growing pool {0}. New size: {1}",poolName,poolSize));
				//create new object
				po = NewObjectInstance();
			} else {
				Debug.LogWarning("No object available & cannot grow pool: " + poolName);
			}
			
			GameObject result = null;
			if(po != null) {
				po.isPooled = false;
				result = po.gameObject;
				result.SetActive(true);
				
				result.transform.position = position;
				result.transform.rotation = rotation;
			}
			
			return result;
		} 

		public bool isNewCreateObject()
        {
			if (availableObjStack.Count > 0)
			{
				return false;
			}
			return true;
		}

		//o(1)
		public void ReturnObjectToPool(PoolObject po) {
			
			if(poolName.Equals(po.poolName)) {
				
				/* we could have used availableObjStack.Contains(po) to check if this object is in pool.
				 * While that would have been more robust, it would have made this method O(n) 
				 */
				if(po.isPooled) {
					//Debug.LogWarning(po.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");	
				} else {
					AddObjectToPool(po);
				}
				
			} else {
				Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}",po.poolName,poolName));
			}
		}
	}

	/// <summary>
	/// Easy object pool.
	/// </summary>
	public class EasyObjectPool : MonoBehaviour {

		public static EasyObjectPool instance;
		[Header("Editing Pool Info value at runtime has no effect")]
		public PoolInfo[] poolInfo;

		//mapping of pool name vs list
		private Dictionary<string, Pool> poolDictionary  = new Dictionary<string, Pool>();
		private List<GameObject> listObj = new List<GameObject>();

		// Use this for initialization
		void Start () {
			//set instance
			instance = this;
			//check for duplicate names
			CheckForDuplicatePoolNames();
			//create pools
			CreatePools();
		}
		
		private void CheckForDuplicatePoolNames() {
			for (int index = 0; index < poolInfo.Length; index++) {
				string poolName = poolInfo[index].poolName;
				if(poolName.Length == 0) {
					Debug.LogError(string.Format("Pool {0} does not have a name!",index));
				}
				for (int internalIndex = index + 1; internalIndex < poolInfo.Length; internalIndex++) {
					if(poolName.Equals(poolInfo[internalIndex].poolName)) {
						Debug.LogError(string.Format("Pool {0} & {1} have the same name. Assign different names.", index, internalIndex));
					}
				}
			}
		}

		private void CreatePools() {
			foreach (PoolInfo currentPoolInfo in poolInfo) {
				
				Pool pool = new Pool(currentPoolInfo.poolName, currentPoolInfo.prefab, 
				                     currentPoolInfo.poolSize, currentPoolInfo.fixedSize);

				
				//Debug.Log("Creating pool: " + currentPoolInfo.poolName);
				//add to mapping dict
				poolDictionary[currentPoolInfo.poolName] = pool;
			}
		}


		/* Returns an available object from the pool 
		OR 
		null in case the pool does not have any object available & can grow size is false.
		*/
		public GameObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation) {
			GameObject result = null;
			
			if(poolDictionary.ContainsKey(poolName)) {
				Pool pool = poolDictionary[poolName];
                bool isCreateNew = false;
                if (pool.isNewCreateObject() == true)
                {
                    isCreateNew = true;
                }
                result = pool.NextAvailableObject(position,rotation);
                if (isCreateNew && result.tag == "Enemy" && result != null)
                {
                    GameObject vector2;
                    vector2 = result;
                    listObj.Add(vector2);
                }
				if (isCreateNew && result.tag == "Boss" && result != null)
				{
					GameObject vector2;
					vector2 = result;
					listObj.Add(vector2);
				}
				//scenario when no available object is found in pool
				if (result == null) {
					Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
				}
				
			} else {
				Debug.LogError("Invalid pool name specified: " + poolName);
			}
			
			return result;
		}

		public void ReturnObjectToPool(GameObject go) {
			PoolObject po = go.GetComponent<PoolObject>();
			if(po == null) {
				Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
			} else {
				if(poolDictionary.ContainsKey(po.poolName)) {
					Pool pool = poolDictionary[po.poolName];
					pool.ReturnObjectToPool(po);
				} else {
					Debug.LogWarning("No pool available with name: " + po.poolName);
				}
			}
		}

		private GameObject playerObj;

		public Transform getNearestHitPosition(GameObject player)
        {
			playerObj = player;
			float distance = float.MaxValue;
			int obj = -1;

			for (int i = 0; i < listObj.Count; i++)
            {
				Vector3 delta = player.transform.position - listObj[i].transform.position;
				if (listObj[i].tag == "Enemy")
				{
					if (listObj[i].activeInHierarchy == true && listObj[i].GetComponent<MonsterController>().getIsDead() == false
						&& delta.magnitude < 6f)
					{
						if (distance > delta.magnitude)
						{
							distance = delta.magnitude;
							obj = i;
						}
						if (delta.magnitude < 1f)
						{
							listObj[i].GetComponent<MonsterController>().triggerWaypoints();
						}
					}
				} else if(listObj[i].tag == "Boss")
                {
					if (listObj[i].activeInHierarchy == true && listObj[i].GetComponent<BossController>().getIsDead() == false
						&& delta.magnitude < 6f)
					{
						if (distance > delta.magnitude)
						{
							distance = delta.magnitude;
							obj = i;
						}
					}
				}
			}
			if(obj < 0)
            {
				return null;
            }
			return listObj[obj].transform;
        }

		public Transform getNearestExcludeGameObjectPosition(GameObject en)
		{
			if(en.tag == "Boss")
            {
				return null;
            }
			float distance = float.MaxValue;
			int obj = -1;

			for (int i = 0; i < listObj.Count; i++)
			{
				Vector3 delta = playerObj.transform.position - listObj[i].transform.position;
				Vector3 deltaPrevious = en.transform.position - listObj[i].transform.position;

				if (listObj[i].tag == "Enemy")
				{
					if (listObj[i].activeInHierarchy == true && listObj[i].GetComponent<MonsterController>().getIsDead() == false
						&& listObj[i].GetComponent<MonsterController>().getIdData() != en.GetComponent<MonsterController>().getIdData() && delta.magnitude < 6f)
					{
						if (distance > delta.magnitude && deltaPrevious.magnitude > 0.7f)
						{
							distance = delta.magnitude;
							obj = i;
						}
					}
				} else if(listObj[i].tag == "Boss")
                {
					return null;
				}
			}
			if (obj < 0)
			{
				return null;
			}
			return listObj[obj].transform;
		}

		public void getAllObjectInPosition(GameObject playerObject, int size, MyHeroes heroes, int dame)
		{
			float distance = 1.35f * (0.7f + (size - 1) * 0.2f) / 0.7f;

			for (int i = 0; i < listObj.Count; i++)
			{
				Vector3 delta = playerObject.transform.position - listObj[i].transform.position;
				if(listObj[i].tag == "Enemy")
                {
					if (listObj[i].activeInHierarchy == true && listObj[i].GetComponent<MonsterController>().getIsDead() == false
	&& delta.magnitude <= distance)
					{
						listObj[i].gameObject.GetComponent<MonsterController>().enemyHurt(heroes, dame);
						GameController.Instance.addParticle(listObj[i].gameObject, 4);
					}
				} else if (listObj[i].tag == "Boss")
                {
					if (listObj[i].activeInHierarchy == true && listObj[i].GetComponent<BossController>().getIsDead() == false
	&& delta.magnitude <= distance)
					{
						listObj[i].gameObject.GetComponent<BossController>().enemyHurt(heroes, dame);
						GameController.Instance.addParticle(listObj[i].gameObject, 4);
					}
				}
			}
		}

		public bool getObjAvailable()
		{
			for (int i = 0; i < listObj.Count; i++)
			{
				if (listObj[i].tag == "Enemy")
				{
					if (listObj[i].activeInHierarchy == true)
					{
						return true;
					}
				}
			}
			return false;
		}

	}
}
