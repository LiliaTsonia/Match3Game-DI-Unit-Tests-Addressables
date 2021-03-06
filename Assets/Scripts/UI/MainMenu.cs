using UnityEngine;
using UnityEngine.UI;

public class MainMenu : CommonUIPanel, IPanelWithListeners
{
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        AddListeners();
    }

    public void AddListeners()
    {
        _playBtn.onClick.AddListener(() => GameManager.Instance.LoadScene("Game"));
        _exitBtn.onClick.AddListener(GameManager.Instance.ExitGame);
    }
}
