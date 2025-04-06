using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField, Tooltip("Максимальное расстояние для захвата ящика")]
    private float grabRange = 2.0f;
    [SerializeField, Tooltip("Клавиша для захвата/отпуска ящика")]
    private KeyCode grabKey = KeyCode.E;

    [Header("Box Movement Settings")]
    [SerializeField, Tooltip("Максимальная скорость ящика")]
    private float maxSpeed = 5f;
    [SerializeField, Tooltip("Скорость поворота ящика (градусы в секунду)")]
    private float turnSpeed = 100f;

    // Ссылка на ящик, за который цепляемся
    public Transform attachedBox = null;
    // Сохраняем исходный слой ящика, чтобы вернуть его при отпускании
    private int originalBoxLayer;
    // Локальный offset между позицией ящика и игрока в момент захвата
    private Vector3 localGrabOffset = Vector3.zero;
    // Текущая скорость движения ящика (положительная – вперёд, отрицательная – назад)
    private float currentSpeed = 0f;

    private void Update()
    {
        // Обработка клавиши захвата/отпуска
        if (Input.GetKeyDown(grabKey))
        {
            if (attachedBox == null)
                TryAttachBox();
            else
                DetachBox();
        }

        if (attachedBox != null)
        {
            // Получаем ввод от игрока
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // Если есть ввод по вертикали – мгновенно устанавливаем скорость
            if (Mathf.Abs(vertical) > 0.1f)
            {
                currentSpeed = vertical * maxSpeed;
            }
            else
            {
                currentSpeed = 0f;
            }

            // Поворот ящика осуществляется с помощью горизонтального ввода (только если есть движение)
            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                float turnAmount = horizontal * turnSpeed * Time.deltaTime;
                // Если движение идет назад, инвертируем поворот для интуитивного управления
                if (vertical < 0)
                    turnAmount = -turnAmount;
                attachedBox.Rotate(0, turnAmount, 0);
            }

            // Перемещаем ящик: он движется в направлении, куда смотрит игрок
            attachedBox.position += transform.forward * currentSpeed * Time.deltaTime;

            // Обновляем позицию игрока, чтобы сохранить исходный локальный offset относительно ящика
            transform.position = attachedBox.TransformPoint(localGrabOffset);

            // Игрок всегда смотрит на ящик (по горизонтали)
            Vector3 dirToBox = attachedBox.position - transform.position;
            dirToBox.y = 0;
            if (dirToBox != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dirToBox);
        }
    }

    // Поиск ближайшего ящика и захват (цепление)
    private void TryAttachBox()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, grabRange);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Box"))
            {
                attachedBox = col.transform;
                // Сохраняем исходный слой ящика
                originalBoxLayer = attachedBox.gameObject.layer;
                // Меняем слой удерживаемого ящика на "HeldBox"
                attachedBox.gameObject.layer = LayerMask.NameToLayer("HeldBox");

                // Вычисляем локальный offset: положение игрока в локальной системе координат ящика
                localGrabOffset = attachedBox.InverseTransformPoint(transform.position);
                currentSpeed = 0f;
                // Делаем игрока дочерним объектом ящика
                transform.SetParent(attachedBox);
                Debug.Log("Цепление: Игрок захватил ящик " + attachedBox.name);
                return;
            }
        }
        Debug.Log("Ящик в зоне захвата не найден.");
    }

    // Отпускание ящика
    private void DetachBox()
    {
        if (attachedBox != null)
        {
            Debug.Log("Цепление снято с ящика " + attachedBox.name);
            // Возвращаем исходный слой ящика
            attachedBox.gameObject.layer = originalBoxLayer;
            // Отсоединяем игрока от ящика
            transform.SetParent(null);
            attachedBox = null;
            currentSpeed = 0f;
        }
    }

    // Для визуализации области захвата в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
