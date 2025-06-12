' This VBA macro gets values from the current Outlook Appointment (event) form
' and stores them as custom properties (user metadata) on the event.

Sub StoreEventFormFieldsAsMetadata()
    Dim appt As Outlook.AppointmentItem
    Set appt = Application.ActiveInspector.CurrentItem

    ' Example: Get standard fields
    Dim subject As String
    Dim location As String
    Dim startTime As Date
    Dim endTime As Date

    subject = appt.Subject
    location = appt.Location
    startTime = appt.Start
    endTime = appt.End

    ' Store as custom properties (user properties)
    appt.UserProperties.Add "Meta_Subject", olText, True
    appt.UserProperties("Meta_Subject").Value = subject

    appt.UserProperties.Add "Meta_Location", olText, True
    appt.UserProperties("Meta_Location").Value = location

    appt.UserProperties.Add "Meta_StartTime", olDateTime, True
    appt.UserProperties("Meta_StartTime").Value = startTime

    appt.UserProperties.Add "Meta_EndTime", olDateTime, True
    appt.UserProperties("Meta_EndTime").Value = endTime

    ' Example: Get a custom field from the form (replace "CustomFieldName" as needed)
    On Error Resume Next
    Dim customValue As String
    customValue = appt.UserProperties("CustomFieldName").Value
    If customValue <> "" Then
        appt.UserProperties.Add "Meta_CustomField", olText, True
        appt.UserProperties("Meta_CustomField").Value = customValue
    End If
    On Error GoTo 0

    appt.Save
    MsgBox "Event metadata saved to user properties.", vbInformation
End Sub

' Sample subroutine to get and display event form fields from the current Outlook Appointment

Sub ShowEventFormFields()
    Dim appt As Outlook.AppointmentItem
    Set appt = Application.ActiveInspector.CurrentItem

    Dim msg As String
    msg = "Subject: " & appt.Subject & vbCrLf
    msg = msg & "Location: " & appt.Location & vbCrLf
    msg = msg & "Start: " & appt.Start & vbCrLf
    msg = msg & "End: " & appt.End & vbCrLf
    msg = msg & "Body: " & appt.Body & vbCrLf

    ' Example: Get a custom user property (replace "CustomFieldName" as needed)
    On Error Resume Next
    Dim customValue As String
    customValue = appt.UserProperties("CustomFieldName").Value
    If customValue <> "" Then
        msg = msg & "CustomFieldName: " & customValue & vbCrLf
    End If
    On Error GoTo 0

    MsgBox msg, vbInformation, "Event Form Fields"
End Sub

' filepath: ROOT\Projects\M365\Web\Outlook Event Form - June 2025.vba
' Sample function to create an Outlook event (appointment) form with a custom people picker field.
' The people picker is simulated using a custom text field for demonstration.

Sub CreateEventWithCustomPeoplePicker()
    Dim appt As Outlook.AppointmentItem
    Set appt = Application.CreateItem(olAppointmentItem)
    
    ' Set standard fields
    appt.Subject = "Team Meeting"
    appt.Location = "Conference Room"
    appt.Start = Now + 1
    appt.End = Now + 1 + TimeSerial(1, 0, 0) ' 1 hour meeting
    appt.Body = "Agenda: Project updates and planning."

    ' Simulate a people picker by adding a custom user property (comma-separated emails or names)
    Dim peoplePickerValue As String
    peoplePickerValue = InputBox("Enter attendees (comma-separated emails or names):", "Custom People Picker")
    If peoplePickerValue <> "" Then
        appt.UserProperties.Add "CustomPeoplePicker", olText, True
        appt.UserProperties("CustomPeoplePicker").Value = peoplePickerValue
    End If

    appt.Display
End Sub

' filepath: ROOT\Projects\M365\Web\Outlook Event Form - June 2025.vba

' Function to send an approval request email to a user and instruct them to reply Yes or No.
' This function sends the email; you can process replies using Outlook rules or additional VBA.

Sub SendApprovalRequestEmail(recipientEmail As String, subjectText As String, bodyText As String)
    Dim mail As Outlook.MailItem
    Set mail = Application.CreateItem(olMailItem)
    
    mail.To = recipientEmail
    mail.Subject = subjectText
    mail.Body = bodyText & vbCrLf & vbCrLf & _
        "Please reply to this email with 'Yes' to approve or 'No' to reject."
    
    mail.Importance = olImportanceHigh
    mail.Display   ' Use .Send to send immediately, or .Display to review before sending
End Sub

' Example usage:
' Call SendApprovalRequestEmail("user@example.com", "Approval Needed", "Do you approve this event?")

' Note: To process the reply and extract the Yes/No response, you would need to handle incoming mail
' using an Outlook rule or additional VBA code in the ThisOutlookSession module.

' filepath: ROOT\Projects\M365\Web\Outlook Event Form - June 2025.vba

' This code goes in the ThisOutlookSession module.
' It sends an email notification if any field in an Outlook event (appointment) changes.

Private WithEvents myItem As Outlook.AppointmentItem

' Call this subroutine to start monitoring the currently open event
Sub MonitorCurrentEventForChanges()
    Set myItem = Application.ActiveInspector.CurrentItem
End Sub

' This event fires when the item is changed and saved
Private Sub myItem_Write(Cancel As Boolean)
    Static lastSubject As String
    Static lastLocation As String
    Static lastStart As Date
    Static lastEnd As Date
    Static lastBody As String

    Dim changed As Boolean
    changed = False

    ' Compare fields to previous values
    If lastSubject <> "" Then
        If myItem.Subject <> lastSubject Then changed = True
        If myItem.Location <> lastLocation Then changed = True
        If myItem.Start <> lastStart Then changed = True
        If myItem.End <> lastEnd Then changed = True
        If myItem.Body <> lastBody Then changed = True
    End If

    ' Update stored values
    lastSubject = myItem.Subject
    lastLocation = myItem.Location
    lastStart = myItem.Start
    lastEnd = myItem.End
    lastBody = myItem.Body

    If changed Then
        Call NotifyEventChanged(myItem)
    End If
End Sub

' Sends an email notification about the change
Sub NotifyEventChanged(appt As Outlook.AppointmentItem)
    Dim mail As Outlook.MailItem
    Set mail = Application.CreateItem(olMailItem)
    mail.To = "your@email.com" ' <-- Change to your notification recipient
    mail.Subject = "Outlook Event Changed: " & appt.Subject
    mail.Body = "The following event was changed:" & vbCrLf & _
        "Subject: " & appt.Subject & vbCrLf & _
        "Location: " & appt.Location & vbCrLf & _
        "Start: " & appt.Start & vbCrLf & _
        "End: " & appt.End & vbCrLf & _
        "Body: " & appt.Body
    mail.Send
End Sub

' Usage:
' 1. Open an event in Outlook.
' 2. Run MonitorCurrentEventForChanges from the VBA editor.
' 3. When you change and save the event, an email will be sent if any field changes.

' Note: This code must be placed in the ThisOutlookSession module to work with events.

' filepath: ROOT\Projects\M365\Web\Outlook Event Form - June 2025.vba

' Function to get and display all standard fields from the currently selected Outlook calendar event
Sub GetAllStandardEventFields()
    Dim appt As Outlook.AppointmentItem
    On Error Resume Next
    Set appt = Application.ActiveExplorer.Selection.Item(1)
    If appt Is Nothing Then
        MsgBox "No calendar event selected.", vbExclamation
        Exit Sub
    End If
    On Error GoTo 0

    Dim msg As String
    msg = "Subject: " & appt.Subject & vbCrLf
    msg = msg & "Location: " & appt.Location & vbCrLf
    msg = msg & "Start: " & appt.Start & vbCrLf
    msg = msg & "End: " & appt.End & vbCrLf
    msg = msg & "AllDayEvent: " & appt.AllDayEvent & vbCrLf
    msg = msg & "BusyStatus: " & appt.BusyStatus & vbCrLf
    msg = msg & "Categories: " & appt.Categories & vbCrLf
    msg = msg & "Importance: " & appt.Importance & vbCrLf
    msg = msg & "MeetingStatus: " & appt.MeetingStatus & vbCrLf
    msg = msg & "Organizer: " & appt.Organizer & vbCrLf
    msg = msg & "RequiredAttendees: " & appt.RequiredAttendees & vbCrLf
    msg = msg & "OptionalAttendees: " & appt.OptionalAttendees & vbCrLf
    msg = msg & "Resources: " & appt.Resources & vbCrLf
    msg = msg & "ReminderSet: " & appt.ReminderSet & vbCrLf
    msg = msg & "ReminderMinutesBeforeStart: " & appt.ReminderMinutesBeforeStart & vbCrLf
    msg = msg & "Sensitivity: " & appt.Sensitivity & vbCrLf
    msg = msg & "Body: " & appt.Body & vbCrLf
    msg = msg & "EntryID: " & appt.EntryID & vbCrLf
    msg = msg & "GlobalAppointmentID: " & appt.GlobalAppointmentID & vbCrLf
    msg = msg & "CreationTime: " & appt.CreationTime & vbCrLf
    msg = msg & "LastModificationTime: " & appt.LastModificationTime & vbCrLf

    MsgBox msg, vbInformation, "Standard Event Fields"
End Sub