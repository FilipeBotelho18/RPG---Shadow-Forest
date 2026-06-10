namespace ChroniclesRPG.Entidades
{
    public class EventoJornada
    {
        public string Titulo { get; }
        public string Descricao { get; }
        public Inimigo? Inimigo { get; }
        public int Cura { get; }
        public int Ouro { get; }
        public int XP { get; }
        public bool ConcedePocao { get; }
        public string Tipo { get; }
        public bool Boss { get; }
        public string Cena { get; }

        public bool TemCombate => Inimigo != null;

        public EventoJornada(string titulo, string descricao, Inimigo? inimigo = null, int cura = 0, int ouro = 0, int xp = 0, bool concedePocao = false, string tipo = "combate", bool boss = false, string cena = "")
        {
            Titulo = titulo;
            Descricao = descricao;
            Inimigo = inimigo;
            Cura = cura;
            Ouro = ouro;
            XP = xp;
            ConcedePocao = concedePocao;
            Tipo = tipo;
            Boss = boss;
            Cena = cena;
        }
    }
}
