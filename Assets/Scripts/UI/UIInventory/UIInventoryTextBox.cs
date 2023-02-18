using UnityEngine;
using TMPro;

public class UIInventoryTextBox : MonoBehaviour
{
    // 在InventoryTextBox下所有关于文本的组件的引用
    // 需要在editor中手动拖动
    [SerializeField]
    private TextMeshProUGUI textMeshTop1;
    [SerializeField]
    private TextMeshProUGUI textMeshTop2;
    [SerializeField]
    private TextMeshProUGUI textMeshTop3;
    [SerializeField]
    private TextMeshProUGUI textMeshBottom1;
    [SerializeField]
    private TextMeshProUGUI textMeshBottom2;
    [SerializeField]
    private TextMeshProUGUI textMeshBottom3;

    // 调用该方法，对所有text进行填充
    public void SetTextBoxText(string textTop1, string textTop2, string textTop3, string textBottom1, string textBottom2, string textBottom3)
    {
        textMeshTop1.text = textTop1;
        textMeshTop2.text = textTop2;
        textMeshTop3.text = textTop3;
        textMeshBottom1.text = textBottom1;
        textMeshBottom2.text = textBottom2;
        textMeshBottom3.text = textBottom3;
    }
}
