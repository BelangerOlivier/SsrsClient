using SsrsClient;

var client = new SsrsClientBuilder()
    .WithBaseUrl("")
    .WithNtlm("", "", "")
    .Build();

var report = await client.GetReportAsync("");
Console.WriteLine(report.Id);