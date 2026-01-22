using UnityEngine;
using UnityEngine.UI;

public class NarrativeHUDController : MonoBehaviour
{
    public static NarrativeHUDController Instance;

    [Header("UI")]
    public Text narrativeText;
    public Transform choicesPanel;
    public GameObject choiceButtonPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowNode(NarrativeNode node, NarrativeManager manager)
    {
        narrativeText.text = node.narrativeText;

        foreach (Transform t in choicesPanel)
            Destroy(t.gameObject);

        foreach (var choice in node.choices)
        {
            var obj = Instantiate(choiceButtonPrefab, choicesPanel);
            var btn = obj.GetComponent<NarrativeChoice>();
            btn.Setup(manager, choice);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
