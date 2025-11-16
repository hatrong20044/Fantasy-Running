using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Loading : MonoBehaviour
{
    public VideoPlayer introVid;
    public int nextSceneIndex = 1;
    public Image image;
    public float fadeDuration = 0.5f;    // Thời gian fade-out (giây)
    private AsyncOperation loadOperation;
    void Start()
    {
        // Play video
        this.PlayVideo();

        // Đăng ký event khi video kết thúc
        introVid.loopPointReached += OnVideoFinished;

        // Bắt đầu pre-load scene ngay
        StartCoroutine(PreloadScene());

    }
    public void PlayVideo()
    {
        if (introVid)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, "IntroLogo.mp4");
            Debug.Log(videoPath);
            introVid.url = videoPath;
            introVid.Play();
        }

    }

    IEnumerator PreloadScene()
    {
        // Load scene nhưng KHÔNG cho active (dừng ở 90%)
        loadOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        loadOperation.allowSceneActivation = false;

        // Chờ scene load xong (đến 90%)
        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }

    }

    void OnVideoFinished(VideoPlayer vp)
    {

        StartCoroutine(HandleTransition());
    }


    IEnumerator HandleTransition()
    {

        yield return StartCoroutine(FadeOut());
        loadOperation.allowSceneActivation = true;

    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = image.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            image.color = c;
            yield return null;
        }
        c.a = 1f;
        image.color = c;
    }
    void OnDestroy()
    {

        introVid.loopPointReached -= OnVideoFinished;

    }
}


