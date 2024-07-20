using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;
    public Transform trashPlace;
    public int trashCount = 10;
    public Vector3 spawnAreaSize;
    [SerializeField] public List<GameObject> trashList = new List<GameObject>();

    public static TrashSpawner instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < trashCount; i++)
        {
            Vector3 randomPosition = transform.position + new Vector3(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),0.2f, Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));
            GameObject trash = Instantiate(trashPrefab, randomPosition, Quaternion.identity, trashPlace);
            trash.transform.Rotate(-90, 0, 0);
            trashList.Add(trash);
        }
    }
}
