using System.IO;

namespace Net
{
    using System;
    using System.Text;

    public class OctetsStream : OctetsBuffer
    {
        private int pos;

        public OctetsStream()
        {
        }

        public OctetsStream(OctetsBuffer o)
            : base(o)
        {
        }

        public OctetsStream(int size) : base(size)
        {
        }

        public object Clone()
        {
            return new OctetsStream((OctetsBuffer)base.Clone()) { pos = this.pos };
        }

        public bool eos()
        {
            return (this.pos == base.size());
        }

        public OctetsStream marshal(StructData stdata)
        {
            stdata.WriteData(this);
            return this;
        }

        public OctetsStream marshal(OctetsBuffer o)
        {
            this.marshal_int(o.size());
            base.insert(base.size(), o);
            return this;
        }

        public OctetsStream marshal(bool b)
        {
            base.push_back(!b ? ((byte)0) : ((byte)1));
            return this;
        }

        public OctetsStream marshal(byte x)
        {
            base.push_back(x);
            return this;
        }

        public OctetsStream marshal(double x)
        {
            return this.marshal(BitConverter.ToInt64(BitConverter.GetBytes(x), 0));
        }

        public OctetsStream marshal(short x)
        {
            return this.marshal((ushort)x);
        }

        public OctetsStream marshal(int x)
        {
            return this.marshal((uint)x);
        }

        public OctetsStream marshal(long x)
        {
            return this.marshal((ulong)x);
        }

        public OctetsStream marshal(sbyte x)
        {
            base.push_back((byte)x);
            return this;
        }

        public OctetsStream marshal(float x)
        {
            return this.marshal(BitConverter.ToInt32(BitConverter.GetBytes(x), 0));
        }

        public OctetsStream marshal(string str)
        {
            return this.marshal(str, null);
        }

        public OctetsStream marshal(string str, int size)
        {
            byte[] tempdata = Encoding.Default.GetBytes(str);
            base.insert(base.size(), tempdata);
            if (size > tempdata.Length)
                base.insert(base.size(), new byte[size - tempdata.Length]);
            return this;
        }

        public OctetsStream marshal(ushort x)
        {
            if (BitConverter.IsLittleEndian)
                return this.marshal((byte)(x)).marshal((byte)(x >> 8));
            else
                return this.marshal((byte)(x >> 8)).marshal((byte)(x));
        }

        public OctetsStream marshal(uint x)
        {
            if (BitConverter.IsLittleEndian)
            {
                return
                    this.marshal((byte)(x))
                        .marshal((byte)(x >> 8))
                        .marshal((byte)(x >> 16))
                        .marshal((byte)(x >> 24));
            }
            else
            {
                return
                    this.marshal((byte)(x >> 24))
                        .marshal((byte)(x >> 16))
                        .marshal((byte)(x >> 8))
                        .marshal((byte)(x));
            }
        }

        public OctetsStream marshal(ulong x)
        {
            if (BitConverter.IsLittleEndian)
            {
                return
                    this.marshal((byte)(x))
                        .marshal((byte)(x >> 8))
                        .marshal((byte)(x >> 16))
                        .marshal((byte)(x >> 24))
                        .marshal((byte)(x >> 32))
                        .marshal((byte)(x >> 40))
                        .marshal((byte)(x >> 48))
                        .marshal((byte)(x >> 56));
            }
            else
            {
                return
                    this.marshal((byte)(x >> 56))
                        .marshal((byte)(x >> 48))
                        .marshal((byte)(x >> 40))
                        .marshal((byte)(x >> 32))
                        .marshal((byte)(x >> 24))
                        .marshal((byte)(x >> 16))
                        .marshal((byte)(x >> 8));
            }
        }

        public OctetsStream marshal(byte[] bytes)
        {
            this.marshal_int(bytes.Length);
            base.insert(base.size(), bytes);
            return this;
        }

        public OctetsStream marshal(string str, string charset)
        {
            try
            {
                if (charset == null)
                {
                    this.marshal(Encoding.Default.GetBytes(str));
                }
                else
                {
                    this.marshal(Encoding.GetEncoding(charset).GetBytes(str));
                }
            }
            catch (Exception exception)
            {
                throw new SystemException(exception.Message);
            }
            return this;
        }

        public void marshal_string(string str, int size)
        {
            this.marshal(str, size);
        }

        public void marshal_string(string str)
        {
            this.marshal(str);
        }

        public OctetsStream marshal_bytes(byte[] bytes, int size)
        {
            base.insert(base.size(), bytes, 0, size);
            if (size > bytes.Length)
                base.insert(base.size(), new byte[size - bytes.Length]);
            return this;
        }

        public void marshal_boolean(bool x)
        {
            this.marshal(x);
        }

        public void marshal_byte(byte x)
        {
            this.marshal(x);
        }

        public void marshal_double(double x)
        {
            this.marshal(x);
        }

        public void marshal_float(float x)
        {
            this.marshal(x);
        }

        public void marshal_int(int x)
        {
            this.marshal(x);
        }

        public void marshal_long(long x)
        {
            this.marshal(x);
        }

        public void marshal_Octets(OctetsBuffer o)
        {
            this.marshal(o);
        }

        public void marshal_sbyte(sbyte x)
        {
            this.marshal(x);
        }

        public void marshal_short(short x)
        {
            this.marshal(x);
        }

        public void marshal_uint(uint x)
        {
            this.marshal(x);
        }

        public void marshal_ulong(ulong x)
        {
            this.marshal(x);
        }

        public void marshal_ushort(ushort x)
        {
            this.marshal(x);
        }

        public OctetsStream marshal_struct(StructCmd scmd)
        {
            return scmd.WriteStruct(this);
        }

        public void write_ushort(int index, ushort x)
        {
            buffer()[index] = (byte)x;
            buffer()[index + 1] = (byte)(x >> 8);
        }

        public void write_byte(int index, byte x)
        {
            buffer()[index] = (byte)x;
        }

        public int position()
        {
            return this.pos;
        }

        public int position(int pos)
        {
            this.pos = pos;
            return this.pos;
        }

        public OctetsStream push_bytes(byte[] bytes)
        {
            base.insert(base.size(), bytes);
            return this;
        }

        public OctetsStream push_bytes(byte[] bytes, int len)
        {
            base.insert(base.size(), bytes, 0, len);
            return this;
        }

        public int remain()
        {
            return (base.size() - this.pos);
        }


        public OctetsStream unmarshal(OctetsBuffer os)
        {
            int size = (int)this.unmarshal_uint();
            if ((this.pos + size) > base.size())
            {
                throw new MarshalException();
            }
            os.replace(this, this.pos, size);
            this.pos += size;
            return this;
        }

        public OctetsStream unmarshal(OctetsBuffer os, int size)
        {
            if ((this.pos + size) > base.size())
            {
                throw new MarshalException();
            }
            os.replace(this, this.pos, size);
            this.pos += size;
            return this;
        }

        public bool unmarshal_boolean()
        {
            return (this.unmarshal_byte() == 1);
        }

        public byte unmarshal_byte()
        {
            if ((this.pos + 1) > base.size())
            {
                throw new MarshalException();
            }
            return base.getByte(this.pos++);
        }

        public byte[] unmarshal_bytes()
        {
            int length = this.unmarshal_ushort();
            if ((this.pos + length) > base.size())
            {
                throw new MarshalException();
            }
            byte[] destinationArray = new byte[length];
            Array.Copy(base.buffer(), this.pos, destinationArray, 0, length);
            this.pos += length;
            return destinationArray;
        }

        public byte[] unmarshal_bytes(int length)
        {
            if ((this.pos + length) > base.size())
            {
                throw new MarshalException();
            }
            byte[] destinationArray = new byte[length];
            Array.Copy(base.buffer(), this.pos, destinationArray, 0, length);
            this.pos += length;
            return destinationArray;
        }

        public double unmarshal_double()
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(this.unmarshal_long()), 0);
        }

        public float unmarshal_float()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(this.unmarshal_int()), 0);
        }

        public int unmarshal_int()
        {
            if ((this.pos + 4) > base.size())
            {
                throw new MarshalException();
            }
            byte num = base.getByte(this.pos++);
            byte num2 = base.getByte(this.pos++);
            byte num3 = base.getByte(this.pos++);
            byte num4 = base.getByte(this.pos++);

            if (BitConverter.IsLittleEndian)
            {
                return (((((num & 0xff))
                        | ((num2 & 0xff) << 8))
                        | ((num3 & 0xff) << 16))
                        | ((num4 & 0xff) << 24));
            }
            else
            {
                return (((((num4 & 0xff))
                        | ((num3 & 0xff) << 8))
                        | ((num2 & 0xff) << 16))
                        | ((num & 0xff) << 24));
            }
        }

        public long unmarshal_long()
        {
            return (long)this.unmarshal_ulong();
        }

        public OctetsBuffer unmarshal_Octets()
        {
            int size = (int)this.unmarshal_uint();
            if ((this.pos + size) > base.size())
            {
                throw new MarshalException();
            }
            OctetsBuffer octets = new OctetsBuffer(this, this.pos, size);
            this.pos += size;
            return octets;
        }

        public sbyte unmarshal_sbyte()
        {
            if ((this.pos + 1) > base.size())
            {
                throw new MarshalException();
            }
            return (sbyte)base.getByte(this.pos++);
        }

        public short unmarshal_short()
        {
            return (short)this.unmarshal_ushort();
        }


        public string unmarshal_String(int length)
        {
            string str;
            try
            {
                int len = length;
                if ((this.pos + len) > base.size())
                {
                    throw new MarshalException();
                }
                int pos = this.pos;
                this.pos += len;
                str = string.Copy(base.getString(pos, len));
            }
            catch (Exception exception)
            {
                throw new SystemException(exception.Message);
            }
            return str;
        }

        public string unmarshal_String()
        {
            return this.unmarshal_String(null);
        }

        public string unmarshal_String(string charset)
        {
            string str;
            try
            {
                int len = (int)this.unmarshal_ushort();
                if ((this.pos + len) > base.size())
                {
                    throw new MarshalException();
                }
                int pos = this.pos;
                this.pos += len;
                str = (charset != null) ? string.Copy(base.getString(pos, len, charset)) : string.Copy(base.getString(pos, len));
            }
            catch (Exception exception)
            {
                throw new SystemException(exception.Message);
            }
            return str;
        }

        public uint unmarshal_uint()
        {
            if ((this.pos + 4) > base.size())
            {
                throw new MarshalException();
            }
            byte num = base.getByte(this.pos++);
            byte num2 = base.getByte(this.pos++);
            byte num3 = base.getByte(this.pos++);
            byte num4 = base.getByte(this.pos++);

            if (BitConverter.IsLittleEndian)
            {
                return (uint)(((((num & 0xff))
                            | ((num2 & 0xff) << 8))
                            | ((num3 & 0xff) << 16))
                            | ((num4 & 0xff) << 24));
            }
            else
            {
                return (uint)(((((num4 & 0xff))
                                | ((num3 & 0xff) << 8))
                                | ((num2 & 0xff) << 16))
                                | ((num & 0xff) << 24));
            }
        }

        public ulong unmarshal_ulong()
        {
            if ((this.pos + 8) > base.size())
            {
                throw new MarshalException();
            }
            byte num = base.getByte(this.pos++);
            byte num2 = base.getByte(this.pos++);
            byte num3 = base.getByte(this.pos++);
            byte num4 = base.getByte(this.pos++);
            byte num5 = base.getByte(this.pos++);
            byte num6 = base.getByte(this.pos++);
            byte num7 = base.getByte(this.pos++);
            byte num8 = base.getByte(this.pos++);

            if (BitConverter.IsLittleEndian)
            {
                return
                    (ulong)
                        (((((((((num & 0xffL))
                            | ((num2 & 0xffL) << 8))
                            | ((num3 & 0xffL) << 16))
                            | ((num4 & 0xffL) << 24))
                            | ((num5 & 0xffL) << 32))
                            | ((num6 & 0xffL) << 40))
                            | ((num7 & 0xffL) << 48))
                            | ((num8 & 0xffL) << 56));
            }
            else
            {
                return
                    (ulong)
                        (((((((((num8 & 0xffL))
                            | ((num7 & 0xffL) << 8))
                            | ((num6 & 0xffL) << 16))
                            | ((num5 & 0xffL) << 24))
                            | ((num4 & 0xffL) << 32))
                            | ((num3 & 0xffL) << 40))
                            | ((num2 & 0xffL) << 48))
                            | ((num & 0xffL) << 56));
            }
        }

        public ushort unmarshal_ushort()
        {
            if ((this.pos + 2) > base.size())
            {
                throw new MarshalException();
            }

            byte num = base.getByte(this.pos++);
            byte num2 = base.getByte(this.pos++);

            if (BitConverter.IsLittleEndian)
            {
                return (ushort)(((num & 0xff)) | ((num2 & 0xff) << 8));
            }
            else
            {
                return (ushort)(((num2 & 0xff)) | ((num & 0xff) << 8));
            }
        }

        public OctetsStream unmarshal_struct(StructCmd scmd)
        {
            return scmd.ReadStruct(this);
        }

        public static OctetsStream wrap(OctetsBuffer o)
        {
            OctetsStream stream = new OctetsStream();
            stream.swap(o);
            return stream;
        }
    }
}

