namespace Net
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class OctetsBuffer : ICloneable, IComparable
    {
        private byte[] _buffer;
        private int position;
        private static Encoding DEFAULT_CHARSET = Encoding.UTF8;
        private static readonly int DEFAULT_SIZE = 0x80;

        public OctetsBuffer()
        {
            this.reserve(DEFAULT_SIZE);
        }

        public OctetsBuffer(OctetsBuffer rhs)
        {
            this.replace(rhs);
        }

        public OctetsBuffer(int size)
        {
            this.reserve(size);
        }

        public OctetsBuffer(byte[] rhs)
        {
            this.replace(rhs);
        }

        public OctetsBuffer(string str)
        {
            this.replace(DEFAULT_CHARSET.GetBytes(str));
        }

        private OctetsBuffer(byte[] bytes, int length)
        {
            this._buffer = bytes;
            this.position = length;
        }

        public OctetsBuffer(string str, Encoding encoding)
        {
            this.replace(encoding.GetBytes(str));
        }

        public OctetsBuffer(byte[] rhs, int pos, int size)
        {
            this.replace(rhs, pos, size);
        }

        public OctetsBuffer(OctetsBuffer rhs, int pos, int size)
        {
            this.replace(rhs, pos, size);
        }

        public byte[] array()
        {
            return this.array(0, -1);
        }

        public byte[] array(int offset, int len = -1)
        {
            if (len < 0)
            {
                len = this.size() - offset;
            }
            byte[] destinationArray = new byte[len];
            Array.Copy(this._buffer, offset, destinationArray, 0, len);
            return destinationArray;
        }

        public byte[] buffer()
        {
            return this._buffer;
        }

        public int capacity()
        {
            return this._buffer.Length;
        }

        public OctetsBuffer clear()
        {
            this.position = 0;
            return this;
        }

        public object Clone()
        {
            return new OctetsBuffer(this);
        }

        public int CompareTo(OctetsBuffer rhs)
        {
            int num = this.position - rhs.position;
            if (num != 0)
            {
                return num;
            }
            byte[] buffer = this._buffer;
            byte[] buffer2 = rhs._buffer;
            for (int i = 0; i < this.position; i++)
            {
                int num3 = buffer[i] - buffer2[i];
                if (num3 != 0)
                {
                    return num3;
                }
            }
            return 0;
        }

        public int CompareTo(object o)
        {
            return this.CompareTo((OctetsBuffer) o);
        }

        public void dump()
        {
            for (int i = 0; i < this.size(); i++)
            {
                Console.Write(this._buffer[i] + " ");
            }
            Console.WriteLine(string.Empty);
        }

        public override bool Equals(object o)
        {
            return ((this == o) || (this.CompareTo(o) == 0));
        }

        public OctetsBuffer erase(int from, int to)
        {
            Array.Copy(this._buffer, to, this._buffer, from, this.position - to);
            this.position -= to - from;
            return this;
        }

        public byte getByte(int pos)
        {
            return this._buffer[pos];
        }

        public byte[] getBytes()
        {
            byte[] destinationArray = new byte[this.position];
            Array.Copy(this._buffer, 0, destinationArray, 0, this.position);
            return destinationArray;
        }

        public override int GetHashCode()
        {
            if (this._buffer == null)
            {
                return 0;
            }
            int num = 1;
            for (int i = 0; i < this.position; i++)
            {
                num = (0x1f * num) + this._buffer[i];
            }
            return num;
        }

        public string getString()
        {
            return this.getString(0, this.position);
        }

        public string getString(string encoding)
        {
            return this.getString(0, this.position, encoding);
        }

        public string getString(Encoding encoding)
        {
            return this.getString(0, this.position, encoding);
        }

        public string getString(int pos, int len)
        {
            return DEFAULT_CHARSET.GetString(this._buffer, pos, len);
        }

        public string getString(int pos, int len, string encoding)
        {
            string str;
            try
            {
                str = Encoding.GetEncoding(encoding).GetString(this._buffer, pos, len);
            }
            catch (SystemException exception)
            {
                throw new SystemException(exception.Message);
            }
            return str;
        }

        public string getString(int pos, int len, Encoding encoding)
        {
            string str;
            try
            {
                str = encoding.GetString(this._buffer, pos, len);
            }
            catch (SystemException exception)
            {
                throw new SystemException(exception.Message);
            }
            return str;
        }

        public string getStringUnicode()
        {
            return this.getString(0, this.position, Encoding.Unicode);
        }

        public OctetsBuffer insert(int from, byte[] data)
        {
            return this.insert(from, data, 0, data.Length);
        }

        public OctetsBuffer insert(int from, OctetsBuffer data)
        {
            return this.insert(from, data._buffer, 0, data.size());
        }

        public OctetsBuffer insert(int from, byte[] data, int pos, int size)
        {
            this.reserve(this.position + size);
            Array.Copy(this._buffer, from, this._buffer, from + size, this.position - from);
            Array.Copy(data, pos, this._buffer, from, size);
            this.position += size;
            return this;
        }

        public OctetsBuffer insert(int from, OctetsBuffer data, int pos, int size)
        {
            return this.insert(from, data._buffer, pos, size);
        }

        public OctetsBuffer push_back(byte data)
        {
            this.reserve(this.position + 1);
            this._buffer[this.position++] = data;
            return this;
        }

        public OctetsBuffer replace(byte[] data)
        {
            return this.replace(data, 0, data.Length);
        }

        public OctetsBuffer replace(OctetsBuffer data)
        {
            return this.replace(data._buffer, 0, data.position);
        }

        public OctetsBuffer replace(byte[] data, int pos, int size)
        {
            this.reserve(size);
            Array.Copy(data, pos, this._buffer, 0, size);
            this.position = size;
            return this;
        }

        public OctetsBuffer replace(OctetsBuffer data, int pos, int size)
        {
            return this.replace(data._buffer, pos, size);
        }

        public void reserve(int size)
        {
            if (this._buffer == null)
            {
                this._buffer = this.roundup(size);
            }
            else if (size > this._buffer.Length)
            {
                byte[] destinationArray = this.roundup(size);
                Array.Copy(this._buffer, 0, destinationArray, 0, this.position);
                this._buffer = destinationArray;
            }
        }

        public OctetsBuffer resize(int size)
        {
            this.reserve(size);
            this.position = size;
            return this;
        }

        private byte[] roundup(int size)
        {
            int num = 0x10;
            while (size > num)
            {
                num = num << 1;
            }
            return new byte[num];
        }

        public void setByte(int pos, byte b)
        {
            this._buffer[pos] = b;
        }

        public static void setDefaultCharset(string name)
        {
            DEFAULT_CHARSET = Encoding.GetEncoding(name);
        }

        public void setString(string str)
        {
            this._buffer = DEFAULT_CHARSET.GetBytes(str);
            this.position = this._buffer.Length;
        }

        public void setStringUnicode(string str)
        {
            this._buffer = Encoding.Unicode.GetBytes(str);
            this.position = this._buffer.Length;
        }

        public int size()
        {
            return this.position;
        }

        public OctetsBuffer swap(OctetsBuffer rhs)
        {
            int count = this.position;
            this.position = rhs.position;
            rhs.position = count;
            byte[] buffer = rhs._buffer;
            rhs._buffer = this._buffer;
            this._buffer = buffer;
            return this;
        }

        public override string ToString()
        {
            return this.getString();
        }

        public static OctetsBuffer wrap(byte[] bytes)
        {
            return wrap(bytes, bytes.Length);
        }

        public static OctetsBuffer wrap(byte[] bytes, int length)
        {
            return new OctetsBuffer(bytes, length);
        }

        public static OctetsBuffer wrap(string str, string encoding)
        {
            OctetsBuffer octets;
            try
            {
                octets = wrap(Encoding.GetEncoding(encoding).GetBytes(str));
            }
            catch (ArgumentException exception)
            {
                throw exception;
            }
            return octets;
        }
    }
}

