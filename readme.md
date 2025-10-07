# Currency Converter Program

Thank you for reviewing my currency converter program. The current functionality includes:

## Features

1. **Menu of options** – Choose what the program does:
   - **Option 1:** Convert USD to any currency in the database.
     - All user input is validated with error handling to ensure only correct data is used.
   - **Option 2:** View the exchange rate of a currency relative to USD.

## Setup Instructions

Before running the program, make sure the rates file exists and is converted from SQL to a `.db` file:

```bash
sqlite3 rates.db < rates.sql
