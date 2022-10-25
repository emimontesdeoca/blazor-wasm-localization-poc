using System.Net.Http.Json;

namespace BlazorWasmLocalizationPoc.Services
{
    public sealed class LocaleService
    {
        private static List<Locale> Locales = new();
        private static bool Loaded = false;
        private static string SelectedLocale = "en";
        public static Func<Task> Notify;

        private readonly HttpClient _httpClient;

        public LocaleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Load(Func<Task> func)
        {
            if (!Loaded)
            {
                Locales = await _httpClient.GetFromJsonAsync<List<Locale>>("locale.json");
                Notify = func;
                Loaded = true;
            }
        }

        public void UpdateLocale(string locale)
        {
            SelectedLocale = locale;

            if (Notify is not null)
            {
                Notify.Invoke();
            }
        }

        public string GetByKey(string key)
        {
            if (Loaded)
            {
                return Locales
                    .Single(x => x.key == key).translations
                    .Single(x => x.language == SelectedLocale).value
                    .ToString();
            }

            return string.Empty;
        }

        public class Locale
        {
            public string key { get; set; }
            public List<Translation> translations { get; set; }
        }

        public class Translation
        {
            public string language { get; set; }
            public string value { get; set; }
        }
    }
}
