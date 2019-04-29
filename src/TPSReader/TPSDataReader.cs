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
using System.Data;
using System.Linq;

namespace TPSReader
{
	/// <summary>
	/// Description of TPSDataReader.
	/// This is to be used like a SQLDataReader.. but slightly different because of our limitations
	/// 
	/// Example usage:
	/// TPSDataReader tdr = TPSReader.ExecuteDataReader(tablename);
	/// while (tdr.Read() ){ tdr.Get("First Name");}
	/// </summary>
	public class TPSDataReader
	{
		private TableSchema _schema;
		
		private List<Dictionary<string, string>> _returnDataStack;
		private List<TPSPage> _tpsPages;
		
		private Dictionary<string, string> _currentRow;
		
		public TPSDataReader(List<TPSPage> tpsPages, TableSchema tableSchema)
		{
			_tpsPages = tpsPages;
			_schema = tableSchema;
			
			_returnDataStack = new List<Dictionary<string, string>>();
			
		}
		
		/// <summary>
		/// Will read the next TPSRecord.
		/// True or false depending if there is another one in the queue
		/// </summary>
		/// <returns></returns>
		public bool Read(){
			
			//if there is anything on our temporary stack, just send the first one back to the user and remove it from thelist
			if ( _returnDataStack.Count > 0 ){
				_currentRow = _returnDataStack[0];
				_returnDataStack.RemoveAt(0);
				return true;
			}
			
			
			//read page after page until either there are no more pages or we have something on our stack to return
			
			while ( _tpsPages.Count > 0 && _returnDataStack.Count < 1 ){
				TPSPage page = _tpsPages[0];
				_tpsPages.RemoveAt(0); //pop the page off the stack
			
				List<TPSRecord> pageRecords = page.GetRecords();
				foreach( TPSRecord record in pageRecords){
					//go through each record to see if we need it
					if ( record.RecordType == TPSRecord.TYPE_DATA )
					{
						//Check if this record belongs to a table that we are searchin for
						//Notice that we need to generate the table ID before we can check
						//This part is normally in the TableDataRecord file, but we want to know before
						//parsing the whole record in the name of efficiency
						byte[] recordTableIDBA = new byte[4];
						if ( record.RecordData.Length < 4 )
						{
							continue;
						}
						
						//Get the tableID ( it is backwards )
						recordTableIDBA[0] = record.RecordData[3];
						recordTableIDBA[1] = record.RecordData[2];
						recordTableIDBA[2] = record.RecordData[1];
						recordTableIDBA[3] = record.RecordData[0];
						int recordTableID = BitConverter.ToInt32(recordTableIDBA, 0);
						
						if ( _schema.TableID == recordTableID ) {
						
							//Generate the record
							Record.TableDataRecord tdr = new Record.TableDataRecord(record, _schema);
							Dictionary<string, string> newRow = new Dictionary<string, string>();
							for(int i=0; i<tdr.TableDataRow.Table.Columns.Count; i++)
								newRow.Add(tdr.TableDataRow.Table.Columns[i].ColumnName, tdr.TableDataRow[tdr.TableDataRow.Table.Columns[i].ColumnName].ToString());
							
							_returnDataStack.Add(newRow);
						}
					}
				}
			}
			
			//Now send back a record. If not, we're out and send false
			if ( _returnDataStack.Count > 0 ){
				_currentRow = _returnDataStack[0];
				_returnDataStack.RemoveAt(0);
				return true;
			}else{
				return false;
			}
			
			return false;
		}
		
		public string Get(string columnName){
			if ( _currentRow == null )
				throw new Exception("Attepting to get data from a row while no row is loaded. Try calling .Read() first.");
			
			return _currentRow[columnName];
		}
		public string Get(int columnIndex){
			if ( _currentRow == null )
				throw new Exception("Attepting to get data from a row while no row is loaded. Try calling .Read() first.");
			
			
			return _currentRow.ElementAt(columnIndex).Value.ToString();
		}
			
		
	}
}
