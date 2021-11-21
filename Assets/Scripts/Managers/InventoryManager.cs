using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    private Dictionary<string, int> _items; // при объ€влении словар€ указываютс€ два типа: тип ключа и тип значени€

    public ManagerStatus status { get; private set; } // свойство читаетс€ откуда угодно, но задаетс€ только в этом сценарии
    public string equippedItem { get; private set; }
    private NetworkService _network;

    public void Startup(NetworkService service) {
        Debug.Log("Inventory manager starting..."); // сюда идут все задачи запуска с долгим временем выполнени€
        _network = service;
        UpdateData(new Dictionary<string, int>()); // инициализируем пустой список
        status = ManagerStatus.Started; // дл€ задач с долгим временем выполнени€ используем состо€ние Initializing
    }

    public void UpdateData(Dictionary<string, int> items) {
        _items = items;
    }

    public Dictionary<string, int> GetData() { // дл€ сохранени€ данных необходима функци€ чтени€
        return _items;
    }

    public void DisplayItems() { // выводим в консоль сообщени€ о текущем инвентаре
        string itemDisplay = "Items: ";
        foreach(KeyValuePair<string, int> item in _items) {
            itemDisplay += item.Key + "(" + item.Value + ") ";
        }
        Debug.Log(itemDisplay);
    }

    public void AddItem(string name) { // другие сценарии не могут напр€мую управл€ть списком элементов, но могут вызывать этот метод
        if (_items.ContainsKey(name)) { // перед вводом данных провер€ем, не существует ли такой записи
            _items[name] += 1;
        } else {
            _items[name] = 1;
        }
        DisplayItems();
    }

    public List<string> GetItemList() { // возвращаем список всех ключей словар€
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
        if (_items.ContainsKey(name) && equippedItem != name) { // провер€ем, что в интентаре есть указанный элемент, но он ещЄ не подготовлен
            equippedItem = name;
            Debug.Log("Eqipped " + name);
            return true;
        }

        equippedItem = null;
        Debug.Log("Unequipped");
        return false;
    }

    public bool ConsumeItem(string name) {
        if (_items.ContainsKey(name)) { // провер€ем, есть ли в интентаре нужный элемент
            _items[name]--;
            if (_items[name] == 0) { // удал€ем запись, если количество равно 0
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
