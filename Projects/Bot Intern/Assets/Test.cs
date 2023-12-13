using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;

public class Test : MonoBehaviour
{
    public RawImage image;

    private void OnStart()
    {
        byte[] bytes = new byte[]
        {
            255, 0, 0, 255,
            0, 0, 255, 255,
            255, 255, 0, 255,
            0, 255, 0, 255
        };

        Color32[] colors = new Color32[4];
        for (int i = 0; i < colors.Length; i++)
        {
            int row = i * 4;
            colors[i] = new Color32(bytes[row], bytes[row + 1], bytes[row + 2], bytes[row + 3]);
        }

        bytes = File.ReadAllBytes("D:\\Programming\\C#\\LearnSelenium\\LearnSelenium\\bin\\Debug\\net8.0\\Screenshot\\3.png");
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(bytes);
        texture.Apply();
        
        image.texture = texture;
    }

    private void Start()
    {
        print("Search started");
        Search("#games", 20);
        print("Search end");
    }

    public async void Search(string keyword, int maxPostCount)
    {
        for (int i = 0; i < 3; i++)
        {
            print($"Searching {i}");
            await SearchAsync(keyword, maxPostCount);
        }
    }

    public async Task SearchAsync(string keyword, int maxPostCount)
    {
        await Task.Run(() => OnSearch(keyword, maxPostCount));
    }

    private void OnSearch(string keyword, int maxPostCount)
    {
        Thread.Sleep(5000);
    }
}
