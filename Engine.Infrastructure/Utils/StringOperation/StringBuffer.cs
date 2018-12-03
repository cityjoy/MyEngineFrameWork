using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 对StringBuilder进行加强封装
    /// </summary>

    public class StringBuffer
    {
        private StringBuilder _innerBuilder = new StringBuilder();

        /// <summary>
        /// StringBuilder实例
        /// </summary>
        public StringBuilder InnerBuilder
        {
            get { return _innerBuilder; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            get
            {
                return _innerBuilder.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Remove(int startIndex, int length)
        {
            _innerBuilder.Remove(startIndex, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public void Replace(string oldValue, string newValue)
        {
            _innerBuilder.Replace(oldValue, newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public StringBuffer()
        {
            _innerBuilder = new StringBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public StringBuffer(int capacity)
        {
            _innerBuilder = new StringBuilder(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public StringBuffer(string value)
        {
            _innerBuilder = new StringBuilder(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="maxCapacity"></param>
        public StringBuffer(int capacity, int maxCapacity)
        {
            _innerBuilder = new StringBuilder(capacity, maxCapacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="capacity"></param>
        public StringBuffer(string value, int capacity)
        {
            _innerBuilder = new StringBuilder(value, capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="capacity"></param>
        public StringBuffer(string value, int startIndex, int length, int capacity)
        {
            _innerBuilder = new StringBuilder(value, startIndex, length, capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerBuilder"></param>
        public StringBuffer(StringBuilder innerBuilder)
        {
            _innerBuilder = innerBuilder;
        }

        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, bool value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, byte value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, char value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, char[] value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, decimal value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, double value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, float value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, int value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, long value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, object value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, sbyte value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, short value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, string value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, uint value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, ulong value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }
        /// <summary>
        /// 模拟StringBuilder +操作符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer buffer, ushort value)
        {
            buffer.InnerBuilder.Append(value);

            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InnerBuilder.ToString();
        }
    }
}
