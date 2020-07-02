using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    protected virtual void InitGameManager() { }
    protected virtual void InitGameObject() { }
    protected void Awake()
    {
        InitGameObject();
        InitGameManager();
    }
}
