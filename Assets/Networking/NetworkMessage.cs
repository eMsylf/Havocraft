using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace BobJeltes
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
        PlayerReady,
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
        TurnStart,              // int=PlayerID
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
        // Server
        public static void Send(ServerMessage serverMessageType, NativeArray<byte> additionalData, ServerBehaviour sender, NetworkConnection receiver)
        {
            var writer = sender.m_Driver.BeginSend(receiver);
            writer.WriteByte((byte)serverMessageType);
            Debug.Log("Send: " + serverMessageType.ToString() + " to " + receiver.InternalId);
            switch (serverMessageType)
            {
                case ServerMessage.Ping:
                    sender.m_Driver.EndSend(writer);
                    break;
                case ServerMessage.Disconnection:
                    break;
                case ServerMessage.GameStart:
                    sender.m_Driver.EndSend(writer);
                    break;
                case ServerMessage.GameOver:
                    break;
                case ServerMessage.TurnStart:
                    break;
                case ServerMessage.TurnEnd:
                    break;
                case ServerMessage.ScoreUpdate:
                    writer.WriteBytes(additionalData);
                    sender.m_Driver.EndSend(writer);
                    break;
                case ServerMessage.PlayerPositions:
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

        public static void SendAll(ServerMessage serverMessageType, NativeArray<byte> additionalData, ServerBehaviour sender, NativeList<NetworkConnection> receivers)
        {
            for (int i = 0; i < receivers.Length; i++)
            {
                Send(serverMessageType, additionalData, sender, receivers[i]);
            }
        }

        public static void Read(DataStreamReader stream, int playerID, ServerBehaviour reader)
        {
            ClientMessage clientMessageType = (ClientMessage)stream.ReadByte();
            Debug.Log(reader.name + " got message of type " + clientMessageType.ToString());

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                    reader.ReadPong(playerID);
                    break;
                case ClientMessage.PlayerReady:
                    reader.PlayerReady(playerID);
                    break;
                case ClientMessage.MovementInput:
                    float x = stream.ReadFloat();
                    float y = stream.ReadFloat();
                    reader.ReadMovementInput(x, y, playerID);
                    break;
                case ClientMessage.ShootInput:
                    reader.ShootInput(playerID);
                    break;
                case ClientMessage.QuitGame:
                    reader.ClientQuit(playerID);
                    break;
                default:
                    break;
            }
        }

        // Server: specialized functions
        public static void SendScore(int score, ServerBehaviour sender, NetworkConnection receiver)
        {
            NativeArray<byte> scoreBytes = new NativeArray<byte>();
            scoreBytes.CopyFrom(BitConverter.GetBytes(score));
            Send(ServerMessage.ScoreUpdate, scoreBytes, sender, receiver);
        }

        // Client
        public static void Send(ClientMessage clientMessageType, ClientBehaviour sender, byte[] AdditionalData = null)
        {
            DataStreamWriter writer = sender.m_Driver.BeginSend(sender.m_Connection);
            writer.WriteByte((byte)clientMessageType);

            switch (clientMessageType)
            {
                case ClientMessage.Pong:
                    sender.m_Driver.EndSend(writer);
                    break;
                case ClientMessage.PlayerReady:
                    sender.m_Driver.EndSend(writer);
                    break;
                case ClientMessage.MovementInput:
                    SendMovementInput(sender.m_Driver, sender.m_Connection, sender.MovementInput);
                    break;
                case ClientMessage.ShootInput:
                    // If the isShooting variable is set, send this
                    NativeArray<byte> bytes = new NativeArray<byte>();
                    bytes.CopyFrom(AdditionalData);
                    writer.WriteBytes(bytes);
                    sender.m_Driver.EndSend(writer);
                    break;
                case ClientMessage.QuitGame:
                    sender.m_Driver.EndSend(writer);
                    break;
                default:
                    break;
            }
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
                    reader.GameOver(Convert.ToBoolean(stream.ReadByte()));
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

        public static void SendMovementInput(NetworkDriver driver, NetworkConnection connection, Vector2 input)
        {
            var writer = driver.BeginSend(connection);
            writer.WriteByte((byte)ClientMessage.MovementInput);
            writer.WriteFloat(input.x);
            writer.WriteFloat(input.y);
            driver.EndSend(writer);
        }

        public static void ReadMovementInput(Vector2 input)
        {

        }

        public static void ReadGameStart()
        {

        }

        public static void ReadGameEnd()
        {

        }

        public static void ReadTurnStart()
        {

        }

        public static void ReadTurnEnd()
        {

        }
    }
}