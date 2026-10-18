namespace nom_ws_datosempleado_be
{
    using Renci.SshNet;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class SshTunnelService : IDisposable
    {
        private SshClient? _sshClient;
        private ForwardedPortLocal? _forwardedPort;
        private uint _localPort;
        private string? _lastError;

        public void Connect(IConfiguration configuration)
        {
            var ssh = configuration.GetSection("SSH");

            string host = ssh["Host"]!;
            int port = ssh.GetValue<int>("Port");
            string username = ssh["Username"]!;
            string password = ssh["Password"]!;

            uint localPort = ssh.GetValue<uint>("LocalPort");
            string remoteHost = ssh["RemoteHost"]!;
            uint remotePort = ssh.GetValue<uint>("RemotePort");

            try
            {
                _sshClient = new SshClient(
                    host,
                    port,
                    username,
                    password
                );

                // Conectar mediante SSH
                _sshClient.Connect();

                if (!_sshClient.IsConnected)
                {
                    Console.WriteLine("No se pudo establecer la conexión SSH.");
                    return;
                }

                // Crear túnel SSH
                _forwardedPort = new ForwardedPortLocal(
                    "127.0.0.1",
                    localPort,
                    remoteHost,
                    remotePort
                );

                // almacenar puerto local para validaciones posteriores
                _localPort = localPort;

                _sshClient.AddForwardedPort(_forwardedPort);

                // Iniciar túnel
                _forwardedPort.Start();

                Console.WriteLine($"Túnel SSH activo en 127.0.0.1:{localPort}");
            }
            catch (Exception ex)
            {
                // No detener el arranque de la aplicación por errores en el túnel SSH.
                _lastError = ex.Message;
                Console.WriteLine($"Error iniciando túnel SSH: {ex.Message}");
                _forwardedPort = null;
                _sshClient = null;
            }
        }

        /// <summary>
        /// Valida si el túnel SSH está activo intentando conectar al puerto local
        /// que se configuró para el reenvío. Retorna true si el puerto responde.
        /// </summary>
        public bool Validate(int timeoutMs = 1000)
        {
            try
            {
                _lastError = null;
                if (_localPort == 0)
                {
                    _lastError = "Local port not configured";
                    return false;
                }
                using var client = new TcpClient();
                var task = client.ConnectAsync("127.0.0.1", (int)_localPort);
                var completed = Task.WhenAny(task, Task.Delay(timeoutMs)).GetAwaiter().GetResult();
                if (!task.IsCompleted)
                {
                    _lastError = $"Timeout connecting to 127.0.0.1:{_localPort}";
                    return false;
                }
                if (!client.Connected)
                {
                    _lastError = $"Connection refused to 127.0.0.1:{_localPort}";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Versión async de Validate.
        /// </summary>
        public async Task<bool> ValidateAsync(int timeoutMs = 1000)
        {
            try
            {
                _lastError = null;
                if (_localPort == 0)
                {
                    _lastError = "Local port not configured";
                    return false;
                }
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync("127.0.0.1", (int)_localPort);
                var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs));
                if (completed != connectTask)
                {
                    _lastError = $"Timeout connecting to 127.0.0.1:{_localPort}";
                    return false;
                }
                if (!client.Connected)
                {
                    _lastError = $"Connection refused to 127.0.0.1:{_localPort}";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtiene el último mensaje de error ocurrido en la conexión/validación del túnel SSH.
        /// </summary>
        public string? GetLastError() => _lastError;

        public void Dispose()
        {
            _forwardedPort?.Stop();
            _forwardedPort?.Dispose();

            _sshClient?.Disconnect();
            _sshClient?.Dispose();
        }
    }
}
