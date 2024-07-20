using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private GameObject npcTruckPrefab;
    [SerializeField] private GameObject npcPlayerParent;
    [SerializeField] private GameObject npcTruckParent;
    public List<Transform> trashPlaces = new List<Transform>();
    [SerializeField] private List<GameObject> npcList = new List<GameObject>();
    private int amount = 5;

    private void Update()
    {
        if(amount >= 40)
        {
            UI_Manager.instance.increaseSpeedBtn.interactable = false;
        }
        UI_Manager.instance.increaseSpeedText.text = "Buy " + amount;
    }
    public void AddNPC()
    {
        SpawnNPCs();
    }

    public void AddNpcTruck()
    {
        SpawnNPCs_Truck();
    }
    void SpawnNPCs()
    {
        GameObject npc = Instantiate(npcPrefab, transform.position, Quaternion.identity,npcPlayerParent.transform);
        NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
        npcMovement.trashPlaces = trashPlaces;
        npcList.Add(npc);
    }
    void SpawnNPCs_Truck()
    {
        GameObject npc = Instantiate(npcTruckPrefab, transform.position, Quaternion.identity, npcTruckParent.transform);
        NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
        npcMovement.trashPlaces = trashPlaces;
        npcList.Add(npc);
    }
    public void UpgradeNPCSpeed(float multiplier)
    {
        if (GameManager.instance.TotalCoins - amount >= 0)
        {
            foreach (GameObject npc in npcList)
            {
                NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
                npcMovement.speedMultiplier = multiplier;
                npcMovement.agent.speed = npcMovement.speed + multiplier;
                if(npcMovement.gameObject.tag == "NPC_Player")
                {
                    if (npcMovement.speed <= 5)
                    {

                        npcMovement.speed += multiplier;
                    }
                }
                if (npcMovement.gameObject.tag == "NPC_Truck")
                {
                    if (npcMovement.speed <= 5)
                    {

                        npcMovement.speed += multiplier * 2;
                    }
                }
                else
                {
                    UI_Manager.instance.increaseSpeedBtn.interactable = false;
                }

            }
            GameManager.instance.BuyWithCoins(amount);
            amount += 5;
        }
        else
        {
            StartCoroutine(UI_Manager.instance.StartFadeText(2f));
        }
        

        
    }
}
