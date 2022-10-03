using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<int> OnSafeZone;    
    private int currentLevel = 1;
    
    void Start()
    {
        WheelOfFortune.OnSpin += AddAdditionalLevel;
        WheelOfFortune.OnLevelFinish += ResetCurrentLevel;
    }
    private void AddAdditionalLevel(int level)
    {
        currentLevel += level;
        
        OnSafeZone?.Invoke(currentLevel);      
    }   
    private void ResetCurrentLevel()
    {
        currentLevel = 1;
    }

}
