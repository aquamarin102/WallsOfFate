using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    [SerializeField] private GameObject _miniGame;
    [SerializeField] private GameObject _castle;
    [SerializeField] private CameraSwitch _switch;
    [SerializeField] private MiniGamePlayer _enemy;
    [SerializeField] private MiniGamePlayer _player;

    private bool _cheakCamera = false;

    private void Start()
    {
        _switch = GetComponentInChildren<CameraSwitch>();
        _player.OnPlayerDeath += EndMiniGame;
        _enemy.OnPlayerDeath += EndMiniGame;
    }

    private void Update()
    {
        string pokemonName = ((Ink.Runtime.StringValue)DialogueManager
            .GetInstance()
            .GetVariablesState("pokemon_name")).value;


        if (pokemonName == "Charmander" && !_cheakCamera)
        {
            _miniGame.SetActive(true);
            _switch.SwitchCamera();
            _castle.SetActive(false);
            _cheakCamera = true;
        }
    }

    public void EndMiniGame(string playerName)
    {
        _miniGame.SetActive(false);
        _castle.SetActive(true);
        _switch.SwitchCamera();
    }
    
}
