using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Dispatcher))]
[RequireComponent(typeof(Receiver))]
[RequireComponent(typeof(Sender))]
public class SpeckleTokenConnector : MonoBehaviour
{
    [SerializeField] private string _token = "";
    [SerializeField] bool _doesAutoReceive = true;
    [SerializeField] private AbstractUIController _ui;
    [SerializeField] private Transform _container;

    public abstract class AbstractUIController : MonoBehaviour
    {
        public abstract void Initialize(Action onReceive, Action onSend, Action<string> onTokenChanged,
            Action<int> onStreamSelected, string token);

        public abstract void UpdateUI(bool isConnected, List<string> streams, int selectedStream, List<string> branches,
            int selectedBranch, List<string> commits, int selectedCommit);

        public abstract void LogMessage(string message);
    }

    private Account _selectedAccount;
    private string _selectedStreamID;
    private Receiver _receiver;
    private Sender _sender;
    private const int StreamsLimit = 30;

    [ContextMenu("Test Mesh")]
    public void TestMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        // assign the array of colors to the Mesh.
        mesh.colors = colors;
    }
    
    void Start()
    {
        _receiver = GetComponent<Receiver>();
        _sender = GetComponent<Sender>();
        _ui.Initialize(
            Receive,
            SendContainer,
            token =>
            {
                _token = token;
                Initialize();
            },
            InitializeStream,
            _token
        );
        Initialize();
    }

    [ContextMenu("Initialize")]
    private void Initialize()
    {
        InitializeStream(0);
    }

    public void InitializeStream(int streamIndex)
    {
        List<Stream> streams = null;
        Stream stream = null;
        List<Branch> branches = null;
        List<string> commits = null;

        void UpdateUIOnMainThread(bool isTokenValid) => Dispatcher
            .Instance()
            .Enqueue(() =>
                _ui.UpdateUI(
                    isTokenValid,
                    streams?.Select(s => s.name)
                        .ToList(),
                    streamIndex,
                    branches?.Select(b => b.name)
                        .ToList(),
                    0,
                    commits?.ToList(),
                    0)
            );

        Task.Run(async () =>
        {
            try
            {
                _selectedAccount = GetAccount(_token);
                var client = new Client(_selectedAccount);
                streams = await client.StreamsGet(StreamsLimit);
                LogOnMain($"Found {streams.Count} streams at {client.ServerUrl}");
                if (streams.Any())
                {
                    _selectedStreamID = streams[streamIndex].id;
                    stream = await client.StreamGet(_selectedStreamID, 5);
                    LogOnMain($"Pick stream index {streamIndex}: {stream.name}");

                    branches = stream.branches.items;
                    var mainBranch = await client.BranchGet(_selectedStreamID, branches[0]
                        .name, 1);
                    commits = mainBranch.commits.items.Select(c => c.message).ToList();

                    _receiver.Stream = stream;
                    _receiver.BranchName = branches[0]
                        .name;
                    _receiver.Init(
                        stream.id,
                        _doesAutoReceive,
                        true,
                        _selectedAccount,
                        onDataReceivedAction: (go) => { Debug.Log($"Received {go.name}"); },
                        onTotalChildrenCountKnown: (count) => { _receiver.TotalChildrenCount = count; },
                        onProgressAction: (dict) =>
                        {
                            var progress = dict.Values.Average() / (0.01d * _receiver.TotalChildrenCount);
                            progress = Math.Min(progress, 100);
                            LogOnMain($"Receiving ... {progress:0}%");
                        }
                    );
                }

                UpdateUIOnMainThread(true);
                LogOnMain("Connector initialized.");
            }
            catch
            {
                UpdateUIOnMainThread(false);
                LogOnMain("Connector failed to initialize.");
                throw;
            }
        });
    }

    public void Send(System.Collections.Generic.ISet<GameObject> objectsToSend)
    {
        try
        {
            _sender
                .Send(
                    _selectedStreamID,
                    objectsToSend,
                    _selectedAccount,
                    onProgressAction: (dict) =>
                    {
                        var val = dict.Values.Average();
                        LogOnMain($"Did send: {val:##.0}");
                    },
                    onDataSentAction: (objectId) => { LogOnMain($"Send operation completed, object id: {objectId}"); },
                    onErrorAction: (message, e) => { LogOnMain("Send operation Failed!"); });
        }
        catch (Exception e)
        {
            LogOnMain(e.ToString());
        }
    }

    [ContextMenu("Receive")]
    public void Receive()
    {
        _receiver.Receive();
    }

    [ContextMenu("Send Container")]
    public void SendContainer()
    {
        Send(_container.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToImmutableHashSet());
    }

    private static Account GetAccount(string token)
    {
        try
        {
            var a = new Account
            {
                token = token,
                serverInfo = new ServerInfo
                {
                    url = "https://speckle.xyz/"
                }
            };
            return a;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    private void LogOnMain(string message) => Dispatcher
        .Instance()
        .Enqueue(() => _ui.LogMessage(message));
}