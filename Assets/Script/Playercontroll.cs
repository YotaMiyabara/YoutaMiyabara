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

    [SerializeField, Tooltip("回転速度")]
    private float _TurnSpeed = 10.0f;

    [SerializeField, Tooltip("体力")]
    private float _HP = 500;



    [SerializeField]
    private Rigidbody _RigidBody = null;

    [SerializeField]
    private Animator _Animetor = null;

    #endregion

    public WeponDataList _weponList;


    private StateEnum _State = StateEnum.Non;

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
    private List<Trigger> _UseMainWepon = new List<Trigger>();

    private List<GameObject> _UseSubWepon = new List<GameObject>();

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

        for (int i = 0; i < 4; ++i)
        {
            if (_MainWeponList[i] != 0)
            {
                _UseMainWepon.Add(new Trigger(Instantiate(SerchWepon(_MainWeponList[i]), transform.position, Quaternion.identity)));
                _UseMainWepon[i].Obj.transform.parent=this.transform;
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
        foreach (Wepon w in _weponList.wepons)//武器リストから検索
        {
            if (id == w.GetID())
            {
                string path = "Prefab/" + w.GetName() + "_Prefab";
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


        ///移動

        //前進後進
        float Ver = Input.GetAxisRaw("Vertical");

        //左右
        float Hori = Input.GetAxisRaw("Horizontal");

        var AnimeMotion = _Animetor.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (AnimeMotion == "Stand" || AnimeMotion == "Run")
        {
            _RigidBody.PlayerControll(Hori, Ver, _MoveSpeed);
            RunAnime(Ver, Hori);
            RotationMove(Hori, Ver);
        }
        

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
    private void RotationMove(float h1, float v1)
    {
        var cameraForward = Vector3.Scale(_CameraTransform.forward, new Vector3(1, 0, 1)).normalized;

        if (h1 != 0 || v1 != 0)
        {
            //カメラの向きから移動方向を決定
            Vector3 moveForward = cameraForward * v1 + Camera.main.transform.right * h1;

            //体の向きを変化
            Quaternion rotation = Quaternion.LookRotation(moveForward);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * _TurnSpeed);
        }
    }

    private void MainOn()
    {
        if (Input.GetKeyDown("joystick button 7") && _UseMainWepon[_MainWeponCounter].State.GetState() == WeponStateEnum.Non)
        {
            _UseMainWepon[_MainWeponCounter].State.SetState(WeponStateEnum.Set_1);


            //位置を設定
            WeponOffsetPos(0);
        }
    }


    private void MainShot()
    {
        if (Input.GetKeyDown("joystick button 3") && _UseMainWepon[_MainWeponCounter].State.GetState() == WeponStateEnum.Set_2)
        {
            _UseMainWepon[_MainWeponCounter].State.SetState(WeponStateEnum.Shot);
            TriggerMotionAnim(_MainWeponList[_MainWeponCounter]);
        }

    }

    private void SubOn()
    {
        if (Input.GetKeyDown("joystick button 6") && _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().GetState() == WeponStateEnum.Non)
        {
            _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().SetState(WeponStateEnum.Set_1);
        }
    }

    private void SubShot()
    {
        if (Input.GetKeyDown("joystick button 0") && _UseSubWepon[_SubWeponCounter].GetComponent<WeponState>().GetState() == WeponStateEnum.Set_2)
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
        if (Input.GetKeyDown("joystick button 4"))
        {
            CounterUp(ref _SubWeponCounter);
        }

    }

    private void WeponOffsetPos(int right)//Mainなら右＝0、Subなら左＝1
    {
        float[] table =new float[2]{ 1.5f,-1};

        Vector3 Offset = new Vector3(table[right], 1.0f, 1.0f);

        var myForward = this.transform.forward;

        switch (_MainWeponList[_MainWeponCounter]%100)
        {
            case 1:
                {
                    _UseMainWepon[_MainWeponCounter].Obj.transform.localPosition=Offset;
                }
                break;
            default:
                break;
        }

    }


    //==========================================================================================
    //==========================================================================================
    //アニメーション

    private void RunAnime(float x, float y)
    {
        _Animetor.SetBool("IsRun", false);

        if (x != 0.0f || y != 0.0f)
        {
            _Animetor.SetBool("IsRun", true);
        }
    }

    private void TriggerMotionAnim(int id)
    {
        if (Input.GetAxisRaw("Vertical")!=0|| Input.GetAxisRaw("Horizontal") != 0)
        {
            return;
        }

        id = id % 100;
        switch (id)
        {
            case 1:
                {
                    _Animetor.Play("ShotMotion");
                    _RigidBody.velocity = new Vector3(0, 0, 0);
                }
                break;
            default:
                break;
        }
    }


    #endregion


    #region コルーチン


    #endregion

}
