using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocialTrender;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

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

    public string[] PostLinks { get; private set; }
    public List<byte[]> Images { get; private set; }
    public float Progress { get; private set; }
    public string ProgressInfo { get; private set; }

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
        Trender.Init(m_ShowBrowser, null);
        Trender.SetSearchCompleteCallback(OnSearchCompleted);
        Trender.SetSearchFailedCallback(OnSearchFailed);
        Trender.SetProgressReceivedCallback(OnReceiveProgress);
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

    private void OnReceiveProgress(float progress, string info)
    {
        Progress = progress;
        ProgressInfo = info;

        OnProgressReceived?.Invoke();
    }

    private void OnSearchCompleted(string[] postLinks, List<byte[]> images)
    {
        PostLinks = postLinks;
        Images = images;

        OnCompleted?.Invoke();
    }

    private void OnSearchFailed()
    {
        OnFailed?.Invoke();

        print("SEARCH FAILED !!!!!!!!");
    }
}
