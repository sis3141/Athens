using UnityEngine;
using UnityEngine.UI;

namespace Inventory {

    public class SelectedItemTracker : MonoBehaviour {

        [SerializeField] Canvas canvas;
        public Vector2 trackerSize; //트래킹 이미지 사이즈
        public Color trackerColor; //트래킹 이미지 색상
        public Color selectedSlotColor; //선택된 슬롯의 색상

        private InventorySlot lastSlot;
        private SlotItem lastItem;
        private Image trackingImage;
        private RectTransform tracker_rt;
        private Color lastSlotColor;

        private void Start () {

            //이벤트 핸들러 추가
            foreach (var handler in ItemHandler.HandlerList) {
                handler.OnItemSelected += OnItemSelected;
                handler.OnEventEnded += OnEventEnded;
            }

            //트래커 이미지 생성
            GameObject tracker = new GameObject ("PointerTracker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            trackingImage = tracker.GetComponent<Image> ();
            trackingImage.color = trackerColor;
            trackingImage.raycastTarget = false;

            tracker_rt = tracker.GetComponent<RectTransform> ();
            tracker_rt.sizeDelta = trackerSize;
            tracker_rt.gameObject.SetActive (false);
            tracker_rt.SetParent (canvas.transform);

        }

        //활성화된 동안 마우스 트래킹
        private void LateUpdate () {
            if (tracker_rt.gameObject.activeSelf) {
                tracker_rt.position = Input.mousePosition;

                if (lastSlot != null) {
                    if (lastItem == lastSlot.Item) lastSlot.slotIcon.color = selectedSlotColor;
                    else lastSlot.slotIcon.color = lastSlotColor;
                }
            }
        }

        //아이템 선택 이벤트
        public void OnItemSelected (InventorySlot slot, SlotItem item) {
            if (slot.Item != null) {
                lastSlot = slot;
                lastItem = slot.Item;
                lastSlotColor = slot.slotIcon.color;

                slot.slotIcon.color = selectedSlotColor;
                trackingImage.sprite = item.Icon;
                tracker_rt.gameObject.SetActive (true);
            }
        }

        //이벤트 종료시 이벤트
        public void OnEventEnded () {
            if (lastSlot != null) lastSlot.slotIcon.color = lastSlotColor;
            tracker_rt.gameObject.SetActive (false);
        }

        public Image GetTracker () {
            if (trackingImage != null) return trackingImage;
            else return null;
        }
    }
}
