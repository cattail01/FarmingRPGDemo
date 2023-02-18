using System.Collections;
using UnityEngine;

/// <summary>
/// 对象模糊调节器
/// 放到对象身上，用于调节遮挡物体本身的透明度
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    /// <summary>
    /// 物体的精灵渲染器
    /// </summary>
    private SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 物体淡出
    /// 当主角被物体遮挡时，淡出物体，露出主角
    /// </summary>
    public void FadeOut()
    {
        // 开启协程：淡出
        StartCoroutine(FadeOutRoutine());
    }

    /// <summary>
    /// 物体淡入
    /// 当主角离开物体时，淡入物体，物体显示
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    /// <summary>
    /// 物体渐出协程
    /// 每一帧消失一下
    /// 最后一帧消失为目标alpha
    /// </summary>
    private IEnumerator FadeOutRoutine()
    {
        // throw new System.Exception();
        float currentAlpha = spriteRenderer.color.a;
        float distance = currentAlpha - Settings.TargetAlpha;

        while (currentAlpha - Settings.TargetAlpha > 0.1f)
        {
            currentAlpha = currentAlpha - distance / Settings.FadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            // 直到下一帧才执行下一次循环
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.TargetAlpha);
    }

    /// <summary>
    /// 物体渐入协程
    /// 每一帧渐入一下，最后一帧alpha为1
    /// </summary>
    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        // float distance = currentAlpha - Settings.TargetAlpha;
        float distance = 1f - currentAlpha;

        while (1f - currentAlpha > 0.1f)
        {
            currentAlpha = currentAlpha + distance / Settings.FadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            // 直到下一帧才执行下一次循环
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
}
