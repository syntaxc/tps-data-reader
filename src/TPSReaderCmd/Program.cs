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
using System.IO;
using NDesk.Options;
using TPSReader;


namespace TPSReaderCmd
{
	class Program
	{
		private static bool _verbose;
		private static bool _quiet;
		private static string _sourceFile;
		private static string _sourceDirectory;
		private static string _outputDirectory;
		private static List<string> _tables;
		
		public static void Main(string[] args)
		{
			
			
			bool show_help = false;
			_verbose = false;
			_quiet = false;
			
			_outputDirectory = System.IO.Directory.GetCurrentDirectory();
			_tables = new List<string>();
	
	        var p = new OptionSet () {
	            { "s|source=", "the SOURCE TPS file.\nCannot be used with directory.",
	              v => _sourceFile = v },
	            { "d|directory=", 
	                "the DIRECTORY which to convert ALL TPS files.\n" + 
	                    "Cannot be used with source.\nCannot be used with tables.",
	              v => _sourceDirectory = v },
	            { "o|output=", "the OUTPUT directory in which the CSV files will be saved.\n"
	        			+ "Default is current working directory",
	              v => _outputDirectory = v },
				{ "t|tables=", "limit conversion only to these tables.\n"
	        			+ "You can use this multiple times.\nCannot use with DIRECTORY.",
					v => _tables.Add(v) },
				{ "v|verbose", "Verbose for more information.",
	              v => _verbose = true },
				{ "q|quiet", "Quiet. Will only output errors. Use this if scheduling conversions.",
	              v => _quiet = true },
	            { "h|help",  "show this message and exit", 
	              v => show_help = v != null },
	        };
			
			 try {
	            p.Parse (args);
	        }
	        catch (OptionException e) {
	            Console.Write ("bundling: ");
	            Console.WriteLine (e.Message);
	            Console.WriteLine ("Try `TPSReaderCmd --help' for more information.");
	            return;
	        }
			if (show_help) {
	            ShowHelp (p);
	            return;
	        }
			
			if ( string.IsNullOrEmpty(_sourceDirectory) && string.IsNullOrEmpty(_sourceFile)){
				Console.Error.WriteLine("ERROR You must select a source file or source directory");
				Console.Error.WriteLine ("Try `TPSReaderCmd --help' for more information.");
				return;
			}
			//ensure we do not have both sourceFile and sourceDirectory
			if ( !string.IsNullOrEmpty(_sourceDirectory) && !string.IsNullOrEmpty(_sourceFile )){
				Console.Error.WriteLine("ERROR with arguments: Cannot set both source file and source directory. Use only one.");
				Console.Error.WriteLine ("Try `TPSReaderCmd --help' for more information.");
				return;
			}
			
			//ensure we are not limiting tables for sourceDirectory
			if ( !string.IsNullOrEmpty(_sourceDirectory) && _tables.Count > 0 ){
				Console.Error.WriteLine("ERROR with arguments: Cannot limit the table output when specifying a source directory.");
				Console.Error.WriteLine ("Try `TPSReaderCmd --help' for more information.");
				return;
			}
			
			//lets get parsing
			if ( !string.IsNullOrEmpty(_sourceFile )){
				ConvertSingleFile();
			}else if ( !string.IsNullOrEmpty(_sourceDirectory)){
				ConvertDirectory();
			}
			
			if ( !_quiet )
				Console.WriteLine("Done");
		}
		
		/// <summary>
		/// Converts one file
		/// </summary>
		static void ConvertSingleFile(){
			if ( !File.Exists(_sourceFile )){
				Console.Error.WriteLine("Error: Cannot find source file: " + _sourceFile);
				return;
			}
			if ( !Directory.Exists(_outputDirectory)){
				Console.Error.WriteLine("Error: Cannot find output directory: " + _outputDirectory);
				return;
			}
			
			if ( !_quiet )
				Console.WriteLine("Starting conversion of file: " + _sourceFile);
			//Open th TPS File
			TPSReader.TPSReader tpsR = new TPSReader.TPSReader(_sourceFile);
			tpsR.Open();
			tpsR.Process(); //process the file
			
			TableSchemaCollection tsc = tpsR.GetTableSchemas();
			TableSchemaCollection limittoTables = new TableSchemaCollection();
			
			//If we are going to limit the table output... then make sure they are in the collection
			if ( _tables.Count > 0 ){
				foreach(TableSchema ts in tsc.Values)
					if ( _tables.Contains(ts.TableName) )
						limittoTables.Add(ts.TableID, ts);
			}
			
			if ( _tables.Count > 0 )
				tpsR.ExportDataToCSV(limittoTables, _outputDirectory);
			else
				tpsR.ExportDataToCSV(tsc, _outputDirectory);
			
			if (!_quiet)
				Console.WriteLine("Done converting file: " + _sourceFile);
			
		}
		static void ConvertDirectory(){
			
			if ( !Directory.Exists(_sourceDirectory) ){
				Console.Error.WriteLine("Error: Cannot find source directory: " + _sourceDirectory);
				return;
			}
			
			if ( !Directory.Exists(_outputDirectory)){
				Console.Error.WriteLine("Error: Cannot find output directory: " + _outputDirectory);
				return;
			}
			
			string[] TPSFiles = Directory.GetFiles(_sourceDirectory, "*.TPS");
			
			foreach( string TPSFile in TPSFiles){
				if ( !_quiet )
					Console.WriteLine("Starting conversion of file: " + TPSFile);
				try{
					TPSReader.TPSReader tpsR = new TPSReader.TPSReader(TPSFile);
					tpsR.Open();
					tpsR.Process();
					tpsR.ExportDataToCSV(tpsR.GetTableSchemas(), _outputDirectory);
					tpsR.Close();
				}catch(Exception ex){
					Console.Error.WriteLine("Error converting file: " + TPSFile);
					Console.Error.WriteLine("Error: " + ex.ToString());
				}
				if ( !_quiet)
					Console.WriteLine("Done converting file: " + TPSFile);
			}
		}
		 static void ShowHelp (OptionSet p)
	    {
	        Console.WriteLine ("Usage:  [OPTIONS]+");
	        Console.WriteLine ("Program used to convert TPS Database files to CSV.");
	        Console.WriteLine ();
	        Console.WriteLine ("Options:");
	        p.WriteOptionDescriptions (Console.Out);
	    }
	}
}