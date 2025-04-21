using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeOptionUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public void SetupOption(UpgradeOption option)
    {
        if (titleText != null)
        {
            titleText.text = option.title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = option.description;
        }
    }
}