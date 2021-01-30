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
        PlayerID,
        Pong,
        PlayerReady,        // int=playerID
        MovementInput,      // vector2
        ShootInput,         // 
        QuitGame
    }


    public enum ServerMessage : byte
    {
        Ping,
        Disconnection,          // byte=reason
        GameStart,              //
        GameOver,               // byte=winner
        TurnStart,              //
        TurnEnd,                //
        ScoreUpdate,            // float=score
        PlayerPositions,      // Vector3 array?
        PlayerRotation,       // Vector3 array?
        ProjectilePositions,  // Vector3 array?
        ProjectileImpacts,    // Vector3 array?
        PlayerTakesDamage     // int=ReceiverPlayerID, float=damage, int=DealerPlayerID
        //Explosion,            // Vector3?
    }

    public static class NetworkMessage
    {
        #region Server

        public static void Send(ServerMessage serverMessageType, NativeArray<byte> additionalData, ServerBehaviour sender, NetworkConnection receiver)
        {
            var writer = sender.m_Driver.BeginSend(receiver);
            writer.WriteByte((byte)serverMessageType);
            Debug.Log("Send: " + serverMessageType.ToString() + " to " + receiver.InternalId);
            switch (serverMessageType)
            {
                case ServerMessage.Ping:
                case ServerMessage.GameStart:
                case ServerMessage.TurnStart:
                case ServerMessage.TurnEnd:
                default:
                    break;
                case ServerMessage.Disconnection:
                case ServerMessage.GameOver:
                case ServerMessage.ScoreUpdate:
                case ServerMessage.PlayerPositions:
                case ServerMessage.PlayerRotation:
                case ServerMessage.ProjectilePositions:
                case ServerMessage.ProjectileImpacts:
                case ServerMessage.PlayerTakesDamage:
                    writer.WriteBytes(additionalData);
                    break;
            }

            sender.m_Driver.EndSend(writer);
        }

        public static void SendAll(ServerMessage serverMessageType, NativeArray<byte> additionalData, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            for (int i = 0; i < receivers.Length; i++)
            {
                Send(serverMessageType, additionalData, sender, receivers[i]);
            }
        }

        // Server: specialized Send functions
        public static void SendScore(int score, ServerBehaviour sender, NetworkConnection receiver)
        {
            NativeArray<byte> scoreBytes = new NativeArray<byte>();
            scoreBytes.CopyFrom(BitConverter.GetBytes(score));
            Send(ServerMessage.ScoreUpdate, scoreBytes, sender, receiver);
        }

        public static void SendPlayerPosition(int playerID, Vector3 position, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            NativeArray<byte> data = new NativeArray<byte>(BitConverter.GetBytes(playerID), Allocator.None);
            data.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(position));
            SendAll(
                ServerMessage.PlayerPositions,
                data,
                sender,
                receivers
                );
        }

        public static void SendPlayerRotation(int playerID, Vector3 rotation, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            NativeArray<byte> rotationInBytes = new NativeArray<byte>(BitConverter.GetBytes(playerID), Allocator.None);
            rotationInBytes.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(rotation));
            SendAll(ServerMessage.PlayerRotation,
                NetworkMessageUtilities.Vector3ToByteNativeArray(rotation),
                sender,
                receivers);
        }

        public static void SendProjectilePositions(List<Vector3> positions, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            //NativeArray<byte> positionsInBytes = new NativeArray<byte>(BitConverter.GetBytes(positions.Count), Allocator.None);
            //positionsInBytes.CopyFrom(NetworkMessageUtilities.Vector3ListToByteNativeArray(positions));
            SendAll(ServerMessage.PlayerRotation,
                NetworkMessageUtilities.Vector3ListToByteNativeArray(positions),
                sender,
                receivers);
        }

        public static void SendProjectileImpact(Vector3 position, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            //NativeArray<byte> data = new NativeArray<byte>();
            //data.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(position));
            SendAll(ServerMessage.ProjectileImpacts,
                NetworkMessageUtilities.Vector3ToByteNativeArray(position),
                sender,
                receivers);
        }

        public static void SendPlayerTakesDamage()
        {

        }

        public static void Read(ServerBehaviour reader, int connectionID, DataStreamReader stream)
        {
            ClientMessage clientMessageType = (ClientMessage)stream.ReadByte();
            Debug.Log(reader.name + " got message of type " + clientMessageType.ToString());

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                    reader.ReadPong(connectionID);
                    break;
                case ClientMessage.PlayerReady:
                    reader.PlayerReady(connectionID);
                    break;
                case ClientMessage.MovementInput:
                    reader.ReadMovementInput(ExtractMovementInput(ref stream), connectionID);
                    break;
                case ClientMessage.ShootInput:
                    reader.ShootInput(connectionID);
                    break;
                case ClientMessage.QuitGame:
                    reader.ClientQuit(connectionID);
                    break;
                case ClientMessage.PlayerID:
                    reader.AssignPlayerIDToConnection(connectionID, stream.ReadInt());
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Client

        public static void Send(ClientMessage clientMessageType, ClientBehaviour sender, NativeArray<byte> AdditionalData)
        {
            DataStreamWriter writer = sender.m_Driver.BeginSend(sender.m_Connection);
            writer.WriteByte((byte)clientMessageType);

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                case ClientMessage.QuitGame:
                default:
                    break;
                case ClientMessage.MovementInput:
                    WriteMovementInput(ref writer, sender.m_Driver, sender.m_Connection, sender.MovementInput);
                    break;
                case ClientMessage.PlayerReady:
                case ClientMessage.ShootInput:
                    writer.WriteBytes(AdditionalData);
                    break;
                case ClientMessage.PlayerID:
                    writer.WriteInt(sender.clientInfo.id);
                    break;
            }
            sender.m_Driver.EndSend(writer);
        }

        public static void Read(DataStreamReader stream, ClientBehaviour reader)
        {
            ServerMessage serverMessageType = (ServerMessage)stream.ReadByte();
            Debug.Log(reader.name + " got message of type " + serverMessageType.ToString());
            switch (serverMessageType)
            {
                case ServerMessage.Ping:
                    SendPing(reader.m_Driver, reader.m_Connection);
                    break;
                case ServerMessage.Disconnection:
                    reader.Disconnect((DisconnectionReason)stream.ReadByte());
                    break;
                case ServerMessage.GameStart:
                    reader.GameStart();
                    break;
                case ServerMessage.GameOver:
                    reader.GameOver(Convert.ToInt32(stream.ReadByte()));
                    break;
                case ServerMessage.TurnStart:
                    reader.TurnStart();
                    break;
                case ServerMessage.TurnEnd:
                    reader.TurnEnd();
                    break;
                case ServerMessage.ScoreUpdate:
                    NativeArray<byte> bytes = new NativeArray<byte>();
                    stream.ReadBytes(bytes);
                    byte[] byteArray = new byte[4];
                    bytes.CopyTo(byteArray);

                    int score = BitConverter.ToInt32(byteArray, 0);
                    reader.ScoreUpdate(score);
                    break;
                case ServerMessage.PlayerPositions:
                    NativeArray<byte> data = new NativeArray<byte>();
                    stream.ReadBytes(data);
                    reader.UpdatePlayerPositions(NetworkMessageUtilities.ByteNativeArrayToVector3List(data));
                    break;
                case ServerMessage.PlayerRotation:
                    break;
                case ServerMessage.ProjectilePositions:
                    break;
                case ServerMessage.ProjectileImpacts:
                    break;
                case ServerMessage.PlayerTakesDamage:
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

        public static void WriteMovementInput(ref DataStreamWriter writer, NetworkDriver driver, NetworkConnection connection, Vector2 input)
        {
            writer.WriteFloat(input.x);
            writer.WriteFloat(input.y);
        }

        public static Vector2 ExtractMovementInput(ref DataStreamReader stream)
        {
            Vector2 input;
            input.x = stream.ReadFloat();
            input.y = stream.ReadFloat();
            return input;
        }
        #endregion
    }
}