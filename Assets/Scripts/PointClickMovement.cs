using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointClickMovement : MonoBehaviour
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
    public float decelerarion = 25.0f;
    public float targetBuffer = 1.5f;
    private float _curSpeed = 0f;
    private Vector3 _targetPos = Vector3.one;

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

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        { // задаем целевую точку по щелчку мыши
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // бросаем луч в точку щелчка
            RaycastHit mouseHit;
            if (Physics.Raycast(ray, out mouseHit))
            {
                GameObject hitObject = mouseHit.transform.gameObject;
                if (hitObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    _targetPos = mouseHit.point; // устанавливаем цель в точке попадания луча
                    _curSpeed = moveSpeed;
                }
            }
        }

        if (_targetPos != Vector3.one)
        { // если целевая точка задана, перемещаем
            if (_curSpeed > moveSpeed * .5f)
            {
                Vector3 adjustPos = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);
                Quaternion targetRot = Quaternion.LookRotation(adjustPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            }

            movement = _curSpeed * Vector3.forward;
            movement = transform.TransformDirection(movement);

            if (Vector3.Distance(_targetPos, transform.position) < targetBuffer)
            {
                _curSpeed -= decelerarion * Time.deltaTime; // при приближении к цели снижаем скорость до 0
                if (_curSpeed <= 0)
                {
                    _targetPos = Vector3.one;
                }
            }
        }

        _animator.SetFloat("Speed", movement.sqrMagnitude);

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        { // проверяем, падает ли персонаж
            // высота контроллера персонажа + скругленные углы делится на половину высоты персонажа (луч идёт из центра)
            float check = (_charController.height + _charController.radius) / 1.9f; // расстояние, с которым производится сравнение (слегка выходит за нижнюю часть капсулы)
            hitGround = hit.distance <= check;
        }

        if (hitGround)
        { // свойство isGrounded компонента CharacterController проверяет, соприкасается ли контроллер с поверхностью
            _vertSpeed = minFall;
            _animator.SetBool("Jumping", false);
        } else
        { // если персонаж не стоит на поверхности, применяем гравитацию, пока не будет достигнута предельная скорость
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null)
            {
                _animator.SetBool("Jumping", true);
            }
            if (_charController.isGrounded)
            { // метод бросания лучей не обнаружил поверхность, но капсула с ней соприкасается
                if (Vector3.Dot(movement, _contact.normal) < 0)
                { // реакция меняется в зависимости от того, смотрит ли персонаж в сторону точки контакта
                    movement = _contact.normal * moveSpeed;
                } else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime; // перемещения всегда нужно умножать на deltaTime, чтобы они были независимыми от частоты кадров
        _charController.Move(movement);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;

        Rigidbody body = hit.collider.attachedRigidbody; // проверяем, есть ли у участвующего в столкновении объекта компонент Rigidbody
        if (body != null && !body.isKinematic)
        {
            body.velocity = hit.moveDirection * pushForce; // назначаем физическому телу скорость
        }
    }
}
