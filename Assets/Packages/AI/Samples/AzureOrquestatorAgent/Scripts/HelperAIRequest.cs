using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Timers;
using UnityEngine;
using Xennial.API;
using Xennial.Services;

namespace Services.AI.AIProcedureHelp
{
    public class HelperAIRequest : Request<HelperAIServiceData>, IRecieverMicDataCollaborator, IService, IAIHandler, IAIHandlerExtention
    {
        public event Action<string> OnBase64EncodeGenerated;
        public event Action<byte[]> OnGetRecordData;
        public event Action<AIAPIResponse> OnAISingleResponse;
        public event Action<AIAPIResponse> OnAIStreamingResponse;
        public event Action<bool> OnCollaboratorStateChanged;
        [SerializeField]
        private AIConfigData _config;
        [SerializeField, ReadOnly]
        private bool _isRequestRunning;
        [SerializeField]
        private int _timeout;
        [SerializeField, ReadOnly]
        private int _timeoutCount;
        [SerializeReference]
        private IRequestJsonProvider _requestProvider;

        private MicrophoneRecorder _microphoneRecorder;
        private string _clientId = "1";
        private string _sessionId;
        private string _baser64;
        private string _moduleId;
        private AIAPIResponse _aiApiResponse;
        private Timer _timeoutTimer;

        private EventBusService _busService;
        protected override object[] RequestParams
        {
            get
            {
                return new object[] { };
            }
        }

        public AIConfigData GetConfigData => _config;


        private void Awake()
        {
            Register();
        }
        private void OnDestroy()
        {
            Unregister();
        }
        private void Start()
        {
            _timeoutTimer = new Timer(1000);
            _timeoutTimer.Elapsed += OnTimeOut;
            _requestData = new HelperAIServiceData();
            _aiApiResponse = new AIAPIResponse(AIResponseType.None, null);
            _requestProvider.Init();
            Build();
            _busService = ServiceLocator.Instance.Get<EventBusService>();
        }

        public void InjectConfig(AIConfigData config)
        {
            _config = config;
            Build();
        }

        private void OnTimeOut(object sender, ElapsedEventArgs e)
        {
            _timeoutCount++;
            if (_timeoutCount >= _timeout)
            {
                StopTimer();
            }
        }

        private void StopTimer()
        {
            _timeoutTimer.Stop();
            _timeoutCount = 0;
            _isRequestRunning = false;
        }
        private void Build()
        {
            if (_config == null)
                throw new Exception("The AIConfig file is null please add one");
            

            _requestData.WithUrl(_config.Url)
                .WithModuleId(_moduleId)
                .WithServiceUrl(_config.AIModel)
                .WithClientId(_clientId)
                .WithSessionId(_sessionId)
                .WithContentType("audio/wav");

        }
        [Button]
        public void Input(string input = null)
        {
            if (_isRequestRunning)
                return;
            _isRequestRunning = true;

            _busService?.Broadcast(nameof(AIEventEnums.TruncateSpeech));
            ServiceLocator.Instance.Get<MicDataSenderMediator>().SetReciverCollaborator(this);

            if (_microphoneRecorder != null)
                _microphoneRecorder.Init();
        }

        protected override void OnResponseReceived(string response)
        {

            StopTimer();
            DefaultHelperAIResponse helperAIResponse = JsonConvert.DeserializeObject<DefaultHelperAIResponse>(response);

            if (helperAIResponse != null)
            {
                TryUpdateAndNotify(helperAIResponse.transcription, AIResponseType.UserTranscription);
                TryUpdateAndNotify(helperAIResponse.text_to_speech, AIResponseType.AIResponse);
                AIHelperActionsResponse _actions = new AIHelperActionsResponse(helperAIResponse.actions);
                TryUpdateAndNotify(_actions.ToJson(), AIResponseType.Json_extraData);
            }
            ServiceLocator.Instance.Get<MicDataSenderMediator>().ReturnDefaultCollaborator();

        }


        private void TryUpdateAndNotify(string value, AIResponseType type)
        {
            if (string.IsNullOrEmpty(value))
                return;

            _aiApiResponse.UpdateData(type, value);
            OnAISingleResponse?.Invoke(_aiApiResponse);

        }
        public void RecieveBase64Data(string base64)
        {
            OnBase64EncodeGenerated?.Invoke(base64);
        }



        public void RecieveByteData(byte[] data)
        {
            _baser64 = Convert.ToBase64String(data);
            OnGetRecordData?.Invoke(data);
        }

        public void EnableCollaborator(MicrophoneRecorder microphone)
        {
            if (_microphoneRecorder == null)
                _microphoneRecorder = microphone;

            OnCollaboratorStateChanged?.Invoke(true);
            _microphoneRecorder.IsStreaming = false;
        }

        public void DisposeCollaborator()
        {
            _microphoneRecorder.IsStreaming = false;
            OnCollaboratorStateChanged?.Invoke(false);
        }

        public void MicStarted() { }

        public void MicEnded()
        {
            _requestData.SetAudioData(_baser64)
                .SetInputRequest(_requestProvider.ParseToJson());

            BuildSendRequest(_timeout);
            _timeoutTimer.Start();
            _microphoneRecorder.Dispose();
        }

        public void Register()
        {
            ServiceLocator.Instance.Register(this);
        }

        public void Unregister()
        {
            ServiceLocator.Instance.Unregister(this);
        }

        private TValue GetServiceValue<TService, TValue>(Func<TService, TValue> selector, TValue defaultValue = default) where TService : IService
        {
            if (ServiceLocator.Instance.Exist<TService>())
            {
                return selector(ServiceLocator.Instance.Get<TService>());
            }
            return defaultValue;
        }


    }
}

