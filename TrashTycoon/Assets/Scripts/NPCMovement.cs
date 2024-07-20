using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class NPCMovement : MonoBehaviour
{
    public List<Transform> trashPlaces = new List<Transform>();
    public GameObject dumpyard;
    public GameObject npcHut;
    public float speed = 1f;
    public float speedMultiplier = 1f;
    public NavMeshAgent agent;
    private Animator anim;
    private GameObject carryingTrash = null;
    public float pickUpRadius = 1.5f;

    public Vector3[,] trashGrid = new Vector3[5, 5];
    private int currentLayer = 0;
    private int currentRow = 0;
    private int currentColumn = 0;

    private int trashedCount = 0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        dumpyard = GameObject.FindGameObjectWithTag("DumpYard");
        npcHut = FindObjectOfType<NPC_Spawner>().gameObject;
        trashPlaces = npcHut.GetComponent<NPC_Spawner>().trashPlaces;
        if(gameObject.tag == "NPC_Player")
        {
            agent.speed = speed;
        }
        else
        {
            agent.speed = speed * 2;
        }
        InitializeTrashGrid();
        SetTargetToNearestTrashPlace();
    }

    void Update()
    {
        bool isRunning = agent.velocity.magnitude > 0.1f;
        if (anim != null)
        {
            anim.SetBool("IsRun", isRunning);
        }
        
        if (carryingTrash == null && trashPlaces.Count > 0 && agent.remainingDistance < pickUpRadius)
        {
            PickUpTrash();
        }
        else if (carryingTrash != null && Vector3.Distance(transform.position, dumpyard.transform.position) < 0.2f)
        {
            DropOffTrash();
            if (trashedCount == 3)
            {
                UI_Manager.instance.hutCanvas.SetActive(true);
            }
        }
        else if (carryingTrash == null && !HasTrashInAnyPlace())
        {
            SetTarget(npcHut.transform.position);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
        Gizmos.color = Color.red;
    }

    void SetTarget(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
        
    }

    void SetTargetToNearestTrashPlace()
    {
        Transform nearestTrashPlace = GetNearestTrashPlace();
        if (nearestTrashPlace != null)
        {
            SetTarget(nearestTrashPlace.position);
        }
    }

    Transform GetNearestTrashPlace()
    {
        Transform nearestTrashPlace = null;
        float minDistance = float.MaxValue;

        foreach (var trashPlace in trashPlaces)
        {
            float distance = Vector3.Distance(transform.position, trashPlace.position);
            if (distance < minDistance && HasTrash(trashPlace))
            {
                minDistance = distance;
                nearestTrashPlace = trashPlace;
            }
        }

        return nearestTrashPlace;
    }

    bool HasTrash(Transform trashPlace)
    {
        foreach (Transform child in trashPlace)
        {
            if (child.CompareTag("Trash"))
            {
                return true;
            }
        }
        return false;
    }

    bool HasTrashInAnyPlace()
    {
        foreach (var trashPlace in trashPlaces)
        {
            if (HasTrash(trashPlace))
            {
                return true;
            }
        }
        return false;
    }

    void PickUpTrash()
    {
        Transform nearestTrashPlace = GetNearestTrashPlace();

        if (nearestTrashPlace != null && Vector3.Distance(transform.position, nearestTrashPlace.position) < pickUpRadius)
        {
            foreach (Transform child in nearestTrashPlace)
            {
                if (child.CompareTag("Trash"))
                {
                    carryingTrash = child.gameObject;
                    carryingTrash.transform.SetParent(transform);
                    carryingTrash.transform.localPosition = Vector3.one;
                    SetTarget(dumpyard.transform.position);
                    break;
                }
            }
        }
    }

    void DropOffTrash()
    {
        carryingTrash.transform.SetParent(null);
        Vector3 dropPosition = GetNextGridPosition(); 
        carryingTrash.transform.position = dropPosition;
        carryingTrash.transform.SetParent(dumpyard.transform);
        carryingTrash = null;
        SetTargetToNearestTrashPlace();
        GameManager.instance.AddCoins(10);
        trashedCount++;
    }
    Vector3 GetNextGridPosition()
    {
        Vector3 position = dumpyard.transform.position + trashGrid[currentRow, currentColumn] + new Vector3(0, currentLayer, 0);

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
}
