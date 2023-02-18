using UnityEngine;

/// <summary>
/// 让所有被碰到的物体（包括子物体）半透显示
/// extend MonoBehaviour：挂给player
/// </summary>
public class TriggerObscuringItemFader : MonoBehaviour
{
    // 碰撞进入
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 获取碰撞物体以及其所有子物体的所有隐藏组件
        ObscuringItemFader[] obscuringItemFaders = other.GetComponentsInChildren<ObscuringItemFader>();

        // 遍历，并执行隐藏行为
        if (obscuringItemFaders.Length <= 0)
            return;
        foreach (ObscuringItemFader obscuringItemFader in obscuringItemFaders)
        {
            obscuringItemFader.FadeOut();
        }
    }

    // 碰撞退出
    private void OnTriggerExit2D(Collider2D other)
    {
        // 获取碰撞物体以及其所有子物体的所有隐藏组件
        ObscuringItemFader[] obscuringItemFaders = other.GetComponentsInChildren<ObscuringItemFader>();
        if (obscuringItemFaders.Length <= 0)
            return;
        // 遍历，并执行显示行为
        foreach (ObscuringItemFader obscuringItemFader in obscuringItemFaders)
        {
            obscuringItemFader.FadeIn();
        }
    }
}