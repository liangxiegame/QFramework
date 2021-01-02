using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace UI.Xml
{
    public class XmlSchemaProcessor
    {
        string baseXSDPath = "";
        XElement schemaElement;
        XNamespace ns;

        Dictionary<CustomXmlAttribute.eAttributeGroup, Dictionary<string, CustomXmlAttribute>> customAttributesToAdd =
            new Dictionary<CustomXmlAttribute.eAttributeGroup, Dictionary<string, CustomXmlAttribute>>();

        Dictionary<string, Dictionary<string, ElementTagHandler>> customElementTagHandlersToAdd =
            new Dictionary<string, Dictionary<string, ElementTagHandler>>();

        /// <summary>
        /// Entry point for this process
        /// Called whenever Unity finishes loading scripts and compiling them
        /// </summary>
        [DidReloadScripts(1)]
        public static void ProcessXmlSchema()
        {
            ProcessXmlSchema(false);
        }

        public static void ProcessXmlSchema(bool force)
        {
            var schemaAssetPath = string.Format("{0}/{1}", Application.dataPath,
                AssetDatabase.GetAssetPath(XmlLayoutUtilities.XmlLayoutConfiguration.BaseXSDFile)
                    .Substring("Assets/".Length));
            var processor = new XmlSchemaProcessor(schemaAssetPath);

            processor.ProcessCustomAttributes();
            processor.ProcessCustomTags();

            if (force || processor.HasChanges())
            {
                processor.Output(string.Format("{0}/{1}", Application.dataPath,
                    AssetDatabase.GetAssetPath(XmlLayoutUtilities.XmlLayoutConfiguration.XSDFile)
                        .Substring("Assets/".Length)));
            }

            processor = null;
        }

        public XmlSchemaProcessor(string schemaPath)
        {
            baseXSDPath = schemaPath;

            var xDoc = XDocument.Load(schemaPath);
            ns = XNamespace.Get(@"http://www.w3.org/2001/XMLSchema");

            schemaElement = xDoc.Elements().First();
        }

        public bool HasChanges()
        {
            return customAttributesToAdd.Any() || customElementTagHandlersToAdd.Any();
        }

        public void Output(string outputPath)
        {
            if (!XmlLayoutUtilities.XmlLayoutConfiguration.SuppressXSDUpdateMessage)
                Debug.Log("[XmlLayout] Updating XSD File.");

            var xmlTextReader = new XmlTextReader(baseXSDPath);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += ValidationCallback;
            schemaSet.Add(XmlSchema.Read(xmlTextReader, ValidationCallback));
            schemaSet.Compile();

            XmlSchema schema = null;
            foreach (XmlSchema s in schemaSet.Schemas())
            {
                schema = s;
            }

            schema.Namespaces.Add(String.Empty, String.Empty);
            schema.Namespaces.Add("xmlLayout", @"http://www.w3schools.com");

            if (customAttributesToAdd.Any())
            {
                foreach (var group in customAttributesToAdd)
                {
                    XmlSchemaAttributeGroup schemaAttributeGroup = null;
                    foreach (DictionaryEntry attributeGroup in schema.AttributeGroups)
                    {
                        if (attributeGroup.Key.ToString().EndsWith(GetAttributeGroupName(group.Key),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            schemaAttributeGroup = (XmlSchemaAttributeGroup) attributeGroup.Value;
                        }
                    }

                    foreach (var attribute in group.Value)
                    {
                        if (attribute.Value.ValueDataType.Contains(','))
                        {
                            var acceptableValues =
                                attribute.Value.ValueDataType.Split(',').Select(s => s.Trim()).ToList();

                            var simpleType = new XmlSchemaSimpleType();
                            var restriction = new XmlSchemaSimpleTypeRestriction()
                                {BaseTypeName = new XmlQualifiedName("xs:token")};
                            simpleType.Content = restriction;

                            foreach (var value in acceptableValues)
                            {
                                restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = value});
                            }

                            schemaAttributeGroup.Attributes.Add(new XmlSchemaAttribute
                                {Name = attribute.Key, SchemaType = simpleType});
                        }
                        else
                        {
                            schemaAttributeGroup.Attributes.Add(new XmlSchemaAttribute()
                            {
                                Name = attribute.Key,
                                SchemaTypeName = new XmlQualifiedName(attribute.Value.ValueDataType)
                            });
                        }

                        /*schemaAttributeGroup.Attributes.Add(
                            new XmlSchemaAttribute() 
                            { 
                                Name = attribute.Key, 
                                SchemaTypeName = new XmlQualifiedName(attribute.Value.ValueDataType)                                
                            });*/
                    }
                }
            }

            if (customElementTagHandlersToAdd.Any())
            {
                var defaultsElement =
                    (XmlSchemaElement) schema.Elements[new XmlQualifiedName("Defaults", @"http://www.w3schools.com")];
                var defaultsComplexType = (XmlSchemaComplexType) defaultsElement.SchemaType;
                var defaultsComplexContent =
                    (XmlSchemaComplexContentExtension) defaultsComplexType.ContentModel.Content;
                var defaultsChoice = (XmlSchemaChoice) defaultsComplexContent.Particle;

                foreach (var group in customElementTagHandlersToAdd)
                {
                    XmlSchemaGroup schemaElementGroup = null;
                    foreach (DictionaryEntry elementGroup in schema.Groups)
                    {
                        if (elementGroup.Key.ToString().EndsWith(group.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            schemaElementGroup = (XmlSchemaGroup) elementGroup.Value;
                        }
                    }

                    // We need to create the group
                    if (schemaElementGroup == null)
                    {
                        schemaElementGroup = new XmlSchemaGroup() {Name = group.Key};
                        schemaElementGroup.Particle = new XmlSchemaChoice() { };
                        schema.Items.Add(schemaElementGroup);
                    }

                    foreach (var tag in group.Value)
                    {
                        var element = new XmlSchemaElement() {Name = tag.Key};
                        schema.Items.Add(element);

                        var complexType = new XmlSchemaComplexType();
                        var complexContent = new XmlSchemaComplexContent() {IsMixed = true};
                        var choice = new XmlSchemaChoice() {MinOccurs = 0, MaxOccursString = "unbounded"};
                        var extension = new XmlSchemaComplexContentExtension()
                        {
                            Particle = choice,
                            BaseTypeName = new XmlQualifiedName(tag.Value.extension, @"http://www.w3schools.com")
                        };

                        element.SchemaType = complexType;
                        complexType.ContentModel = complexContent;
                        complexContent.Content = extension;

                        choice.Items.Add(new XmlSchemaGroupRef
                            {RefName = new XmlQualifiedName(tag.Value.elementChildType, @"http://www.w3schools.com")});

                        // add attributes
                        var attributes = tag.Value.attributes;
                        if (attributes.Any())
                        {
                            foreach (var attribute in attributes)
                            {
                                // If the value contains commas, then it is a comma-seperated list and should be converted into an enum
                                if (attribute.Value.Contains(','))
                                {
                                    var acceptableValues = attribute.Value.Split(',').Select(s => s.Trim()).ToList();

                                    var simpleType = new XmlSchemaSimpleType();
                                    var restriction = new XmlSchemaSimpleTypeRestriction()
                                        {BaseTypeName = new XmlQualifiedName("xs:token")};
                                    simpleType.Content = restriction;

                                    foreach (var value in acceptableValues)
                                    {
                                        restriction.Facets.Add(new XmlSchemaEnumerationFacet {Value = value});
                                    }

                                    extension.Attributes.Add(new XmlSchemaAttribute
                                        {Name = attribute.Key, SchemaType = simpleType});
                                }
                                else
                                {
                                    extension.Attributes.Add(new XmlSchemaAttribute()
                                        {Name = attribute.Key, SchemaTypeName = new XmlQualifiedName(attribute.Value)});
                                }
                            }
                        }

                        // add the reference to the default group
                        var refElement = new XmlSchemaElement()
                            {RefName = new XmlQualifiedName(tag.Key, @"http://www.w3schools.com")};
                        schemaElementGroup.Particle.Items.Add(refElement);


                        if (!group.Key.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            var item = new XmlSchemaElement
                                {RefName = new XmlQualifiedName(tag.Key, @"http://www.w3schools.com")};
                            defaultsChoice.Items.Add(item);
                        }
                    }
                }
            }

            // output to file            
            FileStream file = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite);
            XmlTextWriter xwriter = new XmlTextWriter(file, new UTF8Encoding());
            xwriter.Formatting = Formatting.Indented;

            schema.Write(xwriter);
        }

        public void ProcessCustomAttributes()
        {
            var customAttributes = XmlLayoutUtilities.GetGroupedCustomAttributeNames();

            foreach (var group in customAttributes)
            {
                var existingAttributes = GetAttributeGroup(group.Key);

                foreach (var attribute in group.Value)
                {
                    if (!existingAttributes.Contains(attribute, StringComparer.OrdinalIgnoreCase))
                    {
                        if (!customAttributesToAdd.ContainsKey(group.Key))
                            customAttributesToAdd.Add(group.Key, new Dictionary<string, CustomXmlAttribute>());

                        customAttributesToAdd[group.Key].Add(Char.ToLower(attribute[0]) + attribute.Substring(1),
                            XmlLayoutUtilities.GetCustomAttribute(attribute));
                    }
                }
            }
        }

        public void ProcessCustomTags()
        {
            var tags = XmlLayoutUtilities.GetXmlTagHandlerNames();
            var groupedTags = new Dictionary<string, Dictionary<string, ElementTagHandler>>();
            foreach (var tag in tags)
            {
                var tagHandler = XmlLayoutUtilities.GetXmlTagHandler(tag);

                if (!tagHandler.isCustomElement) continue;

                if (!groupedTags.ContainsKey(tagHandler.elementGroup))
                    groupedTags.Add(tagHandler.elementGroup, new Dictionary<string, ElementTagHandler>());

                groupedTags[tagHandler.elementGroup].Add(tag, tagHandler);
            }

            foreach (var group in groupedTags)
            {
                var existingTags = GetElementGroup(group.Key);

                foreach (var tag in group.Value)
                {
                    if (!existingTags.Contains(tag.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        if (!customElementTagHandlersToAdd.ContainsKey(group.Key))
                            customElementTagHandlersToAdd.Add(group.Key, new Dictionary<string, ElementTagHandler>());

                        customElementTagHandlersToAdd[group.Key].Add(tag.Key, tag.Value);
                    }
                }
            }
        }

        protected List<string> GetAttributeGroup(CustomXmlAttribute.eAttributeGroup attributeGroup)
        {
            var attributeGroupName = GetAttributeGroupName(attributeGroup);

            var ag = schemaElement.Elements(ns + "attributeGroup")
                .First(s =>
                {
                    var name = s.Attribute("name");
                    if (name != null)
                    {
                        return name.Value.Equals(attributeGroupName, StringComparison.OrdinalIgnoreCase);
                    }

                    return false;
                });

            return ag.Elements()
                .Where(e => e.Name.LocalName == "attribute")
                .Select(e => e.Attribute("name").Value)
                .ToList();
        }

        protected List<string> GetElementGroup(string elementGroup)
        {
            var eg = schemaElement.Elements(ns + "group")
                .FirstOrDefault(s =>
                {
                    var name = s.Attribute("name");
                    if (name != null)
                    {
                        return name.Value.Equals(elementGroup, StringComparison.OrdinalIgnoreCase);
                    }

                    return false;
                });

            if (eg == null)
            {
                return new List<string>();
            }

            return eg.Element(ns + "choice")
                .Elements()
                .Where(e => e.Name.LocalName == "element")
                .Select(e => e.Attribute("ref").Value)
                .ToList();
        }

        protected string GetAttributeGroupName(CustomXmlAttribute.eAttributeGroup attributeGroup)
        {
            switch (attributeGroup)
            {
                case CustomXmlAttribute.eAttributeGroup.Animation: return "animation";
                case CustomXmlAttribute.eAttributeGroup.Button: return "button";
                case CustomXmlAttribute.eAttributeGroup.Events: return "events";
                case CustomXmlAttribute.eAttributeGroup.Image: return "image";
                case CustomXmlAttribute.eAttributeGroup.LayoutBase: return "layoutBase";
                case CustomXmlAttribute.eAttributeGroup.LayoutElement: return "layoutElement";
                case CustomXmlAttribute.eAttributeGroup.RectPosition: return "rectPosition";
                case CustomXmlAttribute.eAttributeGroup.RectTransform: return "rectTransform";
                case CustomXmlAttribute.eAttributeGroup.AllElements: return "simpleAttributes";
                case CustomXmlAttribute.eAttributeGroup.Text: return "text";
            }

            return null;
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Debug.Log("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Debug.Log("ERROR: ");

            Debug.Log(args.Message);
        }
    }
}