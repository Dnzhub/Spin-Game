using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class WheelOfFortune : MonoBehaviour
{
    public static event Action<int> OnSpin;
    public static event Action OnLevelFinish;
    
    #region Spin
    [Header("Spin Attributes")]
    private int m_randomValue;
    private bool m_canSpin = false;
    private int m_finalAngle;
    private float m_totalAngle;
    private int m_additionalLevel = 1;
    private int m_safeZoneIndex = 5;
    private int m_superZoneIndex = 30;
    private int m_sliceSection = 8;
    //Ödül acý ile hesaplanýrsa kullanýlacak
    private int itemToInventoryDegree = 100;
    

    [SerializeField] private int minRotateDegree = 200;
    [SerializeField] private int maxRotateDegree = 350;
    [SerializeField] private int spinTurnNumber = 50;
    #endregion
    [Header("Dotween")]
    private Sequence sequence;   
    #region lists
    [Header("Lists")]
    public ItemListSO bronzSpinItemList;
    public ItemListSO silverSpinItemList;
    public ItemListSO goldenSpinItemList;
    [HideInInspector]
    public List<Transform> imageList;
    [SerializeField] private InventoryItemListSO inventory;
    public List<ItemSO> tempItemList; //Kullanýcýnýn devam etmesi veya býrakmasý halinde iþlenecek gecici liste
    ItemListSO.SpinSelector currentSpinSelector;
    #endregion  
    [Header("Addressable Attributes")]
    #region Addressable Components
    public AssetReferenceT<SpriteAtlas> newAtlas;
    public string spriteAtlasAddress;
    public bool useAddress;
    private AsyncOperationHandle<SpriteAtlas> atlasOperation;
    #endregion
    private Image levelIndicator;
    private Canvas mainCanvas;
    private void Awake()
    {     
        mainCanvas = GetComponentInParent<Canvas>();
    }
    private void Start()
    {
        LevelManager.OnSafeZone += LevelManager_OnSafeZone;
        levelIndicator = UIItemsContainer.Instance.GetLevelText().transform.parent.gameObject.GetComponent<Image>();
        GetImageChildren();

        if (useAddress)
        {
            atlasOperation = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress);
            atlasOperation.Completed += SpriteAtlasLoaded;
        }
        else
        {
            atlasOperation = newAtlas.LoadAssetAsync();
            atlasOperation.Completed += SpriteAtlasLoaded;
        }
        
        m_canSpin = true;
        m_totalAngle = 360 / m_sliceSection; 
        SetSpinButton();
        SetTakeAllButton();
        SetGameOverButton();
    }

    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                //ilk spin olustur
                SpinSetUp(obj,bronzSpinItemList);
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("Sprite load failed. Using default sprite.");
                break;
            default:
                // case AsyncOperationStatus.None:
                break;
        }
    }
    //Tipine göre spin olustur
    private void SpinSetUp(AsyncOperationHandle<SpriteAtlas> obj,ItemListSO spinType)
    {       
        for (int i = 0; i < imageList.Count; i++)
        {
            
            transform.GetComponent<Image>().sprite = obj.Result.GetSprite(spinType.spinTypeName);
            imageList[i].GetComponent<Image>().sprite = obj.Result.GetSprite(spinType.list[i].atlasedSpriteName);
        }
        //Level göstergesi spinType ne ise ona göre þekillenecek (bronz-silver-gold)
        levelIndicator.sprite = obj.Result.GetSprite(spinType.spinLevelIndicator);
    }

    //LevelManager event
    //mevcut levelin duruma baðlý olarak spin özellikleri deðiþtir
    private void LevelManager_OnSafeZone(int currentLevel)
    {
        UIItemsContainer.Instance.GetLevelText().text = currentLevel.ToString();
        if (currentLevel % m_superZoneIndex == 0)
        {
            SpinSetUp(atlasOperation, goldenSpinItemList);
            currentSpinSelector = ItemListSO.SpinSelector.Golden;            
          
        }
        else if(currentLevel % m_safeZoneIndex == 0)
        {
            SpinSetUp(atlasOperation, silverSpinItemList);
            currentSpinSelector = ItemListSO.SpinSelector.Silver;
        }
        else
        {
            if(currentSpinSelector != ItemListSO.SpinSelector.Bronz)
            {
                SpinSetUp(atlasOperation, bronzSpinItemList);
                currentSpinSelector = ItemListSO.SpinSelector.Bronz;           
            }             
          
        }
   
    }

    //Spin butonunun iþlevini atama
    private void SetSpinButton()
    {
        UIItemsContainer.Instance.GetSpinButton().onClick.AddListener(() =>
        {
            if (m_canSpin)
            {
                DOTween.RestartAll();             
                HandleSpin();
            }
        });
    }
    //TakeAll butonunun iþlevini atama
    private void SetTakeAllButton()
    {
        //Eger oyuncu oyundan cekilip itemleri almak ister ise gecici listeden alýp kullanýcýnýn inventory(scriptable object) atýlacak
        UIItemsContainer.Instance.TakeAllButton().onClick.AddListener(() =>
        {
            if (tempItemList.Count != 0)
            {
                for (int i = 0; i < tempItemList.Count; i++)
                {
                    inventory.inventoryItemList.Add(tempItemList[i]);
                }
                tempItemList.Clear();
                foreach (Transform item in UIItemsContainer.Instance.GetInventoryContainer().transform)
                {
                    Destroy(item.gameObject);
                }
                OnLevelFinish?.Invoke();
                UIItemsContainer.Instance.GetLevelText().text = "1";
                itemToInventoryDegree = 100;
            }        
        });
    }

    private void SetGameOverButton()
    {
        UIItemsContainer.Instance.GetGameOverButton().onClick.AddListener(() =>
        {
            UIItemsContainer.Instance.GetLevelText().text = "1";
        
            UIItemsContainer.Instance.GetGameOverPanel().SetActive(false);
            tempItemList.Clear();
            foreach (Transform item in UIItemsContainer.Instance.GetInventoryContainer().transform)
            {
                Destroy(item.gameObject);
            }
            itemToInventoryDegree = 100;
            OnLevelFinish?.Invoke();
        });
    }
    private void HandleSpin()
    {
        //Onceden bir tweening deðeri kalmasýna karsýn tweenleri temizle
        ClearTweening();     

        m_randomValue = UnityEngine.Random.Range(minRotateDegree, maxRotateDegree);
        
        if (sequence == null) // Mevcut bir dotween iþlemi yok ise devam et
        {
         
            sequence = DOTween.Sequence();
            m_canSpin = false;

            UIItemsContainer.Instance.GetSpinButton().interactable = false;
            transform.DORotate(new Vector3(0, 0, m_randomValue), 0.05f).SetLoops(spinTurnNumber, LoopType.Incremental).             
               OnComplete(() =>
               {
                   m_finalAngle = Mathf.RoundToInt(transform.eulerAngles.z);
                   m_finalAngle = Mathf.Abs(SetNumberDivisiblebyTotalAngle(m_finalAngle));    
                   //Golden spin geldiðinde spinobjesi jackpot gibi titresim yapacak
                   if (currentSpinSelector == ItemListSO.SpinSelector.Golden)
                   {
                       transform.DOPunchPosition(Vector3.up * 200, 2, 20, 0);
                   }
                   else
                   {
                       transform.DOScale(0, .1f).SetEase(Ease.InFlash).OnComplete(() =>
                       {
                           transform.DOScale(1, 1f).SetEase(Ease.InFlash).OnComplete(() =>
                           {
                               DOTween.Kill(transform);
                           });
                       });
                       //transform.DOShakePosition(1f, 80f);                    
                   }
                   OnSpin?.Invoke(m_additionalLevel);

                   //Tüm imagelerin rotasyonunu resetle aksi takdirde acýlarý bozuluyor
                   foreach (Transform image in imageList)
                   {
                       image.transform.rotation = Quaternion.Euler(0,0,0);
                   }                  
                   if (m_finalAngle >= 360)
                   {
                       m_finalAngle = 0;
                   }
                   #region Açý ile hesaplama
                   //Mevcut ödülü kontrol et Eger acý ile almak istenirse
                   //Fakat bu yöntem üretilen dönme acýsýna göre oldugu icin itemlerin drop rate oranlarýda random olacak
                   //Isteðe baðlý olarak kodlar üzerinden ufak bir deðiþiklik ile iþleme sokulabilir
                   //for (int i = 0; i < m_sliceSection; i++)
                   //{
                   //    if (m_finalAngle == i * m_totalAngle)
                   //    {
                   //        CheckReward(i);                     
                   //    }                                      
                   //}
                   #endregion
                   CheckReward();
                   m_canSpin = true;                
               });
              
        }
        sequence.Play();
    }
   
   
    private void CheckReward()
    {
        ItemSO newRewardItem;
        Image reward = Instantiate(UIItemsContainer.Instance.GetRewardUI(),mainCanvas.transform);
        Text rewardTxt = reward.GetComponentInChildren<Text>();
       
        if (currentSpinSelector == ItemListSO.SpinSelector.Golden)
        {
            newRewardItem = goldenSpinItemList.GetRandomItem();          
        }
        else if (currentSpinSelector == ItemListSO.SpinSelector.Silver)
        {
            newRewardItem = silverSpinItemList.GetRandomItem();
        }
        else
        {
            newRewardItem = bronzSpinItemList.GetRandomItem();
            //Eger gelen item bomba ise oyunu bitir
            if (newRewardItem.isBomb)
            {
                GameOver();
            }
        }

        tempItemList.Add(newRewardItem);
        reward.sprite = atlasOperation.Result.GetSprite(newRewardItem.atlasedSpriteName);
        rewardTxt.text = newRewardItem.itemName + " " + newRewardItem.GetRandomAmount().ToString();
       
        reward.transform.DOScale(0, 2).SetEase(Ease.InElastic).OnComplete(() =>
         {
             reward.transform.SetParent(UIItemsContainer.Instance.GetInventoryContainer());
             reward.transform.localScale = new Vector3(1, 1, 1);
             reward.rectTransform.sizeDelta = new Vector2(100, 100);
             DOTween.Kill(reward.transform);
             UIItemsContainer.Instance.GetSpinButton().interactable = m_canSpin;
         });
     
    }
    private void GetImageChildren()
    {
        foreach (Transform item in transform)
        {
            if (item.CompareTag("ItemImage"))
            {
                imageList.Add(item);
            }
        }
    }
    private int SetNumberDivisiblebyTotalAngle(int i)
    {
        return (i % (int)m_totalAngle) == 0 ? i : i + (int)m_totalAngle - (int)(i % m_totalAngle);
    }
    private void ClearTweening()
    {
        DOTween.Kill(transform);
        sequence = null;
    }
    private void GameOver()
    {
        UIItemsContainer.Instance.GetGameOverPanel().SetActive(true);
        
        Debug.Log("Game Over...");
    }

    private void OnDestroy()
    {
        if (atlasOperation.IsValid())
        {
            Addressables.Release(atlasOperation);
            Debug.Log("Successfully released atlas load operation.");
        };
    }
}

//tempItemList.Add(goldenSpinItemList.list[index]);
//rewardTxt.text = goldenSpinItemList.list[index].itemName + " " + goldenSpinItemList.list[index].amount + " x";
//rewardImage.sprite = atlasOperation.Result.GetSprite(goldenSpinItemList.list[index].atlasedSpriteName);