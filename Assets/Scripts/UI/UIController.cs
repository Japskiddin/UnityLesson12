using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text healthLabel; // ссылаемся на UI объект в сцене
    [SerializeField] private InventoryPopup popup;

    private void Awake() { // задаем подписчика для события обновления здоровья
        Messenger.AddListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
    }

    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.HEALTH_UPDATED, OnHealthUpdated);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnHealthUpdated();
        popup.gameObject.SetActive(false); // инициализируем вслпывающее окно закрытым
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) { // отображаем вслывающее окно по клавише M
            bool isShowing = popup.gameObject.activeSelf;
            popup.gameObject.SetActive(!isShowing);
            popup.Refresh();
        }
    }

    private void OnHealthUpdated() { // подписчик события вызывает функцию для обновления метки health
        string message = "Health: " + Managers.Player.health + " / " + Managers.Player.maxHealth;
        healthLabel.text = message;
    }
}
