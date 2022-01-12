using System.Net.Sockets;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

/// <summary>
/// The class that connects to fluentd server.
/// </summary>
internal class FluentdClient : IDisposable
{
	private readonly TcpClient m_Tcp;
	private readonly FluentdOptions m_Setting;
	private bool m_Disposed;
	private NetworkStream m_Stream = default!;

	/// <summary>
	/// Create a new <see cref="FluentdClient"/> instance.
	/// </summary>
	/// <param name="setting">The setting for connecting to fluentd server.</param>
	public FluentdClient(FluentdOptions setting)
	{
		m_Setting = setting ?? throw new ArgumentNullException(nameof(setting));
		m_Tcp = new TcpClient
		{
			NoDelay = true
		};
	}

	/// <summary>
	/// Destructor for not calling <see cref="Dispose()"/> method.
	/// </summary>
	~FluentdClient()
	{
		Dispose(false);
	}

	/// <inheritdoc cref="IFluentdClient.ConnectAsync" />
	public async Task ConnectAsync()
	{
		if (m_Setting.Timeout.HasValue)
			m_Tcp.SendTimeout = m_Setting.Timeout.Value;

		await m_Tcp.ConnectAsync(m_Setting.Host, m_Setting.Port).ConfigureAwait(false);
	}

	public async Task SendAsync(string tag, Batch<LogRecord> batch)
	{
		var value = m_Setting.Serializer!.Serialize(tag, batch);

		await SendAsyncInternal(value).ConfigureAwait(false);
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Free, release, or reset managed or unmanaged resources.
	/// </summary>
	/// <param name="disposing">Wether to free, release, or resetting unmanaged resources or not.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (m_Disposed)
			return;

		if (disposing)
		{
			m_Stream?.Dispose();
			m_Tcp?.Dispose();
		}

		m_Disposed = true;
	}

	private async Task SendAsyncInternal(byte[] message)
	{
		if (!m_Tcp.Connected)
			await ConnectAsync().ConfigureAwait(false);

		m_Stream = m_Tcp.GetStream();

		await m_Stream.WriteAsync(message).ConfigureAwait(false);

		await m_Stream.FlushAsync().ConfigureAwait(false);
	}
}
