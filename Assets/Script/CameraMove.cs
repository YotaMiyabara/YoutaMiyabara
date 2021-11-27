using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField, Tooltip("Player")]
    private GameObject _Player;

    [SerializeField, Tooltip("回転速度")]
    private float _TurnSpeed = 1.0f;


    private Transform _PlayerTransform;
    

    private Vector3 _DistanceOffset;

    void Start()
    {
        _PlayerTransform = _Player.transform;

        _DistanceOffset = this.transform.position - _PlayerTransform.position;//自分とPlayerとの相対距離を求める
    }

    void Update()
    {
        //入力
        float vertical = Input.GetAxis("Horizontal2");


        this.transform.position = _PlayerTransform.position + _DistanceOffset;//新しいトランスフォームの値を代入する( 追跡用)

        RotationMove(vertical);



    }

    private void RotationMove(float v1)
    {
        if (v1 > 0)//反時計回り
        {
            this.transform.RotateAround(_PlayerTransform.position, Vector3.up, _TurnSpeed);
            _DistanceOffset = this.transform.position - _PlayerTransform.position;
        }

        if (v1 < 0)//時計回り
        {
            this.transform.RotateAround(_PlayerTransform.position, Vector3.up, -_TurnSpeed);
            _DistanceOffset = this.transform.position - _PlayerTransform.position;
        }
    }


}
