# Currency Converter Program

Thank you for reviewing my currency converter program. The current functionality includes:

## Features

1. **Menu of options** – Choose what the program does:
   - **Option 1:** Convert from ANY currency in the database to ANY currency in the database.
     - All user input is validated with error handling to ensure only correct data is used.
     - There is a date added for refrence of when the conversion was completed. This can be used for further implementation, when and API is involved, in making real time checks to ensure the accurracy of conversion.
   - **Option 2:** View the exchange rate of ANY currency to ANY currency (found in the database).

## Setup Instructions

Before running the program, make sure the rates file exists and is converted from SQL to a `.db` file:

```bash
sqlite3 rates.db < rates.sql
