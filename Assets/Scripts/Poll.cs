using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwitchChatter;
using System;

public class Poll : MonoBehaviour {

	public GameObject shoutBubble;

    public static Poll Instance;

    public string _channel;

    public Text _iceCountDisplay;
    public Text _fireCountDisplay;

    private List<string> _voterList;

    void Awake()
    {
		if (Instance != null) {
			Destroy (Instance);
		} else {
			Instance = this;
		}

        _voterList = new List<string>();
    }

    void Start () {
        if (TwitchChatClient.singleton != null)
        {
            TwitchChatClient.singleton.AddChatListener(OnChatMessage);
        }

        if (!string.IsNullOrEmpty(_channel))
        {
            TwitchChatClient.singleton.JoinChannel(_channel);
        }
        else
        {
            Debug.LogWarning("No channel name entered for poll! Enter a channel name and restart the scene.", this);
        }
			
    }

	void FixedUpdate() {

	}
		
    private void OnDestroy()
    {
        if (TwitchChatClient.singleton != null)
        {
            TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
        }
    }

    private void OnChatMessage(ref TwitchChatMessage msg)
    {
        if (!_voterList.Contains(msg.userName))
        {
            bool isValidVote = false;

			string iceCommand = "#ice";
			string fireCommand = "#fire";

			if(msg.chatMessagePlainText.Equals(iceCommand, StringComparison.InvariantCultureIgnoreCase)) {
                isValidVote = true;

                ++GameMaster.Instance.iceCount;//++_iceCount;

                _iceCountDisplay.text = "" + GameMaster.Instance.iceCount;

				createShoutBubble (msg.userName, iceCommand);
            }

			else if (msg.chatMessagePlainText.Equals(fireCommand, StringComparison.InvariantCultureIgnoreCase)) {
                isValidVote = true;

                ++GameMaster.Instance.fireCount;

                _fireCountDisplay.text = "" + GameMaster.Instance.fireCount;

				createShoutBubble (msg.userName, fireCommand);
            }

            if (isValidVote)
            {
                _voterList.Add(msg.userName);
            }
        }
    }

	private void createShoutBubble (string username, string command) {

		float bubbleSize = 0.2f; // size of shout bubble relative to screen size;

		float x = 0.0f, y = 0.0f; // lowerleft coordinates (x, y) of the shoutbubble

		// determine lowerleft coordinates (x, y) of the shoutbubble randomly
		float tempRandom = UnityEngine.Random.Range (0.0f, (Screen.width + Screen.height) * (1 - bubbleSize)); // random variable for determining the position of the shout bubble
		if (UnityEngine.Random.Range (0.0f, 1.0f) < 0.5f) {
			x = Math.Min (tempRandom, (Screen.width) * (1 - bubbleSize));
			y = Math.Max (tempRandom - (Screen.width) * (1 - bubbleSize), 0.0f);
		} else {
			y = Math.Min (tempRandom, (Screen.height) * (1 - bubbleSize));
			x = Math.Max (tempRandom - (Screen.height) * (1 - bubbleSize), 0.0f);
		}

		// instantiate shout bubble
		GameObject newShoutBubble = instantiateUI (shoutBubble, x, y, Screen.width * bubbleSize, Screen.height * bubbleSize);
		newShoutBubble.GetComponent<Image> ().color = (command == "#fire")? new Color (1.0f, 0.5f, 0.5f): new Color (0.5f, 0.5f, 1.0f);
		newShoutBubble.transform.GetChild(0).GetComponent<Text>().text = username;
		newShoutBubble.transform.GetChild(1).GetComponent<Text>().text = (command == "#fire")? "FIRE!" : "ICE!";
		Destroy (newShoutBubble, 3.0f);
	}

	// create UI element based on the lowerleft coordinates (x, y), and its width and height
	private GameObject instantiateUI(GameObject child, float x, float y, float width, float height)
	{
		GameObject newUI = Instantiate (child);
		newUI.transform.SetParent (transform);
		newUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2(0, 0);
		newUI.GetComponent<RectTransform> ().offsetMax = new Vector2 (x + width, y + height);
		newUI.GetComponent<RectTransform> ().offsetMin = new Vector2 (x, y);
		newUI.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.0f, 0.0f);
		newUI.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.0f, 0.0f);
		newUI.GetComponent<RectTransform> ().pivot = new Vector2 (0.0f, 0.0f);

		return newUI;
	}

	public void ResetVote()
	{
		_iceCountDisplay.text = "0";
		_fireCountDisplay.text = "0";

		GameMaster.Instance.iceCount = 0;
		GameMaster.Instance.fireCount = 0;

		_voterList.Clear();
	}

}