using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;

namespace SCADAStationNetFrameWork
{
    public delegate void ClientConnectionEventHandler(string clientId);
    public delegate void ClientNameChangedEventHandler(string clientId, string newName);
    //public delegate void ClientGroupEventHandler(string clientId, string groupName);

    //public delegate void MessageReceivedEventHandler(string senderClientId, string message);
    public delegate void WriteTagEventHandler(int tagid, object value);
    public delegate void GetAlarmValueEventHandler(TagInfo taginfo, object value);
    public delegate void GetTrendPointsEventHandler(int trendsettingid);
    public delegate void GetTrendValueByDateTimeEventHandler(int trendsettingid, DateTime begintime, DateTime endtime);
    public delegate void AcknowledgeAlarmPointEventHandler(int alarmpointId);


    public class SCADAHub : Hub
    {
        static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

        public static event ClientConnectionEventHandler ClientConnected;
        public static event ClientConnectionEventHandler ClientDisconnected;
        public static event WriteTagEventHandler ClientWriteTag;
        public static event GetAlarmValueEventHandler  ClientGetAlarmValue;
        public static event AcknowledgeAlarmPointEventHandler AcknowledgeAlarmPoint;
        public static event GetTrendPointsEventHandler ClientGetTrendPoints;
        public static event ClientNameChangedEventHandler ClientNameChanged;
        //
        //public static event ClientGroupEventHandler ClientJoinedToGroup;
        //public static event ClientGroupEventHandler ClientLeftGroup;
        //
        //public static event MessageReceivedEventHandler MessageReceived;

        public static void ClearState()
        {
            _users.Clear();
        }

        //Called when a client is connected
        public override Task OnConnected()
        {
            _users.TryAdd(Context.ConnectionId, Context.ConnectionId);

            ClientConnected?.Invoke(Context.ConnectionId);

            return base.OnConnected();
        }

        //Called when a client is disconnected
        public override Task OnDisconnected(bool stopCalled)
        {
            string userName;
            _users.TryRemove(Context.ConnectionId, out userName);

            ClientDisconnected?.Invoke(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public void WriteTag(int tagid, object value)
        {
            // string userName;
            // _users.TryRemove(Context.ConnectionId, out userName);

            ClientWriteTag?.Invoke(tagid, value);

        }

        public void AckAlarmPoint(int alarmId)
        {
            // string userName;
            // _users.TryRemove(Context.ConnectionId, out userName);

            AcknowledgeAlarmPoint?.Invoke(alarmId);

        }

        public void GetTrendPoints(int trendsettingid)
        {
            // string userName;
            // _users.TryRemove(Context.ConnectionId, out userName);

            ClientGetTrendPoints?.Invoke(trendsettingid);

        }
        public void SetUserName(string userName)
        {
            _users[Context.ConnectionId] = userName;

            ClientNameChanged?.Invoke(Context.ConnectionId, userName);
        }
        #region Client Methods



        //public async Task JoinGroup(string groupName)
        //{
        //    await Groups.Add(Context.ConnectionId, groupName);

        //    ClientJoinedToGroup?.Invoke(Context.ConnectionId, groupName);
        //}

        //public async Task LeaveGroup(string groupName)
        //{
        //    await Groups.Remove(Context.ConnectionId, groupName);

        //    ClientLeftGroup?.Invoke(Context.ConnectionId, groupName);
        //}

        //public void Send(string msg)
        //{
        //    Clients.All.addMessage(_users[Context.ConnectionId], msg);

        //    MessageReceived?.Invoke(Context.ConnectionId, msg);
        //}

        #endregion        
    }
}
