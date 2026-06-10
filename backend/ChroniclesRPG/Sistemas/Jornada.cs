using System.Text.Json;
using ChroniclesRPG.Entidades;

namespace ChroniclesRPG.Sistemas
{
    public static class Jornada
    {
        public static List<EtapaJornada> CarregarEtapas(string caminhoJornada, string caminhoInimigos)
        {
            var opcoes = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string jornadaJson = File.ReadAllText(caminhoJornada);
            string inimigosJson = File.ReadAllText(caminhoInimigos);

            var jornada = JsonSerializer.Deserialize<JornadaDto>(jornadaJson, opcoes)
                ?? throw new InvalidOperationException("Arquivo de jornada vazio ou invalido.");
            var inimigos = JsonSerializer.Deserialize<List<InimigoDto>>(inimigosJson, opcoes)
                ?? throw new InvalidOperationException("Arquivo de inimigos vazio ou invalido.");

            var catalogoInimigos = inimigos.ToDictionary(inimigo => inimigo.Id, StringComparer.OrdinalIgnoreCase);
            var etapas = new List<EtapaJornada>();

            foreach (var etapaDto in jornada.Etapas.OrderBy(etapa => etapa.Numero))
            {
                Dictionary<CaminhoJornada, EventoJornada> eventos;

                if (etapaDto.Evento != null)
                {
                    var eventoLinear = CriarEvento(etapaDto.Evento, catalogoInimigos);
                    eventos = new Dictionary<CaminhoJornada, EventoJornada>
                    {
                        [CaminhoJornada.Esquerda] = eventoLinear,
                        [CaminhoJornada.Meio] = eventoLinear,
                        [CaminhoJornada.Direita] = eventoLinear
                    };
                }
                else
                {
                    eventos = new Dictionary<CaminhoJornada, EventoJornada>
                    {
                        [CaminhoJornada.Esquerda] = CriarEvento(etapaDto.Esquerda ?? new EventoDto(), catalogoInimigos),
                        [CaminhoJornada.Meio] = CriarEvento(etapaDto.Meio ?? new EventoDto(), catalogoInimigos),
                        [CaminhoJornada.Direita] = CriarEvento(etapaDto.Direita ?? new EventoDto(), catalogoInimigos)
                    };
                }

                etapas.Add(new EtapaJornada(etapaDto.Numero, etapaDto.Nome, eventos));
            }

            return etapas;
        }

        public static EventoJornada EscolherEvento(EtapaJornada etapa)
        {
            Console.WriteLine($"\nEtapa {etapa.Numero}: {etapa.Nome}");
            Console.WriteLine("[1] Esquerda - Bom / sorte");
            Console.WriteLine("[2] Meio     - Neutro / seguro");
            Console.WriteLine("[3] Direita  - Ruim / perigo");
            Console.Write("Escolha o caminho > ");

            string? escolha = Console.ReadLine();
            CaminhoJornada caminho = escolha switch
            {
                "1" => CaminhoJornada.Esquerda,
                "3" => CaminhoJornada.Direita,
                _ => CaminhoJornada.Meio
            };

            return etapa.Eventos[caminho];
        }

        private static EventoJornada CriarEvento(EventoDto eventoDto, Dictionary<string, InimigoDto> catalogoInimigos)
        {
            Inimigo? inimigo = null;

            if (!string.IsNullOrWhiteSpace(eventoDto.InimigoId))
            {
                if (!catalogoInimigos.TryGetValue(eventoDto.InimigoId, out var inimigoDto))
                {
                    throw new InvalidOperationException($"Inimigo nao encontrado no catalogo: {eventoDto.InimigoId}");
                }

                inimigo = CriarInimigo(inimigoDto);
            }

            return new EventoJornada(
                eventoDto.Titulo,
                eventoDto.Descricao,
                inimigo,
                eventoDto.Cura,
                eventoDto.Ouro,
                eventoDto.XP,
                eventoDto.ConcedePocao,
                string.IsNullOrWhiteSpace(eventoDto.Tipo) ? (inimigo == null ? "evento" : "combate") : eventoDto.Tipo,
                eventoDto.Boss,
                eventoDto.Cena);
        }

        private static Inimigo CriarInimigo(InimigoDto inimigoDto)
        {
            return new Inimigo(
                inimigoDto.Nome,
                inimigoDto.ClasseArmadura,
                inimigoDto.HpMaximo,
                inimigoDto.BonusAtaque,
                inimigoDto.DadoDeDano,
                inimigoDto.BonusDano,
                inimigoDto.XPConcedido);
        }

        private sealed class JornadaDto
        {
            public List<EtapaDto> Etapas { get; set; } = new();
        }

        private sealed class EtapaDto
        {
            public int Numero { get; set; }
            public string Nome { get; set; } = string.Empty;
            public EventoDto? Evento { get; set; }
            public EventoDto? Esquerda { get; set; }
            public EventoDto? Meio { get; set; }
            public EventoDto? Direita { get; set; }
        }

        private sealed class EventoDto
        {
            public string Titulo { get; set; } = string.Empty;
            public string Descricao { get; set; } = string.Empty;
            public string? InimigoId { get; set; }
            public int Cura { get; set; }
            public int Ouro { get; set; }
            public int XP { get; set; }
            public bool ConcedePocao { get; set; }
            public string Tipo { get; set; } = string.Empty;
            public bool Boss { get; set; }
            public string Cena { get; set; } = string.Empty;
        }

        private sealed class InimigoDto
        {
            public string Id { get; set; } = string.Empty;
            public string Nome { get; set; } = string.Empty;
            public int ClasseArmadura { get; set; }
            public int HpMaximo { get; set; }
            public int BonusAtaque { get; set; }
            public string DadoDeDano { get; set; } = "1d4";
            public int BonusDano { get; set; }
            public int XPConcedido { get; set; }
        }
    }
}
