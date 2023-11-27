using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialoguePiece
{
    public string ID;
    public Sprite image;
    [TextArea] public string text;
    public List<DialogueOption> options = new List<DialogueOption>();
}