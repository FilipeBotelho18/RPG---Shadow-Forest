namespace ChroniclesRPG.Entidades.Habilidades{
    // Característica da Tradição Arcana (Escola de Abjuração) — Nível 2
    // Cria um escudo mágico com HP = (Nível × 2) + Mod Inteligência
    // Ao conjurar magias de Abjuração, o escudo recupera HP = nível da magia × 2
    public class ProtecaoArcana : Habilidade{
        public ProtecaoArcana() 
            : base("Proteção Arcana", "Tradição Arcana: Cria um Escudo Arcano com HP = (Nível×2) + ModINT. O escudo absorve dano antes do HP.", TipoAcao.AcaoPrincipal, TipoHabilidade.CaracteristicaClasse) 
        { }

        public override int Executar(FichaPersonagem usuario, FichaPersonagem? alvo = null){
            // Calcula o HP máximo do escudo com base no nível e inteligência atuais
            int hpMax = (usuario.Nivel * 2) + usuario.ModificadorInteligencia;
            hpMax = Math.Max(1, hpMax);

            usuario.HpMaxEscudoArcano = hpMax;
            usuario.HpEscudoArcano = hpMax;

            // Proteção Projetada: +2 CA enquanto o escudo estiver ativo
            // (aplicado aqui ao ativar; remover ao escudo chegar a 0 no ReceberDano)
            if (!usuario.TemEscudoDaFe){ // Evita empilhar com Escudo da Fé do Paladino
                usuario.ClasseArmadura += 2;
                usuario.TemEscudoDaFe = true; // Reutiliza a flag para indicar bônus ativo
            }

            Console.WriteLine($"  {usuario.Nome} conjura a Proteção Arcana!");
            Console.WriteLine($"    Escudo Arcano ativo: {usuario.HpEscudoArcano}/{usuario.HpMaxEscudoArcano} HP | CA ajustada para {usuario.ClasseArmadura} (+2 Proteção Projetada)");
            return hpMax;
        }
    }
}
