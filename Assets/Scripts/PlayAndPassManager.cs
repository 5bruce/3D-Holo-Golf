using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates and manages players in a play-and-pass game
/// </summary>
public class PlayAndPassManager : MonoBehaviour {

    /// <summary>
    /// Prefab or gameobject to use as tempate for generating players
    /// </summary>
    public GameObject firstPlayer;

    SettingsManager settingsAccess;
    int numberOfPlayers;
    List<GameObject> players = new List<GameObject>();
    Material[] playerColors;
    GameObject activePlayer;

	// Use this for initialization
	void Start () {
        if (!firstPlayer)
        {
            Debug.LogError(this.name + ": no player creation prefab assigned");
        }
        else
        {
            // ensure that the firstPlayer is active and first to play
            if (firstPlayer.activeSelf == false)
            {
                firstPlayer.SetActive(true);
            }
            activePlayer = firstPlayer;

            // get access to settings information
            settingsAccess = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();

            // get number of player for this game
            numberOfPlayers = settingsAccess.numberOfPlayers;
            Debug.Log(gameObject.name + ": " + this.GetType().Name + ": numberOfPlayers = " + numberOfPlayers);

            // get player colors
            playerColors = settingsAccess.playerColors;

            // add the first player to players list (there should always be a first player)
            //players.Add(firstPlayer);

            // create list of other Player gameobject children and instantiate them
            Debug.Log(this.name + ": creating players");
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject player;
                // 
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
                player.transform.FindChild("FlagPole").transform.FindChild("Flag").GetComponent<MeshRenderer>().material = playerColors[i];
                player.transform.FindChild("Projectile").GetComponent<TrailRenderer>().endColor = playerColors[i].color;
                player.transform.FindChild("Projectile").GetComponent<TrailRenderer>().endColor = playerColors[i].color;
                player.transform.FindChild("Projectile").GetComponent<Light>().color = playerColors[i].color;

                // initially, hide all other player balls so first toss does not toss all
                if (player != firstPlayer) {
                    player.SetActive(false);
                }

                Debug.Log(this.name + ": " + firstPlayer.name + "clone created: " + player.name);
                players.Add(player); 
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // TODO: convert from using SetActive() to broadcasting Activate/Deactivate to children
    void ChangeActivePlayer ()
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

        // enable this player object if first time being activated
        if (!activePlayer.activeSelf)
        {
            activePlayer.SetActive(true);
        }
        activePlayer.BroadcastMessage("Activate");
        activePlayer.transform.FindChild("Canvas").gameObject.SetActive(true);
    }
}
