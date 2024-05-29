import json
import os
import xml.etree.ElementTree as ET
import glob
import sqlite3
import itertools
import sys


def listify():
    depth = 0

    for line in final:
        line = line.rstrip()
        newDepth = sum(1 for i in itertools.takewhile(lambda c: c=='\t', line))
        if newDepth > depth:
            #print("<ul>"*(newDepth-depth))
            writehtml.write("<ul>"*(newDepth-depth))
        elif depth > newDepth:
            #print("</ul>"*(depth-newDepth))
            writehtml.write("</ul>"*(depth-newDepth))
        #print("<li>%s" %(line.strip()))
        writehtml.write("<li %s" %(line.strip()))
        depth = newDepth

def textt(rootindx, hhh):
	for asd in listoflists:
		#asd = x.split(",")
		matching = []
		if str(rootindx.replace("(", "[").replace(")","]")) == asd[0]:
			for fg in strings[int(asd[0][1:-1])]:
				final.append("\t"*hhh + fg + "\n")
			for ffff in asd:
				#link
				if "+++" in str(ffff):
					final.append("\t"*(hhh+1) + " class='goto'><span class='goto' data-id='" + ffff[4:-4] + "'> Link to Node "+ ffff[4:-4] + "</span></li>" + "\n")
				elif "===" in str(ffff):
					repepepeepp = ""
					if "<span class='dialog'>" in str(strings[int(ffff[4:-4])][0]):
						#print(str(strings[int(ffff[4:-4])][0]))
						if "class='npcplayer'>Player</span><" in str(strings[int(ffff[4:-4])][0]):
							repepepeepp += "Player: "
						else:
							if "npcgroup" in str(strings[int(ffff[4:-4])][0]):
								repepepeepp += str(strings[int(ffff[4:-4])][0]).split("style='display: inline-block;'>")[1].split("</div></a>")[0] + ": "
							else:
								repepepeepp += str(strings[int(ffff[4:-4])][0]).split("<div class='npc' style='display: inline-block;'>")[1].split("</div></a>")[0] + ": "
						repepepeepp += str(strings[int(ffff[4:-4])][0]).split("<span class='dialog'>")[1].split("</span>")[0]
						final.append("\t"*(hhh+1) + " class='goto' title='" + repepepeepp.replace("<", "&lt;").replace(">", "&gt;").replace("'", "&#39") + "'><span class='goto' data-id='" + ffff[4:-4] + "'>Alias to Node " + str(ffff[4:-4]) + "</span></li>" + "\n")
					else:
						final.append("\t"*(hhh+1) + " class='goto' title='" + str(strings[int(ffff[4:-4])][0].replace("<", "&lt;").replace(">", "&gt;").replace("'", "&#39") + "'><span class='goto' data-id='" + ffff[4:-4] + "'>Alias to Node " + str(ffff[4:-4]) + "</span></li>" + "\n"))
				elif "(" in str(ffff):
					matching.append(ffff)
			hhh+=1
			for l in matching:
				textt(l,hhh)

def parsenode():
	global uuidlistcheck
	global uuidlist
	for idx, x in enumerate(nodelist):
		links = []
		uuidlist = []
		test = []
		constructor = x['constructor']['value']

		if "child" in x['children'][0].keys():
			for c in x['children'][0]['child']:
				uuididx = getuuididx(c['UUID']['value'])
				if c['UUID']['value'] not in uuidlistcheck:
					uuidlistcheck.append(c['UUID']['value'])
					uuidlist.append(uuididx)
				else:
					for idx2, ggg in enumerate(nodelist):
						if ggg['UUID']['value'] == c['UUID']['value']:
							links.append("[+++"+ str(idx2) + "+++]")

		test.append("[" + str(idx) + "]")
		for asd in uuidlist:
			test.append("(" + str(asd) + ")")

		for fff in links:
			test.append(fff)

		if constructor == "Jump":
			for idx2, ggg in enumerate(nodelist):
				if ggg['UUID']['value'] == x['jumptarget']['value']:
					test.append("[&&&"+ str(idx2) + "&&&]")

		if constructor == "Alias":
			for idx2, ggg in enumerate(nodelist):
				if ggg['UUID']['value'] == x['SourceNode']['value']:
					test.append("[==="+ str(idx2) + "===]")

		listoflists.append(test)


def getuuididx(uuid):
	for idx, x in enumerate(nodelist):
		if x['UUID']['value'] == uuid:
			return idx

def flatten_data(y):
	out = {}
	def flatten(x, name=''):
		if type(x) is dict:
			for a in x:
				flatten(x[a], name + a + '_')
		elif type(x) is list:
			i = 0
			for a in x:
				flatten(a, name + str(i) + '_')
				i += 1
		else:
			out[name[:-1]] = x
	flatten(y)
	return out




db_file = "bg3.db"
db = sqlite3.connect(db_file)
db_cursor = db.cursor()

dialogpath = sys.argv[1]


#dialogpath = "D:\\Games\\bg3unp\\Gustav\\Mods\\Gustav\\Story\Dialogs\\"
#dialogpath = "D:\\Games\\bg3unp\\Shared\\Mods\\Shared\\Story\\Dialogs\\"
dialogfiles = glob.glob(dialogpath + "/**/*.lsj", recursive=True)

for dind, dialog in enumerate(dialogfiles, 1):
	#print(dialog)
	if os.path.getsize(dialog):
		with open(dialog, encoding="utf-8") as f:
			data = json.load(f)
	else:
		continue


	rootnodelist = []

	if "RootNodes" in data['save']['regions']['dialog']['nodes'][0].keys():
		rootnodelist = data['save']['regions']['dialog']['nodes'][0]['RootNodes']

	
	nodelist = data['save']['regions']['dialog']['nodes'][0]['node']

	howtotrigger = data['save']['regions']['editorData']['HowToTrigger']['value']

	


	synopsis = data['save']['regions']['editorData']['synopsis']['value']



	if "speaker" in data['save']['regions']['dialog']['speakerlist'][0].keys():
		speakerlist = data['save']['regions']['dialog']['speakerlist'][0]['speaker']

	root = []
	speakers = []
	speakersdict = {}

	if not rootnodelist:
		root.append("(" + str(0) + ")")
	else:
		for x in rootnodelist:
			r = getuuididx(x['RootNodes']['value'])
			root.append("(" + str(r) + ")")


	for x in speakerlist:
		if "list" in x:
			speakers.append(x['list']['value'])
			speakersdict |= {x['index']['value']:x['list']['value']}


	nodes = flatten_data(nodelist)



	strings = {}
	childsdict = {}

	for nodeidx in range(len(nodelist)):

		ttt  = str(nodeidx) + "_"
		#print("node: " + str(nodeidx))


		uuid = ""
		constructor = ""

		childslist = []



		#ruletag and text
		text = []


		speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a>"
		
		alias = ""
		tags = []
		checkflag = []
		setflag = []
		approval = ""
		approval2 = ""

		poplevel = ""

		#eRoll
		ability = ""
		skill = ""
		difficulty = ""
		advantage = ""
		rollres = ""
		jump = ""
		endnode = ""

		context = ""

		for nn, x in enumerate(nodes):
			if x.startswith(ttt):
				#UUID
				#if "_child" not in x and "_flag" not in x and "_UUID_value" in x:
					#print("uuid: " + nodes[x])
				#text string
				if "TagText_handle" in x:
					#print("text: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						text.append("<span class='dialog'>" + nodes[x] + "</span>")
					else:
						text.append("<span class='dialog'>" +  str(exist[1]).replace("<br>", "&lt;br&gt;") + "</span>")
				#childrens
				if "_children" in x and "_UUID_value" in x:
					#print("children: " + nodes[x])
					childid = getuuididx(nodes[x])
					childslist.append(childid)
				#setflag
				if "_setflags" in x and "_UUID_value" in x:
					#print("setflag: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						setflag.append("<span class='setflag' title='"+ str(None) +"'>" + nodes[x] + "</span>")
					else:
						setflag.append("<span class='setflag' title='"+ str(exist[2]).replace("'", "&#39") +"'>" +  str(exist[1]) + "</span>")
				if "_setflags" in x and "_value_value" in x:
					#print("setflag: " + str(nodes[x]))
					setflag.append(nodes[x])

				#checkflag
				if "_checkflag" in x and "_UUID_value" in x:
					#print("checkflag: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						checkflag.append("<span class='checkflag' title='"+ str(None) +"'>" + str(nodes[x]) + "</span>")
					else:
						checkflag.append("<span class='checkflag' title='"+ str(exist[2]).replace("'", "&#39") +"'>" + str(exist[1]) + "</span>")
							
				if "_checkflag" in x and "_value_value" in x:
					#print("checkflag: " + str(nodes[x]))
					checkflag.append(nodes[x])
				#ruletag
				if "Rules" in x and "_Object_value" in x:
					#print("ruletag: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						text.append("<span class='ruletag' title='" + str(None) + "'>" + nodes[x] + "</span>")
					else:
						text.append("<span class='ruletag' title='" + str(exist[2]).replace("'", "&#39") + "'>" +  str(exist[1]) + "</span>")
				#tag
				if "Tags" in x and "_Tag_value" in x:
					#print("tag: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						tags.append("<span class='tags' title='" + str(None) + "'>" + nodes[x] + "</span>")
					else:
						tags.append("<span class='tags' title='" + str(exist[2]).replace("'", "&#39") + "'>" +  str(exist[1]) + "</span>")
				if "jumptarget_value" in x:
					#print("jumptarget: " + nodes[x])
					jumptar = getuuididx(nodes[x])
					jump = "<span class='goto' data-id='" + str(jumptar) + "'> Jump to Node " + str(jumptar)
				if "jumptargetpoint_value" in x:
					jumptargetpoint	= nodes[x]
					jump += " ("+ str(jumptargetpoint) + ")</span>"
				#if "SourceNode_value" in x:
					#print("SourceNode: " + nodes[x])
				if "constructor_value" in x:
					#print("constructor: " + nodes[x])
					constructor = nodes[x]
				if "PopLevel" in x:
					poplevel = str(nodes[x])
				if "endnode_value" in x:
					#print("endnode: " + str(nodes[x]))
					if nodes[x] == True:
						endnode = "<span class='end'>End</span>"


				if nodes[x] == "CinematicNodeContext":
					CinematicNodeContext = nodes[x.replace("key_value", "val_value")]
					if CinematicNodeContext != "" and CinematicNodeContext != "<placeholder>":
						context += "CinematicNodeContext: " + CinematicNodeContext.replace("'", "&#39") + "&#013;"
				if nodes[x] == "InternalNodeContext":
					InternalNodeContext = nodes[x.replace("key_value", "val_value")]
					if InternalNodeContext != "":
						context += "InternalNodeContext: " + InternalNodeContext.replace("'", "&#39") + "&#013;"
				if nodes[x] == "NodeContext":
					NodeContext = nodes[x.replace("key_value", "val_value")]
					if NodeContext != "":
						context += "NodeContext: " + NodeContext.replace("'", "&#39") + "&#013;"

				if "Rules" not in x and "_speaker_value" in x:
					#print("speaker: " + str(nodes[x]))
					if nodes[x] == -666:
						speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>Narrator</div></a></div><span>: </span>"
					elif nodes[x] < 0:
						speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a>"
					else:
						speakerlistidlist = []
						speakerlistid = speakersdict[str(nodes[x])].split(";")
						if len(speakerlistid) > 1:
							for speakerid in speakerlistid:
								exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (speakerid,)).fetchone()
								if exist == None:
									speakerlistidlist.append(speakerid)
									#speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + speakerid + "</div></a></div><span>: </span>"
								else:
									if str(exist[1]) == "Player":
										speakerlistidlist.append("Player")
										#speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><span class='npcplayer'>Player</span><span>: </span>"
									else:
										speakerlistidlist.append(str(exist[1]))
										#speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + str(exist[1]) + "</div></a></div><span>: </span>"
							speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + ", ".join([str(ad) for ad in speakerlistidlist]) + "</div></a></div><span>: </span>"
						else:
							speakerid = speakersdict[str(nodes[x])]
							exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (speakerid,)).fetchone()
							if exist == None:
								speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + speakerid + "</div></a></div><span>: </span>"
							else:
								if str(exist[1]) == "Player":
									speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><span class='npcplayer'>Player</span><span>: </span>"
								else:
									if exist[2] != None:
										speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npcgroup' title='" + str(exist[2]).replace("'", "&#39") + "' style='display: inline-block;'>" + str(exist[1]) + "</div></a></div><span>: </span>"
									else:
										speaker = "><div><span class='nodeid'>" + str(nodeidx) + ". </span><a class='anchor' id='n" + str(nodeidx) + "'></a><div style='display:inline-block;'><div class='npc' style='display: inline-block;'>" + str(exist[1]) + "</div></a></div><span>: </span>"

				if "_ApprovalRatingID_value" in x:
					#print("ApprovalRatingID: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						approval = "<span class='approval'>" + nodes[x] + "</span>"
					else:
						approval = "<span class='approval'>" + str(exist[1]) + "</span>"
				#rolls
				if "Success_value" in x:
					#print("rollsuccess: " + str(nodes[x]))
					rollres = rollres + str(nodes[x])
				if "Ability_value" in x:
					#print("Ability: " + nodes[x])
					ability = nodes[x]
				if "DifficultyClassID_value" in x:
					#print("DifficultyClassID: " + nodes[x])
					exist = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (nodes[x],)).fetchone()
					if exist == None:
						difficulty = nodes[x]
					else:
						difficulty = str(exist[1])
				if "Skill_value" in x:
					#print("Skill: " + nodes[x])
					skill = nodes[x]
				if "Advantage_value" in x:
					#print("Advantage: " + str(nodes[x]))
					advantage = str(nodes[x])

		rolls = ""
		if "eRoll" in constructor:
			rolls = "<span class='rolls'>" + "Roll " + "(" + ability + ", " + skill + ") vs " + difficulty + " (" + advantage + ")" + "</span>"
		

		if context != "":
			context = "<span class='context' title='" + context + "'><sup>devnote</sup></span>"


		setflag2 = []
		for idx, flag in enumerate(setflag):
			if flag == True:
				setflag2.append(setflag[idx - 1])
			elif flag == False:
				setflag2.append(setflag[idx - 1] + " = False")


		checkflag2 = []
		for idx, flag in enumerate(checkflag):
			if flag == True:
				checkflag2.append(checkflag[idx - 1])
			elif flag == False:
				checkflag2.append(checkflag[idx - 1] + " = False")

		childsdict |= {nodeidx:childslist}

		if text:
			text.reverse()

			aa = [i for i, s in enumerate(text) if s.startswith("<span class='dialog")]
			y = aa[1:] + [len(text)]
			z = [text[i:j] for i, j in zip(aa, y)]


			xfh44ew = []
			for af in reversed(z):
				xfh44ew.append(speaker + ", ".join([str(ad) for ad in af]) + context + ", ".join([str(ad) for ad in tags]) + "<span class='checkflag'>" + ", ".join([str(ad) for ad in checkflag2]) + "</span>" + "<span class='setflag'>" +  ", ".join([str(ad) for ad in setflag2]) + "</span>" +  rolls + approval + "<br>" + endnode)
			strings |= {nodeidx:xfh44ew}
		else:
			strings |= {nodeidx:[speaker + "[" + constructor + "] " + poplevel + jump + context + rollres + ", ".join([str(ad) for ad in tags]) + "<span class='checkflag'>" + ", ".join([str(ad) for ad in checkflag2]) + "</span>" + "<span class='setflag'>" +  ", ".join([str(ad) for ad in setflag2]) + "</span>" + rolls + approval + "<br>" + endnode]}



	test = []
	listoflists = []
	uuidlist = []
	uuidlistcheck = []
	uuidlist1 = []
	final = []

	parsenode()

	hhh = 0
	lst = [int(e[1:-1]) for e in root]
	lst.sort()
	root = ['('+str(e)+')' for e in lst]
	for ff in root:
		textt(ff,hhh)



	newpath = "Dialogs\\" + os.path.relpath(os.path.split(dialog)[0], dialogpath) + "\\" 

	

	os.makedirs(newpath, exist_ok=True)


	print(str(dind) + "/" + str(len(dialogfiles)), end='\r')

	#print(os.path.split(dialog)[-1])

	level = str(newpath + os.path.split(dialog)[-1][:-4]).split("\\")
	level = len(level) - 1 



	htmlfile = newpath + os.path.split(dialog)[-1][:-4] + ".html"

	if os.path.exists(htmlfile):
		os.remove(htmlfile)

	writehtml = open(htmlfile, "a", encoding="utf-8")

	writehtml.write("<!DOCTYPE html><html><head><script src=\"" + "../"*level + "styles/" + "jquery-3.7.1.min.js\"></script> <link rel=\"stylesheet\" href=\"" + "../"*level + "styles/" + "styles.css\" /> </head> <body><script type='text/javascript' src='" + "../"*level + "styles/" + "CollapsibleLists.compressed.js'></script><script src='" + "../"*level + "styles/" + "scripts.js'></script><div class='dialog'> <ul class='dialogList collapsibleList'> <span class='ecall' onclick='ecall()'>Expand all</span> | <span class='shflags' onclick='shflags()'>Hide all flags</span> | <span class='shcontext' onclick='shcontext()'>Hide context</span> | <span class='shtags' onclick='shtags()'>Hide tags</span> | <span class='shcheckflag' onclick='shcheckflag()'>Hide checkflag</span> | <span class='shsetflag' onclick='shsetflag()'>Hide setflag</span> | <span class='shroll' onclick='shroll()'>Hide roll</span> | <span class='shapprov' onclick='shapprov()'>Hide approv</span> | <span class='shrules' onclick='shrules()'>Hide ruletag</span> | <span class='shid' onclick='shid()'>Hide node id</span><br>")

	if synopsis != "":
		writehtml.write("<span class='synopsis'>" + synopsis.replace("\r\n", "<br>") + "</span><br><br>")


	listify()


	#if howtotrigger != "":
	#	writehtml.write("<br><br><br><br><span class='howtotrigger'>" + howtotrigger.replace("\r\n", "<br>") + "</span>")

	writehtml.write("</body></html>")


	writehtml.close()

db.close()
print("\n")
print("Done")