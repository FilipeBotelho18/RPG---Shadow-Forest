namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaDestruicaoTrovejante : Habilidade{
        public MagiaDestruicaoTrovejante() 
            : base("Destruição Trovejante", "Seu próximo ataque causará dano trovejante extra e pode derrubar o alvo.", TipoAcao.AcaoBonus, TipoHabilidade.Magia, 1) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(1)){
                Console.WriteLine($"  {usuario.Nome} não possui espaços de magia suficientes!");
                return 0;
            }

            usuario.ProximoAtaqueTrovejante = true;
            Console.WriteLine($"  A arma de {usuario.Nome} estala com um trovão! (Destruição Trovejante carregada no próximo ataque)");
            
            return 1;
        }
    }
}
