using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Joke", menuName = "ScriptableObjects/Joke", order = 1)]
public class JokeFormat : ScriptableObject
{
    public JokeStuff[] jokeComponents;
}

public enum ThingTypes
{
    Text, Noun, Verb, Adjective
}

[Serializable]
public class JokeStuff
{
    public ThingTypes thingTypes;
    //Only use if text type is used
    public AudioClip audioSource;
}