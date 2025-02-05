using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonNPC : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color charmanderColor = Color.red;
    [SerializeField] private Color bulbasaurColor = Color.green;
    [SerializeField] private Color squirtleColor = Color.blue;
    [SerializeField] private GameObject _miniGame;

    private Renderer colorRenderer;

    private void Start()
    {
        colorRenderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        string pokemonName = ((Ink.Runtime.StringValue) DialogueManager
            .GetInstance()
            .GetVariablesState("pokemon_name")).value;

        switch(pokemonName)
        {
            case "":
                //colorRenderer.material.color = defaultColor;
                break;
            case "Charmander":
                _miniGame.SetActive(false);
                //colorRenderer.material.color = charmanderColor;
                break;
            case "Bulbasaur":
                //colorRenderer.material.color = bulbasaurColor;
                break;
            case "Squirtle":
                //colorRenderer.material.color = squirtleColor;
                break;
        }
    }
}
