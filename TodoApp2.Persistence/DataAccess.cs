using System;
using System.IO;
using System.Linq;
using TodoApp2.Entity;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence
{
    public class DataAccess
    {
        public const string DatabaseName = "000.db";
        
        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, "WWWTodoDatabase", DatabaseName);
        
        public DataAccess()
        {
            string connectionString = $"Data Source={DatabasePath};";
            AppContext appContext = new AppContext(connectionString);

            //var count = appContext.Categories.Count();

            //var categories = appContext.Categories.GetAll($"{nameof(Category.Trashed)} = FALSE", 10);
            //var categories2 = appContext.Categories.GetAll();

            //var single = appContext.Categories.GetOne("name = 'MyName2'");


            var result0 = appContext.Categories.GetAll();
            var result01 = appContext.Categories.GetFirst();
            var single  = appContext.Categories.GetFirst(x => x.Id == 2);
            var single2  = appContext.Categories.GetFirst(x => x.Name == "MyName2");
            

            //appContext.Categories.Add(new Category() { ListOrder = "13456", Name = "AAAAA", Trashed = true});
            //count = appContext.Categories.Count();
            //appContext.Categories.Add(new Category() { ListOrder = "563473673", Name = "Yess"});
            //count = appContext.Categories.Count();
        }
    }
}
