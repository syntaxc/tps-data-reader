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
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using TPSReader;


namespace TPSReaderGUI
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private TPSReader.TPSReader _tpsReader;
		private PleaseWaitForm _pleaseWait;
		private TableSchemaCollection _tableInfo;
		private string _tpsFilename = "";
		private int _selectedTableID = -1;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			_pleaseWait = new PleaseWaitForm();
			
		}
		
		
		
		void OpenTPSFileToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenTPSFile();
			
		}
		
		private void OpenTPSFile(){
			if ( openFileDialog1.ShowDialog() != DialogResult.OK )
				return;
			
			if ( fileInfoBW.IsBusy )
				return;
			
			_tpsFilename = openFileDialog1.FileName;
			
			fileInfoBW.RunWorkerAsync(openFileDialog1.FileName);
			//show the please wait
			_pleaseWait = new PleaseWaitForm();
			_pleaseWait.ShowDialog();
		}
		
		#region File Info Background Worker
		void FileInfoBWDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			
			string filename = e.Argument as string;
			if ( !System.IO.File.Exists(filename) )
				throw new Exception("TPS File does not exist");
			
			_tpsReader = new TPSReader.TPSReader(filename);
			_tpsReader.Open();
			
			//Process the TPS File
			_tpsReader.Process();
				
			//Get The Table List
			TableSchemaCollection tableSchemas = _tpsReader.GetTableSchemas();
			
			e.Result = tableSchemas;
			
			return;
			
		}
		
		void FileInfoBWRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			//Close the Please wait
			if ( _pleaseWait != null )
			{
				try{
					_pleaseWait.Close();
				}catch(Exception ){}
			}
			if ( e.Error != null ){
				MessageBox.Show(e.Error.ToString());
				return;
			}
			
			if ( e.Result == null )
				return;
			
			//save our new table info
			_tableInfo = e.Result as TableSchemaCollection;
			
			//clear out the other GUI items
			tableList.Items.Clear();
			tableFieldList.Items.Clear();
			
			try{
				//setup our new gui
				foreach( TableSchema ts in _tableInfo.Values ){
					if ( ts.TableName == null )
						MessageBox.Show("Table Name is null:  " + ts.TableID.ToString());
					tableList.Items.Add(ts.TableName);
				}
			}catch(Exception ex ){
				MessageBox.Show("Error: Cannot add table to table list: " + ex.ToString());
			}
			
			_selectedTableID = -1;
			toolStripLoadedFile.Text = "File: " + _tpsFilename;
			
		}
		#endregion
		
		
		void TableListSelectedIndexChanged(object sender, EventArgs e)
		{
			if ( tableList.SelectedItems.Count < 1 )
				return;
			
			string tableName = tableList.SelectedItems[0].ToString();
			LoadFieldData(tableName);
			viewDataTb.Enabled = true;
			exportToCSVBtn.Enabled = true;
			
		}
		private void LoadFieldData(string tableName){
			
			//find the table schema
			int tsIndex = _tableInfo.searchTableName(tableName);
			if ( tsIndex < 0 )
				return;
			
			TableSchema ts = _tableInfo[tsIndex];
			
			tableFieldList.Items.Clear();
			foreach( TableField tf in ts.Fields ){
				string[] row = new string[5];
				
				tableFieldList.Items.Add( new ListViewItem( new string[]{ tf.Index.ToString(), tf.FieldName, tf.getFieldTypeName(), tf.Length.ToString(), tf.Offset.ToString()}));
			}
			_selectedTableID = tsIndex;
			
			
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void ViewDataTbClick(object sender, EventArgs e)
		{
			
			if ( tableDataBW.IsBusy )
				return;
			
			tableDataBW.RunWorkerAsync(_selectedTableID);
			
			_pleaseWait = new PleaseWaitForm();
			_pleaseWait.ShowDialog();
		}
		
		
		
		#region Table Data Background Worker
		void TableDataBWDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			
			if ( _tpsReader == null )
				throw new Exception("TPS Reader is null. Is a file open?");
			
			int tableIndex = e.Argument as int? ?? -1;
			if ( !_tableInfo.ContainsKey(tableIndex) )
				return;
			
			
			//prep our selected tables - GetTableData can look for multiple tables/
			//we'll only be searching for one
			TableSchemaCollection tsc = new TableSchemaCollection();
			tsc.Add(tableIndex, _tableInfo[tableIndex]);
			e.Result = _tpsReader.GetTableData(tsc);
			
			return ;
		}
		
		void TableDataBWRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			
			if ( _pleaseWait != null )
			{
				try{ _pleaseWait.Close(); }catch(Exception){}
				
			}
			
			if ( e.Error != null )
			{
				MessageBox.Show("Error while loading data: " + e.Error.ToString());
				return;
			}
			
			if ( e.Result == null ){
				MessageBox.Show("Error: No data returned");
				return;
			}
			
			DataSet ds = e.Result as DataSet;
			Application.DoEvents();
			if ( ds.Tables.Count > 0 )
				(new DataTableViewerForm(ds.Tables[0])).ShowDialog();
			
			
		}
		#endregion
		
		#region export
		
		void ExportBWDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			if ( _tpsReader == null )
				throw new Exception("TPS Reader is null. Is a file open?");
			
			object[] args = e.Argument as object[];
			
			int tableIndex = args[0] as int? ?? -1;
			if ( !_tableInfo.ContainsKey(tableIndex) )
				return;
			
			string outputFolder = args[1] as string ?? "";
			if ( String.IsNullOrEmpty(outputFolder) )
				return;
			
			//prep our selected tables - GetTableData can look for multiple tables/
			//we'll only be searching for one
			TableSchemaCollection tsc = new TableSchemaCollection();
			tsc.Add(tableIndex, _tableInfo[tableIndex]);
			e.Result = _tpsReader.ExportDataToCSV(tsc, outputFolder);
			
			return ;
		}
		
		void ExportBWRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if ( _pleaseWait != null )
			{
				try{ _pleaseWait.Close(); }catch(Exception){}
				
			}
			
			if ( e.Error != null )
			{
				MessageBox.Show("Error while loading data: " + e.Error.ToString());
				return;
			}
			
			MessageBox.Show("Export Complete");
		}
		void ExportToCSVBtnClick(object sender, EventArgs e)
		{
			if ( exportBW.IsBusy )
				return;
			try{
				System.IO.FileInfo fi = new System.IO.FileInfo(_tpsFilename);
				exportFolder.SelectedPath = fi.DirectoryName;
			}catch(Exception){}
			if ( exportFolder.ShowDialog() != DialogResult.OK )
				return;
			
		exportBW.RunWorkerAsync(new object[] {_selectedTableID, exportFolder.SelectedPath});
			
			_pleaseWait = new PleaseWaitForm();
			_pleaseWait.ShowDialog();
		}
		#endregion
		
		
	}
}
