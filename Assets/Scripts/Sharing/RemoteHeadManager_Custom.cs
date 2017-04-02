// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Broadcasts the head transform of the local user to other users in the session,
    /// and adds and updates the head transforms of remote users for local tracking.
    /// Head transforms are sent and received in the local coordinate space of the GameObject 
    /// this component is on.
    /// </summary>
    public class RemoteHeadManager : Singleton<RemoteHeadManager>
    {
        public class RemoteHeadInfo
        {
            public long UserID;
            public GameObject HeadObject;
        }

        /// <summary>
        /// Maintains a list of the remote heads, indexed by XTools userID
        /// </summary>
        private Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();

        /// <summary>
        /// Assign hooks for CustomMesssages and events
        /// </summary>
        private void Start()
        {
            // receive headtransform update messages
            CustomMessages.Instance.MessageHandlers[CustomMessages.MessageID.HeadTransform] = UpdateHeadTransform_RemoteHeadManager;

            // SharingStage should be valid at this point.
            SharingStage.Instance.SharingManagerConnected += SharingManagerConnected_RemoteHeadManager;
        }

        /// <summary>
        /// Single-use event handler used for assigning SharingStage event hooks
        /// once the SharingStage manager is conected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SharingManagerConnected_RemoteHeadManager(object sender, EventArgs e)
        {
            // unhook this event handler
            SharingStage.Instance.SharingManagerConnected -= SharingManagerConnected_RemoteHeadManager;
            // add hooks for SharingStage joining/leaving events 
            SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession_RemoteHeadManager;
            SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession_RemoteHeadManager;
        }

        /// <summary>
        /// Grab the current head transform of local user and broadcast it 
        /// to all the other users in the session
        /// </summary>
        private void Update()
        {
            // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation from world space into local space
            Vector3 headPosition = transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(transform.rotation) * headTransform.rotation;

            // broadcast transform info
            CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);
        }

        /// <summary>
        /// Called when a new user is leaving the current session.
        /// </summary>
        /// <param name="user">User that left the current session.</param>
        private void UserLeftSession_RemoteHeadManager(User user)
        {
            // check that the user is not the local user themselves
            int userId = user.GetID();
            if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                RemoveRemoteHead(remoteHeads[userId].HeadObject);
                remoteHeads.Remove(userId);
            }
        }

        /// <summary>
        /// Called when a user is joining the current session.
        /// </summary>
        /// <param name="user">User that joined the current session.</param>
        private void UserJoinedSession_RemoteHeadManager(User user)
        {
            // check that the user is not just the local user themselves
            if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                GetRemoteHeadInfo(user.GetID());
            }
        }

        /// <summary>
        /// Called when a remote user sends a head transform.
        /// Updates the user's local remote head position.
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateHeadTransform_RemoteHeadManager(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();

            Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);
            Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
            headInfo.HeadObject.transform.localPosition = headPos;
            headInfo.HeadObject.transform.localRotation = headRot;
        }

        /// <summary>
        /// Gets the data structure for the remote users' head position
        /// from this component's list of remoteHeads.
        /// </summary>
        /// <param name="userId">User ID for which the remote head info should be obtained.</param>
        /// <returns>RemoteHeadInfo for the specified user.</returns>
        public RemoteHeadInfo GetRemoteHeadInfo(long userId)
        {
            RemoteHeadInfo headInfo;

            // Get the head info if its already in the list of remote heads, otherwise add it
            if (!remoteHeads.TryGetValue(userId, out headInfo))
            {
                headInfo = new RemoteHeadInfo();
                headInfo.UserID = userId;
                headInfo.HeadObject = CreateRemoteHead();

                remoteHeads.Add(userId, headInfo);
            }

            return headInfo;
        }

        /// <summary>
        /// Creates a new game object to represent the user's head.
        /// </summary>
        /// <returns></returns>
        private GameObject CreateRemoteHead()
        {
            GameObject newHeadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newHeadObj.transform.parent = gameObject.transform;
            newHeadObj.transform.localScale = Vector3.one * 0.2f;
            return newHeadObj;
        }

        /// <summary>
        /// When a user has left the session this will cleanup their
        /// head data.
        /// </summary>
        /// <param name="remoteHeadObject"></param>
        private void RemoveRemoteHead(GameObject remoteHeadObject)
        {
            DestroyImmediate(remoteHeadObject);
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                if (SharingStage.Instance.SessionUsersTracker != null)
                {
                    SharingStage.Instance.SessionUsersTracker.UserJoined -= UserJoinedSession_RemoteHeadManager;
                    SharingStage.Instance.SessionUsersTracker.UserLeft -= UserLeftSession_RemoteHeadManager;
                }
            }

            base.OnDestroy();
        }
    }

}