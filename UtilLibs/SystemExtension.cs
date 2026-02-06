// Source - https://stackoverflow.com/a/67687186
// Posted by PMF
// Retrieved 2026-02-06, License - CC BY-SA 4.0

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Reserved to be used by the compiler for tracking metadata.
	/// This class should not be used by developers in source code.
	/// This dummy class is required to compile records when targeting .NET Standard
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IsExternalInit
	{
	}
}
