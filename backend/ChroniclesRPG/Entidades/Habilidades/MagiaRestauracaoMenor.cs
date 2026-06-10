namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaRestauracaoMenor : Habilidade{
        public MagiaRestauracaoMenor() 
            : base("Restauração Menor", "Remove uma doença ou condição que aflige o alvo.", TipoAcao.AcaoPrincipal, TipoHabilidade.Magia, 2) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(2)){
                Console.WriteLine($"  {usuario.Nome} não possui espaços de magia suficientes!");
                return 0;
            }

            if (alvo != null) {
                Console.WriteLine($"  {usuario.Nome} tocou em {alvo.Nome} e conjurou Restauração Menor! (Condições negativas removidas)");
            } else {
                Console.WriteLine($"  {usuario.Nome} conjurou Restauração Menor em si mesmo! (Condições negativas removidas)");
            }
            
            return 1;
        }
    }
}
