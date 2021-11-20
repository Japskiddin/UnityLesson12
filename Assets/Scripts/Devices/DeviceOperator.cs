using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    public float radius = 1.5f; // расстояние, на котором становится возможной активация устройств

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3")) { // реакция на кнопку ввода
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius); // метод OverlapSphere() возвращает список ближайших объектов
            foreach (Collider hitCollder in hitColliders) {
                Vector3 direction = hitCollder.transform.position - transform.position; // направление от персонажа к объекту
                if (Vector3.Dot(transform.forward, direction) > .5f) { // сообщение отправляется только при корректной ориентации персонажа
                    hitCollder.SendMessage("Operate", SendMessageOptions.DontRequireReceiver); // метод SendMessage() пытается вызвать функцию независимо от целевого объекта
                }
            }
        }
    }
}
