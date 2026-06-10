namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaBencao : Habilidade{
        public MagiaBencao() 
            : base("Bênção", "Abençoa até 3 aliados, dando bônus nas jogadas de ataque.", TipoAcao.AcaoPrincipal, TipoHabilidade.Magia, 1) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(1)){
                Console.WriteLine($"  {usuario.Nome} não possui mais espaços de magia disponíveis para conjurar Bênção!");
                return 0;
            }

            // Simulação de buff em um alvo (na versão completa iteraria em 3 alvos)
            if (alvo != null) {
                alvo.TemBencao = true;
                Console.WriteLine($"  {usuario.Nome} conjurou Bênção em {alvo.Nome}! (+1d4 nos ataques)");
            } else {
                usuario.TemBencao = true;
                Console.WriteLine($"  {usuario.Nome} conjurou Bênção em si mesmo! (+1d4 nos ataques)");
            }
            
            return 1;
        }
    }
}
