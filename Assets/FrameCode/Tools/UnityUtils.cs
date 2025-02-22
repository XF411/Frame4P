using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using SType = System.Type;
using UObject = UnityEngine.Object;

public sealed partial class UnityUtils
{
    public static GameObject FindObjectByName(string name, GameObject root = null)
    {
        if (null == root)
            return GameObject.Find(name);

        GameObject thisObject = null;
        foreach (Transform child in root.transform)
        {
            thisObject = child.gameObject;
            if (name.Equals(thisObject.name, StringComparison.Ordinal))
                return thisObject;
        }

        foreach (Transform child in root.transform)
        {
            if (null != (thisObject = FindObjectByName(name, child.gameObject)))
                return thisObject;
        }

        return null;
    }

    public static Transform FindObjectByPath(string path, GameObject root = null)
    {
        if (null == root)
            return GameObject.Find(path).transform;

        return root.transform.Find(path);      
    }

    public static GameObject FindObjectByName(string name, Component root)
    {
        if (root == null)
            return GameObject.Find(name);
        return FindObjectByName(name, root.gameObject);
    }

    public static GameObject FindObjectByNameNDeep(string name, GameObject root = null)
    {
        if (null == root)
            return GameObject.Find(name);

        GameObject thisObject = null;
        foreach (Transform child in root.transform)
        {
            thisObject = child.gameObject;
            if (name.Equals(thisObject.name, StringComparison.Ordinal))
                return thisObject;
        }
        return null;
    }

    public static GameObject FindObjectInParentByName(string name, GameObject child)
    {
        if (null == child)
            return null;

        if (name.Equals(child.name, StringComparison.Ordinal))
            return child;

        GameObject result = null;
        if (null != child.transform.parent)
        {
            result = FindObjectByName(name, child.transform.parent.gameObject);

            if (null == result)
                return FindObjectInParentByName(name, child.transform.parent.gameObject);
        }

        return result;
    }

    public static GameObject[] FindAllObjectsWithName(string name, GameObject root = null)
    {
        List<GameObject> resultList = new List<GameObject>();

        if (null == root)
        {
            GameObject go = GameObject.Find(name);
            if (null != go)
                resultList.Add(go);
        }
        else
        {
            GameObject thisObject = null;
            foreach (Transform child in root.transform)
            {
                thisObject = child.gameObject;

                if (name.Equals(thisObject.name, StringComparison.Ordinal))
                    resultList.Add(thisObject);

                GameObject[] objects = FindAllObjectsWithName(name, thisObject);
                if (objects.Length > 0)
                    resultList.AddRange(objects);
            }
        }

        return resultList.ToArray();
    }

    public static GameObject[] FindAllObjectByNameWithContainsString(string name, GameObject root = null)
    {
        List<GameObject> resultList = new List<GameObject>();

        if (null != root)
        {
            GameObject thisObject = null;
            foreach (Transform child in root.transform)
            {
                thisObject = child.gameObject;
                if (thisObject.name.Contains(name))
                    resultList.Add(thisObject);

                GameObject[] objects = FindAllObjectByNameWithContainsString(name, thisObject);
                if (objects.Length > 0)
                    resultList.AddRange(objects);
            }
        }

        return resultList.ToArray();
    }

    public static GameObject FindObjectByNameWithContainsString(string name, GameObject root = null)
    {
        if (null == root)
            return null;

        GameObject thisObject = null;
        foreach (Transform child in root.transform)
        {
            thisObject = child.gameObject;
            if (thisObject.name.Contains(name))
                return thisObject;

            if (null != (thisObject = FindObjectByName(name, thisObject)))
                return thisObject;
        }

        return null;
    }

    #region 把节点遍历并保存 避免使用递归找

    private static Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
    private static Transform nowParent;

    private static void SetDict(Transform parent)
    {
        if (nowParent != parent)
        {
            dict.Clear();
            nowParent = parent;
            FindAll(parent);
        }
    }

    public static T GetTarGetComponent<T>(string name, Transform parent) where T : Component
    {
        if (parent == null)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
                return obj.GetComponent<T>();
        }

        SetDict(parent);
        if (dict.TryGetValue(name, out Transform target))
        {
            T ret = target.GetComponent<T>();
            if (null != ret)
                return ret;
        }
        return null;
    }

    private static void FindAll(Transform target)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(target);
        AddDict(target);

        while (queue.Count > 0)
        {
            Transform tem = queue.Dequeue();
            Transform[] childrens = tem.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childrens.Length; i++)
            {
                if (!dict.ContainsKey(childrens[i].name))
                {
                    queue.Enqueue(childrens[i]);
                    AddDict(childrens[i]);
                }
            }
        }
    }

    private static void AddDict(Transform target)
    {
        if (!dict.ContainsKey(target.name))
            dict.Add(target.name, target);
    }

    #endregion

    public static T FindComponentWithName<T>(string name, GameObject root = null) where T : Component
    {
        GameObject go = FindObjectByName(name, root);
        if (null == go)
        {
            //Debug.LogWarningFormat("#UnityUtils::FindComponentWithName<T>, no component {0} with name {1} is found!", typeof(T).Name, name);
            return null;
        }

        return go.GetComponent<T>();

        //return GetTarGetComponent<T>(name, root.transform);//暂时不上传这个
    }

    public static T FindComponentWithPath<T>(string path, GameObject root = null) where T : Component
    {
        Transform go = FindObjectByPath(path, root);
        if (null == go)
        {
            //Debug.LogWarningFormat("#UnityUtils::FindComponentWithName<T>, no component {0} with name {1} is found!", typeof(T).Name, name);
            return null;
        }

        return go.GetComponent<T>();

        //return GetTarGetComponent<T>(name, root.transform);//暂时不上传这个
    }

    public static T AddComponentOnObjectWithContainsString<T>(string name, GameObject root = null) where T : Component
    {
        GameObject result = FindObjectByNameWithContainsString(name, root);
        if (null != result)
            return AddComponent<T>(result);

        return null;
    }

    public static T[] FindAllComponentsWithContainsString<T>(string name, GameObject root = null)
    {
        GameObject[] objectList = FindAllObjectByNameWithContainsString(name, root);

        List<T> resultList = new List<T>();
        for (int i = 0; i < objectList.Length; ++i)
        {
            resultList.Add(objectList[i].GetComponent<T>());
        }

        return resultList.ToArray();
    }

    public static T[] AddComponentOnAllWithContainsString<T>(string name, GameObject root = null) where T : Component
    {
        GameObject[] objectList = FindAllObjectByNameWithContainsString(name, root);

        List<T> resultList = new List<T>();
        for (int i = 0; i < objectList.Length; ++i)
        {
            T component = AddComponent<T>(objectList[i]);

            resultList.Add(component);
        }

        return resultList.ToArray();
    }

    public static GameObject AddNodeAsChild(string name, GameObject parent, bool normalize = false)
    {
        if (null == parent)
            return null;

        GameObject goGen = new GameObject(name);
        goGen.transform.parent = parent.transform;

        if (normalize)
        {
            NormalizeGameObject(goGen);
        }

        return goGen;
    }

    public static void NormalizeGameObject(GameObject target, string name)
    {
        target.name = name;
        NormalizeGameObject(target);
    }

    public static void NormalizeGameObject(GameObject target)
    {
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;
    }

    public static Component AddComponent(GameObject bindTarget, SType componentType, bool ignoreExsited = false)
    {
        if (null == bindTarget)
        {
            Debug.LogError("<Error> Can NOT add component because bing target is null!");
            return null;
        }

        Component component = null;

        if (!ignoreExsited)
            component = bindTarget.GetComponent(componentType);

        if (null == component)
            component = bindTarget.AddComponent(componentType);

        return component;
    }

    public static T AddComponent<T>(GameObject bindTarget, bool ignoreExsited = false)
            where T : Component
    {
        if (null == bindTarget)
        {
            Debug.LogError("<Error> Can NOT add component because bing target is null!");
            return null;
        }

        T component = null;

        if (!ignoreExsited)
            component = bindTarget.GetComponent<T>();

        if (null == component)
            component = bindTarget.AddComponent<T>();

        return component;
    }

    public static T AddComponent<T>(Transform bindTarget, bool ignoreExsited = false)
            where T : Component
    {
        if (null == bindTarget)
            return null;
        return AddComponent<T>(bindTarget.gameObject, ignoreExsited);
    }

    public static T AddComponentAsChild<T>(string name, GameObject parent, bool normalize = false)
            where T : Component
    {
        if (null == parent)
            return null;

        GameObject goGen = AddNodeAsChild(name, parent, normalize);

        return AddComponent<T>(goGen);
    }

    public static GameObject CloneAsBrother(GameObject go, bool normalise = true)
    {
        GameObject result = GameObject.Instantiate(go);
        result.transform.SetParent(go.transform.parent);

        if (normalise)
        {
            NormalizeGameObject(result);
        }

        return result;
    }

    public static GameObject CloneAsChild(GameObject go, Transform parent = null, bool normalise = true)
    {
        if (go == null)
            return null;
        GameObject result = GameObject.Instantiate(go);
        if (parent != null)
            result.transform.SetParent(parent);

        if (normalise)
        {
            NormalizeGameObject(result);
        }

        return result;
    }

    public static bool GetCastDownPoint(Vector3 startPoint, out Vector3 point, int layer, float dis = 50)
    {
        startPoint.y += 30;
        Ray rayLine = new Ray(startPoint, -Vector3.up);
        if (Physics.Raycast(rayLine, out var hit, dis, layer))
        {
            point = hit.point;
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    public static GameObject CreatAsChild(Transform parent = null, bool normalise = true)
    {
        GameObject result = new GameObject();
        if (parent != null)
            result.transform.SetParent(parent);

        if (normalise)
        {
            NormalizeGameObject(result);
        }

        return result;
    }

    public static T GetChildComponent<T>(GameObject go, bool includeInactive = false) where T : Component
    {
        if (null == go)
            return null;

        var ret = go.GetComponentInChildren<T>(includeInactive);
        if (ret != null)
            return ret;

        return go.GetComponent<T>();
    }

    public static T[] GetChildren<T>(GameObject go)
            where T : Component
    {
        if (null == go)
            return null;

        return go.GetComponentsInChildren<T>();
    }

    public static T[] GetChildren<T>(GameObject go, bool includeInactive)
        where T : Component
    {
        if (null == go)
            return null;

        return go.GetComponentsInChildren<T>(includeInactive);
    }

    public static T[] GetChildrenExcludeSelf<T>(GameObject go, bool includeInactive) where T : MonoBehaviour
    {
        List<T> resultList = new List<T>();
        T[] results = go.GetComponentsInChildren<T>(includeInactive);

        for (int i = 0; i < results.Length; ++i)
        {
            if (results[i].gameObject == go)
                continue;

            resultList.Add(results[i]);
        }

        return resultList.ToArray();
    }

    public static void RemoveComponentsInChildren<T>(GameObject go, bool includeInactive = true, bool immediately = false) where T : Component
    {
        T[] components = GetChildren<T>(go, includeInactive);
        for (int i = 0; i < components.Length; ++i)
        {
            T t = components[i];
            if (immediately)
                DestroyImmediate(t);
            else
                Destroy(t);
        }
    }

    public static void RemoveComponents<T>(GameObject go)
            where T : Component
    {
        RemoveComponents<T>(go, false);
    }

    public static void RemoveComponents<T>(GameObject go, bool immediately)
        where T : Component
    {
        if (null == go)
            return;

        foreach (T t in go.GetComponents<T>())
        {
            if (immediately)
                DestroyImmediate(t);
            else
                Destroy(t);
        }
    }

    public static T GetComponentInParentsAndBrother<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;

        var ret = go.GetComponentInParent<T>();
        if (ret != null)
            return ret;

        var parent = go.transform.parent;
        if (parent == null)
            return null;

        ret = parent.GetComponentInChildren<T>(true);
        return ret;
    }

    public static T[] GetComponentsInParents<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;

        return go.GetComponentsInParent<T>();
    }

    public static T GetComponent<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;

        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.GetComponentInChildren<T>();
        }
        return t;
    }

    public static T GetComponent<T>(string objectName) where T : Component
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
            return null;

        return obj.GetComponent<T>();
    }

    public static void ChangeLayer(GameObject go, int target)
    {
        if (null == go)
            return;

        Transform[] transf = go.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transf.Length; ++i)
            transf[i].gameObject.layer = target;
    }

    public static void ChangeRenderLayer(GameObject go, int target)
    {
        if (null == go)
            return;

        Renderer[] renderer = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderer.Length; ++i)
            renderer[i].gameObject.layer = target;
    }

    public static void ChangeColliderLayer(GameObject go, int target)
    {
        if (null == go)
            return;

        Collider[] collider = go.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < collider.Length; ++i)
            collider[i].gameObject.layer = target;
    }

    public static void DestroyChildren<T>(GameObject obj, bool includeInactive) where T : MonoBehaviour
    {
        T[] children = obj.GetComponentsInChildren<T>(includeInactive);
        for (int i = 0; i < children.Length; ++i)
        {
            Destroy(children[i].gameObject);
        }
    }
    public static void ClearContentChild(Transform contentRoot)
    {
        for (int i = 0; i < contentRoot.childCount; i++)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }
    }

    public static void Destroy(UObject obj)
    {
        DestroyDelay(obj, 0f);
    }

    public static void DestroyDelay(UObject obj, float time)
    {
        if (null == obj)
            return;

        UObject.Destroy(obj, time);
        obj = null;
    }

    public static void DestroyImmediate(UObject obj)
    {
        DestroyImmediate(obj, false);
    }

    public static void DestroyImmediate(UObject obj, bool destroyAsset)
    {
        if (null == obj)
            return;

        UObject.DestroyImmediate(obj, destroyAsset);
        obj = null;
    }

    public static int GetStringWidthWithFont(Font font, int fontSize, string content)
    {
        font.RequestCharactersInTexture(content, fontSize, FontStyle.Normal);
        CharacterInfo characterInfo;
        int width = 0;

        for (int i = 0; i < content.Length; i++)
        {
            font.GetCharacterInfo(content[i], out characterInfo, fontSize);
            width += characterInfo.advance;
        }

        return width;
    }

    public static Vector3 GetReflectedDir(Vector3 dir, Vector3 normal)
    {
        float length = Vector3.Dot(-dir, normal);
        return dir + (normal * length * 2f);
    }

    //
    //  函数功能：将Vector3压缩成32位整数，以减少数据量
    //  说明：这里比较特殊，x和z保存到一个int中，其中有15位可以存储数值，另外的一位存储符号，y由一个sbyte存储
    //
    public static void PackVector3ToInt32AndSByte(Vector3 value, out int xzValue, out byte yValue)
    {
        ushort xPack = (ushort)((short)value.x * 10f);
        ushort zPack = (ushort)((short)value.z * 10f);

        xzValue = (xPack << 16) | zPack;
        yValue = (byte)(value.y * 10f);
    }

    //
    //  函数功能：从32位整数解压Vector3数据
    //
    public static Vector3 UnpackVector3FromInt32AndSByte(int xzValue, byte yValue)
    {
        float x = (short)((xzValue >> 16) & 0xFFFF) * 0.1f;
        float z = (short)((xzValue) & 0xFFFF) * 0.1f;
        float y = (sbyte)(yValue & 0xFF) * 0.1f;

        return new Vector3(x, y, z);
    }

    //
    //  函数功能：将Quaternion压缩成Int32数据
    //
    public static int PackQuaternionToInt32(Quaternion value)
    {
        byte x = (byte)(sbyte)(value.x * 100f);
        byte y = (byte)(sbyte)(value.y * 100f);
        byte z = (byte)(sbyte)(value.z * 100f);
        byte w = (byte)(sbyte)(value.w * 100f);

        return (x << 24) | (y << 16) | (z << 8) | w;
    }

    //
    //  函数功能：将Int32数据转换成Quaternion
    //
    public static Quaternion UnpackQuaternionFromInt32(int value)
    {
        float x = ((sbyte)((value >> 24) & 0xFF)) * 0.01f;
        float y = ((sbyte)((value >> 16) & 0xFF)) * 0.01f;
        float z = ((sbyte)((value >> 8) & 0xFF)) * 0.01f;
        float w = ((sbyte)((value) & 0xFF)) * 0.01f;

        return new Quaternion(x, y, z, w);
    }

    //
    //  函数功能：激活组件
    //
    public static void ActivateComponent<T>(Transform root, bool activate) where T : MonoBehaviour
    {
        if (null == root)
            return;

        T result = GetChildComponent<T>(root.gameObject);
        if (null != result)
            result.enabled = activate;
    }

    //
    //  函数功能：激活组件
    //
    public static void ActivateComponents<T>(Transform root, bool activate) where T : MonoBehaviour
    {
        if (null == root)
            return;

        T[] results = GetChildren<T>(root.gameObject);
        for (int i = 0; i < results.Length; ++i)
        {
            results[i].enabled = activate;
        }
    }

    //
    //  函数功能：激活物件节点
    //  参数1：根节点
    //  参数2：节点名字
    //  参数3：激活还是不激活
    //
    public static bool ActivateGameObjectByName(Transform root, string name, bool activate)
    {
        if (root == null)
            return false;

        Transform targetTransform = null;
        targetTransform = root.name.CompareTo(name) == 0 ? root : root.Find(name);

        if (null == targetTransform)
            return false;

        targetTransform.gameObject.SetActive(activate);
        return true;
    }

    //
    //  函数功能：激活所有名字为xxx的组件
    //
    public static void ActivateGameObjectsByName(Transform root, string name, bool activate)
    {
        if (null == root)
            return;

        GameObject[] children = FindAllObjectsWithName(name, root.gameObject);
        for (int i = 0; i < children.Length; ++i)
            children[i].SetActive(activate);
    }

    //
    //  函数功能：激活所有MeshRenderer
    //  参数1：根节点
    //  参数2：激活的起始节点
    //  参数3：是否激活
    //
    public static void ActivateMeshRenderers(Transform root, string startName, bool activate)
    {
        if (null == root)
            return;

        Transform targetTransform = null;
        targetTransform = root.name.CompareTo(startName) == 0 ? root : root.Find(startName);

        if (null == targetTransform)
            return;

        MeshRenderer[] renderers = GetChildren<MeshRenderer>(targetTransform.gameObject);
        for (int i = 0; i < renderers.Length; ++i)
            renderers[i].enabled = activate;
    }

    /// <summary>
    /// 激活单个MeshRenderer
    /// </summary>
    /// <param name="root">根节点</param>
    /// <param name="objName">MeshRender节点名字</param>
    /// <param name="activate">是否激活</param>
    public static bool ActivateMeshRender(Transform root, string objName, bool activate)
    {
        if (null == root)
            return false;

        Transform targetTransform = null;
        targetTransform = root.name.CompareTo(objName) == 0 ? root : root.Find(objName);

        if (null == targetTransform)
            return false;

        MeshRenderer render = targetTransform.GetComponent<MeshRenderer>();
        if (render == null)
            return false;

        render.enabled = activate;
        return true;
    }

    public static void ActivateColliders(Transform root, string objName, bool activate)
    {
        if (null == root)
            return;

        Transform targetTransform = null;
        targetTransform = root.name.CompareTo(objName) == 0 ? root : root.Find(objName);

        if (targetTransform == null)
            return;

        var colliders = targetTransform.GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = activate;
        }
    }

    public static void PlayPartical(GameObject root)
    {
        var par = root.GetComponent<ParticleSystem>();
        if (par != null)
        {
            par.Stop();
            par.Play();
        }
        for (int i = 0; i < root.transform.childCount; i++)
        {
            PlayPartical(root.transform.GetChild(i).gameObject);
        }
    }

    public static void StopPartical(GameObject root)
    {
        var par = root.GetComponent<ParticleSystem>();
        if (par != null)
        {
            ParticleSystem.MainModule module = par.main;
            module.loop = false;
        }
        for (int i = 0; i < root.transform.childCount; i++)
        {
            StopPartical(root.transform.GetChild(i).gameObject);
        }
    }

    public static void SetParticlesUnscaleTime(Transform transParitcle, bool unscaleTime)
    {
        if (null == transParitcle)
            return;
        ParticleSystem[] particles = transParitcle.GetComponentsInChildren<ParticleSystem>();
        if (null != particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                var mode = particles[i].main;
                if (unscaleTime)
                {
                    particles[i].Play();
                }
                mode.useUnscaledTime = unscaleTime;
            }
        }
    }

    public static void SetAniUnscaleTime(Transform transAni, bool unscaleTime)
    {
        if (null == transAni)
            return;
        Animator[] animators = transAni.GetComponentsInChildren<Animator>();
        if (null != animators)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].updateMode = unscaleTime ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
            }
        }
    }

    //Added by Fudo
    public static Material GetMaterial(Renderer render)
    {
#if UNITY_EDITOR
        return render.material;
#else
		return render.sharedMaterial;
#endif
    }

    public static T DeepCopyByReflection<T>(T obj)
    {
        if (obj is string || obj.GetType().IsValueType)
            return obj;

        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        foreach (var field in fields)
        {
            try
            {
                field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
            }
            catch { }
        }

        return (T)retval;
    }

    public static void HideAllChildren(Transform parentTrans)
    {
        if (parentTrans == null)
            return;

        for (int i = 0; i < parentTrans.childCount; i++)
        {
            parentTrans.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static GameObject CreateGameObjectAtPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        var obj = GameObject.Find(path);
        if (obj == null)
        {
            //在最近父节点进行创建；
            var index = path.LastIndexOf('/');
            if (index == -1)
                return new GameObject(path);

            var parentPath = path.Substring(0, index);
            var selftPath = path.Substring(index + 1);

            obj = new GameObject(selftPath);

            var parent = CreateGameObjectAtPath(parentPath);
            if (parent != null)
                obj.transform.parent = parent.transform;

            NormalizeGameObject(obj);
        }

        return obj;
    }

    public static GameObject FindGameObjectInTarget(GameObject root, string path)
    {
        if (root == null)
            return CreateGameObjectAtPath(path);

        string rootName = root.name;

        if (path == rootName)
            return root;

        //如果路径是以 rootName 为开头，需要截掉这一段
        if (path.StartsWith(rootName))
        {
            int rootNameLength = rootName.Length;
            path = path.Substring(rootNameLength + 1);// name+斜杠
        }

        return FindGameObjectInTarget_Internal(root, path);
    }

    private static GameObject FindGameObjectInTarget_Internal(GameObject root, string path)
    {
        if (path.IsUnityNull())
            return root;

        var obj = root.transform.Find(path);
        if (obj == null)
        {
            //在最近父节点进行创建；
            var index = path.LastIndexOf('/');
            if (index == -1)
            {
                var newObj = new GameObject(path);
                newObj.transform.parent = root.transform ;
                NormalizeGameObject(newObj);
                return newObj;
            }

            var parentPath = path.Substring(0, index);
            var selftPath = path.Substring(index + 1);

            obj = new GameObject(selftPath).transform;

            var parent = FindGameObjectInTarget_Internal(root, parentPath);
            if (parent != null)
                obj.transform.SetParent(parent.transform);
            NormalizeGameObject(obj.gameObject);
        }
        return obj.gameObject;
    }
}