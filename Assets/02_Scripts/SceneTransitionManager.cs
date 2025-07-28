using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("----- ���̵� ���� -----")]
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeSpeed = 1f;
    [SerializeField] Color _fadeColor = Color.black;

    [Header("----- �ε� UI -----")]
    [SerializeField] GameObject _loadingPanel;
    [SerializeField] Slider _loadingSlider;
    [SerializeField] TMPro.TextMeshProUGUI _loadingText;

    /// <summary>
    /// �� ��ȯ �Ŵ��� �̱���
    /// </summary>
    public static SceneTransitionManager Instance { get; private set; }

    private bool _isTransitioning = false;

    private void Awake()
    {
        // �̱��� ����
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
        // ���� ���� �� ���̵���
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// ���̵� �̹��� �ʱ�ȭ
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
    /// ���̵�� �Բ� �� ��ȯ
    /// </summary>
    public void TransitionToScene(string sceneName)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }

    /// <summary>
    /// �� ��ȯ �ڷ�ƾ
    /// </summary>
    private IEnumerator TransitionCoroutine(string sceneName)
    {
        _isTransitioning = true;

        // ���̵� �ƿ�
        yield return StartCoroutine(FadeOut());

        // �ε� �г� Ȱ��ȭ
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(true);
        }

        // �� �񵿱� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // �ε� ���൵ ǥ��
        while (asyncLoad.progress < 0.9f)
        {
            UpdateLoadingUI(asyncLoad.progress / 0.9f);
            yield return null;
        }

        // �ε� �Ϸ� ��� (�ּ� �ð�)
        yield return new WaitForSeconds(0.5f);
        UpdateLoadingUI(1f);

        // �� Ȱ��ȭ
        asyncLoad.allowSceneActivation = true;

        // �� �ε� �Ϸ� ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �ε� �г� ��Ȱ��ȭ
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(false);
        }

        // ���̵� ��
        yield return StartCoroutine(FadeIn());

        _isTransitioning = false;
    }

    /// <summary>
    /// �ε� UI ������Ʈ
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
    /// ���̵� �ƿ� �ڷ�ƾ
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
    /// ���̵� �� �ڷ�ƾ
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
    /// ��� ���̵� �ƿ� (�� ���� �� ���)
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
    /// ��� ���̵� �� (�� ���� �� ���)
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
