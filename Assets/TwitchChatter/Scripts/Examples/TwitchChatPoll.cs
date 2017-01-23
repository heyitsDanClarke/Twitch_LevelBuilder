using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TwitchChatter;

public class TwitchChatPoll : MonoBehaviour
{
	// Name of the Twitch channel to join for the poll
	public string _pollChannelName;

	// Keywords used to indicate the poll options
	// Chat messages will be compared to these strings
	// to see if a user has cast a vote.
	public InputField _optionOneLabel;
	public InputField _optionTwoLabel;
	public InputField _optionThreeLabel;

	// Label used to display the vote totals for each option
	public Text _optionOneCountLabel;
	public Text _optionTwoCountLabel;
	public Text _optionThreeCountLabel;

	// Internal counts for each option
	private int _optionOneCount;
	private int _optionTwoCount;
	private int _optionThreeCount;

	// List of names of users that have cast a vote
	// This is used to ensure each user can only
	// cast one vote.
	private List<string> _voterList;

	private void Awake()
	{
		_voterList = new List<string>();
	}

	private void Start()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);
		}

		if (!string.IsNullOrEmpty(_pollChannelName))
		{
			TwitchChatClient.singleton.JoinChannel(_pollChannelName);
		}
		else
		{
			Debug.LogWarning("No channel name entered for poll! Enter a channel name and restart the scene.", this);
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
		if (!_voterList.Contains(msg.userName))
		{
			bool isValidVote = false;

			if (msg.chatMessagePlainText.Equals("#1"))
			{
				isValidVote = true;

				++_optionOneCount;

				_optionOneCountLabel.text = "" + _optionOneCount;
			}
			else if (msg.chatMessagePlainText.Equals("#2"))
			{
				isValidVote = true;

				++_optionTwoCount;

				_optionTwoCountLabel.text = "" + _optionTwoCount;
			}
			else if (msg.chatMessagePlainText.Equals("#3"))
			{
				isValidVote = true;

				++_optionThreeCount;

				_optionThreeCountLabel.text = "" + _optionThreeCount;
			}

			if (isValidVote)
			{
				_voterList.Add(msg.userName);
			}
		}
	}

	public void OnResetButtonPressed()
	{
		_optionOneLabel.text = "";
		_optionTwoLabel.text = "";
		_optionThreeLabel.text = "";

		_optionOneCount = 0;
		_optionTwoCount = 0;
		_optionThreeCount = 0;

		_optionOneCountLabel.text = "0";
		_optionTwoCountLabel.text = "0";
		_optionThreeCountLabel.text = "0";

		_voterList.Clear();
	}
}