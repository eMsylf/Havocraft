﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace BobJeltes.Networking
{
    public static class NetworkMessageUtilities
    {
        public static NativeArray<byte> Vector3ToByteNativeArray(Vector3 vector3)
        {
            List<byte> bytes = new List<byte>();
            foreach (byte digitByte in BitConverter.GetBytes(vector3.x))
            {
                bytes.Add(digitByte);
            }
            foreach (byte digitByte in BitConverter.GetBytes(vector3.y))
            {
                bytes.Add(digitByte);
            }
            foreach (byte digitByte in BitConverter.GetBytes(vector3.z))
            {
                bytes.Add(digitByte);
            }
            NativeArray<byte> vector3Bytes = new NativeArray<byte>(bytes.ToArray(), Allocator.Temp);
            return vector3Bytes;
        }

        public static NativeArray<byte> Vector3ListToByteNativeArray(List<Vector3> vector3s)
        {
            List<byte> bytes = new List<byte>();
            foreach (Vector3 vector3 in vector3s)
            {
                foreach (byte digitByte in BitConverter.GetBytes(vector3.x))
                {
                    bytes.Add(digitByte);
                }
                foreach (byte digitByte in BitConverter.GetBytes(vector3.y))
                {
                    bytes.Add(digitByte);
                }
                foreach (byte digitByte in BitConverter.GetBytes(vector3.z))
                {
                    bytes.Add(digitByte);
                }
            }
            NativeArray<byte> vector3Bytes = new NativeArray<byte>(bytes.ToArray(), Allocator.Temp);
            return vector3Bytes;
        }

        //public static Vector3 ByteNativeArrayToVector3(NativeArray<byte> byteNativeArray)
        //{
        //    byte[] byteArray = byteNativeArray.ToArray();

        //    Vector3 newestVector3 = new Vector3();

        //    for (int i = 0; i < 12; i += 4)
        //    {
        //        int currentPositionInVector3 = i % 12;

        //        // Pak bytes van huidige positie, tot en met huidige positie + 4
        //        byte[] singleDigit = new byte[4];
        //        Array.Copy(byteArray, i, singleDigit, 0, 4);

        //        if (currentPositionInVector3 == 0)
        //        {
        //            // Maak nieuwe vector aan
        //            newestVector3 = new Vector3();
        //            // x
        //            newestVector3.x = BitConverter.ToSingle(singleDigit, 0);
        //        }
        //        else if (currentPositionInVector3 == 4)
        //        {
        //            // y
        //            newestVector3.y = BitConverter.ToSingle(singleDigit, 0);
        //        }
        //        else if (currentPositionInVector3 == 8)
        //        {
        //            // z
        //            newestVector3.z = BitConverter.ToSingle(singleDigit, 0);
        //        }
        //    }
        //    return newestVector3;
        //}

        public static List<Vector3> ByteNativeArrayToVector3List(NativeArray<byte> byteNativeArray)
        {
            byte[] byteArray = byteNativeArray.ToArray();

            List<Vector3> vector3s = new List<Vector3>();
            Vector3 newestVector3 = new Vector3();

            // Voor zoveel vector3's als er in de array zitten
            for (int i = 0; i < byteArray.Length; i += sizeof(float))
            {
                int currentPositionInVector3 = i % (sizeof(float)*3);

                // Pak bytes van huidige positie, tot en met huidige positie + sizeof(float)
                byte[] subArray = new byte[sizeof(float)];
                for (int j = 0; j < sizeof(float); j++)
                {
                    subArray[j] = byteArray[i + j];
                }

                if (currentPositionInVector3 == 0)
                {
                    // Maak nieuwe vector aan
                    newestVector3 = new Vector3();
                    // x
                    newestVector3.x = BitConverter.ToSingle(subArray, 0);
                }
                else if (currentPositionInVector3 == sizeof(float))
                {
                    // y
                    newestVector3.y = BitConverter.ToSingle(subArray, 0);
                }
                else if (currentPositionInVector3 == sizeof(float)*2)
                {
                    // z
                    newestVector3.z = BitConverter.ToSingle(subArray, 0);
                    // Voeg nieuwe vector 3 toe aan de lijst
                    vector3s.Add(newestVector3);
                }
            }
            return vector3s;
        }
    }
}