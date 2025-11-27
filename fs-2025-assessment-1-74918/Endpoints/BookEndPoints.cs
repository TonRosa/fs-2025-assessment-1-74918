using fs_2025_a_api_demo_002.Data;
using Microsoft.AspNetCore.Mvc;

namespace fs_2025_a_api_demo_002.Endpoints
{
    public static class BookEndPoints
    {
        public static void AddBookEndPoints(this WebApplication app)
        {
            app.MapGet("/books", LoadAllBooksAsync);
            app.MapGet("/books/{id:int}", LoadBookById);

            var v1 = app.MapGroup("/api/v1/");
            v1.MapGet("books", LoadAllBooksAsync);
            v1.MapGet("/books/{id:int}", LoadBookById);
        }

        private static async Task<IResult> LoadBookById(BookData bookData, int id)
        {
            var output = bookData.Books.FirstOrDefault(b => b.id == id);
            if (output is null)
                return Results.NotFound();

            return Results.Ok(output);
        }

        private static async Task<IResult> LoadAllBooksAsync(
           [FromServices] BookData bookData,
            [FromQuery] string? search)
        {
            var output = bookData.Books.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                output = output.Where(b =>
                    b.title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.genre.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return Results.Ok(output);
        }
    
    }
}
