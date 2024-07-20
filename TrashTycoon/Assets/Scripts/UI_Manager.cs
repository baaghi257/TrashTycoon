using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] TextMeshProUGUI noCoinText;
    public TextMeshProUGUI buyNPCText;
    public TextMeshProUGUI increaseSpeedText;
    public TextMeshProUGUI buyTruckText;
    public Button increaseSpeedBtn;

    public static UI_Manager instance;

    private int amount = 20;

    [Header("Hut canvas setting")]

    [SerializeField] public GameObject hutCanvas;
    [SerializeField] GameObject unlockHutCanvas, lockedHutCanvas;
    [SerializeField] Sprite unlockSprite;
    [SerializeField] Image unlockImage;
    [SerializeField] Image fillImg;


    private bool isFilling = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        CanvasSettings();
    }

    private void CanvasSettings()
    {
        mainCanvas.SetActive(false);
        hutCanvas.SetActive(false);
        lockedHutCanvas.SetActive(!GameManager.instance.hutLockState);
        unlockHutCanvas.SetActive(GameManager.instance.hutLockState);
        fillImg.fillAmount = 0;
    }
    private void Update()
    {
        hutCanvas.transform.LookAt(Camera.main.transform);
        buyNPCText.text = "Buy " + amount;
        buyTruckText.text = "Buy 50";
        if (isFilling)
        {
            fillImg.fillAmount += 0.2f;
            if (fillImg.fillAmount >= 1)
            {
                StartCoroutine(ChangeHutState(2f));
            }
        }
    }

    public void UnlockHut()
    {
        isFilling = true;
        GameManager.instance.hutLockState = true;
    }

    IEnumerator ChangeHutState(float delay)
    {
        unlockImage.sprite = unlockSprite;
        yield return new WaitForSeconds(delay);
        lockedHutCanvas.SetActive(false);
        unlockHutCanvas.SetActive(true);
    }

    public void TurnOnCanvas()
    {
        mainCanvas.SetActive(true);
    }

    public void BuyNPC()
    {
        if(GameManager.instance.TotalCoins >= amount)
        {
            GameManager.instance.BuyWithCoins(amount);
            NPC_Spawner npc = FindObjectOfType<NPC_Spawner>();
            npc.AddNPC();
            hutCanvas.SetActive(false);
        }
        else
        {
            StartCoroutine(StartFadeText(2f));
        }
    }

    public void BuyNPC_Truck()
    {
        if (GameManager.instance.TotalCoins >= 50)
        {
            GameManager.instance.BuyWithCoins(50);
            NPC_Spawner npc = FindObjectOfType<NPC_Spawner>();
            npc.AddNpcTruck();
            hutCanvas.SetActive(false);
        }
        else
        {
            StartCoroutine(StartFadeText(2f));
        }
    }



    public IEnumerator StartFadeText(float delay)
    {
        noCoinText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        noCoinText.gameObject.SetActive(false);
    }

    public void CloseBtn()
    {
        mainCanvas.SetActive(false);
    }
}
