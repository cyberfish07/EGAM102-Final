using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    WeaponLevel,
    NewWeapon,
    Health,
    Speed,
    Damage,
    Area,
    Cooldown,
    Duration
}

[System.Serializable]
public class UpgradeOption
{
    public UpgradeType upgradeType;
    public string title;
    public string description;
    public Sprite icon;
    public float upgradeValue;
    public GameObject weaponPrefab;
    public int targetWeaponIndex;
}

[System.Serializable]
public class PlayerWeapon
{
    public GameObject weaponPrefab;
    public string weaponName;
    public int level = 1;
    public int maxLevel = 5;
    public bool isUnlocked = false;

    [HideInInspector]
    public WeaponController weaponController;

    public float[] damageMultipliers;
    public float[] cooldownReductions;
    public float[] areaMultipliers;

    public string GetUpgradeTitle()
    {
        if (level == 0)
        {
            return "New Weapon! " + weaponName;
        }
        else
        {
            return weaponName + " Lv." + (level + 1);
        }
    }

    public string GetUpgradeDescription()
    {
        if (level == 0)
        {
            return "UNLOCK " + weaponName;
        }
        else if (level < maxLevel)
        {
            string desc = weaponName + " LEVEL UP TO " + (level + 1) + "\n";

            if (damageMultipliers != null && level < damageMultipliers.Length)
            {
                desc += "DAMAGE +" + (damageMultipliers[level] * 100) + "%\n";
            }

            if (cooldownReductions != null && level < cooldownReductions.Length)
            {
                desc += "DURATION -" + (cooldownReductions[level] * 100) + "%\n";
            }

            if (areaMultipliers != null && level < areaMultipliers.Length)
            {
                desc += "AREA +" + (areaMultipliers[level] * 100) + "%\n";
            }

            return desc;
        }
        else
        {
            return "MAX LEVEL";
        }
    }
}

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setting")]
    public PlayerWeapon[] availableWeapons;
    public Transform weaponHolder;

    [Header("Multiplier")]
    public float globalDamageMultiplier = 1f;
    public float globalAreaMultiplier = 1f;
    public float globalCooldownMultiplier = 1f;

    private List<PlayerWeapon> activeWeapons = new List<PlayerWeapon>();

    void Start()
    {
        if (weaponHolder == null)
        {
            weaponHolder = transform;
        }
        foreach (PlayerWeapon weapon in availableWeapons)
        {
            if (weapon.isUnlocked)
            {
                SpawnWeapon(weapon);
                activeWeapons.Add(weapon);
            }
        }
    }

    void SpawnWeapon(PlayerWeapon weapon)
    {
        GameObject weaponObj = Instantiate(weapon.weaponPrefab, weaponHolder);

        WeaponController controller = weaponObj.GetComponent<WeaponController>();

        if (controller != null)
        {
            weapon.weaponController = controller;

            if (controller.weaponData != null)
            {
                ApplyGlobalMultipliers(controller);
            }
        }
    }

    void ApplyGlobalMultipliers(WeaponController controller)
    {
        controller.damageMultiplier = globalDamageMultiplier;
        controller.cooldownMultiplier = globalCooldownMultiplier;
        controller.areaMultiplier = globalAreaMultiplier;
    }

    public UpgradeOption[] GetAvailableUpgrades()
    {
        List<UpgradeOption> upgrades = new List<UpgradeOption>();

        for (int i = 0; i < availableWeapons.Length; i++)
        {
            PlayerWeapon weapon = availableWeapons[i];

            if (weapon.isUnlocked && weapon.level < weapon.maxLevel)
            {
                UpgradeOption option = new UpgradeOption
                {
                    upgradeType = UpgradeType.WeaponLevel,
                    title = weapon.GetUpgradeTitle(),
                    description = weapon.GetUpgradeDescription(),
                    targetWeaponIndex = i
                };
                upgrades.Add(option);
            }
            else if (!weapon.isUnlocked)
            {
                UpgradeOption option = new UpgradeOption
                {
                    upgradeType = UpgradeType.NewWeapon,
                    title = weapon.GetUpgradeTitle(),
                    description = weapon.GetUpgradeDescription(),
                    targetWeaponIndex = i
                };
                upgrades.Add(option);
            }
        }

        UpgradeOption healthOption = new UpgradeOption
        {
            upgradeType = UpgradeType.Health,
            title = "MAX HP +20%",
            description = "ADD SOME BLOOD",
            upgradeValue = 0.2f
        };
        upgrades.Add(healthOption);

        UpgradeOption speedOption = new UpgradeOption
        {
            upgradeType = UpgradeType.Speed,
            title = "SPEED +15%",
            description = "FASTER, FASTER!",
            upgradeValue = 0.15f
        };
        upgrades.Add(speedOption);

        UpgradeOption damageOption = new UpgradeOption
        {
            upgradeType = UpgradeType.Damage,
            title = "HARM +10%",
            description = "NOW YOU BEAT THEM HARDER",
            upgradeValue = 0.1f
        };
        upgrades.Add(damageOption);

        UpgradeOption cooldownOption = new UpgradeOption
        {
            upgradeType = UpgradeType.Cooldown,
            title = "DURATION -10%",
            description = "FASTER...AGAIN?",
            upgradeValue = 0.1f
        };
        upgrades.Add(cooldownOption);

        return upgrades.ToArray();
    }

    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.WeaponLevel:
                UpgradeWeapon(upgrade.targetWeaponIndex);
                break;

            case UpgradeType.NewWeapon:
                UnlockWeapon(upgrade.targetWeaponIndex);
                break;

            case UpgradeType.Health:
                PlayerStats playerStats = GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.IncreaseMaxHealth(upgrade.upgradeValue);
                }
                break;

            case UpgradeType.Speed:
                PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.IncreaseSpeed(upgrade.upgradeValue);
                }
                break;

            case UpgradeType.Damage:
                IncreaseDamage(upgrade.upgradeValue);
                break;

            case UpgradeType.Area:
                IncreaseArea(upgrade.upgradeValue);
                break;

            case UpgradeType.Cooldown:
                DecreaseCooldown(upgrade.upgradeValue);
                break;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Upgrade");
    }

    void UpgradeWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
        {
            PlayerWeapon weapon = availableWeapons[weaponIndex];

            if (weapon.isUnlocked && weapon.level < weapon.maxLevel)
            {
                weapon.level++;

                ApplyWeaponLevelUpgrade(weapon);
            }
        }
    }

    void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < availableWeapons.Length)
        {
            PlayerWeapon weapon = availableWeapons[weaponIndex];

            if (!weapon.isUnlocked)
            {
                weapon.isUnlocked = true;
                weapon.level = 1;

                SpawnWeapon(weapon);
                activeWeapons.Add(weapon);
            }
        }
    }

    void ApplyWeaponLevelUpgrade(PlayerWeapon weapon)
    {
        int currentLevel = weapon.level - 1;

        WeaponController controller = weapon.weaponController;
        if (controller != null)
        {
            if (weapon.damageMultipliers != null && currentLevel < weapon.damageMultipliers.Length)
            {
                controller.damageMultiplier *= (1 + weapon.damageMultipliers[currentLevel]);
            }

            if (weapon.cooldownReductions != null && currentLevel < weapon.cooldownReductions.Length)
            {
                controller.cooldownMultiplier *= (1 - weapon.cooldownReductions[currentLevel]);
            }

            if (weapon.areaMultipliers != null && currentLevel < weapon.areaMultipliers.Length)
            {
                controller.areaMultiplier *= (1 + weapon.areaMultipliers[currentLevel]);
            }
        }
    }

    void IncreaseDamage(float multiplier)
    {
        globalDamageMultiplier *= (1 + multiplier);
        foreach (PlayerWeapon weapon in activeWeapons)
        {
            if (weapon.weaponController != null)
            {
                weapon.weaponController.damageMultiplier = globalDamageMultiplier;
            }
        }
    }

    void IncreaseArea(float multiplier)
    {
        globalAreaMultiplier *= (1 + multiplier);
        foreach (PlayerWeapon weapon in activeWeapons)
        {
            if (weapon.weaponController != null)
            {
                weapon.weaponController.areaMultiplier = globalAreaMultiplier;
            }
        }
    }

    void DecreaseCooldown(float reduction)
    {
        globalCooldownMultiplier *= (1 - reduction);
        foreach (PlayerWeapon weapon in activeWeapons)
        {
            if (weapon.weaponController != null)
            {
                weapon.weaponController.cooldownMultiplier = globalCooldownMultiplier;
            }
        }
    }
}