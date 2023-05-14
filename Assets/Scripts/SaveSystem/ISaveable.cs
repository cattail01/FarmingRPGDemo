/// <summary>
/// 接口，可保存的类
/// </summary>
public interface ISaveable
{
    // 定义可存储类的存储的唯一id
    string SaveableUniqueId { get; set; }

    // 定义game object save类
    GameObjectSave GameObjectSave { get; set; }

    // 定义注册可保存类的方法
    void SaveableRegister();

    // 定义解注册可保存类的方法
    void SaveableUnregister();

    // 定义保存场景的方法
    void SaveableStoreScene(string sceneName);

    // 定义解保存场景的方法
    void SaveableRestoreScene(string sceneName);

    GameObjectSave SaveableSave();

    void SaveableLoad(GameSave gameSave);
}
