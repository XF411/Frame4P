using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Warfare
{

    /// <summary>
    /// 通用的工具方法类；
    /// </summary>
    public static class Utils
    {
        #region 数学计算

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            //直线与平面的交点
            Vector3 hitPoint = (d * direct.normalized) + point;
            return hitPoint;
        }

        /// <summary>
        /// 点到直线的距离；
        /// </summary>
        /// <param name="point">目标点</param>
        /// <param name="linePoint1">直线上一点</param>
        /// <param name="linePoint2">直线上另一点</param>
        /// <returns></returns>
        public static float GetDistancePointToLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            Vector3 vec1 = point - linePoint1;
            Vector3 vec2 = linePoint2 - linePoint1;
            Vector3 vecProj = Vector3.Project(vec1, vec2);
            float dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
            return dis;
        }

        /// <summary>
        /// 计算射线与平面的交点
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Ray ray, Vector3 planeNormal, Vector3 planePoint) { return GetIntersectWithLineAndPlane(ray.origin, ray.direction, planeNormal, planePoint); }

        /// <summary>
        /// 计算射线与水平平面的交点
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Ray ray, float height)
        {
            var point = GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, new Vector3(0, height, 0));
            point.y = height;
            return point;
        }

        /// <summary>
        /// 将值限制在两个Vector3之间
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector3 Clamp(Vector3 pos, Vector3 min, Vector3 max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            pos.z = Mathf.Clamp(pos.z, min.z, max.z);
            return pos;
        }

        /// <summary>
        /// 将值限制在两个Vector3之间
        /// </summary>
        public static Vector3 ClampNoY(Vector3 pos, Vector3 min, Vector3 max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.z = Mathf.Clamp(pos.z, min.z, max.z);
            return pos;
        }

        /// <summary>
        /// 将值限制在两个Vector2之间
        /// 不修改Y轴；
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="min">相当于XZ</param>
        /// <param name="max">相当于XZ</param>
        /// <returns></returns>
        public static Vector3 ClampByVec2_NoY(Vector3 pos, Vector2 min, Vector2 max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.z = Mathf.Clamp(pos.z, min.y, max.y);
            return pos;
        }

        /// <summary>
        /// 将值限制在两个Vector2之间
        /// 不修改Z轴；
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="min">相当于XY</param>
        /// <param name="max">相当于XY</param>
        /// <returns></returns>
        public static Vector3 ClampByVec2_NoZ(Vector3 pos, Vector2 min, Vector2 max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            return pos;
        }

        /// <summary>
        /// 将值限制在两个Vector2之间
        /// </summary>
        /// <param name="pos">当前值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static Vector2 Clamp(Vector2 pos, Vector2 min, Vector2 max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            return pos;
        }

        /// <summary>
        /// 将值限制在两个Vector2之间
        /// </summary>
        /// <param name="pos">当前值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static Vector2Int Clamp(Vector2Int pos, Vector2Int min, Vector2Int max)
        {
            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            return pos;
        }

        /// <summary>
        /// 获取角度；
        /// Z旋转，默认朝上；
        /// 一般给UI用；
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float GetAngleZ_Up(Vector2 from, Vector2 to)
        {
            Vector2 dir = to - from;
            float angle = Vector2.Angle(dir, Vector2.up);

            if (dir.x > 0)
                angle = 90 - angle;
            else
                angle = 90 + angle;
            return angle;
        }

        /// <summary>
        /// 获取角度；
        /// Y旋转，给3D平面单位；
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float GetAngleY_Up(Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            float angle = Vector3.Angle(dir, Vector3.up);
            return angle;
        }

        public static Vector3 Lerp(Vector3 from, Vector3 to, float p)
        {
            from.x = Mathf.Lerp(from.x, to.x, p);
            from.y = Mathf.Lerp(from.y, to.y, p);
            from.z = Mathf.Lerp(from.z, to.z, p);
            return from;
        }

        public static Vector3 SmoothStep(Vector3 from, Vector3 to, float p)
        {
            from.x = Mathf.SmoothStep(from.x, to.x, p);
            from.y = Mathf.SmoothStep(from.y, to.y, p);
            from.z = Mathf.SmoothStep(from.z, to.z, p);
            return from;
        }

        /// <summary>
        /// 距离的平方；
        /// </summary>
        public static float DistanceSq(Vector3 a, Vector3 b)
        {
            float xSq = (a.x - b.x) * (a.x - b.x);
            float ySq = (a.y - b.y) * (a.y - b.y);
            float zSq = (a.z - b.z) * (a.z - b.z);
            return xSq + ySq + zSq;
        }

        /// <summary>
        /// 距离的平方；
        /// </summary>
        public static float DistanceSq(Vector2 a, Vector2 b)
        {
            float xSq = (a.x - b.x) * (a.x - b.x);
            float ySq = (a.y - b.y) * (a.y - b.y);
            return xSq + ySq;
        }

        /// <summary>
        /// 获取坐标差值之和；
        /// 忽略Y轴；
        /// 这个计算值一定大于等于其实际距离；
        /// </summary>
        public static float Distance_FastByCoDiff_NoY(Vector3 v1, Vector3 v2)
        {
            float x = v1.x - v2.x;
            if (x < 0)
                x = -x;

            float z = v1.z - v2.z;
            if (z < 0)
                z = -z;

            return x + z;
        }

        /// <summary>
        /// 使一个值与目标值同号；
        /// 如果a b 同号，返回原值；
        ///如果a b 不同号，则返回0 ；
        ///若b为0，则也返回0；
        ///如设定了默认值，则用默认值代替0；
        /// </summary>
        /// <param name="v"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float MakeSame_PN(float a, float b, float defV = 0)
        {
            if (a * b > 0)
                return a;
            return defV;
        }

        /// <summary>
        /// 根据目标值扩充Vec2，一般用于扩充边界；
        /// </summary>
        public static Vector2 GetMinMax(Vector2 oldVal, float val)
        {
            oldVal.x = Mathf.Min(oldVal.x, val);
            oldVal.y = Mathf.Max(oldVal.y, val);
            return oldVal;
        }

        /// <summary>
        /// 根据原值正负返回最大的整数
        /// </summary>
        /// <param name="oldVal"></param>
        /// <returns>正数取较大值；负数取较小值</returns>
        public static int CeilToInt2(float oldVal)
        {
            int ret = Mathf.CeilToInt(oldVal);
            if (oldVal < 0)
                ret -= 1;
            return ret;
        }

        /// <summary>
        /// 通过三个点获取圆的解析式；
        /// </summary>
        /// <returns>（x-a）^2+（y-b）^2=r^2</returns>
        public static Vector3 CircleResolver(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            //在一条直线上则不能形成圆形；
            if ((p2 - p1).normalized == (p3 - p2).normalized)
                return Vector3.zero;

            //计算圆形；
            float h = 2 * (p2.x - p1.x);
            float f = 2 * (p2.y - p1.y);
            float g = (p2.x * p2.x) - (p1.x * p1.x) + (p2.y * p2.y) - (p1.y * p1.y);
            float a = 2 * (p3.x - p2.x);
            float b = 2 * (p3.y - p2.y);
            float c = (p3.x * p3.x) - (p2.x * p2.x) + (p3.y * p3.y) - (p2.y * p2.y);
            float Col = ((g * b) - (c * f)) / ((h * b) - (a * f));
            float Row = ((a * g) - (c * h)) / ((a * f) - (b * h));
            float Rad = Mathf.Sqrt(((Col - p1.x) * (Col - p1.x)) + ((Row - p1.y) * (Row - p1.y)));

            return new Vector3(Col, Row, Rad);
        }

        /// <summary>
        /// 获取朝向
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="isValid"></param>
        /// <param name="ignoreY"></param>
        /// <returns></returns>
        public static Vector3 GetOrientation(Vector3 vectorA, Vector3 vectorB, out bool isValid, bool ignoreY = true)
        {
            isValid = true;
            var forward = vectorA - vectorB;
            if (ignoreY == true)
            {
                forward.y = 0;
            }

            if (forward == Vector3.zero)
            {
                isValid = false;
                forward = Vector3.forward;
            }

            return forward.normalized;
        }

        /// <summary>
        /// 获取旋转
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static Quaternion GetRotation(Vector3 velocity)
        {
            return Quaternion.FromToRotation(Vector3.forward, velocity);
        }


        #endregion

        #region 大世界相关


        public static bool Check_GridOverlap(Vector2 posA, Vector2Int posB, int sizeB)
        {
            if (sizeB == 0)
                return false;

            if (posA.x + sizeB < posB.x ||
                posA.x - sizeB > posB.x ||
                posA.y + sizeB < posB.y ||
                posA.y - sizeB > posB.y)
                return false;

            return true;
        }

        /// <summary>
        /// 检测某个点是否在某个Box内；
        /// 这个检测要求方块是不能旋转的；
        /// </summary>
        /// <returns>在范围内返回True</returns>
        public static bool Check_BoxOverlapNoRotate(Vector3 boxCenter, Vector3 boxSize, Vector3 targetPos)
        {
            Vector3 minPos = boxCenter - (boxSize * 0.5f);
            Vector3 maxPos = boxCenter + (boxSize * 0.5f);

            if (targetPos.x >= minPos.x && targetPos.x <= maxPos.x
                && targetPos.y >= minPos.y && targetPos.y <= maxPos.y
                && targetPos.z >= minPos.z && targetPos.z <= maxPos.z)
                return true;
            return false;
        }

        /// <summary>
        /// 检测某个点是否在某个Box内；
        /// 这个检测要求方块是不能旋转的；
        /// </summary>
        /// <returns>在范围内返回True</returns>
        public static bool Check_BoxOverlapNoRotate(Vector2Int pos, Vector2Int min, Vector2Int max)
        {
            if (pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y)
                return false;
            return true;
        }



        /// <summary>
        /// 获取采集图标的图片名字
        /// </summary>
        /// <param name="isSelf">是自己在采集</param>
        /// <param name="isFaction">是盟友在采集</param>
        /// <returns></returns>
        public static string Get_ResCollectIconName(bool isSelf, bool isFaction)
        {
            if (isSelf)
                return "main_icon_map_state_07";
            ;
            if (isFaction)
                return "main_icon_map_state_05";
            return "main_icon_map_state_06";
        }


        #endregion

        #region 编辑器下使用

#if UNITY_EDITOR

        public static bool DisplayDialog(string title, string message, string ok = "确定")
        { return EditorUtility.DisplayDialog(title, message, ok); }

        public static bool DisplayDialog(string title, string message, string ok, string cancel = "\"\"")
        { return EditorUtility.DisplayDialog(title, message, ok, cancel); }

        /// <summary>
        /// 把字符串用 . 分开；
        /// 然后获取最后一个字符串；
        /// </summary>
        /// <param name="oldStr"></param>
        /// <returns></returns>
        public static string Get_LastStrAfterDot(string oldStr)
        {
            var arrStr = oldStr.Split('.');
            return arrStr[arrStr.Length - 1];
        }

        /// <summary>
        /// 重命名；
        /// </summary>
        public static void Do_RenameAsset(UnityEngine.Object target, string newName)
        {
            if (target == null)
                return;

            string path = AssetDatabase.GetAssetPath(target);
            AssetDatabase.RenameAsset(path, newName);
        }

        /// <summary>
        /// 获取某个资源的大小（KB）
        /// </summary>
        public static int GetAssetSize(UnityEngine.Object target)
        {
            if (target == null)
                return 0;
            string path = AssetDatabase.GetAssetPath(target);
            FileInfo file = new FileInfo(path);
            return Mathf.CeilToInt(file.Length / 1024.0f);//获取文件大小；
        }

#endif

        #endregion;


    }
}