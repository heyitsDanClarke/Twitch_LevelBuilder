using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwitchChatter;
using System;

public class Poll : MonoBehaviour {




	public Sprite iceIcon;
	public Sprite fireIcon;

    public static Poll Instance;

    public string _channel;

    public Text _iceCountDisplay;
    public Text _fireCountDisplay;
    public Text _weaponDisplay;


	public const string iceCommand = "#ice";
	public const string fireCommand = "#fire";

	public const string hammerCommand = "#hammer";
	public const string daggerCommand = "#dagger";
	public const string spearCommand = "#spear";

	public string hammerText = "Hammer";
	public string spearText = "Spear";
	public string knifeText = "Dagger";

	private List<string> _voterListElement;
	private List<string> _voterListWeapon;

    void Awake()
    {
		if (Instance != null) {
			Destroy (Instance);
		} else {
			Instance = this;
		}

        _voterListElement = new List<string>();
		_voterListWeapon = new List<string>();
    }

    void Start () {
		_channel = GameMaster.Instance.username;

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

	void Update() {
		float CanvasWidth = GetComponent<CanvasScaler>().referenceResolution.x;

		// make shout bubbles float
		foreach (Transform child in transform) {
			if (child.CompareTag ("Shout Bubble")) {
				Vector2 bubbleAnchoredPosition = child.GetComponent<RectTransform> ().anchoredPosition;

				// delete bubble if it is out of the screen
				if (bubbleAnchoredPosition.x < -CanvasWidth * 0.55f) {
					Destroy (child.gameObject);
				} else {
					// move bubble
					child.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (bubbleAnchoredPosition.x - Time.deltaTime * CanvasWidth / 3, bubbleAnchoredPosition.y);
				}
			}
		}
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
        if (!_voterListElement.Contains(msg.userName))
        {
            bool isValidVote = false;

            if (msg.chatMessagePlainText.Equals(iceCommand, StringComparison.InvariantCultureIgnoreCase)) {
                isValidVote = true;
                ++GameMaster.Instance.iceCount;
                _iceCountDisplay.text = "" + GameMaster.Instance.iceCount;
				createShoutBubble (iceCommand);
            } else if (msg.chatMessagePlainText.Equals(fireCommand, StringComparison.InvariantCultureIgnoreCase)) {
                isValidVote = true;
                ++GameMaster.Instance.fireCount;
                _fireCountDisplay.text = "" + GameMaster.Instance.fireCount;
				createShoutBubble (fireCommand);
            }

            if (isValidVote) {
                _voterListElement.Add(msg.userName);
            }
        }

		if (!_voterListWeapon.Contains(msg.userName))
		{
			bool isValidVote = false;

			if (msg.chatMessagePlainText.Equals(hammerCommand, StringComparison.InvariantCultureIgnoreCase)) {
				isValidVote = true;
				++GameMaster.Instance.hammerCount;
			} else if (msg.chatMessagePlainText.Equals(daggerCommand, StringComparison.InvariantCultureIgnoreCase)) {
				isValidVote = true;
				++GameMaster.Instance.daggerCount;
			} else if (msg.chatMessagePlainText.Equals(spearCommand, StringComparison.InvariantCultureIgnoreCase)) {
				isValidVote = true;
				++GameMaster.Instance.spearCount;
			}

			if (isValidVote)
			{
				_voterListWeapon.Add(msg.userName);

				determineNextWeapon (); // determine weapon display text
			}
		}
    }

	public void determineNextWeapon () {
		int hammerCount = GameMaster.Instance.hammerCount;
		int daggerCount = GameMaster.Instance.daggerCount;
		int spearCount = GameMaster.Instance.spearCount;
		if (hammerCount == daggerCount && hammerCount == spearCount) { // if all votes are equal
			// select random weapon
			_weaponDisplay.text = (UnityEngine.Random.Range (0.0f, 3.0f) < 1.0f) ? hammerText : ((UnityEngine.Random.Range (0.0f, 2.0f) < 1.0f) ? spearText : knifeText);
		} else if (hammerCount == daggerCount) {
			_weaponDisplay.text = (UnityEngine.Random.Range (0.0f, 2.0f) < 1.0f) ? hammerText : spearText;
		} else if (hammerCount == spearCount) {
			_weaponDisplay.text = (UnityEngine.Random.Range (0.0f, 2.0f) < 1.0f) ? hammerText : knifeText;
		} else if (daggerCount == spearCount) {
			_weaponDisplay.text = (UnityEngine.Random.Range (0.0f, 2.0f) < 1.0f) ? spearText : knifeText;
		} else if (hammerCount > daggerCount && hammerCount > spearCount) {
			_weaponDisplay.text = hammerText;
		} else if (daggerCount > hammerCount && daggerCount > spearCount) {
			_weaponDisplay.text = spearText;
		} else if (spearCount > hammerCount && spearCount > daggerCount) {
			_weaponDisplay.text = knifeText;
		} 
	}

	private void createShoutBubble (string command) {

		float CanvasWidth = GetComponent<CanvasScaler>().referenceResolution.x;
		float CanvasHeight = GetComponent<CanvasScaler>().referenceResolution.y;
		float bubbleSize = CanvasHeight / 7.0f; // size of shout bubble relative to screen size;

		// center coordinates (x, y) of the icon bubble
		float x = UnityEngine.Random.Range (CanvasWidth * 0.55f, CanvasWidth * 0.60f);
		float y = UnityEngine.Random.Range (-CanvasHeight * 0.22f, -CanvasHeight * 0.33f);
	
		// instantiate shout bubble
		instantiateBubble ((command == iceCommand)? iceIcon : fireIcon, x, y, bubbleSize, bubbleSize);
	}

	// create UI element based on the lowerleft coordinates (x, y), and its width and height
	private GameObject instantiateBubble(Sprite sprite, float x, float y, float width, float height)
	{
		GameObject bubble = new GameObject ();
		bubble.AddComponent<RectTransform> ();
		bubble.AddComponent<Image> ();
		bubble.GetComponent<Image> ().sprite = sprite;
		bubble.tag = "Shout Bubble";
		GameObject newUI = Instantiate (bubble);
		newUI.transform.SetParent (transform);
		newUI.GetComponent<RectTransform> ().anchoredPosition = new Vector2(x, y);
		newUI.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		newUI.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 0.5f);
		newUI.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 0.5f);
		newUI.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 0.5f);

		return newUI;
	}

	public void ResetVoteWeapon()
	{
		_weaponDisplay.text = "Next Weapon";
		determineNextWeapon (); // determine weapon display text

		GameMaster.Instance.daggerCount = 0;
		GameMaster.Instance.hammerCount = 0;
		GameMaster.Instance.spearCount = 0;

		_voterListWeapon.Clear();
	}

	public void ResetVoteElement()
	{
		_iceCountDisplay.text = "0";
		_fireCountDisplay.text = "0";

		GameMaster.Instance.iceCount = 0;
		GameMaster.Instance.fireCount = 0;

		_voterListElement.Clear();
	}

}