using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerBoxGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("Максимальное расстояние для захвата ящика")]
    [SerializeField] private float grabRange = 2.0f;
    [Tooltip("Клавиша для захвата/отпуска ящика")]
    [SerializeField] private KeyCode grabKey = KeyCode.E;
    [Tooltip("Скорость перемещения ящика при цеплении")]
    [SerializeField] private float boxMoveSpeed = 3.0f;

    private Transform cameraTransform;

    [Inject]
    private void Construct(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    // Ссылка на ящик, за который цепляемся
    private Transform attachedBox = null;
    // Относительный offset между позицией ящика и игрока в момент захвата
    private Vector3 grabOffset = Vector3.zero;

    void Update()
    {
        // Нажатие клавиши для захвата/отпуска
        if (Input.GetKeyDown(grabKey))
        {
            if (attachedBox == null)
            {
                TryAttachBox();
            }
            else
            {
                DetachBox();
            }
        }

        // Если цепление активно
        if (attachedBox != null)
        {
            // Получаем сырой ввод для перемещения
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(horizontal, 0, vertical);

            if (input.sqrMagnitude > 0.01f)
            {
                input = input.normalized;
                // Получаем изометрические оси из камеры:
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0;
                camForward.Normalize();
                Vector3 camRight = cameraTransform.right;
                camRight.y = 0;
                camRight.Normalize();

                // Итоговое направление движения рассчитываем относительно камеры,
                // чтобы "вперёд" соответствовало направлению, которое задаёт камера
                Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

                // Перемещаем ящик по вычисленному направлению
                attachedBox.position += moveDir * boxMoveSpeed * Time.deltaTime;
            }

            // Обновляем позицию игрока, чтобы сохранить относительный offset от ящика
            transform.position = attachedBox.position + grabOffset;
        }
    }

    // Поиск ближайшего ящика и цепление за него
    private void TryAttachBox()
    {
        // Ищем все объекты в радиусе grabRange от игрока
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange);
        foreach (Collider col in colliders)
        {
            // Проверяем тег (например, "Box")
            if (col.CompareTag("Box"))
            {
                attachedBox = col.transform;
                // Запоминаем offset между позицией игрока и ящика
                grabOffset = transform.position - attachedBox.position;
                Debug.Log("Цепление: Игрок цепляется за ящик " + attachedBox.name);
                return;
            }
        }
        Debug.Log("Ящик в зоне захвата не найден.");
    }

    // Снятие цепления
    private void DetachBox()
    {
        if (attachedBox != null)
        {
            Debug.Log("Цепление снято с ящика " + attachedBox.name);
            attachedBox = null;
        }
    }

    // Визуализация области захвата
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
