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
	/// Description of TPSRecord.
	/// </summary>
	public class TPSRecord
	{
		
		public const int TYPE_DATA = 0;
		public const int TYPE_TABLE_NAME = 1;
		public const int TYPE_INDEX = 2;
		public const int TYPE_MEMO = 3;
		public const int TYPE_METADATA = 4;
		public const int TYPE_TABLE_DEFINITION = 5;
		
		private int _recordType;
		
		private int _flags;
		private int _recordLength;
		private int _headerLength;
		private byte[] _data; //this is the header and footer ( it does not includ the flag, recordLength byte, headerbyte
		
		#region public variables
		public int RecordType{
			get{
				return _recordType;
			}
		}
		public int RecordLength{
			get{
				return _recordLength;
			}
		}
		public int HeaderLength{
			get{
				return _headerLength;
			}
		}
		public byte[] RecordData{
			get{
				return _data;
			}
		}
		
		#endregion
		
		public TPSRecord(ref RandomAccess pageDataRandomAccess)
		{
			_flags = pageDataRandomAccess.leByte();
			if ((_flags & 0xC0) != 0xC0) {
	            throw new Exception("Can't construct a TpsRecord without record lengths");
	        }
	        _recordLength = pageDataRandomAccess.leShort();
	        _headerLength = pageDataRandomAccess.leShort();
	        
	        _data = pageDataRandomAccess.leBytes(_recordLength); //read all of the data into a buffer. 
	        //
	        buildHeader();
		}
		/// <summary>
		/// Reads a record using the previous data available.
		/// TPS uses a stupid ( prossesing costly ) method to save space. It copies data from the last record if possible
		/// </summary>
		/// <param name="previous"></param>
		/// <param name="pageDataRandomAccess"></param>
		public TPSRecord(ref TPSRecord previous, ref RandomAccess pageDataRandomAccess){
			
			//the flag tells us what data we should be copying
			_flags = pageDataRandomAccess.leByte();
	        if ((_flags & 0x80) != 0) {
	            _recordLength = pageDataRandomAccess.leShort();
	        } else {
	            _recordLength = previous.RecordLength;
	        }
	        if ((_flags & 0x40) != 0) {
	            _headerLength = pageDataRandomAccess.leShort();
	        } else {
	            _headerLength = previous.HeaderLength;
	        }
			
			//The last part tells us how much actual record data we should copy
	        int copy = _flags & 0x3F;
	        _data = new byte[_recordLength];
	        try {
	        	Buffer.BlockCopy(previous.RecordData,0, _data, 0, copy);
	        	Buffer.BlockCopy(pageDataRandomAccess.leBytes(_recordLength - copy), 0, _data, copy, (_recordLength - copy));
	        	//We may have to check if we pulled enough bytes from the pageDataRA, incase we hit the end prematurely
	            
	        } catch (Exception ex) {
	            throw new Exception("When reading " + (_recordLength - copy) + " bytes of TpsRecord");
	        }
	        //
	        buildHeader();
		}
		
		/**
	     * constructs the header for the record by peeking at the type.
	     * Most records have their type at the 5th byte, except for the
	     * table name, which has it at position 0.
	     */
	    private void buildHeader() {
	    	
	    	// The header if the first portion of the data
	    	// The length is _headerLength
	        if ( _headerLength >= 5 && _data.Length >=5) {
	            //
	            if ((_data[0] & 0xFF) == 0xFE) {
	            	_recordType = TYPE_TABLE_NAME;
	                //header = new TableNameHeader(new RandomAccess(hdr));
	            } else {
	                //
	                switch ((int) (_data[4] & 0xFF)) {
		                case 0xF3:
		                	_recordType = TYPE_DATA;
		                    //header = new DataHeader(new RandomAccess(hdr));
		                    break;
		                case 0xF6:
		                    _recordType = TYPE_METADATA;
		                    //header = new MetadataHeader(new RandomAccess(hdr));
		                    break;
		                case 0xFA:
		                    _recordType = TYPE_TABLE_DEFINITION;
		                    //header = new TableDefinitionHeader(new RandomAccess(hdr));
		                    break;
		                case 0xFC:
		                    _recordType = TYPE_MEMO;
		                    //header = new MemoHeader(new RandomAccess(hdr));
		                    break;
		                default:
		                    _recordType = TYPE_INDEX;
		                    //header = new IndexHeader(new RandomAccess(hdr));
		                    break;
	                }
	            }
	        } 
	    }
	}
}
