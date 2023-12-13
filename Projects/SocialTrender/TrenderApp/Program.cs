using SocialTrender;

bool m_IsRunning = true;

Trender.Init(false, Log.Info);

Trender.SetSearchCompleteCallback((links, images) =>
{
    m_IsRunning = false;
    Log.Success("Search completed successfully");

    foreach (var link in links)
        Log.Link(link);
});

Trender.SetSearchFailedCallback(() =>
{
    m_IsRunning = false;
    Log.Error("SEARCH FAILED !!!");
});

Trender.Search("#younginnovator", 20);

while (m_IsRunning)
{
    Thread.Sleep(1000);
}

Console.ReadLine();

Trender.Shutdown();
