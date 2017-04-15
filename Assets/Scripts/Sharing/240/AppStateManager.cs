using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

/// <summary>
/// Keeps track of which state of the experience we are currently in.
/// </summary>
public class AppStateManager : Singleton<AppStateManager>
{
    /// <summary>
    /// Enum to track progress through the experience.
    /// </summary>
    public enum AppState
    {
        Starting = 0,
        PickingAvatar,
        WaitingForAnchor,
        WaitingForStageTransform,
        Ready
    }

    // The object to call to make a projectile.
    GameObject shootHandler = null;

    /// <summary>
    /// Tracks the current state ID in the experience.
    /// </summary>
    public AppState CurrentAppState { get; set; }
    private AppState previousAppState { get; set; }  // for debugging purposes

    void Start()
    {
        // The shootHandler shoots projectiles for this player.
        // Associated component should be attached to same gameObject as this component.
        if (GetComponent<ProjectileLauncher>() != null)
        {
            shootHandler = GetComponent<ProjectileLauncher>().gameObject;
        }
        else
        {
            Debug.LogFormat("{0}: {1}: Start(): no ProjectileLauncher component detected", gameObject.name, this.GetType().Name);
        }


        // We start in the 'picking avatar' mode.
        previousAppState = AppState.Starting;
        CurrentAppState = AppState.PickingAvatar;

        // Spatial mapping should be disabled when we start up so as not
        // to distract from the avatar picking.
        SpatialMappingManager.Instance.StopObserver();
        SpatialMappingManager.Instance.gameObject.SetActive(false);

        // On device we start by showing the avatar picker.
        PlayerAvatarStore.Instance.SpawnAvatarPicker();
    }

    /// <summary>
    /// Sets app state back to the WaitingForAnchor state
    /// </summary>
    public void ResetStage()
    {
        // If we fall back to waiting for anchor, everything needed to 
        // get us into setting the target transform state will be setup.
        if (CurrentAppState != AppState.PickingAvatar)
        {
            previousAppState = CurrentAppState;
            CurrentAppState = AppState.WaitingForAnchor;
        }

        // Reset the underworld.
        //if (UnderworldBase.Instance)
        //{
        //    UnderworldBase.Instance.ResetUnderworld();
        //}
    }

    /// <summary>
    /// Sets up different states of the experience when app is in that state
    /// </summary>
    void Update()
    {
        // log app state changes
        if (previousAppState != CurrentAppState)
        {
            Debug.LogFormat("{0}: {1}: currentAppState = {2}", gameObject.name, this.GetType().Name, CurrentAppState);
            previousAppState = CurrentAppState;
        }

        switch (CurrentAppState)
        {
            case AppState.PickingAvatar:
                // Avatar picking is done when the avatar picker has been dismissed.
                if (PlayerAvatarStore.Instance.PickerActive == false)
                {
                    previousAppState = CurrentAppState;
                    CurrentAppState = AppState.WaitingForAnchor;
                }
                break;
            case AppState.WaitingForAnchor:
                // Once the anchor is established we need to run spatial mapping for a 
                // little while to build up some meshes.
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    Debug.LogFormat("{0}: {1}: attempting to activate spatial mapping", gameObject.name, this.GetType().Name);
                    previousAppState = CurrentAppState;
                    CurrentAppState = AppState.WaitingForStageTransform;
                    GestureManager_Custom.Instance.OverrideFocusedObject = HologramPlacement.Instance.gameObject;

                    SpatialMappingManager.Instance.gameObject.SetActive(true);
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                    SpatialMappingDeformation.Instance.ResetGlobalRendering();
                    SpatialMappingManager.Instance.StartObserver();
                    Debug.LogFormat("{0}: {1}: spatial mapping activated", gameObject.name, this.GetType().Name);
                }
                break;
            case AppState.WaitingForStageTransform:
                // Now if we have the stage transform we are ready to go.
                if (HologramPlacement.Instance.GotTransform)
                {
                    previousAppState = CurrentAppState;
                    CurrentAppState = AppState.Ready;
                    // At this point, all air taps are sent to shoothandler rather
                    // than the object 'actually' in focus.
                    GestureManager_Custom.Instance.OverrideFocusedObject = shootHandler;
                }
                break;
        }
    }
}