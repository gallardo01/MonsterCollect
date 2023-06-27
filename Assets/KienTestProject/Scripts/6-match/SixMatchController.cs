
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SixMatchController : Singleton<BingoSkillMatix>
{
    // Start is called before the first frame update

    private const int MAX_X = 5;
    private const int MAX_Y = 5;

    public GameObject tilePrefab;
    public GameObject broad;
    public int life = 6;

    public List<Tile> tilesList = new List<Tile>();
    public Tile player;
    void Start()
    {
        InitData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveUp();
        }
    }

    private void InitData()
    {
        for (int i = 0; i < MAX_X; i++)
        {
            for(int j = 0; j < MAX_Y; j++)
            {
                GameObject item;

                if (i == 2 && j == 2)
                {
                    item = Instantiate(tilePrefab, broad.transform);
                    item.GetComponent<Tile>().posX = i;
                    item.GetComponent<Tile>().posY = j;
                    item.GetComponent<Tile>().type = 0;

                }
                else
                {
                    item = Instantiate(tilePrefab, broad.transform);
                    item.GetComponent<Tile>().posX = i;
                    item.GetComponent<Tile>().posY = j;
                    item.GetComponent<Tile>().type = UnityEngine.Random.Range(1, 4);

                }
                
                tilesList.Add(item.GetComponent<Tile>());

            }
        }

        player = tilesList[2*5+2];
        player.life.text = life.ToString();

    }

    private void MoveLeft()
    {
        if (player.posY > 0)
        {
            SwapTiles(player, tilesList[player.posX * 5 + player.posY - 1]);
            player = tilesList[player.posX * 5 + player.posY - 1];
            ReloadBroad();
        }
    }
    private void MoveRight()
    {
        if (player.posY < 4) {
            SwapTiles(player, tilesList[player.posX * 5 + player.posY + 1]);
            player = tilesList[player.posX * 5 + player.posY+1];
            ReloadBroad();
        }

    }

    private void MoveUp()
    {
        if (player.posX > 0)
        {
            SwapTiles(player, tilesList[(player.posX-1) * 5 + player.posY]);
            player = tilesList[(player.posX-1) * 5 + player.posY];
            ReloadBroad();
        }
    }
    private void MoveDown()
    {
        if (player.posX < 4)
        {
            SwapTiles(player, tilesList[(player.posX + 1) * 5 + player.posY]);
            player = tilesList[(player.posX + 1) * 5 + player.posY];
            ReloadBroad();
        }
    }

    private void SwapTiles(Tile player, Tile tile)
    {
        int tempv;

        tempv = player.type;
        player.type = tile.type;
        tile.type = tempv;

        life--;
    }

    private void ReloadBroad()
    {
        for (int i = 0; i < tilesList.Count; i++)
        {
            foreach (Transform skill in tilesList[i].transform.GetChild(0))
            {
                skill.gameObject.SetActive(false);
            }
            tilesList[i].transform.GetChild(0).GetChild(tilesList[i].type).gameObject.SetActive(true);
            tilesList[i].life.text = "";
        }

        player.life.text = life.ToString();
    }
}
