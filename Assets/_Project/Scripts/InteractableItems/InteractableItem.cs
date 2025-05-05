using UnityEngine;
using GameResources;
using Zenject;
using Zenject.SpaceFighter;

public enum ResourceType { Gold, Food, PeopleSatisfaction, CastleStrength }

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour, ITriggerable
{
    public ResourceType resourceType;
    public int amount = 1;

    [TextArea]
    public string message = "+1 resource";

    public GameObject floatingTextPrefab;

    [Header("Смещение точки спавна текста")]
    [Tooltip("Смещение от позиции игрока до точки, где должен появиться текст")]
    public Vector3 spawnOffset = new Vector3(0f, 2.5f, 0f);

    [Header("Поведение после взаимодействия")]
    [Tooltip("Если true — объект удалится после взаимодействия. Иначе — отключится, но останется.")]
    public bool destroyAfterUse = true;

    private Transform _player;
    private bool _hasBeenUsed = false;
    public bool HasBeenUsed => _hasBeenUsed;


    private void Start()
    {
        if (!_player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) _player = go.transform;
            else Debug.LogError("Player not found — тегните объект игрока как 'Player'.");
        }
    }


    public void Interact()
    {
        if (_hasBeenUsed) return;

        _hasBeenUsed = true;

        // 1) Изменяем ресурсы
        switch (resourceType)
        {
            case ResourceType.Gold:
                GameResources.GameResources.ChangeGold(amount);
                break;
            case ResourceType.Food:
                GameResources.GameResources.ChangeFood(amount);
                break;
            case ResourceType.PeopleSatisfaction:
                GameResources.GameResources.ChangePeopleSatisfaction(amount);
                break;
            case ResourceType.CastleStrength:
                GameResources.GameResources.ChangeCastleStrength(amount);
                break;
        }

        // 2) Спавним текст
        if (floatingTextPrefab != null && _player != null)
        {
            Vector3 worldPos = _player.position + spawnOffset;
            GameObject ftGO = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity, _player);
            var ft = ftGO.GetComponent<FloatingText>();
            if (ft != null)
                ft.SetText(message);
        }

        // 3) Реакция после взаимодействия
        if (destroyAfterUse)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // Отключаем коллайдер и (по желанию) визуальный эффект подсветки
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            // Если есть подсветка — отключаем, например:
            var ps = GetComponentInChildren<ParticleSystem>();
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        var outlines = GetComponentsInChildren<cakeslice.Outline>();
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    public void Triggered() => Interact();
}
