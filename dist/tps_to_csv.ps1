Add-Type -Path .\TPSReader.dll
$tpsR = New-Object TPSReader.TPSReader("PATH_TO_TPS")
$tpsR.Open()
$tpsR.Process()
$tsc = $tpsR.GetTableSchemas()
$tpsR.ExportDataToCSV($tsc, "PATH_TO_OUTPUT_DIR")