using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryPopup : MonoBehaviour
{
    [SerializeField] private Image[] itemIcons;
    [SerializeField] private Text[] itemLabels; // массивы для ссылки на четыре изображения и текстовые метки
    [SerializeField] private Text curItemLabel;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button useButton;

    private string _curItem;

    public void Refresh() {
        List<string> itemList = Managers.Inventory.GetItemList();

        int len = itemIcons.Length;
        for (int i = 0; i < len; i++) {
            if (i < itemList.Count) { // проверка списка инвентаря в процессе циклического просмотра всех изображений элемнтов UI
                itemIcons[i].gameObject.SetActive(true);
                itemLabels[i].gameObject.SetActive(true);

                string item = itemList[i];

                Sprite sprite = Resources.Load<Sprite>("Icons/" + item); // загружаем спрайт из папки Resources
                itemIcons[i].sprite = sprite;
                itemIcons[i].SetNativeSize(); // изменение размера изображения под исходный размер спрайта

                int count = Managers.Inventory.GetItemCount(item);
                string messaage = "x" + count;
                if (item == Managers.Inventory.equippedItem) {
                    messaage = "Equipped\n" + messaage; // на метке может появиться не только количество элементов, но и слово Equipped
                }
                itemLabels[i].text = messaage;

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick; // превращаем значки в интерактивные объекты
                entry.callback.AddListener((BaseEventData data) => {
                    OnItem(item); // лямбда-функция, позволяющая по-разному активировать каждый элемент
                });

                EventTrigger trigger = itemIcons[i].GetComponent<EventTrigger>();
                trigger.triggers.Clear(); // сброс подписчика, чтобы начать с чистого листа
                trigger.triggers.Add(entry); // добавляем функцию-подписчика к классу EventTrigger
            } else {
                itemIcons[i].gameObject.SetActive(false);
                itemLabels[i].gameObject.SetActive(false); // скрываем изображение / текст при отсутствии элементов изображения
            }
        }
        
        if (!itemList.Contains(_curItem)) {
            _curItem = null;
        }

        if (_curItem == null) { // скрываем кнопки при отсутствии выделенных элементов
            curItemLabel.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
        } else { // отображаем выделенный в данный момент элемент
            curItemLabel.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(true);
            if (_curItem == "health") { // используем кнопку только для элемента health
                useButton.gameObject.SetActive(true);
            } else {
                useButton.gameObject.SetActive(false);
            }
            curItemLabel.text = _curItem + ":";
        }
    }

    public void OnItem(string item) { // функция, вызываемая подписчиком события щелчка мыши
        _curItem = item;
        Refresh(); // актуализируем отобржение инвентаря
    }

    public void OnEquip() {
        Managers.Inventory.EquipItem(_curItem);
        Refresh();
    }

    public void OnUse() {
        Managers.Inventory.ConsumeItem(_curItem);
        if (_curItem == "health") {
            Managers.Player.ChangeHealth(25);
        }
        Refresh();
    }
}
