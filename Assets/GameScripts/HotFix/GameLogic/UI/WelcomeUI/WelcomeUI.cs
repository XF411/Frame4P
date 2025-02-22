using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeUI : MonoBehaviour
{
    WelcomeUIBind ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = gameObject.GetComponent<WelcomeUIBind>();//这部分逻辑应该全部挪到UIManager和对应的基类中处理，
                                         //外界调用UI应该只需要写一句UIManager.OpenWindow(name);
                                         //之后内部会把从加载到实例化到运行绑定函数等内容一次性处理完成;
        ui.BindUI();
        ui.Button.onClick.AddListener(() => { Fream4P.Core.Logger.LogBlue("测试UI自生成绑定代码成功"); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
