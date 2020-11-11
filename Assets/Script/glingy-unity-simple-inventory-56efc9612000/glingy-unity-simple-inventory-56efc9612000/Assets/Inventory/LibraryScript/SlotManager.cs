﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory {

    public class SlotManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

        private static List<SlotManager> ManagerList = new List<SlotManager> ();

        [Tooltip ("아이템 슬롯 리스트 (필요시 등록)")]
        public List<InventorySlot> slotList = new List<InventorySlot> (); //슬롯 리스트
        private List<RectTransform> slotRects; //슬롯 렉트 트랜스폼 리스트 (포인터 이벤트 감지용)
        
        [Tooltip ("이름 순으로 슬롯 정렬")]
        public bool sortByName = true;

        [Space(8f)]
        [Tooltip("아이템 없을 경우 표시될 아이콘")]
        public Sprite defaultIcon;

        [Tooltip ("아이템 이벤트 핸들러")]
        public ItemHandler itemHandler;

        public InventoryTab LastRefreshedTab { get; internal set; } //최근 새로고침된 탭

        internal static InventorySlot SelectedSlot { get; set; }

        private InventorySlot lastEnteredSlot;
        private bool pointerEntered;

        private void Awake () {
            if (!ManagerList.Contains (this)) ManagerList.Add (this);
        }

        private void Start () {
            SlotSort (); //최초 정렬
        }

        //슬롯 이름 순으로 정렬 및 인덱스 지정
        public void SlotSort () {
            if (sortByName) slotList.Sort ((InventorySlot x, InventorySlot y) => x.name.CompareTo (y.name));
            for (int i = 0; i < slotList.Count; i++) slotList[i].Index = i;

            //슬롯의 렉트 트랜스폼을 리스트에 추가
            slotRects = new List<RectTransform> (slotList.Count);
            foreach (var slot in slotList) {
                slotRects.Add (slot.GetComponent<RectTransform> ());
            }
        }

        //아이템 테이블 새로고침 (탭 아이템 리스트 시각화)
        public void Refresh (InventoryTab tab) {
            //Tab이 null일 경우 중단
            if (tab == null) return;

            if (tab.Capacity > slotList.Count) {
                Debug.Log ("탭의 아이템 수용량이 슬롯 수를 초과합니다 : " + tab.TabName);
                return;
            }

            //슬롯 아이템 및 현재 탭 설정
            for (int i = 0; i < tab.Capacity; i++) {
                slotList[i].SetSlot (tab.ItemTable[i]);
            }

            LastRefreshedTab = tab;
        }

        //모든 아이템 테이블 마지막 탭으로 새로고침
        public static void RefreshAll () {
            foreach (var manager in ManagerList) {
                if (manager.LastRefreshedTab != null) 
                    manager.Refresh (manager.LastRefreshedTab);
            }
        }

        //모든 슬롯의 허용 타입 설정
        public void SetSlotType (string[] types) {
            foreach (var slot in slotList) {
                slot.allowTypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++) {
                    slot.allowTypes[i] = types[i];
                }
            }
        }

        //포인터 다운 이벤트 (아이템 선택)
        public void OnPointerDown (PointerEventData eventData) {
            //좌클릭 이벤트
            if (eventData.button == PointerEventData.InputButton.Left) {
                InventorySlot slot = GetSlotFromPointer ();

                if (slot != null) {
                    itemHandler.OnSlotDown?.Invoke (eventData, slot);

                    //선택 아이템 등록 (이동 활성화 시)
                    if (itemHandler.movable) {
                        if (slot.Item != null) {
                            ItemHandler.SelectedItem = slot.Item;
                            SelectedSlot = slot;
                            itemHandler.OnItemSelected?.Invoke (slot, ItemHandler.SelectedItem);
                        }
                    }
                }
            }
        }

        //포인터 업 이벤트 (아이템 드롭)
        public void OnPointerUp (PointerEventData eventData) {
            ItemHandler.RequestItemResetStop = true; //아이템 핸들러 아이템 초기화 감지 끄기

            //좌클릭 이벤트
            if (eventData.button == PointerEventData.InputButton.Left) {
                InventorySlot targetSlot = GetSlotFromPointer ();

                if (targetSlot != null) {
                    itemHandler.OnSlotUp?.Invoke (eventData, targetSlot);

                    //이동 활성화 및 선택된 아이템 존재시 이벤트 실행
                    if (itemHandler.movable && ItemHandler.SelectedItem != null) {
                        ItemHandler.TargetItem = targetSlot.Item; //대상 아이템 등록

                        //슬롯 매니저가 서로 다를 경우 이벤트 처리
                        if (SelectedSlot.slotManager != this) {
                            if (!itemHandler.moveToOtherSlot || !targetSlot.itemHandler.moveToOtherSlot) {
                                itemHandler.SlotMoveFailEvent?.Invoke (ItemHandler.SelectedItem);
                                itemHandler.ResetItems ();
                                return;
                            }
                        }

                        //선택된 아이템과 타겟 아이템이 일치하지 않을 경우 이벤트 실행
                        if (ItemHandler.SelectedItem != ItemHandler.TargetItem) {

                            //타겟이 없을 경우 아이템 이동
                            if (ItemHandler.TargetItem == null) {

                                //타겟 슬롯에 타입이 없거나 두 타입이 일치할 경우 실행
                                if (targetSlot.allowTypes.Length == 0 || targetSlot.HasType (ItemHandler.SelectedItem.Type)) {
                                    itemHandler.Move (ItemHandler.SelectedItem, targetSlot);
                                } else
                                    itemHandler.TypeNotMatchEvent?.Invoke (ItemHandler.SelectedItem, targetSlot);

                            } else {
                                //아이템 병합이 활성화일 때 이벤트 실행
                                if (itemHandler.merging && (ItemHandler.SelectedItem.Name == ItemHandler.TargetItem.Name)) {
                                    itemHandler.Merge (ItemHandler.SelectedItem, ItemHandler.TargetItem);

                                    //아이템 위치 교환
                                } else if (itemHandler.switching) {
                                    if (targetSlot.allowTypes.Length == 0 || targetSlot.HasType (ItemHandler.SelectedItem.Type)) {
                                        itemHandler.Switch (ItemHandler.SelectedItem, ItemHandler.TargetItem);

                                    } else
                                        itemHandler.TypeNotMatchEvent?.Invoke (ItemHandler.SelectedItem, targetSlot);
                                }
                            }

                            //선택된 슬롯의 매니저 및 대상 슬롯 매니저 새로고침
                            SelectedSlot.slotManager.Refresh (SelectedSlot.slotManager.LastRefreshedTab);
                            if (SelectedSlot.slotManager != this)
                                Refresh (LastRefreshedTab);
                        }
                    }
                }
            }

            ItemHandler.RequestItemResetStop = false; //아이템 핸들러 초기화 감지 켜기
            ItemHandler.CallEventEnd (); //초기화 진행
            SelectedSlot = null;
        }

        //포인터 클릭 이벤트 (아이템 사용)
        public void OnPointerClick (PointerEventData eventData) {
            InventorySlot slot = GetSlotFromPointer ();
            if (slot != null) {
                itemHandler.OnSlotClick?.Invoke (eventData, slot);

                //좌 더블 클릭 이벤트
                if (eventData.button == PointerEventData.InputButton.Left) {

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
                    if (!(eventData.clickCount >= 2))
                        return; //PC 더블 클릭 체크
#endif

                    //슬롯에 아이템이 있을 경우 실행
                    if (itemHandler.usable && slot.Item != null) {
                        itemHandler.Use (slot.Item);
                        Refresh (LastRefreshedTab);
                    }
                }
            }
        }

        //포인터 엔터 이벤트
        public void OnPointerEnter (PointerEventData eventData) {
            //Pointer 이벤트들은 기본적으로 한 오브젝트 내에서만 동작됨
            //Enter 마다 현재 인식된 오브젝트를 변경하여 고정 방지
            eventData.pointerPress = gameObject;

            if (itemHandler.enablePointerEnterAndExitEvent) {
                pointerEntered = true;
                StartCoroutine (PointerUpdate (eventData));
            }
        }

        //포인터 익스트 이벤트
        public void OnPointerExit (PointerEventData eventData) {
            eventData.pointerPress = null; //오브젝트 고정 방지
            if (pointerEntered) {
                pointerEntered = false; //포인터 Enter, Exit 이벤트 종료
            }

            if (itemHandler.enablePointerEnterAndExitEvent) {

                if (lastEnteredSlot != null) { //마지막 슬롯이 있다면 Exit 이벤트 호출
                    itemHandler.OnSlotExit (eventData, lastEnteredSlot);
                    lastEnteredSlot = null;
                }
            }
        }

        //엔터, 익스트 이벤트를 위한 포인터 갱신 및 이벤트 호출
        IEnumerator PointerUpdate (PointerEventData eventData) {
            WaitForSeconds interval = new WaitForSeconds (itemHandler.pointerUpdateInterval); 
            while (pointerEntered) {
                InventorySlot slot = GetSlotFromPointer ();

                if (slot != null) { //포인터에 슬롯이 있을 경우
                    itemHandler.OnSlotEnter (eventData, slot); //Enter 이벤트 호출

                    //마지막 슬롯과 현재 슬롯이 다를 경우 마지막 슬롯 Exit 이벤트 호출
                    if (lastEnteredSlot != null && lastEnteredSlot != slot) {
                        itemHandler.OnSlotExit (eventData, lastEnteredSlot);
                    }

                    lastEnteredSlot = slot; //마지막 슬롯 설정

                } else { //포인터에 슬롯이 없을 경우
                    if (lastEnteredSlot != null) { //마지막 슬롯이 있다면 Exit 이벤트 호출
                        itemHandler.OnSlotExit (eventData, lastEnteredSlot);
                        lastEnteredSlot = null;
                    }
                }

                yield return interval; //대기
            }
        }

        //포인터 위치로부터 슬롯 가져오기 
        private InventorySlot GetSlotFromPointer () {
            Vector2 pointer = GetPointerPosition (); //포인터 위치

            //슬롯의 위치 및 거리
            Vector2 pos;
            float xDist, yDist;

            for (int i = 0; i < slotRects.Count; i++) {
                pos = slotRects[i].position;
                xDist = slotRects[i].sizeDelta.x * 0.5f;
                yDist = slotRects[i].sizeDelta.y * 0.5f;

                //포인터가 슬롯의 영역 내에 있을 경우 해당 인덱스 반환
                if (pointer.x >= pos.x - xDist && pointer.x <= pos.x + xDist && 
                    pointer.y >= pos.y - yDist && pointer.y <= pos.y + yDist) {
                    return slotList[i];
                }
            }

            return null;
        }

        //PC, 모바일에 따라 포인터 위치 가져오기
        private Vector2 GetPointerPosition () {
            //Mobile
#if (UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR)
            Touch touch = Input.GetTouch (0);
                return touch.position;
#endif
            //PC
            return Input.mousePosition;
        }
    }
}
