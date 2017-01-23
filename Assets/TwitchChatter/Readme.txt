Welcome to Twitch Chatter!

For questions, comments, and requests, please email us at contact@thedragonloft.com or reach us on Twitter @TheDragonloft. We also have a Discord chat here: https://discordapp.com/invite/MYq8U

Getting started:
  To get started with Twitch Chatter, just drag the TwitchChatter prefab into your scene and type a channel name into the auto-join channels list. Press Play and Twitch chat messages will start appearing!

Features:
  You can perform the following operations programmatically once you have a TwitchChatClient component running in your scene:
   - Join Twitch channels
   - Leave Twitch channels
   - Send chat messages
   - Receive chat messages, including user name and color, moderator status, subscriber status, turbo status, as well as emoticon data.
   - Send/receive whispers

  Twitch Chatter also comes with two UI components, TwitchText and TwitchEmoteImage.
   - TwitchText can be used to draw chat messages from Twitch with Twitch emoticons embedded in the text.
   - TwitchEmoteImage is a specialized Image that can be used to easily draw emoticon sprites retrieved from Twitch.

  There is also a TwitchEmoteCache component that can retrieve Twitch emoticons from the internet and create sprites from them. It will cache all textures downloaded in this way so that subsequent retrievals use the same Sprite from local memory.

Code documentation:
  The included TwitchChatClient.cs file contains all of the functionality you'll need to get started, along with documentation and a basic usage example.

  Documentation for TwitchText, TwitchEmoteImage, and TwitchEmoteCache are also contained at the bottom of TwitchChatClient.cs.

Examples:
  Check out the sample scenes and some of the included example scripts to see some of what's possible with TwitchChatter.