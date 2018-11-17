﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TSFramework
{
    public static class Utils
    {
        public static Vector3 V3FromFloats(float[] Floats)
        {
            var vector = new Vector3(Floats[0], Floats[1], Floats[2]);
            return vector;
        }

        // Scan the give reader for a series of 3 floats that are in the given range, returns a list of all triples that match
        // Walks the file 4 bytes at a time
        public static List<Tuple<int,float[]>> ScanForVector3(BinaryReader R, float FloatMin, float FloatMax, int StartOffset = -1, int EndOffset = -1)
        {
            const int NUM_FLOATS = 3;
            var vec3s            = new List<Tuple<int, float[]>>();

            if (StartOffset != -1) { R.BaseStream.Seek(StartOffset, SeekOrigin.Begin); }

            for (int aye = 0; aye < 100000; aye++)
            {
                var pos       = (int)R.BaseStream.Position;
                var bytesLeft = R.BaseStream.Length - pos;
                var vec3      = new float[NUM_FLOATS];
                var canRead   = EndOffset != -1 ? ((EndOffset - pos) > vec3.Length) : (bytesLeft > vec3.Length);

                if (!canRead) { break; }

                var bytes = R.ReadBytes(sizeof(float) * NUM_FLOATS);

                Buffer.BlockCopy(bytes, 0, vec3, 0, bytes.Length);

                bool allInRange = true;
                for (int i = 0; i < NUM_FLOATS; i++)
                {
                    var f = vec3[i];
                    var asString = $"{f}";
                    if (f < FloatMin || f > FloatMax || asString.ToUpper().Contains("E"))
                    {
                        allInRange = false;
                    }
                }

                if (allInRange)
                {
                    vec3s.Add(Tuple.Create(pos, vec3));
                }
                else
                {
                    R.BaseStream.Seek(-8, SeekOrigin.Current);
                }
            }


            return vec3s;
        }

        public static Vector3 RadRotationToDeg(Vector3 Rot)
        {
            var newRotation = new Vector3()
            {
                x = Rot.x * Mathf.Rad2Deg,
                y = Rot.y * Mathf.Rad2Deg,
                z = Rot.z * Mathf.Rad2Deg
            };

            return newRotation;
        }
    }
}