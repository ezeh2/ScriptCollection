// app.js
const STORAGE_KEY = 'flash_es_en_v1';
const DEFAULT_WORDS = [
  // From the covers/headline
  ["Vivir","To live"],["en","on/in"],["la","the"],["luna","moon"],["Empieza","Begins"],
  ["cuenta","count/countdown"],["atrás","back"],["para","to/for"],["construir","build"],
  ["ciudades","cities"],["los","the"],["próximos","next/upcoming"],["años","years"],
  // From inner page
  ["Mientras","While"],["humanidad","humanity"],["prepara","prepares"],["volver","return"],
  ["enviar","send"],["misiones","missions"],["tripuladas","crewed/manned"],["Luna","Moon"],
  ["científicos","scientists"],["disponen","arrange/prepare"],["adaptar","adapt"],
  ["superficie","surface"],["lunar","lunar"],["necesidades","needs"],["humanas","human"],
  ["expediciones","expeditions"],["cada","each/every"],["vez","time/occasion"],["más","more"],
  ["prolongadas","prolonged"],["reportaje","report/feature"],["iniciativas","initiatives"],
  ["marcha","underway"],["hacer","make/do"],["nuestro","our"],["satélite","satellite"],
  ["lugar","place"],["armónico","harmonious"],["sostenible","sustainable"],["posible","possible"],
  ["portada","cover"],["Representación","representation"],["proyecto","project"],
  ["aldea","village"],["compuesto","composed"],["módulos","modules"],["hinchables","inflatable"],
  ["Ilustración","illustration"],["Textos","texts/writings"],["y","and"],["de","of/by"],["un","a"],["lo","it/neutral article"],["las","the"],["a","to"]
];

function loadState() {
  const saved = JSON.parse(localStorage.getItem(STORAGE_KEY) || 'null');
  if (saved) return saved;
  // initialize Leitner-like boxes: 0=new, 1=learning, 2=known
  const words = DEFAULT_WORDS.map(([es,en]) => ({es,en,box:0}));
  const state = { words, reveal:false, dir:'es-en' };
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  return state;
}
function saveState() { localStorage.setItem(STORAGE_KEY, JSON.stringify(state)); }

let state = loadState();
const el = {
  card: document.getElementById('card'),
  again: document.getElementById('again'),
  got: document.getElementById('got'),
  skip: document.getElementById('skip'),
  tbl: document.getElementById('tbl').querySelector('tbody'),
  es: document.getElementById('es'),
  en: document.getElementById('en'),
  add: document.getElementById('add'),
  reveal: document.getElementById('reveal'),
  direction: document.getElementById('direction'),
  mode: document.getElementById('mode'),
  reset: document.getElementById('reset')
};

let current = null;
let showingAnswer = false;

function filteredWords() {
  const m = el.mode.value;
  return state.words.filter(w => {
    if (m==='new') return w.box===0;
    if (m==='learning') return w.box===1;
    if (m==='known') return w.box===2;
    return true;
  });
}

function pickNext() {
  const pool = filteredWords();
  if (pool.length===0) { el.card.textContent = 'No cards in this mode.'; current=null; return; }
  current = pool[Math.floor(Math.random() * pool.length)];
  showingAnswer = false;
  drawCard();
}

function drawCard() {
  if (!current) return;
  const dir = el.direction.value;
  el.card.textContent = dir==='es-en' ? current.es : current.en;
  el.card.dataset.front = '1';
}

function flip() {
  if (!current) return;
  const dir = el.direction.value;
  const front = el.card.dataset.front === '1';
  el.card.textContent = front ? (dir==='es-en' ? current.en : current.es) :
                                 (dir==='es-en' ? current.es : current.en);
  el.card.dataset.front = front ? '0' : '1';
  showingAnswer = !front;
}

function updateBox(correct) {
  if (!current) return;
  if (correct) current.box = Math.min(2, current.box + 1);
  else current.box = 0;
  saveState();
  renderTable();
  pickNext();
}

function renderTable() {
  el.tbl.innerHTML = '';
  state.words.forEach((w, idx) => {
    const tr = document.createElement('tr');
    tr.innerHTML = `<td>${w.es}</td><td>${w.en}</td><td>${['New','Learning','Known'][w.box]}</td>
      <td><button data-i="${idx}" class="del">Delete</button>
          <button data-i="${idx}" class="to0">New</button>
          <button data-i="${idx}" class="to1">Learning</button>
          <button data-i="${idx}" class="to2">Known</button></td>`;
    el.tbl.appendChild(tr);
  });
}

el.card.addEventListener('click', () => {
  flip();
  if (state.reveal && showingAnswer) setTimeout(pickNext, 500);
});
el.again.addEventListener('click', () => updateBox(false));
el.got.addEventListener('click', () => updateBox(true));
el.skip.addEventListener('click', () => pickNext());
el.add.addEventListener('click', () => {
  const es = el.es.value.trim(), en = el.en.value.trim();
  if (!es || !en) return;
  state.words.push({es,en,box:0});
  el.es.value = ''; el.en.value = '';
  saveState(); renderTable(); pickNext();
});
el.tbl.addEventListener('click', (e) => {
  const b = e.target.closest('button'); if (!b) return;
  const i = +b.dataset.i; if (Number.isNaN(i)) return;
  if (b.className==='del') state.words.splice(i,1);
  if (b.className==='to0') state.words[i].box=0;
  if (b.className==='to1') state.words[i].box=1;
  if (b.className==='to2') state.words[i].box=2;
  saveState(); renderTable(); 
});
el.reveal.addEventListener('change', () => { state.reveal = el.reveal.checked; saveState(); });
el.direction.value = state.dir; 
el.direction.addEventListener('change', () => { state.dir = el.direction.value; saveState(); drawCard(); });
el.mode.addEventListener('change', pickNext);
el.reset.addEventListener('click', () => { localStorage.removeItem(STORAGE_KEY); state = loadState(); renderTable(); pickNext(); });
el.reveal.checked = !!state.reveal;

renderTable();
pickNext();
