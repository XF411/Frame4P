using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fream4P.Core.UI
{
    /// <summary>
    /// 用于执行UI组件绑定的基类
    /// </summary>
    public abstract class UIBindBase : MonoBehaviour
    {
        public abstract void BindUI();
    }
}
