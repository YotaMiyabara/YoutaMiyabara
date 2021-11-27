using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Wepon",menuName ="CreateWepon")]
public class Wepon : ScriptableObject
{
    [SerializeField, Tooltip("ID")]
    private int ID;

    [SerializeField, Tooltip("名前")]
    private string Name;

    [SerializeField, Tooltip("速さ")]
    private float Speed;

    [SerializeField, Tooltip("攻撃力")]
    private float AttackPower;

    [SerializeField, Tooltip("射程距離")]
    private float Range;

    [SerializeField, Tooltip("コスト")]
    private int Cost;

    public int GetID()
    {
        return ID;
    }

    public string GetName()
    {
        return Name;
    }

}
