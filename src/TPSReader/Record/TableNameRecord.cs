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

namespace TPSReader.Record
{
	/// <summary>
	/// Description of TableName.
	/// </summary>
	public class TableNameRecord
	{
		
		private string _tableName;
		private int _tableID;
		
		public string TableName{
			get{
				return _tableName;
			}
		}
		public int TableID{
			get{
				return _tableID;
			}
		}
		
		public TableNameRecord(TPSRecord tpsRecord)
		{
			RandomAccess ra = new RandomAccess();
			ra.OpenStream(new MemoryStream(tpsRecord.RecordData));
			
			
			ra.jumpRelative(1); //skip the first byte in the header
			
			_tableName = ra.fixedLengthString(tpsRecord.HeaderLength - (int)ra.position).Trim(); //the rest of the header is the name
			_tableID = ra.beLong(); //followed by the table ID
			
			
			if ( _tableName == null )
				_tableName = "NULL_NAME_" + _tableID.ToString();
			ra = null;			
		}
		
		public override string ToString()
		{
			return string.Format("[TableNameRecord TableName={0}, TableID={1}]", _tableName, _tableID);
		}

		
		
	}
}
