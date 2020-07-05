using UnityEngine;

public partial class GameEntry : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        InitBuiltinComponents(); //获得绑定在GameFrameWork下的各个模块的组件
    }
}
