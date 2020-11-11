using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory {

    public class QuickSlotHandler : MonoBehaviour {

        [Serializable]
        public class ShortcutSlot {
            public InventorySlot slot;
            public KeyCode key;
        }

        public ItemHandler itemHandler; //아이템 핸들러
        public List<ShortcutSlot> quickSlotList = new List<ShortcutSlot> (); //퀵 슬롯 리스트

        private Dictionary<KeyCode, InventorySlot> quickSlotDctn = new Dictionary<KeyCode, InventorySlot> (); //퀵 슬롯 딕셔너리

        private void Awake () {

            //딕셔너리에 등록
            foreach (var slot in quickSlotList) {
                quickSlotDctn.Add (slot.key, slot.slot);
            }
        }

        private void Update () {

            //아무 키나 눌렸을 때 이벤트 실행
            if (Input.anyKeyDown) {

                foreach (var slot in quickSlotList) {

                    //슬롯의 키가 눌렸을 때 해당 슬롯의 아이템 사용
                    if (Input.GetKeyDown (slot.key)) {
                        InventorySlot usedSlot = quickSlotDctn[slot.key];
                        if (usedSlot.Item != null) {
                            itemHandler.Use (usedSlot.Item);
                            usedSlot.slotManager.Refresh (usedSlot.slotManager.LastRefreshedTab);
                        }
                    }
                }
            }
        }
    }
}
