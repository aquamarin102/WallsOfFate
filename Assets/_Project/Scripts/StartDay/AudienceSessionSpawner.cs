using UnityEngine;
using System.Collections.Generic;
using Quest;
using UnityEngine.SceneManagement;

public class AudienceSessionSpawner : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _dialogSpot;
    [SerializeField] private Transform _exitSpot;

    [Header("Main Quest Givers ─ day 0 / day 1 / day 2")]
    [SerializeField] private NPCDefinition[] _mainQuestGivers;

    private readonly Queue<NPCDefinition> _sessionQueue = new();
    private NPCController _current;

    private void Start()
    {
        // формируем очередь и подписываемся
        for (int i = 0; i < 3; i++)
            if (AudiencePool.Instance.TryTakeRandom(out var def))
                _sessionQueue.Enqueue(def);

        // добавляем «босса»…
        int day = Quest.QuestCollection.CurrentDayNumber;
        if (day < _mainQuestGivers.Length)
            _sessionQueue.Enqueue(_mainQuestGivers[day]);

        if (_sessionQueue.Count == 0)
        {
            EndSession();               // сразу переходим, если никого нет
            return;
        }

        DialogueManager.GetInstance().DialogueFinished += OnDialogueFinished;
        SpawnNextFromQueue();
    }

    private void SpawnNextFromQueue()
    {
        if (_sessionQueue.Count == 0)
        {
            EndSession();               // очередь закончена — переходим дальше
            return;
        }

        NPCDefinition def = _sessionQueue.Dequeue();
        GameObject go = Instantiate(def.prefab, _spawnPoint.position, _spawnPoint.rotation);

        _current = go.GetComponent<NPCController>();
        _current.Init(_dialogSpot, _exitSpot);

        _current.Arrived += npc => { /* optional bow */ };
        _current.Left += OnNpcLeft;
    }

    private void EndSession()
    {
        // здесь можно вставить any cleanup / анимацию ухода
        Debug.Log("Приём окончен — загружаем следующую сцену");
        LoadingScreenManager.Instance.LoadScene("MainRoom");  // ← укажите имя сцены или её индекс
    }

    private void OnDialogueFinished() => _current?.Leave();

    private void OnNpcLeft(NPCController npc)
    {
        npc.Left -= OnNpcLeft;
        Destroy(npc.gameObject, 0.3f);
        SpawnNextFromQueue();            // переходим к следующему из троих
    }

    private void OnDestroy()
    {
        if (DialogueManager.HasInstance)
            DialogueManager.GetInstance().DialogueFinished -= OnDialogueFinished;
    }
}
