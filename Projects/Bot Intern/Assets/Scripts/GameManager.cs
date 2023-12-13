using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_DebugText;
    [SerializeField] private TMP_InputField m_SearchField;
    [SerializeField] private RawImage[] m_Images;

    private void OnEnable()
    {
        SocialMediaTrendManager.Instance.OnCompleted += OnSuccess;
        SocialMediaTrendManager.Instance.OnFailed += OnFail;
        SocialMediaTrendManager.Instance.OnSearchBegin += OnSearchBegin;
    }

    private void OnDisable()
    {
        SocialMediaTrendManager.Instance.OnCompleted -= OnSuccess;
        SocialMediaTrendManager.Instance.OnFailed -= OnFail;
        SocialMediaTrendManager.Instance.OnSearchBegin -= OnSearchBegin;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
            QuitApplication();

        if (onBegin)
        {
            onBegin = false;
            m_DebugText.text = "Begin";
        }

        if (onSuccess)
        {
            onSuccess = false;
            m_DebugText.text = $"Search Completed : {SocialMediaTrendManager.Instance.PostLinks.Length}";

            UpdateImages();
        }

        if (onFail)
        {
            onFail = false;
            m_DebugText.text = "Search Failed";
        }
    }

    public void QuitApplication()
    {
        if (Application.isEditor)
            return;

        Application.Quit();
    }

    private bool onBegin = false;
    private bool onSuccess = false;
    private bool onFail = false;

    private void OnSearchBegin()
    {
        onBegin = true;
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
        string[] links = SocialMediaTrendManager.Instance.PostLinks;

        for (int i = 0; i < images.Count; i++)
        {
            Texture2D texture = new Texture2D(800, 600);
            texture.LoadImage(images[i]);
            m_Images[i].texture = texture;
            m_Images[i].GetComponent<PostHolder>().link = links[i];
        }
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
