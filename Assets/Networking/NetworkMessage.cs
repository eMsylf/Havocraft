using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace BobJeltes.Networking
{
    public enum DisconnectionReason : byte
    {
        Unknown,
        // Client-side
        ManualClientDisconnection,
        ClientDestroyed,
        Reconnection,
        Timeout,
        // Server-side
        ServerStopped
    }

    public enum ClientMessage : byte
    {
        Pong,
        PlayerID,
        PlayerReady,        // int=playerID
        MovementInput,      // vector2
        ShootInput         // 
        //QuitGame
    }


    public enum ServerMessage : byte
    {
        Ping,
        ConnectionID,
        //Disconnection,          // byte=reason
        GameStart,              // int=connectionID
        GameOver,               // byte=winner
        TurnStart,              //
        TurnEnd,                //
        ScoreUpdate,            // float=score
        PlayerPositions,      // Vector3 array?
        PlayerRotations,       // Vector3 array?
        ProjectilePositions,  // Vector3 array?
        //ProjectileImpact,    // Vector3 array?
        PlayerTakesDamage     // int=ReceiverPlayerID, int=DealerPlayerID, float=damage
        //Explosion,            // Vector3?
    }

    public static class NetworkMessage
    {
        #region Server

        public static void Send(ServerMessage serverMessageType, ServerBehaviour sender, NetworkConnection receiver)
        {
            var writer = sender.m_Driver.BeginSend(receiver);
            writer.WriteByte((byte)serverMessageType);
            //Debug.Log("Send: " + serverMessageType.ToString() + " to " + receiver.InternalId);
            switch (serverMessageType)
            {
                case ServerMessage.Ping:
                case ServerMessage.TurnStart:
                case ServerMessage.TurnEnd:
                default:
                    break;
                case ServerMessage.ConnectionID:
                    WriteConnectionID(sender, ref writer, receiver);
                    break;
                case ServerMessage.GameStart:
                    writer.WriteInt(sender.m_Connections.Length);
                    break;
                case ServerMessage.GameOver:
                    WriteGameOver(sender, ref writer);
                    break;
                case ServerMessage.ScoreUpdate:
                    WriteScoreUpdate(sender, ref writer);
                    break;
                case ServerMessage.PlayerTakesDamage:
                    WritePlayerTakesDamage(sender, ref writer);
                    break;
                case ServerMessage.PlayerPositions:
                    WriteVector3ListToStream(sender.playerPositions, ref writer);
                    break;
                case ServerMessage.PlayerRotations:
                    WriteVector3ListToStream(sender.playerRotations, ref writer);
                    break;
                case ServerMessage.ProjectilePositions:
                    WriteVector3ListToStream(sender.projectilePositions, ref writer);
                    break;
                //case ServerMessage.ProjectileImpact:
                //    break;
            }

            sender.m_Driver.EndSend(writer);
        }

        public static void SendAll(ServerMessage serverMessageType, ServerBehaviour sender)
        {
            if (!sender.m_Connections.IsCreated)
                return;
            for (int i = 0; i < sender.m_Connections.Length; i++)
            {
                Send(serverMessageType, sender, sender.m_Connections[i]);
            }
        }

        // Server: specialized Send functions
        public static void WriteConnectionID(ServerBehaviour sender, ref DataStreamWriter stream, NetworkConnection receiver)
        {
            stream.WriteInt(receiver.InternalId);
        }

        public static void WriteGameOver(ServerBehaviour sender, ref DataStreamWriter writer)
        {
            writer.WriteInt(sender.PlayerIDWinner);
            writer.WriteInt(sender.PlayerIDSecond);
            writer.WriteInt(sender.WinnerScore);
        }

        public static void WriteScoreUpdate(ServerBehaviour sender, ref DataStreamWriter writer)
        {
            writer.WriteFloat(sender.latestDamageData.m_damage);
        }

        private static void WritePlayerTakesDamage(ServerBehaviour sender, ref DataStreamWriter writer)
        {
            writer.WriteInt(sender.latestDamageData.m_receiver.ID);
            writer.WriteInt(sender.latestDamageData.m_dealer.ID);
            writer.WriteFloat(sender.latestDamageData.m_damage);
        }

        public static void WriteVector3ListToStream(List<Vector3> vector3s, ref DataStreamWriter writer)
        {
            writer.WriteInt(vector3s.Count);
            foreach (Vector3 vector3 in vector3s)
            {
                writer.WriteFloat(vector3.x);
                writer.WriteFloat(vector3.y);
                writer.WriteFloat(vector3.z);
            }
        }

        public static void Read(ServerBehaviour reader, int connectionID, DataStreamReader stream)
        {
            ClientMessage clientMessageType = (ClientMessage)stream.ReadByte();
            //Debug.Log(reader.name + " got message of type " + clientMessageType.ToString());

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                    reader.ReadPong(connectionID);
                    break;
                case ClientMessage.PlayerReady:
                    reader.PlayerReady(connectionID);
                    break;
                case ClientMessage.MovementInput:
                    reader.ReadMovementInput(ExtractVector2(ref stream), connectionID);
                    break;
                //case ClientMessage.ShootInput:
                //    reader.ShootInput(connectionID, Convert.ToBoolean(stream.ReadByte())); 
                //    break;
                case ClientMessage.PlayerID:
                    int val = stream.ReadInt();
                    reader.AssignPlayerIDToConnection(connectionID, val);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Client

        public static void Send(ClientMessage clientMessageType, ClientBehaviour sender)
        {
            DataStreamWriter writer = sender.m_Driver.BeginSend(sender.m_Connection);
            writer.WriteByte((byte)clientMessageType);
            Debug.LogError("Player_id: " + ClientBehaviour.player_id);

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                case ClientMessage.PlayerReady:
                default:
                    break;
                case ClientMessage.MovementInput:
                    WriteVector2(ref writer, sender.MovementInput);
                    break;
                //case ClientMessage.ShootInput:
                //    writer.WriteByte(Convert.ToByte(sender.IsShooting));
                //    break;
                case ClientMessage.PlayerID:
                    writer.WriteInt(ClientBehaviour.player_id);
                    break;
            }
            sender.m_Driver.EndSend(writer);
            //AdditionalData.Dispose()
        }

        public static void Read(DataStreamReader stream, ClientBehaviour reader)
        {
            ServerMessage serverMessageType = (ServerMessage)stream.ReadByte();
            Debug.Log(reader.name + " got message of type " + serverMessageType.ToString());
            switch (serverMessageType)
            {
                case ServerMessage.ConnectionID:
                    reader.SaveConnectionID(stream.ReadInt());
                    break;
                case ServerMessage.Ping:
                    SendPing(reader.m_Driver, reader.m_Connection);
                    break;
                //case ServerMessage.Disconnection:
                //    reader.Disconnect((DisconnectionReason)stream.ReadByte());
                    //break;
                case ServerMessage.GameStart:
                    reader.GameStart(stream.ReadInt());
                    break;
                case ServerMessage.GameOver:
                    reader.GameOver(ref stream);
                    break;
                case ServerMessage.TurnStart:
                    reader.TurnStart();
                    break;
                case ServerMessage.TurnEnd:
                    reader.TurnEnd();
                    break;
                case ServerMessage.ScoreUpdate:
                    int score = (int)stream.ReadFloat();
                    reader.ScoreUpdate(score);
                    break;
                case ServerMessage.PlayerPositions:
                    reader.UpdatePlayerPositions(ExtractVector3List(ref stream));
                    break;
                case ServerMessage.PlayerRotations:
                    reader.UpdatePlayerRotations(ExtractVector3List(ref stream));
                    break;
                case ServerMessage.ProjectilePositions:
                    reader.UpdateProjectilePositions(ExtractVector3List(ref stream));
                    break;
                case ServerMessage.PlayerTakesDamage:
                    reader.PlayerTakesDamage(ref stream);
                    break;
                default:
                    break;
            }
        }

        public static void SendPing(NetworkDriver driver, NetworkConnection connection)
        {
            var writer = driver.BeginSend(connection);
            writer.WriteByte(0);
            driver.EndSend(writer);
        }

        public static void WriteVector2(ref DataStreamWriter writer, Vector2 input)
        {
            writer.WriteFloat(input.x);
            writer.WriteFloat(input.y);
        }

        public static Vector2 ExtractVector2(ref DataStreamReader stream)
        {
            Vector2 input;
            input.x = stream.ReadFloat();
            input.y = stream.ReadFloat();
            return input;
        }

        public static List<Vector3> ExtractVector3List(ref DataStreamReader stream)
        {
            int vector3Count = stream.ReadInt();
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < vector3Count; i++)
            {
                Vector3 newVector3 = new Vector3();
                newVector3.x = stream.ReadFloat();
                newVector3.y = stream.ReadFloat();
                newVector3.z = stream.ReadFloat();
                vector3s.Add(newVector3);
            }
            Debug.Log("Extracted " + vector3s.Count + " vector3s");
            return vector3s;
        }
        #endregion
    }
}