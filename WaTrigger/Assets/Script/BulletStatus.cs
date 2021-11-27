using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStatus : MonoBehaviour
{
    private float _AttackPoint = 0.0f;

    private float _Range = 0.0f;

    [SerializeField]
    private Rigidbody _Rigidbody=null;

    private Vector3 _ShotPos;

    void Start()
    {
        _ShotPos = this.transform.position;
        GetComponent<TrailRenderer>().startWidth = this.transform.localScale.x;
    }

    void Update()
    {

        //�˒������O��������j�󂷂�
        if (IsOverRange(this.transform.position, _ShotPos, _Range))
        {
            Destroy(this.gameObject);
        }

    }

    #region public
    public void SetStatus(float at,float rg)
    {
        _AttackPoint = at;
        _Range = rg;
    }

    #endregion


    #region private
    private void OnCollisionEnter(Collision collision)
    {
        



    }

    private bool IsOverRange(Vector3 myPos,Vector3 setPos,float range)
    {
        //2�_�Ԃ̋������v�Z
        float distance = Vector3.Distance(myPos, setPos);

        //���݈ʒu���˒������O��������
        if (distance >= range)
        {
            return true;
        }

        return false;
    }

    #endregion
}
