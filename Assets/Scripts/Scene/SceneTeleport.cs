using Enums;
using UnityEngine;

/// <summary>
/// Scene Teleport Class (scene change class)
/// </summary>
///<remarks>
/// Inherit from MonoBehaviour, required component BoxCollider2D,
/// change scene and set player's position when player touch this game object
/// </remarks>
[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    // The scene to be switched
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;

    // The player's position after the scene changes
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();

    #region unity massages

    //private void OnCollisionEnter2D(Collision2D collision)
    private void OnCollisionStay2D(Collision2D collision)
    {
        ChangeSceneAndSetPlayerPosition(collision);
    }

    #endregion

    // define change scene and set player position method
    private void ChangeSceneAndSetPlayerPosition(Collision2D collision)
    {
        // try to get player component from collision
        PlayerSingletonMonoBehavior player = collision.gameObject.GetComponent<PlayerSingletonMonoBehavior>();
        if (player == null)
        {
            return;
        }

        // calculate player's position
        // 检查定义好的场景位置，如果该位置为0，则将该位置设置为玩家位置；否则则设置为场景位置
        float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f)
            ? player.transform.position.x
            : scenePositionGoto.x;
        float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f)
            ? player.transform.position.y
            : scenePositionGoto.y;
        float zPosition = 0;

        // change scene
        SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
    }
}
