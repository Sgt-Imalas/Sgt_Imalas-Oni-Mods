namespace SkillingQueue
{
	internal class STRINGS
	{
		public class SKILLQUEUE
		{
			public static LocString QUEUE = "Hold " + global::STRINGS.UI.PRE_KEYWORD + "{0}" + global::STRINGS.UI.PST_KEYWORD + " and click to add to skill queue";
			public static LocString DEQUEUE = "Currently at position {0} in the skill queue.\nHold " + global::STRINGS.UI.PRE_KEYWORD + "{1}" + global::STRINGS.UI.PST_KEYWORD + " and click to remove from the queue";
		}


		public class SKILLQUEUE_STATUSITEMS
		{
			public class SQ_NEW_SKILL_LEARNED
			{
				public static LocString NAME = "New Skill: {0}";
				public static LocString DESCRIPTION = "{0} has learned the skill {1}";
			}
			public class SQ_QUEUE_COMPLETED
			{
				public static LocString NAME = "{0}: Skillqueue completed";
				public static LocString DESCRIPTION = "{0} has learned all queued skills.";
			}

		}
		public class SKILLQUEUE_CONFIG
		{
			public class NOTIFICATION_ON_SKILL
			{
				public static LocString NAME = "Notify on Skill learned";
				public static LocString DESCRIPTION = "Get a notification when a dupe learns a new skill through a skill queue.";
			}
			public class NOTIFICATION_ON_QUEUE_COMPLETE
			{
				public static LocString NAME = "Notify on skill queue completion";
				public static LocString DESCRIPTION = "Get a notification when a skill queue gets completed.";
			}
			public class RAINBOW_QUEUE
			{
				public static LocString NAME = "Rainbow queue indicator";
				public static LocString DESCRIPTION = "The queue position indicator numbers are tinted with rainbow progression for easier distinction.";
			}
		}
	}
}
