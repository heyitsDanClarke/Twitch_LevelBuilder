using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace TwitchChatter
{

[CustomEditor(typeof(TwitchText))]
public class TwitchTextEditor : UnityEditor.UI.TextEditor
{
	private SerializedProperty _listenModeProperty;
	private SerializedProperty _channelNameProperty;
	private SerializedProperty _userNameProperty;
	private SerializedProperty _textModeProperty;
	private SerializedProperty _emoticonScaleFactorProperty;
	private SerializedProperty _maxCharacterCountProperty;
	private SerializedProperty _renderUserNames;

	protected override void OnEnable()
	{
		base.OnEnable();

		_listenModeProperty = this.serializedObject.FindProperty("_listenMode");
		_channelNameProperty = this.serializedObject.FindProperty("_channelName");
		_userNameProperty = this.serializedObject.FindProperty("_userName");
		_textModeProperty = this.serializedObject.FindProperty("_textMode");
		_emoticonScaleFactorProperty = this.serializedObject.FindProperty("_emoticonScaleFactor");
		_maxCharacterCountProperty = this.serializedObject.FindProperty("_maxCharacterCount");
		_renderUserNames = this.serializedObject.FindProperty("_renderUserNames");
	}

	public override void OnInspectorGUI()
	{
		int initialListenMode = _listenModeProperty.enumValueIndex;
		float initialEmoteScaleFactor = _emoticonScaleFactorProperty.floatValue;
		float maxCharacterCount = _maxCharacterCountProperty.intValue;

		this.serializedObject.Update();

		EditorGUILayout.PropertyField(_listenModeProperty);

		if (_listenModeProperty.enumValueIndex == (int)TwitchText.ListenMode.Chat)
		{
			EditorGUILayout.PropertyField(_channelNameProperty);
		}
		else if (_listenModeProperty.enumValueIndex == (int)TwitchText.ListenMode.Whisper)
		{
			EditorGUILayout.PropertyField(_userNameProperty);
		}

		EditorGUILayout.PropertyField(_textModeProperty);
		EditorGUILayout.PropertyField(_emoticonScaleFactorProperty);
		EditorGUILayout.PropertyField(_maxCharacterCountProperty);
		EditorGUILayout.PropertyField(_renderUserNames);

		this.serializedObject.ApplyModifiedProperties();

		if (initialListenMode != _listenModeProperty.enumValueIndex)
		{
			(this.target as TwitchText).OnListenModeModified();
		}
		if (initialEmoteScaleFactor != _emoticonScaleFactorProperty.floatValue)
		{
			(this.target as TwitchText).OnEmoticonScaleFactorModified();
		}
		if (maxCharacterCount != _maxCharacterCountProperty.intValue)
		{
			(this.target as TwitchText).OnMaxCharacterCountModified();
		}

		base.OnInspectorGUI();
	}
}

}	// TwitchChatter