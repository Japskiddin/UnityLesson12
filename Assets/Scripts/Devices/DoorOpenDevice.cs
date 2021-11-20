using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenDevice : MonoBehaviour
{
    [SerializeField] private Vector3 dPos; // смещение открытой двери относительно закрытой
    private bool _open; // слежение за открытым состоянием двери

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Operate() {
        if (_open) { // открываем или закрываем дверь в зависимости от её состояния
            Vector3 pos = transform.position - dPos;
            transform.position = pos;
        } else {
            Vector3 pos = transform.position + dPos;
            transform.position = pos;
        }
        _open = !_open;
    }

    public void Activate() {
        if (!_open) { // открывает дверь при условии, что она закрыта
            Vector3 pos = transform.position + dPos;
            transform.position = pos;
            _open = true;
        } 
    }

    public void Deactivate() {
        if (_open) { // закрывает дверь при условии, что она открыта
            Vector3 pos = transform.position - dPos;
            transform.position = pos;
            _open = false;
        }
    }
}
