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

namespace TPSReader
{
	/// <summary>
	/// Description of TableSchemaCollection.
	/// </summary>
	public class TableSchemaCollection : Dictionary<int, TableSchema>
	{
		public TableSchemaCollection()
		{
		}
		
		/// <summary>
		/// 
		/// Returns TableID if found
		/// Returns -1 if not found
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public int searchTableName(string tableName){
			
			foreach ( TableSchema ts in this.Values ) {
				if ( tableName == ts.TableName )
					return ts.TableID;
			}
			
			return -1;
		}
	}
}
