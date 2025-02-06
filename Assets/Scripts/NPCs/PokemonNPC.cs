using System.Collections;
using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _castle;
    [SerializeField] private GameObject _dialogPanel;
    [SerializeField] private CameraSwitch _switch;
    [SerializeField] private GameProcess _miniGameProcessor;

    private bool _isMiniGameActive = false; // Отвечает за старт мини-игры
    private bool _isMiniGameFinished = false; // Отвечает за конец мини-игры
    private bool _isWaitingForMiniGame = false; // Флаг ожидания перед стартом мини-игры

    private void Start()
    {
        _switch = GetComponentInChildren<CameraSwitch>();
        _miniGameProcessor.OnEndGame += EndMiniGame;
    }

    private void Update()
    {
        if (_isMiniGameActive || _isMiniGameFinished || _isWaitingForMiniGame)
            return; // Не запускаем мини-игру повторно

        string pokemonName = ((Ink.Runtime.StringValue)DialogueManager
            .GetInstance()
            .GetVariablesState("pokemon_name")).value;

        if (pokemonName == "Charmander")
        {
            StartMiniGame();
        }
    }

    private void StartMiniGame()
    {
        _switch.SwitchCamera();
        _miniGame.SetActive(true);
        _castle.SetActive(false);
        _isMiniGameActive = true;
    }

    public void EndMiniGame(string winnerName, string loserName)
    {
        if (_isMiniGameFinished) return; // Не вызываем повторно

        Debug.LogWarning("Activate end game");
        _switch.SwitchCamera();
        _castle.SetActive(true);
        _miniGame.SetActive(false);
        _isMiniGameFinished = true;

        //StartCoroutine(ResetMiniGameState());
    }
}
