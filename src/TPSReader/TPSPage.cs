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
using System.Collections.Generic;
using System.IO;

namespace TPSReader
{
	/// <summary>
	/// Description of TPSPage.
	/// </summary>
	public class TPSPage
	{
		private int addr;
		private int pageSize;
		private int pageSizeUncompressed;
		private int pageSizeUncompressedWithoutHeader;
		private int recordCount;
		private int flags;
		
		
		public TPSPage()
		{
			
		}
		
		public int PageSize{
			get{
				return pageSize;
			}
		}
		public int RecordCount{
			get{
				return recordCount;
			}
		}
		public bool IsCompressed{
			get{
				return ( pageSize == pageSizeUncompressed );
			}
		}
		public void Process(){
			RandomAccess ra = RandomAccess.GetInstance();
			
			addr = ra.leLong();
			pageSize = ra.leShort();
			pageSizeUncompressed = ra.leShort();
			pageSizeUncompressedWithoutHeader = ra.leShort();
			recordCount = ra.leShort();
			flags = (int)ra.leByte();
			
			ra.jumpRelative( pageSize - 13 ); //burn these bytes. This will leave the RandomAccess at the very end of the page
		}
		
		public override string ToString()
		{
			RandomAccess ra = RandomAccess.GetInstance();
			return string.Format("[TPSPage Addr={0}, PageSize={1}, PageSizeUncompressed={2}, PageSizeUncompressedWithoutHeader={3}, RecordCount={4}, Flags={5}]", ra.toHex8(addr), ra.toHex4(pageSize), ra.toHex4(pageSizeUncompressed), ra.toHex4(pageSizeUncompressedWithoutHeader), ra.toHex4(recordCount), ra.toHex2(flags));
		}
		
		/// <summary>
		/// Returns all records of the page
		/// <returns></returns>
		public List<TPSRecord> GetRecords(){
			
			List<TPSRecord> records = new List<TPSRecord>();
			
			byte[] pageData = GetData();
			//go through each record in the page
			RandomAccess ra = new RandomAccess();
			ra.OpenStream(new MemoryStream(pageData));
			
			records.Clear();
	        // Skip pages with non 0x00 flags as they don't seem to contain TpsRecords.
	        if (flags == 0x00) {
	            //data.pushPosition();
	            try {
	                TPSRecord prev = null;
	                do {
	                    TPSRecord current = null;
	                    if (prev == null) {
	                        current = new TPSRecord(ref ra);
	                    } else {
	                        current = new TPSRecord(ref prev, ref ra);
	                    }
						
	                    records.Add(current);
	                    prev = current;
	                } while (!ra.isAtEnd() && records.Count < recordCount);
	            } finally {
	                //data.popPosition();
	            }
	        }
			
			return records;
		}
		
		
		/// <summary>
		/// Gets data from the page
		/// If the page is compressed, it will uncompress it and return it.
		/// </summary>
		/// <returns></returns>
		private byte[] GetData(){
			//set our Randomaccess to the start of this page
			RandomAccess ra = RandomAccess.GetInstance();
			byte[] pageData;
			ra.jumpAbs(addr);
			
			ra.jumpRelative(13); //skip the header
			
			//pull the page data
			//This really shouldn't be very big.. on my files it was about 8kb 
			//If the file is using compression... uncompress it
			
			if ((pageSize != pageSizeUncompressed) && (flags == 0)) {
	            try {
	                pageData = ra.deRle(pageSize - 13);
	            } catch (Exception ex) {
					throw new Exception("Bad RLE DataBlock ( compressed: " + pageSize  + " uncompressed: " + pageSizeUncompressed + " Page Address: " + addr + "Record Count: " + recordCount.ToString() + "): ", ex );
	            }
	        } else {
				pageData = ra.leBytes( pageSize - 13 );
	        }
			
			return pageData;
			
			
		}


	}
}
