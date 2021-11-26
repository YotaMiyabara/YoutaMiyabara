using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Wepon",menuName ="CreateWepon")]
public class Wepon : ScriptableObject
{
    [SerializeField, Tooltip("ID")]
    private int ID;

    [SerializeField, Tooltip("���O")]
    private string Name;

    [SerializeField, Tooltip("����")]
    private float Speed;

    [SerializeField, Tooltip("�U����")]
    private float AttackPower;

    [SerializeField, Tooltip("�˒�����")]
    private float Range;

    [SerializeField, Tooltip("�R�X�g")]
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
