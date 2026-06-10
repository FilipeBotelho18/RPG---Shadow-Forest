document.body.classList.add("intro-active");

// ── DOM references ────────────────────────────────────────
const stageEl            = document.querySelector("#stage");
const hpEl               = document.querySelector("#hp");
const hpBarEl            = document.querySelector("#hp-bar");
const xpEl               = document.querySelector("#xp");
const levelEl            = document.querySelector("#level");
const playerNameEl       = document.querySelector("#player-name");
const playerPortraitEl   = document.querySelector("#player-portrait");
const contentGrid        = document.querySelector(".content-grid");
const sheetPanel         = document.querySelector("#sheet-panel");
const logPanel           = document.querySelector("#log-panel");
const toggleSheetButton  = document.querySelector("#toggle-sheet");
const toggleLogButton    = document.querySelector("#toggle-log");
const creationPanel      = document.querySelector("#creation-panel");
const classCardsEl       = document.querySelector("#class-cards");
const choicePanel        = document.querySelector("#choice-panel");
const eventPanel         = document.querySelector("#event-panel");
const eventTypeEl        = document.querySelector("#event-type");
const eventTitleEl       = document.querySelector("#event-title");
const eventDescriptionEl = document.querySelector("#event-description");
const shopListEl         = document.querySelector("#shop-list");
const restButton         = document.querySelector("#rest-button");
const continueButton     = document.querySelector("#continue-button");
const combatPanel        = document.querySelector("#combat-panel");
const battlefieldEl      = document.querySelector("#battlefield");
const pathCardsEl        = document.querySelector("#path-cards");
const enemyNameEl        = document.querySelector("#enemy-name");
const enemyPlateNameEl   = document.querySelector("#enemy-plate-name");
const enemyHpEl          = document.querySelector("#enemy-hp");
const enemyArmorEl       = document.querySelector("#enemy-armor");
const enemyHpBarEl       = document.querySelector("#enemy-hp-bar");
const enemySpriteEl      = document.querySelector("#enemy-sprite");
const battlePlayerHpEl   = document.querySelector("#battle-player-hp");
const combatPlayerImageEl = document.querySelector("#combat-player-image");
const feedbackEl         = document.querySelector("#feedback");
const sheetEl            = document.querySelector("#sheet");
const inventoryListEl    = document.querySelector("#inventory-list");
const logEl              = document.querySelector("#log");
const finalPanel         = document.querySelector("#final-panel");
const finalTitleEl       = document.querySelector("#final-title");
const finalResultEl      = document.querySelector("#final-result");
const finalLevelEl       = document.querySelector("#final-level");
const finalKillsEl       = document.querySelector("#final-kills");
const finalGoldEl        = document.querySelector("#final-gold");
const finalPathsEl       = document.querySelector("#final-paths");

const saveKey = "chronicles-shadow-woods-state";

const panelState = {
  sheet: false,
  log: false
};

const classImages = {
  Guerreiro: "assets/guerreiro.png",
  Mago: "assets/mago.png",
  Paladino: "assets/paladino.png"
};

const enemySpriteClasses = {
  "Twig Blight": "twig",
  "Lobo Corrompido": "wolf",
  "Esqueleto de Aventureiro": "skeleton",
  "Zumbi Putrefato": "zombie",
  "Orc da Garra de Sangue": "orc",
  "Bugbear Urso-Coruja Menor": "ogre",
  "Ogro Esmagador": "ogre",
  "Arvore Desperta Maligna": "tree",
  "Manticora Faminta": "manticore",
  "Lich Primordial Enfraquecido": "lich awakened"
};

const enemyImages = {
  "Twig Blight": "assets/enemies/twig_blight.png",
  "Esqueleto de Aventureiro": "assets/enemies/esqueleto_aventureiro.png",
  "Lobo Corrompido": "assets/enemies/lobo_corrompido.png",
  "Zumbi Putrefato": "assets/enemies/zumbi_putrefato.png",
  "Orc da Garra de Sangue": "assets/enemies/orc_sangue.png",
  "Bugbear Urso-Coruja Menor": "assets/enemies/bugbear_menor.png",
  "Ogro Esmagador": "assets/enemies/ogro_esmagador.png",
  "Arvore Desperta Maligna": "assets/enemies/arvore_maligna.png",
  "Manticora Faminta": "assets/enemies/manticora_faminta.png",
  "Lich Primordial Enfraquecido": "assets/enemies/lich_desperto.png"
};

const icons = {
    atacar: "⚔",
    golpe: "🪓",
    folego: "💨",
    surto: "🔥",
    rajada: "✨",
    raio_gelo: "❄",
    raio_fogo: "🔥",
    protecao_laminas: "🛡",
    protecao_arcana: "🔷",
    misil: "☄",
    paralisar: "🌀",
    smite: "✨",
    cura: "💛",
    bencao: "🙏",
    escudo_fe: "🛡",
    trovejante: "⚡",
    ajuda: "🤝",
    restauracao: "🌿",
    arma_magica: "🔱",
    pocao: "🧪",
    foco: "🎯",
    encerrar: "⏭"
};

async function postJson(url, body = {}) {
  const response = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body)
  });

  return response.json();
}

async function loadGame() {
  // Não inicia automaticamente.
  // A tela inicial aparece primeiro.
  // O jogo só começa quando clicar em "NOVA AVENTURA".
}

function render(state) {
  localStorage.setItem(saveKey, JSON.stringify(state));

  renderPlayer(state.jogador);
  renderStage(state);
  renderClasses(state);
  renderPaths(state.caminhos || []);
  renderEvent(state);
  renderCombat(state);
  renderScene(state);
  renderSheet(state.jogador);
  renderFinal(state);
  renderLog(state.log || []);
  renderPanels();
}

function renderPanels() {
  const hasSidePanel = panelState.sheet || panelState.log;

  contentGrid.classList.toggle("has-side-panel", hasSidePanel);
  sheetPanel.classList.toggle("hidden", !panelState.sheet);
  logPanel.classList.toggle("hidden", !panelState.log);
  toggleSheetButton.classList.toggle("active", panelState.sheet);
  toggleLogButton.classList.toggle("active", panelState.log);
}

function renderScene(state) {
  const etapa = state.etapa?.numero || state.etapaAtual || 1;
  const forestPhase = etapa >= 15 ? "forest-cursed" : etapa >= 8 ? "forest-dark" : "forest-day";

  battlefieldEl.className = `battlefield scene-${state.cenario || "bosque"} ${forestPhase}`;
}

// Stats mostrados por classe no HUD
const classHudStats = {
  Guerreiro: (j) => [
    { icon: "⛨", label: "Armadura",  val: j.classeArmadura, id: "armor" },
    { icon: "🧪", label: "Poções",   val: j.pocoes,          id: "potions" },
    { icon: "💰", label: "Ouro",     val: j.ouro,            id: "gold" },
  ],
  Mago: (j) => [
    { icon: "⛨", label: "Armadura",    val: j.classeArmadura,                       id: "armor" },
    { icon: "🔷", label: "Slots Nv.1",  val: j.slotsNivel1,                          id: "slots1" },
    { icon: "🔶", label: "Slots Nv.2",  val: j.slotsNivel2,                          id: "slots2" },
    { icon: "🧪", label: "Poções",      val: j.pocoes,                               id: "potions" },
    { icon: "💰", label: "Ouro",        val: j.ouro,                                 id: "gold" },
  ],
  Paladino: (j) => [
    { icon: "⛨", label: "Armadura",     val: j.classeArmadura, id: "armor" },
    { icon: "✨", label: "Smite",        val: j.slotsNivel1,    id: "slots" },
    { icon: "💛", label: "Cura (mãos)", val: j.curaPelasMaos,  id: "layhands" },
    { icon: "🧪", label: "Poções",      val: j.pocoes,          id: "potions" },
    { icon: "💰", label: "Ouro",        val: j.ouro,            id: "gold" },
  ],
};

function renderPlayer(jogador) {
  const hpPercent = Math.max(0, Math.min(100, (jogador.hpAtual / jogador.hpMaximo) * 100));
  const image = classImages[jogador.classe] || classImages.Paladino;

  // Identidade
  playerNameEl.textContent    = jogador.nome;
  playerPortraitEl.style.backgroundImage = `linear-gradient(180deg, rgba(0,0,0,0), rgba(0,0,0,.35)), url("${image}")`;
  combatPlayerImageEl.src     = image;
  hpEl.textContent            = `HP ${jogador.hpAtual}/${jogador.hpMaximo}`;
  battlePlayerHpEl.textContent = `HP ${jogador.hpAtual}/${jogador.hpMaximo}`;
  hpBarEl.style.width         = `${hpPercent}%`;
  xpEl.textContent            = `XP ${jogador.xp}`;
  levelEl.textContent         = `Nível ${jogador.nivel}`;

  // Tag de classe
  const classTagEl = document.getElementById("hud-class-tag");
  if (classTagEl) classTagEl.textContent = jogador.classe;

  // Stats dinâmicos por classe
  const statsEl = document.getElementById("hud-stats");
  if (statsEl) {
    const statsFactory = classHudStats[jogador.classe];
    const stats = statsFactory ? statsFactory(jogador) : [];

    statsEl.replaceChildren(...stats.map(s => {
      const chip = document.createElement("div");
      chip.className = "hud-stat-chip";
      chip.id = `hud-${s.id}`;
      chip.innerHTML = `
        <span class="hud-stat-icon">${s.icon}</span>
        <div class="hud-stat-body">
          <span class="hud-stat-label">${s.label}</span>
          <strong class="hud-stat-val">${s.val}</strong>
        </div>
      `;
      return chip;
    }));
  }
}

function renderStage(state) {
  if (state.fase === "criacao") {
    stageEl.textContent = "Escolha quem vai atender ao chamado da Floresta de Letes.";
    return;
  }

  if (state.fase === "fim") {
    stageEl.textContent = state.vitoria
      ? "Vitoria: a floresta foi purificada."
      : "Derrota: o bosque ainda domina o caminho.";

    return;
  }

  stageEl.textContent = `Etapa ${state.etapa.numero}/${state.totalEtapas}: ${state.etapa.nome}`;
}

// Descrições completas e stats por classe para o modal
const classDetails = {
  guerreiro: {
    desc: "Mestre das armas e da resistência física. Com o maior HP entre as classes, o Guerreiro dispõe de Retomar Fôlego para se curar no combate, Golpe Preciso para impor desvantagem ao inimigo e Surto de Ação para recuperar uma ação extra no turno. A classe mais direta ao ponto.",
    stats: [
      { label: "HP inicial", val: "10 + CON" },
      { label: "Dado de vida", val: "1d10" },
      { label: "Armadura", val: "Pesada" },
      { label: "Ataque Extra", val: "Nível 5" }
    ],
    placeholder: "Kael Varn"
  },
  mago: {
    desc: "Erudito arcano de baixo HP mas alto potencial de dano. Conta com Rajada Arcana, Raio de Gelo, Raio de Fogo, Mísseis Mágicos e Paralisar para controlar o inimigo. Proteção Arcana cria um escudo mágico de HP extra enquanto durar.",
    stats: [
      { label: "HP inicial", val: "8 + CON" },
      { label: "Dado de vida", val: "1d6" },
      { label: "Armadura", val: "Leve" },
      { label: "Slots de magia", val: "Nível 1 e 2" }
    ],
    placeholder: "Eldrin"
  },
  paladino: {
    desc: "O equilíbrio entre ataque e sustentação. Combina armadura pesada com Cura pelas Mãos para recuperar HP e Destruição Divina (Smite) para golpes devastadores. Bênção, Escudo da Fé e Arma Mágica ampliam suas opções táticas.",
    stats: [
      { label: "HP inicial", val: "10 + CON" },
      { label: "Dado de vida", val: "1d10" },
      { label: "Armadura", val: "Pesada" },
      { label: "Cura pelas Mãos", val: "Nível × 5" }
    ],
    placeholder: "Sir Alden"
  }
};

let _selectedClass = null;

function renderClasses(state) {
  const gameShell = document.querySelector(".game-shell");
  const isCreating = state.fase === "criacao";

  creationPanel.classList.toggle("hidden", !isCreating);
  gameShell.classList.toggle("is-creating", isCreating);

  if (!isCreating) {
    return;
  }

  classCardsEl.replaceChildren(...state.classes.map((classe) => {
    const card = document.createElement("button");

    card.className = "class-card";
    card.type = "button";

    card.innerHTML = `
      <img src="${classe.imagem}" alt="">
      <strong>${classe.nome}</strong>
      <span>${classe.arquetipo}</span>
      <p>${classe.descricao}</p>
    `;

    card.addEventListener("click", () => {
      showClassModal(classe);
    });

    return card;
  }));
}

function showClassModal(classe) {
  _selectedClass = classe;
  const modal = document.getElementById("class-confirm-modal");
  const detail = classDetails[classe.id] || { desc: classe.descricao, stats: [], placeholder: "" };

  document.getElementById("modal-class-img").src = classe.imagem;
  document.getElementById("modal-class-archetype").textContent = classe.arquetipo;
  document.getElementById("modal-class-name").textContent = classe.nome;
  document.getElementById("modal-class-desc").textContent = detail.desc;

  const statsEl = document.getElementById("modal-class-stats");
  statsEl.replaceChildren(...detail.stats.map(s => {
    const el = document.createElement("div");
    el.className = "modal-stat-item";
    el.innerHTML = `<strong>${s.val}</strong>${s.label}`;
    return el;
  }));

  const nameInput = document.getElementById("modal-name-input");
  nameInput.value = "";
  nameInput.placeholder = detail.placeholder || classe.nome;

  modal.classList.remove("hidden");
  nameInput.focus();
}

function closeClassModal() {
  document.getElementById("class-confirm-modal").classList.add("hidden");
  _selectedClass = null;
}

const pathTypeIcons = {
  "Combate 1x1": "⚔",
  "Chefe":       "💀",
  "Zona segura": "🏕",
  "Mercador":    "🛒",
  "Historia":    "📜"
};

function renderPaths(caminhos) {
  pathCardsEl.replaceChildren(...caminhos.map((caminho) => {
    const card = document.createElement("button");
    const isBoss = caminho.boss;
    const icon = pathTypeIcons[caminho.natureza] || "⚔";

    card.className = `path-mission-card ${isBoss ? "mission-boss" : ""}`;
    card.type = "button";
    card.dataset.path = caminho.id;

    card.innerHTML = `
      <div class="mission-header">
        <span class="mission-badge ${isBoss ? "badge-boss" : ""}">
          ${icon} ${caminho.natureza}
        </span>
        ${isBoss ? `<span class="mission-boss-tag">⚠ CHEFE</span>` : ""}
      </div>

      <div class="mission-body">
        <h3 class="mission-title">${caminho.titulo}</h3>
        <p class="mission-desc">${caminho.descricao}</p>
      </div>

      <div class="mission-footer">
        <div class="mission-info-grid">
          <div class="mission-info-item">
            <span class="mission-info-label">⚔ Desafio</span>
            <span class="mission-info-val">${caminho.risco}</span>
          </div>
          <div class="mission-info-item">
            <span class="mission-info-label">🏆 Recompensa</span>
            <span class="mission-info-val">${caminho.recompensa}</span>
          </div>
        </div>
        <div class="mission-cta">
          <span>${isBoss ? "Enfrentar Chefe" : "Iniciar Missão"} →</span>
        </div>
      </div>
    `;

    card.addEventListener("click", async () => {
      render(await postJson("/api/jogo/caminho", { caminho: caminho.id }));
    });

    return card;
  }));
}

function renderEvent(state) {
  const show = ["cutscene", "descanso", "loja"].includes(state.fase);

  eventPanel.classList.toggle("hidden", !show);
  choicePanel.classList.toggle("hidden", state.fase !== "caminho");

  if (!show || !state.evento) {
    return;
  }

  eventTypeEl.textContent = state.evento.tipo;
  eventTitleEl.textContent = state.evento.titulo;
  eventDescriptionEl.textContent = state.evento.descricao;

  restButton.classList.toggle("hidden", state.fase !== "descanso");
  continueButton.classList.toggle("hidden", state.fase === "descanso");
  shopListEl.classList.toggle("hidden", state.fase !== "loja");

  if (state.fase === "loja") {
    shopListEl.replaceChildren(...state.loja.map((item) => {
      const row = document.createElement("article");

      row.className = "shop-item";

      row.innerHTML = `
        <div>
          <strong>${item.nome}</strong>
          <p>${item.descricao}</p>
        </div>
        <button type="button">${item.preco} ouro</button>
      `;

      row.querySelector("button").disabled = state.jogador.ouro < item.preco;

      row.querySelector("button").addEventListener("click", async () => {
        render(await postJson("/api/jogo/evento", { acao: "comprar", item: item.id }));
      });

      return row;
    }));
  } else {
    shopListEl.replaceChildren();
  }
}

// ── Parser: extrai atacante, ação e resultado da entrada de log ──
function parseCombatAction(logText) {
  if (!logText) return null;

  const t = logText.toLowerCase();
  const youActing = /^você\b/.test(t);

  // ── Atacante ──────────────────────────────────────────────────
  let attacker, atkClass;
  if (youActing) {
    attacker = "Você";
    atkClass = "fb-player";
  } else {
    // Nome do inimigo: tudo antes de "ataca", "usa" ou "dispara"
    const m = logText.match(/^(.+?)\s+(?:ataca|usa|dispara|lança)/i);
    attacker = m ? m[1] : "Inimigo";
    atkClass = "fb-enemy";
  }

  // ── Ação/Habilidade ────────────────────────────────────────────
  const actionMap = [
    [/smite/i,                    "⚡", "Smite Divino"    ],
    [/bênção|bencao/i,            "✨", "Bênção"          ],
    [/escudo.+fé|escudo_fe/i,     "🛡", "Escudo da Fé"   ],
    [/trovejante/i,               "⚡", "Trovejante"      ],
    [/ajuda/i,                    "🤝", "Ajuda"           ],
    [/restauraç/i,                "💊", "Restauração"     ],
    [/arma.+mágica/i,             "🗡", "Arma Mágica"    ],
    [/cura.+mãos|lay.+hand/i,     "💛", "Cura pelas Mãos"],
    [/rajada/i,                   "🔮", "Rajada Arcana"   ],
    [/raio.+gelo/i,               "❄", "Raio de Gelo"   ],
    [/raio.+fogo/i,               "🔥", "Raio de Fogo"   ],
    [/mísseis|misil/i,            "✨", "Mísseis Mágicos" ],
    [/paralisar|paralisa/i,       "💠", "Paralisar"       ],
    [/prot.+lâminas/i,            "🛡", "Prot. Lâminas"  ],
    [/prot.+arcana/i,             "🔮", "Prot. Arcana"    ],
    [/golpe.+preciso/i,           "🎯", "Golpe Preciso"   ],
    [/surto/i,                    "⚡", "Surto de Ação"   ],
    [/fôlego|folego/i,            "💨", "Retomar Fôlego"  ],
    [/foco/i,                     "🎯", "Foco"            ],
    [/poção|pocao/i,              "🧪", "Poção"           ],
  ];

  let atkIcon = "⚔", atkLabel = "Ataque";
  for (const [pat, icon, label] of actionMap) {
    if (pat.test(logText)) { atkIcon = icon; atkLabel = label; break; }
  }

  // ── Resultado ──────────────────────────────────────────────────
  let result = "neutral", resultLabel = "—";
  if      (/acerta!?|acerto!?/i.test(logText)) { result = "hit";     resultLabel = "Acertou!";    }
  else if (/erra!?|falha!?/i.test(logText))    { result = "miss";    resultLabel = "Errou!";      }
  else if (/cura\s+\d|recupera\s+\d/i.test(logText)) { result = "heal"; resultLabel = "Curou!"; }
  else if (/paralisa/i.test(logText))           { result = "special"; resultLabel = "Paralisado!"; }
  else if (/protege/i.test(logText))            { result = "special"; resultLabel = "Protegido!";  }
  else if (/bênção/i.test(logText))             { result = "special"; resultLabel = "Abençoado!";  }
  else if (/encerra/i.test(logText))            { result = "neutral"; resultLabel = "Turno encerrado"; }

  // ── Valor numérico (dano ou cura) ─────────────────────────────
  const dmgM  = logText.match(/\b(\d+)\s+de\s+dano\b/i);
  const healM = logText.match(/(?:cura|recupera)\s+(\d+)/i);
  let detailVal = null, detailType = null;
  if      (dmgM)  { detailVal = `−${dmgM[1]}`;  detailType = "damage"; }
  else if (healM) { detailVal = `+${healM[1]}`;  detailType = "heal";   }

  return { attacker, atkClass, atkIcon, atkLabel, result, resultLabel, detailVal, detailType };
}

// ── Renderiza o painel de feedback estruturado ─────────────────
function renderFeedback(state, inimigo) {
  // Usa o texto simples do backend como chave de mudança
  const rawKey = state.feedback?.texto || "pronto";
  if (rawKey === _prevFeedbackText) return; // nada mudou
  _prevFeedbackText = rawKey;

  const tipo = state.feedback?.tipo || "neutral";
  const latestLog = state.log?.[state.log.length - 1] || null;
  const p = parseCombatAction(latestLog);

  if (p && rawKey !== "Pronto" && p.result !== "neutral") {
    feedbackEl.className = `feedback ${tipo} feedback-rich`;
    feedbackEl.innerHTML = `
      <div class="fb-who">
        <span class="fb-attacker ${p.atkClass}">${p.attacker}</span>
        <span class="fb-sep">›</span>
        <span class="fb-action-label">${p.atkIcon} ${p.atkLabel}</span>
      </div>
      <div class="fb-result fb-result--${p.result}">${p.resultLabel}</div>
      ${p.detailVal
        ? `<div class="fb-detail fb-detail--${p.detailType}">${p.detailVal} HP</div>`
        : ""}
    `;
  } else {
    // Estado inicial / encerrar turno / sem ação
    feedbackEl.className = `feedback ${tipo}`;
    feedbackEl.textContent = rawKey === inimigo?.nome ? "" : rawKey;
  }

  // Pulso de entrada
  feedbackEl.classList.remove("feedback-pulse");
  void feedbackEl.offsetWidth;
  feedbackEl.classList.add("feedback-pulse");
}

// ── Estado de rastreamento de combate ────────────────────
let _lastCombatEnemyName = null;
let _lastLogLength       = 0;
let _prevFeedbackText    = "";


// ── Classifica tipo da entrada de log ────────────────────
function getLogEntryType(text) {
  const t = text.toLowerCase();
  if (/acerta|cura|recupera|ganha|proteg|bênção|arma mágica|golpe/.test(t)) return "success";
  if (/erra|recebe|sofre|perde|ataca você|paralisa|falha/.test(t))           return "danger";
  if (/smite|magia|arcana|trovejante|mísseis|restaura|ajuda|foco|surto/.test(t)) return "special";
  return "neutral";
}

// ── Formata a entrada com ícone + destaques inline ────────
function formatLogEntry(text) {
  const type  = getLogEntryType(text);
  const icons = { success: "⚔", danger: "🩸", special: "✨", neutral: "◆" };

  // Escape HTML antes de processar
  const esc = text.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;");

  const html = esc
    // Expressões de dado: d20=15+5=20 ou 2d6+3=11
    .replace(/(\d*d\d+[=+\-\d]*)/gi,
      '<span class="dice-expr">$1</span>')
    // Transição de HP: HP: 18 → 7  ou  HP 18 → 7
    .replace(/HP[:\s]+(\d+)\s*[→>]\s*(\d+)/gi,
      'HP <span class="hp-before">$1</span> → <span class="hp-after">$2</span>')
    // "X de dano"
    .replace(/\b(\d+)\s+de\s+dano\b/gi,
      '<strong class="dmg-num">$1</strong> de dano')
    // Palavras de acerto
    .replace(/\b(acerta!?|acerto!?|cura!?|recupera!?)\b/gi,
      '<span class="kw-hit">$1</span>')
    // Palavras de erro
    .replace(/\b(erra!?|falha!?)\b/gi,
      '<span class="kw-miss">$1</span>');

  return `<span class="log-entry-icon" aria-hidden="true">${icons[type]}</span>`
       + `<span class="log-entry-text">${html}</span>`;
}

// ── Renderiza log com animações escalonadas ────────────────
function renderCombatLog(logEntries) {
  const logList = document.getElementById("combat-log-entries");
  if (!logList) return;

  if (!logEntries?.length) {
    logList.innerHTML = `<li class="combat-log-entry entry-neutral"><span class="log-entry-icon">◆</span><span class="log-entry-text">Aguardando ação...</span></li>`;
    return;
  }

  const newCount = Math.max(0, logEntries.length - _lastLogLength);
  _lastLogLength = logEntries.length;

  const recent = logEntries.slice(-6).reverse(); // mais recente no topo

  logList.replaceChildren();
  recent.forEach((entry, index) => {
    const li = document.createElement("li");
    const isNew = index < newCount;

    li.className = `combat-log-entry entry-${getLogEntryType(entry)}`;
    li.innerHTML  = formatLogEntry(entry);

    if (isNew) {
      // delay escalonado: 1ª entrada imediata, demais +130ms cada
      li.style.animationDelay = `${index * 130}ms`;
      li.classList.add("entry-anim");
    }
    logList.appendChild(li);
  });
}

// ── renderCombat principal ────────────────────────────────
function renderCombat(state) {
  const emCombate = state.fase === "combate" && state.inimigo;

  // Detecta vitória
  if (_lastCombatEnemyName && !emCombate && state.fase !== "fim") {
    showVictoryPopup(_lastCombatEnemyName);
  }
  // Novo combate → zera contador de log
  if (emCombate && !_lastCombatEnemyName) {
    _lastLogLength = 0;
    _prevFeedbackText = "";
  }
  _lastCombatEnemyName = emCombate ? state.inimigo.nome : null;

  combatPanel.classList.toggle("hidden", !emCombate);
  finalPanel.classList.toggle("hidden", state.fase !== "fim");

  if (!emCombate) return;

  const inimigo = state.inimigo;
  const enemyPercent = Math.max(0, Math.min(100, (inimigo.hpAtual / inimigo.hpMaximo) * 100));

  enemyNameEl.textContent = inimigo.boss ? `⚠ BOSS: ${inimigo.nome}` : inimigo.nome;
  enemyPlateNameEl.textContent = inimigo.nome;
  enemyHpEl.textContent = `HP ${inimigo.hpAtual}/${inimigo.hpMaximo}`;
  enemyArmorEl.textContent = `CA ${inimigo.classeArmadura}`;

  // Flash na barra de HP do inimigo quando diminui
  const prevWidth = parseFloat(enemyHpBarEl.style.width) || 100;
  const newWidth  = enemyPercent;
  enemyHpBarEl.style.width = `${newWidth}%`;
  if (newWidth < prevWidth) {
    enemyHpBarEl.classList.remove("hp-bar-hit");
    void enemyHpBarEl.offsetWidth;
    enemyHpBarEl.classList.add("hp-bar-hit");
  }

  const spriteClass = enemySpriteClasses[inimigo.nome] || "twig";
  const image = enemyImages[inimigo.nome] || enemyImages["Twig Blight"];
  enemySpriteEl.className = `sprite enemy-sprite image-enemy ${spriteClass}`;
  enemySpriteEl.innerHTML = `<img src="${image}" alt="">`;

  // Feedback estruturado: usa a última entrada do log
  renderFeedback(state, inimigo);


  // Log com animações
  renderCombatLog(state.log);

  updateActionButtons(state);

  // Oculta grupos sem botões visíveis
  document.querySelectorAll(".action-group").forEach(group => {
    const has = [...group.querySelectorAll("[data-action]")].some(b => !b.classList.contains("hidden"));
    group.style.display = has ? "" : "none";
  });
}

function updateActionButtons(state) {
  const classe = state.jogador.classe;
  const mainUsed = state.turno.acaoPrincipalUsada;
  const bonusUsed = state.turno.acaoBonusUsada;

  document.querySelectorAll("[data-action]").forEach((button) => {
    const action = button.dataset.action;

    const isPaladin = [
      "atacar",
      "smite",
      "bencao",
      "escudo_fe",
      "trovejante",
      "ajuda",
      "restauracao",
      "arma_magica",
      "cura",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const isWarrior = [
      "atacar",
      "golpe",
      "folego",
      "surto",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const isMage = [
      "rajada",
      "raio_gelo",
      "raio_fogo",
      "protecao_laminas",
      "protecao_arcana",
      "misil",
      "paralisar",
      "pocao",
      "foco",
      "encerrar"
    ].includes(action);

    const visible = classe === "Paladino"
      ? isPaladin
      : classe === "Guerreiro"
        ? isWarrior
        : isMage;

    button.classList.toggle("hidden", !visible);

    const bonus = [
      "cura",
      "pocao",
      "folego",
      "foco",
      "surto",
      "escudo_fe",
      "trovejante",
      "arma_magica"
    ].includes(action);

    let disabled = bonus ? bonusUsed : mainUsed;

    if (action === "encerrar") disabled = !mainUsed;

    if (["smite", "bencao", "escudo_fe", "trovejante"].includes(action)) {
      disabled = disabled || state.jogador.slotsNivel1 <= 0;
    }

    if (["ajuda", "restauracao", "arma_magica"].includes(action)) {
      disabled = disabled || state.jogador.slotsNivel2 <= 0;
    }

    if (action === "misil") {
      disabled = disabled || state.jogador.slotsNivel1 <= 0;
    }

    if (action === "paralisar") {
      disabled = disabled || (state.jogador.slotsNivel1 + state.jogador.slotsNivel2 <= 0);
    }

    if (action === "cura") {
      disabled = disabled || state.jogador.curaPelasMaos <= 0 || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "pocao") {
      disabled = disabled || state.jogador.pocoes <= 0 || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "folego") {
      disabled = disabled || state.jogador.hpAtual >= state.jogador.hpMaximo;
    }

    if (action === "surto") {
      disabled = disabled || !state.turno.surtoDisponivel;
    }
        const labels = {
      atacar: "Ataque",
      golpe: "Golpe Preciso",
      folego: "Retomar Fôlego",
      surto: "Surto de Ação",
      rajada: "Rajada",
      raio_gelo: "Raio de Gelo",
      raio_fogo: "Raio de Fogo",
      protecao_laminas: "Proteção Lâminas",
      protecao_arcana: "Proteção Arcana",
      misil: "Mísseis Mágicos",
      paralisar: "Paralisar",
      smite: "Smite",
      cura: "Cura",
      bencao: "Bênção",
      escudo_fe: "Escudo da Fé",
      trovejante: "Golpe Trovejante",
      ajuda: "Ajuda",
      restauracao: "Restauração",
      arma_magica: "Arma Mágica",
      pocao: "Poção",
      foco: "Foco",
      encerrar: "Encerrar Turno"
    };

    button.classList.remove("action-paladin", "action-mage", "action-warrior");

    if (classe === "Paladino") {
      button.classList.add("action-paladin");
    }

    if (classe === "Mago") {
      button.classList.add("action-mage");
    }

    if (classe === "Guerreiro") {
      button.classList.add("action-warrior");
    }

    button.textContent = `${icons[action] || "✦"} ${labels[action] || action}`;

    button.disabled = disabled;
  });
}

function renderSheet(jogador) {
  const attrs = jogador.atributos;

  sheetEl.innerHTML = `
    <article><span>Classe</span><strong>${jogador.classe}</strong></article>
    <article><span>Arma</span><strong>${jogador.arma}</strong></article>
    <article><span>Armadura</span><strong>${jogador.armadura}</strong></article>
    <article><span>Slots</span><strong>1:${jogador.slotsNivel1} 2:${jogador.slotsNivel2} 3:${jogador.slotsNivel3}</strong></article>
    <article><span>FOR</span><strong>${attrs.forca}</strong></article>
    <article><span>DES</span><strong>${attrs.destreza}</strong></article>
    <article><span>CON</span><strong>${attrs.constituicao}</strong></article>
    <article><span>INT</span><strong>${attrs.inteligencia}</strong></article>
    <article><span>SAB</span><strong>${attrs.sabedoria}</strong></article>
    <article><span>CAR</span><strong>${attrs.carisma}</strong></article>
  `;

  inventoryListEl.replaceChildren(...jogador.inventario.map((item) => {
    const li = document.createElement("li");

    li.textContent = item;

    return li;
  }));
}

function renderFinal(state) {
  if (state.fase !== "fim" || !state.resumoFinal) {
    return;
  }

  finalTitleEl.textContent = state.vitoria ? "Vitoria" : "Derrota";
  finalResultEl.textContent = state.resumoFinal.resultado;
  finalLevelEl.textContent = state.resumoFinal.nivelFinal;
  finalKillsEl.textContent = state.resumoFinal.inimigosDerrotados;
  finalGoldEl.textContent = state.resumoFinal.ouroFinal;

  finalPathsEl.replaceChildren(...state.resumoFinal.caminhosEscolhidos.map((path) => {
    const item = document.createElement("li");
    item.textContent = path;
    return item;
  }));

  mostrarTelaFinal(state);
}

function mostrarTelaFinal(state) {
  let overlay = document.getElementById("final-overlay");

  if (!overlay) {
    overlay = document.createElement("div");
    overlay.id = "final-overlay";
    document.body.appendChild(overlay);
  }

  const titulo = state.vitoria ? "JORNADA<br>VENCIDA" : "JORNADA<br>PERDIDA";

  const subtitulo = state.vitoria
    ? "A FLORESTA FOI PURIFICADA"
    : "AS SOMBRAS AINDA DOMINAM";

  const texto = state.vitoria
    ? "Você venceu a jornada em Shadow Forest.<br>A Pluma de Prata agora canta sua lenda."
    : "Sua jornada terminou nas sombras.<br>Mas uma nova aventura ainda pode começar.";

  overlay.innerHTML = `
    <div class="final-content">
      <div class="intro-icon">⚔</div>

      <h1>${titulo}</h1>
      <h2>${subtitulo}</h2>

      <p>${texto}</p>

      <div class="final-stats-overlay">
        <article>
          <span>Nível final</span>
          <strong>${state.resumoFinal.nivelFinal}</strong>
        </article>

        <article>
          <span>Inimigos derrotados</span>
          <strong>${state.resumoFinal.inimigosDerrotados}</strong>
        </article>

        <article>
          <span>Ouro final</span>
          <strong>${state.resumoFinal.ouroFinal}</strong>
        </article>
      </div>

      <button id="restart-adventure" type="button">
        NOVA AVENTURA ▶
      </button>
    </div>
  `;

  overlay.classList.add("show");

  document.getElementById("restart-adventure").addEventListener("click", async () => {
    localStorage.removeItem(saveKey);
    overlay.remove();

    const novoJogo = await postJson("/api/jogo/novo");
    render(novoJogo);
  });
}

function renderLog(log) {
  logEl.replaceChildren(...log.map((entry) => {
    const item = document.createElement("li");

    item.textContent = entry;

    return item;
  }));
}

document.querySelector("#new-game").addEventListener("click", async () => {
  localStorage.removeItem(saveKey);

  render(await postJson("/api/jogo/novo"));
});

toggleSheetButton.addEventListener("click", () => {
  panelState.sheet = !panelState.sheet;

  renderPanels();
});

toggleLogButton.addEventListener("click", () => {
  panelState.log = !panelState.log;

  renderPanels();
});

document.querySelectorAll("[data-close-panel]").forEach((button) => {
  button.addEventListener("click", () => {
    panelState[button.dataset.closePanel] = false;

    renderPanels();
  });
});

// Modal de personagem — cancelar / fechar
document.getElementById("modal-cancel").addEventListener("click", closeClassModal);
document.getElementById("modal-close").addEventListener("click", closeClassModal);
document.querySelector(".modal-backdrop").addEventListener("click", closeClassModal);

// Modal de personagem — confirmar
document.getElementById("modal-confirm").addEventListener("click", async () => {
  if (!_selectedClass) return;

  // Salva os valores ANTES de fechar o modal (closeClassModal zera _selectedClass)
  const classeId = _selectedClass.id;
  const nomeDigitado = document.getElementById("modal-name-input").value.trim();

  closeClassModal();

  render(await postJson("/api/jogo/personagem", {
    classe: classeId,
    nome: nomeDigitado || null
  }));
});

// Confirmar com Enter no campo de nome
document.getElementById("modal-name-input").addEventListener("keydown", async (e) => {
  if (e.key === "Enter" && _selectedClass) {
    document.getElementById("modal-confirm").click();
  }
  if (e.key === "Escape") {
    closeClassModal();
  }
});

document.querySelectorAll("[data-action]").forEach((button) => {
  button.addEventListener("click", async () => {
    render(await postJson("/api/jogo/acao", { acao: button.dataset.action }));
  });
});

restButton.addEventListener("click", async () => {
  render(await postJson("/api/jogo/evento", { acao: "descansar" }));
});

continueButton.addEventListener("click", async () => {
  render(await postJson("/api/jogo/evento", { acao: "continuar" }));
});

document.addEventListener("DOMContentLoaded", () => {
  const intro = document.getElementById("intro-screen");
  const startButton = document.getElementById("start-adventure");
  const welcomeScreen = document.getElementById("welcome-screen");
  const chooseHeroButton = document.getElementById("choose-hero-button");

  // Passo 1 — clica em "NOVA AVENTURA": intro some, welcome aparece
  if (intro && startButton) {
    startButton.addEventListener("click", () => {
      intro.style.opacity = "0";
      intro.style.transition = "opacity 0.8s ease";

      setTimeout(() => {
        intro.remove();
        document.body.classList.remove("intro-active");

        // Mostra a tela de boas-vindas
        welcomeScreen.classList.remove("hidden");
      }, 800);
    });
  }

  // Passo 2 — clica em "ESCOLHER HERÓI": welcome some, jogo inicia
  if (chooseHeroButton && welcomeScreen) {
    chooseHeroButton.addEventListener("click", async () => {
      localStorage.removeItem(saveKey);

      welcomeScreen.classList.add("fade-out");

      const novoJogo = await postJson("/api/jogo/novo");

      setTimeout(() => {
        welcomeScreen.classList.add("hidden");
        welcomeScreen.classList.remove("fade-out");
        render(novoJogo);
      }, 700);
    });
  }
});

// ── Popup de Vitória ──────────────────────────────────────
function showVictoryPopup(enemyName) {
  // Remove popup anterior se existir
  document.getElementById("victory-popup")?.remove();

  const popup = document.createElement("div");
  popup.id = "victory-popup";
  popup.setAttribute("role", "dialog");
  popup.setAttribute("aria-modal", "true");
  popup.innerHTML = `
    <div class="victory-card">
      <div class="victory-icon">⚔</div>
      <h2 class="victory-title">Inimigo Derrotado!</h2>
      <p class="victory-enemy">${enemyName}</p>
      <p class="victory-text">Você venceu a batalha.<br>Avance para a próxima etapa da jornada.</p>
      <button class="victory-btn" id="victory-close" type="button">Avançar →</button>
    </div>
  `;
  document.body.appendChild(popup);

  // Anima entrada com duplo requestAnimationFrame para garantir o CSS transition
  requestAnimationFrame(() => requestAnimationFrame(() => popup.classList.add("show")));

  function closePopup() {
    popup.classList.remove("show");
    setTimeout(() => popup.remove(), 400);
  }

  document.getElementById("victory-close").addEventListener("click", closePopup);
  // Fecha ao clicar fora do card
  popup.addEventListener("click", (e) => { if (e.target === popup) closePopup(); });
  // Auto-fecha após 8 segundos
  setTimeout(() => { document.getElementById("victory-popup") && closePopup(); }, 8000);
}

loadGame();