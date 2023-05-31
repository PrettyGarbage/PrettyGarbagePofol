using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

[Serializable]
public class DialogueBody
{
    [field: SerializeField, TextArea] public string Message { get; private set; }
    [field: SerializeField] public AudioClip Clip { get; private set; }
    [field: SerializeField, Range(0, 1)] public float Volume { get; private set; } = 0.5f;
}

///<summary>
///대사데이터
///</summary>
[Serializable]
public class Dialogue
{
    [field: SerializeField] public Define.SubtitleType Type { get; private set; }
    [field: SerializeField] public List<DialogueBody> Body { get; private set; }
    [field: SerializeField] public InteractableObjectModel[] InteractableObjects { get; private set; }
    
    public string Title => Type switch
    {
        Define.SubtitleType.Guide => "지시 자막",
        Define.SubtitleType.Description => "상황 자막",
        Define.SubtitleType.Shouting => "대사 자막",
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };
}
