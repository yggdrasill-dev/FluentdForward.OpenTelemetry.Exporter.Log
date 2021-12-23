using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FluentdClient.Sharp.MsgPack")]
[assembly: InternalsVisibleTo("FluentdClient.Sharp.MessagePack")]

namespace FluentdForward.OpenTelemetry.Exporter.Logs
{
	internal static class Extensions
	{
		internal static TimeSpan GetUnixTimestamp(this DateTime dateTime)
		{
			return dateTime.ToUniversalTime().Subtract(DateTime.UnixEpoch);
		}

		internal static TimeSpan GetUnixTimestamp(this DateTimeOffset dateTimeOffset)
		{
			return dateTimeOffset.ToUniversalTime().Subtract(DateTimeOffset.UnixEpoch);
		}

		internal static bool IsAnonymous(this TypeInfo type)
		{
			var hasAttribute = type.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
			var containsName = type.FullName?.Contains("AnonymousType") == true;

			return hasAttribute && containsName;
		}
	}
}
