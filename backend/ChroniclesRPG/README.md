# Shadow Forest

RPG 2D web local em ASP.NET inspirado em AdventureQuest Worlds, com combate em turnos
e regras simplificadas baseadas no SRD da 5ª Edicao de D&D.

O jogo acompanha aventureiros da guilda Pluma de Prata em uma campanha linear de
20 etapas pela Floresta de Letes e pelo Bosque dos Lamentos.

## Rodar

Entre na pasta do projeto:

```powershell
cd "C:\Users\User\Documents\New project\zip_inspect\Projeto-jogo-main\backend\ChroniclesRPG"
```

Inicie o servidor local:

```powershell
dotnet run --urls http://localhost:5177
```

Depois abra no navegador:

```text
http://localhost:5177
```

Para parar o servidor, volte no terminal e pressione:

```text
Ctrl + C
```

Se voce alterar arquivos de HTML, CSS ou JS e a tela nao atualizar, use:

```text
Ctrl + F5
```

Se a porta `5177` estiver ocupada, rode em outra porta:

```powershell
dotnet run --urls http://localhost:5180
```

E abra:

```text
http://localhost:5180
```

-## O que ja funciona

- Interface web jogavel fora do console.
- Tela inicial com escolha de classe.
- Classes jogaveis:
  - Guerreiro, com Ataque, Golpe, Retomar Folego, Surto e Foco.
  - Mago, com Rajada, Misseis Magicos e Paralisar.
  - Paladino, com Ataque, Smite, Cura pelas Maos e Pocao.
- Imagens estaticas dos tres personagens, sem fundo branco.
- Ficha do personagem com atributos, arma, armadura, slots, inventario e ouro.
- Ficha e registro em paineis auxiliares que podem ser abertos e fechados.
- Jornada linear com 20 etapas.
- Combates 1x1 em turnos.
- Separacao basica entre acao principal e acao bonus.
- Botao de encerrar turno para o inimigo agir.
- Dados da jornada em `Dados/jornada.json`.
- Catalogo de inimigos em `Dados/inimigos.json`.
- Save automatico em `Dados/savegame.json`.
- Ataque com d20, erro, critico, dano e morte.
- XP, ouro e subida de nivel.
- Ataque Extra a partir do nivel 5 para classes que recebem esse recurso.
- Mini-chefes e chefe final com destaque visual.
- Safe zones com descanso para recuperar HP e recursos.
- Mercador com compra de pocoes, armas e armadura.
- Sistema simples de status:
  - vantagem
  - desvantagem
  - paralisia
- Cutscenes simples em estilo Visual Novel.
- Tela de vitoria e derrota com resumo.
- Visual estatico de combate com personagem, inimigos e cenario.
- Sprites/imagens estaticas dos inimigos no combate.
- Cenario SVG inline da floresta dentro da area de combate.
- Tres fases visuais da floresta:
  - etapas 1 a 7: floresta de dia
  - etapas 8 a 14: floresta mais escura
  - etapas 15 a 20: floresta sombria/corrompida
- Interface com bordas levemente arredondadas.
- Tela de batalha ampliada e centralizada para dar mais destaque ao combate.
- Layout geral expandido para usar melhor telas maiores, com HUD, arena, sprites, placas de HP e botoes de combate maiores.
- Batalha ocupa a largura principal quando Ficha/Registro estao fechados.
- Ficha e Registro abrem em uma coluna lateral organizada apenas quando o jogador clica nos botoes.
- Balanceamento inicial por classe:
  - Guerreiro com menos ouro inicial por ja comecar bem equipado.
  - Mago com mais HP inicial e mais ouro para compensar fragilidade e dependencia de recursos.
  - Paladino mantendo o ponto medio entre resistencia, cura e Smite.
- Balanceamento revisado de HP, dano, XP e ouro dos inimigos/etapas.
- Cutscenes principais expandidas com falas entre Mestre da Guilda, aventureiro, Espirito da Floresta e Lich.

## Estrutura principal

- `Program.cs`: endpoints da API e servidor web.
- `Web/SessaoJogo.cs`: estado da partida, combate, progresso, loja, descanso, status e save.
- `Entidades/Classes/`: classes jogaveis, incluindo Paladino, Guerreiro e Mago.
- `Dados/jornada.json`: etapas da campanha.
- `Dados/inimigos.json`: catalogo de inimigos.
- `Dados/savegame.json`: progresso salvo localmente.
- `wwwroot/index.html`: estrutura da interface e tela.
- `wwwroot/app.js`: logica da interface web, novo jogo e paineis auxiliares.
- `wwwroot/styles.css`: visual da tela, HUD, batalha, personagens e bordas.
- `wwwroot/assets/`: imagens dos personagens.
- `wwwroot/assets/enemies/`: imagens dos inimigos usadas no combate.

## Onde mexer manualmente

- Layout, tamanho da batalha, bordas e organizacao visual:
  - `wwwroot/styles.css`
- Estrutura HTML da tela, paineis e SVG do cenario de combate:
  - `wwwroot/index.html`
- Logica da interface, troca de fase visual da floresta e botoes da tela:
  - `wwwroot/app.js`
- Balanceamento de HP, CA, dano, ataque e XP dos inimigos:
  - `Dados/inimigos.json`
- Etapas, recompensas, ouro, pocoes, descanso, loja e cutscenes:
  - `Dados/jornada.json`
- Balanceamento inicial das classes e regras web da partida:
  - `Web/SessaoJogo.cs`
- Atributos base e evolucao individual das classes:
  - `Entidades/Classes/Guerreiro.cs`
  - `Entidades/Classes/Mago.cs`
  - `Entidades/Classes/Paladino.cs`

## Fluxo atual

1. O jogador escolhe Guerreiro, Mago ou Paladino.
2. A historia inicia na guilda Pluma de Prata.
3. A campanha avanca por 20 etapas lineares.
4. Cada etapa pode ser combate, cutscene, descanso ou loja.
5. O jogador vence ao derrotar o Lich Primordial no final da jornada.

## Proximos passos naturais

- Melhorar loja com mais itens e comparacao de equipamento.
- Criar inventario interativo mais completo.
- Adicionar retratos ou sprites finais para inimigos e NPCs.
- Separar `savegame.json` de exemplo do save local do jogador.
