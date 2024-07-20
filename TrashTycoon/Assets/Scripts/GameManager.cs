using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [Header("SaveDetails")]
    public GameSaveLoadManager saveLoadManager;
    public List<Transform> trashPlaces = new List<Transform>();
    public GameObject npcPrefab;
    public GameObject truckPrefab;
    public GameObject trashPrefab;
    public Transform npcParent;
    public Transform truckParent;
    public GameObject hut;
    public GameObject dumpYard;
    public bool hutLockState;
    private int totalTrashCollected;
    private Vector3[,] trashGrid = new Vector3[5, 5];
    private int currentLayer = 0;
    private int currentRow = 0;
    private int currentColumn = 0;

    [Header ("Settings")]
    [SerializeField] private int totalCoins;
    private int currentCoins;
    public int TotalCoins { get { return totalCoins; }
                            set { if (totalCoins < 0) 
                                    { totalCoins = 0; } 
                                  else
                                    { totalCoins = value; }
                                } 
                            }
    [SerializeField] TextMeshProUGUI coinText;


    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        
    }

    private void Start()
    {
        InitializeTrashGrid();
        LoadGame();
        currentCoins = TotalCoins;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    void InitializeTrashGrid()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                trashGrid[i, j] = new Vector3(j, 0, i);
            }
        }
    }
    private void Update()
    {
        coinText.text = "Coins : " + TotalCoins;
        totalTrashCollected = dumpYard.transform.childCount;
        for (int i = dumpYard.transform.childCount-1; i >= 20; i--)
        {
            Destroy(dumpYard.transform.GetChild(i).gameObject);
        }
    }

    public int AddCoins(int amount)
    {
        TotalCoins += amount;
        StartCoroutine(AnimateCoins(true));
        return TotalCoins;
    }

    public int BuyWithCoins(int amount)
    {
        TotalCoins -= amount;
        StartCoroutine(AnimateCoins(false));    
        return TotalCoins;
    }

    IEnumerator AnimateCoins(bool isIncrease)
    {
        while(currentCoins != TotalCoins)
        {
            if(isIncrease)
            {
                currentCoins++;
            }
            else
            {
                currentCoins--;
            }

            coinText.text = "Coins : " + currentCoins;
            yield return new WaitForSeconds(0.002f);
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        data.totalTrashCollected = totalTrashCollected;
        data.hutLockState = hutLockState;
        data.totalNPCs = npcParent.childCount;
        data.totalTrucks = truckParent.childCount;
        data.totalCoins = TotalCoins;
        foreach (Transform trash in trashPlaces)
        {
            foreach (Transform child in trash)
            {
                if (child.CompareTag("Trash"))
                {
                    data.trashPosition[0] = child.position.x;
                    data.trashPosition[1] = child.position.y;
                    data.trashPosition[2] = child.position.z;
                }
            }
        }

        saveLoadManager.SaveGame(data);
    }
    public void LoadGame()
    {
        GameData data = saveLoadManager.LoadGame();

        totalTrashCollected = data.totalTrashCollected;
        hutLockState = data.hutLockState;
        TotalCoins = data.totalCoins;
        
        for(int i = 0; i < totalTrashCollected; i++)
        {
            GameObject trash = Instantiate(trashPrefab, GetNextGridPosition(), Quaternion.identity);
            trash.transform.Rotate(-90, 0, 120);
            trash.transform.SetParent(dumpYard.transform);
        }
        for (int i = 0; i < data.totalNPCs; i++)
        {
            Instantiate(npcPrefab, npcParent);
        }
        for(int i = 0; i < data.totalTrucks; i++)
        {
            Instantiate(truckPrefab, truckParent);
        }

        UpdateHutLockState();
    }
    Vector3 GetNextGridPosition()
    {
        Vector3 position = dumpYard.transform.position + trashGrid[currentRow, currentColumn] + new Vector3(0, currentLayer, 0);

        currentColumn++;
        if (currentColumn >= 5)
        {
            currentColumn = 0;
            currentRow++;
            if (currentRow >= 5)
            {
                currentRow = 0;
                currentLayer++;
            }
        }

        return position;
    }

    void UpdateHutLockState()
    {
        hut.SetActive(hutLockState);
    }
}
