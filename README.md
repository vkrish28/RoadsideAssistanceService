# Emergency Roadside Assistance Service
###		Implement a service that helps Geico customers with a disabled vehicle, to get immediate roadside assistance, by connecting
###		them directly with emergency assistance service trucks that are available and operating at nearby locations.
## Functionality
###		� Update location of a service provider
###		� Return the 5 nearest service trucks ordered by ascending distance
###		� Reserve a service provider for a customer
###		� Release a service provider from a customer

## How to run the app
###		� Make sure you have some version of the SQL Server 2022 installed in your local
###		� Pull the code and open the solution in Visual Studio 2022
###		� Locate the appsettings.json file inside the EmergencyAssistanceAPI project and adjust the connection string per your installation
###		� Run the code with (Ctrl+F5)
###		� You should see the Swagger page in your browser

## Data considerations
###		� When you runt the app, the sample list of Assistants and their locations (from ServiceProviders.json) will be loaded to SQL DB
###		� also, sample of 9 customers will be loaded to the SQL DB, their Ids are 1,2,3,4,5,6,7,8 & 9
###		� Please ignore the 'ID' in the ServiceProviders.json file, it is NOT used in the app

## How to test
###		� On the swagger page you should see 4 URIs for the functionalities listed above
##		� NearestAssitants: expects Customer's Longitude, latitude and # of nearest Assistants to be returned
###					This API returns list of the nearest Assistants based on customer's longitude and latitude, below are some samples requests:
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;			{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;               "CustomerLongitude":  -97.426657,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;               "CustomerLatitude": 37.909629,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;               "Limit": 10
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						                        }
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;							"CustomerLongitude": -83.739077,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;							"CustomerLatitude": 32.198597,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;							"Limit": 20
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###								

##		� UpdateAssistantLocation: expectes Id of the Assistant , longitude, latitude representing the current location of the Assistant
###					This API allows the service provider to update their own geolocation, below are some samples requests:
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "AssistantId": 832,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "Longitude": -88.95165,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "Latitude": 38.854012
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "AssistantId": 3645,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "Longitude": -88.955659,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "Latitude": 40.152761
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###		
##		� ReserveAssistant: expects Id of the customer, longitude, latitude, it reserves the most nearest Assistant available per the given coordinates
###					This API allows to reserve nearest available assistant for a customer, below are some samples requests:
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerId": 1,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerLongitude": -88.95165,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerLatitude": 38.854012
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerId": 2,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerLongitude": -88.955659,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerLatitude": 40.152761
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###	
##		� ReleaseAssistant: expectes Id of the customer and Id of assistant
###					This API allows to releases assistant(s) from a customer, below are some samples requests:
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerId": 1,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "AssistantId": 832
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						{
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "CustomerId": 2,
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						  "AssistantId": 3645							
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;						}
###		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;








