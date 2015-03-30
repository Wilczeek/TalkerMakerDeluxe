﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TalkerMakerDeluxe
{
    public class XMLHandler
    {
        public static TalkerMakerProject LoadXML(string xml_file)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TalkerMakerProject));
            return xmlSerializer.Deserialize(new StreamReader(xml_file)) as TalkerMakerProject;
        }

        public static void SaveXML(TalkerMakerProject talkerMakerProject, string xml_file)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TalkerMakerProject));
            StreamWriter streamWriter = new StreamWriter(xml_file, false, System.Text.Encoding.UTF8);
            xmlSerializer.Serialize(streamWriter, talkerMakerProject);
            streamWriter.Close();
        }
    }

    [Serializable]
    public class Actor
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();

        [XmlAttribute("ID")]
        public int ID;
    }

    [Serializable]
    public class Assets
    {
        //
        // Fields
        //
        [XmlArray("Conversations"), XmlArrayItem("Conversation")]
        public List<Conversation> Conversations = new List<Conversation>();

        [XmlArray("UserVariables"), XmlArrayItem("UserVariable")]
        public List<UserVariable> UserVariables = new List<UserVariable>();

        [XmlArray("Locations"), XmlArrayItem("Location")]
        public List<Location> Locations = new List<Location>();

        [XmlArray("Actors"), XmlArrayItem("Actor")]
        public List<Actor> Actors = new List<Actor>();

        [XmlArray("Items"), XmlArrayItem("Item")]
        public List<Item> Items = new List<Item>();
    }

    [XmlRoot("ChatMapperProject")]
    public class TalkerMakerProject
    {
        //
        // Fields
        //
        [XmlAttribute("EmphasisStyle3")]
        public string EmphasisStyle3;

        [XmlAttribute("EmphasisColor4Label")]
        public string EmphasisColor4Label = string.Empty;

        [XmlAttribute("EmphasisColor3Label")]
        public string EmphasisColor3Label = string.Empty;

        [XmlAttribute("EmphasisColor3")]
        public string EmphasisColor3;

        [XmlAttribute("EmphasisColor4")]
        public string EmphasisColor4;

        public string UserScript;

        public Assets Assets;

        [XmlAttribute("EmphasisStyle4")]
        public string EmphasisStyle4;

        public string Description;

        [XmlAttribute("Author")]
        public string Author;

        [XmlAttribute("EmphasisColor1Label")]
        public string EmphasisColor1Label = string.Empty;

        [XmlAttribute("Title")]
        public string Title;

        [XmlAttribute("Version")]
        public string Version;

        [XmlAttribute("EmphasisColor1")]
        public string EmphasisColor1;

        [XmlAttribute("EmphasisColor2")]
        public string EmphasisColor2;

        [XmlAttribute("EmphasisStyle2")]
        public string EmphasisStyle2;

        [XmlAttribute("EmphasisStyle1")]
        public string EmphasisStyle1;

        [XmlAttribute("EmphasisColor2Label")]
        public string EmphasisColor2Label = string.Empty;

        //
        // Methods
        //
        //public DialogueDatabase ToDialogueDatabase()
        //{
        //    return ChatMapperToDialogueDatabase.ConvertToDialogueDatabase(this);
        //}
    }

    public class Conversation
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();

        [XmlArray("DialogEntries"), XmlArrayItem("DialogEntry")]
        public List<DialogEntry> DialogEntries = new List<DialogEntry>();

        [XmlAttribute("LockedMode")]
        public string LockedMode;

        [XmlAttribute("ID")]
        public int ID;

        [XmlAttribute("NodeColor")]
        public string NodeColor;
    }

    public class DialogEntry
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();

        [XmlAttribute("ConditionPriority")]
        public string ConditionPriority;

        [XmlArray("OutgoingLinks"), XmlArrayItem("Link")]
        public List<Link> OutgoingLinks = new List<Link>();

        public string UserScript;

        public string ConditionsString;

        [XmlAttribute("FalseCondtionAction")]
        public string FalseCondtionAction;

        [XmlAttribute("IsRoot")]
        public bool IsRoot;

        [XmlAttribute("ID")]
        public int ID;

        [XmlAttribute("IsGroup")]
        public bool IsGroup;

        [XmlAttribute("DelaySimStatus")]
        public bool DelaySimStatus;

        [XmlAttribute("NodeColor")]
        public string NodeColor;
    }

    public class Field
    {
        //
        // Fields
        //
        public string Title;

        public string Value;

        [XmlAttribute("Hint")]
        public string Hint;

        [XmlAttribute("Type")]
        public string Type;
    }

    public class Item
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();

        [XmlAttribute("ID")]
        public int ID;
    }

    public class Link
    {
        //
        // Fields
        //
        [XmlAttribute("OriginDialogID")]
        public int OriginDialogID;

        [XmlAttribute("DestinationDialogID")]
        public int DestinationDialogID;

        [XmlAttribute("IsConnector")]
        public bool IsConnector;

        [XmlAttribute("ConversationID")]
        public int ConversationID;

        [XmlAttribute("OriginConvoID")]
        public int OriginConvoID;

        [XmlAttribute("DestinationConvoID")]
        public int DestinationConvoID;
    }

    public class Location
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();

        [XmlAttribute("ID")]
        public int ID;
    }

    public class UserVariable
    {
        //
        // Fields
        //
        [XmlArray("Fields"), XmlArrayItem("Field")]
        public List<Field> Fields = new List<Field>();
    }
}
