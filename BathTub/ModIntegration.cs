using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.ROOMS.TYPES;

namespace BathTub
{
    internal class ModIntegration
    {
        public class Rooms_Expanded
        {
            //public static bool IsInTheRoom(KMonoBehaviour item, string roomId)
            public delegate bool Rooms_Expanded_IsInTheRoom_Delegate(KMonoBehaviour item, string roomId);
            public static Rooms_Expanded_IsInTheRoom_Delegate IsInTheRoom;

            public static string BathRoom_RoomId = "BathroomRoom";
            public static string PlumbedBathroomId = null;
            public static string PrivateBedroomId = "BathroomRoom";
            public static float BathRoomBonus = 0.2f;

            public static bool BathroomBonusActive(KMonoBehaviour item)
            {
                if (PlumbedBathroomId == null)
                    PlumbedBathroomId = Db.Get().RoomTypes.PlumbedBathroom.Id;
                if (PrivateBedroomId == null)
                    PrivateBedroomId = Db.Get().RoomTypes.PrivateBedroom.Id;

                if (ModAssets.RoomsExpandedActive)
                {
                    if (IsInTheRoom(item, BathRoom_RoomId)
                    || IsInTheRoom(item, PlumbedBathroomId)
                    || IsInTheRoom(item, PrivateBedroomId))
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool InitializeIntegration()
            {
                //UtilMethods.ListAllTypesWithAssemblies();
                SgtLogger.l("Initializing RoomsExpanded integration");
                var type = Type.GetType("RoomsExpanded.RoomTypes_AllModded, RoomsExpandedMerged", false, false);
                if (type == null)
                {
                    SgtLogger.l("RoomsExpanded.RoomTypes_AllModded not found, RoomsExpanded Integration going to sleep. zzzzz");
                    return false;
                }

                var m_IsInTheRoom = AccessTools.Method(type, "IsInTheRoom",
                    new[]
                    {
                    typeof(KMonoBehaviour),
                    typeof(string)
                    });

                if (m_IsInTheRoom == null)
                {
                    Debug.LogWarning("IsInTheRoom is not a method on RoomsExpanded.RoomTypes_AllModded.");
                    return false;
                }

                IsInTheRoom = (Rooms_Expanded_IsInTheRoom_Delegate)Delegate.CreateDelegate(typeof(Rooms_Expanded_IsInTheRoom_Delegate), m_IsInTheRoom);

                InitBathRoomBonus();
                return true;
            }
            static void InitBathRoomBonus()
            {
                SgtLogger.l("grabbing bathroom bonus value...");
                var type = Type.GetType("RoomsExpanded.Settings, RoomsExpandedMerged", false, false);
                if (type == null)
                {
                    Debug.LogWarning("RoomsExpanded.Settings is not a method on RoomsExpanded.RoomTypes_AllModded.");
                    return;
                }
                var settingsInstance = AccessTools.PropertyGetter(type, "Instance");
                if (settingsInstance == null)
                {
                    Debug.LogWarning("RoomsExpanded.Settings.Get not found");
                    return;
                }
                object instance = settingsInstance.Invoke(null, null);
                if (instance == null)
                {
                    Debug.LogWarning("instance was null");
                    return;

                }
                object bathroomData = Traverse.Create(instance).Property("Bathroom").GetValue();

                if (bathroomData == null)
                {
                    Debug.LogWarning("bathroomData was null");
                    return;
                }

                bool roomActive = (bool)Traverse.Create(bathroomData).Property("IncludeRoom").GetValue();
                float bonus = (float)Traverse.Create(bathroomData).Property("Bonus").GetValue();
                SgtLogger.l("bonus: " + bonus);
                SgtLogger.l("roomActive: " + roomActive);
                BathRoomBonus = roomActive ? bonus : 0;
            }
        }
    }
}
