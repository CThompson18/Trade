//+------------------------------------------------------------------+
//|                                                   MqlSqlDemo.mq5 |
//|                        Copyright 2018, MetaQuotes Software Corp. |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright 2018, MetaQuotes Software Corp."
#property link      "https://www.mql5.com"
#property version   "1.00"
//--- input parameters
input string   ConnectionString="Server=localhost;Database=master;Integrated Security=True";

// Description of the imported functions.
#import "MqlSqlDemo.dll"

// Function for opening a connection:
int CreateConnection(string sConnStr);
// Function for reading the last message:
string GetLastMessage();
// Function for executing the SQL command:
int ExecuteSql(string sSql);
// Function for reading an integer:
int ReadInt(string sSql);
// Function for reading a string:
string ReadString(string sSql);
// Function for closing a connection:
void CloseConnection();

// Successful execution of the function:
#define iResSuccess  0
// Error while executing the function:
#define iResError 1

// End of import:
#import

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
   // Try to open a connection:
   if (CreateConnection(ConnectionString) != iResSuccess)
   {
      // Failed to establish the connection.
      // Print the message and exit:
      Print("Error when opening connection. ", GetLastMessage());
      return(INIT_FAILED);
   }
   Print("Connected to database.");
   // The connection was established successfully.
   // Try to execute queries.
   // Create a table and write the data into it:
   if (ExecuteSql(
      "create table DemoTest(DemoInt int, DemoString nvarchar(10));")
      == iResSuccess)
      Print("Created table in database.");
   else
      Print("Failed to create table. ", GetLastMessage());
   if (ExecuteSql(
      "insert into DemoTest(DemoInt, DemoString) values(1, N'Test');")
      == iResSuccess)
      Print("Data written to table.");
   else
      Print("Failed to write data to table. ", GetLastMessage());
   // Proceed to reading the data. Read an integer from the database:
   int iTestInt = ReadInt("select top 1 DemoInt from DemoTest;");
   string sMessage = GetLastMessage();
   if (StringLen(sMessage) == 0)
      Print("Number read from database: ", iTestInt);
   else // Failed to read number.
      Print("Failed to read number from database. ", GetLastMessage());
   // Now read a string:
   string sTestString = ReadString("select top 1 DemoString from DemoTest;");
   sMessage = GetLastMessage();
   if (StringLen(sMessage) == 0)
      Print("String read from database: ", sTestString);
   else // Failed to read string.
      Print("Failed to read string from database. ", GetLastMessage());
   // The table is no longer needed - it can be deleted.
   if (ExecuteSql("drop table DemoTest;") != iResSuccess)
      Print("Failed to delete table. ", GetLastMessage());
   // Completed the work - close the connection:
   CloseConnection();
   // Complete initialization:
   return(INIT_SUCCEEDED);
  }

//+------------------------------------------------------------------+
