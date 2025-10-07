using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

class Program {

  static (int Id, string Code, decimal Rate)[] Rates;

  static void Main() {

    // The dbPath name should be the same name as the .db file you created.
    string dbPath = "rates.db";

    // Initialized the Rates array

    // Lines 17-29 checks to see if the file, then connection exists. If both are true the program runs. If one or both are false the program immediately stops.
    try {
      if (!File.Exists(dbPath)) throw new Exception("DB file not found.");
      
      using var conn = new SqliteConnection($"Data Source={dbPath}");
      conn.Open();

      Rates = ReadData(conn).ToArray();
    }
    catch (Exception e) {
      Console.WriteLine("Error: " + e.Message);
      Environment.Exit(1);
      return;
    }

    Console.WriteLine("Welcome to the currency converter program\nBelow you will find a list of currencies, please refer to that list!");

    bool continueProgram = true;

    // This is where the program starts and ends, based on this while statement
    while(continueProgram){
      // The below 4 lines print the exchange rate codes and ids
      Console.WriteLine("----------------");
      foreach (var rate in Rates){
        Console.WriteLine($"{rate.Id} : {rate.Code}");
      }
      Console.WriteLine("----------------");
      
      // The below line is the printed menu
      Console.WriteLine("Please select the operation you want to complete from the following options:\n1) Perform exchange rate calculation from USD\n2) View an exchange rate\n3) Run tests");
      if (int.TryParse(Console.ReadLine(), out int menuOptionChoice)){
        // Lines 50-55 is the CONVERSION code
        if(menuOptionChoice == 1){
          var userData = getUserData(Rates);
          if(userData != null){
            decimal result = ConvertCurrency(
              userData.Value.CurrencyFrom, 
              userData.Value.CurrencyTo, 
              userData.Value.Amount
            );

            if(result != -99m){
              Console.WriteLine($"\n********\n{userData.Value.Amount} {userData.Value.CurrencyFrom} is {result} {userData.Value.CurrencyTo} (date reference {DateTime.Now})\n********");
            }else {
              Console.WriteLine("An error occured during the calculation, please try again.");
            }
            Console.WriteLine("\nDo you want to perform another conversion or lookup? (Y/N)");
            string continueChoice = Console.ReadLine()?.ToUpper() ?? "N";
            continueProgram = (continueChoice == "Y");
          } else{
            continueProgram = false;
          }
        // Lines 60-89 is the code for viewing an exchange rate.
        } else if (menuOptionChoice == 2){
          bool validChoice = false;
          while(!validChoice){
            Console.WriteLine("From which currency do you want to convert?: ");
            string exchangeRateChoiceFrom = Console.ReadLine()?.ToUpper() ?? string.Empty;
            Console.WriteLine($"What would you like to convert {exchangeRateChoiceFrom} to?: ");
            string exchangeRateChoiceTo = Console.ReadLine()?.ToUpper() ?? string.Empty;
            if(
              exchangeRateChoiceFrom == string.Empty || 
              !Rates.Any(r => r.Code.Equals(exchangeRateChoiceFrom, StringComparison.OrdinalIgnoreCase)) ||
              exchangeRateChoiceTo == string.Empty ||
              !Rates.Any(r => r.Code.Equals(exchangeRateChoiceTo, StringComparison.OrdinalIgnoreCase))
              ){
              Console.WriteLine("One or both currencies you selected do not exist, please try again.\n");
              continue;
            }
            var exchangeRateChoiceFromValue = Rates.FirstOrDefault(r => r.Code == exchangeRateChoiceFrom);
            var exchangeRateChoiceToValue = Rates.FirstOrDefault(r => r.Code == exchangeRateChoiceTo);
            var exchangeRateActualValue = Math.Round((exchangeRateChoiceToValue.Rate/exchangeRateChoiceFromValue.Rate), 2);
            Console.WriteLine($"\n*****\nThe exchange rate from {exchangeRateChoiceFrom} to {exchangeRateChoiceTo} is {exchangeRateActualValue}.\n*****\n");
            validChoice = true;
          }
        Console.WriteLine("\nDo you want to perform another lookup or conversion? (Y/N)");
        string continueChoice = Console.ReadLine()?.ToUpper() ?? "N";
        continueProgram = (continueChoice == "Y");
        // This else statement runs the unit tests.
        } else if(menuOptionChoice == 3){
          RunManualTests();
          continue;
        // The below else statement checks to see if the user selected a vlaue that is within range for the menu.
        }else{
          Console.WriteLine("****\nThe option you selected was invalid, please try again.\n****");
        }
      }
    }
    Console.WriteLine("Thank you for using the currency converter!");
  }

  // The ReadData function takes the db connection and converts the data to a List that we can use for the rest of the program.
  static List<(int Id, string Code, decimal Rate)> ReadData(SqliteConnection conn)
  {
    var Rates = new List<(int Id, string Code, decimal Rate)>();
    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT * FROM Rates";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
      int id = reader.GetInt32(0);
      string code = reader.GetString(1);
      decimal rate = reader.GetDecimal(2);
      Rates.Add((id,code,rate));
    }

    conn.Close();
    return Rates;
  }


  // The getUserData function gets and handles all data from the user.
  private static (string CurrencyFrom, string CurrencyTo, decimal Amount, decimal ExchangeVal)? getUserData((int Id, string Code, decimal Rate)[] Rates)
  {
    string currencyFrom = "";
    string currencyTo = "";
    decimal amount = 0;

    while (true)
    {
      Console.WriteLine("What currency would you like to convert from?: ");
      // * The below line checks to ensure there is a value, if there is not one it is set to null.
      currencyFrom = Console.ReadLine()?.ToUpper() ?? string.Empty;

      Console.WriteLine("What currency would you like to convert to?: ");
      //* The below line checks to ensure there is a value, if there is not one it is set to null.
      currencyTo = Console.ReadLine()?.ToUpper() ?? string.Empty;

      //* Check currencies BEFORE asking for amount. Better UX IMO
      var fromRate = Rates.FirstOrDefault(r => r.Code == currencyFrom);
      var toRate = Rates.FirstOrDefault(r => r.Code == currencyTo);

      //* Pulls the exchange rate from the rates list.
      decimal exchangeVal = toRate.Rate;

      if (fromRate == default || toRate == default) {
        Console.WriteLine("\nOne of both currencies not found. Please try again.\nRefer to the list!\n");
        continue;
      }
      Console.WriteLine($"How much do you want to convert from {currencyFrom} to {currencyTo}?: ");
      if (decimal.TryParse(Console.ReadLine(), out amount) && amount >= 0){
        return (currencyFrom, currencyTo, amount, toRate.Rate);
      } else if(amount < 0){
        Console.WriteLine("The amount submitted was less than 0, please try again.\n");
      } else {
        Console.WriteLine("Invalid amount. Please enter a number.\n");
      }
    }
  }

  // This function is where the conversion actually takes place
  public static decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount)
  {
    // This if statement solves an edge case, that being if the user gets to this method without validated information.
    if (
      string.IsNullOrWhiteSpace(currencyFrom) || 
      string.IsNullOrWhiteSpace(currencyTo) || 
      amount < 0){
      // The return value is -99 for testing purposes, when any negative value is returned that means there was an error, and this value should be returned.
      return -99m;
    }

    // Look up exchange rates in the Rates array
    var fromRate = Rates.FirstOrDefault(r => r.Code.Equals(currencyFrom, StringComparison.OrdinalIgnoreCase));
    var toRate = Rates.FirstOrDefault(r => r.Code.Equals(currencyTo, StringComparison.OrdinalIgnoreCase));

    if (fromRate == default || toRate == default){
      return -99m;
    }

    decimal convertedAmount = Math.Round(amount * (toRate.Rate/fromRate.Rate), 2);
    return convertedAmount;
  }


  // These are unit tests, tests 1-3 should all pass. Test 4 is a designed fail
  static void RunManualTests() {
    Console.WriteLine("Running manual tests...");

    decimal result1 = ConvertCurrency("USD", "PHP", 10m);
    Console.WriteLine(result1 == 431.23m ? "Pass" : "Fail");

    decimal result2 = ConvertCurrency("", "PHP", 10m);
    Console.WriteLine(result2 == -99m ? "Pass" : "Fail");

    decimal result3 = ConvertCurrency("USD", "php", -1m);
    Console.WriteLine(result3 == -99m ? "Pass" : "Fail");

    // Designed fail negative values are never returned
    decimal result4 = ConvertCurrency("USD", "PHP", 10m);
    Console.WriteLine(result4 == -50m ? "Pass" : "Fail");
    
    // This will pass, because if a negative value is entered, -99 will ALWAYS be returned.
    decimal result5 = ConvertCurrency("USD", "PHP", -10m);
    Console.WriteLine(result5 == -99m ? "Pass" : "Fail");
    }
}

