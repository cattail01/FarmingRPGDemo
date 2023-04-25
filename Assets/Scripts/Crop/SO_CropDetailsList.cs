using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "ScriptableObject/Crop/CropDetailsList")]
public class SO_CropDetailsList : MonoBehaviour
{
    [SerializeField]
    public List<CropDetails> CropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return CropDetails.Find(x => x.SeedItemCode == seedItemCode);
    }
}
