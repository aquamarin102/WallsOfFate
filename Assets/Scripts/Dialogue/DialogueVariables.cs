using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set;}

    private Story _globalVariablesStory;

    private const string _saveVariablesKey = "INK_VARIABLES";

    public DialogueVariables(TextAsset loadGlobalJSON)
    {
        _globalVariablesStory = new Story(loadGlobalJSON.text);
        //if we have saved data, load it
        //if(PlayerPrefs.HasKey(_saveVariablesKey))
        //{
        //    string jsonState = PlayerPrefs.GetString(_saveVariablesKey);
        //    _globalVariablesStory.state.LoadJson(jsonState);
        //}

        // initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in _globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = _globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialigue variable: " + name + " = " + value);
        }
    }

    //public void SaveVariables()
    //{
    //    if (_globalVariablesStory != null)
    //    {
    //        VariablesToStory(_globalVariablesStory);

    //        // NOTE: eventually, you'd want to replace this with an actual save/load method
    //        // rather than using PlayerPrefs.
    //        //PlayerPrefs.SetString(_saveVariablesKey, _globalVariablesStory.state.ToJson());
    //    }
    //}

    public void StartListening(Story story)
    {
        // it's important that VariablesTostory is before assigning the listener
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {

        //only maintain variables that were init from the globals ink file
        Debug.Log("Variable changed: " + name + " = " + value);
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
