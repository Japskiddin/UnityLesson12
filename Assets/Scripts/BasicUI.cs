using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    private void OnGUI() {
        int posX = 10;
        int posY = 10;
        int width = 100;
        int height = 30;
        int buffer = 10;

        List<string> itemList = Managers.Inventory.GetItemList();
        if (itemList.Count == 0) {
            GUI.Box(new Rect(posX, posY, width, height), "No Items"); // отображаем сообщение об отсутствии инвентар€
        }
        
        foreach(string item in itemList) {
            int count = Managers.Inventory.GetItemCount(item);
            Texture2D image = Resources.Load<Texture2D>("Icons/" + item); // метод, загружающий ресурсы из папки Resources
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("(" + count + ")", image));
            posX += width + buffer; // при каждом прохождении цикла сдвигаемс€ в сторону
        }

        string equipped = Managers.Inventory.equippedItem;
        if (equipped != null) { // отображаем подготовленный элемент
            posX = Screen.width;
            Texture2D image = Resources.Load("Icons/" + equipped) as Texture2D;
            GUI.Box(new Rect(posX, posY, width, height), new GUIContent("Equipped", image));
        }

        posX = 10;
        posY += height + buffer;

        foreach(string item in itemList) { // просматриваем все элементы в цикле дл€ создани€ кнопок
            if (GUI.Button(new Rect(posX, posY, width, height), "Equip " + item)) {
                Managers.Inventory.EquipItem(item);
            }

            if (item == "health") {
                if (GUI.Button(new Rect(posX, posY + height + buffer, width, height), "Use health")) { // запускаем вложенный код при щелчке по кнопке
                    Managers.Inventory.ConsumeItem("health");
                    Managers.Player.ChangeHealth(25);
                }
            }

            posX += width + buffer;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
