using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates and manages players in a play-and-pass game
/// </summary>
public class PlayAndPassManager : Singleton<PlayAndPassManager> {

    /// <summary>
    /// Prefab or gameobject to use as tempate for generating players
    /// </summary>
    public GameObject firstPlayer;

    SettingsManagerLoader settingsAccess;
    int numberOfPlayers;
    List<GameObject> players = new List<GameObject>();
    Material[] playerColors;
    GameObject activePlayer;

	// Use this for initialization
	void Start () {
        if (!firstPlayer)
        {
            Debug.LogError(this.name + ": no player creation prefab/tempate assigned");
            Application.Quit();
        }
        else
        {
            // ensure that the firstPlayer is active and first to play
            if (firstPlayer.activeSelf == false)
            {
                firstPlayer.SetActive(true);
            }
            activePlayer = firstPlayer;

            // try to access the SettignsManagerLoader singleton
            settingsAccess = SettingsManagerLoader.Instance;
            if (!settingsAccess)
            {
                Debug.LogError(gameObject.name + ": " + this.GetType().Name + 
                    ": could not load a SettingsManagerLoader instance");
            }

            // get number of player for this game
            numberOfPlayers = settingsAccess.numberOfPlayers;
            Debug.Log(gameObject.name + ": " + this.GetType().Name + 
                ": numberOfPlayers = " + numberOfPlayers);

            // get player colors
            playerColors = settingsAccess.playerColors;

            // add the first player to players list (there should always be a first player)
            //players.Add(firstPlayer);

            // create list of other Player gameobject children and instantiate them
            Debug.Log(this.name + ": creating players");
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject player;
                // certain setup steps can be skipped for the 1st player (since is the expected template for the others)
                if (i != 0) {
                    player = Instantiate(firstPlayer, firstPlayer.transform.position, firstPlayer.transform.rotation, gameObject.transform);
                    player.name = "Player" + (1 + i);
                    player.BroadcastMessage("Deactivate");
                } else
                {
                    player = firstPlayer;
                }

                // set this player's colors
                player.transform.FindChild("Projectile").GetComponent<MeshRenderer>().material = playerColors[i];
                //player.transform.FindChild("Projectile").GetComponent<DirectionIndicator>().DirectionIndicatorColor = playerColors[i].color;
                player.transform.FindChild("FlagPole").transform.FindChild("Flag").GetComponent<MeshRenderer>().material = playerColors[i];
                player.transform.FindChild("Projectile").GetComponent<TrailRenderer>().endColor = playerColors[i].color;
                player.transform.FindChild("Projectile").GetComponent<TrailRenderer>().endColor = playerColors[i].color;
                player.transform.FindChild("Projectile").GetComponent<Light>().color = playerColors[i].color;

                // initially, hide all other player balls so first toss does not toss all
                if (player != firstPlayer) {
                    Debug.Log(this.name + ": " + firstPlayer.name + " clone created: " + player.name);
                    player.SetActive(false);
                }

                players.Add(player); 
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void ChangeActivePlayer ()
    {
        // safely deactivate current active player
        /*
         Deactivated players should have only thier ball visible.
         The ball should be able to roll around and continue to place its flag,
         but is otherwise not interactable (no response to gestures or voice).
         */
        activePlayer.BroadcastMessage("Deactivate");
        activePlayer.transform.FindChild("Canvas").gameObject.SetActive(false);

        // cycle thru players in round-robin schedule
        activePlayer = (players.IndexOf(activePlayer) + 1 < numberOfPlayers)
                        ? players[players.IndexOf(activePlayer) + 1]
                        : players[0];
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": activePlayer: " + activePlayer.name);

        // enable this player object if not in obstacle selection round (eg. if first time being activated or start of new round)
        // generally ok cost if only for setting activate and player objects have few child transforms
        activePlayer.SetActive(true);
        if (!ObjectSelectionHandler.Instance.gameObject.activeSelf) {
            foreach (Transform child in activePlayer.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log(this.GetType().Name +
                    ": settting active: " + activePlayer.name + ": " + child.gameObject.name);
            }
        }
        activePlayer.BroadcastMessage("Activate");
        activePlayer.transform.FindChild("Canvas").gameObject.SetActive(true);
    }

    public void setFirstPlayerActive()
    {
        activePlayer.BroadcastMessage("Deactivate");
        activePlayer.transform.FindChild("Canvas").gameObject.SetActive(false);

        // set firstPlayer active
        activePlayer = firstPlayer;
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": activePlayer: " + activePlayer.name);

        // enable this player object if not in obstacle selection round (eg. if first time being activated or start of new round)
        // generally ok cost if only for setting activate and player objects have few child transforms
        activePlayer.SetActive(true);
        if (!ObjectSelectionHandler.Instance.gameObject.activeSelf)
        {
            foreach (Transform child in activePlayer.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log(this.GetType().Name +
                    ": settting active: " + activePlayer.name + ": " + child.gameObject.name);
            }
        }
        activePlayer.BroadcastMessage("Activate");
        activePlayer.transform.FindChild("Canvas").gameObject.SetActive(true);
    }
}
