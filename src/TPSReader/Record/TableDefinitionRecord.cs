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
	/// The TableDefinition is a Record in the TPS db.
	/// These may not always be in order. They consist of chunks through the file.
	/// So we need to collect all of the chunks. Put them together and then parse them for the information
	/// 
	/// </summary>
	public class TableDefinitionRecord
	{
		private int _tableID;
		private int _blockID;
		
		public int TableID{
			get{
				return _tableID;
			}
		}
		public int BlockID{
			get{
				return _blockID;
			}
		}
		public TableDefinitionRecord(TPSRecord record)
		{
			RandomAccess ra = new RandomAccess();
			ra.OpenStream ( new System.IO.MemoryStream(record.RecordData));
			_tableID = ra.beLong();
			ra.leByte(); //TPSRecord Type
			_blockID = ra.leShort();
		}
		
		public override string ToString()
		{
			return string.Format("[TableDefinitionRecord TableID={0}, BlockID={1}]", _tableID, _blockID);
		}

	}
}
