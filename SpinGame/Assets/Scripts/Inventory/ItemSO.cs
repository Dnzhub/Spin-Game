using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/ItemType")]
public class ItemSO : ScriptableObject
{
    //Addressable sprite atlas icerisinden sprite cekmek icin sprite ismini girmek yeterli
    public string atlasedSpriteName;
    public string itemName;
    public int maxAmount;
    public int amount;
    public bool isBomb = false;
    [Range(0,100)]
    public float dropRate;
    [System.NonSerialized] private bool isInitialized = false;

    private void InitializeItem()
    {
        if (!isInitialized)
        {
            amount = 1;          
            isInitialized = true;
        }
    }
    public int GetRandomAmount()
    {
        InitializeItem();
        amount = (int)Random.Range(1f, maxAmount);
        return amount;
        
    }

}
