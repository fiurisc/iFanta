var rose = [];
var panchina = [];
var players = [];
var teamimg = [];
var teamall = [];
var np = 0;
var nd = 0;
var nc = 0;
var na = 0;

function pad(value, length) {
  return (value.toString().length < length) ? pad("0"+value, length):value;
}

function loadFile(filePath) {
  var result = null;
  var xmlhttp = new XMLHttpRequest();
  xmlhttp.open("GET", filePath, false);
  xmlhttp.send();
  if (xmlhttp.status==200) {
    result = xmlhttp.responseText;
  }
  return result;
}

function GetRose(){
    
    rose=[];
	
	// Carico la rosa//
	var file = loadFile("script/rose.txt")
	lines = file.split("\n");
	for (x = 0; x < lines.length; x++) {
		value = lines[x].split("|");
		var tind = parseInt(value[0], 10);
		var pind = parseInt(value[1], 10);
		if (typeof rose[tind] === 'undefined') rose[tind]=[];
		rose[tind][pind]=[value[2],value[3],value[4]]; 
    }
}

function SetTeamList(menu) {
	
	var team = [];
	
	// Carico la lista dei teams//
	var file = loadFile("script/team.txt")
	lines = file.split("\n");
	for (x = 0; x < lines.length; x++) {
		value = lines[x].split("|");
		var tind = parseInt(value[0], 10);
		team[tind]= value[1];
		teamall[tind]= value[2];
		teamimg[tind]= value[3];
    }
	
	for (let i = 0; i < team.length; i++) {
		var option = document.createElement("option");
		option.value = team[i];
		option.text = team[i];
		menu.add(option);
	}
}

function SetDaysList(menu) {

	for (let i = 1; i < 39; i++) {
		var option = document.createElement("option");
		option.value = i;
		option.text = pad(String(i),2,'0');
		menu.add(option);
	}
}

function GetRuoloForeColor(Ruolo){
	
	console.log(Ruolo);
	if (Ruolo == "P") {
		return "orange";
	} else if (Ruolo == "D") {
		return "green";
	} else if (Ruolo == "C") {
		return "red";
	} else {
		return "blue";
	}
}

function LoadRosa(teamid){

	
	players = [];
	panchina = [];
	np = 0;
	nd = 0;
	nc = 0;
	na = 0;
	
	for (let i = 0; i < rose[teamid].length; i++) {
		let user = {
			type:0,
			idform:0,
			ruolo: rose[teamid][i][0],
			nome: rose[teamid][i][1],
			squadra: rose[teamid][i][2]
		};
		players.push(user);
	}
	
	for (let i = 0; i < 10; i++) {
		panchina.push(-1);
	}
}

function GetHtmlPlayersList(){

	var html;
	html="<table width='100%' id='tb2' border='0'>\n";
	html+="<tr class='header'><td style='text-align:center;'>ID</td><td style='text-align:center;'>T</td><td style='text-align:center;'>P</td><td style='text-align:center;'>R.</td><td>Nome</td><td>Squadra</td></tr>\n";
	
	for (let i = 0; i < players.length; i++) {
		html+="<tr><td width='30pt' style='text-align:center;'>" + (i+1) + "</td><td width='30pt'><input type='image' id='imgtit" + i + "' src='img/okdis.png' style='width:50px;' onclick='tit_click(this)' /></td><td width='30pt'><input type='image' id='imgpanc" + i + "' src='img/okdis.png' style='width:50px;' onclick='panc_click(this)'/></td><td width='30pt'><span id='t1ruolo" + i + "' style='color:"+GetRuoloForeColor(players[i].ruolo)+";'>" + players[i].ruolo + "</span></td><td><span id='t1nome0'>" + players[i].nome + "</span></td><td><span id='t1squadra0'>" + players[i].squadra + "</span></td></tr>\n";
	}
	html+="</table>\n";
	return html;
}

function GetHtmlPanchinaList(){

	var html;
	html="<table width='100%' id='tb3' border='0'>\n";
	html+="<tr class='header'><td>ID</td><td style='text-align:center;'>Up</td><td style='text-align:center;'>Do</td><td style='text-align:center;'>R.</td><td>Nome</td><td>Squadra</td></tr>\n";
	
	for (let i = 0; i < panchina.length; i++) {
		html+="<tr><td width='30pt' style='text-align:center;'>" + (i+1) + "</td><td width='30pt'><input type='image' id='imgpancup" + i + "' src='img/up.png' style='width:60px;' onclick='panc_up_click(this)'/></td><td width='30pt'><input type='image' id='imgpancdown" + i + "' src='img/down.png' style='width:60px;' onclick='panc_down_click(this)'/></td><td width='30pt'><span id='t2ruolo" + i + "' style='color:orange;'></span></td><td><span id='t2nome" + i + "'></span></td><td><span id='t2squadra" + i + "'></span></td></tr>\n";
	}
	html+="</table>\n";
	return html;
}
	
function GetModule(){

	np = 0;
	nd = 0;
	nc = 0;
	na = 0;
	
	for (let i = 0; i < players.length; i++) {
		if (players[i].type == 1){
			if (players[i].ruolo == "P") {
				np += 1;
			} else if (players[i].ruolo == "D") {
				nd += 1;
			} else if (players[i].ruolo == "C") {
				nc += 1;
			} else {
				na += 1;
			}
		}
	}
	return nd + "-" + nc + "-" + na;

}


function CheckMudule(Ruolo,CurrP,CurrD,CurrC,CurrA){

	var ris = 0;
	var tot = CurrP + CurrD + CurrC + CurrA + 1;

	if (Ruolo == "P") {
		CurrP += 1;
	} else if (Ruolo == "D") {
		CurrD += 1;
	} else if (Ruolo == "C") {
		CurrC += 1;
	} else {
		CurrA += 1;
	}
	
	if (CurrP < 2 && CurrD < 4 && CurrC < 5 && CurrA < 4) {
		ris = 1;
	} else if (CurrP < 2 && CurrD < 3 && CurrC < 5 && CurrA < 3) { //343
		ris = 1;
	} else if (CurrP < 2 && CurrD < 4 && CurrC < 6 && CurrA < 3) { //352
		ris = 1;
	} else if (CurrP < 2 && CurrD < 5 && CurrC < 4 && CurrA < 4) { //433
		ris = 1;
	} else if (CurrP < 2 && CurrD < 5 && CurrC < 5 && CurrA < 3) { //442
		ris = 1;
	} else if (CurrP < 2 && CurrD < 5 && CurrC < 6 && CurrA < 2) { //451
		ris = 1;
	} else if (CurrP < 2 && CurrD < 6 && CurrC < 4 && CurrA < 3) { //532
		ris = 1;
	} else if (CurrP < 2 && CurrD < 6 && CurrC < 5 && CurrA < 2) { //541
		ris = 1;
	}

	return ris;

}

function GetPrimaRigaLiberaPanchina(Ruolo){

	var ind = -1;

	if (Ruolo == "P" && panchina[0] == -1){
		return 0;
	}else{
		for (i = 1 ; i < panchina.length; i++){
			if (panchina[i] == -1){
				return i;
			}
		}
	}

	return ind;
}

function GetIndicePanchina(Id){

	var ind = -1;

	for (i = 0 ; i < panchina.length; i++){
		if (panchina[i] == Id){
			return i;
		}
	}

	return ind;
}