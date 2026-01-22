using UnityEngine;
using UnityEngine.UI;

public class ProgressionUIController : MonoBehaviour
{
    public static ProgressionUIController Instance;

    [Header("UI References")]
    public GameObject panel;
    public Button advanceButton;
    public Button backButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Hide();
    }

    // =================== MOSTRAR OPCIONES ===================

    public void Show(bool canGoBack)
    {
        panel.SetActive(true);

        backButton.gameObject.SetActive(canGoBack);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    // =================== BOTONES ===================

    public void OnAdvancePressed()
    {
        Hide();
        ProgressionManager.Instance.ChooseOption(0);
    }

    public void OnBackPressed()
    {
        Hide();
        ProgressionManager.Instance.ChooseOption(1);
    }
}
