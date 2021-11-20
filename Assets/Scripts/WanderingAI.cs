using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAI : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    private GameObject _fireball;
    public float speed = 3.0f; // значения для скорости движения и расстояния, с которого начинается реакция на препятствие
    public float obstacleRange = 5.0f;
    private bool _alive;

    // Start is called before the first frame update
    void Start()
    {
        _alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_alive) {
            transform.Translate(0, 0, speed * Time.deltaTime); // непрерывно движемся вперёд в каждом кадре, несмотря на повороты

            Ray ray = new Ray(transform.position, transform.forward); // луч находится в том же положении и нацеливается в том же направлении, что и персонаж
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.75f, out hit)) { // бросаем луч с описанной вокруг него окружностью
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.GetComponent<PlayerCharacter>()) { // проверяем, игрок ли спереди
                    if (_fireball == null) { // создаём фаерболл там, где стоит враг
                        _fireball = Instantiate(fireballPrefab) as GameObject;
                        _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
                        _fireball.transform.rotation = transform.rotation;
                    }
                } else if (hit.distance < obstacleRange) {
                    float angle = Random.Range(-110, 110); // поворот с наполовину случайным выбором нового направления
                    transform.Rotate(0, angle, 0);
                }
            }
        }
    }

    public void SetAlive(bool alive) {
        _alive = alive;
    }
}
