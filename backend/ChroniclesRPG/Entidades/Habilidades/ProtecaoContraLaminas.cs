namespace ChroniclesRPG.Entidades.Habilidades{
    // Truque (custo 0) — Ação Principal
    // Concede resistência física até o próximo turno (reduz dano de corte/concussão/perfurante à metade)
    public class ProtecaoContraLaminas : Habilidade{
        public ProtecaoContraLaminas() 
            : base("Proteção contra Lâminas", "Truque: Até o fim do próximo turno, você tem resistência a dano de concussão, cortante e perfurante de ataques com armas.", TipoAcao.AcaoPrincipal, TipoHabilidade.TruqueMagico) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            usuario.TemResistenciaFisica = true;
            Console.WriteLine($"  {usuario.Nome} traça um símbolo arcano no ar! (Resistência Física ativa até o fim do próximo turno)");
            return 0;
        }
    }
}
