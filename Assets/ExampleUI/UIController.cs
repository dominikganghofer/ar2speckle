using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SpeckleTokenConnector.AbstractUIController
{
    [SerializeField] private Button _receiveButton;
    [SerializeField] private Button _sendButton;

    [SerializeField] private TMP_InputField _tokenInput;

    [SerializeField] private TMP_Dropdown _streamsDropdown;
    [SerializeField] private TMP_Dropdown _branchesDropdown;
    [SerializeField] private TMP_Dropdown _commitsDropdown;
    [SerializeField] private TMP_Text _log;

    public override void Initialize(Action onReceive, Action onSend, Action<string> onTokenChanged,
        Action<int> onStreamSelected,  string token)
    {
        _receiveButton.onClick.AddListener(() => onReceive());
        _sendButton.onClick.AddListener(() => onSend());
        _streamsDropdown.onValueChanged.AddListener(iStream => onStreamSelected(iStream));
        _tokenInput.onSubmit.AddListener(t => onTokenChanged(t));
        _tokenInput.text = token;
        _log.text = "";
    }

    public override void UpdateUI(
        bool isConnected,
        List<string> streams,
        int selectedStream,
        List<string> branches,
        int selectedBranch,
        List<string> commits,
        int selectedCommit)
    {
        _sendButton.gameObject.SetActive(isConnected);
        _receiveButton.gameObject.SetActive(false);
        _streamsDropdown.gameObject.SetActive(isConnected);

        if(!isConnected)
            return;
        _streamsDropdown?.ClearOptions();
        _streamsDropdown?.AddOptions(streams);
        _streamsDropdown?.SetValueWithoutNotify(selectedStream);

        _branchesDropdown?.ClearOptions();
        _branchesDropdown?.AddOptions(branches);
        _branchesDropdown?.SetValueWithoutNotify(selectedBranch);

        _commitsDropdown?.ClearOptions();
        _commitsDropdown?.AddOptions(commits);
        _commitsDropdown?.SetValueWithoutNotify(selectedCommit);
    }

    public override void LogMessage(string message)
    {
        Debug.Log(message);
        _log.text = message + '\n' + _log.text;
    }
}