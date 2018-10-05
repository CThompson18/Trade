using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace MqlSqlDemo
{
    public static class MqlSqlDemo
    {
        private static SqlConnection conn = null;
        private static SqlCommand com = null;
        private static string sMessage = string.Empty;

        public const int iResSuccess = 0;
        public const int iResError = 1;

        [DllExport("CreateConnection", CallingConvention = CallingConvention.StdCall)]
        public static int CreateConnection(
            [MarshalAs(UnmanagedType.LPWStr)] string sConnStr)
        {
            // Очищаем строку сообщения:
            sMessage = string.Empty;
            // Если соединение уже есть - закрываем его и меняем
            // строку подключения на новую, если нет -
            // создаём заново объекты подключения и команды:
            if (conn != null)
            {
                conn.Close();
                conn.ConnectionString = sConnStr;
            }
            else
            {
                conn = new SqlConnection(sConnStr);
                com = new SqlCommand();
                com.Connection = conn;
            }
            // Пробуем открыть подключение:
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                // Почему-то подключение не открылось.
                // Заносим информацию об ошибке в строку сообщения:
                sMessage = ex.Message;
                // Освобождаем ресурсы и обнуляем объекты:
                com.Dispose();
                conn.Dispose();
                conn = null;
                com = null;
                // Ошибка:
                return iResError;
            }
            // Всё прошло хорошо, подключение открыто:
            return iResSuccess;
        }

        [DllExport("GetLastMessage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static string GetLastMessage()
        {
            return sMessage;
        }

        [DllExport("ExecuteSql", CallingConvention = CallingConvention.StdCall)]
        public static int ExecuteSql(
            [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Очищаем строку сообщения:
            sMessage = string.Empty;
            // Сначала нужно проверить, установлено ли подключение.
            if (conn == null)
            {
                // Соединение ещё не открыто.
                // Сообщаем об ошибке и возвращаем флаг ошибки:
                sMessage = "Connection is null, call CreateConnection first.";
                return iResError;
            }
            // Соединение готово, пробуем выполнить команду.
            try
            {
                com.CommandText = sSql;
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Ошибка при выполнении команды.
                // Заносим информацию об ошибке в строку сообщения:
                sMessage = ex.Message;
                // Возвращаем флаг ошибки:
                return iResError;
            }
            // Всё прошло хорошо - возвращаем флаг успешного выполнения:
            return iResSuccess;
        }

        [DllExport("ReadInt", CallingConvention = CallingConvention.StdCall)]
        public static int ReadInt(
            [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Очищаем строку сообщения:
            sMessage = string.Empty;
            // Сначала нужно проверить, установлено ли подключение.
            if (conn == null)
            {
                // Соединение ещё не открыто.
                // Сообщаем об ошибке и возвращаем флаг ошибки:
                sMessage = "Connection is null, call CreateConnection first.";
                return iResError;
            }
            // Переменная для получения возвращаемого результата:
            int iResult = 0;
            // Соединение готово, пробуем выполнить команду.
            try
            {
                com.CommandText = sSql;
                iResult = (int)com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // Ошибка при выполнении команды.
                // Заносим информацию об ошибке в строку сообщения:
                sMessage = ex.Message;
            }
            // Возвращаем полученный результат:
            return iResult;
        }

        [DllExport("ReadString", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static string ReadString(
                [MarshalAs(UnmanagedType.LPWStr)] string sSql)
        {
            // Очищаем строку сообщения:
            sMessage = string.Empty;
            // Сначала нужно проверить, установлено ли подключение.
            if (conn == null)
            {
                // Соединение ещё не открыто.
                // Сообщаем об ошибке и возвращаем флаг ошибки:
                sMessage = "Connection is null, call CreateConnection first.";
                return string.Empty;
            }
            // Переменная для получения возвращаемого результата:
            string sResult = string.Empty;
            // Соединение готово, пробуем выполнить команду.
            try
            {
                com.CommandText = sSql;
                sResult = com.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // Ошибка при выполнении команды.
                // Заносим информацию об ошибке в строку сообщения:
                sMessage = ex.Message;
            }
            // Возвращаем полученный результат:
            return sResult;
        }

        [DllExport("CloseConnection", CallingConvention = CallingConvention.StdCall)]
        public static void CloseConnection()
        {
            // Сначала нужно проверить, установлено ли подключение.
            if (conn == null)
                // Соединение ещё не открыто - значит и закрывать его тоже не нужно:
                return;
            // Соединение открыто - его надо закрыть:
            com.Dispose();
            com = null;
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
}
