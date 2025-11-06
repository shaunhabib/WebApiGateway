using ApiGateway.Config;
using Microsoft.Extensions.Options;
using PEPRPC;
using PEPSignal;
using System.Diagnostics;

namespace ApiGateway.Services
{
    public class AuthService : GatewayService
    {
        private readonly EventLog _eventLog;
        private const string SourceName = "AuthService";
        private const string LogName = "Application";
        public AuthService(PEPSignalR signalService, PEPGRPC rpcService, IOptions<GatewayConfig> options) : base(signalService, rpcService, options)
        {
            // Ensure Event Source exists
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                _eventLog = new EventLog(LogName)
                {
                    Source = SourceName
                };

                _eventLog.WriteEntry("AuthService initialized successfully.", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Failed to initialize EventLog: {ex.Message}");
            }
        }

        public override void Process(string message)
        {
            try
            {
                string info = $"Auth Service received data: {message}";
                Console.WriteLine(info);

                // Log to Windows Event Viewer
                _eventLog?.WriteEntry(info, EventLogEntryType.Information);

                SendToSignalR("Notify", "Response", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception: {ex.Message}");

                // Log to Windows Event Viewer as Error
                try
                {
                    _eventLog?.WriteEntry($"Exception: {ex}", EventLogEntryType.Error);
                }
                catch
                {
                    // Ignore EventLog failures
                }
            }
        }
    }
}
