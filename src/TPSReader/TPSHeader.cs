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
using System.Collections.Generic;

namespace TPSReader
{
	/// <summary>
	/// Description of TPSHeader.
	/// </summary>
	public class TPSHeader
	{
		public bool IsTopSpeedFile =false;
		public int[] PageEnd;
		public int[] PageStart;
		public int Addr;
		public short HeaderSize;
		public int FileLength1;
		public int FileLength2;
		public string TopSpeed;
		public int Zeros;
		public int LastIssuedRow;
		public int Changes;
		public int ManagementPageRef;
		
		public TPSHeader(  )
		{
			
		}
		
		public void Process(){
			
			RandomAccess ra = RandomAccess.GetInstance();
			ra.position = 0;

			Addr = ra.leLong(); 
			if ( Addr != 0 )
				throw new Exception("File doesn't start with 0x00000000 - it's not a TPS databse");
			
			HeaderSize = ra.leShort();
			FileLength1 = ra.leLong();
			FileLength2 = ra.leLong();
			TopSpeed = ra.fixedLengthString(4);
			Zeros  = ra.leShort();
			LastIssuedRow = ra.beLong();
			Changes = ra.leLong();
			ManagementPageRef = ra.toFileOffset(ra.leLong());
			
			PageStart = ra.toFileOffset(ra.leLongArray((0x110 - 0x20) / 4));
			PageEnd = ra.toFileOffset(ra.leLongArray((0x200 - 0x110) / 4));
		}
		
		public override string ToString()
		{
			 
			RandomAccess ra = RandomAccess.GetInstance();
	        string str = "";
	        str += "TpsHeader(" + ra.toHex8(Addr) + "," + ra.toHex4(HeaderSize) + "," + ra.toHex8(FileLength1) + "," + ra.toHex8(FileLength2) + "," + TopSpeed
	                + "," + ra.toHex4(Zeros) + "," + ra.toHex8(LastIssuedRow) + "," + ra.toHex8(Changes) + "," + ra.toHex8(ManagementPageRef) + ")\n";
	        for (int t = 0; t < PageStart.Length; t++) {
	            str += ra.toHex8(PageStart[t]) + ".." + ra.toHex8(PageEnd[t]) + "\n";
	        }
	        return str;
    
		}

	}
}
