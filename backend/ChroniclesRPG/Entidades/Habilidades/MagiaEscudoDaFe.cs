namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaEscudoDaFe : Habilidade{
        public MagiaEscudoDaFe() 
            : base("Escudo da Fé", "Concede +2 de CA a um alvo.", TipoAcao.AcaoBonus, TipoHabilidade.Magia, 1) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(1)){
                Console.WriteLine($"  {usuario.Nome} não possui mais espaços de magia disponíveis para conjurar Escudo da Fé!");
                return 0;
            }

            if (alvo != null) {
                if (!alvo.TemEscudoDaFe){
                    alvo.TemEscudoDaFe = true;
                    alvo.ClasseArmadura += 2;
                    Console.WriteLine($"  {usuario.Nome} conjurou Escudo da Fé em {alvo.Nome}! (CA aumentou para {alvo.ClasseArmadura})");
                } else {
                    Console.WriteLine($"  {alvo.Nome} já está sob efeito do Escudo da Fé.");
                }
            } else {
                if (!usuario.TemEscudoDaFe){
                    usuario.TemEscudoDaFe = true;
                    usuario.ClasseArmadura += 2;
                    Console.WriteLine($"  {usuario.Nome} conjurou Escudo da Fé em si mesmo! (CA aumentou para {usuario.ClasseArmadura})");
                }
            }
            
            return 1;
        }
    }
}
