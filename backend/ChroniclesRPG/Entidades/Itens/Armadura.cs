namespace ChroniclesRPG.Entidades.Itens{
    
    public enum TipoArmadura{
        Leve,
        Media,
        Pesada
    }

    // Armadura "é um" Item
    public class Armadura : Item{

        public int BonusDefesa { get; set; }
        public TipoArmadura Tipo { get; set; } // ← propriedade para guardar o tipo

        // O ": base()" repassa o nome, desc e valor para o construtor do Pai
        public Armadura(string nome, string descricao, TipoArmadura tipo, int valor, int bonusDefesa)
            : base(nome, descricao, valor){
            Tipo = tipo;           // ← agora o tipo é salvo corretamente
            BonusDefesa = bonusDefesa;
        }
    }
}