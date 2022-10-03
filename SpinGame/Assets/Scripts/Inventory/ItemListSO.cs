using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemTypeList")]
public class ItemListSO : ScriptableObject
{
    public enum SpinSelector
    {
        Bronz,
        Silver,
        Golden
    }
    [System.NonSerialized] private bool isInitialized = false;
    private float totalWeight;
    public SpinSelector spinSelector;
    public List<ItemSO> list;
    public string spinTypeName;
    public string spinLevelIndicator;
    

    private void InitializeList()
    {
        if (!isInitialized)
        {
            totalWeight = list.Sum(item => item.dropRate);
            isInitialized = true;
        }
    }

    public ItemSO GetRandomItem()
    {
        InitializeList();
        float randomRoll = Random.Range(0f, totalWeight);
        foreach (ItemSO item in list)
        {
            if(item.dropRate >= randomRoll)
            {                            
                return item;           
            }
            randomRoll -= item.dropRate;
        }
        throw new System.Exception("No Reward!");
    }
  




}


