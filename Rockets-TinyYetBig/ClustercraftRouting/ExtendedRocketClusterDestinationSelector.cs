using KSerialization;
using System;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.ClustercraftRouting
{
    public enum RouteMode
    {
        undefined = 0,
        Circle = 1,
        Line = 2,
    }

    internal class ExtendedRocketClusterDestinationSelector : RocketClusterDestinationSelector
    {
        [Serialize]
        public List<AxialI> RouteDestinations = new List<AxialI>();

        [Serialize]
        public int CurrentDestinationIndex=0;

        [Serialize]
        public int CurrentRouteMode = (int)RouteMode.Circle;

        [Serialize]
        public bool GoingBackwards =false;




        internal void ProceedToNextTarget()
        {
            CurrentDestinationIndex += GoingBackwards ? -1 : 1;

            if (CurrentDestinationIndex == RouteDestinations.Count || CurrentDestinationIndex < 0)
            {
                switch (CurrentRouteMode)
                {
                    case (int)RouteMode.Line:
                        CurrentDestinationIndex = GoingBackwards ? 0 : RouteDestinations.Count - 1;
                        GoingBackwards = !GoingBackwards;
                        break;
                    default:
                    case (int)RouteMode.Circle:
                        CurrentDestinationIndex = GoingBackwards ? 0 : RouteDestinations.Count - 1;
                        break;
                }
            }
        }
        public AxialI CurrentTarget => RouteDestinations[CurrentDestinationIndex];

    }
}