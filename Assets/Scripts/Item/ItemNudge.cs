using System.Collections;
using Enums;
using UnityEngine;


/**
 * 物体的轻轻摆动的效果
 * 当主角经过时，物体（item）将会轻轻摆动
 * 并且根据主角退出的方向摆动
 * todo：建议使用unity animator animation，或者itween、dotween插件，而不是代码控制
 */

public class ItemNudge : MonoBehaviour
{
    // 用于协程的yield return返回，表示等待x秒
    private WaitForSeconds pause;

    // 表示物体正在进行动画
    private bool isAnimating;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !isAnimating)
        {
            // 判断两者x的相对位置，并执行相应animation
            // 如果碰撞的物体在右边
            if (gameObject.transform.position.x < other.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

            if (other.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlaySound(SoundName.EffectRustle);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAnimating)
        {
            // 判断两者x的相对位置，并执行相应animation
            // 如果碰撞的物体在左边
            if (gameObject.transform.position.x > other.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

            if (other.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlaySound(SoundName.EffectRustle);
            }
        }
    }

    private IEnumerator RotateAntiClock()
    {
        // 加动画锁
        isAnimating = true;

        // 摆动
        for (int i = 0; i < 4; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }
        // 摆正
        for (int i = 0; i < 5; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }
        // 返回
        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
        yield return pause;

        // 解除动画锁
        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        // 加动画锁
        isAnimating = true;

        // 摆动
        for (int i = 0; i < 4; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }
        // 摆正
        for (int i = 0; i < 5; ++i)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }
        // 返回
        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
        yield return pause;

        // 解除动画锁
        isAnimating = false;
    }
}
