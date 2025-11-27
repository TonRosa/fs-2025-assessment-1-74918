using fs_2025_a_api_demo_002.Models;
using System.Text.Json;

namespace fs_2025_a_api_demo_002.Data
{
    public class BookData
    {
        public List<Book> Books { get; private set; } = new List<Book>();

        public BookData()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bookdata.json");
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                Books = JsonSerializer.Deserialize<List<Book>>(jsonData, options) ?? new List<Book>();
            }
        }
    }
}
