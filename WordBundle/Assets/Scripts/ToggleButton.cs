using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a toggleable button
/// </summary>
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

    /// <summary>
    /// Called at the start of the scene, before Start() and the first frame update
    /// </summary>
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

    /// <summary>
    /// The listener for button clicks to toggle the button.
    /// Invokes the Toggled event with the current toggle state.
    /// </summary>
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

    /// <summary>
    /// Set the toggle state directly.
    /// Does not invoke the Toggled event.
    /// </summary>
    /// <param name="newState"> the state to toggle to </param>
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

    /// <summary>
    /// Returns the state of the button
    /// </summary>
    /// <returns> the state of the button </returns>
    public bool IsToggled()
    {
        return toggled;
    }
}
