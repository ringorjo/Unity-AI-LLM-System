using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Services.AI;

[CreateAssetMenu(fileName = "ElevenLabVoices", menuName = "Xennial Digital/AI/ElevenLabs/ElevenLabVoices", order = 1)]
        
public class ElevenLabVoices : ScriptableObject
{
    public List<ElevenLabsVoice> voices;

    public string GetVoiceId(string voiceName)
    {
       return voices.Find(voice => voice.name == voiceName)?.voice_id;
    }


    public void AddVoice(ElevenLabsVoice voice)
    {
        if (!voices.Contains(voice))
        {
            voices.Add(voice);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }



}


