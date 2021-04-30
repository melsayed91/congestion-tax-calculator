# Congestion Tax Calculator!

Simple API to calculate toll fees for vehicles

# Features
 - Web APIs
 - Swagger Documentation
 - Unit Tests
 - All the rules are in configuration file ( can be in database )
## Issues Found
 -  Between 09:00-09:30, 10:00-10:30, 11:00-11:30, 12:00-12:30, 13:00-13:30 and 14:00-14:30  `GetTollFee`  would always return zero for all vehicle types. 
- `public int GetTollFee(Vehicle vehicle, DateTime[] dates)`  Is order sensitive and assumes that passages DateTimes are passed in ascending order. Further more an incorrect assumption is on this line:

```
 long diffInMillies = date.Millisecond - intervalStart.Millisecond;

```

This is the millisecond part of the DateTime. So this expression can equal to zero even if two different DateTimes are compared. Further more fees are not properly group together by hour, only the first hour might work. I rewrote the whole method to fix all these issues.
-  `private Boolean IsTollFreeDate(DateTime date)`  should return true if it's a national holiday. This method returns true for some holidays during 2013. I remedied this using the PublicHoliday package so I don't have to deal with calculating when Easter is for example.
-  It appears to miss some of the vehicle types. For example, trucks, busses, taxi etc.

## Further Development 

 1. Logging
 2. API Authentication
 3. Use external database to configure the calculation rules, probably will need some Admin UI to manage those values
 4. Add docker support 

## Run Project Locally
navigate to API folder and run the following commands
``cd TaxCalculatorService.API/``
``dotnet run``

you can open the generated Swagger documentations by open a browser on the following URL
``https://localhost:5001/index.html``
or you can try the API using Postman

## Run Tests
navigate to the solution folder, open any command line tool and execute the following command
``dotnet test``