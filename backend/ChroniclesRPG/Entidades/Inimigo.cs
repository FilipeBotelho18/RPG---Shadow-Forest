namespace ChroniclesRPG.Entidades
{
    public class Inimigo
    {
        public string Nome { get; }
        public int ClasseArmadura { get; }
        public int HpMaximo { get; }
        public int HpAtual { get; set; }
        public int BonusAtaque { get; }
        public string DadoDeDano { get; }
        public int BonusDano { get; }
        public int XPConcedido { get; }

        public bool EstaVivo => HpAtual > 0;

        public Inimigo(string nome, int classeArmadura, int hpMaximo, int bonusAtaque, string dadoDeDano, int bonusDano, int xpConcedido)
        {
            Nome = nome;
            ClasseArmadura = classeArmadura;
            HpMaximo = hpMaximo;
            HpAtual = hpMaximo;
            BonusAtaque = bonusAtaque;
            DadoDeDano = dadoDeDano;
            BonusDano = bonusDano;
            XPConcedido = xpConcedido;
        }
    }
}
