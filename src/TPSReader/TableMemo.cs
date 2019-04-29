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
	/// Description of TableMemo.
	/// </summary>
	public class TableMemo
	{
		private string _externalFile;
	    private string _name;
	    private int _length;
	    private int _flags;
	    
	    #region public variables
	    public string ExternalFile{
	    	get{
	    		return _externalFile;
	    	}
	    }
	    public string Name{
	    	get{
	    		return _name;
	    	}
	    }
	    public int Length{
	    	get{
	    		return _length;
	    	}
	    }
	    public int Flags{
	    	get{
	    		return _flags;
	    	}
	    }
	    #endregion
		public TableMemo(ref RandomAccess ra)
		{
			_externalFile = ra.zeroTerminatedString();
	        if (_externalFile.Length == 0) {
	            if (ra.leByte() != 1) {
	                throw new Exception("Bad Memo Definition, missing 0x01 after zero string:" + _externalFile);
	            }
	        }
	        _name = ra.zeroTerminatedString();
	        _length = ra.leShort();
	        _flags = ra.leShort();
		}
		
		public bool isMemo() {
	        return (_flags & 0x04) == 0;
	    }
	
	    public bool isBlob() {
	        return (_flags & 0x04) != 0;
	    }
		public override string ToString()
		{
			return string.Format("[TableMemo ExternalFile={0}, Name={1}, Length={2}, Flags={3}]\n", _externalFile, _name, _length, _flags);
		}

	}
	
}
