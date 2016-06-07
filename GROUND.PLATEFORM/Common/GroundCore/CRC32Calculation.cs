/// 
namespace PIS.Ground.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using PIS.Ground.Core.Data;
    using PIS.Ground.Core.LogMgmt;

    /// <summary>
    /// Class for the Calculation of CRC-32
    /// </summary>
    public class Crc32
    {
        #region Variable Decleration
        /// <summary>
        /// polynomial Key used by CRC-32 calculation
        /// </summary>
        private const uint Polykey = 0x04C11DB7;

        /// <summary>
        /// 
        /// </summary>
        private const bool Refin = true;

        /// <summary>
        /// 
        /// </summary>
        private const int Order = 32;

        /// <summary>
        /// 
        /// </summary>
        private const uint Crcinit = 0xffffffff;

        /// <summary>
        /// 
        /// </summary>
        private const bool Refout = true;

        /// <summary>
        /// 
        /// </summary>
        private const uint Crcxor = 0xffffffff;

        /// <summary>
        /// 
        /// </summary>
        private const uint MaskForTopBit = 0x80000000;

        /// <summary>
        /// 
        /// </summary>
        private const uint MaskForAllBits = 0xffffffff;

        /// <summary>
        /// 
        /// </summary>
        private static uint register;

        /// <summary>
        /// 
        /// </summary>
        private static CRCTable table;
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculate the checksum value
        /// </summary>
        /// <param name="filePath"> file path</param>
        /// <returns>checksm value</returns>
        public static uint CalculateChecksum(string filePath)
        {
            uint crcValue = 0;
            table = new CRCTable();
            register = Crcinit;
            table.Init(Polykey);
            TextReader textReader = null;
            if (!File.Exists(filePath))
            {
                return 0;
            }
            else
            {
                try
                {
                    FileInfo objFile = new FileInfo(filePath);
                    if (objFile != null)
                    {
                        textReader = new StreamReader(filePath);
                        crcValue = ComputeCrc32(textReader.ReadToEnd());
                    }

                    return crcValue;
                }
                catch (IOException ex)
                {
                    LogManager.WriteLog(TraceType.ERROR, ex.Message, "PIS.Ground.Core.Crc32.CalculateChecksum", null);
                    return 0;
                }
                finally
                {
                    if (textReader != null)
                    {
                        textReader.Close();
                        textReader.Dispose();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the Crc32 Checksum
        /// </summary>
        /// <param name="strData">input data</param>
        /// <returns>checksum value</returns>
        private static uint ComputeCrc32(string strData)
        {
            for (int i = 0; i < strData.Length; i++)
            {
                PutByte(Convert.ToByte(strData[i]));
            }

            uint crcValue = Done();
            return crcValue;
        }

        /// <summary>
        /// Reflects the lower 'bitnum' bits of 'crc'
        /// </summary>
        /// <param name="crc"> crc number</param>
        /// <param name="bitnum"> bit number</param>
        /// <returns></returns>
        private static uint Reflect(uint crc, int bitnum)
        {
            uint i, j = 1, crcout = 0;
            for (i = (uint)1 << (bitnum - 1); Convert.ToBoolean(i); i >>= 1)
            {
                if (Convert.ToBoolean(crc & i))
                {
                    crcout |= j;
                }

                j <<= 1;
            }

            return crcout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static uint Done()
        {
            register ^= Crcxor;
            register = register & MaskForAllBits;
            uint tmp = register;
            register = 0;
            return tmp;
        }

        /// <summary>
        /// puts the byte value
        /// </summary>
        /// <param name="bt"> input byte</param>
        private static void PutByte(byte bt)
        {
            uint top = register;
            if (Refin)
            {
                // For the first time
                if (register == Crcinit)
                {
                    register = Reflect(register, Order);
                }

                top = register;
                top = top & 0xff;
                top ^= bt;
                register = (register >> 8) ^ table.GetTableData(top);
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class CRCTable
    {
        #region Variable Decleration
        /// <summary>
        /// 
        /// </summary>
        private const uint MaskForAllBits = 0xffffffff;

        /// <summary>
        /// 
        /// </summary>
        private const uint MaskForTopBit = 0x80000000;

        /// <summary>
        /// 
        /// </summary>
        private const bool Refin = true;

        /// <summary>
        /// 
        /// </summary>
        private const int Order = 32;

        /// <summary>
        /// holds the polynomial key
        /// </summary>
        private uint key;

        /// <summary>
        /// array to hold the data
        /// </summary>
        private uint[] tableData = new uint[256];
        #endregion

        #region Public Methods

        /// <summary>
        /// initialize the key 
        /// </summary>
        /// <param name="pkey">polynomial key</param>
        internal void Init(uint pkey)
        {
            // assert (key != 0);
            if (pkey == this.key)
            {
                return;
            }

            this.key = pkey;

            // for all possible byte values
            for (uint i = 0; i < 256; ++i)
            {
                uint crc = i;
                if (Refin)
                {
                    crc = this.Reflect(crc, 8);
                }

                uint reg = crc << (Order - 8);

                // for all bits in a byte
                for (int j = 0; j < 8; ++j)
                {
                    bool topBit = (reg & MaskForTopBit) != 0;
                    reg <<= 1;
                    if (topBit)
                    {
                        reg ^= this.key;
                    }
                }

                if (Refin)
                {
                    reg = this.Reflect(reg, Order);
                }

                reg = reg & MaskForAllBits;
                this.tableData[i] = reg;
            }
        }

        /// <summary>
        /// Get the Table Date at index
        /// </summary>
        /// <param name="index">input index</param>
        /// <returns>Data at index</returns>
        internal uint GetTableData(uint index)
        {
            return this.tableData[index];
        }

        /// <summary>
        /// Reflects the lower 'bitnum' bits of 'crc'
        /// </summary>
        /// <param name="crc"> crc value</param>
        /// <param name="bitnum">bit number</param>
        /// <returns> crc checksum</returns>
        private uint Reflect(uint crc, int bitnum)
        {
            uint i, j = 1, crcout = 0;
            for (i = (uint)1 << (bitnum - 1); Convert.ToBoolean(i); i >>= 1)
            {
                if (Convert.ToBoolean(crc & i))
                {
                    crcout |= j;
                }

                j <<= 1;
            }

            return crcout;
        }
        #endregion
    }
}
