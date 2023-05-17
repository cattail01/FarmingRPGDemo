using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

[Serializable]
public class ColorSwap
{
    public Color FromColor;
    public Color ToColor;

    public ColorSwap(Color fromColor, Color toColor)
    {
        this.FromColor = fromColor;
        this.ToColor = toColor;
    }
}

public class ApplyCharacterCustomisation: MonoBehaviour
{
    [Header("Base Textures")] [SerializeField]
    private Texture2D maleFarmerBaseTexture;

    [SerializeField] private Texture2D femaleFarmerBaseTexture;
    [SerializeField] private Texture2D shirtsBaseTexture;
    [SerializeField] private Texture2D hairBaseTexture;
    [SerializeField] private Texture2D hatsBaseTexture;

    private Texture2D farmerBaseTexture;

    [Header("Output Base Texture To Be Used For Animation")] [SerializeField]
    private Texture2D farmerBaseCustomised;

    [SerializeField] private Texture2D hairCustomised;
    [SerializeField] private Texture2D hatsCustomised;

    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D selectedShirt;

    [Header("Select Shirt Style")] [Range(0, 1)] [SerializeField]
    private int inputShirtStyleNo;

    [Header("Select Hair Style")] [Range(0, 2)] [SerializeField]
    private int inputHairStyleNo;

    [Header("Select Skin Type")] [Range(0, 3)] [SerializeField]
    private int inputSkinType;

    [Header("Select Sex: 0-Male, 1-Female")]
    [Range(0, 1)]
    [SerializeField]
    private int inputSex;

    [Header("Select Hat Style")]
    [Range(0, 1)]
    [SerializeField] private int inputHatStyleNo;


    [SerializeField] private Color inputTrouserColor = Color.blue;
    [SerializeField] private Color inputHairColor = Color.black;

    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;

    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;

    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStylesInSpriteWidth = 16;

    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    private int hairStylesInSpriteWidth = 8;

    private int hatTextureWidth = 20;
    private int hatTextureHeight = 80;
    private int hatStylesInSpriteWidth = 12;

    private List<ColorSwap> colorSwapList;

    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);

    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255);
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255);
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255);
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255);

    private void MergeColourArray(Color[] baseArray, Color[] mergeArray)
    {
        for (int i = 0; i < baseArray.Length; ++i)
        {
            if (mergeArray[i].a > 0)
            {
                if (mergeArray[i].a >= 1)
                {
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    float alpha = mergeArray[i].a;

                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }

    private void MergeCustomisations()
    {
        Color[] farmerShirtPixels =
            farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        //Color[] farmerTrouserPixelsSelection = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288, 0, 96, farmerBaseTexture.height);
        Color[] farmerBodyPixels =
            farmerBaseCustomised.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        MergeColourArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColourArray(farmerBodyPixels, farmerShirtPixels);

        farmerBaseCustomised.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height,
            farmerBodyPixels);

        farmerBaseCustomised.Apply();
    }

    private bool IsSameColor(Color color1, Color color2)
    {
        if ((color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b) && (color1.a == color2.a))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChangePixelColors(Color[] baseArray, List<ColorSwap> colorSwapList)
    {
        for (int i = 0; i < baseArray.Length; ++i)
        {
            if (colorSwapList.Count > 0)
            {
                for (int j = 0; j < colorSwapList.Count; j++)
                {
                    if (IsSameColor(baseArray[i], colorSwapList[j].FromColor))
                    {
                        baseArray[i] = colorSwapList[j].ToColor;
                    }
                }
            }
        }
    }

    private void PopulateArmColorSwapList()
    {
        colorSwapList.Clear();

        colorSwapList.Add(new ColorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new ColorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new ColorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }

    private void ProcessArms()
    {
        Color[] farmerPixelsToRecolours = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);
        PopulateArmColorSwapList();
        ChangePixelColors(farmerPixelsToRecolours, colorSwapList);
        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolours);
        farmerBaseCustomised.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }
        texture2D.SetPixels(fill);
    }

    private void ApplyShirtTextureToBase()
    {
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;

        SetTextureToTransparent(farmerBaseShirtsUpdated);

        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        for (int x = 0; x < bodyColumns; ++x)
        {
            for (int y = 0; y < bodyRows; ++y)
            {
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                if (bodyShirtOffsetArray[x, y] != null)
                {
                    if (bodyShirtOffsetArray[x, y].x == 99 && bodyShirtOffsetArray[x, y].y == 99)
                        continue;
                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }

                switch (bodyFacingArray[x, y])
                {
                    case Facing.None:
                        break;
                    case Facing.Front:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight,
                            frontShirtPixels);
                        break;
                    case Facing.Back:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight,
                            backShirtPixels);
                        break;
                    case Facing.Right:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight,
                            rightShirtPixels);
                        break;

                    default:
                        break;

                }

            }
        }

        farmerBaseShirtsUpdated.Apply();
    }

    private void AddShirtToTexture(int shirtStyleNo)
    {
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        selectedShirt.filterMode = FilterMode.Point;

        int y = (shirtStyleNo / shirtStylesInSpriteWidth) * shirtTextureHeight;
        int x = (shirtStyleNo % shirtStylesInSpriteWidth) * shirtTextureWidth;

        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);
        
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }

    private void PopulateBodyShirtOffsetArray()
    {
        bodyShirtOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 9] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }

    private void PopulateBodyFacingArray()
    {
        bodyFacingArray[0, 0] = Facing.None;
        bodyFacingArray[1, 0] = Facing.None;
        bodyFacingArray[2, 0] = Facing.None;
        bodyFacingArray[3, 0] = Facing.None;
        bodyFacingArray[4, 0] = Facing.None;
        bodyFacingArray[5, 0] = Facing.None;

        bodyFacingArray[0, 1] = Facing.None;
        bodyFacingArray[1, 1] = Facing.None;
        bodyFacingArray[2, 1] = Facing.None;
        bodyFacingArray[3, 1] = Facing.None;
        bodyFacingArray[4, 1] = Facing.None;
        bodyFacingArray[5, 1] = Facing.None;

        bodyFacingArray[0, 2] = Facing.None;
        bodyFacingArray[1, 2] = Facing.None;
        bodyFacingArray[2, 2] = Facing.None;
        bodyFacingArray[3, 2] = Facing.None;
        bodyFacingArray[4, 2] = Facing.None;
        bodyFacingArray[5, 2] = Facing.None;

        bodyFacingArray[0, 3] = Facing.None;
        bodyFacingArray[1, 3] = Facing.None;
        bodyFacingArray[2, 3] = Facing.None;
        bodyFacingArray[3, 3] = Facing.None;
        bodyFacingArray[4, 3] = Facing.None;
        bodyFacingArray[5, 3] = Facing.None;

        bodyFacingArray[0, 4] = Facing.None;
        bodyFacingArray[1, 4] = Facing.None;
        bodyFacingArray[2, 4] = Facing.None;
        bodyFacingArray[3, 4] = Facing.None;
        bodyFacingArray[4, 4] = Facing.None;
        bodyFacingArray[5, 4] = Facing.None;

        bodyFacingArray[0, 5] = Facing.None;
        bodyFacingArray[1, 5] = Facing.None;
        bodyFacingArray[2, 5] = Facing.None;
        bodyFacingArray[3, 5] = Facing.None;
        bodyFacingArray[4, 5] = Facing.None;
        bodyFacingArray[5, 5] = Facing.None;

        bodyFacingArray[0, 6] = Facing.None;
        bodyFacingArray[1, 6] = Facing.None;
        bodyFacingArray[2, 6] = Facing.None;
        bodyFacingArray[3, 6] = Facing.None;
        bodyFacingArray[4, 6] = Facing.None;
        bodyFacingArray[5, 6] = Facing.None;

        bodyFacingArray[0, 7] = Facing.None;
        bodyFacingArray[1, 7] = Facing.None;
        bodyFacingArray[2, 7] = Facing.None;
        bodyFacingArray[3, 7] = Facing.None;
        bodyFacingArray[4, 7] = Facing.None;
        bodyFacingArray[5, 7] = Facing.None;

        bodyFacingArray[0, 8] = Facing.None;
        bodyFacingArray[1, 8] = Facing.None;
        bodyFacingArray[2, 8] = Facing.None;
        bodyFacingArray[3, 8] = Facing.None;
        bodyFacingArray[4, 8] = Facing.None;
        bodyFacingArray[5, 8] = Facing.None;

        bodyFacingArray[0, 9] = Facing.None;
        bodyFacingArray[1, 9] = Facing.None;
        bodyFacingArray[2, 9] = Facing.None;
        bodyFacingArray[3, 9] = Facing.None;
        bodyFacingArray[4, 9] = Facing.None;
        bodyFacingArray[5, 9] = Facing.None;

        bodyFacingArray[0, 10] = Facing.Back;
        bodyFacingArray[1, 10] = Facing.Back;
        bodyFacingArray[2, 10] = Facing.Right;
        bodyFacingArray[3, 10] = Facing.Right;
        bodyFacingArray[4, 10] = Facing.Right;
        bodyFacingArray[5, 10] = Facing.Right;

        bodyFacingArray[0, 11] = Facing.Front;
        bodyFacingArray[1, 11] = Facing.Front;
        bodyFacingArray[2, 11] = Facing.Front;
        bodyFacingArray[3, 11] = Facing.Front;
        bodyFacingArray[4, 11] = Facing.Back;
        bodyFacingArray[5, 11] = Facing.Back;

        bodyFacingArray[0, 12] = Facing.Back;
        bodyFacingArray[1, 12] = Facing.Back;
        bodyFacingArray[2, 12] = Facing.Right;
        bodyFacingArray[3, 12] = Facing.Right;
        bodyFacingArray[4, 12] = Facing.Right;
        bodyFacingArray[5, 12] = Facing.Right;

        bodyFacingArray[0, 13] = Facing.Front;
        bodyFacingArray[1, 13] = Facing.Front;
        bodyFacingArray[2, 13] = Facing.Front;
        bodyFacingArray[3, 13] = Facing.Front;
        bodyFacingArray[4, 13] = Facing.Back;
        bodyFacingArray[5, 13] = Facing.Back;

        bodyFacingArray[0, 14] = Facing.Back;
        bodyFacingArray[1, 14] = Facing.Back;
        bodyFacingArray[2, 14] = Facing.Right;
        bodyFacingArray[3, 14] = Facing.Right;
        bodyFacingArray[4, 14] = Facing.Right;
        bodyFacingArray[5, 14] = Facing.Right;

        bodyFacingArray[0, 15] = Facing.Front;
        bodyFacingArray[1, 15] = Facing.Front;
        bodyFacingArray[2, 15] = Facing.Front;
        bodyFacingArray[3, 15] = Facing.Front;
        bodyFacingArray[4, 15] = Facing.Back;
        bodyFacingArray[5, 15] = Facing.Back;

        bodyFacingArray[0, 16] = Facing.Back;
        bodyFacingArray[1, 16] = Facing.Back;
        bodyFacingArray[2, 16] = Facing.Right;
        bodyFacingArray[3, 16] = Facing.Right;
        bodyFacingArray[4, 16] = Facing.Right;
        bodyFacingArray[5, 16] = Facing.Right;

        bodyFacingArray[0, 17] = Facing.Front;
        bodyFacingArray[1, 17] = Facing.Front;
        bodyFacingArray[2, 17] = Facing.Front;
        bodyFacingArray[3, 17] = Facing.Front;
        bodyFacingArray[4, 17] = Facing.Front;
        bodyFacingArray[5, 17] = Facing.Front;

        bodyFacingArray[0, 18] = Facing.Back;
        bodyFacingArray[1, 18] = Facing.Back;
        bodyFacingArray[2, 18] = Facing.Back;
        bodyFacingArray[3, 18] = Facing.Right;
        bodyFacingArray[4, 18] = Facing.Right;
        bodyFacingArray[5, 18] = Facing.Right;

        bodyFacingArray[0, 19] = Facing.Right;
        bodyFacingArray[1, 19] = Facing.Right;
        bodyFacingArray[2, 19] = Facing.Right;
        bodyFacingArray[3, 19] = Facing.Front;
        bodyFacingArray[4, 19] = Facing.Front;
        bodyFacingArray[5, 19] = Facing.Front;

        bodyFacingArray[0, 20] = Facing.Front;
        bodyFacingArray[1, 20] = Facing.Front;
        bodyFacingArray[2, 20] = Facing.Front;
        bodyFacingArray[3, 20] = Facing.Back;
        bodyFacingArray[4, 20] = Facing.Back;
        bodyFacingArray[5, 20] = Facing.Back;
    }

    private void ProcessShirt()
    {
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        PopulateBodyFacingArray();

        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        PopulateBodyShirtOffsetArray();

        AddShirtToTexture(inputShirtStyleNo);

        ApplyShirtTextureToBase();
    }

    private void ProcessGender()
    {
        if (inputSex == 0)
        {
            farmerBaseTexture = maleFarmerBaseTexture;
        }
        else if (inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }

        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        farmerBaseCustomised.SetPixels(farmerBasePixels);
        farmerBaseCustomised.Apply();
    }

    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        for (int i = 0; i < basePixelArray.Length; ++i)
        {
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }

    private void ProcessTrousers()
    {
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);
        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);
        farmerBaseCustomised.Apply();
    }

    private void AddHairToTexture(int hairStyleNo)
    {
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth;

        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);

        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }

    private void ProcessHair()
    {
        AddHairToTexture(inputHairStyleNo);

        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();

        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();
    }

    private void PopulateSkinColorSwapList(int skinType)
    {
        colorSwapList.Clear();

        switch (skinType)
        {
            case 0:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, skinTargetColor4));
                break;
            case 1:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(213, 189, 162, 255)));
                break;
            case 2:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(145, 103, 26, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;
            case 3:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;
            default:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }

    private void ProcessSkin()
    {
        Color[] farmerPixelsToRecolour = farmerBaseCustomised.GetPixels(0, 0, 288, farmerBaseTexture.height);
        PopulateSkinColorSwapList(inputSkinType);
        ChangePixelColors(farmerPixelsToRecolour, colorSwapList);
        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolour);
        farmerBaseCustomised.Apply();
    }

    private void AddHatToTexture(int hatStyleNo)
    {
        int y = (hatStyleNo / hatStylesInSpriteWidth) * hatTextureHeight;
        int x = (hatStyleNo % hatStylesInSpriteWidth) * hatTextureWidth;

        Color[] hatPixels = hatsBaseTexture.GetPixels(x, y, hatTextureWidth, hatTextureHeight);

        hatsCustomised.SetPixels(hatPixels);
        hatsCustomised.Apply();
    }

    private void ProcessHat()
    {
        AddHatToTexture(inputHatStyleNo);
    }

    private void ProcessCustomisation()
    {
        ProcessGender();
        ProcessShirt();
        ProcessArms();
        ProcessTrousers();
        ProcessHair();
        ProcessSkin();
        ProcessHat();
        MergeCustomisations();
    }

    private void Awake()
    {
        colorSwapList = new List<ColorSwap>();

        ProcessCustomisation();
    }
}
