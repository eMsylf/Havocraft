﻿using System;
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
        ShootInput         // 
        //QuitGame
    }


    public enum ServerMessage : byte
    {
        ConnectionID,
        Ping,
        Disconnection,          // byte=reason
        GameStart,              // int=connectionID
        GameOver,               // byte=winner
        TurnStart,              //
        TurnEnd,                //
        ScoreUpdate,            // float=score
        PlayerPositions,      // Vector3 array?
        PlayerRotations,       // Vector3 array?
        ProjectilePositions,  // Vector3 array?
        ProjectileImpact,    // Vector3 array?
        PlayerTakesDamage     // int=ReceiverPlayerID, int=DealerPlayerID, float=damage
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
                case ServerMessage.TurnStart:
                case ServerMessage.TurnEnd:
                default:
                    break;
                case ServerMessage.ConnectionID:
                case ServerMessage.Disconnection:
                case ServerMessage.GameStart:
                case ServerMessage.GameOver:
                case ServerMessage.ScoreUpdate:
                case ServerMessage.PlayerPositions:
                case ServerMessage.PlayerRotations:
                case ServerMessage.ProjectilePositions:
                case ServerMessage.ProjectileImpact:
                case ServerMessage.PlayerTakesDamage:
                    if (!additionalData.IsCreated || additionalData.Length == 0)
                    {
                        Debug.LogError("Required sent additional data was null or empty.");
                        return;
                    }
                    writer.WriteBytes(additionalData);
                    break;
            }

            sender.m_Driver.EndSend(writer);
        }

        public static void SendAll(ServerMessage serverMessageType, NativeArray<byte> additionalData, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            if (!receivers.IsCreated)
                return;
            for (int i = 0; i < receivers.Length; i++)
            {
                Send(serverMessageType, additionalData, sender, receivers[i]);
            }
        }

        // Server: specialized Send functions
        public static void SendConnectionIDs(ServerBehaviour sender)
        {
            for (byte i = 0; i < sender.m_Connections.Length; i++)
            {
                NativeArray<byte> idByte = new NativeArray<byte>(new byte[] { i }, Allocator.Temp);
                Send(ServerMessage.ConnectionID, idByte, sender, sender.m_Connections[i]);
            }
        }

        //public static void SendScore(int score, ServerBehaviour sender, NetworkConnection receiver)
        //{
        //    NativeArray<byte> scoreBytes = new NativeArray<byte>();
        //    scoreBytes.CopyFrom(BitConverter.GetBytes(score));
        //    Send(ServerMessage.ScoreUpdate, scoreBytes, sender, receiver);
        //}

        //public static void SendPlayerPositions(int playerID, Vector3 position, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        //{
        //    NativeArray<byte> data = new NativeArray<byte>(BitConverter.GetBytes(playerID), Allocator.None);
        //    data.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(position));
        //    SendAll(
        //        ServerMessage.PlayerPositions,
        //        data,
        //        sender,
        //        receivers
        //        );
        //}

        //public static void SendPlayerRotations(int playerID, Vector3 rotation, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        //{
        //    NativeArray<byte> rotationInBytes = new NativeArray<byte>(BitConverter.GetBytes(playerID), Allocator.None);
        //    rotationInBytes.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(rotation));
        //    SendAll(ServerMessage.PlayerRotations,
        //        NetworkMessageUtilities.Vector3ToByteNativeArray(rotation),
        //        sender,
        //        receivers);
        //}

        //public static void SendProjectilePositions(List<Vector3> positions, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        //{
        //    //NativeArray<byte> positionsInBytes = new NativeArray<byte>(BitConverter.GetBytes(positions.Count), Allocator.None);
        //    //positionsInBytes.CopyFrom(NetworkMessageUtilities.Vector3ListToByteNativeArray(positions));
        //    SendAll(ServerMessage.ProjectilePositions,
        //        NetworkMessageUtilities.Vector3ListToByteNativeArray(positions),
        //        sender,
        //        receivers);
        //}

        //public static void SendProjectileImpact(Vector3 position, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        //{
        //    //NativeArray<byte> data = new NativeArray<byte>();
        //    //data.CopyFrom(NetworkMessageUtilities.Vector3ToByteNativeArray(position));
        //    SendAll(ServerMessage.ProjectileImpact,
        //        NetworkMessageUtilities.Vector3ToByteNativeArray(position),
        //        sender,
        //        receivers);
        //}

        //public static void SendPlayerTakesDamage(int ReceiverPlayerID, int DealerPlayerID, float damage, ServerBehaviour sender)
        //{
        //    NativeArray<byte> data = new NativeArray<byte>();
        //    data.CopyFrom(BitConverter.GetBytes(ReceiverPlayerID));
        //    data.CopyFrom(BitConverter.GetBytes(DealerPlayerID));
        //    data.CopyFrom(BitConverter.GetBytes(damage));
        //    SendAll(ServerMessage.PlayerTakesDamage,
        //        data,
        //        sender,
        //        sender.m_Connections
        //        );
        //}

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
                    reader.ReadMovementInput(ExtractVector2(ref stream), connectionID);
                    break;
                case ClientMessage.ShootInput:
                    reader.ShootInput(connectionID, Convert.ToBoolean(stream.ReadByte()));
                    break;
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

        public static void Send(ClientMessage clientMessageType, ClientBehaviour sender, NativeArray<byte> AdditionalData)
        {
            DataStreamWriter writer = sender.m_Driver.BeginSend(sender.m_Connection);
            writer.WriteByte((byte)clientMessageType);

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                default:
                    break;
                case ClientMessage.MovementInput:
                    WriteVector2(ref writer, sender.MovementInput);
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
            //AdditionalData.Dispose()
        }

        public static void Read(DataStreamReader stream, ClientBehaviour reader)
        {
            ServerMessage serverMessageType = (ServerMessage)stream.ReadByte();
            Debug.Log(reader.name + " got message of type " + serverMessageType.ToString());
            switch (serverMessageType)
            {
                case ServerMessage.ConnectionID:
                    reader.SaveConnectionID(Convert.ToInt32(stream.ReadByte()));
                    break;
                case ServerMessage.Ping:
                    //SendPing(reader.m_Driver, reader.m_Connection);
                    break;
                case ServerMessage.Disconnection:
                    reader.Disconnect((DisconnectionReason)stream.ReadByte());
                    break;
                case ServerMessage.GameStart:
                    reader.GameStart(Convert.ToInt32(stream.ReadByte()));
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
                    int score = (int)stream.ReadFloat();
                    reader.ScoreUpdate(score);
                    break;
                case ServerMessage.PlayerPositions:
                    NativeArray<byte> positionData = new NativeArray<byte>(stream.Length, Allocator.Temp);
                    Debug.Log("Stream length: " + stream.Length);
                    stream.ReadBytes(positionData);
                    reader.UpdatePlayerPositions(NetworkMessageUtilities.ByteNativeArrayToVector3List(positionData));
                    break;
                case ServerMessage.PlayerRotations:
                    NativeArray<byte> rotationdata = new NativeArray<byte>(stream.Length, Allocator.Temp);
                    stream.ReadBytes(rotationdata);
                    reader.UpdatePlayerRotations(NetworkMessageUtilities.ByteNativeArrayToVector3List(rotationdata));
                    break;
                case ServerMessage.ProjectilePositions:
                    NativeArray<byte> projectilePositions = new NativeArray<byte>(stream.Length, Allocator.Temp);
                    stream.ReadBytes(projectilePositions);
                    reader.UpdateProjectilePositions(NetworkMessageUtilities.ByteNativeArrayToVector3List(projectilePositions));
                    break;
                case ServerMessage.ProjectileImpact:
                    NativeArray<byte> projectileRotations = new NativeArray<byte>(stream.Length, Allocator.Temp);
                    stream.ReadBytes(projectileRotations);
                    reader.SpawnProjectileImpacts(NetworkMessageUtilities.ByteNativeArrayToVector3List(projectileRotations));
                    break;
                case ServerMessage.PlayerTakesDamage:
                    NativeArray<byte> playerDamageData = new NativeArray<byte>(stream.Length, Allocator.Temp);
                    stream.ReadBytes(playerDamageData);
                    reader.PlayerTakesDamage(playerDamageData);
                    break;
                default:
                    break;
            }
        }
        //public static void ReadPlayerTakesDamage(int ReceiverPlayerID, int DealerPlayerID, float damage, ServerBehaviour sender)
        //{
        //    NativeArray<byte> data = new NativeArray<byte>();
        //    data.CopyFrom(BitConverter.GetBytes(ReceiverPlayerID));
        //    data.CopyFrom(BitConverter.GetBytes(DealerPlayerID));
        //    data.CopyFrom(BitConverter.GetBytes(damage));
        //    SendAll(ServerMessage.PlayerTakesDamage,
        //        data,
        //        sender,
        //        sender.m_Connections
        //        );
        //}

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
        #endregion
    }
}