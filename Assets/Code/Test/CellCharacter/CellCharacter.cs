using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// cell再odinwindow的数据结构
/// </summary>
public class CellCharacter : SerializedScriptableObject
{
    [HorizontalGroup("Split", 55, LabelWidth = 70)]
    [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
    public Texture Icon;

    [VerticalGroup("Split/Meta")]
    public string Name;

    [VerticalGroup("Split/Meta")]
    public string Surname;

    [VerticalGroup("Split/Meta"), Range(0, 100)]
    public int Age;

    [HorizontalGroup("Split", 290), EnumToggleButtons, HideLabel]
    public CellEvent CharacterAlignment;

   
}
