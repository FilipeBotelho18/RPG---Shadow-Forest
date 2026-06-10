namespace ChroniclesRPG.Entidades.Habilidades{
    // Passiva desbloqueada no nível 6 (Escola de Abjuração)
    // O bônus de +2 CA já é aplicado pela ProtecaoArcana enquanto o escudo estiver ativo.
    // Esta classe serve como marcador de que a habilidade foi aprendida.
    public class ProtecaoProjetada : Habilidade{
        public ProtecaoProjetada() 
            : base("Proteção Projetada", "Passiva: Enquanto o Escudo Arcano estiver ativo, você recebe +2 na Classe de Armadura.", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            Console.WriteLine($"  Esta é uma habilidade passiva. O bônus de +2 CA é aplicado automaticamente enquanto o Escudo Arcano estiver ativo.");
            return 0;
        }
    }
}
