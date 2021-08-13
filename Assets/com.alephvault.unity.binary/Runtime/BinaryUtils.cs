using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlephVault.Unity.Binary
{
    /// <summary>
    ///   Some utilities and shortcuts for this library.
    /// </summary>
    public static class BinaryUtils
    {
        /// <summary>
        ///   Creates a writer from a source array.
        /// </summary>
        /// <param name="source">The source array</param>
        /// <returns>A writer for that array</returns>
        public static System.Tuple<Buffer, Reader> ReaderFor(byte[] source)
        {
            var buffer = new Buffer(source);
            var reader = new Reader(buffer);
            return new System.Tuple<Buffer, Reader>(buffer, reader);
        }

        /// <summary>
        ///   Creates a writer for a target array.
        /// </summary>
        /// <param name="target">The target array</param>
        /// <returns>A writer for thay array</returns>
        public static System.Tuple<Buffer, Writer> WriterFor(byte[] target)
        {
            var buffer = new Buffer(target);
            var writer = new Writer(buffer);
            return new System.Tuple<Buffer, Writer>(buffer, writer);
        }

        /// <summary>
        ///   Dumps an object into a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="target">The target array to serialize the object into</param>
        /// <returns>The count of written bytes</returns>
        public static long Dump(ISerializable obj, byte[] target)
        {
            var bufferAndWriter = WriterFor(target);
            obj.Serialize(new Serializer(bufferAndWriter.Item2));
            return bufferAndWriter.Item1.Position;
        }

        /// <summary>
        ///   Loads an object from a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to load</typeparam>
        /// <param name="obj">The object to hidrate</param>
        /// <param name="source">The source array to de-serialize the object from</param>
        /// <returns>The count of read bytes</returns>
        public static long Load<T>(T obj, byte[] source) where T : ISerializable
        {
            var bufferAndReader = ReaderFor(source);
            obj.Serialize(new Serializer(bufferAndReader.Item2));
            return bufferAndReader.Item1.Position;
        }
    }
}
