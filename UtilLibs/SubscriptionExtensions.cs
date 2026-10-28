using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// wrapper for properly creating and cleaning up subscriptions
/// skips need for manually declaring/storing handles
/// </summary>
public static class SubscriptionExtensions
{
	//weak table so it doesnt keep destroyed behaviors referenced and prevents them from getting GC'd.
	private static readonly ConditionalWeakTable<KMonoBehaviour, List<int>> registry = [];

	public static void Sub(this KMonoBehaviour self, GameHashes hash, Action<object> handler)
	{
		int handle = self.Subscribe((int)hash, handler);
		registry.GetOrCreateValue(self).Add(handle);
	}
	public static void Sub<T>(this T self, GameHashes hash, EventSystem.IntraObjectHandler<T> handler)
		where T : KMonoBehaviour
	{
		registry.GetOrCreateValue(self).Add(self.Subscribe((int)hash, handler));
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