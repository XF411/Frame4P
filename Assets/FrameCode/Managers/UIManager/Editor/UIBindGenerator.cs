using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Fream4P.Core.UI 
{
    public struct ComponentInfo
    {
        public string ComponentName;
        public string ComponentType;
        public string OriginalName;
        public string ParentName;

        public ComponentInfo(string componentName, string componentType, string originalName, string parentName)
        {
            ComponentName = componentName;
            ComponentType = componentType;
            OriginalName = originalName;
            ParentName = parentName;
        }
    }

    /// <summary>
    /// 用于自动生成UI绑定脚本
    /// </summary>
    public static class UIBindGenerator
    {
        [MenuItem("GameObject/ScriptGenerator/GeneratorUIBind")]
        public static void GenerateScript()
        {
            // 获取选中的根对象
            GameObject root = Selection.activeGameObject;
            if (root == null)
            {
                Debug.LogError("Please select a root GameObject.");
                return;
            }

            GenerateScriptForObject(root, "GameLogic");
        }

        private static void GenerateScriptForObject(GameObject obj, string namespaceName)
        {
            string className = obj.name + "Bind";
            string outputPath = "Assets/GameScripts/HotFix/GameLogic/UI/UIBind/" + className + ".cs";

            // 创建保存目录
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // 收集组件信息
            List<ComponentInfo> componentInfoList = new List<ComponentInfo>();
            CollectComponentInfo(obj, obj, componentInfoList, new Dictionary<string, int>());

            // 生成脚本内容
            string scriptContent = GenerateScriptContent(className, namespaceName, componentInfoList);

            // 写入脚本文件
            File.WriteAllText(outputPath, scriptContent);
            AssetDatabase.Refresh();
        }

        private static void CollectComponentInfo(GameObject root, GameObject current, List<ComponentInfo> componentInfoList, Dictionary<string, int> nameCount, string parentName = null)
        {
            foreach (Transform child in current.transform)
            {
                string originalName = child.name;
                if (originalName.Contains("SubMeshUI"))
                {
                    if (parentName != null && parentName.EndsWith("TextMeshProUGUI")) // 如果是TextMeshProUGUI的子物体，不需要绑定
                    {
                        continue;
                    }
                }
                string componentName = GetUniqueName(originalName, nameCount);
                string componentType = GetComponentType(child.gameObject);
                string processedParentName = parentName;

                if (!string.IsNullOrEmpty(componentType))
                {
                    componentInfoList.Add(new ComponentInfo(componentName, componentType, originalName, parentName));

                    // 如果是绑定脚本节点
                    if (componentType.EndsWith("Bind"))
                    {
                        // 重新生成该节点的绑定脚本
                        GenerateScriptForObject(child.gameObject, "GameLogic");

                        // 添加调用BindUI的信息
                        componentInfoList.Add(new ComponentInfo(componentName, componentType, originalName, parentName));
                        continue; // 跳过其子物体的遍历
                    }
                }

                // 继续遍历子节点
                CollectComponentInfo(root, child.gameObject, componentInfoList, nameCount, componentName);
            }
        }

        private static string GetUniqueName(string baseName, Dictionary<string, int> nameCount)
        {
            // 替换空格为下划线
            baseName = baseName.Replace(" ", "_");

            // 删除其他特殊符号
            char[] invalidChars = { '-', '.', ',', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=', '[', ']', '{', '}', '|', '\\', ':', ';', '"', '\'', '<', '>', '/', '?', '~', '`' };
            foreach (char invalidChar in invalidChars)
            {
                baseName = baseName.Replace(invalidChar.ToString(), string.Empty);
            }

            if (!nameCount.ContainsKey(baseName))
            {
                nameCount[baseName] = 0;
                return baseName;
            }
            nameCount[baseName]++;
            return baseName + nameCount[baseName];
        }

        private static string GetComponentType(GameObject obj)
        {
            if (obj.GetComponent<UIBindBase>() != null) return obj.GetComponent<UIBindBase>().GetType().Name;
            if (obj.GetComponent<Button>() != null) return "Button";
            if (obj.GetComponent<Text>() != null) return "Text";
            if (obj.GetComponent<Image>() != null) return "Image";
            if (obj.GetComponent<RawImage>() != null) return "RawImage";
            if (obj.GetComponent<Toggle>() != null) return "Toggle";
            if (obj.GetComponent<Slider>() != null) return "Slider";
            if (obj.GetComponent<Scrollbar>() != null) return "Scrollbar";
            if (obj.GetComponent<Dropdown>() != null) return "Dropdown";
            if (obj.GetComponent<InputField>() != null) return "InputField";
            if (obj.GetComponent<Canvas>() != null) return "Canvas";
            if (obj.GetComponent<CanvasGroup>() != null) return "CanvasGroup";
            if (obj.GetComponent<ScrollRect>() != null) return "ScrollRect";
            if (obj.GetComponent<TMPro.TMP_InputField>() != null) return "TMP_InputField";
            if (obj.GetComponent<TMPro.TMP_Dropdown>() != null) return "TMP_Dropdown";
            if (obj.GetComponent<TMPro.TextMeshProUGUI>() != null) return "TextMeshProUGUI";
            return "RectTransform";
        }

        private static string GenerateScriptContent(string className, string namespaceName, List<ComponentInfo> componentInfoList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using GameLogic.Common;");
            sb.AppendLine();

            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");

            sb.AppendLine("    public class " + className + " : UIBindBase");
            sb.AppendLine("    {");

            foreach (var info in componentInfoList)
            {
                sb.AppendLine($"        public {info.ComponentType} {info.ComponentName};");
            }

            sb.AppendLine();
            sb.AppendLine("        public override void BindUI()");
            sb.AppendLine("        {");

            foreach (var info in componentInfoList)
            {
                string parentObject = info.ParentName == null ? "gameObject" : $"{info.ParentName}.gameObject";
                sb.AppendLine($"            {info.ComponentName} = UnityUtils.FindComponentWithName<{info.ComponentType}>(\"{info.OriginalName}\", {parentObject});");

                // 检查是否是绑定脚本
                if (info.ComponentType.EndsWith("Bind"))
                {
                    sb.AppendLine($"            {info.ComponentName}.BindUI();");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

