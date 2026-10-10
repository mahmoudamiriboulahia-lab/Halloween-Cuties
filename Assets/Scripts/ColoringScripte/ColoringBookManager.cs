using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

#if UNITY_WEBGL
using System.IO;
#endif

public class ColoringBookManager : MonoBehaviour
{
    public string Catgory;
    #region variables
    public int ColoringCount;
    public int StickerCount;
    public int PatternsCount;
    public int GlittersCount;
    public Material maskTexMaterial;
    private Texture2D maskTex;
    private List<Sprite> maskTexList = new List<Sprite>();
    public static int maskTexIndex = -1;
    public static string ID = "0";

    // list of drawmodes
    public enum DrawMode
    {
        Pencil,
        Marker,
        Spray,
        Sticker,
        Pattern,
        Glitters,
        Magic
    }
    public GameObject[] Tools;
    //	*** Default settings ***
    public GameObject _ContentColor;
    public List<Color32> paintColors;
    List<Image> buttonColors = new List<Image>();
    public GameObject PrefabColor;
    private Color32 paintColor = new Color32(255, 0, 0, 255);
    private int brushSize = 8; // default brush size
    public List<int> brushesSize;
    public List<Button> buttonBrushesSize;
    public Button paintBucketButton;
    private DrawMode drawMode = DrawMode.Pencil;
    private bool useLockArea = true;
    private byte[] lockMaskPixels; // locking mask pixels
    private int selectedColorNumber = 0;

    private Color32 magicColor = new Color32(255, 0, 0, 255);
    private float changeMagicColorTime = 0;

    // Stickers

    public GameObject PrefabSticker;
    public GameObject _ContentStickers;
    List<Texture2D> stickers = new List<Texture2D>();
    List<Image> buttonStickers = new List<Image>();
    private int selectedSticker = 0; // currently selected sticker index
    private byte[] stickerBytes;
    private int stickerWidth;
    private int stickerHeight;
    private int stickerWidthHalf;
    private int texWidthMinusStickerWidth;
    private int texHeightMinusStickerHeight;

    // Pattern
    public GameObject PrefabPattern;

    public GameObject _ContentPatterns;
    List<Texture2D> customPatterns = new List<Texture2D>();
    List<Image> buttonPatterns = new List<Image>();


    public GameObject PrefabGlitters;

    public GameObject _ContentGlitters;
    List<Texture2D> customGlitters = new List<Texture2D>();
    List<Image> buttonGlitters = new List<Image>();

    private byte[] patternBrushBytes;
    private int customPatternWidth;
    private int customPatternHeight;
    private int selectedPattern = 0;
    private int selectedGlitters = 0;
    private bool paintBucketMode = false;

    // SprayMode
    public Texture2D sprayModeBrush;
    private byte[] sprayModeBrushBytes;
    private int sprayModeWidth;
    private int sprayModeHeight;
    private int sprayModeWidthHalf;
    private int texWidthMinusSprayModeWidth;
    private int texHeightMinusSprayModeHeight;
    private Vector2 lastPaintedSprayModePos;

    // UNDO
    private List<byte[]> undoPixels; // undo buffer(s)
    private int redoIndex = 0;
    private int RedoIndex
    {
        set
        {
            redoIndex = value;

            undoButton.interactable = undoPixels.Count - RedoIndex - 1 > 0;
            redoButton.interactable = undoPixels.Count > 0 && RedoIndex > 0;
        }

        get
        {
            return redoIndex;
        }
    }

    //	*** private variables ***
    private byte[] pixels; // byte array for texture painting, this is the image that we paint into.
    private byte[] maskPixels; // byte array for mask texture

    private Texture2D tex; // texture that we paint into (it gets updated from pixels[] array when painted)

    private int texWidth = 1024;
    private int texHeight = 576;
    private RaycastHit hit;
    private bool wentOutside = false;

    private Vector2 pixelUV; // with mouse
    private Vector2 pixelUVOld; // with mouse

    private bool textureNeedsUpdate = false; // if we have modified texture

    ////////////////////////////////////////////////////

    public Button undoButton, redoButton;
    public Button musicButtonController;
    public Sprite musicButtonOn, musicButtonOff;

    public Image brushColorImage;

    public List<Color> themes;
    public Camera mainCamera;

    public GameObject waterMark;

    GraphicRaycaster m_Raycaster;
    EventSystem m_EventSystem;
    bool clickOnUI = false;

    #endregion


    #region Init And Control Functions
    
    private void Awake()
    {
        for (int i = 0; i < ColoringCount; i++)
        {
            Sprite loadedSprite = Resources.Load<Sprite>(Catgory+"/0" + i);
            maskTexList.Add(loadedSprite);
            if (loadedSprite == null)
                Debug.LogError("DEBUG: Failed to load sprite at index " + i + " path: " + Catgory + "/0" + i);
            else
                Debug.Log("DEBUG: Loaded sprite index " + i + " size: " + loadedSprite.texture.width + "x" + loadedSprite.texture.height);
        }

        Debug.Log("DEBUG: maskTexIndex = " + maskTexIndex);
        Debug.Log("DEBUG: Catgory = " + Catgory);
        Debug.Log("DEBUG: ColoringCount = " + ColoringCount);
        Debug.Log("DEBUG: maskTexList count = " + maskTexList.Count);

        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();

        GetComponent<Renderer>().sortingOrder = -99;

        if (maskTexIndex < 0)
        {
            Debug.Log("DEBUG: maskTexIndex is negative, no mask texture will be used");
            maskTex = null;
        }
        else
        {
            if (maskTexIndex >= maskTexList.Count)
            {
                Debug.LogError("DEBUG: maskTexIndex " + maskTexIndex + " is out of range! maskTexList only has " + maskTexList.Count + " items");
                maskTex = null;
            }
            else if (maskTexList[maskTexIndex] == null)
            {
                Debug.LogError("DEBUG: Sprite at maskTexIndex " + maskTexIndex + " is NULL!");
                maskTex = null;
            }
            else
            {
                Debug.Log("DEBUG: Loading mask texture at index " + maskTexIndex + " texture size: " + maskTexList[maskTexIndex].texture.width + "x" + maskTexList[maskTexIndex].texture.height);
                maskTex = DuplicateTexture(maskTexList[maskTexIndex].texture);
                Debug.Log("DEBUG: After DuplicateTexture, maskTex size: " + maskTex.width + "x" + maskTex.height);
            }
        }

        InitializeEverything();
    }

    private Texture2D DuplicateTexture(Texture2D originalTexture)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    originalTexture.width,
                    originalTexture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(originalTexture, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(originalTexture.width, originalTexture.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    private Texture2D ResizeTexture(Texture2D originalTexture, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(originalTexture, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }

    private Texture2D RotateTexture(Texture2D originalTexture, float eulerAngles)
    {
        int x;
        int y;
        int i;
        int j;
        float phi = eulerAngles / (180 / Mathf.PI);
        float sn = Mathf.Sin(phi);
        float cs = Mathf.Cos(phi);
        Color32[] arr = originalTexture.GetPixels32();
        Color32[] arr2 = new Color32[arr.Length];
        int W = originalTexture.width;
        int H = originalTexture.height;
        int xc = W / 2;
        int yc = H / 2;

        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);

                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);

                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }

        Texture2D newImg = new Texture2D(W, H);
        newImg.SetPixels32(arr2);
        newImg.Apply();

        return newImg;
    }

    private Texture2D FlipTexture(Texture2D originalTexture, bool upSideDown = true)
    {
        Texture2D flipped = new Texture2D(originalTexture.width, originalTexture.height);

        int xN = originalTexture.width;
        int yN = originalTexture.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                if (upSideDown)
                {
                    flipped.SetPixel(j, xN - i - 1, originalTexture.GetPixel(j, i));
                }
                else
                {
                    flipped.SetPixel(xN - i - 1, j, originalTexture.GetPixel(i, j));
                }
            }
        }
        flipped.Apply();

        return flipped;
    }

    private void InitializeEverything()
    {
        CreateFullScreenQuad();

        // create texture
        if (maskTex)
        {
            GetComponent<Renderer>().material = maskTexMaterial;

            texWidth = maskTex.width;
            texHeight = maskTex.height;
            GetComponent<Renderer>().material.SetTexture("_MaskTex", maskTex);

            useLockArea = true;
        }
        else
        {
            texWidth = 1024;
            texHeight = 576;

            useLockArea = false;
        }

        if (!GetComponent<Renderer>().material.HasProperty("_MainTex")) Debug.LogError("Fatal error: Current shader doesn't have a property: '_MainTex'");

        // create new texture
        tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

        // init pixels array
        pixels = new byte[texWidth * texHeight * 4];

        OnClearButtonClicked();

        // set texture modes
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        //tex.wrapMode = TextureWrapMode.Repeat;

        if (maskTex)
        {
            ReadMaskImage();
        }

        // undo system
        undoPixels = new List<byte[]>();
        undoPixels.Add(new byte[texWidth * texHeight * 4]);
        RedoIndex = 0;

        byte[] loadPixels = new byte[texWidth * texHeight * 4];
        loadPixels = LoadImage(ID);

        if (loadPixels != null)
        {
            pixels = loadPixels;
            System.Array.Copy(pixels, undoPixels[0], pixels.Length);

            tex.LoadRawTextureData(pixels);
            tex.Apply(false);
        }
        else
        {
            System.Array.Copy(pixels, undoPixels[0], pixels.Length);
        }

        // locking mask enabled
        if (useLockArea)
        {
            lockMaskPixels = new byte[texWidth * texHeight * 4];
        }
    }

private void CreateFullScreenQuad()
    {
        Camera cam = Camera.main;
        Mesh initialMesh = GetComponent<MeshCollider>().sharedMesh;
        // create mesh plane, fits in camera view (with screensize adjust taken into consideration)
        Mesh go_Mesh = GetComponent<MeshFilter>().mesh;
        go_Mesh.Clear();
        go_Mesh.vertices = new[] {
                transform.GetChild(0).position,
                transform.GetChild(1).position,
                transform.GetChild(2).position,
                transform.GetChild(3).position,

        };
        go_Mesh.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        go_Mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };

        go_Mesh.RecalculateNormals();

        go_Mesh.tangents = new[] { new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f) };

        // mesh collider
        gameObject.GetComponent<MeshCollider>().sharedMesh = go_Mesh;






    }


    private void ReadMaskImage()
    {
        maskPixels = new byte[texWidth * texHeight * 4];

        int pixel = 0;
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Color c = maskTex.GetPixel(x, y);
                maskPixels[pixel] = (byte)(c.r * 255);
                maskPixels[pixel + 1] = (byte)(c.g * 255);
                maskPixels[pixel + 2] = (byte)(c.b * 255);
                maskPixels[pixel + 3] = (byte)(c.a * 255);
                pixel += 4;
            }
        }
    }

    private byte[] LoadImage(string key)
    {
#if UNITY_WEBGL
        string file = Application.persistentDataPath + "/Landscape" + key + ".sav";
        if (File.Exists(file))
        {
            return System.Convert.FromBase64String(File.ReadAllText(file));
        }
        else
        {
            return null;
        }
#else
        if (PlayerPrefs.HasKey(key))
        {
            return System.Convert.FromBase64String(PlayerPrefs.GetString(key));
        }
        else
        {
            return null;
        }
#endif
    }

    private void SaveImage(string key)
    {
#if UNITY_WEBGL
        string file = Application.persistentDataPath + "/Landscape" + key + ".sav";
        string fileData = System.Convert.ToBase64String(pixels);
        File.WriteAllText(file, fileData);
#else
        PlayerPrefs.SetString(key, System.Convert.ToBase64String(pixels));
        PlayerPrefs.Save();
#endif
    }

    private void Start()
    {
       for (int i = 0; i < paintColors.Count; i++)
        {
            GameObject _postionColor = Instantiate(PrefabColor, _ContentColor.transform);
            buttonColors.Add(_ContentColor.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Image>());
            buttonColors[i].color = paintColors[i];
          
            
            _postionColor.GetComponent<Button>().AddEventListener(i, OnChangeColorButtonClicked);
        }

        paintColor = paintColors[selectedColorNumber];
        // OnChangeColorButtonClicked(selectedColorNumber);


        for (int i = 0; i < StickerCount; i++)
        {
            stickers.Add(Resources.Load<Texture2D>("_Stickers/0" + i));

            GameObject _postionStickers = Instantiate(PrefabSticker, _ContentStickers.transform);
            buttonStickers.Add(_ContentStickers.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Image>());
            buttonStickers[i].sprite = Sprite.Create(stickers[i], new Rect(0, 0, stickers[i].width, stickers[i].height), new Vector2(0.5f, 0.5f));

            _postionStickers.GetComponent<Button>().AddEventListener(i, OnStickerButtonClicked);

        }


        for (int i = 0; i < PatternsCount; i++)
        {
            customPatterns.Add(Resources.Load<Texture2D>("_Patterns/0" + i));
            GameObject _postionPatterns = Instantiate(PrefabPattern, _ContentPatterns.transform);
            buttonPatterns.Add(_ContentPatterns.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>());
            buttonPatterns[i].sprite = Sprite.Create(customPatterns[i], new Rect(0, 0, customPatterns[i].width, customPatterns[i].height), new Vector2(0.5f, 0.5f));

            //Sprite MyImage = Resources.Load<Sprite>("_Patterns/" + i);
            //_ContentPatterns.transform.GetChild(i).transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MyImage;


            _postionPatterns.GetComponent<Button>().AddEventListener(i, OnPatternButtonClicked);
        }

        for (int i = 0; i < GlittersCount; i++)
        {
            customGlitters.Add(Resources.Load<Texture2D>("_Glitters/0" + i));
            GameObject _postionGlitters = Instantiate(PrefabGlitters, _ContentGlitters.transform);
            buttonGlitters.Add(_ContentGlitters.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>());
            buttonGlitters[i].sprite = Sprite.Create(customGlitters[i], new Rect(0, 0, customGlitters[i].width, customGlitters[i].height), new Vector2(0.5f, 0.5f));

            //Sprite MyImage = Resources.Load<Sprite>("_Glitters/" + i);
            //x_ContentGlitters.transform.GetChild(i).transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MyImage;


            _postionGlitters.GetComponent<Button>().AddEventListener(i, OnGlittersButtonClicked);
        }


        OnChangeBrushSizeButtonClicked(2);







    }

    private void ReadSprayModeTexture()
    {
        Texture2D selectedCustomBrushTexture = DuplicateTexture(sprayModeBrush);
        selectedCustomBrushTexture = ResizeTexture(selectedCustomBrushTexture, brushSize * 2, brushSize * 2);

        if (Random.Range(0, 2) == 1)
        {
            selectedCustomBrushTexture = FlipTexture(selectedCustomBrushTexture, false);
        }

        selectedCustomBrushTexture = RotateTexture(selectedCustomBrushTexture, Random.Range(-40, 40));

        // NOTE: this works only for square brushes
        sprayModeWidth = selectedCustomBrushTexture.width;
        sprayModeHeight = selectedCustomBrushTexture.height;
        sprayModeBrushBytes = new byte[sprayModeWidth * sprayModeHeight * 4];

        int pixel = 0;
        Color32[] brushPixel = selectedCustomBrushTexture.GetPixels32();
        for (int y = 0; y < sprayModeHeight; y++)
        {
            for (int x = 0; x < sprayModeWidth; x++)
            {
                sprayModeBrushBytes[pixel] = brushPixel[x + y * sprayModeWidth].r;
                sprayModeBrushBytes[pixel + 1] = brushPixel[x + y * sprayModeWidth].g;
                sprayModeBrushBytes[pixel + 2] = brushPixel[x + y * sprayModeWidth].b;
                sprayModeBrushBytes[pixel + 3] = brushPixel[x + y * sprayModeWidth].a;
                pixel += 4;
            }
        }

        // precalculate few brush size values
        sprayModeWidthHalf = (int)(sprayModeWidth * 0.5f);
        texWidthMinusSprayModeWidth = texWidth - sprayModeWidth;
        texHeightMinusSprayModeHeight = texHeight - sprayModeHeight;
    }

    private void LoadSetting()
    {
        // Music
        //musicButtonController.image.sprite = (int)AudioListener.volume == 1 ? musicButtonOn : musicButtonOff;

        // Theme
        // OnChangeThemeButtonClicked(true);
    }

    private void LateUpdate()
    {
        ChangeMagicColor();

        MousePaint();

        UpdateTexture();
    }

    private void MousePaint()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);
            clickOnUI = results.Count > 0;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null || !hit.collider.gameObject.name.Contains("PaintingBoard"))
                {
                    return;
                }
            }
            else
            {
                RaycastHit2D hit2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit2.collider == null || !hit2.collider.gameObject.name.Contains("PaintingBoard"))
                {
                    return;
                }
            }
        }

        if (!clickOnUI)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (useLockArea)
                {
                    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1)) return;
                    CreateAreaLockMask((int)(hit.textureCoord.x * texWidth), (int)(hit.textureCoord.y * texHeight));
                }

                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1)) { wentOutside = true; return; }

                pixelUVOld = pixelUV; // take previous value, so can compare them
                pixelUV = hit.textureCoord;
                pixelUV.x *= texWidth;
                pixelUV.y *= texHeight;

                if (wentOutside) { pixelUVOld = pixelUV; wentOutside = false; }

                // lets paint where we hit
                switch (drawMode)
                {
                    case DrawMode.Sticker: // Sticker
                        DrawSticker((int)pixelUV.x, (int)pixelUV.y);
                        break;

                    case DrawMode.Spray:
                        {
                            lastPaintedSprayModePos = Input.mousePosition;

                            DrawSprayBrush((int)pixelUV.x, (int)pixelUV.y);
                        }
                        break;

                    default: // unknown mode
                        break;
                }

                textureNeedsUpdate = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1)) { wentOutside = true; return; }

                // when starting, grab undo buffer first
                if (RedoIndex > 0)
                {
                    undoPixels.RemoveRange(undoPixels.Count - RedoIndex, RedoIndex);
                }

                undoPixels.Add(new byte[texWidth * texHeight * 4]);
                System.Array.Copy(pixels, undoPixels[undoPixels.Count - 1], pixels.Length);

                RedoIndex = 0;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                // Only if we hit something, then we continue
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1)) { wentOutside = true; return; }

                pixelUVOld = pixelUV; // take previous value, so can compare them
                pixelUV = hit.textureCoord;
                pixelUV.x *= texWidth;
                pixelUV.y *= texHeight;

                if (wentOutside) { pixelUVOld = pixelUV; wentOutside = false; }

                // lets paint where we hit
                switch (drawMode)
                {
                    case DrawMode.Pencil: // drawing
                        {
                            if (!paintBucketMode)
                            {
                                DrawCircle((int)pixelUV.x, (int)pixelUV.y);
                            }
                            else
                            {
                                if (maskTex)
                                {
                                    FloodFillMaskOnlyWithThreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                                else
                                {
                                    FloodFillWithTreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                            }
                        }
                        break;

                    case DrawMode.Marker: // drawing
                        DrawAdditiveCircle((int)pixelUV.x, (int)pixelUV.y);
                        break;

                    case DrawMode.Pattern: // draw with pattern
                        {
                            if (paintBucketMode)
                            {
                                if (maskTex)
                                {
                                    FloodFillPatternMaskOnlyWithThreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                                else
                                {
                                    FloodFillPatternWithTreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                            }
                            else
                            {
                                DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y);
                            }
                        }
                        break;
                    case DrawMode.Glitters: // draw with pattern
                        {
                            if (paintBucketMode)
                            {
                                if (maskTex)
                                {
                                    FloodFillPatternMaskOnlyWithThreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                                else
                                {
                                    FloodFillPatternWithTreshold((int)pixelUV.x, (int)pixelUV.y);
                                }
                            }
                            else
                            {
                                DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y);
                            }
                        }
                        break;

                    case DrawMode.Spray:
                        {
                            if (Vector2.Distance(lastPaintedSprayModePos, Input.mousePosition) >= sprayModeWidthHalf * 2 / 3f)
                            {
                                lastPaintedSprayModePos = Input.mousePosition;

                                DrawSprayBrush((int)pixelUV.x, (int)pixelUV.y);
                            }
                        }
                        break;

                    case DrawMode.Magic: // drawing
                        {
                            DrawMagicCircle((int)pixelUV.x, (int)pixelUV.y);
                        }
                        break;

                    default: // unknown mode
                        break;
                }

                textureNeedsUpdate = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // take this position as start position
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1)) return;

                pixelUVOld = pixelUV;
            }

            // check distance from previous drawing point and connect them with DrawLine
            if (Vector2.Distance(pixelUV, pixelUVOld) > brushSize)
            {
                switch (drawMode)
                {
                    case DrawMode.Pencil: // drawing
                        {
                            DrawLine(pixelUVOld, pixelUV);
                        }
                        break;

                    case DrawMode.Marker: // drawing
                        DrawAdditiveLine(pixelUVOld, pixelUV);
                        break;

                    case DrawMode.Pattern:
                        {
                            if (!paintBucketMode)
                            {
                                DrawLineWithPattern(pixelUVOld, pixelUV);
                            }
                        }
                        break;

                    case DrawMode.Magic: // drawing
                        {
                            DrawMagicLine(pixelUVOld, pixelUV);
                        }
                        break;

                    default: // other modes
                        break;
                }
                pixelUVOld = pixelUV;
                textureNeedsUpdate = true;
            }
        }
    }

    private void CreateAreaLockMask(int x, int y)
    {
        if (maskTex)
        {
            LockAreaFillWithThresholdMaskOnly(x, y);
        }
        else
        {
            LockMaskFillWithThreshold(x, y);
        }
    }

    private void LockAreaFillWithThresholdMaskOnly(int x, int y)
    {
        // create locking mask floodfill, using threshold, checking pixels from mask only

        // get canvas color from this point
        byte hitColorR = maskPixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = maskPixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = maskPixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = maskPixels[((texWidth * (y) + x) * 4) + 3];

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {

            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down

                if (lockMaskPixels[pixel] == 0 // this pixel is not used yet
                    && (CompareThreshold(maskPixels[pixel + 0], hitColorR)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(maskPixels[pixel + 1], hitColorG))
                    && (CompareThreshold(maskPixels[pixel + 2], hitColorB))
                    && (CompareThreshold(maskPixels[pixel + 3], hitColorA)))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(maskPixels[pixel + 0], hitColorR)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(maskPixels[pixel + 1], hitColorG))
                    && (CompareThreshold(maskPixels[pixel + 2], hitColorB))
                    && (CompareThreshold(maskPixels[pixel + 3], hitColorA)))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(maskPixels[pixel + 0], hitColorR)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(maskPixels[pixel + 1], hitColorG))
                    && (CompareThreshold(maskPixels[pixel + 2], hitColorB))
                    && (CompareThreshold(maskPixels[pixel + 3], hitColorA)))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(maskPixels[pixel + 0], hitColorR)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(maskPixels[pixel + 1], hitColorG))
                    && (CompareThreshold(maskPixels[pixel + 2], hitColorB))
                    && (CompareThreshold(maskPixels[pixel + 3], hitColorA)))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void LockMaskFillWithThreshold(int x, int y)
    {
        // create locking mask floodfill, using threshold

        // get canvas color from this point
        byte hitColorR = pixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = pixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = pixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = pixels[((texWidth * (y) + x) * 4) + 3];

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {

            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down

                if (lockMaskPixels[pixel] == 0 // this pixel is not used yet
                    && (CompareThreshold(pixels[pixel + 0], hitColorR) || CompareThreshold(pixels[pixel + 0], paintColor.r)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(pixels[pixel + 1], hitColorG) || CompareThreshold(pixels[pixel + 1], paintColor.g))
                    && (CompareThreshold(pixels[pixel + 2], hitColorB) || CompareThreshold(pixels[pixel + 2], paintColor.b))
                    && (CompareThreshold(pixels[pixel + 3], hitColorA) || CompareThreshold(pixels[pixel + 3], paintColor.a)))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(pixels[pixel + 0], hitColorR) || CompareThreshold(pixels[pixel + 0], paintColor.r)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(pixels[pixel + 1], hitColorG) || CompareThreshold(pixels[pixel + 1], paintColor.g))
                    && (CompareThreshold(pixels[pixel + 2], hitColorB) || CompareThreshold(pixels[pixel + 2], paintColor.b))
                    && (CompareThreshold(pixels[pixel + 3], hitColorA) || CompareThreshold(pixels[pixel + 3], paintColor.a)))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(pixels[pixel + 0], hitColorR) || CompareThreshold(pixels[pixel + 0], paintColor.r)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(pixels[pixel + 1], hitColorG) || CompareThreshold(pixels[pixel + 1], paintColor.g))
                    && (CompareThreshold(pixels[pixel + 2], hitColorB) || CompareThreshold(pixels[pixel + 2], paintColor.b))
                    && (CompareThreshold(pixels[pixel + 3], hitColorA) || CompareThreshold(pixels[pixel + 3], paintColor.a)))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && (CompareThreshold(pixels[pixel + 0], hitColorR) || CompareThreshold(pixels[pixel + 0], paintColor.r)) // if pixel is same as hit color OR same as paint color
                    && (CompareThreshold(pixels[pixel + 1], hitColorG) || CompareThreshold(pixels[pixel + 1], paintColor.g))
                    && (CompareThreshold(pixels[pixel + 2], hitColorB) || CompareThreshold(pixels[pixel + 2], paintColor.b))
                    && (CompareThreshold(pixels[pixel + 3], hitColorA) || CompareThreshold(pixels[pixel + 3], paintColor.a)))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void UpdateTexture()
    {
        if (textureNeedsUpdate)
        {
            textureNeedsUpdate = false;
            tex.LoadRawTextureData(pixels);
            tex.Apply(false);
        }
    }

    private void ChangeMagicColor()
    {
        if (Time.time - changeMagicColorTime > 0.1f)
        {
            changeMagicColorTime = Time.time;
            magicColor = paintColors[Random.Range(0, paintColors.Count)];

            if (drawMode == DrawMode.Magic)
            {
                brushColorImage.sprite = null;
                brushColorImage.color = magicColor;
            }
        }
    }

    #endregion


    #region OnButtonsClicked

    public void PlaySoundButtonClicked()
    {
        //        MusicController.USE.PlaySound(MusicController.USE.clickSound);
    }

    public void OnPencilButtonClicked()
    {
        drawMode = DrawMode.Pencil;
        OnChangeColorButtonClicked(selectedColorNumber);
        OnChangeBrushSizeButtonClicked(3);
    }
    public void OnPencilButtonClicked2()
    {
        drawMode = DrawMode.Pencil;
        OnChangeColorButtonClicked(selectedColorNumber);
        SetPaintBucketMode(true);

    }

    public void OnMarkerButtonClicked()
    {
        drawMode = DrawMode.Marker;
        OnChangeColorButtonClicked(selectedColorNumber);
        OnChangeBrushSizeButtonClicked(3);
        OnChangeBrushSizeMenuButtonClicked();
    }

    public void OnMagicButtonClicked()
    {
        drawMode = DrawMode.Magic;
        OnChangeBrushSizeMenuButtonClicked();
    }

    public void OnSprayButtonClicked()
    {
        ReadSprayModeTexture();

        drawMode = DrawMode.Spray;
        OnChangeColorButtonClicked(selectedColorNumber);
        OnChangeBrushSizeButtonClicked(3);
        OnChangeBrushSizeMenuButtonClicked();
    }

    public void OnEraserButtonClicked()
    {
        drawMode = DrawMode.Pencil;

        paintColor = paintColors[2];
        brushColorImage.sprite = null;
        brushColorImage.color = paintColor;
        OnChangeBrushSizeMenuButtonClicked();
    }

    public void OnChangeColorButtonClicked(int num)
    {
        selectedColorNumber = num;

        paintColor = paintColors[selectedColorNumber];
        brushColorImage.sprite = null;
        brushColorImage.color = paintColor;

        for (int c = 0; c < Tools.Length; c++)
        {
            Tools[c].transform.GetComponent<Image>().color = paintColor;
        }
    }

    public void OnStickerButtonClicked()
    {
        drawMode = DrawMode.Sticker;

        OnStickerButtonClicked(selectedSticker);
        OnChangeBrushSizeMenuButtonClicked();
    }

    public void OnStickerButtonClicked(int num)
    {
        selectedSticker = num;

        brushColorImage.sprite = buttonStickers[selectedSticker].sprite = Sprite.Create(stickers[selectedSticker], new Rect(0, 0, stickers[selectedSticker].width, stickers[selectedSticker].height), new Vector2(0.5f, 0.5f));
        brushColorImage.color = Color.white;

        Texture2D selectedStickerTexture = DuplicateTexture(stickers[selectedSticker]);
        selectedStickerTexture = ResizeTexture(selectedStickerTexture, brushSize * 5, brushSize * 5);

        stickerWidth = selectedStickerTexture.width;
        stickerHeight = selectedStickerTexture.height;
        stickerBytes = new byte[stickerWidth * stickerHeight * 4];

        int pixel = 0;
        for (int y = 0; y < stickerHeight; y++)
        {
            for (int x = 0; x < stickerWidth; x++)
            {
                Color brushPixel = selectedStickerTexture.GetPixel(x, y);
                stickerBytes[pixel] = (byte)(brushPixel.r * 255);
                stickerBytes[pixel + 1] = (byte)(brushPixel.g * 255);
                stickerBytes[pixel + 2] = (byte)(brushPixel.b * 255);
                stickerBytes[pixel + 3] = (byte)(brushPixel.a * 255);
                pixel += 4;
            }
        }

        // precalculate values
        stickerWidthHalf = (int)(stickerWidth * 0.5f);
        texWidthMinusStickerWidth = texWidth - stickerWidth;
        texHeightMinusStickerHeight = texHeight - stickerHeight;
    }

    public void OnPatternButtonClicked()
    {
        drawMode = DrawMode.Pattern;

        OnPatternButtonClicked(selectedPattern);
        OnChangeBrushSizeMenuButtonClicked();
    }

   
    public void OnPatternButtonClicked(int num)
    {
        selectedPattern = num;

        brushColorImage.color = Color.white;
        brushColorImage.sprite = Sprite.Create(customPatterns[selectedPattern], new Rect(0, 0, customPatterns[selectedPattern].width, customPatterns[selectedPattern].height), new Vector2(0.5f, 0.5f));

        Texture2D selectedTexture = DuplicateTexture(customPatterns[selectedPattern]);

        customPatternWidth = selectedTexture.width;
        customPatternHeight = selectedTexture.height;
        patternBrushBytes = new byte[customPatternWidth * customPatternHeight * 4];

        int pixel = 0;
        for (int x = 0; x < customPatternWidth; x++)
        {
            for (int y = 0; y < customPatternHeight; y++)
            {
                Color brushPixel = selectedTexture.GetPixel(x, y);

                patternBrushBytes[pixel] = (byte)(brushPixel.r * 255);
                patternBrushBytes[pixel + 1] = (byte)(brushPixel.g * 255);
                patternBrushBytes[pixel + 2] = (byte)(brushPixel.b * 255);
                patternBrushBytes[pixel + 3] = (byte)(brushPixel.a * 255);

                pixel += 4;
            }
        }
    }


    public void OnGlittersButtonClicked()
    {
        drawMode = DrawMode.Glitters;

        OnGlittersButtonClicked(selectedGlitters);
        OnChangeBrushSizeMenuButtonClicked();
    }
    public void OnGlittersButtonClicked(int num)
    {
        selectedGlitters = num;

        brushColorImage.color = Color.white;
        brushColorImage.sprite = Sprite.Create(customGlitters[selectedGlitters], new Rect(0, 0, customGlitters[selectedGlitters].width, customGlitters[selectedGlitters].height), new Vector2(0.5f, 0.5f));

        Texture2D selectedTexture = DuplicateTexture(customGlitters[selectedGlitters]);

        customPatternWidth = selectedTexture.width;
        customPatternHeight = selectedTexture.height;
        patternBrushBytes = new byte[customPatternWidth * customPatternHeight * 4];

        int pixel = 0;
        for (int x = 0; x < customPatternWidth; x++)
        {
            for (int y = 0; y < customPatternHeight; y++)
            {
                Color brushPixel = selectedTexture.GetPixel(x, y);

                patternBrushBytes[pixel] = (byte)(brushPixel.r * 255);
                patternBrushBytes[pixel + 1] = (byte)(brushPixel.g * 255);
                patternBrushBytes[pixel + 2] = (byte)(brushPixel.b * 255);
                patternBrushBytes[pixel + 3] = (byte)(brushPixel.a * 255);

                pixel += 4;
            }
        }
    }

    public void OnChangeBrushSizeMenuButtonClicked()
    {
        foreach (Button b in buttonBrushesSize)
        {
            b.image.color = Color.white;
        }

        paintBucketButton.image.color = Color.gray;

        switch (drawMode)
        {
            case DrawMode.Pencil:
            case DrawMode.Pattern:
                {
                    paintBucketButton.gameObject.SetActive(true);
                }
                break;
            case DrawMode.Glitters:
                {
                    paintBucketButton.gameObject.SetActive(true);
                }
                break;
            case DrawMode.Marker:
            case DrawMode.Spray:
            case DrawMode.Magic:
            case DrawMode.Sticker:
                {
                    SetPaintBucketMode(false);
                    // paintBucketButton.gameObject.SetActive(false);
                }
                break;
        }

        if (paintBucketMode)
        {
            paintBucketButton.image.color = Color.white;
        }
        else
        {
            buttonBrushesSize[brushesSize.IndexOf(brushSize)].image.color = Color.black;
        }
    }

    public void SetPaintBucketMode(bool value)
    {
        paintBucketMode = value;
    }

    public void OnChangeBrushSizeButtonClicked(int num)
    {
        brushSize = brushesSize[num];
        buttonBrushesSize[num].image.color = Color.black;
        SetPaintBucketMode(false);
        OnChangeBrushSizeMenuButtonClicked();

        switch (drawMode)
        {
            case DrawMode.Spray:
                {
                    ReadSprayModeTexture();
                }
                break;


            case DrawMode.Sticker:
                {
                    OnStickerButtonClicked();
                }
                break;
        }
    }

    public void OnUndoButtonClicked()
    {
        if (undoPixels.Count - RedoIndex - 1 > 0)
        {
            System.Array.Copy(undoPixels[undoPixels.Count - RedoIndex - 2], pixels, undoPixels[undoPixels.Count - RedoIndex - 2].Length);
            tex.LoadRawTextureData(undoPixels[undoPixels.Count - RedoIndex - 2]);
            tex.Apply(false);

            RedoIndex++;
        }
    }

    public void OnRedoButtonClicked()
    {
        if (undoPixels.Count > 0 && RedoIndex > 0)
        {
            System.Array.Copy(undoPixels[undoPixels.Count - RedoIndex], pixels, undoPixels[undoPixels.Count - RedoIndex].Length);
            tex.LoadRawTextureData(undoPixels[undoPixels.Count - RedoIndex]);
            tex.Apply(false);

            RedoIndex--;
        }
    }

    public void OnClearButtonClicked()
    {
        int pixel = 0;
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                pixels[pixel] = 255;
                pixels[pixel + 1] = 255;
                pixels[pixel + 2] = 255;
                pixels[pixel + 3] = 255;
                pixel += 4;
            }
        }
        tex.LoadRawTextureData(pixels);
        tex.Apply(false);

        if (undoPixels != null)
        {
            if (RedoIndex > 0)
            {
                undoPixels.RemoveRange(undoPixels.Count - RedoIndex, RedoIndex);
                RedoIndex = 0;
            }

            //undoPixels.Add(new byte[texWidth * texHeight * 4]);
            //System.Array.Copy(pixels, undoPixels[undoPixels.Count - 1], pixels.Length);
        }
    }

    public void OnScreenshotButtonClicked()
    {
        StartCoroutine(OnSavePictureClickListener());
    }

    private IEnumerator OnSavePictureClickListener()
    {


        yield return null;
    }

    private Rect GUIRectWithObject(GameObject go)
    {
        Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        Vector3 ext = go.GetComponent<Renderer>().bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
         {
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
               WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
               WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
         };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        return screenPoint;
    }

  

    public void OnChangeThemeButtonClicked(bool set = false)
    {
        int changeThemeIndex = PlayerPrefs.GetInt("Theme", 0);
        if (!set) changeThemeIndex++;

        if (changeThemeIndex >= themes.Count)
        {
            changeThemeIndex = 0;
        }

        PlayerPrefs.SetInt("Theme", changeThemeIndex);
        PlayerPrefs.Save();

        // mainCamera.backgroundColor = themes[changeThemeIndex];
    }

    public void OnHomeButtonClicked()
    {
        SaveImage(ID);



    }

    #endregion


    #region Painting Functions

    private void DrawCircle(int x, int y)
    {
        int pixel = 0;

        // draw fast circle: 
        int r2 = brushSize * brushSize;
        int area = r2 << 2;
        int rr = brushSize << 1;
        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - brushSize;
            int ty = (i / rr) - brushSize;

            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4;

                if (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1))
                {
                    pixels[pixel] = paintColor.r;
                    pixels[pixel + 1] = paintColor.g;
                    pixels[pixel + 2] = paintColor.b;
                    pixels[pixel + 3] = paintColor.a;
                }
            }
        }
    }

    private void DrawAdditiveCircle(int x, int y)
    {
        int pixel = 0;

        // draw fast circle: 
        int r2 = brushSize * brushSize;
        int area = r2 << 2;
        int rr = brushSize << 1;
        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - brushSize;
            int ty = (i / rr) - brushSize;
            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4;

                // additive over white also
                if (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1))
                {
                    pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel], paintColor.r, paintColor.a / 255f * 0.1f);
                    pixels[pixel + 1] = (byte)Mathf.Lerp(pixels[pixel + 1], paintColor.g, paintColor.a / 255f * 0.1f);
                    pixels[pixel + 2] = (byte)Mathf.Lerp(pixels[pixel + 2], paintColor.b, paintColor.a / 255f * 0.1f);
                    pixels[pixel + 3] = (byte)Mathf.Lerp(pixels[pixel + 3], paintColor.a, paintColor.a / 255 * 0.1f);
                }
            }
        }
    }

    private void DrawMagicCircle(int x, int y)
    {
        int pixel = 0;

        // draw fast circle: 
        int r2 = brushSize * brushSize;
        int area = r2 << 2;
        int rr = brushSize << 1;
        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - brushSize;
            int ty = (i / rr) - brushSize;

            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4;

                if (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1))
                {


                    if (Random.Range(0, 2) == 1)
                    {
                        pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel], magicColor.r, magicColor.a / 255f * 0.1f);
                        pixels[pixel + 1] = (byte)Mathf.Lerp(pixels[pixel + 1], magicColor.g, magicColor.a / 255f * 0.1f);
                        pixels[pixel + 2] = (byte)Mathf.Lerp(pixels[pixel + 2], magicColor.b, magicColor.a / 255f * 0.1f);
                        pixels[pixel + 3] = (byte)Mathf.Lerp(pixels[pixel + 3], magicColor.a, magicColor.a / 255 * 0.1f);
                    }
                    else
                    {
                        pixels[pixel] = magicColor.r;
                        pixels[pixel + 1] = magicColor.g;
                        pixels[pixel + 2] = magicColor.b;
                        pixels[pixel + 3] = magicColor.a;
                    }
                }
            }
        }
    }

    private void DrawSticker(int px, int py)
    {
        // get position where we paint
        int startX = (int)(px - stickerWidthHalf);
        int startY = (int)(py - stickerWidthHalf);

        if (startX < 0)
        {
            startX = 0;
        }
        else
        {
            if (startX + stickerWidth >= texWidth) startX = texWidthMinusStickerWidth;
        }

        if (startY < 1)
        {
            startY = 1;
        }
        else
        {
            if (startY + stickerHeight >= texHeight) startY = texHeightMinusStickerHeight;
        }

        int pixel = (texWidth * startY + startX) * 4;
        int brushPixel = 0;

        for (int y = 0; y < stickerHeight; y++)
        {
            for (int x = 0; x < stickerWidth; x++)
            {
                brushPixel = (stickerWidth * (y) + x) * 4;

                // brush alpha is over 0 in this pixel
                if (stickerBytes[brushPixel + 3] > 0)
                {
                    pixels[pixel] = stickerBytes[brushPixel];
                    pixels[pixel + 1] = stickerBytes[brushPixel + 1];
                    pixels[pixel + 2] = stickerBytes[brushPixel + 2];
                    pixels[pixel + 3] = stickerBytes[brushPixel + 3];
                }

                pixel += 4;

            } // for x

            pixel = (texWidth * (startY == 0 ? 1 : startY + y) + startX + 1) * 4;
        } // for y
    }

    private void FloodFillMaskOnlyWithThreshold(int x, int y)
    {
        // get canvas hit color
        byte hitColorR = maskPixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = maskPixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = maskPixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = maskPixels[((texWidth * (y) + x) * 4) + 3];

        if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {
            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void FloodFillWithTreshold(int x, int y)
    {
        // get canvas hit color
        byte hitColorR = pixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = pixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = pixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = pixels[((texWidth * (y) + x) * 4) + 3];

        if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {
            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    DrawPoint(pixel);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void DrawPatternCircle(int x, int y)
    {
        int pixel = 0;

        int r2 = brushSize * brushSize;
        int area = r2 << 2;
        int rr = brushSize << 1;

        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - brushSize;
            int ty = (i / rr) - brushSize;

            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4; // << 2

                if (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1))
                {
                    float yy = Mathf.Repeat(y + ty, customPatternWidth);
                    float xx = Mathf.Repeat(x + tx, customPatternWidth);
                    int pixel2 = (int)Mathf.Repeat((customPatternWidth * xx + yy) * 4, patternBrushBytes.Length);

                    pixels[pixel] = patternBrushBytes[pixel2];
                    pixels[pixel + 1] = patternBrushBytes[pixel2 + 1];
                    pixels[pixel + 2] = patternBrushBytes[pixel2 + 2];
                    pixels[pixel + 3] = patternBrushBytes[pixel2 + 3];
                }
            }
        }
    }

    private void FloodFillPatternMaskOnlyWithThreshold(int x, int y)
    {
        // get canvas hit color
        byte hitColorR = maskPixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = maskPixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = maskPixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = maskPixels[((texWidth * (y) + x) * 4) + 3];

        //if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {
            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(maskPixels[pixel + 0], hitColorR)
                    && CompareThreshold(maskPixels[pixel + 1], hitColorG)
                    && CompareThreshold(maskPixels[pixel + 2], hitColorB)
                    && CompareThreshold(maskPixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void FloodFillPatternWithTreshold(int x, int y)
    {
        // get canvas hit color
        byte hitColorR = pixels[((texWidth * (y) + x) * 4) + 0];
        byte hitColorG = pixels[((texWidth * (y) + x) * 4) + 1];
        byte hitColorB = pixels[((texWidth * (y) + x) * 4) + 2];
        byte hitColorA = pixels[((texWidth * (y) + x) * 4) + 3];

        //if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;

        Queue<int> fillPointX = new Queue<int>();
        Queue<int> fillPointY = new Queue<int>();
        fillPointX.Enqueue(x);
        fillPointY.Enqueue(y);

        int ptsx, ptsy;
        int pixel = 0;

        lockMaskPixels = new byte[texWidth * texHeight * 4];

        while (fillPointX.Count > 0)
        {
            ptsx = fillPointX.Dequeue();
            ptsy = fillPointY.Dequeue();

            if (ptsy - 1 > -1)
            {
                pixel = (texWidth * (ptsy - 1) + ptsx) * 4; // down
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy - 1);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx + 1 < texWidth)
            {
                pixel = (texWidth * ptsy + ptsx + 1) * 4; // right
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx + 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsx - 1 > -1)
            {
                pixel = (texWidth * ptsy + ptsx - 1) * 4; // left
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx - 1);
                    fillPointY.Enqueue(ptsy);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }

            if (ptsy + 1 < texHeight)
            {
                pixel = (texWidth * (ptsy + 1) + ptsx) * 4; // up
                if (lockMaskPixels[pixel] == 0
                    && CompareThreshold(pixels[pixel + 0], hitColorR)
                    && CompareThreshold(pixels[pixel + 1], hitColorG)
                    && CompareThreshold(pixels[pixel + 2], hitColorB)
                    && CompareThreshold(pixels[pixel + 3], hitColorA))
                {
                    fillPointX.Enqueue(ptsx);
                    fillPointY.Enqueue(ptsy + 1);
                    DrawPatternPaintBucketCircle(ptsx, ptsy);
                    lockMaskPixels[pixel] = 1;
                }
            }
        }
    }

    private void DrawPatternPaintBucketCircle(int x, int y)
    {
        int pixel = 0;

        int r2 = 1 * 1;
        int area = r2 << 2;
        int rr = 1 << 1;

        for (int i = 0; i < area; i++)
        {
            int tx = (i % rr) - 1;
            int ty = (i / rr) - 1;

            if (tx * tx + ty * ty < r2)
            {
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue;

                pixel = (texWidth * (y + ty) + x + tx) * 4; // << 2

                if (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1))
                {
                    float yy = Mathf.Repeat(y + ty, customPatternWidth);
                    float xx = Mathf.Repeat(x + tx, customPatternWidth);
                    int pixel2 = (int)Mathf.Repeat((customPatternWidth * xx + yy) * 4, patternBrushBytes.Length);

                    pixels[pixel] = patternBrushBytes[pixel2];
                    pixels[pixel + 1] = patternBrushBytes[pixel2 + 1];
                    pixels[pixel + 2] = patternBrushBytes[pixel2 + 2];
                    pixels[pixel + 3] = patternBrushBytes[pixel2 + 3];
                }
            }
        }
    }

    private void DrawSprayBrush(int px, int py)
    {
        ReadSprayModeTexture();

        int startX = (int)(px - sprayModeWidthHalf);
        int startY = (int)(py - sprayModeWidthHalf);

        if (startX < 0)
        {
            startX = 0;
        }
        else
        {
            if (startX + sprayModeWidth >= texWidth) startX = texWidthMinusSprayModeWidth;
        }

        if (startY < 1)
        {
            startY = 1;
        }
        else
        {
            if (startY + sprayModeHeight >= texHeight) startY = texHeightMinusSprayModeHeight;
        }

        int pixel = (texWidth * startY + startX) << 2;
        int brushPixel = 0;

        for (int y = 0; y < sprayModeHeight; y++)
        {
            for (int x = 0; x < sprayModeWidth; x++)
            {
                brushPixel = (sprayModeWidth * (y) + x) << 2;

                if (sprayModeBrushBytes[brushPixel + 3] > 0 && (!useLockArea || (useLockArea && lockMaskPixels[pixel] == 1)))
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel], paintColor.r, paintColor.a / 255f * 0.1f);
                        pixels[pixel + 1] = (byte)Mathf.Lerp(pixels[pixel + 1], paintColor.g, paintColor.a / 255f * 0.1f);
                        pixels[pixel + 2] = (byte)Mathf.Lerp(pixels[pixel + 2], paintColor.b, paintColor.a / 255f * 0.1f);
                        pixels[pixel + 3] = (byte)Mathf.Lerp(pixels[pixel + 3], paintColor.a, paintColor.a / 255 * 0.1f);
                    }
                    else
                    {
                        pixels[pixel] = paintColor.r;
                        pixels[pixel + 1] = paintColor.g;
                        pixels[pixel + 2] = paintColor.b;
                        //pixels[pixel + 3] = sprayModeBrushBytes[brushPixel + 3];
                        pixels[pixel + 3] = 255;
                    }
                }

                pixel += 4;
            }

            pixel = (texWidth * (startY == 0 ? 1 : startY + y) + startX + 1) * 4;
        }
    }

    private bool CompareThreshold(byte a, byte b)
    {
        if (a < b)
        {
            a ^= b; b ^= a; a ^= b;
        }

        return (a - b) <= 128;
    }

    private void DrawPoint(int pixel)
    {
        pixels[pixel] = paintColor.r;
        pixels[pixel + 1] = paintColor.g;
        pixels[pixel + 2] = paintColor.b;
        pixels[pixel + 3] = paintColor.a;
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx, sy;
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;
        bool loop = true;
        int minDistance = (int)(brushSize >> 1);
        int pixelCount = 0;
        int e2;
        while (loop)
        {
            pixelCount++;
            if (pixelCount > minDistance)
            {
                pixelCount = 0;
                DrawCircle(x0, y0);
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }

    private void DrawAdditiveLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx, sy;
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;
        bool loop = true;
        int minDistance = (int)(brushSize >> 1);
        int pixelCount = 0;
        int e2;
        while (loop)
        {
            pixelCount++;
            if (pixelCount > minDistance)
            {
                pixelCount = 0;
                DrawAdditiveCircle(x0, y0);
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }

    private void DrawMagicLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx, sy;
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;
        bool loop = true;
        int minDistance = (int)(brushSize >> 1);
        int pixelCount = 0;
        int e2;
        while (loop)
        {
            pixelCount++;
            if (pixelCount > minDistance)
            {
                pixelCount = 0;
                DrawMagicCircle(x0, y0);
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }

    private void DrawLineWithPattern(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx, sy;
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;
        bool loop = true;
        int minDistance = (int)(brushSize >> 1);
        int pixelCount = 0;
        int e2;
        while (loop)
        {
            pixelCount++;
            if (pixelCount > minDistance)
            {
                pixelCount = 0;
                DrawPatternCircle(x0, y0);
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }

    #endregion


    #region Public Method

    public void GotoNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GotoPreviousLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    public void Ads()
    {

    }
    #endregion
    
}