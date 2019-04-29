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
using System.Data;
using System.Linq;

namespace TPSReader
{
	/// <summary>
	/// Description of TPSReader.
	/// </summary>
	public class TPSReader
	{
		private string _filename;
		
		private TPSHeader _tpsHeader;
		private List<TPSBlock> _tpsBlocks;
		private List<TPSPage> _tpsPages;
		private TableSchemaCollection _tpsTables;
		
		
		private bool _tableInfoLoaded = false;
		
		
		private string _log = "";
		
		#region public variables
		public TPSHeader TpsHeader{
			get{
				return _tpsHeader;
			}
		}
		public List<TPSBlock> TpsBlocks{
			get{
				return _tpsBlocks;
			}
		}
		public List<TPSPage> TpsPages{
			get{
				return _tpsPages;
			}
		}
		public TableSchemaCollection TPSTables{
			get{
				return _tpsTables;
			}
		}
		#endregion
		public TPSReader(string filename)
		{
			_filename = filename;
			
		}
		
		public void Open(){
			
			RandomAccess ra = RandomAccess.GetInstance();
			ra.OpenFile(_filename);
			
			
			_log += "INFO: Opened File: " + _filename;
			
		}
		
		/// <summary>
		/// Initial Process of the TPS
		/// will load the table schema
		/// </summary>
		public void Process(){
			
			_tpsTables = new TableSchemaCollection();
			//read the header info
			ReadTPSHeaderInfo();
			GetTPSPageInfo();
			GetTPSTableSchema();
		}
		/// <summary>
		/// Will return the table schema
		/// </summary>
		/// <returns></returns>
		public TableSchemaCollection GetTableSchemas(){
			if ( !_tableInfoLoaded )
				throw new Exception("Table Schema has not been loaded.");
			return _tpsTables;
		}
		
		public void Close(){
			
			RandomAccess.GetInstance().CloseFile();
		}
		
		/// <summary>
		/// Returns as a dataset with ALL data from ALL tables
		/// </summary>
		/// <returns></returns>
		public DataSet GetTableData(){
			return GetTableData(_tpsTables);
		}
		
		/// <summary>
		/// Returns a dataset with one more datatables
		/// </summary>
		/// <param name="tableNames"></param>
		/// <returns></returns>
		public DataSet GetTableData(TableSchemaCollection tableSchemas){
			
			DataSet retSet = new DataSet();
			if ( tableSchemas.Count < 1 )
				return retSet; //return empty set if we aren't actually searching for anything
			
			//create a database for each of the schemas
			Dictionary<int, DataTable> dataTables = new Dictionary<int, DataTable>();
			foreach( TableSchema schema in tableSchemas.Values ){
				DataTable dt = schema.BuildEmptyDataTable();
				dataTables.Add(schema.TableID, dt);
			}
			
			try{
				//1. Fetch all of the TPSRecords from the TPS file
				foreach(TPSPage page in _tpsPages ){
					
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
							
							
							if ( tableSchemas.ContainsKey(recordTableID) ){
								//we generate the record. This will actually create a datarow within the TDR
								Record.TableDataRecord tdr = new Record.TableDataRecord(record, tableSchemas[recordTableID]);
							
								dataTables[recordTableID].Rows.Add(tdr.TableDataRow.ItemArray);
							}
						}
						          
					}
				}
			}catch(Exception ex ){
				throw new Exception("Error searching for data: ", ex);
			}
			foreach( DataTable dt in dataTables.Values)
				retSet.Tables.Add(dt);
			
			return retSet;
			
			
		}
		
		/// <summary>
		/// Will export data from all tables in the collection to a CSV
		/// </summary>
		/// <param name="tables"></param>
		/// <returns></returns>
		public bool ExportDataToCSV(TableSchemaCollection tableSchemas, string outputFolder){
			
			if ( tableSchemas.Count < 1 )
				return true; //return empty set if we aren't actually searching for anything
			
			//create a database for each of the schemas
			Dictionary<int, DataTable> dataTables = new Dictionary<int, DataTable>();
			Dictionary<int, StreamWriter> outputFiles = new Dictionary<int, StreamWriter>();
			foreach( TableSchema schema in tableSchemas.Values ){
				
				if ( outputFiles.ContainsKey(schema.TableID) )
					continue;
				
				string newFile = outputFolder + "\\";
				FileInfo fi = new FileInfo(_filename);
				if ( fi.Extension.Length > 0 )
					newFile += fi.Name.Replace(fi.Extension, "-" + schema.TableName + ".CSV");
				else
					newFile += fi.Name + "-" + schema.TableName + ".CSV";
				
				StreamWriter sw = new StreamWriter(newFile);
				outputFiles.Add(schema.TableID, sw);
				
				DataTable dt = schema.BuildEmptyDataTable();
				string[] columnNames = dt.Columns.Cast<DataColumn>().
					Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"").
                                  ToArray();
				outputFiles[schema.TableID].WriteLine(string.Join(",", columnNames));
				dataTables.Add(schema.TableID, dt);
			}
			try{
				//1. Fetch all of the TPSRecords from the TPS file
				foreach(TPSPage page in _tpsPages ){
					
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
							
							
							if ( tableSchemas.ContainsKey(recordTableID) ){
								//we generate the record. This will actually create a datarow within the TDR
								Record.TableDataRecord tdr = new Record.TableDataRecord(record, tableSchemas[recordTableID]);
							
								//dataTables[recordTableID].Rows.Add(tdr.TableDataRow.ItemArray);
								string[] fields = tdr.TableDataRow.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"").
                                    								ToArray();
    											
    							outputFiles[recordTableID].WriteLine(string.Join(",", fields));
							}
						}
						          
					}
				}
			}catch(Exception ex ){
				throw new Exception("Error exporting data: ", ex);
			}
			
			//attempt to close any open files
			foreach(StreamWriter sw in outputFiles.Values){
				try{
					sw.Flush();
					sw.Close();
				}catch(Exception ){}
			}
			return true;
		}
		/// <summary>
		/// Will read the very top head of the TPS file.
		/// It will also fill in the block info list
		/// </summary>
		/// <returns></returns>
		private bool ReadTPSHeaderInfo(){
			
			_tpsHeader = new TPSHeader();
			_tpsHeader.Process();
			
			//fill in our blocks
			_tpsBlocks = new List<TPSBlock>();
			long tpsFilelength = RandomAccess.GetInstance().fileSize;
			for (int t = 0; t < _tpsHeader.PageStart.Length; t++) {
	            int ofs = _tpsHeader.PageStart[t];
	            int end = _tpsHeader.PageEnd[t];
	            // Skips the first entry (0 length) and any blocks that are beyond
	            // the file size.
	            if (((ofs == 0x0200) && (end == 0x200)) || (ofs >= tpsFilelength)) {
	                continue;
	            } else {
	            	_tpsBlocks.Add( new TPSBlock(ofs, end));
	            }
	        }
			
			return true;
			
		}
		
		/// <summary>
		/// The Type files is split into blocks -> pages -> records
		/// The blocks are listed in the TPS header
		/// We will get the pages along with their addresses for use later on
		/// </summary>
		/// <returns></returns>
		private bool GetTPSPageInfo(){
			
			_tpsPages = new List<TPSPage>();
			foreach(TPSBlock block in _tpsBlocks ){
				_tpsPages.AddRange( block.getPages());
			}
			
			return true;
		}
		
		/// <summary>
		/// Will generate all the table information for the database
		/// This will not collect or export data
		/// This must be done prior to trying any export
		/// </summary>
		/// <returns></returns>
		private bool GetTPSTableSchema(){
			
			List<TPSRecord> records = new List<TPSRecord>();
			Dictionary<int, List<TPSRecord>> tableDefChunks = new Dictionary<int, List<TPSRecord>>(); //used to collect the chunks prior to parsing
			Dictionary<int, string> tableNameList = new Dictionary<int, string>();
			
			int processedPages = 0;
			
			try{
				//1. Fetch all of the TPSRecords from the TPS file
				foreach(TPSPage page in _tpsPages ){
					//Get TPSRecords which hold table info from all pages
					List<TPSRecord> pageRecords = page.GetRecords();
					
					foreach( TPSRecord record in pageRecords){
						//go through each record to see if we need it
						if ( record.RecordType == TPSRecord.TYPE_TABLE_NAME )
						{
							Record.TableNameRecord ti = new Record.TableNameRecord(record);
							
							
							if ( !tableNameList.ContainsKey(ti.TableID) )
								tableNameList.Add(ti.TableID, ti.TableName);
							
						}else if ( record.RecordType == TPSRecord.TYPE_INDEX ){
							//Record.TableIndexRecord ti = new Record.TableIndexRecord(record);
							//result += ti.ToString();
							;
						}else if ( record.RecordType == TPSRecord.TYPE_TABLE_DEFINITION ) {
							Record.TableDefinitionRecord td = new Record.TableDefinitionRecord(record);
							
							//check if the table is in our list of chucks
							if ( !tableDefChunks.ContainsKey(td.TableID) )
								tableDefChunks.Add (td.TableID,  new List<TPSRecord>() );
							
							//ensure the list is large enough
							while ( tableDefChunks[td.TableID].Count <= td.BlockID )
								tableDefChunks[td.TableID].Add(null); //put in a null filler
							
							//place our block in it's place
							tableDefChunks[td.TableID][td.BlockID] = record;
						}
						          
					}
					processedPages++;
					
				}
				
				
				//We have gone through all of the pages
				//Now process the table definitions and metadata
				foreach( KeyValuePair<int, List<TPSRecord>> definitionChucks in tableDefChunks){
					//first ensure all chucks are there.
					
					MemoryStream ms = new MemoryStream();
					
					for(int i=0; i<definitionChucks.Value.Count; i++){
						if ( definitionChucks.Value[i] == null )
							throw new Exception("Incomplete Table Definition Data");
						
						//merge all the data together.
						//notice that we skip the first 7 bytes because this has header data
						ms.Write(definitionChucks.Value[i].RecordData, 7, definitionChucks.Value[i].RecordData.Length-7);
					}
					
					//Build the Table Schema
					TableSchema ts = new TableSchema();
					ts.ProcessTableDefinitionData(definitionChucks.Key, ms.ToArray());
					if ( tableNameList.ContainsKey(definitionChucks.Key ) )
						ts.TableName = tableNameList[definitionChucks.Key];
					else{
						ts.TableName = "UNKNOWN_NAME_" + ts.TableID;
					}
					
					
					_tpsTables.Add(definitionChucks.Key, ts);
				} 
				
			}catch(Exception ex){
				throw new Exception("Pages procssed: " + processedPages , ex);
			}
			
			
			_tableInfoLoaded = true;
			return true;
		}
		
		/// <summary>
		/// Creates a new DataReader for the table specified
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public TPSDataReader NewDataReader(string tableName){
			
			int index = _tpsTables.searchTableName(tableName);
			if ( index < 0 )
				throw new Exception("Cannot create DataReader for a table that does not exist");
			TPSDataReader tdr = new TPSDataReader(_tpsPages, _tpsTables[index]);
			return tdr;
		}
	}
}
