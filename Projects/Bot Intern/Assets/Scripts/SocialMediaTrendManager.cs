using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocialTrender;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;

public class SocialMediaTrendManager : MonoBehaviour
{
    private static SocialMediaTrendManager s_Instance;

    public static SocialMediaTrendManager Instance
    {
        get { return s_Instance; }
    }

    [SerializeField] private bool m_ShowBrowser = false;
    [SerializeField] private int m_MaxPostCount = 8;
    //[SerializeField] private string m_Keyword = "games";

    public PostData[] Posts { get; private set; }
    public List<byte[]> Images { get; private set; }
    public float Progress { get; private set; }
    public string ProgressMessage { get; private set; }

    public event Action OnCompleted;
    public event Action OnFailed;
    public event Action OnProgressReceived;

    private void Awake()
    {
        if (s_Instance != null)
        {
            Destroy(this);
            return;
        }

        s_Instance = this;
    }

    private void Start()
    {
        CallbackData data = new CallbackData();
        data.SearchCompleteCallback = OnSearchCompleted;
        data.SearchFailCallback = OnSearchFailed;
        data.ProgressCallback = OnReceiveProgress;
        data.LogMessageCallback = print;

        Trender.Init(m_ShowBrowser, Directory.GetCurrentDirectory());

        //Trender.SetSearchCompleteCallback(OnSearchCompleted);
        //Trender.SetSearchFailedCallback(OnSearchFailed);

    }

    private void OnDestroy()
    {
        if (s_Instance == this)
        {
            Trender.Shutdown();
        }
    }

    public void Search(string keyword)
    {
        Trender.Search(keyword, m_MaxPostCount);
    }

    private void OnReceiveProgress(ProgressData data)
    {
        Progress = data.Progress;
        ProgressMessage = data.Message;

        OnProgressReceived?.Invoke();
    }

    //private void OnSearchCompleted(string[] links, List<byte[]> images)
    //{
    //    Posts = links;
    //    Images = images;

    //    OnCompleted?.Invoke();
    //}

    private void OnSearchCompleted(UnityData data)
    {
        print("SEARCH Successfull !!!!!!!!");

        Posts = data.Posts;
        Images = data.Images;

        OnCompleted?.Invoke();
    }

    private void OnSearchFailed()
    {
        print("SEARCH FAILED !!!!!!!!");

        OnFailed?.Invoke();
    }
}
