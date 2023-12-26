using SocialTrender;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_SearchField;
    [SerializeField] private RawImage[] m_Images;
    [SerializeField] private TMP_Text[] m_ProgressTexts;
    [SerializeField] private Material m_ProgressMat;
    [SerializeField] private GameObject[] m_LoadingPages;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        SocialMediaTrendManager.Instance.OnCompleted += OnSuccess;
        SocialMediaTrendManager.Instance.OnFailed += OnFail;
        SocialMediaTrendManager.Instance.OnProgressReceived += UpdateProgress;
    }

    private void OnDisable()
    {
        SocialMediaTrendManager.Instance.OnCompleted -= OnSuccess;
        SocialMediaTrendManager.Instance.OnFailed -= OnFail;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
            QuitApplication();

        if (onSuccess)
        {
            onSuccess = false;

            UpdateImages();
            SetActivePostImages(true);
            SetActiveLoadingPages(false);
        }

        if (onFail)
        {
            onFail = false;

            foreach (TMP_Text progressText in m_ProgressTexts)
            {
                progressText.text = "Failed";
            }
        }

        if (m_UpdateProgress)
        {
            m_UpdateProgress = false;

            m_ProgressMat.SetFloat("_Progress", m_Progress);

            foreach (TMP_Text progressText in m_ProgressTexts)
            {
                progressText.text = m_ProgressInfo;
            }
        }
    }

    public void QuitApplication()
    {
        if (Application.isEditor)
            return;

        Application.Quit();
    }

    private bool onSuccess = false;
    private bool onFail = false;
    private bool m_UpdateProgress = false;
    private float m_Progress = 0.0f;
    private string m_ProgressInfo = "";

    public void SetActiveLoadingPages(bool enable)
    {
        foreach (GameObject page in m_LoadingPages)
        {
            page.SetActive(enable);
        }
    }

    public void SetActivePostImages(bool enable)
    {
        foreach (RawImage post in m_Images)
        {
            post.gameObject.SetActive(enable);
        }
    }

    private void OnSuccess()
    {
        onSuccess = true;
    }

    private void OnFail()
    {
        onFail = true;
    }

    private void UpdateImages()
    {
        List<byte[]> images = SocialMediaTrendManager.Instance.Images;
        PostData[] posts = SocialMediaTrendManager.Instance.Posts;

        for (int i = 0; i < images.Count; i++)
        {
            Texture2D texture = new Texture2D(800, 600);
            texture.LoadImage(images[i]);
            m_Images[i].texture = texture;
            m_Images[i].GetComponent<PostHolder>().link = posts[i].Link;
        }
    }

    private void UpdateProgress()
    {
        m_Progress = Mathf.Clamp01(SocialMediaTrendManager.Instance.Progress);
        m_ProgressInfo = SocialMediaTrendManager.Instance.ProgressMessage;
        m_UpdateProgress = true;
    }

    public void SearchKeyword()
    {
        SocialMediaTrendManager.Instance.Search("#" + m_SearchField.text);
    }

    public void OnSearchValueChanged(string searchValue)
    {
        if (string.IsNullOrEmpty(searchValue) || string.IsNullOrWhiteSpace(searchValue))
        {
            m_SearchField.text = "";
            return;
        }

        if (searchValue[searchValue.Length - 1] == ' ')
        {
            m_SearchField.text = searchValue.Split(' ')[0].Trim();
        }
    }
}
