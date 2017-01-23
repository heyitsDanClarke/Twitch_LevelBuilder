using System.Collections.Generic;

using UnityEngine;

using TwitchChatter;

[RequireComponent(typeof(TwitchChatClient))]
public class TwitchChatDebugLogger : MonoBehaviour
{
	public bool printChatMessages;
	public bool printServerMessages;
	public bool printWhispers;

	private void Start()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);
			TwitchChatClient.singleton.AddServerListener(OnServerMessage);
			TwitchChatClient.singleton.AddWhisperListener(OnWhisper);
		}
	}

	private void OnDestroy()
	{
		if (TwitchChatClient.singleton != null)
		{
			TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);
			TwitchChatClient.singleton.RemoveServerListener(OnServerMessage);
			TwitchChatClient.singleton.RemoveWhisperListener(OnWhisper);
		}
	}

	private void OnChatMessage(ref TwitchChatMessage msg)
	{
		if (printChatMessages)
		{
			Debug.Log("TwitchChatter: " + msg.userName + "(" + msg.channelName + "): " + msg.chatMessagePlainText);
		}
	}

	private void OnServerMessage(ref TwitchServerMessage msg)
	{
		if (printServerMessages)
		{
			Debug.Log("TwitchChatter: " + msg.rawText);
		}
	}

	private void OnWhisper(ref TwitchChatMessage msg)
	{
		if (printWhispers)
		{
			Debug.Log("TwitchChatter: " + msg.userName + "(whisper): " + msg.chatMessagePlainText);
		}
	}
}