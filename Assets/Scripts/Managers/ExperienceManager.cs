using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Upgrade Values")]
    public int baseExperienceToLevel = 100;
    public float experienceLevelMultiplier = 1.2f;
    public int currentExperience = 0;
    public int currentLevel = 1;
    public int experienceToNextLevel;

    [Header("UI")]
    public GameObject levelUpPanel;
    public Transform upgradeOptionsContainer;
    public GameObject upgradeOptionPrefab;
    public Slider experienceBar;
    public TMP_Text levelText;

    [Header("Weapon")]
    public WeaponManager weaponManager;

    void Start()
    {
        levelUpPanel.SetActive(false);
        CalculateExperienceToNextLevel();

        if (weaponManager == null)
            weaponManager = GetComponent<WeaponManager>();

        UpdateExperienceUI();
    }

    void CalculateExperienceToNextLevel()
    {
        experienceToNextLevel = Mathf.RoundToInt(baseExperienceToLevel * Mathf.Pow(experienceLevelMultiplier, currentLevel - 1));
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        if (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        UpdateExperienceUI();
    }

    void LevelUp()
    {
        currentLevel++;

        int overflowExperience = currentExperience - experienceToNextLevel;
        currentExperience = overflowExperience;

        CalculateExperienceToNextLevel();

        ShowUpgradeOptions();
    }

    void UpdateExperienceUI()
    {
        if (experienceBar != null)
        {
            experienceBar.maxValue = experienceToNextLevel;
            experienceBar.value = currentExperience;
        }

        if (levelText != null)
        {
            levelText.text = "Lv. " + currentLevel;
        }
    }

    public void ShowUpgradeOptions()
    {
        UpgradeOption[] allAvailableUpgrades = weaponManager.GetAvailableUpgrades();

        UpgradeOption[] selectedUpgrades = GetRandomUpgradeOptions(allAvailableUpgrades, 3);

        if (levelUpPanel != null)
            levelUpPanel.SetActive(true);

        if (upgradeOptionsContainer != null)
        {
            foreach (Transform child in upgradeOptionsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < selectedUpgrades.Length; i++)
        {
            GameObject optionObj = Instantiate(upgradeOptionPrefab, upgradeOptionsContainer);

            TMP_Text titleText = optionObj.transform.Find("TitleText")?.GetComponent<TMP_Text>();
            TMP_Text descText = optionObj.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

            if (titleText != null)
                titleText.text = selectedUpgrades[i].title;

            if (descText != null)
                descText.text = selectedUpgrades[i].description;

            Button button = optionObj.GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    ApplyUpgrade(selectedUpgrades[index]);
                });
            }
        }

        Time.timeScale = 0;
    }

    private UpgradeOption[] GetRandomUpgradeOptions(UpgradeOption[] availableOptions, int count)
    {
        int selectionCount = Mathf.Min(count, availableOptions.Length);

        List<UpgradeOption> selectedOptions = new List<UpgradeOption>();

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < availableOptions.Length; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < selectionCount; i++)
        {
            if (availableIndices.Count == 0)
                break;

            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedIndex = availableIndices[randomIndex];

            selectedOptions.Add(availableOptions[selectedIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        return selectedOptions.ToArray();
    }

    void ApplyUpgrade(UpgradeOption upgrade)
    {
        if (weaponManager != null)
        {
            weaponManager.ApplyUpgrade(upgrade);
        }

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Upgrade");
    }
}