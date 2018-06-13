Lees ook de installatie-instructies op https://github.com/microsoftgraph/aspnet-webhooks-rest-sample

1. Required permissions:
* Read my groups
* Read my mail			mail.read
* Read videos

...
API = Microsoft Graph -> 
	Calendars.Read.Shared, 
	Groups.ReadWrite.All (admin), 
	Directory.AccessAsUser.All (admin) -> required for creating schema extensions
	Calendars.Read
API = Office 365 SharePoint Online -> AllSites.Read
API = Office 365 Exchange Online -> Mail.Read
API = Windows Azure Active Directory -> User.Read	

2. Start ngrok op met poort 22116: 
ngrok.exe http 22116 -host-header=localhost:22116

3. Paste https://7217b4ba.ngrok.io into web.config at ida:NotificationUrl

# Database troubleshooting
If you get the following error:
	An exception occurred while initializing the database. See the InnerException for details.
	The underlying provider failed on Open.
Then it could help to delete the TokenCache DB:

	Delete the tokencache.mdf under app_data
	Next, execute the following statements through cmd.exe:
	sqllocaldb.exe stop
	sqllocaldb.exe delete
	sqllocaldb.exe start

# EventManagerDemo


# Office 365 Connected Services
https://developer.microsoft.com/en-us/graph/docs/concepts/office_365_connected_services

Event manager
An event consists of an Office 365 Group with planner for the keeping track of the project, an Exchange Online Mailbox for the mail, an Exchange Online Calendar for meetings, an OneNote for managing, a SharePoint Team Site for storing the documents, an Office 365 Video for storing the introduction movie, etc 

* Create an Event (simple form for creating a Group – should include date of the event, event manager,  confidentiality, event type (simple, medium, complex), Group members. Based on the complexity, we will build a basic schedule and activities in planner 

-	Creating a group via Graph API see: https://github.com/microsoftgraph/uwp-csharp-snippets-rest-sample
-	Add an excel sheet with cost / expenses of the event
-	Store specific group data (confidentiality, start date, end date, budget, etc in common data model) by using Microsoft Graph Extensibility
	https://channel9.msdn.com/Events/Connect/2016/213
 
*	Landing page shows all groups for which the user is a member, or of which he is the event manager 
-	Reading all group data: https://github.com/microsoftgraph/uwp-csharp-snippets-rest-sample
-	Status from planner of the event
-	Status on budget from the Excel sheet

*	Plan a new meeting – Creates a meeting based on availability of all group members 
-	See: http://simonjaeger.com/microsoft-graph-find-meeti33ng-times-api-preview/

# Microsoft.Graph
https://www.nuget.org/packages/Microsoft.Graph
https://github.com/microsoftgraph/msgraph-sdk-dotnet
https://github.com/microsoftgraph/uwp-csharp-connect-sample
https://vlasenko.org/2016/12/22/microsoft-graph-api-insufficient-privileges-to-delete-a-group/

https://developer.microsoft.com/en-us/graph/docs/concepts/extensibility_open_users
https://developer.microsoft.com/en-us/graph/docs/concepts/extensibility_schema_groups
https://github.com/microsoftgraph/MSGraph-SDK-Code-Generator
https://developer.microsoft.com/en-us/graph/docs/concepts/extensibility_overview
Additionally, to create and manage schema extension definitions, an application must be granted the Directory.AccessAsUser.All permission.


# test queries
https://graph.microsoft.com/v1.0/groups?$filter=MailNickname eq 'mynickname'
https://graph.microsoft.com/v1.0/groups?$filter=mseventmanager_EventManagerDemo/mykey eq 'test'
https://graph.microsoft.com/v1.0/groups?$select=id,displayName,mseventmanager_EventManagerDemo&$filter=mseventmanager_EventManagerDemo/ManagedEvent eq true