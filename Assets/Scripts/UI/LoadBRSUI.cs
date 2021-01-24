using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadBRSUI : MonoBehaviour
{
    public GameObject GameScripts;
    public GameObject BuildButtonPrefab;
    public Button LoadBricksButton;
    public Transform ScrollContent;
    public Image BuildPreviewImage;
    public Text BuildPreviewText;

    private List<Transform> Buttons = new List<Transform>();
    private Color OriginalPreviewColor;
    private Sprite PreviewSprite;
    private BRS SelectedBRS;

    private void Start()
    {
        OriginalPreviewColor = BuildPreviewImage.color;
        LoadGallery();

        LoadBricksButton.onClick.AddListener(() => LoadBricks());

        if (GameScripts == null)
            GameScripts = GameObject.Find("Game Scripts");

        if (SelectedBRS == null)
            SelectedBRS = GameScripts.GetComponent<BRS>();
    }

    public void LoadGallery()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            Destroy(Buttons[i]);
        }

        Buttons.Clear();

        string[] Files = Directory.GetFiles(BRS.BUILDS_PATH);

        StartCoroutine(LoadNext(Files, 0));
    }

    public IEnumerator LoadNext(string[] Files, int i)
    {
        yield return new WaitForEndOfFrame();

        FileInfo FInfo = new FileInfo(Files[i]);

        if (FInfo.Extension == ".brs")
        {
            //Debug.Log("Found BRS: " + FInfo.Name);

            SelectedBRS.buildName = FInfo.Name.Replace(FInfo.Extension, "");

            SelectedBRS.LoadBRS(false);

            GameObject NewButton = Instantiate(BuildButtonPrefab, ScrollContent);
            Buttons.Add(NewButton.transform);

            SpecializedButton ButtonScript = NewButton.GetComponent<SpecializedButton>();
            ButtonScript.ButtonText.text = SelectedBRS.buildName;

            Texture2D PreviewTex = SelectedBRS.GetPreview();
            PreviewSprite = null;

            if (PreviewTex != null)
            {
                PreviewSprite = Sprite.Create(PreviewTex, new Rect(0, 0, PreviewTex.width, PreviewTex.height), new Vector2(0.5f, 0.5f));
                ButtonScript.ButtonImage.sprite = PreviewSprite;
                ButtonScript.ButtonImage.color = Color.white;
            }

            ButtonScript.StoredObjects = new object[1];

            ButtonScript.StoredObjects[0] = SelectedBRS.buildName;
            ButtonScript.SelfButton.onClick.AddListener(() => SetPreviewedBRS((string)ButtonScript.StoredObjects[0], ButtonScript.ButtonImage.sprite));
        }

        if(i < Files.Length-1)
            StartCoroutine(LoadNext(Files, ++i));
    }

    public void SetPreviewedBRS(string Save, Sprite Preview)
    {
        BuildPreviewText.text = "  " + Save;

        if (Preview != null)
        {
            BuildPreviewImage.sprite = Preview;
            BuildPreviewImage.color = Color.white;
        } else
        {
            BuildPreviewImage.sprite = null;
            BuildPreviewImage.color = OriginalPreviewColor;
        }

        BuildPreviewImage.preserveAspect = true;

        SelectedBRS.buildName = Save;
    }

    public void LoadBricks()
    {
        SelectedBRS.LoadBRS();
        Close();
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
