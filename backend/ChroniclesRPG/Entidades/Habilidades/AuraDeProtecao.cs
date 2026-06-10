namespace ChroniclesRPG.Entidades.Habilidades{
    public class AuraDeProtecao : Habilidade{
        public AuraDeProtecao() 
            : base("Aura de Proteção", "Passiva: Concede bônus em testes de resistência para aliados próximos com base no seu Carisma.", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            Console.WriteLine("Esta é uma habilidade passiva (Aura de Proteção) e afeta permanentemente aliados em alcance.");
            return 0;
        }
    }
}
