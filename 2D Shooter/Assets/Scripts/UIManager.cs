using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager _instance;
    public static UIManager Instance{
        get{
            if(_instance==null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }
    #endregion
    
    [SerializeField]Text pathFindTimer;

    public void DisplayTime(long time){
        pathFindTimer.text = "Path found in :- "+time.ToString()+"ms"; 
    }    

}
