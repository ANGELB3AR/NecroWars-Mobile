using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Zindeaxx.SoundSystem.Utils
{
    public static class SoundUtils
    {

        #region Extensions

        public static void Clear(this Transform container)
        {
            if (container != null)
            {
                foreach (Transform t in container)
                {
                    GameObject.Destroy(t.gameObject);
                }
            }
            else
            {
                DebugError("Could not clear Transform. It was NULL");
            }
        }

        public static T GetOrAddComponent<T>(this Component comp) where T : Component
        {
            var a = comp.GetComponent<T>();
            if (a != null)
            {
                return a;
            }

            return comp.gameObject.AddComponent<T>();
        }


        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var a = gameObject.GetComponent<T>();
            if (a != null)
            {
                return a;
            }

            return gameObject.AddComponent<T>();
        }

        public static T GetRandomValue<T>(this IList<T> list)
        {
            return list[RandomRange(0, list.Count)];
        }

        public static T GetWeightedValue<T>(this IList<LuckyListEntry<T>> list)
        {
            List<T> objList = new List<T>();
            foreach (var entry in list)
            {
                for (int i = 0; i < entry.Chance; i++)
                {
                    objList.Add(entry.Item);
                }
            }
            return objList.GetRandomValue();
        }

        #endregion

        #region Randomness

        public static int RandomBoolInt()
        {
            return RandomBool() ? 1 : 0;
        }

        public static bool RandomBool()
        {
            return RandomRange(0, 100) <= 50;
        }

        private static System.Random rand;
        /// <summary>
        /// Returns a value between 1 and 0
        /// </summary>
        public static double Random()
        {
            if (rand == null)
            {
                rand = new System.Random();
            }

            return rand.NextDouble();
        }

        public static int RandomIndex(IList list)
        {
            return RandomRange(0, list.Count);
        }

        private static RNGCryptoServiceProvider randomGen;
        private static Int64 diff;
        public static Int32 RandomRange(Int32 minValue, Int32 maxValue)
        {
            if (randomGen == null)
            {
                randomGen = new RNGCryptoServiceProvider();
            }

            byte[] _uint32Buffer = new byte[4];

            if (minValue > maxValue)
            {
                int oldmin = minValue;
                minValue = maxValue;
                maxValue = oldmin;
            }


            if (minValue == maxValue)
            {
                return minValue;
            }

            diff = maxValue - minValue;
            while (true)
            {
                randomGen.GetBytes(_uint32Buffer);
                UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                Int64 max = (1 + (Int64)UInt32.MaxValue);
                Int64 remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (Int32)(minValue + (rand % diff));
                }
            }
        }

        public static float RandomRange(Vector2 range)
        {
            return RandomRange(range.x, range.y);
        }

        public static int RandomRange(Vector2Int range)
        {
            return RandomRange(range.x, range.y);
        }

        public static float RandomRange(float minValue, float maxValue)
        {
            int max = (int)maxValue * 1000;
            int min = (int)minValue * 1000;
            return ((float)RandomRange(min, max)) / 1000;
        }
        #endregion

        #region Classes

        [Serializable]
        public class LuckyListEntry<T>
        {
            public T Item;
            public int Chance;

            public LuckyListEntry(T item, int chance)
            {
                Item = item;
                Chance = chance;

            }

            public LuckyListEntry()
            {
                Chance = 1;
            }
        }

        #endregion

        #region Editor

        private static bool m_DebuggingEnabled = true;

        /// <summary>
        /// Toggles debugging state. If turned off no more outputs will be made! May increase performance!
        /// </summary>
        /// <param name="newState"></param>
        public static void SetDebuggingState(bool newState)
        {
            m_DebuggingEnabled = newState;
        }

        public static void DebugOutput(string messageText)
        {
            DebugOutput(messageText, MessageType.Normal, null);
        }

        public static void DebugError(string messageText)
        {
            DebugOutput(messageText, MessageType.Error, null);
        }
        public static void DebugWarning(string messageText)
        {
            DebugOutput(messageText, MessageType.Warning, null);
        }

        public static void DebugOutput(string messageText, MessageType messageType, MonoBehaviour sender = null)
        {
            if (m_DebuggingEnabled)
            {
                string finalMessage = messageText;

                if (sender != null)
                {
                    finalMessage = sender.gameObject.name + " - " + sender.name + ": " + messageText;
                }

                switch (messageType)
                {
                    case MessageType.Normal:
                        Debug.Log(finalMessage, sender);
                        break;
                    case MessageType.Warning:
                        Debug.LogWarning(finalMessage, sender);
                        break;
                    case MessageType.Error:
                        Debug.LogError(finalMessage, sender);
                        break;
                    case MessageType.Exception:
                        Debug.LogException(new Exception(messageText), sender);
                        break;
                }
            }
        }



        #endregion

    }
    public enum MessageType
    {
        Normal,
        Error,
        Warning,
        Exception
    }
}