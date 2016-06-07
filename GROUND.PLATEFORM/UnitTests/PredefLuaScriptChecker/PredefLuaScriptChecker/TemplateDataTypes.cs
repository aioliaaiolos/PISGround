// <copyright file="TemplateDataTypes.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
// </copyright>

namespace PIS.Ground.InstantMessage
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public enum TemplateCategory
    {
        Predefined,
        Scheduled,
        FreeText
    }

    public enum TemplateParameterType
    {
        StationId,
        CarNumber,
        Delay,
        DelayReasonCode,
        Text,
        Hour
    }

    public class Template
    {
        public Template()
        {
            this.DescriptionList = new List<TemplateDescription>();
            this.ParameterList = new List<TemplateParameterType>();
        }

        public string ID { get; set; }

        public TemplateCategory Category { get; set; }

        public string Class { get; set; }        

        [XmlArrayItem("Description")]
        public List<TemplateDescription> DescriptionList { get; set; }
        
        [XmlArrayItem("Parameter")]
        public List<TemplateParameterType> ParameterList { get; set; }
    }

    public class TemplateDescription
    {
        [XmlAttribute]
        public string Language { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}