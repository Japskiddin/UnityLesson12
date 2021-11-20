using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // ссылка на объект, вокруг которого происходит облет
    public float rotSpeed = 1.5f;
    private float _rotY;
    private Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
        _rotY = transform.eulerAngles.y;
        _offset = target.position - transform.position; // сохраняем начальное смещение между камерой и целью
    }

    public void LateUpdate() {
        _rotY -= Input.GetAxis("Horizontal") * rotSpeed;

        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
        transform.position = target.position - (rotation * _offset); // поддерживаем начальное смещение, сдвигаемое в соответствии с поворотом камеры
        transform.LookAt(target); // где бы ни находилась камера, она всегда смотрит на цель
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
