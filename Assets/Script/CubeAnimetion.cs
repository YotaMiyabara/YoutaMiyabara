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

    [SerializeField, Tooltip("��]��")]
    private int _RoringNum=0;

    [SerializeField, Tooltip("��]�{��")]
    private float _RoringValue;

    [SerializeField, Tooltip("�g��{��")]
    private float _ScaleUpValue;

    [SerializeField, Tooltip("���̑傫��")]
    private float _CubuValue=0.5f;

    [SerializeField, Tooltip("�e�̐�")]
    private int _BulletNum=0;


    [SerializeField, Tooltip("��΂��e��")]
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
                    //List���̍Ō�̒e�ۂ���������
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
            Debug.Log("�e�ۂ̐�����͂��ĉ�����");
            return;
        }
        //_bullet�̏�����
        _bullet = new List<GameObject>();

        //model�̑傫�����擾�i�����`�j
        float ModelScaleHarf = _CubePrefab.transform.localScale.x/2.0f;

        for (int z = 0; z < _BulletNum; ++z)
        {
            for(int y = 0; y < _BulletNum; ++y)
            {
                for(int x = 0; x < _BulletNum; ++x)
                {
                    //Object�����
                    GameObject obj = Instantiate(_CubePrefab, new Vector3(0,0,0), Quaternion.identity);

                    //List�ɒǉ�����
                    _bullet.Add(obj);

                    //�q�I�u�W�F�N�g�ɂ���
                    obj.transform.parent = this.transform;

                    //Position��ݒ�i�l�p�����悤��Position�ݒ�)���S�ɍ��킹��
                    Vector3 pos = new Vector3(x * _CreateScale-ModelScaleHarf, y * _CreateScale-ModelScaleHarf, z * _CreateScale-ModelScaleHarf);

                    //�e�ۂ�Inspector���̐ݒ�ύX
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
        int PosXValue = 0;//2�񖈂ɐ����オ��
        int[] SetTableX = { 1, -1 };
        float SpritPosX = _CreateScale + (_CreateScale / 2.0f);

        //_SpritPos���N���A�ɂ��Ă���
        _SpritPos.Clear();

        //�z�u���Z�b�g
        foreach(GameObject obj in _bullet)
        {
            //�z�u�̐ݒ�
            Vector3 pos=new Vector3(0,0,0);
            pos.x = SpritPosX * PosXValue * SetTableX[NumCounter%2];
            _SpritPos.Add(pos);

            ++NumCounter;

            if (NumCounter % 2 == 1)
            {//���
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
        //�R���[�`���̊J�n
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
            SplitModel();//�z�uPos��ݒ�
            StartCoroutine("Sprit_SetMove");//�z�uPos�Ɉړ�������

            
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
        while (scale<0.6f)//������ԂɂȂ����烋�[�v�I��
        {
            //�^�񒆂���傫������ׂ�Position�ݒ�

            float AddPos = _ScaleUpValue / 2.0f;

            this.transform.position += new Vector3(AddPos, AddPos, AddPos);

            //�g��k��
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
        //�v�f��ǉ�
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
                    //X��
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
                    //y��
                    //�܂��v��Ȃ�

                    //z��
                    //�܂��v��Ȃ�


                    Vector3 LPos=_bullet[i].transform.localPosition;
                    LPos += pos;
                    Debug.Log(i);
                    _bullet[i].GetComponent<NormalBullet>().SetLocalPosition(LPos);
                }
                ++i;
            }

            if (IsSet.All(it => it == true))//IsSet��List���S��true�ɂȂ�����
            {
                
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        //��Ԉڍs���ďI������
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
