using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 测试使用的odin数据结构
/// </summary>
public class OdinCharacterWrapper 
{
    [TableColumnWidth(50, false)] [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
    public Texture Icon;

    [TableColumnWidth(120)]
    [ShowInInspector]
    public string Name;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Shooting;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Melee ;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Social ;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Animals ;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Medicine ;

    [ShowInInspector, ProgressBar(0, 100)]
    public float Crafting ;
}
