using System.Net.Sockets;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

/// <summary>
/// The class that connects to fluentd server.
/// </summary>
internal class FluentdClient : IDisposable
{
	private readonly TcpClient _tcp;
	private readonly FluentdOptions _setting;
	private bool _disposed;
	private NetworkStream _stream = default!;

	/// <summary>
	/// Create a new <see cref="FluentdClient"/> instance.
	/// </summary>
	/// <param name="setting">The setting for connecting to fluentd server.</param>
	public FluentdClient(FluentdOptions setting)
	{
		_setting = setting ?? throw new ArgumentNullException(nameof(setting));
		_tcp = new TcpClient();

		_tcp.NoDelay = true;
	}

	/// <summary>
	/// Destructor for not calling <see cref="Dispose()"/> method.
	/// </summary>
	~FluentdClient()
	{
		Dispose(false);
	}

	/// <inheritdoc cref="IFluentdClient.ConnectAsync" />
	public async ValueTask ConnectAsync()
	{
		if (_setting.Timeout.HasValue)
			_tcp.SendTimeout = _setting.Timeout.Value;

		await _tcp.ConnectAsync(_setting.Host, _setting.Port).ConfigureAwait(false);
	}

	public async ValueTask SendAsync(string tag, LogRecord message)
	{
		var value = _setting.Serializer!.Serialize(tag, message);

		await SendAsyncInternal(value).ConfigureAwait(false);
	}

	public async ValueTask FlushAsync()
		=> await _stream.FlushAsync().ConfigureAwait(false);

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
		if (_disposed)
			return;

		if (disposing)
		{
			_stream?.Dispose();
			_tcp?.Dispose();
		}

		_disposed = true;
	}

	private async ValueTask SendAsyncInternal(byte[] message)
	{
		if (!_tcp.Connected)
			await ConnectAsync().ConfigureAwait(false);

		_stream = _tcp.GetStream();

		await _stream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
	}
}
