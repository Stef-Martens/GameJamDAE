using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI _diamondText;

    private void Start()
    {
        _diamondText= GetComponent<TextMeshProUGUI>();
    }

    public void UpdateDiamondText(PlayerInventory playerInventory)
    {
        _diamondText.text = playerInventory.NumberOfDiamonds.ToString();
    }
}
