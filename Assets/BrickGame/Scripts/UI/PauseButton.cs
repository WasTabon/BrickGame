using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    public PausePanel pausePanel;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (pausePanel != null) pausePanel.Pause();
        });
    }
}
