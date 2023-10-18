' DADA1 - Directional Antenna Deployment Assistant
'
' BEFORE USE : download the open Government dataset from the URL below :
' https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910
' save the Excel file as CSV under the name OpenGovernmentLatLongTambon.csv
' place the file into same folder as the EXE and run.
'
'
' Development History
'
' First idea : Feb 2009, creation maxtrix table of every district in Thailand when I was E22JNE :)
' by taking coordinate of every district office calculate to create :
'
'     1. Distance matrix
'     2. Angle matrix
'
' Then distribute the angle matrix through ClubStation antenna by HS1BFR Prasit Sottipattanapong
'
' Later in 2019, right after reading this news of how heavily calculation ham operator has to be
' done to calculate the angle to rotate the antenna in order to communicate with aboard HTMS Chakrinarubet
' that I decide to write this application but cannot distribute due to the coordinate data is a
' copyrighted dataset of commercial organization.
'
' https://data.go.th/th/dataset/item_c6d42e1b-3219-47e1-b6b7-dfe914f27910
'
' Just early October 2023 that E25VBE (my renewed callsign) found Lat/Long data of every Tambon in
' OpenGovernmentData web where everyone can download and use.  So I begin to fix the code to
' utilize this open Government dataset.
'
' This is the main part of the app to calculate distance and angle between two stations.  Next step
' is to come up with some hardware to interface to antenna rotator like KenPro and integration with
' CAT-protocol supported tranceivers.
'
' Please feel free to further develop this code for the advancement of Thai hams ^_^
' E25VBE Pat Jojo Sadavongvivad
'

Imports System.IO

Public Class Form1
    ' 2 available value : 1 OpenGovernmentLatLongTambon and 2 UTM
    Public searchMode As String
    Public searchText As String     ' text to search for / ข้อความที่ต้องการค้นหา
    Public csvFilePath As String = ".\OpenGovernmentLatLongTambon.csv" ' path to data file
    Public csvData As New List(Of String())

    ' Declare to handle database connection
    Public dBaseConnection As New System.Data.OleDb.OleDbConnection

    ' fsHomeX & fsHomeY is float single precision Home QTH Lat/Long or UTM
    Public fsHomeX, fsHomeY, fDestX, fDestY, fDeltaX, fDeltaY As Single

    ' fsCompass is electronic compass report angle
    ' fsHeading is routing heading angle reported from GPS
    Public fsCompass, fsHeading As Single
    Public fsAngle, fsDistance As Single

    ' iTargetAngle is target antenna angle pointing to destination location
    Public iTargetAngle As Integer
    Public sTargetPlaceName As String

    ' Return value of a location from OpenGovernmentLatLongTambon
    Public strLat, strLong, strSubdistrict, strDistrict, strProvince As String
    Public ConnectDB As String


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ConnectDB = "CSV"
        searchMode = "OpenGovernmentLatLongTambon"

        Log("Initializing app...")

        'Me.Text = "Directional Antenna Deployment Assistant 1.0 -- F8-Set MyQTH         F9-Set antenna angle"

        If ConnectDB = "CSV" Then
            ' Verify if coordinate data file exist / ตรวจสอบว่าไฟล์ข้อมูลพิกัดตำบล-อำเภอเป็น CSV มีอยู่หรือไม่
            If Not File.Exists(csvFilePath) Then
                MessageBox.Show("ไม่พบไฟล์ / not found OpenGovernmentLatLongTambon.csv, exit app.")
                Return
            End If

            ' Read coordinate data from CSV file into array
            ' อ่านข้อมูลจากไฟล์ CSV และเก็บในรูปแบบ array

            Log("  reading " & csvFilePath)
            Using reader As New StreamReader(csvFilePath)
                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    Dim values As String() = line.Split(","c) ' แยกข้อมูลโดยใช้ "," เป็นตัวแยก
                    csvData.Add(values)
                End While
            End Using
            Log("      done and is ready to run.")
        End If


        If ConnectDB = "ampur.dbf" Then
            Try
                Dim tb As DataTable
                tb = getdBasetable("select * from ampur;")
                DBFDataGrid.DataSource = tb
                TextBox1.Select()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If


        Me.Top = 52 : Me.Left = 52
        DBFDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        Timer1.Interval = 1000
        Timer1.Enabled = True
        fsHomeX = 0
        fsHomeY = 0
        lblMyPlace.AutoSize = True : lblMyPlace.Width = 20
        lblMyPlace.Text = "My QTH: Unset"
        lblDestination.Text = "คู่สถานี / Destination :"
        lblDistance.Text = "ระยะทาง / Distance :"
        lblUTMY.AutoSize = True : lblUTMY.Width = 28
        lblUTMY.Text = "Home LatLong :"
        lblHeading.AutoSize = True : lblHeading.Width = 30
        lblCompass.AutoSize = True : lblCompass.Width = 30
        lblTargetAngle.Text = "Target Antenna Angle :"
        lblCurAngle.Text = "Current Antenna Angle :"



        Me.KeyPreview = True
        AddHandler Me.KeyDown, AddressOf KeyDownHandler
        AddHandler Me.KeyUp, AddressOf KeyUpHandler

    End Sub

    Private Sub LblFunctionKey_Click(sender As Object, e As EventArgs) Handles lblFunctionKey.Click

    End Sub

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
            MsgBox(ex.Message & vbCrLf & SqlString, 16, "Error")
            End
        End Try
        Return ReturnableTable
    End Function

    Public Sub OpendBConnection()
        Try
            Dim ConnectionString As String
            ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\;Extended Properties=dBase IV"
            dBaseConnection = New System.Data.OleDb.OleDbConnection(ConnectionString)
            If dBaseConnection.State = 0 Then dBaseConnection.Open()
        Catch ex As Exception
            MsgBox(ex.Message, 16, "Error")
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        'Label1.Text = TextBox1.Text
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress

        If e.KeyChar = Chr(13) Then
            Dim temp As Integer = 0
            Dim j As Integer = 5 ' Search in District name field
            Dim sTmp As String
            Dim iPos As Integer
            Dim sSearchFor, sCellToSearch As String

            If searchMode = "OpenGovernmentLatLongTambon" Then
                SearchOpenGovernmentLatLongTambon(TextBox1.Text)

                If strLat <> "not found" Then
                    lblSearchStatus.Text = "Matched."
                    lblSearchStatus.Refresh()

                    sTargetPlaceName = strSubdistrict & " " & strDistrict & " " & strProvince
                    lblDestination.Text = "Destination :" & sTargetPlaceName
                    lblDestination.Refresh()

                    lblDestUTM.Text = strLat & "," & strLong      ' UTMX
                    lblDestUTM.Refresh()


                    ' calculate angle
                    fDestX = Val(strLong)         ' Destination Longitude
                    fDestY = Val(strLat)          ' Destination Latitude
                    fDeltaX = fDestX - fsHomeX
                    fDeltaY = fDestY - fsHomeY

                    'fsDistance = Math.Sqrt(((fDestX - fsHomeX) ^ 2 + (fDestY - fsHomeY) ^ 2)) / 10                  

                    fsAngle = (Math.Atan2(fDeltaY, fDeltaX)) * (180 / Math.PI)
                    If fsAngle >= 0 And fsAngle <= 90 Then
                        iTargetAngle = 90 - fsAngle
                    ElseIf fsAngle < 0 And fsAngle > -180 Then
                        iTargetAngle = -(fsAngle - 90)
                    ElseIf fsAngle > 90 And fsAngle <= 180 Then
                        iTargetAngle = 450 - fsAngle
                    End If

                    ' verify heading and compass, warn if inconsistent

                    ' compensate angle from heading

                    ' calculate distance

                    If fsHomeX <> 0 And fsHomeY <> 0 Then
                        fsDistance = CalculateDistanceLatLongKm(fsHomeY, fsHomeX, fDestY, fDestX)
                        lblDistance.Text = "Distance :" & fsDistance & " km"
                        lblDistance.Refresh()

                        Log("Angle : " & iTargetAngle & " degree, " & fsDistance & "km.")
                    End If

                    'iTargetAngle = Rnd() * 360
                    lblTargetAngle.Text = "Target Angle :" & iTargetAngle.ToString

                    temp = 1
                Else
                    lblSearchStatus.Text = "Not found"
                    lblSearchStatus.Refresh()
                End If

            End If

            If searchMode = "UTM" Then
                sSearchFor = TextBox1.Text
                For i As Integer = 0 To DBFDataGrid.RowCount - 2
                    'For j As Integer = 0 To DBFDataGrid.ColumnCount - 1
                    iPos = TextBox2.Text.Length
                    iPos = If(iPos > 2000, 2000, iPos)
                    sTmp = TextBox2.Text.Substring(iPos)


                    sCellToSearch = DBFDataGrid.Rows(i).Cells(j).Value.ToString
                    Try
                        lblSearchStatus.Text = i : lblSearchStatus.Refresh()
                        If InStr(sCellToSearch, sSearchFor) > 0 Then
                            sTargetPlaceName = DBFDataGrid.Rows(i).Cells(j).Value.ToString & " " & DBFDataGrid.Rows(i).Cells(4).Value.ToString
                            lblDestination.Text = "Destination :" & sTargetPlaceName
                            lblDestination.Refresh()

                            ' calculate angle
                            fDestX = DBFDataGrid.Rows(i).Cells(1).Value.ToString       ' Destination UTMX
                            fDestY = DBFDataGrid.Rows(i).Cells(2).Value.ToString()     ' Destination UTMY
                            fDeltaX = fDestX - fsHomeX
                            fDeltaY = fDestY - fsHomeY
                            fsDistance = Math.Sqrt(((fDestX - fsHomeX) ^ 2 + (fDestY - fsHomeY) ^ 2)) / 100000
                            fsAngle = (Math.Atan2(fDeltaY, fDeltaX)) * (180 / Math.PI)
                            If fsAngle >= 0 And fsAngle <= 90 Then
                                iTargetAngle = 90 - fsAngle
                            ElseIf fsAngle < 0 And fsAngle > -180 Then
                                iTargetAngle = -(fsAngle - 90)
                            ElseIf fsAngle > 90 And fsAngle <= 180 Then
                                iTargetAngle = 450 - fsAngle
                            End If

                            ' verify heading and compass, warn if inconsistent

                            ' compensate angle from heading

                            ' calculate distance


                            lblDistance.Text = "Distance :" & fsDistance & " km"
                            lblDistance.Refresh()

                            'iTargetAngle = Rnd() * 360
                            lblTargetAngle.Text = "Target Angle :" & iTargetAngle.ToString

                            'Label2.Text = DBFDataGrid.Rows(i).Cells(1).Value.ToString       ' UTMX
                            'Label2.Refresh()
                            'Label3.Text = DBFDataGrid.Rows(i).Cells(2).Value.ToString()     ' UTMY
                            'Label3.Refresh()


                            temp = 1
                            Exit For
                        End If
                    Catch

                    End Try
                    'Next
                Next
                If temp = 0 Then
                    lblDestination.Text = "Not Found"
                    iTargetAngle = -1
                End If
            End If ' End If SearchUTM
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        'fsHomeX = fsHomeX + Rnd() * 50 - 25
        'fsHomeY = fsHomeY + (Rnd() * 50) - 25
        fsHeading = fsHeading + (4 * Rnd() - 2)
        Dim iDisplayHeading As Integer = If(fsHeading < 0, 360 - fsHeading, Int(fsHeading))

        'lblUTMY.Text = "UTMY " & fsHomeY.ToString
        lblHeading.Text = "Heading " & iDisplayHeading.ToString
        lblCompass.Text = "Compass " & iDisplayHeading.ToString

    End Sub


    Private Sub KeyUpHandler(ByVal o As Object, ByVal e As KeyEventArgs)
        e.SuppressKeyPress = True

        ' Key F8 - Process set my QTH

        If e.KeyCode = Keys.F8 Then
            If iTargetAngle >= 0 Then
                fsHomeX = fDestX
                fsHomeY = fDestY
                'TextBox2.AppendText(System.DateTime.Today.ToString & " Set Current position to 47 N " & fsHomeX & " " & fsHomeY & " " & sTargetPlaceName & vbCrLf)
                'TextBox2.AppendText(System.DateTime.Today.ToString & " Set my QTH to " & sTargetPlaceName & vbCrLf)
                Log("#### Set my QTH to " & sTargetPlaceName & " ####")
                TextBox1.Text = ""
                lblUTMY.Text = strLat & "," & strLong      ' UTMX
                lblDestUTM.Refresh()
                lblMyPlace.Text = "MyQTH:" & sTargetPlaceName
            End If
        End If

        ' Shift-F8 to save my QTH
        If e.KeyCode = Keys.LShiftKey And Keys.F8 Then

            Log("save MyQTH " & sTargetPlaceName & "to config file.")
            ' If fsHomeX & Y already initialized, save the coordinate to file

            If fsHomeX > 0 And fsHomeY > 0 Then

                MessageBox.Show("Save HomeQTH to file not implemented yet.")

            End If

        End If


        If e.KeyCode = Keys.F9 Then
            If iTargetAngle >= 0 Then

                Log(" Rotating antenna to " & iTargetAngle & " " & sTargetPlaceName)
                TextBox1.Text = ""
                lblCurAngle.Text = "Current Antenna Angle : " & iTargetAngle

            Else

                Log("Target destination not set.")

            End If
        End If


        If e.KeyCode = Keys.F3 Then




        End If
        'TextBox2.AppendText(
        'String.Format("'{0}' '{1}' '{2}' '{3}' {4}", e.Modifiers, e.KeyValue, e.KeyData, e.KeyCode, Environment.NewLine))
    End Sub

    Private Sub KeyDownHandler(ByVal o As Object, ByVal e As KeyEventArgs)
        'e.SuppressKeyPress = True
        'TextBox2.AppendText(
        'String.Format("'{0}' '{1}' '{2}' '{3}' {4}", e.Modifiers, e.KeyValue, e.KeyData, e.KeyCode, Environment.NewLine))
    End Sub


    Private Sub SearchOpenGovernmentLatLongTambon(keyword As String)

        strLat = "not found" : strLong = strLat : strSubdistrict = strLat : strDistrict = strLat : strProvince = strLat

        searchText = TextBox1.Text.Trim()

        Dim found As Boolean = False

        ' Loop through array searching for matched location name
        ' วนลูปใน array เพื่อค้นหาข้อมูล
        For Each row In csvData
            For Each cell In row
                If cell.Contains(searchText) Then
                    ' ถ้าพบข้อมูลที่ต้องการ / found matched string pattern
                    'MessageBox.Show($"พบข้อมูลที่ค้นหา: {cell} LatLong:" & row(10) & "," & row(11))
                    strLat = row(10) : strLong = row(11)
                    strSubdistrict = row(2) : strDistrict = row(5) : strProvince = row(8)
                    Log("search for " & keyword & " and found " & strSubdistrict & " " & strDistrict & " " & strProvince & " at " & strLat & " / " & strLong)
                    found = True
                    Exit For ' หยุดการค้นหาเมื่อพบข้อมูล / exit
                End If
            Next
            If found Then Exit For
        Next

        If Not found Then
            MessageBox.Show("ไม่พบข้อมูลที่ค้นหา, location name not found.")
        End If
    End Sub


    Public Shared Function CalculateDistanceLatLongKm(
        ByVal lat1 As Double,
        ByVal lon1 As Double,
        ByVal lat2 As Double,
        ByVal lon2 As Double) As Double

        Dim earthRadius As Double = 6371 ' Radius of the Earth in kilometers

        ' Convert latitude and longitude from degrees to radians
        Dim lat1Rad As Double = ConvertToRadians(lat1)
        Dim lon1Rad As Double = ConvertToRadians(lon1)
        Dim lat2Rad As Double = ConvertToRadians(lat2)
        Dim lon2Rad As Double = ConvertToRadians(lon2)

        ' Haversine formula
        Dim dLat As Double = lat2Rad - lat1Rad
        Dim dLon As Double = lon2Rad - lon1Rad
        Dim a As Double = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
        Dim c As Double = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a))
        Dim distance As Double = earthRadius * c

        Return distance
    End Function

    Private Shared Function ConvertToRadians(ByVal degrees As Double) As Double
        Return degrees * (Math.PI / 180)
    End Function

    Private Sub Log(st As String)
        TextBox2.Text = TextBox2.Text & System.DateTime.Today.ToString & ":" & st & vbCrLf
        TextBox2.Refresh()
        ' Scroll to the last line
        TextBox2.SelectionStart = TextBox2.Text.Length
        TextBox2.ScrollToCaret()
    End Sub

End Class

