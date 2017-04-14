using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Script to handle the user selecting an avatar.
/// </summary>
public class AvatarSelector : MonoBehaviour, IInputClickHandler
{
    /// <summary>
    /// This is the index set by the PlayerAvatarStore for the avatar.
    /// </summary>
    public int AvatarIndex { get; set; }

    /*
    /// <summary>
    /// Called when the user is gazing at this avatar and air taps it.
    /// This sends the user's selection to the rest of the devices in the experience.
    /// </summary>
    void OnSelect()
    {
        Debug.LogFormat("{0}: {1}: OnSelect()", gameObject.name, this.GetType().Name);
        PlayerAvatarStore.Instance.DismissAvatarPicker();

        LocalPlayerManager.Instance.SetUserAvatar(AvatarIndex);
    }
    */
    /// <summary>
    /// Called when the user is gazing at this avatar and air taps it.
    /// This sends the user's selection to the rest of the devices in the experience.
    /// </summary>
    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        Debug.LogFormat("{0}: {1}: OnSelect()", gameObject.name, this.GetType().Name);

        PlayerAvatarStore.Instance.DismissAvatarPicker();
        LocalPlayerManager.Instance.SetUserAvatar(AvatarIndex);
    }


        void Start()
    {
        // Add Billboard component so the avatars always faces the user.
        Billboard billboard = gameObject.GetComponent<Billboard>();
        if (billboard == null)
        {
            billboard = gameObject.AddComponent<Billboard>();
        }

        // Lock rotation along the Y axis.
        billboard.PivotAxis = PivotAxis.Y;
    }
}