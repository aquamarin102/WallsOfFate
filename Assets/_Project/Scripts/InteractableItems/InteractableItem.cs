using UnityEngine;
using UnityEngine.SceneManagement;

public enum ResourceType { Gold, Food, PeopleSatisfaction, CastleStrength }

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour, ITriggerable
{
    [Header("Resource Settings")]
    public ResourceType resourceType;
    public int amount = 1;

    [TextArea]
    public string message = "+1 resource";

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;
    public Vector3 spawnOffset = new Vector3(0f, 2.5f, 0f);

    [Header("Post-Use Behavior")]
    public bool destroyAfterUse = true;

    [Header("Move-to settings")]
    [Tooltip("На каком расстоянии от предмета игрок останавливается")]
    [SerializeField] private float approachDistance = 1.2f;

    private Transform _player;
    private bool _hasBeenUsed = false;
    public bool HasBeenUsed => _hasBeenUsed;

    void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go) _player = go.transform;
        else Debug.LogError("Player not found — please tag the player object as 'Player'.");

        // Load state from collection
        CheckUsability();
    }

    private void Update()
    {
        CheckUsability(); 
    }

    private void CheckUsability()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (InteractableItemCollection.TryGetItemState(sceneName, gameObject.name, out bool hasBeenUsed))
        {
            _hasBeenUsed = hasBeenUsed;
            if (_hasBeenUsed)
            {
                if (destroyAfterUse)
                    gameObject.SetActive(false);
                else
                {
                    var col = GetComponent<Collider>();
                    if (col) col.enabled = false;
                }
                foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
                    o.enabled = false;
            }
        }
    }

    void OnMouseUpAsButton()
    {
        if (_hasBeenUsed) return;

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        var mover = playerGO?.GetComponent<PlayerMoveController>();
        if (mover == null) return;

        float approach = 1.2f;              // на каком расстоянии хватит
        mover.MoveToAndCallback(
            /* target  */ this.transform,
            /* run     */ true,
            /* arrive  */ () => Interact(),
            /* stop    */ approach
        );
    }

    // Добавьте в InteractableItem.cs, если ещё нет
    public void ResetForRespawn()
    {
        _hasBeenUsed = false;

        if (TryGetComponent<Collider>(out var col))
            col.enabled = true;

        foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
            o.enabled = true;

        string scene = SceneManager.GetActiveScene().name;
        InteractableItemCollection.SetItemState(scene, gameObject.name, false);
    }



    public void Interact()
    {
        if (_hasBeenUsed) return;
        _hasBeenUsed = true;

        // Save state to collection
        string sceneName = SceneManager.GetActiveScene().name;
        InteractableItemCollection.SetItemState(sceneName, gameObject.name, _hasBeenUsed);

        // 1) Ресурсы
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

        // 2) Всплывающий текст
        if (floatingTextPrefab != null && _player != null)
        {
            Vector3 worldPos = _player.position + spawnOffset;
            var ftGO = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity, _player);
            if (ftGO.TryGetComponent<FloatingText>(out var ft))
                ft.SetText(message);
        }

        // 3) Убираем объект
        if (destroyAfterUse)
            gameObject.SetActive(false);
        else
        {
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
        }

        InteractableItemCollection.SetItemState(SceneManager.GetActiveScene().name, this.gameObject.name, _hasBeenUsed);

        // 4) Отключаем Outline (если был)
        foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
            o.enabled = false;
    }

    public void Triggered() => Interact();

}
