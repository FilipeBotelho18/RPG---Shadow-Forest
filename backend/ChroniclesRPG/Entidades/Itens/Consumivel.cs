using ChroniclesRPG.Entidades;

namespace ChroniclesRPG.Entidades.Itens{
    
    public class Consumivel : Item{
        // Quantidade de HP que este item recupera ao ser usado
        public int Cura { get; set; }

        public Consumivel(string nome, string descricao, int valor, int cura) 
            : base(nome, descricao, valor){
            Cura = cura;
        }

        // ==========================================
        // MÉTODO DE USO
        // ==========================================

        // Aplica o efeito do consumível no alvo e remove o item do inventário
        public void Usar(FichaPersonagem alvo){
            alvo.HpAtual += Cura;
            
            // Garante que a cura não ultrapasse o HP Máximo
            if (alvo.HpAtual > alvo.HpMaximo)
                alvo.HpAtual = alvo.HpMaximo;

            Console.WriteLine($"  {alvo.Nome} usou {Nome} e recuperou {Cura} de HP! (HP: {alvo.HpAtual}/{alvo.HpMaximo})");

            // Remove o item do inventário após o uso, pois consumíveis são de uso único
            alvo.Inventario.Remove(this);
        }
    }
}
