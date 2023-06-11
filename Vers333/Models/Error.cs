using System.ComponentModel.DataAnnotations;

namespace Vers333.Models
{
    public class Error
    {
        public string? Title { get; set; }

        public string? UrlFromAction { get; set; }

        public string? UrlFromController { get; set; }

        public string? Content { get; set; }

        public string? Path { get; set; }

        public Error(string? title, string? urlFromAction, string? urlFromController, string? content)
        {
            Title = title;
            UrlFromAction = urlFromAction;
            UrlFromController = urlFromController;
            Content = content;
        }
        public Error(string? title, string? path, string? content)
        {
            Title = title;
            Path = path;
            Content = content;
        }
    }
}
