using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;


// CREDITOS:
// https://github.com/trevermock/ink-dialogue-system/blob/7-dialogue-audio-implemented/Assets/Scripts/Dialogue/DialogueVariables.cs
// https://www.youtube.com/watch?v=fA79neqH21s
public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;

    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        Debug.Log("DialogueVariables created start");
        // create the story
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        // initialize the dictionary
        InitializeVariablesDictionary();
        Debug.Log("DialogueVariables created finished");
    }

    public void LoadJsonState(string jsonState)
    {
        Debug.Log("DialogueVariables LoadJsonState fired ");
        Debug.Log(jsonState);
        globalVariablesStory.state.LoadJson(jsonState);
        Debug.Log("DialogueVariables LoadJson completed");
        InitializeVariablesDictionary();
        Debug.Log("Dictionary initialized completed");
    }

    public string GetJsonState()
    {
        VariablesToStory(globalVariablesStory);
        return globalVariablesStory.state.ToJson();
    }

    public void SetVariable<T>(string key, T primitiveValue)
    {
        globalVariablesStory.variablesState[key] = primitiveValue;
        Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(key);

        if(!value)
        {
            Debug.LogWarning("Error while setting dialogue variable with name '" 
                + key + "' and primitive value '" + primitiveValue + "'");

            return;
        }

        if (!variables.ContainsKey(key))
        {
            Debug.LogWarning("Trying to set an invalid dialogue variable with key: " + key);
            return;
        }
        
        VariableChanged(key, value);
    }


    public T GetVariable<T>(string key)
    {
        if (!variables.ContainsKey(key))
            return default(T);

        Ink.Runtime.Object value = variables[key];

        try
        {
            return (T)(value as Value).valueObject;
        }
        catch (InvalidCastException e)
        {
            Debug.LogWarning(e.Message);
            Debug.LogWarning("GetVariable changetype error with key '" + key + "' returning default value: " + default(T));
            return default(T);
        }
    }


    public void StartListening(Story story)
    {
        // it's important that VariablesToStory is before assigning the listener!
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        // only maintain variables that were initialized from the globals ink file
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    private void InitializeVariablesDictionary()
    {
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

}