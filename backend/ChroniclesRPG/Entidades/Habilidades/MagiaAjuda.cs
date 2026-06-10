namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaAjuda : Habilidade{
        public MagiaAjuda() 
            : base("Ajuda", "Aumenta o HP máximo de até 3 aliados em 5.", TipoAcao.AcaoPrincipal, TipoHabilidade.Magia, 2) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(2)){
                Console.WriteLine($"  {usuario.Nome} não possui mais espaços de magia de nível 2 ou superior!");
                return 0;
            }

            if (alvo != null) {
                alvo.HpMaximo += 5;
                alvo.HpAtual += 5;
                Console.WriteLine($"  {usuario.Nome} conjurou Ajuda em {alvo.Nome}! (HP Máximo aumentou em 5)");
            } else {
                usuario.HpMaximo += 5;
                usuario.HpAtual += 5;
                Console.WriteLine($"  {usuario.Nome} conjurou Ajuda em si mesmo! (HP Máximo aumentou em 5)");
            }
            
            return 1;
        }
    }
}
