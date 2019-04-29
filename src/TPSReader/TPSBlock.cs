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
	/// Description of TPSBlock.
	/// </summary>
	public class TPSBlock
	{
		private int _start;
		private int _end;
		public TPSBlock(int start, int end)
		{
			_start = start;
			_end = end;
		}
		public int Start{
			get{
				return _start;
			}set{
				_start = value;
			}
			
		}
		public int End{
			get{
				return _end;
			}
			set{
				_end = value;
			}
		}
		
		
		/// <summary>
		/// Will return a list of all pages in the block
		/// </summary>
		/// <returns></returns>
		public List<TPSPage> getPages(){
			
			RandomAccess ra = RandomAccess.GetInstance();
			List<TPSPage> pages = new List<TPSPage>();
			ra.jumpAbs(_start); //jump to the start of the block
			
			try{
				while ( ra.position < _end ){ //while we have not fallen off the end of the file
					
					TPSPage page = new TPSPage(); 
					page.Process(); 
					pages.Add(page);
					
					//pages always start on the position % 100 = 0
					//So let's jump forward to find the next start
					if ( (ra.position & 0xFF) != 0x00 ){
						ra.jumpAbs( ( ra.position & 0xFFFFFF00L) + 0x0100);
					}
					
					//we can find the next page because the address of the page will be in the data
					int addr = 0;
					if ( !ra.isAtEnd() ) {
						do {
							addr = ra.leLong();
							ra.jumpRelative(-4); //backup 4 bytes
							
							if ( addr != ra.position){
								ra.jumpRelative(0x0100);
							}
						}while ( (addr != ra.position) && !ra.isAtEnd());
					}
					
				}
			}catch(Exception ex ){
				;
			}
			
			return pages;
		}
	}
}
