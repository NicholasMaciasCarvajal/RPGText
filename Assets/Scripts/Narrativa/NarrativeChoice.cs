using UnityEngine;
using UnityEngine.UI;

public class NarrativeChoice : MonoBehaviour
{
    public Text label;

    private NarrativeChoiceData data;
    private NarrativeManager manager;

    public void Setup(NarrativeManager mgr, NarrativeChoiceData choiceData)
    {
        manager = mgr;
        data = choiceData;

        label.text = data.choiceText;
    }

    public void OnClick()
    {
        manager.SelectChoice(data);
    }
}
