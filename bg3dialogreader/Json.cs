using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bg3dialogreader
{
    internal class Json
    {
        public class Rootobject
        {
            public Save save { get; set; }
        }

        public class Save
        {
            public Header header { get; set; }
            public Regions regions { get; set; }
        }

        public class Header
        {
            public string version { get; set; }
        }

        public class Regions
        {
            public Dialog dialog { get; set; }
            public Editordata1 editorData { get; set; }
        }

        public class Dialog
        {
            public Defaultaddressedspeaker[] DefaultAddressedSpeakers { get; set; }
            public Timelineid TimelineId { get; set; }
            public UUID UUID { get; set; }
            public Category category { get; set; }
            public Node[] nodes { get; set; }
            public Speakerlist[] speakerlist { get; set; }
        }

        public class Timelineid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class UUID
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Category
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Defaultaddressedspeaker
        {
            public Object[] Object { get; set; }
        }

        public class Object
        {
            public Mapkey MapKey { get; set; }
            public Mapvalue MapValue { get; set; }
        }

        public class Mapkey
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Mapvalue
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Node
        {
            public Rootnode[] RootNodes { get; set; }
            public Node1[] node { get; set; }
        }

        public class Rootnode
        {
            public Rootnodes RootNodes { get; set; }
        }

        public class Rootnodes
        {
            public string type { get; set; }
            public string value { get; set; }
        }
        public class PopLevel
        {
            public string type { get; set; }
            public int value { get; set; }
        }
        public class Node1
        {
            public Gamedata[] GameData { get; set; }
            public Sourcenode SourceNode { get; set; }
            public Tag[] Tags { get; set; }
            public UUID1 UUID { get; set; }
            public Checkflag[] checkflags { get; set; }
            public Child[] children { get; set; }
            public Constructor constructor { get; set; }
            public Editordata[] editorData { get; set; }
            public Setflag[] setflags { get; set; }
            public Speaker speaker { get; set; }
            public Waittime waittime { get; set; }
            public DifficultyMod DifficultyMod { get; set; }
            public LevelOverride LevelOverride { get; set; }
            public PersuasionTargetSpeakerIndex PersuasionTargetSpeakerIndex { get; set; }
            public StatName StatName { get; set; }
            public StatsAttribute StatsAttribute { get; set; }
            public PopLevel PopLevel { get; set; }
            public Showonce ShowOnce { get; set; }
            public Taggedtext[] TaggedTexts { get; set; }
            public Validatedflag[] ValidatedFlags { get; set; }
            public Jumptarget jumptarget { get; set; }
            public Jumptargetpoint jumptargetpoint { get; set; }
            public Endnode endnode { get; set; }
            public Approvalratingid ApprovalRatingID { get; set; }
            public Success Success { get; set; }
            public Groupid GroupID { get; set; }
            public Groupindex GroupIndex { get; set; }
            public Root Root { get; set; }
            public Ability Ability { get; set; }
            public Advantage Advantage { get; set; }
            public Difficultyclassid DifficultyClassID { get; set; }
            public Excludecompanionsoptionalbonuses ExcludeCompanionsOptionalBonuses { get; set; }
            public Excludespeakeroptionalbonuses ExcludeSpeakerOptionalBonuses { get; set; }
            public Rolltargetspeaker RollTargetSpeaker { get; set; }
            public Rolltype RollType { get; set; }
            public Skill Skill { get; set; }
            public Transitionmode transitionmode { get; set; }
            public Optional optional { get; set; }
        }

        public class Sourcenode
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class UUID1
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Constructor
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Speaker
        {
            public string type { get; set; }
            public int value { get; set; }
        }
        public class Waittime
        {
            public string type { get; set; }
            public float value { get; set; }
        }
        public class DifficultyMod
        {
            public int type { get; set; }
            public int value { get; set; }
        }
        public class LevelOverride
        {
            public int type { get; set; }
            public int value { get; set; }
        }
        public class PersuasionTargetSpeakerIndex
        {
            public int type { get; set; }
            public int value { get; set; }
        }
        public class StatName
        {
            public int type { get; set; }
            public string value { get; set; }
        }
        public class StatsAttribute
        {
            public int type { get; set; }
            public string value { get; set; }
        }

        public class Showonce
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Jumptarget
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Jumptargetpoint
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Endnode
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Approvalratingid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Success
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Groupid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Groupindex
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Ability
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Advantage
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Difficultyclassid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Excludecompanionsoptionalbonuses
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Excludespeakeroptionalbonuses
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Rolltargetspeaker
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Rolltype
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Skill
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Transitionmode
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Optional
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Gamedata
        {
            public Aipersonality[] AiPersonalities { get; set; }
            public Musicinstrumentsound[] MusicInstrumentSounds { get; set; }
            public Originsound[] OriginSound { get; set; }
        }

        public class Aipersonality
        {
            public Aipersonality1[] AiPersonality { get; set; }
        }

        public class Aipersonality1
        {
            public Aipersonality2 AiPersonality { get; set; }
        }

        public class Aipersonality2
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Musicinstrumentsound
        {
        }

        public class Originsound
        {
        }

        public class Tag
        {
            [JsonProperty("Tag")]
            public Tag1[] Tagg { get; set; }
        }

        public class Tag1
        {
            public Tag2 Tag { get; set; }
        }

        public class Tag2
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Checkflag
        {
            public Flaggroup[] flaggroup { get; set; }
        }

        public class Flaggroup
        {
            public Flag[] flag { get; set; }
            public Type type { get; set; }
        }

        public class Type
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Flag
        {
            public UUID2 UUID { get; set; }
            public Value value { get; set; }
            public Paramval paramval { get; set; }
            public Name name { get; set; }
        }

        public class UUID2
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Value
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Paramval
        {
            public string type { get; set; }
            public int value { get; set; }
        }
        public class Name
        {
            public int type { get; set; }
            public string value { get; set; }
        }

        public class Child
        {
            public Child1[] child { get; set; }
        }

        public class Child1
        {
            public UUID3 UUID { get; set; }
        }

        public class UUID3
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Editordata
        {
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public Key key { get; set; }
            public Val val { get; set; }
        }

        public class Key
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Val
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Setflag
        {
            public Flaggroup1[] flaggroup { get; set; }
        }

        public class Flaggroup1
        {
            public Flag1[] flag { get; set; }
            public Type1 type { get; set; }
        }

        public class Type1
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Flag1
        {
            public UUID4 UUID { get; set; }
            public Paramval1 paramval { get; set; }
            public Value1 value { get; set; }
            public Name1 name { get; set; }
        }

        public class UUID4
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Paramval1
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Value1
        {
            public string type { get; set; }
            public bool value { get; set; }
        }
        public class Name1
        {
            public int type { get; set; }
            public string value { get; set; }
        }

        public class Taggedtext
        {
            public Taggedtext1[] TaggedText { get; set; }
        }

        public class Taggedtext1
        {
            public Hastagrule HasTagRule { get; set; }
            public Rulegroup[] RuleGroup { get; set; }
            public Tagtext[] TagTexts { get; set; }
        }

        public class Hastagrule
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Rulegroup
        {
            public Rule[] Rules { get; set; }
            public Tagcombineop TagCombineOp { get; set; }
        }

        public class Tagcombineop
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Rule
        {
            [JsonProperty("Rule")]
            public Rule1[] Rulee { get; set; }
        }

        public class Rule1
        {
            public Haschildrules HasChildRules { get; set; }
            public Tagcombineop1 TagCombineOp { get; set; }
            public Tag3[] Tags { get; set; }
            public Speaker1 speaker { get; set; }
        }

        public class Haschildrules
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Tagcombineop1
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Speaker1
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Tag3
        {
            public Tag4[] Tag { get; set; }
        }

        public class Tag4
        {
            public Object1 Object { get; set; }
        }

        public class Object1
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Tagtext
        {
            public Tagtext1[] TagText { get; set; }
        }

        public class Tagtext1
        {
            public Lineid LineId { get; set; }
            public Tagtext2 TagText { get; set; }
            public Stub stub { get; set; }
        }

        public class Lineid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Tagtext2
        {
            public string handle { get; set; }
            public string value { get; set; }
            public string type { get; set; }
            public int version { get; set; }
        }

        public class Stub
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Validatedflag
        {
            public Validatedhasvalue ValidatedHasValue { get; set; }
        }

        public class Validatedhasvalue
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Speakerlist
        {
            public Speaker2[] speaker { get; set; }
        }

        public class Speaker2
        {
            public Speakermappingid SpeakerMappingId { get; set; }
            public Index index { get; set; }
            public List list { get; set; }
            public Ispeanutspeaker IsPeanutSpeaker { get; set; }
        }

        public class Speakermappingid
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Index
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class List
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Ispeanutspeaker
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Editordata1
        {
            public Howtotrigger HowToTrigger { get; set; }
            public Defaultattitude[] defaultAttitudes { get; set; }
            public Defaultemotion[] defaultEmotions { get; set; }
            public Isimportantforstaging[] isImportantForStagings { get; set; }
            public Ispeanut[] isPeanuts { get; set; }
            public Needlayout needLayout { get; set; }
            public Nextnodeid nextNodeId { get; set; }
            public Speakerslotdescription[] speakerSlotDescription { get; set; }
            public Synopsis synopsis { get; set; }
        }

        public class Howtotrigger
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Needlayout
        {
            public string type { get; set; }
            public bool value { get; set; }
        }

        public class Nextnodeid
        {
            public string type { get; set; }
            public int value { get; set; }
        }

        public class Synopsis
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Defaultattitude
        {
            public Datum1[] data { get; set; }
        }

        public class Datum1
        {
            public Key1 key { get; set; }
            public Val1 val { get; set; }
        }

        public class Key1
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Val1
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Defaultemotion
        {
            public Datum2[] data { get; set; }
        }

        public class Datum2
        {
            public Key2 key { get; set; }
            public Val2 val { get; set; }
        }

        public class Key2
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Val2
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Isimportantforstaging
        {
        }

        public class Ispeanut
        {
            public Datum3[] data { get; set; }
        }

        public class Datum3
        {
            public Key3 key { get; set; }
            public Val3 val { get; set; }
        }

        public class Key3
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Val3
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Speakerslotdescription
        {
        }
    }
}
