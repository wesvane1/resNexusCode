Thank you for viewing my currency converter program. Current functionality includes:
1. A menu of options in which you can choose what the program does. Those options include:
    1. Option 1: Converting USD to any given currency in the database.
        1. There is error handling that comes along with all user input ensuring that the end product can only be reached with the correct data.
    2. Option 2: View the exchange rate of a currency from USD.

Prior to running the code, ensure that the rates file has been converted from SQL to a .db file by running the line below in your terminal

`sqlite3 rates.db < rates.sql`

You may also run into errors with SQLite. If you do, run the following code in your terminal

`dotnet add package Microsoft.Data.Sqlite`
