using UnityEngine;
using MagicOnionStudy.Shared; // MyVector3‚Ì–¼‘O‹óŠÔ
public static class TransformExtensions
{
    public static MyVector3 ToMyVector3(this Vector3 v)
    {
        return new MyVector3(v.x, v.y, v.z);
    }
    public static Vector3 ToUnityVector3(this MyVector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }
    
    // MyQuaternion‚È‚Ç‚ğ©ì‚·‚éê‡‚ÍAŸ‚Ì‚æ‚¤‚È•ÏŠ·‚ª‚ ‚é‚Æ‚æ‚¢‚©‚àH
    public static MyQuaternion ToMyQuaternion(this Quaternion q)
    {
        return new MyQuaternion(q.x, q.y, q.z, q.w);
    }
    
    public static Quaternion ToUnityQuaternion(this MyQuaternion q)
    {
        return new Quaternion(q.X, q.Y, q.Z, q.W);
    }    
}