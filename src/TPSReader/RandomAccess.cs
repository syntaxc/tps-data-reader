/*
 *  Copyright 2014 C.Chenier
 *  Special thanks to E.Hooijmeijer for his work on tps-to-csv ( http://ctrl-alt-dev.nl/Projects/TPS-to-CSV/TPS-to-CSV.html )
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
using System;
using System.IO;

namespace TPSReader
{
	/// <summary>
	/// Description of RandomAccess.
	/// </summary>
	public class RandomAccess
	{
		//this instance is used for the main file
		private static RandomAccess _instance;
		
		private BinaryReader _binaryReader;
		private string _filename;
		
		private long ofs; //offset
		
		
		public static RandomAccess GetInstance(){
			if ( _instance == null )
				_instance = new RandomAccess();
			return _instance;
		}
		public RandomAccess()
		{
		}
		
		public void OpenFile(string filename){
			_filename = filename;
			_binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			ofs = 0;
		}
		/// <summary>
		/// Used for reading in Records
		/// </summary>
		/// <param name="memoryStream"></param>
		public void OpenStream(MemoryStream memoryStream){
			_binaryReader = new BinaryReader(memoryStream);
		}
		public void CloseFile(){
			if ( _binaryReader != null )
				_binaryReader.Close();
		}
		
		#region Public Variables
		public long position{
			get{
				return ofs;
			}
			set{
				ofs = value;
			}
		}
		public long fileSize{
			get{
				return _binaryReader.BaseStream.Length;
			}
		}
		#endregion
		
		public void jumpAbs(long position){
			ofs = position;
			_binaryReader.BaseStream.Position = ofs;
		}
		public void jumpRelative(long offset){
			
			_binaryReader.BaseStream.Seek((long)offset, SeekOrigin.Current);
			ofs += offset;
		}
		public bool isAtEnd(){
			return ofs >= _binaryReader.BaseStream.Length -1;
		}
		
		
		#region little endian
		public Int32 leLong(){
			ofs += 4;
			return _binaryReader.ReadInt32();
		}
		public Int16 leShort(){
			ofs += 2;
			return _binaryReader.ReadInt16();
		}
		public UInt32 leULong(){
			ofs += 4;
			return _binaryReader.ReadUInt32();
		}
		public UInt16 leUShort(){
			ofs += 2;
			return _binaryReader.ReadUInt16();
		}
		
		public byte leByte(){
			ofs++;
			return _binaryReader.ReadByte();
		}
		public byte[] leBytes(int count){
			ofs += count;
			return _binaryReader.ReadBytes(count);
		}
		public float leFloat(){
			ofs += 4;
			return _binaryReader.ReadSingle();
		}
		public double leDouble(){
			ofs += 8;
			return _binaryReader.ReadDouble();
		}
		public int[] leLongArray(int i) {
	        int[] results = new int[i];
	        for (int t = 0; t < i; t++) {
	            results[t] = leLong();
	        }
	        return results;
	    }
		#endregion
				
		#region big endian
		private byte[] a16 = new byte[2];
	    private byte[] a32 = new byte[4];
	    private byte[] a64 = new byte[8];
	   
	    public Int32 beLong()
	    {
	        a32 = _binaryReader.ReadBytes(4);
	        Array.Reverse(a32);
	        ofs += 4;
	        return BitConverter.ToInt32(a32,0);
	        
	    }
	    public Int16 beShort()
	    {
	        a16 = _binaryReader.ReadBytes(2);
	        Array.Reverse(a16);
	        ofs += 2;
	        return BitConverter.ToInt16(a16, 0);
	    }
	   	public UInt32 beULong()
	    {
	   		a32 = _binaryReader.ReadBytes(4);
	        Array.Reverse(a32);
	        ofs += 4;
	        return BitConverter.ToUInt32(a32,0);
	    }
	    public UInt16 beUShort()
	    {
	    	a16 = _binaryReader.ReadBytes(2);
	        Array.Reverse(a16);
	        ofs += 2;
	        return BitConverter.ToUInt16(a16, 0);
	    }
	    public byte beByte(){
	    	ofs++;
	    	return _binaryReader.ReadByte();
	    }
		#endregion
		
		#region strings 
		public string fixedLengthString(int length){
			return fixedLengthString(length, System.Text.Encoding.GetEncoding("ISO-8859-1"));
		}
		public string fixedLengthString(int length, System.Text.Encoding encoding){
			ofs += length;
			return encoding.GetString( _binaryReader.ReadBytes(length));
		}
		public String zeroTerminatedString() {
	        return zeroTerminatedString(System.Text.Encoding.GetEncoding("ISO-8859-1"));
	    }
	
	    public String zeroTerminatedString(System.Text.Encoding encoding) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
	        int value = 0;
	        do {
	            value = leByte();
	            if (value != 0) {
	            	sb.Append((char)value);
	            }
	        } while (value != 0);
	        
	        return sb.ToString();
	    }
		/**
	     * pascal strings have their length encoded in the first byte.
	     * @return the string.
	     */
	    public string pascalString() {
	    	return pascalString(System.Text.Encoding.GetEncoding("ISO-8859-1"));
	    }
	
	    public string pascalString(System.Text.Encoding encoding) {
	        int len = leByte();
	        System.Text.StringBuilder sb = new System.Text.StringBuilder();
	        for (int t = 0; t < len; t++) {
	        	sb.Append((char)leByte());
	        }
	        return sb.ToString();
	    }
		#endregion
		
		public int toFileOffset(int pageReference) {
        	return (pageReference << 8) + 0x200;
	    }
	
	    public int[] toFileOffset(int[] pageReferences) {
	        int[] results = new int[pageReferences.Length];
	        for (int t = 0; t < results.Length; t++) {
	            results[t] = toFileOffset(pageReferences[t]);
	        }
	        return results;
	    }
		
		public String binaryCodedDecimal(int len, int totalDigits, int digitsAfterDecimalPoint) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
	        foreach(byte b in leBytes(len)) {
	            sb.Append(toHex2(b & 0xFF));
	        }
			string sign = sb.ToString().Substring(0, 1);
			string number = sb.ToString().Substring(1);
	        if (digitsAfterDecimalPoint > 0) {
	            int decimalIndex = number.Length - digitsAfterDecimalPoint;
	            number = trimLeadingZeros(number.Substring(0, decimalIndex)) + "." + number.Substring(decimalIndex);
	        } else {
	            number = trimLeadingZeros(number);
	        }
	        return (!sign.Equals("0") ? "-" : "") + number;
	    }
		    /**
	     * @param number a number string.
	     * @return the number string without leading zeros.
	     */
	    private string trimLeadingZeros(string number) {
	        int idx = -1;
	        for (int t = 0; t < number.Length; t++) {
	        	if (number[t] == '0') {
	                idx++;
	            } else {
	                break;
	            }
	        }
	        if (idx == number.Length - 1) {
	            return number.Substring(idx);
	        } else {
	            return number.Substring(idx + 1);
	        }
	    }
	

		
		public byte[] deRle( int dataSize ){
			
			long endPosition = position + dataSize;
			using ( MemoryStream ms = new MemoryStream()){
				do {
	                int skip = leByte();
	                if (skip == 0) {
	                	throw new Exception("Bad RLE Skip (0x00), Skip: " + skip.ToString() + " position: " + position.ToString() + " end position: " + endPosition.ToString());
	                }
	                if (skip > 0x7F) {
	                    int msb = leByte();
	                    int lsb = (skip & 0x7F);
	                    int shift = 0x80 * (msb & 0x01);
	                    skip = ((msb << 7) & 0x00FF00) + lsb + shift;
	                }
	                ms.Write(leBytes(skip), 0, skip);
	                if ( ofs < (_binaryReader.BaseStream.Length - 1)  ) {
	                    jumpRelative(-1);
	                    int toRepeat = leByte();
	                    int repeatsMinusOne = leByte();
	                    if (repeatsMinusOne > 0x7F) {
	                        int msb = leByte();
	                        int lsb = (repeatsMinusOne & 0x7F);
	                        int shift = 0x80 * (msb & 0x01);
	                        repeatsMinusOne = ((msb << 7) & 0x00FF00) + lsb + shift;
	                    }
	                    byte[] repeat = new byte[repeatsMinusOne];
	                    for (int t = 0; t < repeat.Length; t++) {
	                        repeat[t] = (byte) toRepeat;
	                    }
	                    ms.Write(repeat, 0, repeat.Length);
	                }
	            } while (position < endPosition -1);
				
				return ms.ToArray();
			}
		}
		
		#region hex output
		public string toHex8(int value){
			string tmp = value.ToString("X");
			while (tmp.Length < 8 )
				tmp = "0" + tmp;
			return tmp;
		}
		public string toHex4(int value){
			string tmp = value.ToString("X");
			while (tmp.Length < 4 )
				tmp = "0" + tmp;
			return tmp;
		}
		public string toHex2(int value){
			string tmp = value.ToString("X");
			while (tmp.Length < 2 )
				tmp = "0" + tmp;
			return tmp;
		}
		#endregion
		
	}
}
