import os
import xml.etree.ElementTree as ET
import glob
import sqlite3
import sys


enxml = sys.argv[1]
unppath = sys.argv[2]

paths = [unppath + "\\Public\\Gustav\\Flags\\", 
		unppath + "\\Public\\Gustav\\Tags\\",
		unppath + "\\Public\\Shared\\Flags\\", 
		unppath + "\\Public\\Shared\\Tags\\",
		unppath + "\\Public\\GustavDev\\Flags\\", 
		unppath + "\\Public\\GustavDev\\Tags\\",
		unppath + "\\Public\\SharedDev\\Flags\\", 
		unppath + "\\Public\\SharedDev\\Tags\\"]

questflags = unppath + "\\Mods\\GustavDev\\Story\\Journal\\quest_prototypes.lsx"

reactionspath = [unppath + "\\Public\\Gustav\\ApprovalRatings\\Reactions\\",
				unppath + "\\Public\\GustavDev\\ApprovalRatings\\Reactions\\"]

difficulty = [unppath + "\\Public\\Shared\\DifficultyClasses\\DifficultyClasses.lsx",
				unppath + "\\Public\\SharedDev\\DifficultyClasses\\DifficultyClasses.lsx"]


if os.path.exists("bg3.db"):
	os.remove("bg3.db")

db_file = "bg3.db"
db = sqlite3.connect(db_file)
db_cursor = db.cursor()
db_cursor.execute('CREATE TABLE tagsflags (uuid text, name text, description text)')
db_cursor.execute('CREATE INDEX indx ON tagsflags("uuid")')

print("Parsing translation")
entree = ET.parse(enxml)
entreeroot = entree.getroot()

for x in range(len(entreeroot)):
	handle = entreeroot[x].attrib['contentuid']
	translate = entreeroot[x].text
	db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,null)', (handle, translate))



print("Parsing tags and flags")
for path in paths:
	filenames = glob.glob(path + "/**/*.lsx", recursive=True)
	for x in filenames:
		tree = ET.parse(x)
		root = tree.getroot()
		name = ""
		desc = ""
		for att in root[1][0]:
			ids = att.get("id")
			if ids == "Name":
				name = att.get("value")
			if ids == "Description":
				desc = att.get("value")

		fname = os.path.split(x)[-1][:-4]
		
		if name == "":
			name = None
		if desc == "":
			desc = None

		db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,?)', (fname, name, desc))



print("Parsing questflags")
tree = ET.parse(questflags)
root = tree.getroot()

for x in range(len(root[1][0][0])):
	if root[1][0][0][x].attrib.get('id') == "Quest":
		for s in range(len(root[1][0][0][x][-1])):
			if root[1][0][0][x][-1][s].attrib.get('id') == "QuestStep":
				uuid = ""
				name = ""
				desc = ""
				for att in root[1][0][0][x][-1][s]:
					ids = att.get("id")
					if ids == "DialogFlagGUID":
						uuid = att.get("value")
					if ids == "ID":
						name = att.get("value")
					if ids == "Description":
						desc = att.get("handle")

				for dbuuid,dbname,dbdesc in db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (desc,)):
					db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,?)', (uuid, name, dbname))




print("Parsing reaction")

npc = {'2bb39cf2-4649-4238-8d0c-44f62b5a3dfd': 'Shadowheart', 
			'35c3caad-5543-4593-be75-e7deba30f062': 'Gale', 
			'3780c689-d903-41c2-bf64-1e6ec6a8e1e5': 'Astarion', 
			'efc9d114-0296-4a30-b701-365fc07d44fb': 'Wyll', 
			'fb3bc4c3-49eb-4944-b714-d0cb357bb635': 'Lae\'zel',
			'b8b4a974-b045-45f6-9516-b457b8773abd' : 'Karlach',
			'c1f137c7-a17c-47b0-826a-12e44a8ec45c' : 'Jaheira',
			'eae09670-869d-4b70-b605-33af4ee80b34' : 'Minthara',
			'e1b629bc-7340-4fe6-81a4-834a838ff5c5' : 'Minsc',
			'a36281c5-adcd-4d6e-8e5a-b5650b8f17eb' : 'Halsin',
			'38357c93-b437-4f03-88d0-a67bd4c0e3e9' : 'Alfira',
			'5af0f42c-9b32-4c3c-b108-46c44196081b' : 'The Dark Urge',
			'a4b56492-d5ac-4a84-8e45-5437cd9da7f3' : 'Custom' }

for rea in reactionspath:
	filenames = glob.glob(rea + "/**/*.lsx", recursive=True)
	for x in filenames:
		tree = ET.parse(x)
		root = tree.getroot()
		react = []

		ifexist = root[1][0][0][0].find('children')
		if ifexist is None:
			continue
		

		for a in range(len(root[1][0][0][0][2][0][0])):
			uuid = ""
			value = ""
			for att in root[1][0][0][0][2][0][0][a]:
				ids = att.get("id")
				if ids == "id":
					uuid = att.get("value")
				if ids == "value":
					value = att.get("value")

			if value != "0":
				react.append(npc[uuid] + " " + value)

		fname = os.path.split(x)[-1][:-4]

		if not react:
			react = None
			db_cursor.execute('INSERT INTO tagsflags VALUES (?,null, null)', (fname,))
		else:
			db_cursor.execute('INSERT INTO tagsflags VALUES (?,?, null)', (fname, str(react)))



print("Parsing difficulties")

for diffi in difficulty:
	tree = ET.parse(diffi)
	root = tree.getroot()


	for x in range(len(root[1][0][0])):
		uuid = ""
		name = ""
		diff = ""
		for att in root[1][0][0][x]:
			ids = att.get("id")
			if ids == "UUID":
				uuid  = att.get("value")
			if ids == "Name":
				name = att.get("value")
			if ids == "Difficulties":
				diff = att.get("value")


		db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,?)', (uuid, diff, name))



print("Parsing names")
filenames = glob.glob(unppath + "/**/*.lsx", recursive=True)

testdictfound = {'e0d1ff71-04a8-4340-ae64-9684d846eb83' : 'Player'}
testdictnotfound = {}

for x in filenames:
	if ("\\Items\\_merged.lsx" in x or "\\Characters\\_merged.lsx" in x or "\\RootTemplates\\" in x) and "\\Assets\\" not in x:
		#print(x)
		tree = ET.parse(x)
		root = tree.getroot()

		ifexist = root[1][0].find('children')
		if ifexist is None:
			continue

		for a in range(len(root[1][0][0])):
			if root[1][0][0][a].attrib.get('id') == "GameObjects":
				MapKey = ""
				DisplayName = ""
				TemplateName = ""
				ParentTemplateId = ""
				for att in root[1][0][0][a]:
					ids = att.get("id")
					if ids == "MapKey":
						MapKey = att.get("value")
					if ids == "DisplayName":
						DisplayName = att.get("handle")
					if ids == "TemplateName":
						TemplateName = att.get("value")
					if ids == "ParentTemplateId":
						ParentTemplateId = att.get("value")

				if DisplayName.startswith("h"):
					DisplayNamename = db_cursor.execute('SELECT * FROM tagsflags WHERE uuid = ?', (DisplayName,)).fetchone()
					if DisplayNamename != None:
						testdictfound |= {MapKey:DisplayNamename[1]}
				#if DisplayName == "" or DisplayName == "ls::TranslatedStringRepository::s_HandleUnknown":
				elif DisplayName == "":
					if TemplateName != "":
						testdictnotfound |= {MapKey:TemplateName}
					else:
						testdictnotfound |= {MapKey:ParentTemplateId}
	elif "\\Origins\\Origins.lsx" in x:
		tree = ET.parse(x)
		root = tree.getroot()
		for o in range(len(root[1][0][0])):
			if root[1][0][0][o].attrib.get('id') == "Origin":
				UUID = ""
				GlobalTemplate = ""
				for att in root[1][0][0][o]:
					ids = att.get("id")
					if ids == "UUID":
						UUID = att.get("value")
					if ids == "GlobalTemplate":
						GlobalTemplate = att.get("value")

				testdictnotfound |= {UUID:GlobalTemplate}

	elif "\\Voice\\SpeakerGroups.lsx" in x:
		tree = ET.parse(x)
		root = tree.getroot()
		for sg in range(len(root[1][0][0])):
			if root[1][0][0][sg].attrib.get('id') == "SpeakerGroup":
				UUID = ""
				name = ""
				desc = ""
				for att in root[1][0][0][sg]:
					ids = att.get("id")
					if ids == "UUID":
						UUID = att.get("value")
					if ids == "Name":
						name = att.get("value")
					if ids == "Description":
						desc = att.get("value")
				if name == "GROUP_Players":
					continue
				db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,?)', (UUID, name, desc))

				'''
	elif "\\Voice\\SpeakerGroups.lsx" in x:
		tree = ET.parse(x)
		root = tree.getroot()
		for sg in range(len(root[1][0][0])):
			if root[1][0][0][sg].attrib.get('id') == "SpeakerGroup":
				UUID = ""
				OverwriteSpeakerUuid = ""
				for att in root[1][0][0][sg]:
					ids = att.get("id")
					if ids == "UUID":
						UUID = att.get("value")
					if ids == "OverwriteSpeakerUuid":
						OverwriteSpeakerUuid = att.get("value")
				if OverwriteSpeakerUuid != "":
					testdictnotfound |= {UUID:OverwriteSpeakerUuid}
'''

for key, value in testdictnotfound.items():
	if value in testdictfound:
		testdictfound |= {key:testdictfound[value]}
	else:
		if value in testdictnotfound:
			if testdictnotfound[value] in testdictfound:
				testdictfound |= {key:testdictfound[testdictnotfound[value]]}
			else:
				if testdictnotfound[value] in testdictnotfound:
					if testdictnotfound[testdictnotfound[value]] in testdictfound:
						testdictfound |= {key:testdictfound[testdictnotfound[testdictnotfound[value]]]}


for key, value in testdictfound.items():
	db_cursor.execute('INSERT INTO tagsflags VALUES (?,?,null)', (key, value))


print("Done")

db.commit()
db.close()