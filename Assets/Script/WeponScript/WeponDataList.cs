using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName ="WeponListData", menuName = "WeponListData")]
public class WeponDataList : ScriptableObject
{
    
    public List<Wepon> wepons=new List<Wepon>();
    
}
