using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace YZJ
{
    public enum UIAnimationType
    {
        None,
        Default,
        Color,
        Scale,
        SlideFromLeft,
        MoveToPoint,
        FadeInFromChind,
        Custom
    }

    public delegate Tweener CustomAnimationDelegate(GameObject panel, float duration, Ease ease);

    public class UIAnimationMgr
    {
        public static async UniTask PlayOnEffect(GameObject panel, UIAnimationType animationType, UIAnimationParams animationParams, CancellationToken token)
        {
            switch (animationType)
            {
                case UIAnimationType.Custom:
                    if (animationParams is CustomAnimationParams customParams && customParams.CustomAnimation != null)
                    {
                        await PlayTween(customParams.CustomAnimation(panel, customParams.Duration, customParams.Ease), token);
                    }
                    break;
                case UIAnimationType.Scale:
                    if (animationParams is ScaleAnimationParams scaleParams && scaleParams.scale != null)
                    {
                        panel.transform.localScale = Vector3.zero;
                        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            canvasGroup = panel.AddComponent<CanvasGroup>();
                        }
                        canvasGroup.alpha = 0;

                        Sequence sequence = DOTween.Sequence();
                        sequence.Join(panel.transform.DOScale(scaleParams.scale, scaleParams.Duration).SetEase(scaleParams.Ease));
                        sequence.Join(canvasGroup.DOFade(1, scaleParams.Duration).SetEase(scaleParams.Ease));

                        await PlayTween(sequence, token);
                    }
                    break;
                case UIAnimationType.SlideFromLeft:
                    if (animationParams is SlideFromLeftAnimationParams slideParams)
                    {
                        panel.transform.localPosition = new Vector3(-Screen.width, panel.transform.localPosition.y, 0);
                        await PlayTween(panel.transform.DOLocalMoveX(0, slideParams.Duration).SetEase(slideParams.Ease), token);
                    }
                    break;
                case UIAnimationType.FadeInFromChind:  // 处理新的动画类型
                    if (animationParams is FadeInFromChildAnimationParams fadeParams)
                    {
                        await PlaySequentialChildAnimations(panel, animationType, fadeParams.Duration, fadeParams.Ease, fadeParams.DelayBetween, token);
                    }
                    break;
                case UIAnimationType.MoveToPoint:
                    {
                        if (animationParams is MoveToPointAnimationParams moveParams)
                        {
                            Transform uiTransform = panel.transform;
                            // 保持原始的z坐标
                            float originalZ = uiTransform.position.z;
                            Vector3 actualStartPoint = new Vector3(moveParams.StartPoint.x, moveParams.StartPoint.y, originalZ);
                            Vector3 actualEndPoint = new Vector3(moveParams.EndPoint.x, moveParams.EndPoint.y, originalZ);

                            uiTransform.position = actualStartPoint;
                            uiTransform.localScale = Vector3.zero;

                            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                            if (canvasGroup == null)
                            {
                                canvasGroup = panel.AddComponent<CanvasGroup>();
                            }
                            canvasGroup.alpha = 0;
                            Sequence sequence = DOTween.Sequence();
                            sequence.Join(uiTransform.DOMove(actualEndPoint, moveParams.Duration).SetEase(moveParams.Ease));
                            sequence.Join(uiTransform.DOScale(Vector3.one, moveParams.Duration * 1.2f).SetEase(moveParams.Ease));
                            sequence.Join(canvasGroup.DOFade(1, moveParams.Duration).SetEase(moveParams.Ease));

                            await PlayTween(sequence, token);
                        }
                    }
                    break;
                case UIAnimationType.Color:
                    {
                        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            canvasGroup = panel.AddComponent<CanvasGroup>();
                        }
                        canvasGroup.alpha = 1;
                        RawImage rawImage = panel.GetComponent<RawImage>();
                        if (rawImage == null)
                        {
                            Image image = panel.GetComponent<Image>();
                            if (image != null)
                            {
                                if (animationParams is ColorAnimationParams colorParams)
                                {
                                    await PlayTween(image.DOColor(colorParams.color, colorParams.Duration).SetEase(colorParams.Ease), token);
                                }
                                else
                                {
                                    Debug.LogError("默认动画参数类型不正确");
                                }
                            }
                        }
                        else
                        {
                            if (animationParams is ColorAnimationParams colorParams)
                            {
                                await PlayTween(rawImage.DOColor(colorParams.color, colorParams.Duration).SetEase(colorParams.Ease), token);
                            }
                            else
                            {
                                Debug.LogError("默认动画参数类型不正确");
                            }
                        }
                    }
                    break;
                case UIAnimationType.Default:
                default:
                    {
                        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            canvasGroup = panel.AddComponent<CanvasGroup>();
                        }
                        canvasGroup.alpha = 0;
                        if (animationParams is DefaultAnimationParams defaultParams)
                        {
                            await PlayTween(canvasGroup.DOFade(1, defaultParams.Duration).SetEase(defaultParams.Ease), token);
                        }
                        else
                        {
                            Debug.LogError("默认动画参数类型不正确");
                        }
                    }
                    break;
            }
        }

        public static async UniTask PlayCloseEffect(GameObject panel, UIAnimationType animationType, float duration, Ease ease, CancellationToken token, CustomAnimationDelegate customAnimation = null, Vector3 startPoint = default, Vector3 endPoint = default)
        {
            switch (animationType)
            {
                case UIAnimationType.Custom:
                    if (customAnimation != null)
                    {
                        await PlayTween(customAnimation(panel, duration, ease), token);
                    }
                    break;
                case UIAnimationType.Scale:
                    await PlayTween(panel.transform.DOScale(0, duration).SetEase(ease), token);
                    break;
                case UIAnimationType.SlideFromLeft:
                    await PlayTween(panel.transform.DOLocalMoveX(-Screen.width, duration).SetEase(ease), token);
                    break;
                case UIAnimationType.MoveToPoint:
                    {
                        Transform uiTransform = panel.transform;
                        // 保持原始的z坐标
                        Vector3 originalVe3 = uiTransform.position;
                        float originalZ = uiTransform.position.z;
                        // Vector3 actualStartPoint = new Vector3(startPoint.x, startPoint.y, originalZ);
                        Vector3 actualEndPoint = new Vector3(endPoint.x, endPoint.y, endPoint.z);
                        uiTransform.position = startPoint;
                        uiTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            canvasGroup = panel.AddComponent<CanvasGroup>();
                            canvasGroup.alpha = 0;
                        }
                        Sequence sequence = DOTween.Sequence();
                        sequence.Join(uiTransform.DOMove(actualEndPoint, duration).SetEase(ease));
                        sequence.Join(uiTransform.DOScale(Vector3.zero, duration * 1.2f).SetEase(ease));
                        if (canvasGroup != null)
                        {
                            sequence.Join(canvasGroup.DOFade(1, duration).SetEase(ease));
                        }
                        await PlayTween(sequence, token);
                        uiTransform.position = startPoint;
                    }
                    break;
                case UIAnimationType.Default:
                default:
                    {
                        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            canvasGroup = panel.AddComponent<CanvasGroup>();
                        }
                        await PlayTween(canvasGroup.DOFade(0, duration).SetEase(ease), token);
                    }
                    break;
            }
        }
        public static async UniTask PlaySequentialChildAnimations(GameObject parent, UIAnimationType animationType, float duration, Ease ease, float delayBetween, CancellationToken token)
        {
            // 初始化所有子对象的CanvasGroup
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = child.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;  // 初始时完全透明
            }
            // 获取父对象的 CanvasGroup
            CanvasGroup parentCanvasGroup = parent.GetComponent<CanvasGroup>();

            // 创建父对象渐显动画
            Tweener parentFadeTween = parentCanvasGroup.DOFade(1, duration).SetEase(ease);

            // 等待父对象渐显动画完成
            await parentFadeTween.AsyncWaitForCompletion();
            // 为每个子对象播放动画
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();

                if (!child.activeSelf)
                {
                    child.SetActive(true);
                }

                // 创建渐显动画
                Tweener fadeTween = canvasGroup.DOFade(1, duration).SetEase(ease);

                // 创建适当的动画参数
                UIAnimationParams animationParams = new DefaultAnimationParams
                {
                    Duration = duration,
                    Ease = ease
                };

                // 同时进行渐显和其他动画
                var otherTweenTask = PlayOnEffect(child, animationType, animationParams, token);

                // 等待渐显动画完成
                await fadeTween.AsyncWaitForCompletion();

                // 等待其他动画完成
                await otherTweenTask;

                // 等待指定的延迟再显示下一个子对象
                await UniTask.Delay((int)(delayBetween * 1000), cancellationToken: token);
            }
        }

        private static async UniTask PlayTween(Tween tweener, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource();

            tweener.OnComplete(() => tcs.TrySetResult());
            tweener.OnKill(() => tcs.TrySetCanceled());

            token.Register(() =>
            {
                tweener.Kill();
                tcs.TrySetCanceled();
            });

            await tcs.Task;
        }
    }
}
