using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    [Serializable]
    public class DialogueClip
    {
        public string textFile;
        public LineMapping[] mappings;
    }

    public class Scene1DialogueClips : MonoBehaviour
    {
        public List<DialogueClip> dialogMappings;
    }
}