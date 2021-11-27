using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField, Tooltip("Player")]
    private GameObject _Player;

    [SerializeField, Tooltip("��]���x")]
    private float _TurnSpeed = 1.0f;


    private Transform _PlayerTransform;
    

    private Vector3 _DistanceOffset;

    void Start()
    {
        _PlayerTransform = _Player.transform;

        _DistanceOffset = this.transform.position - _PlayerTransform.position;//������Player�Ƃ̑��΋��������߂�
    }

    void Update()
    {
        //����
        float vertical = Input.GetAxis("Horizontal2");


        this.transform.position = _PlayerTransform.position + _DistanceOffset;//�V�����g�����X�t�H�[���̒l��������( �ǐ՗p)

        RotationMove(vertical);



    }

    private void RotationMove(float v1)
    {
        if (v1 > 0)//�����v���
        {
            this.transform.RotateAround(_PlayerTransform.position, Vector3.up, _TurnSpeed);
            _DistanceOffset = this.transform.position - _PlayerTransform.position;
        }

        if (v1 < 0)//���v���
        {
            this.transform.RotateAround(_PlayerTransform.position, Vector3.up, -_TurnSpeed);
            _DistanceOffset = this.transform.position - _PlayerTransform.position;
        }
    }


}
