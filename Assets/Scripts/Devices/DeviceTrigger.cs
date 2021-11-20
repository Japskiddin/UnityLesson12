using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] targets; // список целевых объектов, которые будут активировать триггер
    public bool requireKey;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) { // метод вызывается при попадании объекта в зону триггера
        if (requireKey && Managers.Inventory.equippedItem != "key") {
            return;
        }
        foreach(GameObject target in targets) {
            target.SendMessage("Activate");
        }
    }

    private void OnTriggerExit(Collider other) { // метод вызывается при выходе объекта из зоны триггера
        foreach(GameObject target in targets) {
            target.SendMessage("Deactivate");
        }
    }
}
