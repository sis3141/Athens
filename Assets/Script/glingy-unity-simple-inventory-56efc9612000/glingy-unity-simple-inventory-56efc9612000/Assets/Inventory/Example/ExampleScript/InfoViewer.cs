using UnityEngine;
using UnityEngine.UI;
using Inventory;
using UnityEngine.EventSystems;

public class InfoViewer : ContentViewer {

    [Space(8f)]
    public Text nameText;
    public Text typeText;
    public Text descText;
    public Text priceText;
    public MenuViewer itemMenuViewer;

    protected override void EventCall () {
        foreach (var handler in ItemHandler.HandlerList) {
            handler.OnSlotEnter += OnDisplay;
            //handler.OnSlotUp += OnDisplay;
            handler.OnSlotExit += OnDisappear;
            //handler.OnPointerDown += OnDisappear;
        }
    }

    protected override void OnDisplay (PointerEventData eventData, InventorySlot slot) {
        if (slot.Item != null && !itemMenuViewer.IsEnabled) {
            ViewerEnable (slot);
        }
    }

    protected override void OnDisappear (PointerEventData eventData, InventorySlot slot) {
        ViewerDisable ();
    }

    protected override void DrawContent (InventorySlot slot) {
        SlotItem item = slot.Item;

        //아이템 없을 경우 끝내기
        if (item == null) return;

        //슬롯의 아이템 탭 이름에 따라 정렬 기준 설정
        if (item.Tab.TabName == "ShopTab") anchor = ViewerAnchor.TopRight;
        else if (item.Tab.TabName == "QuickSlotTab") anchor = ViewerAnchor.BottomRight;
        else anchor = ViewerAnchor.TopLeft;

        //슬롯의 아이템 탭 이름에 따라 가격 표시 설정
        if (item is ITradable) {
            ITradable tradable = item as ITradable;
            if (item.Tab.TabName == "ShopTab") priceText.text = "구매 가격 : " + tradable.BuyPrice.ToString ();
            else if (item.Tab.TabName == "QuickSlotTab") priceText.text = "";
            else priceText.text = "판매 가격 : " + tradable.SellPrice * item.Count;
        }

        //아이템 갯수에 따라 이름 및 갯수 표시
        if (item.Count == 1) nameText.text = item.Name;
        else nameText.text = $"{item.Name}({item.Count})";

        //아이템 타입에 따라 색상 변경
        switch (item.Type) {
            case "Sword": typeText.color = new Color32 (50, 160, 160, 255); break;
            case "Shield": typeText.color = new Color32 (160, 50, 160, 255); break;
            case "Potion": typeText.color = new Color32 (160, 160, 50, 255); break;
            default: typeText.color = new Color32 (50, 50, 50, 255); break;
        }

        //아이템 타입 및 설명 표시
        typeText.text = slot.Item.Type;
        descText.text = slot.Item.Description;
    }
}
