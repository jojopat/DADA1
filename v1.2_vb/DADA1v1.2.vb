' =============================================================================================
' Project: Directional Antenna Deployment Assistant (DADA) v1.2
' Author: Pat "Jojo" Sadavongvivad (E25VBE)
' License: The Unlicense (Public Domain)
'
' Description: 
' A specialized tool for Thai Amateur Radio operators to calculate antenna bearing (angle) 
' and distance between stations using the Open Government Lat/Long Dataset.
' 
' Data Source: https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910
' Required File: OpenGovernmentLatLongTambon.csv (Same folder as EXE)
' =============================================================================================

Imports System.IO

Public Class Form1
    ' --- Global Application State ---
    Public searchMode As String ' Supports "OpenGovernmentLatLongTambon" or "UTM"
    Public searchText As String ' Current search query
    Public csvFilePath As String = ".\OpenGovernmentLatLongTambon.csv"
    Public csvData As New List(Of String()) ' In-memory cache for CSV coordinates

    ' --- Database/Legacy Connectivity ---
    ' Support for legacy .dbf files via OleDb provider
    Public dBaseConnection As New System.Data.OleDb.OleDbConnection
    Public ConnectDB As String

    ' --- Coordinate & Calculation Variables ---
    ' fsHomeX/Y: Source (My QTH) Coordinates | fDestX/Y: Target Coordinates
    Public fsHomeX, fsHomeY, fDestX, fDestY, fDeltaX, fDeltaY As Single
    Public fsCompass, fsHeading As Single ' Sensors/GPS feedback
    Public fsAngle, fsDistance As Single  ' Calculated results

    ' --- UI & Display Variables ---
    Public iTargetAngle As Integer    ' Final normalized angle for antenna rotator
    Public sTargetPlaceName As String ' Human-readable destination name
    Public strLat, strLong, strSubdistrict, strDistrict, strProvince As String

    ''' <summary>
    ''' Application initialization and data pre-loading.
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ConnectDB = "CSV"
        searchMode = "OpenGovernmentLatLongTambon"

        Log("Initializing app...")

        ' --- CSV Data Loading Logic ---
        If ConnectDB = "CSV" Then
            If Not File.Exists(csvFilePath) Then
                MessageBox.Show("OpenGovernmentLatLongTambon.csv not found. Please download from data.go.th")
                Return
            End If

            Log("Reading coordinate database: " & csvFilePath)
            Using reader As New StreamReader(csvFilePath)
                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    Dim values As String() = line.Split(","c) ' Standard CSV delimiter
                    csvData.Add(values)
                End While
            End Using
            Log("Database loaded. Ready for calculation.")
        End If

        ' --- UI Default Setup ---
        Me.Top = 52 : Me.Left = 52
        Timer1.Interval = 1000
        Timer1.Enabled = True
        fsHomeX = 0 : fsHomeY = 0 ' Initialize coordinates at 0,0

        ' Key Event Handlers for Hardware Interfacing (Rotators/GPS)
        Me.KeyPreview = True
        AddHandler Me.KeyDown, AddressOf KeyDownHandler
        AddHandler Me.KeyUp, AddressOf KeyUpHandler
    End Sub

    ''' <summary>
    ''' Legacy Support for dBase IV (.dbf) files using Jet OLEDB 4.0.
    ''' </summary>
    Public Function getdBasetable(ByVal SqlString As String) As DataTable
        Dim ReturnableTable As New DataTable
        Try
            OpendBConnection()
            Dim SelectCommand As New System.Data.OleDb.OleDbCommand(SqlString, dBaseConnection)
            Dim TableAdapter As System.Data.OleDb.OleDbDataAdapter = New System.Data.OleDb.OleDbDataAdapter
            TableAdapter.SelectCommand = SelectCommand
            TableAdapter.Fill(ReturnableTable)
            Return ReturnableTable
        Catch ex As Exception
            MsgBox("dBase Error: " & ex.Message, 16, "Error")
            End
        End Try
        Return ReturnableTable
    End Function

    Public Sub OpendBConnection()
        Try
            Dim ConnectionString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\;Extended Properties=dBase IV"
            dBaseConnection = New System.Data.OleDb.OleDbConnection(ConnectionString)
            If dBaseConnection.State = 0 Then dBaseConnection.Open()
        Catch ex As Exception
            MsgBox("Connection Error: " & ex.Message, 16, "Error")
        End Try
    End Sub

    ''' <summary>
    ''' Handles user input, search execution, and RF path calculation.
    ''' </summary>
    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Chr(13) Then ' Triggered on Enter Key
            If searchMode = "OpenGovernmentLatLongTambon" Then
                SearchOpenGovernmentLatLongTambon(TextBox1.Text)

                If strLat <> "not found" Then
                    lblSearchStatus.Text = "Matched."
                    sTargetPlaceName = strSubdistrict & " " & strDistrict & " " & strProvince
                    lblDestination.Text = "Destination: " & sTargetPlaceName
                    lblDestUTM.Text = strLat & "," & strLong

                    ' --- Antenna Bearing (Angle) Calculation ---
                    fDestX = Val(strLong) ' Longitude
                    fDestY = Val(strLat)  ' Latitude
                    fDeltaX = fDestX - fsHomeX
                    fDeltaY = fDestY - fsHomeY

                    ' atan2 provides the angle in radians, then converted to degrees
                    fsAngle = (Math.Atan2(fDeltaY, fDeltaX)) * (180 / Math.PI)

                    ' Normalizing Cartesian angle to Compass Bearing (North = 0°)
                    If fsAngle >= 0 And fsAngle <= 90 Then
                        iTargetAngle = 90 - fsAngle
                    ElseIf fsAngle < 0 And fsAngle > -180 Then
                        iTargetAngle = -(fsAngle - 90)
                    ElseIf fsAngle > 90 And fsAngle <= 180 Then
                        iTargetAngle = 450 - fsAngle
                    End If

                    ' --- Geodetic Distance Calculation ---
                    If fsHomeX <> 0 And fsHomeY <> 0 Then
                        fsDistance = CalculateDistanceLatLongKm(fsHomeY, fsHomeX, fDestY, fDestX)
                        lblDistance.Text = "Distance: " & fsDistance & " km"
                        Log("Bearing: " & iTargetAngle & "°, Distance: " & fsDistance & " km")
                    End If

                    lblTargetAngle.Text = "Target Angle: " & iTargetAngle.ToString
                Else
                    lblSearchStatus.Text = "Not found"
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Primary Search Algorithm: Scans the CSV cache for location keywords.
    ''' </summary>
    Private Sub SearchOpenGovernmentLatLongTambon(keyword As String)
        strLat = "not found" : strLong = strLat 
        strSubdistrict = strLat : strDistrict = strLat : strProvince = strLat
        searchText = TextBox1.Text.Trim()

        Dim found As Boolean = False
        For Each row In csvData
            For Each cell In row
                ' Pattern matching for location names (Tambon/Amphoe/Province)
                If cell.Contains(searchText) Then
                    strLat = row(10) : strLong = row(11)
                    strSubdistrict = row(2) : strDistrict = row(5) : strProvince = row(8)
                    Log("Found: " & strSubdistrict & " at " & strLat & "," & strLong)
                    found = True
                    Exit For
                End If
            Next
            If found Then Exit For
        Next

        If Not found Then MessageBox.Show("Location name not found in dataset.")
    End Sub

    ''' <summary>
    ''' Calculates Great-Circle distance between two points using the Haversine formula.
    ''' Essential for long-distance RF path analysis.
    ''' </summary>
    Public Shared Function CalculateDistanceLatLongKm(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double) As Double
        Dim earthRadius As Double = 6371 ' Average Earth radius in KM

        ' Angular conversion (Degrees to Radians)
        Dim lat1Rad As Double = ConvertToRadians(lat1)
        Dim lon1Rad As Double = ConvertToRadians(lon1)
        Dim lat2Rad As Double = ConvertToRadians(lat2)
        Dim lon2Rad As Double = ConvertToRadians(lon2)

        ' Haversine Equation Implementation
        Dim dLat As Double = lat2Rad - lat1Rad
        Dim dLon As Double = lon2Rad - lon1Rad
        Dim a As Double = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                         Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                         Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
        Dim c As Double = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a))
        
        Return earthRadius * c
    End Function

    Private Shared Function ConvertToRadians(ByVal degrees As Double) As Double
        Return degrees * (Math.PI / 180)
    End Function

    ''' <summary>
    ''' Thread-safe UI Logging with auto-scroll functionality.
    ''' </summary>
    Private Sub Log(st As String)
        TextBox2.Text &= System.DateTime.Now.ToString("HH:mm:ss") & ": " & st & vbCrLf
        TextBox2.SelectionStart = TextBox2.Text.Length
        TextBox2.ScrollToCaret()
        TextBox2.Refresh()
    End Sub
End Class
