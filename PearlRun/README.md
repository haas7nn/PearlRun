<h1 align="center">🦪 PearlRun</h1>

<p align="center">
<b>A 2.5D Side-Scrolling Action Adventure set in Bahrain</b><br>
Built with <b>Unity 6</b> • Course: <b>IT8101 – Games Development</b><br>
Studio: <b>PearlBytes</b>
</p>

<hr>

<style>

body{
font-family:Segoe UI, sans-serif;
background:linear-gradient(135deg,#0f2027,#203a43,#2c5364);
color:white;
}

h1{
font-size:60px;
text-shadow:0 0 20px cyan;
}

h2{
border-bottom:2px solid cyan;
padding-bottom:6px;
margin-top:40px;
}

.card{
background:rgba(255,255,255,0.05);
border-radius:12px;
padding:15px;
margin:10px;
box-shadow:0 0 15px rgba(0,255,255,0.2);
transition:0.3s;
}

.card:hover{
transform:scale(1.03);
box-shadow:0 0 30px rgba(0,255,255,0.5);
}

.level-grid{
display:grid;
grid-template-columns:repeat(auto-fit,minmax(250px,1fr));
gap:15px;
}

.feature{
background:rgba(0,0,0,0.4);
padding:10px;
border-left:5px solid cyan;
margin-bottom:10px;
border-radius:5px;
}

table{
width:100%;
border-collapse:collapse;
margin-top:10px;
}

th,td{
border:1px solid rgba(255,255,255,0.2);
padding:10px;
text-align:left;
}

th{
background:rgba(0,255,255,0.2);
}

footer{
text-align:center;
margin-top:40px;
opacity:0.7;
}

</style>

---

## 🎮 About The Game

<div class="card">

Awal is a young Bahraini running late for a **traditional pearl diving competition**.  

What starts as a simple rush quickly becomes a **country-wide chase across Bahrain**.  

The player must **jump, slide, dodge obstacles, and collect pearls** while racing through iconic locations.

Each level is packed with **local culture, humor, and recognizable landmarks** that make the experience feel authentically Bahraini.

</div>

---

## 🗺️ Levels

<div class="level-grid">

<div class="card">
<h3>Level 1 – Muharraq Streets</h3>
Tutorial level with market stalls, rooftops, and narrow alleys.
</div>

<div class="card">
<h3>Level 2 – Manama City</h3>
Urban parkour with cranes, traffic, and construction hazards.
</div>

<div class="card">
<h3>Level 3 – Qarqaoun Night</h3>
Festive chaos with lights, sweets, and cultural celebration.
</div>

<div class="card">
<h3>Level 4 – Desert & Tree of Life</h3>
Sandstorms, dunes, and survival mechanics.
</div>

<div class="card">
<h3>Level 5 – Amwaj Islands</h3>
Coastal platforming across boats and docks.
</div>

<div class="card">
<h3>Level 6 – Bahrain International Circuit</h3>
Final challenge dodging race cars and high-speed obstacles.
</div>

</div>

---

## 🎯 Game Features

<h3>Basic Features</h3>

<div class="feature">✔ Main Menu (New Game, Level Select, Settings, Credits)</div>
<div class="feature">✔ 6 unique playable levels</div>
<div class="feature">✔ Animated character (run, jump, slide, attack)</div>
<div class="feature">✔ Pearl collection system</div>
<div class="feature">✔ HUD with score and progress</div>
<div class="feature">✔ Music and sound effects</div>
<div class="feature">✔ Windows PC build</div>

---

<h3>Custom Advanced Features</h3>

<div class="feature"><b>Dynamic Enemy AI System</b> – FSM enemies with patrol, detect, chase, attack</div>

<div class="feature"><b>Power-Up System</b> – Shield, Magnet, Slow Motion, Double Points</div>

<div class="feature"><b>Cutscene System</b> – Dialogue story transitions</div>

<div class="feature"><b>Checkpoint System</b> – Auto-save and respawn</div>

<div class="feature"><b>Dynamic Weather</b> – Sandstorms, festival lights, water effects</div>

<div class="feature"><b>Progress HUD</b> – Real-time progress bar and timers</div>

---

## 🕹️ Controls

<table>

<tr>
<th>Key</th>
<th>Action</th>
</tr>

<tr>
<td>A / ←</td>
<td>Move Left</td>
</tr>

<tr>
<td>D / →</td>
<td>Move Right</td>
</tr>

<tr>
<td>Space</td>
<td>Jump / Double Jump</td>
</tr>

<tr>
<td>S / ↓</td>
<td>Slide</td>
</tr>

<tr>
<td>F / Click</td>
<td>Punch Attack</td>
</tr>

<tr>
<td>Shift</td>
<td>Sprint Burst</td>
</tr>

<tr>
<td>Esc</td>
<td>Pause Menu</td>
</tr>

</table>

---

## 👥 Team PearlBytes

<table>

<tr>
<th>Name</th>
<th>Role</th>
<th>Level</th>
</tr>

<tr>
<td>Hasan</td>
<td>Scrum Master + Lead Programmer</td>
<td>Muharraq</td>
</tr>

<tr>
<td>Ameena</td>
<td>AI & Combat Programmer</td>
<td>Manama</td>
</tr>

<tr>
<td>Ruqaya</td>
<td>Systems Programmer</td>
<td>Qarqaoun</td>
</tr>

<tr>
<td>Adil</td>
<td>Level Designer</td>
<td>Desert</td>
</tr>

<tr>
<td>Rana</td>
<td>Animator & UI Designer</td>
<td>Amwaj</td>
</tr>

<tr>
<td>Samana</td>
<td>Audio & QA</td>
<td>Circuit</td>
</tr>

</table>

---

## 🛠️ Tech Stack

<div class="card">

<b>Engine:</b> Unity 6  
<b>Language:</b> C#  
<b>Art:</b> Mixamo + Unity Asset Store  
<b>Audio:</b> Freesound, Pixabay, Mixkit  
<b>Platform:</b> Windows PC  

</div>

---

## 🏗️ Build Instructions

<div class="card">

1. Open the project in **Unity 6**  
2. Go to **File → Build Settings**  
3. Add scenes in correct order  
4. Select **Windows Platform**  
5. Click **Build**

</div>

---

## 📅 Development Timeline

<table>

<tr>
<th>Week</th>
<th>Focus</th>
</tr>

<tr>
<td>1</td>
<td>Project setup, mechanics, assets</td>
</tr>

<tr>
<td>2</td>
<td>Core systems + Level 1 & 2</td>
</tr>

<tr>
<td>3</td>
<td>All 6 levels completed</td>
</tr>

<tr>
<td>4</td>
<td>Integration and polish</td>
</tr>

<tr>
<td>5</td>
<td>Bug fixing and final submission</td>
</tr>

</table>

---

<footer>

Created for **IT8101 – Games Development**  
Bahrain Polytechnic  

</footer>
