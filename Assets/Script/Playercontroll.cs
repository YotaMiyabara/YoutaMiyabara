using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Trigger
{
    public GameObject Obj;
    public WeponState State;


    public Trigger(GameObject obj)
    {
        this.Obj = obj;
        this.State = this.Obj.GetComponent<WeponState>();
    }

}

public class Playercontroll : MonoBehaviour
{
    #region define
    public enum StateEnum
    {
        Non,
        Move,
        Dead
    }

    #endregion


    #region SerializeFild
    [SerializeField, Tooltip("移動速度")]
    private float _MoveSpeed = 10.0f;

    [SerializeField, Tooltip("体力")]
    private float _HP=500;

    [SerializeField]
    private Rigidbody _RigidBody=null;

    [SerializeField]
    private Animator _Animetor = null;

    #endregion

    public WeponDataList _weponList;


    private StateEnum _State=StateEnum.Non;

    private Transform _CameraTransform;


    private int _MainWeponCounter = 0;
    private int _SubWeponCounter = 0;
    private float _CameraFoundTime = 0;

    #region Weapon
    [SerializeField, Tooltip("MainWepon_ID")]
    private List<int> _MainWeponList = new List<int>(4);

    [SerializeField, Tooltip("SubWepon_ID")]
    private List<int> _SubWeponList = new List<int>(4);

    [SerializeField]
    private List<Trigger> _UseMainWepon=new List<Trigger>();

    private List<GameObject> _UseSubWepon=new List<GameObject>();

    #endregion


    #region Unityfunction
    void Start()
    {
        _State = StateEnum.Move;
        _CameraTransform = Camera.main.transform;
        SetWepon();
    }

    
    void Update()
    {
        switch (_State)
        {
            case StateEnum.Non:
                {

                }
                break;
            case StateEnum.Move:
                {
                    PlayerControll();
                }
                break;
            case StateEnum.Dead:
                {

                }
                break;
        }

         


    }
    #endregion

    #region PublicFunction

    #endregion


    #region PrivetFunction
    private void SetWepon()
    {

        for(int i = 0; i < 4; ++i)
        {
            if (_MainWeponList[i] != 0)
            {
                _UseMainWepon.Add(new Trigger(Instantiate(SerchWepon(_MainWeponList[i]), transform.position, Quaternion.identity)));
                //_UseMainWepon[i].transform.parent = this.transform;
            }

            if (_SubWeponList[i] != 0)
            {
                _UseSubWepon.Add(Instantiate(SerchWepon(_SubWeponList[i]), transform.position, Quaternion.identity));
                _UseSubWepon[i].transform.parent = this.transform;
            }
            
           
        }

    }

    private GameObject SerchWepon(int id)
    {
        foreach (Wepon w in _weponList.wepons)
        {
            if (id == w.GetID())
            {
                string path ="Prefab/"+w.GetName() + "_Prefab";
                //読み込み処理
                var obj = Resources.Load<GameObject>(path);
                if (obj == null)
                {
                    Debug.Log("読み込み失敗");
                    return null;
                }
                return obj;
            }
            else if (id == 0)
            {
                Debug.Log("NoData");
                return null;
            }
        }
        Debug.Log("データがない");
        return null;
    }


    //Playerの操作
    private void PlayerControll()
    {
        var cameraForward=Vector3.Scale(_CameraTransform.forward,new Vector3(1,0,1)).normalized;

        ///移動

        //前進後進
        float Ver = Input.GetAxisRaw("Vertical");

        //左右
        float Hori = Input.GetAxisRaw("Horizontal");

        RunAnime(Ver, Hori);
        RotationMove(Hori, Ver);
        _RigidBody.PlayerControll(Hori, Ver, _MoveSpeed);

        ///Main攻撃
        MainOn();
        MainShot();


        ///Sub攻撃
        SubOn();
        SubShot();

        ///武器変更
        ChangeWepon();



    }

    //Playerの向き決定
    private void RotationMove(float h1,float v1)
    {
        var cameraForward = Vector3.Scale(_CameraTransform.forward, new Vector3(1, 0, 1)).normalized;


        if (h1 != 0 || v1 != 0)
        {
            /* Vector3 direction = cameraForward * v1 + _CameraTransform.right * h1;
             Quaternion targetRotation = Quaternion.LookRotation(direction - this.transform.position);
             this.transform.localRotation = Quaternion.Slerp(targetRotation,transform.rotation,Time.deltaTime*0.5f);*/


            Vector3 target = new Vector3(h1, 0.0f, v1);

            //体の向きを変化
            Quaternion rotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
        }
    }

    private void MainOn()
    {
        if(Input.GetKeyDown("joystick button 7")&&_UseMainWepon[_MainWeponCounter].State.GetState()==WeponStateEnum.Non)
        {
            _UseMainWepon[_MainWeponCounter].State.SetState(WeponStateEnum.Set_1);

            var PlayerForward = Vector3.Scale(this.transform.forward, new Vector3(1, 0, 1)).normalized;

            //位置を設定
            Vector3 pos = PlayerForward+transform.position;
            pos.y += 1.0f;
            pos.z += 1.0f;
            _UseMainWepon[_MainWeponCounter].Obj.transform.position = pos;
        }
    }


    private void MainShot()
    {
        if(Input.GetKeyDown("joystick button 3")&& _UseMainWepon[_MainWeponCounter].State.GetState() == WeponStateEnum.Set_2)
        {
            _UseMainWepon[_MainWeponCounter].State.SetState(WeponStateEnum.Shot);
        }

    }

    private void SubOn()
    {
        if (Input.GetKeyDown("joystick button 6")&& _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().GetState() == WeponStateEnum.Non)
        {
            _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().SetState(WeponStateEnum.Set_1);
        }
    }

    private void SubShot()
    {
        if (Input.GetKeyDown("joystick button 0")&& _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().GetState() == WeponStateEnum.Set_2)
        {
            _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().SetState(WeponStateEnum.Shot);
        }
    }

    //武器チェンジ
    private int CounterUp(ref int count)
    {
        _MainWeponCounter++;
        _MainWeponCounter = _MainWeponCounter % 4;
        return _MainWeponCounter;

    }
    private void ChangeWepon()
    {
        //メイン武器
        if (Input.GetKeyDown("joystick button 5"))
        {
            CounterUp(ref _MainWeponCounter);
        }
        //サブ武器
        if(Input.GetKeyDown("joystick button 4"))
        {
            CounterUp(ref _SubWeponCounter);
        }

    }

    //==========================================================================================
    //==========================================================================================
    //アニメーション

    private void RunAnime(float x,float y)
    {
        _Animetor.SetBool("IsRun", false);

        if (x != 0.0f || y != 0.0f)
        {
            _Animetor.SetBool("IsRun", true);
        }
    }

    #endregion


    #region コルーチン
   

    #endregion

}
