using System.Net.WebSockets;
using System.Text;

namespace FasTnT.Host.Extensions;

public static class WebSocketExtensions
{
    public static async Task SendAsync(this WebSocket webSocket, string content, CancellationToken cancellationToken)
    {
        var responseByteArray = Encoding.UTF8.GetBytes(content);

        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    public static async Task WaitForCompletion(this WebSocket webSocket, Action callback, CancellationToken cancellationToken)
    {
        var arraySegment = new ArraySegment<byte>(new byte[2 * 1024]);

        while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            try
            {
                await webSocket.ReceiveAsync(arraySegment, cancellationToken);

                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                }
            }
            catch
            {
                // The loop will stop if the server is shutdown or socket closed
            }
        }

        callback();
    }
}
