using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    private Dictionary<string, int> _items; // при объявлении словаря указываются два типа: тип ключа и тип значения

    public ManagerStatus status { get; private set; } // свойство читается откуда угодно, но задается только в этом сценарии
    public string equippedItem { get; private set; }

    public void Startup() {
        Debug.Log("Inventory manager starting..."); // сюда идут все задачи запуска с долгим временем выполнения
        _items = new Dictionary<string, int>(); // инициализируем пустой список элементов
        status = ManagerStatus.Started; // для задач с долгим временем выполнения используем состояние Initializing
    }

    public void DisplayItems() { // выводим в консоль сообщения о текущем инвентаре
        string itemDisplay = "Items: ";
        foreach(KeyValuePair<string, int> item in _items) {
            itemDisplay += item.Key + "(" + item.Value + ") ";
        }
        Debug.Log(itemDisplay);
    }

    public void AddItem(string name) { // другие сценарии не могут напрямую управлять списком элементов, но могут вызывать этот метод
        if (_items.ContainsKey(name)) { // перед вводом данных проверяем, не существует ли такой записи
            _items[name] += 1;
        } else {
            _items[name] = 1;
        }
        DisplayItems();
    }

    public List<string> GetItemList() { // возвращаем список всех ключей словаря
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    public int GetItemCount(string name) { // возвращаем количество указанных элементов в интентаре
        if (_items.ContainsKey(name)) {
            return _items[name];
        }
        return 0;
    }

    public bool EquipItem(string name) {
        if (_items.ContainsKey(name) && equippedItem != name) { // проверяем, что в интентаре есть указанный элемент, но он ещё не подготовлен
            equippedItem = name;
            Debug.Log("Eqipped " + name);
            return true;
        }

        equippedItem = null;
        Debug.Log("Unequipped");
        return false;
    }

    public bool ConsumeItem(string name) {
        if (_items.ContainsKey(name)) { // проверяем, есть ли в интентаре нужный элемент
            _items[name]--;
            if (_items[name] == 0) { // удаляем запись, если количество равно 0
                _items.Remove(name);
            }
        } else { // отвечаем, что в интентаре нет нужного элемента
            Debug.Log("Cannot consume " + name);
            return false;
        }

        DisplayItems();
        return true;
    }
}
