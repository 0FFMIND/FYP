using Utils.SingletonPattern;
using System.IO;
using UnityEngine;

namespace MVC.Model
{
    public class LocalizationMgr : SingletonMB<LocalizationMgr>
    {
        public string CurrentLanguage = "zh";
        public void SetLanguage(string lanCode)
        {
            CurrentLanguage = lanCode;
        }
        public string GetDialoguePath(string filename)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Localization", CurrentLanguage);
            return Path.Combine(path,filename);
        }
    }
}
