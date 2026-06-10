namespace ChroniclesRPG.Entidades
{
    public class EtapaJornada
    {
        public int Numero { get; }
        public string Nome { get; }
        public Dictionary<CaminhoJornada, EventoJornada> Eventos { get; }

        public EtapaJornada(int numero, string nome, Dictionary<CaminhoJornada, EventoJornada> eventos)
        {
            Numero = numero;
            Nome = nome;
            Eventos = eventos;
        }
    }
}
