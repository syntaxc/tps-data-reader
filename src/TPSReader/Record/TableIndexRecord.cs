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

namespace TPSReader.Record
{
	/// <summary>
	/// Description of TableIndexRecord.
	/// </summary>
	public class TableIndexRecord
	{
		private int _indexID;
		private int _tableID;
		private int _recordID;
		
		
		public int IndexID{
			get{
				return _indexID;
			}
		}
		public int TableID{
			get{
				return _tableID;
			}
		}
		public int RecordID {
			get{
				return _recordID;
			}
		}
		public TableIndexRecord(TPSRecord record)
		{
			RandomAccess ra = new RandomAccess();
			ra.OpenStream( new System.IO.MemoryStream(record.RecordData));
			
			_tableID = ra.beLong();
			_indexID = ra.leByte();
			
			ra.jumpAbs(ra.fileSize - 4 );
			_recordID = ra.beLong();
		
			ra = null;			
		}
		public override string ToString()
		{
			return string.Format("[TableIndexRecord IndexID={0}, TableID={1}, RecordID={2}]", _indexID, _tableID, _recordID);
		}

		
	}
}
