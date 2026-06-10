namespace ChroniclesRPG.Entidades.Habilidades{
    public class MagiaArmaMagica : Habilidade{
        public MagiaArmaMagica() 
            : base("Arma Mágica", "Concede +1 nas jogadas de ataque e dano com a arma temporariamente.", TipoAcao.AcaoBonus, TipoHabilidade.Magia, 2) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            if (!usuario.GastarSlotDeMagia(2)){
                Console.WriteLine($"  {usuario.Nome} não possui espaços de magia suficientes!");
                return 0;
            }

            usuario.TemArmaMagica = true;
            Console.WriteLine($"  A arma de {usuario.Nome} começa a brilhar! (Arma Mágica ativa: +1 Ataque e Dano)");
            
            return 1;
        }
    }
}
