using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField]
    private Color onColor;
    [SerializeField]
    private Color offColor;

    private bool toggled;
    private Button button;
    private Image buttonSprite;

    public delegate void ToggleDelegate(bool toggled);
    public event ToggleDelegate Toggled;

    private void Awake()
    {
        toggled = false;
        button = GetComponent<Button>();
        buttonSprite = GetComponent<Image>();

        if (button == null || buttonSprite == null)
        {
            Debug.LogWarning(gameObject.name + ": Toggle Button needs Button and Image");
        }
        else
        {
            button.onClick.AddListener(Toggle);
        }
    }

    public void Toggle()
    {
        toggled = !toggled;

        if (toggled)
        {
            buttonSprite.color = onColor;
        }
        else
        {
            buttonSprite.color = offColor;
        }

        Toggled?.Invoke(toggled);
    }

    public bool IsToggled()
    {
        return toggled;
    }
}
