
using DG.Tweening;
using UnityEngine;

namespace YZJ
{
    public abstract class UIAnimationParams
    {
        public float Duration { get; set; } = 1f; // 默认持续时间为 1 秒
        public Vector3 defScale { get; set; } = Vector3.one; // 默认缩放为 (1, 1, 1)
        public Ease Ease { get; set; } = Ease.Linear; // 默认缓动方式为线性
    }
    public class ColorAnimationParams : UIAnimationParams
    {
        public Color color { get; set; }
    }
    public class DefaultAnimationParams : UIAnimationParams { }

    public class ScaleAnimationParams : UIAnimationParams
    {
        public Vector3 scale { get; set; }
    }

    public class SlideFromLeftAnimationParams : UIAnimationParams { }

    public class MoveToPointAnimationParams : UIAnimationParams
    {
        public Vector3 StartPoint { get; set; }
        public Vector3 EndPoint { get; set; }
    }

    public class FadeInFromChildAnimationParams : UIAnimationParams
    {
        public float DelayBetween { get; set; }
    }

    public class CustomAnimationParams : UIAnimationParams
    {
        public CustomAnimationDelegate CustomAnimation { get; set; }
    }
}