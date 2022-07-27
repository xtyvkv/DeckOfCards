using DeckOfCards.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeckOfCards.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DisplayDeckOfCards()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            
            const string createDeckOfCardsApiUrl = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";
            var apiResponse = httpClient.GetFromJsonAsync<DeckOfCards_Create>(createDeckOfCardsApiUrl).GetAwaiter().GetResult();

            string deckId = apiResponse.deck_id;
            int noCardsToDraw = 5;
            string drawDeckOfCardsApiFormat = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count={noCardsToDraw}";

            var drawCardsResponse = httpClient.GetFromJsonAsync<DeckOfCards_Draw>(drawDeckOfCardsApiFormat).GetAwaiter().GetResult();
            var displayCardsModel = new DisplayResultsModel();
            displayCardsModel.createResult = apiResponse;
            displayCardsModel.drawResult = drawCardsResponse;
            return View(displayCardsModel);
        }

        public class DeckOfCards_Draw
        {
            public bool success { get; set; }
            public string deck_id { get; set; }
            public int remaining { get; set; }
            public DeckOfCards_Draw_Card[] cards { get; set; }
        }

        public class DeckOfCards_Draw_Card
        {
            public string image { get; set; }
            public string value { get; set; }
            public string suit { get; set; }
            public string code { get; set; }

        }
        public class DeckOfCards_Create
        {
            public bool success { get; set; }
            public string deck_id { get; set; }
            public bool shuffled { get; set; }
            public int remaining { get; set; }
        }

        public class DisplayResultsModel
        {
            public DeckOfCards_Create createResult { get; set; }
            public DeckOfCards_Draw drawResult { get; set; }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}