using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDevice : MonoBehaviour
{
    public float radius = 3.5f;

    public void OnMouseDown() { // функция, запускаемая щелчком
        Transform player = GameObject.FindWithTag("Player").transform;
        if (Vector3.Distance(player.position, transform.position) < radius) {
            Vector3 direction = transform.position - player.position;
            if (Vector3.Dot(player.forward, direction) > .5f) {
                Operate(); // если персонаж рядом с устройством и стоит к нему лицом, вызываем метод Operate()
            }
        }
    }

    public virtual void Operate() { // ключевое слово virtual указывает на метод, который можно переопределить после наследования
        // здесь код поведения конкретного устройства
    }
}
