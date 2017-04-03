using System;
using UnityEngine;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Used to demonstrate how to get notifications when users leave and enter room.
    /// </summary>
    public class UserNotifications_Custom : MonoBehaviour
    {
        private SessionUsersTracker usersTracker;

        private void Start()
        {
            // SharingStage should be valid at this point.
            SharingStage.Instance.SharingManagerConnected += Connected;
        }

        /// <summary>
        /// Single-use event hook to hook callbacks for a SharingStage SessionUsersTracker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connected(object sender, EventArgs e)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            usersTracker = SharingStage.Instance.SessionUsersTracker;

            Debug.LogFormat("{0}: {1} users in room.", this.GetType().Name, usersTracker.CurrentUsers.Count);

            foreach (User currentUser in usersTracker.CurrentUsers)
            {
                Debug.LogFormat(currentUser.GetName());
            }

            usersTracker.UserJoined += NotifyUserJoined;
            usersTracker.UserLeft += NotifyUserLeft;
        }

        private static void NotifyUserJoined(User user)
        {
            Debug.LogFormat("{0}: User {1} has joined the room.", typeof(UserNotifications_Custom).Name, user.GetName());
        }

        private static void NotifyUserLeft(User user)
        {
            Debug.LogFormat("{0}: User {1} has left the room.", typeof(UserNotifications_Custom).Name, user.GetName());
        }

        private void OnDestroy()
        {
            if (usersTracker != null)
            {
                usersTracker.UserJoined -= NotifyUserJoined;
                usersTracker.UserLeft -= NotifyUserLeft;
            }
            usersTracker = null;
        }
    }
}
