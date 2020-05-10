using System;
using System.Linq;
using System.Reflection;
using LinqToDB;
using LinqToDB.Data;

namespace Sociomedia.ReadModel.DataAccess
{
    public static class DataConnectionExtensions
    {
        public static void GenerateMissingTables(this DataConnection dbConnection)
        {
            var tableTypes = dbConnection
                .GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType)
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(ITable<>))
                .Select(x => x.PropertyType.GenericTypeArguments[0])
                .ToArray();

            foreach (var tableType in tableTypes) {
                dbConnection.CreateTableIfDoesNotExists(tableType);
            }
        }

        public static void ClearDatabase(this DataConnection dbConnection)
        {
            var tableTypes = dbConnection
                .GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType)
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(ITable<>))
                .Select(x => x.PropertyType.GenericTypeArguments[0])
                .ToArray();

            foreach (var tableType in tableTypes) {
                dbConnection.DeleteAllFromTable(tableType);
            }
        }

        public static void CreateTableIfDoesNotExists(this DataConnection dbConnection, Type tableType)
        {
            typeof(DataConnectionExtensions)
                .GetMethod(nameof(CreateTableIfDoesNotExists), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(tableType)
                .Invoke(null, new[] { dbConnection });
        }

        public static void DeleteAllFromTable(this DataConnection dbConnection, Type tableType)
        {
            typeof(DataConnectionExtensions)
                .GetMethod(nameof(DeleteAllFromTable), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(tableType)
                .Invoke(null, new[] { dbConnection });
        }

        private static void CreateTableIfDoesNotExists<T>(DataConnection dbConnection) where T : class
        {
            try {
                dbConnection.GetTable<T>().Count();
            }
            catch (Exception e) {
                dbConnection.CreateTable<T>();
            }
        }

        private static void DeleteAllFromTable<T>(DataConnection dbConnection) where T : class
        {
            dbConnection
                .GetTable<T>()
                .Where(x => true)
                .Delete();
        }
    }
}