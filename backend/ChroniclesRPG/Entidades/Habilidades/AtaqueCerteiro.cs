namespace ChroniclesRPG.Entidades.Habilidades{
    // Truque (custo 0, pode usar infinitamente) — Ação Bônus
    // Concede vantagem no primeiro ataque do próximo turno
    public class AtaqueCerteiro : Habilidade{
        public AtaqueCerteiro() 
            : base("Ataque Certeiro", "Truque: Ação Bônus. Você aponta para um alvo, garantindo vantagem no seu próximo ataque.", TipoAcao.AcaoBonus, TipoHabilidade.TruqueMagico) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            usuario.TemVantagemProximoAtaque = true;
            Console.WriteLine($"  {usuario.Nome} estende o dedo e foca na mente do alvo... (Vantagem no próximo ataque!)");
            return 0;
        }
    }
}