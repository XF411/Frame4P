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
        ui = gameObject.GetComponent<WelcomeUIBind>();//�ⲿ���߼�Ӧ��ȫ��Ų��UIManager�Ͷ�Ӧ�Ļ����д���
                                         //������UIӦ��ֻ��Ҫдһ��UIManager.OpenWindow(name);
                                         //֮���ڲ���ѴӼ��ص�ʵ���������а󶨺���������һ���Դ������;
        ui.BindUI();
        ui.Button.onClick.AddListener(() => { Fream4P.Core.Logger.LogBlue("����UI�����ɰ󶨴���ɹ�"); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
