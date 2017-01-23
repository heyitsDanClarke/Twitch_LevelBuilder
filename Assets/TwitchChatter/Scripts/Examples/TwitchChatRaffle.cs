using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TwitchChatter;

public class TwitchChatRaffle : MonoBehaviour
{
	// Name of the Twitch channel to join for the raffle
	public string _raffleChannelName;

	// Text field with the raffle keyword
	public Text _keywordLabel;

	// Label on the button to start/stop the raffle
	public Text _buttonLabel;

	// Text label used to communicate extra info
	public Text _output;

	// List of users entered into the raffle
	private List<string> _raffleEntrants;

	// Has the raffle started?
	private bool _isRaffleStarted;

	private void Awake()
	{
		_raffleEntrants = new List<string>();
	}

	private void Start()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);
		}

		if (!string.IsNullOrEmpty(_raffleChannelName))
		{
			TwitchChatClient.singleton.JoinChannel(_raffleChannelName);
		}
		else
		{
			Debug.LogWarning("No channel name entered for raffle! Enter a channel name and restart the scene.", this);
		}
	}

	private void OnDestroy()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
		}
	}

	public void OnButtonPress()
	{
		if (_isRaffleStarted)
		{
			_isRaffleStarted = false;

			if (_raffleEntrants.Count > 0)
			{
				// pick a winner
				_output.text = _raffleEntrants[Random.Range(0, _raffleEntrants.Count)] + " wins!";
			}
			else
			{
				_output.text = "No winner!";
			}

			_buttonLabel.text = "Start raffle!";
		}
		else
		{
			_isRaffleStarted = true;
			_raffleEntrants.Clear();

			_buttonLabel.text = "Pick winner!";
		}
	}

	private void Update()
	{
		if (_isRaffleStarted)
		{
			// Display entrant count
			_output.text = "" + _raffleEntrants.Count + " entrants!";
		}
	}

	private void OnChatMessage(ref TwitchChatMessage msg)
	{
		if (_isRaffleStarted)
		{
			// don't worry about case-sensitivity
			if (msg.chatMessagePlainText.ToLower().Equals(_keywordLabel.text.ToLower()) &&
				// don't allow multiple entries from the same user
				!_raffleEntrants.Contains(msg.userName))
			{
				_raffleEntrants.Add(msg.userName);
			}
		}
	}
}