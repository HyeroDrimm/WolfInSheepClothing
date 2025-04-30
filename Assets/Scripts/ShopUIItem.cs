using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject unavailabe;

    public Func<ShopItem, bool> OnItemPicked;

    private ShopItem item;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        var output = OnItemPicked?.Invoke(item);

        if (output is true)
        {
            SetVisible(false);
        }
    }

    public void UpdateItem(ShopItem item)
    {
        this.item = item;
        
        iconImage.sprite = item.icon;
        nameText.text = item.name;
        priceText.text = $"{item.price.ToString()}x";
    }

    public void SetVisible(bool state)
    {
        unavailabe.SetActive(!state);
        canvas.interactable = state;
    }
}
