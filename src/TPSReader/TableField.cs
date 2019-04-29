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
	/// Description of TableField.
	/// </summary>
	public class TableField
	{
		private int _fieldType;
	    private int _offset;
	    private string _fieldName;
	    private int _elements;
	    private int _length;
	    private int _flags;
	    private int _index;
	
	    private int _stringLength;
	    private string _stringMask;
	
	    private int _bcdDigitsAfterDecimalPoint;
	    private int _bcdLengthOfElement;
	    
	    #region public variables
	    public int FieldType{
	    	get{
	    		return _fieldType;
	    	}
	    }
	    public int Offset{
	    	get{
	    		return _offset;
	    	}
	    }
	    public string FieldName{
	    	get{
	    		return _fieldName;
	    	}
	    }
	  	public int Elements{
	    	get{
	    		return _elements;
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
	    public int Index{
	    	get{
	    		return _index;
	    	}
	    }
	    public int BcdDigitsAfterDecimalPoint{
	    	get{
	    		return _bcdDigitsAfterDecimalPoint;
	    	}
	    }
	    public int BcdLengthOfElement{
	    	get{
	    		return _bcdLengthOfElement;
	    	}
	    }

	    #endregion
	    
		public TableField()
		{
		}
		public TableField(ref RandomAccess ra){
			_fieldType = ra.leByte();
	        _offset = ra.leShort();
	        _fieldName = ra.zeroTerminatedString();
	        //remove the table name from the start if it is there
	        if ( _fieldName.IndexOf(':') > 0 )
	        	_fieldName = _fieldName.Substring(_fieldName.IndexOf(':')+1);
	        _elements = ra.leShort();
	        _length = ra.leShort();
	        _flags = ra.leShort();
	        _index = ra.leShort();
	        
	         switch (_fieldType) {
		        case 0x0a:
	        		_bcdDigitsAfterDecimalPoint = ra.leByte();
		            _bcdLengthOfElement = ra.leByte();
		            break;
		        case 0x12:
		        case 0x13:
		        case 0x14:
		            _stringLength = ra.leShort();
		            _stringMask = ra.zeroTerminatedString();
		            if (_stringMask.Length == 0) {
		                ra.leByte();
		            }
		            break;
		        }
			
		}
		
		public String getFieldTypeName() {
	        switch (_fieldType) {
	        case 1:
	            return "BYTE";
	        case 2:
	            return "SIGNED-SHORT";
	        case 3:
	            return "UNSIGNED-SHORT";
	        case 4:
	            return "DATE";
	        case 5:
	            return "TIME";
	        case 6:
	            return "SIGNED-LONG";
	        case 7:
	            return "UNSIGNED-LONG";
	        case 8:
	            return "Float";
	        case 9:
	            return "Double";
	        case 0x0A:
	            return "BCD";
	        case 0x12:
	            return "fixed-length STRING";
	        case 0x13:
	            return "zero-terminated STRING";
	        case 0x14:
	            return "pascal STRING";
	        case 0x16:
	            return "GROUP";
	        default:
	            return "unknown";
	        }
	    }
	
		public bool isArray(){
			return _elements > 1;
		}
	    /**
	     * checks if this field is a group field. Group fields
	     * are overlays on top of existing fields and as such
	     * may contain text or binary.
	     * @return true if this field is a group field.
	     */
	    public bool isGroup() {
	        return _fieldType == 0x16;
	    }
	
	    /**
	     * checks if this field fits in the given group field.
	     * @param group the group field.
	     * @return true if it fits.
	     */
	    public bool isInGroup(TableField group) {
	        return ((group.Offset <= _offset) && ((group.Offset + group.Length) >= (_offset + _length)));
	    }
	    
	    public override string ToString()
		{
			return string.Format("[TableField FieldType={0}, Offset={1}, FieldName={2}, Elements={3}, Length={4}, Flags={5}, Index={6}, StringLength={7}, StringMask={8}, BcdDigitsAfterDecimalPoint={9}, BcdLengthOfElement={10}]\n", _fieldType, _offset, _fieldName, _elements, _length, _flags, _index, _stringLength, _stringMask,
_bcdDigitsAfterDecimalPoint, _bcdLengthOfElement);
		}


	}
}
