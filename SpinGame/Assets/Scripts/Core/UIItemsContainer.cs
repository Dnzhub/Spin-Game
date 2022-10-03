using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemsContainer : MonoBehaviour
{
    public static UIItemsContainer Instance { get; private set; }

    [SerializeField] private Image rewardUIPrefab;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private Button spinButton;
    [SerializeField] private Button takeAllButton;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject gameOverPanel;
   

    private void Awake()
    {
        #region Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        #endregion
    }
    private void Start()
    {
        gameOverPanel.SetActive(false);
    }
    public Image GetRewardUI()
    {
        return rewardUIPrefab;
    }
    public Transform GetInventoryContainer()
    {
        return inventoryContainer;
    }
    public Button GetSpinButton()
    {
        return spinButton;
    }
    public TMP_Text GetLevelText()
    {
        return levelText;
    }
    public Button TakeAllButton()
    {
        return takeAllButton;
    }
    public GameObject GetGameOverPanel()
    {
        return gameOverPanel;
    }
    public Button GetGameOverButton()
    {
        return gameOverButton;
    }


}
