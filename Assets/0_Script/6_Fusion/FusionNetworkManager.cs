namespace Fusion
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(FusionBS))]
    public class FusionNetworkManager : Fusion.Behaviour
    {


        /// <summary>
        /// The GUISkin to use as the base for the scalable in-game UI.
        /// </summary>
        [InlineHelp]
        public GUISkin BaseSkin;

        FusionBS _networkDebugStart;
        string _clientCount;
        bool _isMultiplePeerMode;

        Dictionary<FusionBS.Stage, string> _nicifiedStageNames;

#if UNITY_EDITOR

        protected virtual void Reset()
        {
            _networkDebugStart = EnsureNetworkDebugStartExists();
            _clientCount = "1";
            BaseSkin = GetAsset<GUISkin>("e59b35dfeb4b6f54e9b2791b2a40a510");
        }

#endif

        protected virtual void OnValidate()
        {
            ValidateClientCount();
        }

        protected void ValidateClientCount()
        {
            if (_clientCount == null)
            {
                _clientCount = "1";
            }
            else
            {
                _clientCount = System.Text.RegularExpressions.Regex.Replace(_clientCount, "[^0-9]", "");
            }
        }
        protected int GetClientCount()
        {
            try
            {
                return Convert.ToInt32(_clientCount);
            }
            catch
            {
                return 0;
            }
        }

        protected virtual void Awake()
        {

            _nicifiedStageNames = ConvertEnumToNicifiedNameLookup<FusionBS.Stage>("Fusion Status: ");
            _networkDebugStart = EnsureNetworkDebugStartExists();
            _clientCount = "1";
            ValidateClientCount();
        }
        protected virtual void Start()
        {
            _isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
        }

        protected FusionBS EnsureNetworkDebugStartExists()
        {
            if (_networkDebugStart)
            {
                if (_networkDebugStart.gameObject == gameObject)
                    return _networkDebugStart;
            }

            if (TryGetBehaviour<FusionBS>(out var found))
            {
                _networkDebugStart = found;
                return found;
            }

            _networkDebugStart = AddBehaviour<FusionBS>();
            return _networkDebugStart;
        }


        public void StartSharedClient()
        {
            var nds = EnsureNetworkDebugStartExists();
            if (nds.CurrentStage == FusionBS.Stage.Disconnected)
            {
                nds.StartSharedClient();
            }

        }
        public void StartSinglePlay()
        {
            var nds = EnsureNetworkDebugStartExists();
            if (nds.CurrentStage == FusionBS.Stage.Disconnected)
            {
                nds.StartSinglePlayer();
            }

        }
        



        // TODO Move to a utility
        public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null, Dictionary<T, string> nonalloc = null) where T : System.Enum
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (nonalloc == null)
            {
                nonalloc = new Dictionary<T, string>();
            }
            else
            {
                nonalloc.Clear();
            }

            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));
            for (int i = 0, cnt = names.Length; i < cnt; ++i)
            {
                sb.Clear();
                if (prefix != null)
                {
                    sb.Append(prefix);
                }
                var name = names[i];
                for (int n = 0; n < name.Length; n++)
                {
                    // If this character is a capital and it is not the first character add a space.
                    // This is because we don't want a space before the word has even begun.
                    if (char.IsUpper(name[n]) == true && n != 0)
                    {
                        sb.Append(" ");
                    }

                    // Add the character to our new string
                    sb.Append(name[n]);
                }
                nonalloc.Add((T)values.GetValue(i), sb.ToString());
            }
            return nonalloc;
        }
#if UNITY_EDITOR

        public static T GetAsset<T>(string Guid) where T : UnityEngine.Object
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(Guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            else
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }
#endif
    }
}