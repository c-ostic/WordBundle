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

    /**
     * Called at the beginning of the scene starting up
     */
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

    /**
     * The listener for button clicks to toggle the button
     * Invokes the Toggled event with the current toggle state
     */
    private void Toggle()
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

    /**
     * Set the toggle state directly
     * Does not invoke the Toggled event
     */
    public void SetToggleState(bool newState)
    {
        toggled = newState;

        if (toggled)
        {
            buttonSprite.color = onColor;
        }
        else
        {
            buttonSprite.color = offColor;
        }
    }

    /**
     * Returns the state of the button
     */
    public bool IsToggled()
    {
        return toggled;
    }
}
