// ************************
// Author: Sean Cheung
// Create: 2016/06/08/11:21
// Modified: 2016/06/08/15:02
// ************************


using System.Collections;
using System.Net.Sockets;
using System.Text;
using QFramework;
using UnityEngine;

public class AsyncClient : MonoBehaviour
{
    private ISocketClient mClient;

    public string ServerIp = "10.172.99.20";

    private IEnumerator Start()
    {
        mClient = FlexiSocket.Create(ServerIp, 1366, Protocols.BodyLengthPrefix); //ipv6
        
        yield return QWait.ForSeconds(1.0f);
        // wait for server to startup since bot server and clients are in the same scene

        using (var connect = mClient.ConnectAsync())
        {
            yield return connect;
            if (!connect.IsSuccessful)
            {
                Debug.LogException(connect.Exception);
                yield break;
            }
            Debug.Log("Connected", this);
        }

        while (mClient.IsConnected)
        {
            using (var receive = mClient.ReceiveAsync())
            {
                yield return receive;

                if (!receive.IsSuccessful)
                {
                    if (receive.Exception != null)
                        Debug.LogException(receive.Exception);
                    if (receive.Error != SocketError.Success)
                        Debug.LogError(receive.Error);
                    mClient.Close();
                    yield break;
                }

                Debug.Log("Client received: " + Encoding.UTF8.GetString(receive.Data), this);
            }

            var send = mClient.SendAsync("Hey I've got your message");
            yield return send;
            if (!send.IsSuccessful)
            {
                if (send.Exception != null)
                    Debug.LogException(send.Exception);
                if (send.Error != SocketError.Success)
                    Debug.LogError(send.Error);
                mClient.Close();
                yield break;
            }
            Debug.Log("Message sent", this);
            GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        }
    }

    private void OnDestroy()
    {
        mClient.Close();
        mClient = null;
    }

    private void Reset()
    {
        name = "AsyncClient";
    }
}