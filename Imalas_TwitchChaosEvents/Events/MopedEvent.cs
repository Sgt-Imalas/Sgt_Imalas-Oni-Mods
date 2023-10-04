using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{

    public static class stringHelper
    {
        public static string Repeat(this string s, int n) => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }

    public class MopedEvent : ITwitchEventBase
    {
        public static Dictionary<LocText, Tuple<string,string>> OriginalStrings = new Dictionary<LocText, Tuple<string, string>>();
        static bool MopedActive = false;
        public string ID => "ITCE_Moped";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.MOPED.NAME;

        public EventWeight EventWeight => EventWeight.WEIGHT_NORMAL;

        public Action<object> EventAction => (_)=> 
        {
            Mopedication(true);
            ToastManager.InstantiateToast(
                STRINGS.CHAOSEVENTS.MOPED.TOAST,
                STRINGS.CHAOSEVENTS.MOPED.TOASTTEXT);
            GameScheduler.Instance.Schedule("unmoped", 20f, (_) => Mopedication(false));
        };
        public void Mopedication(bool enabling)
        {
            MopedActive = enabling;
            var foundOriginalStrings = GameObject.FindObjectsOfType<LocText>(true);

            if(enabling)
            {
                OriginalStrings.Clear();
                foreach(var originalString in foundOriginalStrings)
                {

                    string prevText = originalString.text != null ? originalString.text : Strings.Get(originalString.key).String;

                    int count = prevText.Length;
                    int mopedLenght = STRINGS.CHAOSEVENTS.MOPED.MOPEDTEXT.text.Length;
                    int mopedCount = Math.Max(count/mopedLenght,1);
                    string newText = stringHelper.Repeat(STRINGS.CHAOSEVENTS.MOPED.MOPEDTEXT,mopedCount);

                    OriginalStrings[originalString] = new (originalString.key,originalString.text);
                    originalString.key = string.Empty;
                    originalString.text = newText;
                }
            }
            else
            {
                foreach (var revert in OriginalStrings)
                {
                    if(revert.Value.second != null)
                    {
                        revert.Key.SetText(revert.Value.second);
                    }
                    else
                    {
                        revert.Key.key = (revert.Value.first);
                        revert.Key.ApplySettings();
                    }
                }
            }
        }

        //too buggy :(
        public Func<object, bool> Condition => (_) => false;//!MopedActive;

        public Danger EventDanger => Danger.Small;
    }
}
