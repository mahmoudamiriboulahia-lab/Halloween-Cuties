using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    public string saveIndexString = "ColoringList";
    public GameObject scrollbar, imageContent;
    private float scroll_pos = 0;
    float[] pos;
    private bool runIt = false;
    private float time;
    private Button takeTheBtn;
    int btnNumber;
     Button _Center;
    public int MyCenterNumber;
    public int ColoringCount;
    public GameObject _Content;
    public GameObject PrefabSelect;
    List<Image> ListColoringImage = new List<Image>();
    bool _start = true;
    public GameObject MyTransition;
    // Start is called before the first frame update

    void Start()
    {
        for (int i = 0; i < ColoringCount; i++)
        {
           //  ListColoringImage.Add(Resources.Load<Image>("_Game/0" + i));
            // GameObject _postion = Instantiate(PrefabSelect, _Content.transform);
//             _Content.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().sprite= Resources.Load<Sprite>("_Game/0" + i);
            //buttonStickers[i].sprite = Sprite.Create(stickers[i], new Rect(0, 0, stickers[i].width, stickers[i].height), new Vector2(0.5f, 0.5f));
           // _postion.GetComponent<Button>().AddEventListenerOr(i, LoadGame);

        }
        TaskOnClick();
        _Center = _Content.transform.GetChild(MyCenterNumber).gameObject.GetComponent<Button>();

    }
    void TaskOnClick()
    {
        // WhichBtnClicked(_Cn);

    }
    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        if (runIt)
        {
            GecisiDuzenle(distance, pos, takeTheBtn);
            time += Time.deltaTime;

            if (time > 1f)
            {
                time = 0;
                runIt = false;
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }


        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                if (_start)
                {
                    WhichBtnClicked(_Center);
                    _start = false;
                }

                Debug.LogWarning("Current Selected Level" + i);
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
//                imageContent.transform.GetChild(i).localScale = Vector2.Lerp(imageContent.transform.GetChild(i).localScale, new Vector2(2.3f, 2.3f), 2f);
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j != i)
                    {
//                        imageContent.transform.GetChild(j).localScale = Vector2.Lerp(imageContent.transform.GetChild(j).localScale, new Vector2(2f, 2f), 0.1f);
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }


    }

    private void GecisiDuzenle(float distance, float[] pos, Button btn)
    {
        // btnSayi = System.Int32.Parse(btn.transform.name);

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[btnNumber], 1f * Time.deltaTime);

            }
        }

        for (int i = 0; i < btn.transform.parent.transform.childCount; i++)
        {
            btn.transform.name = ".";
        }

    }
    public void WhichBtnClicked(Button btn)
    {

        btn.transform.name = "clicked";
        for (int i = 0; i < btn.transform.parent.transform.childCount; i++)
        {
            if (btn.transform.parent.transform.GetChild(i).transform.name == "clicked")
            {
                btnNumber = i;
                takeTheBtn = btn;
                time = 0;
                scroll_pos = (pos[btnNumber]);
                runIt = true;
            }
        }


    }


    public void LoadGame(int index)
    {


        PlayerPrefs.SetInt(saveIndexString, index);
        PlayerPrefs.Save();

        if (_Content.transform.GetChild(index).childCount > 0)
        {
            ColoringBookManager.maskTexIndex = index;
        }
        else
        {
            ColoringBookManager.maskTexIndex = -1;
        }

        ColoringBookManager.ID = saveIndexString + index.ToString();
        MyTransition.transform.GetComponent<LoadSceneManager>()._btn_pindah("PaintScene");
    }
}