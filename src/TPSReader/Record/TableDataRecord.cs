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
using System.Data;
namespace TPSReader.Record
{
	/// <summary>
	/// Description of TableDataRecord.
	/// </summary>
	public class TableDataRecord
	{
		private int _tableID;
		private int _recordID;
		private DataRow _dataRow; //this is the c# datarow version of the data
		
		public DataRow TableDataRow{
			get{
				return _dataRow;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <param name="tableSchema"></param>
		public TableDataRecord(TPSRecord record, TableSchema tableSchema)
		{
			RandomAccess ra = new RandomAccess();
			ra.OpenStream ( new System.IO.MemoryStream(record.RecordData));
			_tableID = ra.beLong();
			ra.leByte(); //TPSRecord Type
			_recordID = ra.beLong();
			
			_dataRow = tableSchema.parseRow(_recordID, ref ra);
			
			ra = null;
			
			
		}
	}
}
