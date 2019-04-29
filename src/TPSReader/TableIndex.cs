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

namespace TPSReader
{
	/// <summary>
	/// Description of TableIndex.
	/// </summary>
	public class TableIndex
	{
		private string _externalFile;
	    private string _name;
	    private int _flags;
	    private int _fieldsInKey;
	    private int[] _keyField;
	    private int[] _keyFieldFlag;
	    
	    #region public variables
	    #endregion
		public TableIndex()
		{
		}
		public TableIndex(ref RandomAccess ra){
			_externalFile = ra.zeroTerminatedString();
	        if (_externalFile.Length == 0) {
	            int read = ra.leByte();
	            if (read != 1) {
	                throw new Exception("Bad Index Definition, missing 0x01 after zero string (" + ra.toHex2(read) + ")");
	            }
	        }
	        _name = ra.zeroTerminatedString();
	        _flags = ra.leByte();
	        _fieldsInKey = ra.leShort();
	        _keyField = new int[_fieldsInKey];
	        _keyFieldFlag = new int[_fieldsInKey];
	        for (int t = 0; t < _fieldsInKey; t++) {
	            _keyField[t] = ra.leShort();
	            _keyFieldFlag[t] = ra.leShort();
	        }
	        
	        
		}
		public override string ToString()
		{
			return string.Format("[TableIndex ExternalFile={0}, Name={1}, Flags={2}, FieldsInKey={3}, KeyField={4}, KeyFieldFlag={5}]\n", _externalFile, _name, _flags, _fieldsInKey, _keyField, _keyFieldFlag);
		}

	}
}
