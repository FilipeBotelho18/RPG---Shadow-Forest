using ChroniclesRPG.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

string dadosDir = Path.Combine(app.Environment.ContentRootPath, "Dados");
var sessao = new SessaoJogo(
    Path.Combine(dadosDir, "jornada.json"),
    Path.Combine(dadosDir, "inimigos.json"));

app.MapPost("/api/jogo/novo", () =>
{
    sessao.NovoJogo();
    return Results.Ok(sessao.CriarResposta());
});

app.MapPost("/api/jogo/personagem", (EscolhaClasseRequest request) =>
{
    sessao.CriarPersonagem(request.Classe, request.Nome);
    return Results.Ok(sessao.CriarResposta());
});

app.MapGet("/api/jogo", () => Results.Ok(sessao.CriarResposta()));

app.MapPost("/api/jogo/caminho", (EscolhaCaminhoRequest request) =>
{
    sessao.EscolherCaminho(request.Caminho);
    return Results.Ok(sessao.CriarResposta());
});

app.MapPost("/api/jogo/acao", (AcaoCombateRequest request) =>
{
    sessao.ExecutarAcao(request.Acao);
    return Results.Ok(sessao.CriarResposta());
});

app.MapPost("/api/jogo/evento", (AcaoEventoRequest request) =>
{
    sessao.ExecutarEvento(request.Acao, request.Item);
    return Results.Ok(sessao.CriarResposta());
});

app.Run();

public sealed record EscolhaCaminhoRequest(string Caminho);
public sealed record AcaoCombateRequest(string Acao);
public sealed record AcaoEventoRequest(string Acao, string? Item);
public sealed record EscolhaClasseRequest(string Classe, string? Nome);
