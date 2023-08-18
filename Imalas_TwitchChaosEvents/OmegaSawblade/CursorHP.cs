using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.OmegaSawblade
{
    internal class CursorHP : KMonoBehaviour, ISim33ms
    {
        public void Sim33ms(float dt)
        {
            transform.SetPosition(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
        }
    }
}
