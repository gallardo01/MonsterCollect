using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BingoSkillMatix : Singleton<BingoSkillMatix>
{

    // Start is called before the first frame update

    public GameObject itemSkillPrefab;

    public int[,] BingoBroad = {
        { 1, 1, 1, 1 },
        { 1, 1, 1, 1 },
        { 1, 1, 1, 1 },
        { 1, 1, 1, 1 }
    }; // 0 = lock; 1 = claimable; 2 = claimed

    //public GameObject[] itemSkill;

    //public GameObject[,] itemSkill;

    public List<BingoSkillItem> itemSkillList = new List<BingoSkillItem>();

    private int[,] partPosX =
    {
        {1,0,1 },
        {0,1,0 },
        {1,0,1 },
        {1,0,1 }
    };

    private int[,] partPosY =
    {
        {1,1,1,1 },
        {0,1,1,0 },
        {1,0,0,1 },
    };


    private bool isFirstTimeUpgrade = false;
    private bool isBingoVertical = false;
    private bool isBingoHorizontal = false;
    void Start()
    {
        initItemSkill();
        //Debug.Log(partPosY[1, 0]);
        //Debug.Log(partPosY[1, 1]);
        //Debug.Log(partPosY[2, 2]);
        //Debug.Log(partPosY[2, 3]);


    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void initItemSkill()
    {
        for (int i = 0; i < BingoBroad.GetLength(0); i++)
        {
            for(int j = 0; j< BingoBroad.GetLength(1); j++)
            {
                GameObject item;

                item = Instantiate(itemSkillPrefab, this.transform);

                item.GetComponent<BingoSkillItem>().posX = i;
                item.GetComponent<BingoSkillItem>().posY = j;
                item.GetComponent<BingoSkillItem>().value = BingoBroad[i, j];
                item.GetComponent<BingoSkillItem>().type = Random.Range(0, 4);

                //itemSkill[i* BingoBroad.GetLength(0) + j] = item;
                //ItemSkill itemSkill = new ItemSkill();
                //itemSkill.x = i;
                //itemSkill.y = j;
                //itemSkill.item = item;
                //itemSkill.value = BingoBroad[i,j];

                itemSkillList.Add(item.GetComponent<BingoSkillItem>());
            }
        }
    }

    public void clickOnItem(int x, int y)
    {
        if (!isFirstTimeUpgrade)
        {
            isFirstTimeUpgrade = true;
            for (int i = 0; i < BingoBroad.GetLength(0); i++)
            {
                for (int j = 0; j < BingoBroad.GetLength(1); j++)
                {
                    BingoBroad[i, j] = 0;

                    itemSkillList[i * BingoBroad.GetLength(0) + j].value = 0;                      
                }
            }
            BingoBroad[x, y] = 2;
            for (int i = 0; i < itemSkillList.Count; i++)
            {
                if (itemSkillList[i].posX == x && itemSkillList[i].posY == y){
                    itemSkillList[i].value = 2;
                }
            }
                
            checkCanUpgrade(x, y);

        }
        else
        {
            if (BingoBroad[x, y] == 1)
            {
                BingoBroad[x, y] = 2;
                for (int i = 0; i < itemSkillList.Count; i++)
                {
                    if (itemSkillList[i].posX == x && itemSkillList[i].posY == y)
                    {
                        itemSkillList[i].value = 2;
                    }
                }
                checkCanUpgrade(x, y);

            }

        }


        if (!isBingoVertical)
        {
            isBingoVertical = checkBingoVertical();
            if (isBingoVertical) claimVertivalAward();
        }
        if (!isBingoHorizontal) {
            isBingoHorizontal = checkBingoHorizontal();
            if(isBingoHorizontal) claimHorizontalAward();
        }

    }

    private void checkCanUpgrade(int x, int y)
    {
       

        //check cot
        
        if (y == 0 )
        {
            if (BingoBroad[x, y + 1] == 0)
            {
                BingoBroad[x, y + 1] = partPosY[y, x];
                itemSkillList[(x) * BingoBroad.GetLength(0) + y + 1].value = BingoBroad[x, y + 1];
            }

        }
        else if (y==3)
        {
            if (BingoBroad[x, y - 1] == 0)
            {
                BingoBroad[x, y - 1] = partPosY[y - 1, x];
                itemSkillList[(x) * BingoBroad.GetLength(0) + y - 1].value = BingoBroad[x, y - 1];
            }

        }else
        {
            if (BingoBroad[x, y + 1] == 0)
            {
                BingoBroad[x, y + 1] = partPosY[y, x];
                itemSkillList[(x) * BingoBroad.GetLength(0) + y + 1].value = BingoBroad[x, y + 1];
            }
            if (BingoBroad[x, y - 1] == 0)
            {
                BingoBroad[x, y - 1] = partPosY[y - 1, x];
                itemSkillList[(x) * BingoBroad.GetLength(0) + y - 1].value = BingoBroad[x, y - 1];
            }
        }

        // check hang


        if (x == 0)
        {
            if (BingoBroad[x + 1, y] == 0)
            {
                BingoBroad[x + 1, y] = partPosX[y, x];
                itemSkillList[(x + 1) * BingoBroad.GetLength(0) + y].value = BingoBroad[x + 1, y];
            }

        }
        else if (x == 3)
        {
            if (BingoBroad[x - 1, y] == 0)
            {
                BingoBroad[x - 1, y] = partPosX[y, x - 1];
                itemSkillList[(x - 1) * BingoBroad.GetLength(0) + y].value = BingoBroad[x - 1, y];
            }

        }
        else
        {
            if (BingoBroad[x + 1, y] == 0)
            {
                BingoBroad[x + 1, y] = partPosX[y, x];
                itemSkillList[(x + 1) * BingoBroad.GetLength(0) + y].value = BingoBroad[x + 1, y];
            }
            if (BingoBroad[x - 1, y] == 0)
            {
                BingoBroad[x - 1, y] = partPosX[y, x - 1];
                itemSkillList[(x - 1) * BingoBroad.GetLength(0) + y].value = BingoBroad[x - 1, y];
            }
        }


        for (int i = 0; i < itemSkillList.Count; i++)
        {
            if (itemSkillList[i].value == 2)
            {
                itemSkillList[i].gameObject.GetComponent<Image>().color = Color.blue;
            }else if (itemSkillList[i].value == 0)
            {
                itemSkillList[i].gameObject.GetComponent<Image>().color = Color.gray;
            }
            else if(itemSkillList[i].value == 1)
            {
                itemSkillList[i].gameObject.GetComponent<Image>().color = Color.white;
            }
        }
    }
    private bool checkBingoVertical()
    {
        bool flag = false;

        //check hang
        for (int i = 0; i < BingoBroad.GetLength(1); i++)
        {
            for (int j = 0; j < BingoBroad.GetLength(0) - 1; j++)
            {
                if (BingoBroad[j, i] == BingoBroad[j + 1, i] && BingoBroad[j, i] == 2)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            if (flag == true) break;
        }

        return flag;
    }

    private bool checkBingoHorizontal()
    {
        bool flag = false;
        //check cot
        for (int i = 0; i < BingoBroad.GetLength(0); i++)
        {
            for (int j = 0; j < BingoBroad.GetLength(1) - 1; j++)
            {
                if (BingoBroad[i, j] == BingoBroad[i, j + 1] && BingoBroad[i, j] == 2)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            if (flag == true) break;
        }

        return flag;
        
    }

    private void claimVertivalAward()
    {
        Debug.Log("an bonus 2 kiem");
    }

    private void claimHorizontalAward() {
        Debug.Log("an bonus chay nhanh");
    }

}
