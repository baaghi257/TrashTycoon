using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int totalTrashCollected;
    public int totalCoins;
    public float[] trashPosition = new float[3];
    public int totalNPCs;
    public int totalTrucks;
    public bool hutLockState;
}
