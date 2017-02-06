using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TwitchChatter;

// TwitchChatClient is the main interface for interacting with Twitch chat.
// 
// The basic usage pattern looks something like this:
//
//		// Add a reference to TwitchChatter at the top of your code file.
//		using TwitchChatter;
//
/*		// Then inside your class...
		void Start()
		{
			// Add a chat listener.
			TwitchChatClient.singleton.AddChatListener(OnChatMessage);

			// Set your credentials. If you're not planning on sending messages,
      		// you can remove these lines.
			TwitchChatClient.singleton.userName = "MyUserName";
			TwitchChatClient.singleton.oAuthPassword = "oauth:###################";

			// Join some channels.
			TwitchChatClient.singleton.JoinChannel("SomeChannelName");

			// If you set your credentials and you'd like to receive whispers,
			//  call EnableWhispers to allow for sending/receiving whispers.
			TwitchChatClient.singleton.EnableWhispers();

			// Then, add any whisper listeners you'd like.
			TwitchChatClient.singleton.AddWhisperListener(OnWhisper);
		}

		void Chat()
		{
			// If you set your credentials, send some messages.
			TwitchChatClient.singleton.SendMessage("SomeChannelName", "Kappa Message sent by Twitch Chatter! Kappa");
		}

		void Whisper()
		{
			// If you set your credentials and enabled whispers with EnableWhispers(), send some whispers.
			TwitchChatClient.singleton.SendWhisper("SomeUserName", "PogChamp Sending a whisper through Twitch Chatter! PogChamp");
		}

		void Cleanup()
		{
			// When you're done, leave the channels and remove the chat listeners.
			TwitchChatClient.singleton.LeaveChannel("SomeChannelName");
			TwitchChatClient.singleton.RemoveChatListener(OnChatMessage);

			// Also remove any whisper listeners you've added.
			TwitchChatClient.singleton.RemoveWhisperListener(OnWhisper);
		}

		// You'd define your chat message callback like this:
		public void OnChatMessage(ref TwitchChatMessage msg)
		{
			// Do something with the message here.
		}

		// You'd define your whisper callback like this:
		public void OnWhisper(ref TwitchChatMessage msg)
		{
			// Do something with the whisper here.
		}
*/
//
// Documentation
//
// All of the functionality you'll need to reference lives in this file.
// To jump to a certain section of the script, search for the following tags:
//   #PublicProperties
//   #SerializedProperties
//   #ChatCommands
//   #WhisperCommands
//   #MessageProperties
//
// There is also documentation for the other classes included in TwitchChatter:
//   #TwitchText
//   #TwitchEmoteImage
//   #TwitchEmoteCache

public class TwitchChatClient : MonoBehaviour
{

    //
    // Public properties
    // #PublicProperties
    //

    // userName
    //  User name to use when connecting to Twitch chat and sending messages

	//
	// Public properties
	// #PublicProperties
	//

	// userName
	//  User name to use when connecting to Twitch chat and sending messages
	public string userName
	{
		get
		{
			return _userName;
		}
		set
		{
			_userName = value;
		}
	}

	// oAuthPassword
	//  OAuth password to use when connecting to Twitch chat
	//  Visit https://twitchapps.com/tmi/ to get yours
	public string oAuthPassword
	{
		get
		{
			return _oAuthPassword;
		}
		set
		{
			_oAuthPassword = value;
		}
	}

	// autoJoinChannels
	//  These channels will be joined when this component's Awake
	//  function is called.
	public string[] autoJoinChannels
	{
		get
		{
			return _autoJoinChannels;
		}
		set
		{
			_autoJoinChannels = value;
		}
	}

	// autoEnableWhispers
	//  Enable whispers immediately upon Awake. You must specify
	//  a user name and OAuth password for whispers to function.
	public bool autoEnableWhispers
	{
		get
		{
			return _autoEnableWhispers;
		}
		set
		{
			_autoEnableWhispers = value;
		}
	}

	public bool isConnected
	{
		get
		{
			return (_internalClient != null &&
					_internalClient.isConnected);
		}
	}

	public bool isWhispersEnabled
	{
		get
		{
			return (_internalClient != null &&
					_internalClient.isWhispersEnabled);
		}
	}

	// Singleton provided for easy access to the TwitchChatClient interface.
	// NOTE: singleton may be null if this component is uninitialized or has
	//  been destroyed.
	public static TwitchChatClient singleton
	{
		get
		{
			return _singleton;
		}
	}

	//
	// Serialized properties
	// #SerializedProperties
	//

	[Tooltip("User name to use when connecting to Twitch chat and sending messages.")]
	[SerializeField] private string _userName;

	[Tooltip("OAuth password to use when connecting to Twitch chat. Visit https://twitchapps.com/tmi/ to get yours.")]
	[SerializeField] private string _oAuthPassword;

	[Tooltip("These channels will be joined when this component's Awake function is called.")]
	[SerializeField] private string[] _autoJoinChannels;

	[Tooltip("Enable whispers immediately upon Awake. You must specify a user name and OAuth password for whispers to function.")]
	[SerializeField] private bool _autoEnableWhispers = false;

	//
	// TwitchChatClient interface
	// #ChatCommands
	//

	// AddChatListener
	//  Add a callback function [func] to receive user chat messages from Twitch chatrooms you
	//  have joined. Always call RemoveChatListener when you are done.
	// The callback is defined as follows:
	//   public delegate void ChatMessageNotificationDelegate(ref TwitchChatMessage msg);
	// To define your callback, create a function as follows:
	//   void YourCallbackFunctionName(ref TwitchChatMessage msg)
	//   {
	//      /* your code here... */
	//   }
	public void AddChatListener(ChatMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.AddChatListener(func);
	}

	// RemoveChatListener
	//  Remove a callback function [func] previously added with AddChatListener.
	public void RemoveChatListener(ChatMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.RemoveChatListener(func);
	}

	// AddServerListener
	//  Add a callback function [func] to receive server messages from Twitch chatrooms you
	//  have joined. Always call RemoveServerListener when you are done.
	//  The callback [func] is defined as follows:
	//   public delegate void ServerMessageNotificationDelegate(ref TwitchChatMessage msg);
	public void AddServerListener(ServerMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.AddServerListener(func);
	}

	// RemoveServerListener
	//  Remove a callback function [func] previously added with AddServerListener.
	public void RemoveServerListener(ServerMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.RemoveServerListener(func);
	}

	// AddChannelJoinedListener
	//  Add a callback function [func] to be notified when a new channel is joined.
	//  Always call RemoveChannelJoinedListener when you are done.
	//  The callback [func] is defined as follows:
	//   public delegate void ChannelJoinedNotificationDelegate(string channelName);
	public void AddChannelJoinedListener(ChannelJoinedNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.AddChannelJoinedListener(func);
	}

	// RemoveChannelJoinedListener
	//  Remove a callback function [func] previously added with AddChannelJoinedListener.
	public void RemoveChannelJoinedListener(ChannelJoinedNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.RemoveChannelJoinedListener(func);
	}

	// JoinChannel
	//  Join a Twitch channel specified by [channelName].
	//  Once you've joined, your chat listeners will be called anytime a new
	//  chat message is received.
	//  The first time you join a channel, a connection with the Twitch chat
	//  servers will be established.
	public void JoinChannel(string channelName)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.JoinChannel(_userName, _oAuthPassword, channelName);
	}

	// LeaveChannel
	//  Leave a Twitch channel specified by [channelName].
	public void LeaveChannel(string channelName)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.LeaveChannel(channelName);
	}

	// SendMessage
	//  Send a message [msg] to a Twitch chat channel specified by [channelName].
	//  Note that messages sent by this client will not be received as chat
	//  events.
	public void SendMessage(string channelName, string msg)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.SendMessage(channelName, msg);
	}

	// #WhisperCommands
	//

	// EnableWhispers
	//  Open a connection to Twitch whisper servers to start sending and receiving whispers.
	public void EnableWhispers()
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_oAuthPassword))
		{
			Debug.LogWarning("A valid user name and OAuth password must be provided to receive whispers. Ensure the _userName and _oAuthPassword members in TwitchChatClient are initialized.", this);
		}
		else
		{
			_internalClient.EnableWhispers(_userName, _oAuthPassword);
		}
	}

	// AddWhisperListener
	//  Add a callback function [func] to receive whispers from Twitch users.
	//  Always call RemoveWhisperListener when you are done.
	//  The callback [func] is defined as follows:
	//   public delegate void WhisperMessageNotificationDelegate(ref TwitchChatMessage msg);
	public void AddWhisperListener(WhisperMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.AddWhisperListener(func);
	}	

	// RemoveWhisperListener
	//  Remove a callback function [func] previously added with AddWhisperListener.
	public void RemoveWhisperListener(WhisperMessageNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.RemoveWhisperListener(func);
	}

	// AddWhispersEnabledListener
	//  Add a callback function [func] to be notified when whispers may be sent/received.
	//  Always call RemoveWhispersEnabledListener when you are done.
	//  The callback [func] is defined as follows:
	//   public delegate void WhispersEnabledNotificationDelegate();
	public void AddWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.AddWhispersEnabledListener(func);
	}	

	// RemoveWhispersEnabledListener
	//  Remove a callback function [func] previously added with AddWhispersEnabledListener.
	public void RemoveWhispersEnabledListener(WhispersEnabledNotificationDelegate func)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.RemoveWhispersEnabledListener(func);
	}

	// SendWhisper
	//  Send a message [msg] to a Twitch user specified by [userName].
	//  Note that you must call EnableWhispers prior to sending whispers.
	public void SendWhisper(string userName, string msg)
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);
		
		_internalClient.SendWhisper(userName, msg);
	}

	// Disconnect
	//  Leave all Twitch channels, disable whispers, and close all connections
	//   to Twitch servers.
	public void Disconnect()
	{
		Debug.Assert(_internalClient != null, "TwitchChatClient is uninitialized!", this);

		_internalClient.Disconnect();
	}

	//
	// TwitchChatMessage documentation
	// #MessageProperties
	//

	// This is a breakdown of the TwitchServerMessage struct.
	/*
	struct TwitchServerMessage
	{	
		// Type of the message.
		enum TwitchServerMessageType
		{
			TwitchChatter,		// Status or debug messages from TwitchChatter
			ConnectionError,	// Error logging in to Twitch servers

			Unhandled,			// Other messages, currently unhandled by TwitchChatter
		}
		public TwitchServerMessageType type;

		// Text string received from Twitch, unmodified.
		public string rawText;
	}
	*/

	// This is a breakdown of the TwitchChatMessage struct.
	/*
	struct TwitchChatMessage
	{
		// Type of the message.
		enum TwitchChatMessageType
		{
			Server = 0,			// Messages reported to server listeners
			UserMessage = 1,	// Messages reported to chat listeners
		}
		TwitchChatMessageType type;
	
		// Name of the user that sent the message. Chat-only.
		public string userName;
	
		// Name of the channel where the message was sent.
		public string channelName;
	
		// Text string received from Twitch, unmodified.
		//	Contains additional tag information.
		public string rawText;
	
		// Text string that contains just the chat message, unmodified.
		public string chatMessagePlainText;
	
		// Text string that contains just the chat message,
		// with all Twitch emote strings removed.
		public string chatMessageMinusEmotes;
	
		// Array of data used to retrieve and insert Twitch
		// emotes into the chat message.
		public class EmoteData
		{
			// Twitch emote ID number
			//  Pass this to the TwitchEmoteCache to retrieve
			//  the corresponding Sprite from the web.
			public int id;
	
			// Character index
			//  Character index in chatMessageMinusEmotes
			//  where the emote should be inserted.
			public int index;
		}
		public EmoteData[] emoteData;
	
		// Color of the user's name
		//  Represented as a string of form #RRGGBB
		//  If no color is specified by Twitch, a random
		//  color is selected form the Twitch color pool.
		public string userNameColor;
	
		// Is this message from a channel moderator?
		public bool isMod;
	
		// Is this message from a channel subscriber?
		public bool isSubscriber;
	
		// Is this message from a Twitch turbo user?
		public bool isTurbo;
	}
	*/

	#region Internal

	// This code is used to communicate with the internal Twitch chat interface.
	private static TwitchChatClient _singleton;

	private TwitchChatClientInternal _internalClient;

	private void Awake()
	{
		Debug.Assert(_singleton == null, "Attempting to create multiple instances of TwitchChatter.TwitchChatClient! Only one instance can exist at any time.");

		_singleton = this;

		_internalClient = TwitchChatClientInternal.singleton;
		_internalClient.Awake();

		if (_autoJoinChannels != null &&
			_autoJoinChannels.Length > 0)
		{
			foreach (string channelName in _autoJoinChannels)
			{
				JoinChannel(channelName);
			}
		}

		if (_autoEnableWhispers &&
			!string.IsNullOrEmpty(_userName) &&
			!string.IsNullOrEmpty(_oAuthPassword))
		{
			EnableWhispers();
		}

		//_internalClient.printVerboseDebugInfo = true;
	}

	private void Update()
	{
		if (_internalClient != null)
		{
			_internalClient.Update();
		}
	}

	private void OnDestroy()
	{
		if (_internalClient != null)
		{
			_internalClient.OnDestroy();

			_internalClient = null;
		}

		_singleton = null;
	}

	#endregion
}

// TwitchChatter.TwitchText
// #TwitchText

// UI Text component that can be used to render Twitch emoticons inside
//  of Twitch chat text.
/*
class TwitchText : Text
{
	// Message stream to listen to
	enum ListenMode
	{
		Chat = 0,		// Render messages from chat rooms
		Whisper = 1,	// Render whispers from users
		Custom = 2,		// Render messages supplied manually to OnCustomMessage
	}
	ListenMode listenMode;

	// (ListenMode.Chat only)
	// Name of the Twitch channel chat to render
	//  If blank, chat messages from all channels will be rendered.
	string channelName;

	// (ListenMode.Whisper only)
	// Name of the Twitch user's whispers to render.
	//  If blank, whispers from all users will be rendered.
	string userName;

	// Mode of operation
	enum TextMode
	{
		PlainText = 0,		// Draw text only, no emoticons.
		WithEmotes = 1,		// Draw text with embedded emoticons.
	}
	TextMode textMode;

	// Custom scale factor for Twitch emoticons
	float emoticonScaleFactor;

	// Maximum number of characters to render
	//  Older messages will be removed.
	int maxCharacterCount;

	// Use this to specify your own messages instead of listening
	//  to Twitch streams directly. Requires ListenMode.Custom.
	public void OnCustomMessage(ref TwitchChatMessage msg);
	
	// Add/remove listeners to provide special initialization for emote images.
	//  Can be used to supply custom materials to emote images when they are added.
	public void AddEmoteImageInitializationListener(TwitchEmoteImageInitializationCallback callback);	
	public void RemoveEmoteImageInitializationListener(TwitchEmoteImageInitializationCallback callback);

	// Callback definition for the previous listener functions
	public delegate void TwitchEmoteImageInitializationCallback(TwitchEmoteImage image);
}
*/

// TwitchChatter.TwitchEmoteImage
// #TwitchEmoteImage

// Enum used to specify different sizes of emote textures when
//  using the TwitchEmoteCache manually.
/*
enum EmoteSize
{
	Standard,	// This is the standard texture size used in TwitchText
	Medium,
	Large,
}
*/

// UI Image component that can be used to easily render Twitch emoticons.
/*
class TwitchEmoteImage : Image
{
	// ID number for the Twitch emoticon to show
	//  These are provided by Twitch chat messages. The values
	//  are static, so they can be set directly with consistent
	//  results.
	int emoteID;

	// Size of the texture retrieved from Twitch servers.
	EmoteSize size;

	// Scale multiplier that respects aspect ratio.
	float scaleFactor;
}
*/

// TwitchChatter.TwitchEmoteCache
// #TwitchEmoteCache

// Utility component that can retrieve Twitch emoticon textures from the
//  web by their ID number and create sprites from them. It will cache all
//  textures downloaded and sprites generated in this way so that subsequent
//  retrievals use the same Sprite from local memory.
/*
class TwitchEmoteCache : MonoBehaviour
{
	// Definition of callback functions should mimic this delegate.
	public delegate void OnLoadCallback(Sprite sprite);

	// Retrieves a sprite defined by [emoteID] and calls [callback] on completion
	//  The first request for an emoteID will download the emoticon texture
	//  from Twitch's static resources and create a new Sprite. Subsequent
	//  requests will immediately return a reference to the same Sprite.
	//  Returns a loading icon on first request (specified in the inspector).
	//  If the emote ID is invalid, subsequent requests will return an invalid
	//  icon (specified in the inspector).
	// Returns EmoteSize.Standard texture by default
	static Sprite GetSpriteForEmoteID(int emoteID, OnLoadCallback callback = null);

	// Same as above with an added EmoteSize parameter
	static Sprite GetSpriteForEmoteID(int emoteID, EmoteSize size, OnLoadCallback callback = null);

	// Releases all memory associated with the cache
	//  Note that this will invalidate any sprites that
	//  hold references to the cached data that still
	//  live in the active scene.
	static void Clear();
}
*/