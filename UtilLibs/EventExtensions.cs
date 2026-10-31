using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UtilLibs;

/// <summary>
/// wrapper for properly creating and cleaning up subscriptions
/// skips need for manually declaring/storing handles
/// </summary>
public static class EventExtensions
{
	//weak table so it doesnt keep destroyed behaviors referenced and prevents them from getting GC'd.
	private static readonly ConditionalWeakTable<KMonoBehaviour, List<int>> registry = [];

	public static void Sub(this KMonoBehaviour self, GameHashes hash, Action<object> handler) => Sub(self, (int)hash, handler);
	public static void Sub(this KMonoBehaviour self, ModHashes hash, Action<object> handler) => Sub(self, (int)hash, handler);
	public static void Sub(this KMonoBehaviour self, int hash, Action<object> handler)
	{
		int handle = self.Subscribe((int)hash, handler);
		registry.GetOrCreateValue(self).Add(handle);
	}
	public static void Sub(this KMonoBehaviour self, GameHashes hash, Action<object, object> contextAndData) => Sub(self, (int)hash, contextAndData);
	public static void Sub(this KMonoBehaviour self, int hash, Action<object, object> contextAndData)
	{
		int handle = self.Subscribe(hash, contextAndData, self);
		registry.GetOrCreateValue(self).Add(handle);
	}

	public static void UnsubAll(this KMonoBehaviour self)
	{
		if (!registry.TryGetValue(self, out List<int> handles))
			return;

		foreach (int handle in handles)
			self.Unsubscribe(handle);

		registry.Remove(self);
	}
}

//public void Sub(this KMonoBehaviour cmp, GameHashes hash, Action<object> handler)
//{
//	RegisterSub(cmp.GetInstanceID(), cmp.Subscribe((int)hash, handler));
//}
//public void Sub<T>(T cmp, GameHashes hash, EventSystem.IntraObjectHandler<T> handler)
//	where T : KMonoBehaviour
//{
//	RegisterSub(cmp.GetInstanceID(), cmp.Subscribe<T>((int)hash, handler));
//}
//private void RegisterSub(int instanceId, int handle)
//{
//	if (!registry.TryGetValue(instanceId, out List<int> list))
//		registry[instanceId] = list = [];
//	list.Add(handle);
//}

//public void UnsubAll(KMonoBehaviour cmp)
//{
//	int instanceId = cmp.GetInstanceID();
//	if (!registry.TryGetValue(instanceId, out List<int> handles))
//		return;

//	foreach (int handle in handles)
//		cmp.Unsubscribe(handle);

//	registry.Remove(instanceId);
//}