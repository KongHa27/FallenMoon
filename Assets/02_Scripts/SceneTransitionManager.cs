using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("----- 페이드 설정 -----")]
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeSpeed = 1f;
    [SerializeField] Color _fadeColor = Color.black;

    [Header("----- 로딩 UI -----")]
    [SerializeField] GameObject _loadingPanel;
    [SerializeField] Slider _loadingSlider;
    [SerializeField] TMPro.TextMeshProUGUI _loadingText;

    /// <summary>
    /// 씬 전환 매니저 싱글톤
    /// </summary>
    public static SceneTransitionManager Instance { get; private set; }

    private bool _isTransitioning = false;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFadeImage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 페이드인
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 페이드 이미지 초기화
    /// </summary>
    private void InitializeFadeImage()
    {
        if (_fadeImage != null)
        {
            _fadeImage.color = _fadeColor;
            _fadeImage.raycastTarget = true;
        }
    }

    /// <summary>
    /// 페이드와 함께 씬 전환
    /// </summary>
    public void TransitionToScene(string sceneName)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }

    /// <summary>
    /// 씬 전환 코루틴
    /// </summary>
    private IEnumerator TransitionCoroutine(string sceneName)
    {
        _isTransitioning = true;

        // 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 로딩 패널 활성화
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(true);
        }

        // 씬 비동기 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // 로딩 진행도 표시
        while (asyncLoad.progress < 0.9f)
        {
            UpdateLoadingUI(asyncLoad.progress / 0.9f);
            yield return null;
        }

        // 로딩 완료 대기 (최소 시간)
        yield return new WaitForSeconds(0.5f);
        UpdateLoadingUI(1f);

        // 씬 활성화
        asyncLoad.allowSceneActivation = true;

        // 씬 로딩 완료 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 로딩 패널 비활성화
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(false);
        }

        // 페이드 인
        yield return StartCoroutine(FadeIn());

        _isTransitioning = false;
    }

    /// <summary>
    /// 로딩 UI 업데이트
    /// </summary>
    private void UpdateLoadingUI(float progress)
    {
        if (_loadingSlider != null)
        {
            _loadingSlider.value = progress;
        }

        if (_loadingText != null)
        {
            _loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
        }
    }

    /// <summary>
    /// 페이드 아웃 코루틴
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (_fadeImage == null) yield break;

        float alpha = 0f;
        _fadeImage.gameObject.SetActive(true);

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * _fadeSpeed;
            Color color = _fadeColor;
            color.a = alpha;
            _fadeImage.color = color;
            yield return null;
        }

        Color finalColor = _fadeColor;
        finalColor.a = 1f;
        _fadeImage.color = finalColor;
    }

    /// <summary>
    /// 페이드 인 코루틴
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (_fadeImage == null) yield break;

        float alpha = 1f;
        _fadeImage.gameObject.SetActive(true);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * _fadeSpeed;
            Color color = _fadeColor;
            color.a = alpha;
            _fadeImage.color = color;
            yield return null;
        }

        Color finalColor = _fadeColor;
        finalColor.a = 0f;
        _fadeImage.color = finalColor;
        _fadeImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 즉시 페이드 아웃 (씬 시작 시 사용)
    /// </summary>
    public void SetFadeOut()
    {
        if (_fadeImage != null)
        {
            Color color = _fadeColor;
            color.a = 1f;
            _fadeImage.color = color;
            _fadeImage.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 즉시 페이드 인 (씬 시작 시 사용)
    /// </summary>
    public void SetFadeIn()
    {
        if (_fadeImage != null)
        {
            Color color = _fadeColor;
            color.a = 0f;
            _fadeImage.color = color;
            _fadeImage.gameObject.SetActive(false);
        }
    }
}
