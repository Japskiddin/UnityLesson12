using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target; // ссылка на объект, относительно которого происходит перемещение
    public float rotSpeed = 15f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f; // предельная скорость падения
    public float minFall = -1.5f; // минимальная скорость падения
    public float pushForce = 3.0f; // величина прилагаемой силы
    private float _vertSpeed;
    private CharacterController _charController;
    private ControllerColliderHit _contact; // для хранение данных о столкновении между функциями
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _vertSpeed = minFall; // инициализируем переменную вертикальной скорости, присваивая ей минимальную скорость падения в начале
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero; // начинаем с вектора (0,0,0), постепенно добавляя компоненты движения
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horInput != 0 || vertInput != 0) { // движение обрабатывается только при нажатии клавиш со стрелками
            movement.x = horInput * moveSpeed;
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed); // ограничиваем движение по диагонали той же скоростью, что и движение вдоль оси

            Quaternion tmp = target.rotation; // сохраняем начальную ориентацию, чтобы вернуться к ней после завершения работы с целевым объектом
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement); // преобразуем направление движения из локальных координат в глобальные
            target.rotation = tmp;

            Quaternion direction = Quaternion.LookRotation(movement); // метод LookRotation() вычисляет кватернион, смотрящий в этом направлении
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit)) { // проверяем, падает ли персонаж
            // высота контроллера персонажа + скругленные углы делится на половину высоты персонажа (луч идёт из центра)
            float check = (_charController.height + _charController.radius) / 1.9f; // расстояние, с которым производится сравнение (слегка выходит за нижнюю часть капсулы)
            hitGround = hit.distance <= check;
        }

        _animator.SetFloat("Speed", movement.sqrMagnitude);

        if (hitGround) { // свойство isGrounded компонента CharacterController проверяет, соприкасается ли контроллер с поверхностью
            if (Input.GetButtonDown("Jump")) { // реакция на кнопку Jump при нахождении на поверхности
                _vertSpeed = jumpSpeed;
            } else {
                _vertSpeed = minFall;
                _animator.SetBool("Jumping", false);
            }
        } else { // если персонаж не стоит на поверхности, применяем гравитацию, пока не будет достигнута предельная скорость
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity) {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null) {
                _animator.SetBool("Jumping", true);
            }
            if (_charController.isGrounded) { // метод бросания лучей не обнаружил поверхность, но капсула с ней соприкасается
                if (Vector3.Dot(movement, _contact.normal) < 0) { // реакция меняется в зависимости от того, смотрит ли персонаж в сторону точки контакта
                    movement = _contact.normal * moveSpeed;
                } else {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime; // перемещения всегда нужно умножать на deltaTime, чтобы они были независимыми от частоты кадров
        _charController.Move(movement);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        _contact = hit;

        Rigidbody body = hit.collider.attachedRigidbody; // проверяем, есть ли у участвующего в столкновении объекта компонент Rigidbody
        if (body != null && !body.isKinematic) {
            body.velocity = hit.moveDirection * pushForce; // назначаем физическому телу скорость
        }
    }
}