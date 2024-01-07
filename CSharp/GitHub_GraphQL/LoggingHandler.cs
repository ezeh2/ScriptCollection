class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler)
	            : base(innerHandler)
    {
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        Console.WriteLine("Sending request:");
        Console.WriteLine($"{request.Method} {request.RequestUri}");
        Console.WriteLine("Headers:");
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        if (request.Content != null)
        {
            Console.WriteLine("Content:");
            Console.WriteLine(await request.Content.ReadAsStringAsync());
        }
        Console.WriteLine();

        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine("Received response:");
        Console.WriteLine($"{(int)response.StatusCode} {response.ReasonPhrase}");
        Console.WriteLine("Headers:");
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        if (response.Content != null)
        {
            Console.WriteLine("Content:");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        Console.WriteLine();

        return response;
    }
}

