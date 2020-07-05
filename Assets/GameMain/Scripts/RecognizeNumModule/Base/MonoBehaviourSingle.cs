using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourSingle<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T s_instance;

    protected Transform m_Tran;

    protected Transform tran //每个脚本挂载在物体上都要有transform组件
    {
        get
        {
            if (m_Tran == null)
            {
                m_Tran = gameObject.transform;
            }

            return m_Tran;
        }
    }

    public static T GetInstance()
    {
        if (s_instance == null)
        {
            GameObject gObj = new GameObject(typeof(T).Name);
            if (Application.isPlaying)
            {
                GameObject.DontDestroyOnLoad(gObj);
            }

            Transform tran = gObj.transform;
            tran.localPosition = Vector3.zero;
            tran.localEulerAngles = Vector3.zero;
            tran.localScale = Vector3.one;

            s_instance = gObj.AddComponent<T>();
        }
        return s_instance;
    }

    public static T Instance
    {
        get
        {
            return GetInstance();
        }
    }

    private void Awake()
    {
        OnInit();
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    private void OnApplicationQuit()
    {

    }

    protected virtual void OnInit()
    {
        s_instance = this as T;
    }
    public virtual void OnReConnect() { }
    public virtual void Clear() { }
    public virtual void OnUpdate(float deltatime) { }
}
