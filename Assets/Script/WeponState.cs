using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeponStateEnum
{
    Non,
    Set_1,
    Set_2,
    Shot,
    Move
}

public class WeponState : MonoBehaviour
{
    [SerializeField]
    private WeponStateEnum _State=WeponStateEnum.Non;

    public WeponStateEnum GetState()
    {
        return _State;
    }

    public void SetState(WeponStateEnum w)
    {
        _State = w;
    }
}
