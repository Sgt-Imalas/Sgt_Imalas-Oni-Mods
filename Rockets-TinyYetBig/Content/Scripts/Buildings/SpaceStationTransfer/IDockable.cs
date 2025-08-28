using KSerialization;
using System;
using UtilLibs;

namespace Rockets_TinyYetBig.Docking
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class IDockable : KMonoBehaviour
	{

		[Serialize]
		private string DockableId;
		public string GUID => DockableId;


		//[Serialize]
		//private int _worldId = -1;
		public int WorldId => spacecraftHandler == null ? -1 : spacecraftHandler.WorldId;

		[MyCmpGet] public NavTeleporter Teleporter;

		public DockingSpacecraftHandler spacecraftHandler;


		public bool HasDupeTeleporter => Teleporter != null;// && MoveTo != null && assignable != null ;


		/// <summary>
		/// Stuff to update when the docking connection changes
		/// </summary>
		public virtual bool UpdateDockingConnection(bool initORCleanupCall = false)
		{
			bool connected = DockingManagerSingleton.Instance.IsDocked(this.GUID, out _);
			SgtLogger.l("isConnected; " + connected);
			this.Trigger(connected ? ModAssets.Hashes.DockingConnectionConnected : ModAssets.Hashes.DockingConnectionDisconnected);
			this.Trigger((int)GameHashes.ChainedNetworkChanged);
			UpdateHandler();
			return connected;
		}
		public void UpdateHandler()
		{
			if (spacecraftHandler != null)
			{
				spacecraftHandler.gameObject.Trigger(ModAssets.Hashes.DockingConnectionChanged);
			}

		}


		/// <summary>
		/// Updates World ID each time a module changes; relevant for dockables that are rocket modules
		/// </summary>
		void RefreshWorldIdForRocketModule()
		{
			if (gameObject.TryGetComponent<RocketModuleCluster>(out var moduleCluster))
			{
				DockingManagerSingleton.Instance.UnregisterDockable(this);
				var world = moduleCluster.CraftInterface.GetInteriorWorld();
				if (world != null)
				{
					SgtLogger.l("new world found for docking module: " + world.id);
					//_worldId = world.id;

					spacecraftHandler = world.gameObject.AddOrGet<DockingSpacecraftHandler>();
					spacecraftHandler.RegisterDockable(this);
				}
				else
				{
					SgtLogger.l("no world found for docking module, defaulting");
					// _worldId = -1;
				}
				DockingManagerSingleton.Instance.RegisterDockable(this);
			}
		}

		public override void OnSpawn()
		{
			SgtLogger.Assert(GUID,"GUID");
			if (DockableId == null || DockableId.Length == 0)
			{
				DockableId = Guid.NewGuid().ToString();
			}
			base.OnSpawn();
			if (WorldId == -1)
			{
				SgtLogger.l("WorldId was default, initializing..", gameObject.GetProperName());

				///module docking ports; ai rocket integration
				if (gameObject.TryGetComponent<RocketModuleCluster>(out var moduleCluster))
				{
					RefreshWorldIdForRocketModule();
					moduleCluster.Subscribe((int)GameHashes.RocketModuleChanged, (_) => RefreshWorldIdForRocketModule());
				}
				///building docking ports inside of worlds
				else
				{
					//_worldId = this.GetMyWorldId();
					var myWorld = this.GetMyWorld();
					spacecraftHandler = myWorld.gameObject.AddOrGet<DockingSpacecraftHandler>();
					spacecraftHandler.RegisterDockable(this);
				}
			}
			else
			{
				var world = ClusterManager.Instance.GetWorld(WorldId);
				spacecraftHandler = world.gameObject.AddOrGet<DockingSpacecraftHandler>();
				spacecraftHandler.RegisterDockable(this);
			}

			DockingManagerSingleton.Instance.RegisterDockable(this);

			UpdateDockingConnection(true);
		}
		protected bool cleaningUp = false;
		public override void OnCleanUp()
		{
			SgtLogger.l("cleaning up on dockable decon");
			cleaningUp = true;
			DockingManagerSingleton.Instance.UnregisterDockable(this);
			if (spacecraftHandler != null)
			{
				spacecraftHandler.UnregisterDockable(this);
			}
			base.OnCleanUp();
		}
	}
}
