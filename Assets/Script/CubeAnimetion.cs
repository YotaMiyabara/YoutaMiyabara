using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class CubeAnimetion : MonoBehaviour
{
    public enum StateEnum
    {
        Non,
        NoActive,
        Pop,
        Split,
        Set,
        Shot
    }

    #region SerializeFiled

    [SerializeField, Tooltip("回転数")]
    private int _RoringNum=0;

    [SerializeField, Tooltip("回転倍率")]
    private float _RoringValue;

    [SerializeField, Tooltip("拡大倍率")]
    private float _ScaleUpValue;

    [SerializeField, Tooltip("箱の大きさ")]
    private float _CubuValue=0.5f;

    [SerializeField, Tooltip("弾の数")]
    private int _BulletNum=0;


    [SerializeField, Tooltip("飛ばす弾丸")]
    private GameObject _CubePrefab=null;

    [SerializeField]
    private WeponState _WeponState;

    [SerializeField]
    private List<GameObject> _bullet = new List<GameObject>();
    #endregion

    private float _RotationAngle = 0;
    private float _CreateScale = 0;
    private List<Vector3> _SpritPos = new List<Vector3>();


    public StateEnum _State;

    //Flag
    private bool IsCoroutine = false;
    private bool IsSprit = false;
    private bool IsCreate = false;

    void Start()
    {
        _State = StateEnum.Non;
        IsCoroutine = false;
        _CreateScale = 1.0f / (float)_BulletNum;
    }

    
    void Update()
    {
        switch (_State)
        {
            case StateEnum.Non:
                {
                    _CreateScale = _CubuValue / (float)_BulletNum;
                    Non_Move();
                }
                break;
            case StateEnum.NoActive:
                {
                    
                }
                break;
            case StateEnum.Pop:
                {
                    Pop_Move();
                }
                break;
            case StateEnum.Split:
                {
                   Split_Move();
                    
                }
                break;
            case StateEnum.Set:
                {
                    Set_Move();
                }
                break;
            case StateEnum.Shot:
                {

                }
                break;
        }

        //============================================================
        switch (_WeponState.GetState())
        {
            case WeponStateEnum.Non:
                {

                }
                break;
            case WeponStateEnum.Set_1:
                {
                    _State = StateEnum.Pop;
                }
                break;
            case WeponStateEnum.Set_2:
                {


                }
                break;
            case WeponStateEnum.Shot:
                {
                    foreach(GameObject obj in _bullet)
                    {
                        obj.transform.GetComponent<WeponState>().SetState(WeponStateEnum.Shot);
                    }
                    _WeponState.SetState(WeponStateEnum.Move);
                    
                }
                break;
            case WeponStateEnum.Move:
                {
                    //List内の最後の弾丸が消えた時
                    if (_bullet[0] == null)
                    {
                        _WeponState.SetState(WeponStateEnum.Non);
                        _State = StateEnum.Non;
                    }

                }
                break;
        }
    }

    private void CreateModel()
    {
        if (_CreateScale < 0)
        {
            Debug.Log("弾丸の数を入力して下さい");
            return;
        }
        //_bulletの初期化
        _bullet = new List<GameObject>();

        //modelの大きさを取得（正方形）
        float ModelScaleHarf = _CubePrefab.transform.localScale.x/2.0f;

        for (int z = 0; z < _BulletNum; ++z)
        {
            for(int y = 0; y < _BulletNum; ++y)
            {
                for(int x = 0; x < _BulletNum; ++x)
                {
                    //Objectを作る
                    GameObject obj = Instantiate(_CubePrefab, new Vector3(0,0,0), Quaternion.identity);

                    //Listに追加する
                    _bullet.Add(obj);

                    //子オブジェクトにする
                    obj.transform.parent = this.transform;

                    //Positionを設定（四角を作るようなPosition設定)中心に合わせる
                    Vector3 pos = new Vector3(x * _CreateScale-ModelScaleHarf, y * _CreateScale-ModelScaleHarf, z * _CreateScale-ModelScaleHarf);

                    //弾丸のInspector内の設定変更
                    obj.transform.localPosition = pos;
                    obj.transform.localScale = new Vector3(_CreateScale, _CreateScale, _CreateScale);
                    obj.transform.localRotation = default;
                    obj.GetComponent<NormalBullet>().SetLocalPosition(pos);
                }
            }


        }
    }

    private void SplitModel()
    {
        int NumCounter = 0;
        int PosXValue = 0;//2回毎に数が上がる
        int[] SetTableX = { 1, -1 };
        float SpritPosX = _CreateScale + (_CreateScale / 2.0f);

        //_SpritPosをクリアにしておく
        _SpritPos.Clear();

        //配置をセット
        foreach(GameObject obj in _bullet)
        {
            //配置の設定
            Vector3 pos=new Vector3(0,0,0);
            pos.x = SpritPosX * PosXValue * SetTableX[NumCounter%2];
            _SpritPos.Add(pos);

            ++NumCounter;

            if (NumCounter % 2 == 1)
            {//奇数毎
                ++PosXValue;

            }
        }

        IsSprit = true;
    }

    

    //*********************************************
    //State
    private void Non_Move()
    {
        IsCoroutine = false;
        IsSprit = false;
        IsCreate = false;

        transform.localScale = new Vector3(0, 0, 0);
        CreateModel();
        _State = StateEnum.NoActive;
    }

    private void Pop_Move()
    {
        //コルーチンの開始
        if (IsCoroutine == false)
        {
            StartCoroutine("ScaleUp");
            IsCoroutine = true;

        }
        
    }

    private void Split_Move()
    {
        if (!IsSprit)
        {
            SplitModel();//配置Posを設定
            StartCoroutine("Sprit_SetMove");//配置Posに移動させる

            
        }
    }
    private void Set_Move()
    {
        if (_WeponState.GetState() != WeponStateEnum.Set_2)
        {
            foreach (GameObject obj in _bullet)
            {
                obj.GetComponent<WeponState>().SetState(WeponStateEnum.Set_2);
                obj.GetComponent<TrailRenderer>().enabled = true;
            }
        }
        else
        {
            _State = StateEnum.Shot;
            
            return;
        }

    }

    private void Shot_Move()
    {
        if (_bullet[0] == null)
        {
            _State = StateEnum.Non;

            Transform pos = this.transform;
        }
    }

    //*******************************************************
    //ObjectMoving

    IEnumerator ScaleUp()
    {
        float scale = 0;
        while (scale<0.6f)//分割状態になったらループ終了
        {
            //真ん中から大きくする為のPosition設定

            float AddPos = _ScaleUpValue / 2.0f;

            this.transform.position += new Vector3(AddPos, AddPos, AddPos);

            //拡大縮小
            this.transform.localScale = new Vector3(scale, scale, scale);
            scale += _ScaleUpValue;


            yield return new WaitForSeconds(0.05f);
        }
        _State = StateEnum.Split;
        yield break;
    }
    IEnumerator Sprit_SetMove()
    {
        List<bool> IsSet = new List<bool>();
        //要素を追加
        for(int i = 0; i < _SpritPos.Count; ++i)
        {
            IsSet.Add(false);
        }


        while (true)
        {
            int i = 0;
            foreach (Vector3 pos in _SpritPos)
            {
                Vector3 ObjPos = _SpritPos[i];
                if (!IsSet[i])
                {
                    //X軸
                    if (ObjPos.x < pos.x)
                    {
                        ObjPos.x += 0.01f;
                    }
                    else if (ObjPos.x > pos.x)
                    {
                        ObjPos.x -= 0.01f;
                    }
                    else
                    {
                        IsSet[i] = true;
                    }
                    //y軸
                    //まだ要らない

                    //z軸
                    //まだ要らない


                    Vector3 LPos=_bullet[i].transform.localPosition;
                    LPos += pos;
                    Debug.Log(i);
                    _bullet[i].GetComponent<NormalBullet>().SetLocalPosition(LPos);
                }
                ++i;
            }

            if (IsSet.All(it => it == true))//IsSetのListが全てtrueになったら
            {
                
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        //状態移行して終了する
        IsSet.Clear();
        _State = StateEnum.Set;
        _WeponState.SetState(WeponStateEnum.Set_2);
        yield break;
    }

    #region publicfunction
    public void SetState_Pop()
    {
        _State = StateEnum.Pop;
    }

    public bool IsVec3AllZero(GameObject obj)
    {
        if(obj.transform.localPosition==new Vector3(0.0f, 0.0f, 0.0f))
        {
            return true;
        }

        return false;
    }


    #endregion


}
