using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

public class NormalBullet : MonoBehaviour
{
    #region define


    #endregion


    #region SerializeField
    [SerializeField, Tooltip("弾丸の速さ（初速）")]
    private float _BulletSpeed=2000.0f;

    [SerializeField, Tooltip("攻撃力")]
    private float _AttackPoint=10.0f;

    [SerializeField, Tooltip("射程距離")]
    private float _ShotReng = 5.0f;


    [SerializeField]
    Rigidbody _RigidBody = null;

    [SerializeField]
    private GameObject _Bullet;

    [SerializeField]
    private WeponState _WeponState=null;

    
    #endregion

    #region PublicVariable




    #endregion

    #region PurivateVariable

    //発射した位置
    private Vector3 _ShotPos;

    private Vector3 _SpritPos;
    private Vector3 _LPos;
    private TrailRenderer _LineRender=null;

    //Flag
    private bool IsBeforSpritPos = false;


    #endregion

    #region UnityFunction

    void Start()
    {
        _WeponState.SetState(WeponStateEnum.Non);

        _LineRender = transform.GetComponent<TrailRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        switch (_WeponState.GetState())
        {
            case WeponStateEnum.Non:
                {
                    _LineRender.enabled = false;
                    transform.localPosition = _LPos;
                }
                break;
            case WeponStateEnum.Set_1:
                {
                   
                }
                break;
            case WeponStateEnum.Set_2:
                {

                }
                break;
            case WeponStateEnum.Shot:
                {
                    BulletShot();
                    GetShotLocalPos();
                }
                break;
            case WeponStateEnum.Move:
                {
                    DestroySeparated();
                }
                break;

        }

    }

    #endregion

    #region PublicFunction
    public void SetPos(Vector3 pos)
    {
        this.transform.position = pos;
    }

    public void SetLocalPosition(Vector3 pos)
    {

        _LPos = pos;
    }

    public void SetSpritPos(Vector3 pos)
    {
        _SpritPos = pos;
        IsBeforSpritPos = true;
    }

    public Vector3 GetSpritPos()
    {
        return _SpritPos;
    }

    #endregion

    #region PurivateFunction
    private void BulletShot()
    {
        //弾丸を作り発射
        GameObject obj = Instantiate(_Bullet, this.transform.position, Quaternion.identity);

        //弾丸の大きさ調整
        obj.transform.localScale = this.transform.localScale/2.0f;

        //ステータスの設定
        obj.GetComponent<BulletStatus>().SetStatus(_AttackPoint, _ShotReng);

        obj.GetComponent<Rigidbody>().AddForce(transform.forward * _BulletSpeed, ForceMode.Impulse);

        //武器を消す
        Destroy(this.gameObject);
    }


    private void GetShotLocalPos()
    {
        _ShotPos = transform.localPosition;
    }

    private void DestroySeparated()
    {
        float distance = Vector3.Distance(_ShotPos, transform.localPosition);
        //一定距離（射程距離以上）で破壊する
        if (distance >= _ShotReng)
        {
            Destroy(this.gameObject);

        }


    }


    #endregion
}
