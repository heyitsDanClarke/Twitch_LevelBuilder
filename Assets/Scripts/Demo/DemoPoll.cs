using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwitchChatter;
using System;

public class DemoPoll : MonoBehaviour {

    public static DemoPoll Instance;

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

            if(msg.chatMessagePlainText.Equals("#ice", StringComparison.InvariantCultureIgnoreCase))

            {
                isValidVote = true;

                ++GameMaster.Instance.iceCount;//++_iceCount;

                _iceCountDisplay.text = "" + GameMaster.Instance.iceCount; 
            }

            else if (msg.chatMessagePlainText.Equals("#fire", StringComparison.InvariantCultureIgnoreCase))

            {
                isValidVote = true;

                ++GameMaster.Instance.fireCount;

                _fireCountDisplay.text = "" + GameMaster.Instance.fireCount; 
            }

            if (isValidVote)
            {
                _voterList.Add(msg.userName);
            }
        }
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