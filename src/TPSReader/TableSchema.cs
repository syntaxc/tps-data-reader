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


namespace TPSReader
{
	/// <summary>
	/// Description of TableInfo.
	/// </summary>
	public class TableSchema
	{
		private int _id;
		private string _name;
		
		private int _driverVersion;
		private int _recordLength;
		private int _noOfFields;
		private int _noOfMemos;
		private int _noOfIndexes;
		
		private List<TableField> _fields;
		private List<TableMemo> _memos;
		private List<TableIndex> _indexes;
		
		private DataTable _tableDT; //a c# datatable of the schema
		
		public string TableName{
			get{
				return _name;
				
			}set{
				_name = value;
			}
		}
		public int TableID{
			get{
				return _id;
			}
			set{
				_id = value;
			}
		}
		public List<TableField> Fields{
			get{
				return _fields;
			}
		}
		public DataTable DataTableSchema{
			get{
				return _tableDT;
			}
		}
		public TableSchema()
		{
		}
		
		public bool ProcessTableDefinitionData(int tableID, byte[] defintionData){
			
			_id = tableID;
			
			_fields = new List<TableField>();
			_memos = new List<TableMemo>();
			_indexes = new List<TableIndex>();
			
			RandomAccess ra = new RandomAccess();
			ra.OpenStream( new System.IO.MemoryStream(defintionData));
			
			_driverVersion = ra.leShort();
	        _recordLength = ra.leShort();
	        _noOfFields = ra.leShort();
	        _noOfMemos = ra.leShort();
	        _noOfIndexes = ra.leShort();
	        
	        
	        try {
	        	
	            for (int t = 0; t < _noOfFields; t++) {
	                _fields.Add(new TableField(ref ra));
	            }
	        	
	            for (int t = 0; t < _noOfMemos; t++) {
	                _memos.Add(new TableMemo(ref ra));
	            }
	            for (int t = 0; t < _noOfIndexes; t++) {
	                _indexes.Add(new TableIndex(ref ra));
	            }
	        } catch (Exception ex) {
	        	throw new Exception("Bad Table Definition ", ex);
	        }
			
			ra = null;
			_tableDT = BuildEmptyDataTable();
			
			return true;
		}
		
		/// <summary>
		/// Returns a datatable setup for this table schema.
		/// There will be no data in it
		/// </summary>
		/// <returns></returns>
		public DataTable BuildEmptyDataTable(){
			
			DataTable retTable = new DataTable();
			retTable.TableName = _name;
			retTable.Columns.Add("ID");
			foreach( TableField tf in _fields ){
				DataColumn dc = new DataColumn();
				dc.ColumnName = tf.FieldName;
				retTable.Columns.Add(dc);
			}
			return retTable;
		}
		
		
		
		public DataRow parseRow(int id, ref RandomAccess ra) {
	        
			DataRow retRow = _tableDT.NewRow();
			retRow["ID"] = id;
	        for (int t = 0; t < _fields.Count; t++) {
				TableField field = _fields[t];
	            
	            if (field.isArray()) {
	                object[] arr = new object[field.Elements];
	                int fieldSize = field.Length / arr.Length;
	                for (int y = 0; y < arr.Length; y++) {
	                    arr[y] = parseField(field.FieldType, field.Offset + 9 + fieldSize * y, fieldSize, field, ra);
	                }
	                retRow[field.FieldName] = arr;
	                
	            } else {
					retRow[field.FieldName] = parseField(field.FieldType, field.Offset + 9, field.Length, field, ra);
	                
	            }
	        }
	        return retRow;
    	}

	    public object parseField(int type, int ofs, int len, TableField field, RandomAccess ra) {
	        ra.jumpAbs(ofs);
	        switch (type) {
		        case 1:
		            // byte
		            assertEqual(1, len);
		            return ra.leByte();
		        case 2:
		            // short
		            assertEqual(2, len);
		            return ra.leShort();
		        case 3:
		            // unsigned short
		            assertEqual(2, len);
		            return ra.leUShort();
		        case 4:
		            // Date, mask encoded.
		            long date = ra.leULong();
		            if (date != 0) {
		                long years = (date & 0xFFFF0000) >> 16;
		                long months = (date & 0x0000FF00) >> 8;
		                long days = (date & 0x000000FF);
		                return new DateTime((int) years, (int) months, (int) days);
		            } else {
		                return null;
		            }
		        case 5:
		            //
		            // Time, mask encoded.
		            // So far i've only had values with hours and minutes
		            // but no seconds or milliseconds so I've no way of
		            // knowing how to decode these.
		            //
		            //TODO: Fix the time here based on decaseconds
		            int time = ra.leLong();
		            int mins = (time & 0x00FF0000) >> 16;
		            int hours = (time & 0x7F000000) >> 24;
		            //
		            return hours + " " + mins; //
		        case 6:
		            // Long
		            assertEqual(4, len);
		            return ra.leLong();
		        case 7:
		            // Unsigned Long
		            assertEqual(4, len);
		            return ra.leULong();
		        case 8:
		            // Float
		            assertEqual(4, len);
		            return ra.leFloat();
		        case 9:
		            // Double
		            assertEqual(8, len);
		            return ra.leDouble();
		        case 0x0A:
		            // BCD encoded.
		            return ra.binaryCodedDecimal(len, field.BcdLengthOfElement, field.BcdDigitsAfterDecimalPoint);
		        case 0x12:
		            // Fixed Length String
		            return ra.fixedLengthString(len);
		        case 0x13:
		            return ra.zeroTerminatedString();
		        case 0x14:
		            return ra.pascalString();
		        case 0x16:
		            // Group (an overlay on top of existing data, can be anything).
		            return ra.leBytes(len);
		        default:
		            throw new Exception("Unsupported type " + type + " (" + len + ")");
	        }
    
		}
		 private void assertEqual(int length, int value) {
	        if (length != value) {
	            throw new Exception(length + " != " + value);
	        }
	    }
		
		public override string ToString()
		{
			string retStr = @"Table (" + _id + ") " + _name 
				+ " [ Fields: " + _noOfFields + ", Indexes: " + _noOfIndexes + ", Memos: " + _noOfMemos + "] "
				+ _recordLength + " bytes per row\n";
			
			
			foreach ( TableField tf in _fields)
				retStr += "Field: " + tf.FieldName + " of type " + tf.getFieldTypeName() + "(" + tf.FieldType.ToString() + ") at offset: " + tf.Offset.ToString() + "," + tf.Length.ToString() + " bytes\n";
			
			
			string memoStr = "";
			foreach( TableMemo tm in _memos )
				memoStr += tm.ToString() + "\n";
			
			string indexStr = "";
			foreach ( TableIndex ti in _indexes )
				indexStr += ti.ToString() + "\n";
			
			return retStr + "\n";
			//return string.Format("[TableSchema Id={0}, Name={1}, DriverVersion={2}, RecordLength={3}, NoOfFields={4}, NoOfMemos={5}, NoOfIndexes={6}, Fields={7}, Memos={8}, Indexes={9}]", _id, _name, _driverVersion, _recordLength, _noOfFields, _noOfMemos, _noOfIndexes, fieldStr, memoStr,indexStr);
		}

	}
}
